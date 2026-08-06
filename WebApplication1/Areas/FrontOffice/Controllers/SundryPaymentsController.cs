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
    // (cash payment/receipt, cheque receipt, credit-batch cash pickup, account
    // closure payout). No dedicated app service exists for this — same as the
    // reference controller, it posts straight through IJournalAppService.
    //
    // Not reproduced: the reference controller's Create screen pulled full
    // AccountClosureRequest / CreditBatchEntry / ExternalCheque sub-objects from
    // the server to compute TotalValue and ChartOfAccountId for those three
    // transaction types. Here the client resolves those via the already-documented
    // account-closure/credit-batch/cheque endpoints and passes the resolved
    // chartOfAccountId + amount directly — consistent with a decoupled client
    // rather than a server-rendered form.
    [Authorize]
    [RoutePrefix("api/frontoffice/sundrypayments")]
    public class SundryPaymentsController : ApiController
    {
        private readonly IJournalAppService _journalAppService;
        private readonly ITellerAppService _tellerAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public SundryPaymentsController(
            IJournalAppService journalAppService,
            ITellerAppService tellerAppService,
            IBranchAppService branchAppService,
            IPostingPeriodAppService postingPeriodAppService)
        {
            _journalAppService = journalAppService ?? throw new ArgumentNullException(nameof(journalAppService));
            _tellerAppService = tellerAppService ?? throw new ArgumentNullException(nameof(tellerAppService));
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
            _postingPeriodAppService = postingPeriodAppService ?? throw new ArgumentNullException(nameof(postingPeriodAppService));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SundryPaymentRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required");

            if (request.ChartOfAccountId == Guid.Empty || request.TotalValue <= 0)
                return BadRequest("chartOfAccountId and a positive totalValue are required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var teller = GetCurrentTeller(serviceHeader);

                if (teller == null || teller.IsLocked)
                    return BadRequest("Current user has no linked, unlocked teller record.");

                var branch = _branchAppService.FindBranch(teller.EmployeeBranchId, serviceHeader);
                var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                var transactionType = (GeneralTransactionType)request.TransactionType;

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

                    default:
                        return BadRequest("Unsupported transaction type");
                }

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

                return Ok(new { success = true, message = "Operation success", data = journal });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
        // GeneralTransactionType: CashReceipt=1, ChequeReceipt=2, CashPayment=4, CashPickup=8.
        public int TransactionType { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public decimal TotalValue { get; set; }

        public string Reference { get; set; }

        public string PrimaryDescription { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }
}
