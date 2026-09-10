using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC SundryPaymentsController — a single-line
    // general GL voucher posted against the current teller's own cash account
    // (cash payment/receipt, cheque receipt, credit-batch cash pickup/sundry
    // payment, account closure payout). No dedicated app service exists for this — same as the
    // reference controller, it posts straight through IJournalAppService.
    //
    // Credit-batch payments are resolved again from the persisted entry here.
    // The request's amount/account are never trusted for those two transaction
    // types: Cash Pickup debits the batch Credit Type clearing account, while a
    // Sundry Payment debits the G/L account captured on the individual entry.
    [Authorize]
    [RoutePrefix("api/frontoffice/sundrypayments")]
    public class SundryPaymentsController : ApiController
    {
        private readonly IJournalAppService _journalAppService;
        private readonly ITellerAppService _tellerAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ICreditBatchAppService _creditBatchAppService;

        public SundryPaymentsController(
            IJournalAppService journalAppService,
            ITellerAppService tellerAppService,
            IBranchAppService branchAppService,
            IPostingPeriodAppService postingPeriodAppService,
            ICreditBatchAppService creditBatchAppService)
        {
            _journalAppService = journalAppService ?? throw new ArgumentNullException(nameof(journalAppService));
            _tellerAppService = tellerAppService ?? throw new ArgumentNullException(nameof(tellerAppService));
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
            _postingPeriodAppService = postingPeriodAppService ?? throw new ArgumentNullException(nameof(postingPeriodAppService));
            _creditBatchAppService = creditBatchAppService ?? throw new ArgumentNullException(nameof(creditBatchAppService));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SundryPaymentRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var teller = GetCurrentTeller(serviceHeader);

                if (teller == null || teller.IsLocked)
                    return BadRequest("Current user has no linked, unlocked teller record.");

                var branch = _branchAppService.FindBranch(teller.EmployeeBranchId, serviceHeader);
                var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                var transactionType = (GeneralTransactionType)request.TransactionType;

                CreditBatchEntryDTO creditBatchEntry = null;
                if (transactionType == GeneralTransactionType.CashPickup ||
                    transactionType == GeneralTransactionType.SundryPayment)
                {
                    if (request.CreditBatchEntryId == Guid.Empty)
                        return BadRequest("creditBatchEntryId is required for a credit-batch teller payment");

                    creditBatchEntry = _creditBatchAppService.FindCreditBatchEntry(request.CreditBatchEntryId, serviceHeader);
                    if (creditBatchEntry == null)
                        return BadRequest("The selected credit-batch entry no longer exists.");

                    var expectedBatchType = transactionType == GeneralTransactionType.CashPickup
                        ? CreditBatchType.CashPickup
                        : CreditBatchType.SundryPayments;

                    if (creditBatchEntry.CreditBatchType != (int)expectedBatchType)
                        return BadRequest("The selected entry does not belong to the requested credit-batch payment type.");

                    if (creditBatchEntry.CreditBatchStatus != (int)BatchStatus.Posted)
                        return BadRequest("The selected entry's batch has not been authorized for teller payment.");

                    if (creditBatchEntry.Status != (int)BatchEntryStatus.Pending)
                        return BadRequest("The selected entry has already been paid or is no longer pending.");

                    request.TotalValue = creditBatchEntry.Principal + creditBatchEntry.Interest;
                    request.ChartOfAccountId = transactionType == GeneralTransactionType.CashPickup
                        ? creditBatchEntry.CreditBatchCreditTypeChartOfAccountId
                        : creditBatchEntry.ChartOfAccountId ?? Guid.Empty;
                }

                if (request.ChartOfAccountId == Guid.Empty || request.TotalValue <= 0)
                    return BadRequest("A postable G/L account and a positive payment amount are required.");

                Guid creditChartOfAccountId, debitChartOfAccountId;
                int transactionCode;

                switch (transactionType)
                {
                    case GeneralTransactionType.CashReceipt:
                        transactionCode = (int)SystemTransactionCode.GeneralCashReceipt;
                        debitChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        creditChartOfAccountId = request.ChartOfAccountId;
                        break;

                    case GeneralTransactionType.ChequeReceipt:
                        transactionCode = (int)SystemTransactionCode.GeneralChequeReceipt;
                        debitChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        creditChartOfAccountId = request.ChartOfAccountId;
                        break;

                    case GeneralTransactionType.CashPayment:
                        transactionCode = (int)SystemTransactionCode.GeneralCashPayment;
                        debitChartOfAccountId = request.ChartOfAccountId;
                        creditChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        break;

                    case GeneralTransactionType.CashPickup:
                        transactionCode = (int)SystemTransactionCode.CreditBatchCashPickup;
                        debitChartOfAccountId = request.ChartOfAccountId;
                        creditChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        break;

                    case GeneralTransactionType.SundryPayment:
                        transactionCode = (int)SystemTransactionCode.CreditBatchSundryPayment;
                        debitChartOfAccountId = request.ChartOfAccountId;
                        creditChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        break;

                    case GeneralTransactionType.CashPaymentAccountClosure:
                        transactionCode = (int)SystemTransactionCode.AccountClosure;
                        debitChartOfAccountId = request.ChartOfAccountId;
                        creditChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty;
                        break;

                    default:
                        return BadRequest("Unsupported transaction type");
                }

                var increasesTellerBalance =
                    transactionType == GeneralTransactionType.CashReceipt ||
                    transactionType == GeneralTransactionType.ChequeReceipt;

                var tellerLimitError = _tellerAppService.ValidateCashMovement(
                    teller.Id,
                    request.TotalValue,
                    increasesTellerBalance,
                    serviceHeader);

                if (!string.IsNullOrWhiteSpace(tellerLimitError))
                    return BadRequest(tellerLimitError);

                var secondaryDescription = string.Format("B{0}/T{1}/#{2}", branch.Code, teller.Code, teller.ItemsCount);

                var journal = _journalAppService.AddNewJournal(
                    teller.EmployeeBranchId,
                    null,
                    request.TotalValue,
                    request.PrimaryDescription ?? "ok",
                    secondaryDescription,
                    request.Reference,
                    request.ModuleNavigationItemCode,
                    transactionCode,
                    DateTime.Today,
                    creditChartOfAccountId,
                    debitChartOfAccountId,
                    serviceHeader);

                if (journal == null)
                    return BadRequest("Failed to post the sundry payment");

                if (transactionType == GeneralTransactionType.CashPickup ||
                    transactionType == GeneralTransactionType.SundryPayment)
                {
                    // Flip the paid entry to Posted so it cannot be selected again.
                    if (!_creditBatchAppService.PostCreditBatchEntry(request.CreditBatchEntryId, request.ModuleNavigationItemCode, serviceHeader))
                        return BadRequest("The journal posted, but the credit-batch entry could not be marked as paid. Do not retry this payment; contact an administrator with the journal reference.");
                }

                return Ok(new { success = true, message = "Operation success", data = journal });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TellerDTO GetCurrentTeller(ServiceHeader serviceHeader)
        {
            var employeeIdClaim = (HttpContext.Current?.User as ClaimsPrincipal)?.FindFirst("EmployeeId");

            if (employeeIdClaim == null || !Guid.TryParse(employeeIdClaim.Value, out var employeeId))
                throw new InvalidOperationException("Current user has no linked employee/teller record.");

            return _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);
        }
    }

    public class SundryPaymentRequest
    {
        // GeneralTransactionType: CashReceipt=1, ChequeReceipt=2, CashPayment=4,
        // CashPickup=8, SundryPayment=16, CashPaymentAccountClosure=32.
        public int TransactionType { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public decimal TotalValue { get; set; }

        public string Reference { get; set; }

        public string PrimaryDescription { get; set; }

        public int ModuleNavigationItemCode { get; set; }

        // Required for CashPickup (8) and SundryPayment (16). The server reloads
        // this entry and derives the real amount and debit G/L account from it.
        public Guid CreditBatchEntryId { get; set; }
    }
}
