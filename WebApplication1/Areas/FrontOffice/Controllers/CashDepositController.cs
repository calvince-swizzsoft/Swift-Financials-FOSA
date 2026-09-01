using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/frontoffice/requests")]
    public class CashDepositController : ApiController
    {
        private readonly ICashDepositRequestAppService _cashDepositRequestAppService;

        private readonly ICashWithdrawalRequestAppService _cashWithdrawalRequestAppService;

        private readonly ICustomerAccountAppService _customerAccountAppService;

        private readonly ICustomerAppService _customerAppService;

        private readonly IBranchAppService _branchAppService;

        private readonly ITellerAppService _tellerAppService;

        private readonly IPostingPeriodAppService _postingPeriodAppService;

        private readonly IJournalAppService _journalAppService;

        private readonly IJournalEntryAppService _journalEntryAppService;

        private readonly IExternalChequeAppService _externalChequeAppService;

        private readonly ITextAlertAppService _textAlertAppService;

        private readonly ISavingsProductAppService _savingsProductAppService;

        private readonly IInvestmentProductAppService _investmentProductAppService;

        private readonly IAuthorizationAppService _authorizationAppService;

        private readonly IWorkflowAppService _workflowAppService;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly IChequeBookAppService _chequeBookAppService;

        //private readonly IWorkflowProcessorAppService _workflowProcessorAppService;


        public CashDepositController(ICashDepositRequestAppService cashDepositRequestAppService,
            ICustomerAccountAppService customerAccountAppService,
            ICustomerAppService customerAppService,
            IBranchAppService branchAppService,
            ITellerAppService tellerAppService,
            IPostingPeriodAppService postingPeriodAppService,
            IJournalAppService journalAppService,
            IJournalEntryAppService journalEntryAppService,
            ICashWithdrawalRequestAppService cashWithdrawalRequestAppService,
            IExternalChequeAppService externalChequeAppService,
            ITextAlertAppService textAlertAppService,
            ISavingsProductAppService savingsProductAppService,
            IInvestmentProductAppService investmentProductAppService,
            IAuthorizationAppService authorizationAppService,
            IWorkflowAppService workflowAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            IChequeBookAppService chequeBookAppService
            )
        {
            _cashDepositRequestAppService = cashDepositRequestAppService;
            _customerAccountAppService = customerAccountAppService;
            _customerAppService = customerAppService;
            _branchAppService = branchAppService;
            _tellerAppService = tellerAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _journalAppService = journalAppService;
            _journalEntryAppService = journalEntryAppService;
            _cashWithdrawalRequestAppService = cashWithdrawalRequestAppService;
            _externalChequeAppService = externalChequeAppService;
            _textAlertAppService = textAlertAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _authorizationAppService = authorizationAppService;
            _workflowAppService = workflowAppService;
            _chartOfAccountAppService = chartOfAccountAppService ?? throw new ArgumentNullException(nameof(chartOfAccountAppService));
            _chequeBookAppService = chequeBookAppService ?? throw new ArgumentNullException(nameof(chequeBookAppService));
        }

        [HttpGet]
        [Route("~/api/frontoffice/requests/queue", Name = "GetFrontOfficeTransactionRequestQueue")]
        public async Task<IHttpActionResult> GetQueue(int? type = null, int? status = null, string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                var rangeStart = startDate ?? DateTime.MinValue;
                var rangeEnd = endDate ?? DateTime.MaxValue;
                // Default to the pending queue — this endpoint's primary use is a checker/teller
                // work queue, not a full audit browse; pass status explicitly for other states.
                var statusFilter = status ?? (int)CashDepositRequestAuthStatus.Pending;

                if (type == (int)FrontOfficeTransactionType.CashDeposit)
                {
                    var cashDeposits = _cashDepositRequestAppService.FindCashDepositRequests(rangeStart, rangeEnd, statusFilter, text ?? "", 0, pageIndex, pageSize, serviceHeader);

                    foreach (var cdp in cashDeposits.PageCollection)
                    {
                        var customeracc = _customerAccountAppService.FindCustomerAccountDTO(cdp.CustomerAccountId, serviceHeader);

                        var customer = _customerAppService.FindCustomer(customeracc.CustomerId, serviceHeader);

                        cdp.CustomerName = customer.IndividualFirstName + " " + customer.IndividualLastName;
                    }

                    return Ok(new { success = true, message = "", data = cashDeposits });
                }

                if (type == (int)FrontOfficeTransactionType.CashWithdrawal)
                {
                    var cashWithdrawals = _cashWithdrawalRequestAppService.FindCashWithdrawalRequests(rangeStart, rangeEnd, statusFilter, text ?? "", 0, pageIndex, pageSize, serviceHeader);

                    foreach (var cwl in cashWithdrawals.PageCollection)
                    {
                        var customeracc = _customerAccountAppService.FindCustomerAccountDTO((Guid)cwl.CustomerAccountId, serviceHeader);

                        var customer = _customerAppService.FindCustomer(customeracc.CustomerId, serviceHeader);

                        cwl.CustomerName = customer.IndividualFirstName + " " + customer.IndividualLastName;
                    }

                    return Ok(new { success = true, message = "", data = cashWithdrawals });
                }

                if (type.HasValue)
                {
                    // ChequeDeposit posts directly (no request row) and CashWithdrawalPaymentVoucher
                    // is stored as TransactionType == CashWithdrawal (see the merge branch below) —
                    // neither has its own request queue to page through.
                    return Ok(new { success = true, message = "", data = new PageCollectionInfo<object> { PageCollection = new List<object>(), ItemsCount = 0 } });
                }

                // No type filter: merge the deposit and withdrawal request queues into one
                // date-sorted page instead of returning nothing. Each row keeps its own native
                // shape (CashDepositRequestDTO or CashWithdrawalRequestDTO) including
                // TransactionType, so the frontend can filter client-side by type if it wants
                // a single-type view instead of the combined one.
                var allDeposits = _cashDepositRequestAppService.FindCashDepositRequests(rangeStart, rangeEnd, statusFilter, text ?? "", 0, 0, int.MaxValue, serviceHeader);
                var allWithdrawals = _cashWithdrawalRequestAppService.FindCashWithdrawalRequests(rangeStart, rangeEnd, statusFilter, text ?? "", 0, 0, int.MaxValue, serviceHeader);

                foreach (var cdp in allDeposits.PageCollection)
                {
                    var customeracc = _customerAccountAppService.FindCustomerAccountDTO(cdp.CustomerAccountId, serviceHeader);
                    var customer = _customerAppService.FindCustomer(customeracc.CustomerId, serviceHeader);
                    cdp.CustomerName = customer.IndividualFirstName + " " + customer.IndividualLastName;
                }

                foreach (var cwl in allWithdrawals.PageCollection)
                {
                    var customeracc = _customerAccountAppService.FindCustomerAccountDTO((Guid)cwl.CustomerAccountId, serviceHeader);
                    var customer = _customerAppService.FindCustomer(customeracc.CustomerId, serviceHeader);
                    cwl.CustomerName = customer.IndividualFirstName + " " + customer.IndividualLastName;
                }

                var merged = allDeposits.PageCollection.Select(d => (sortDate: d.CreatedDate, item: (object)d))
                    .Concat(allWithdrawals.PageCollection.Select(w => (sortDate: w.CreatedDate, item: (object)w)))
                    .OrderByDescending(x => x.sortDate)
                    .Select(x => x.item)
                    .ToList();

                var mergedPage = merged.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                return Ok(new { success = true, message = "", data = new PageCollectionInfo<object> { PageCollection = mergedPage, ItemsCount = merged.Count } });
            }

            catch (Exception)
            {
                throw;
            }
        }

   


        [HttpGet]
        [Route("context")]
        public IHttpActionResult GetOperatorContext()
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var teller = GetCurrentTeller(serviceHeader);

            if (teller == null)
                return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = "Your user account is not linked to a teller profile.", data = (object)null });

            var branch = _branchAppService.FindBranch(teller.EmployeeBranchId, serviceHeader);
            if (branch == null)
                return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = "The teller's branch could not be resolved. Check the employee/teller branch linkage.", data = (object)null });

            return Ok(new
            {
                success = true,
                message = "Teller context resolved successfully.",
                data = new
                {
                    tellerId = teller.Id,
                    tellerDescription = teller.Description,
                    tellerCode = teller.Code,
                    chartOfAccountId = teller.ChartOfAccountId,
                    chartOfAccountName = teller.ChartOfAccountName,
                    branchId = branch.Id,
                    branchDescription = branch.Description,
                    isLocked = teller.IsLocked,
                    bookBalance = teller.BookBalance
                }
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CustomerTransactionModel transactionModel)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (transactionModel == null || transactionModel.CreditCustomerAccountId == Guid.Empty)
                return Json(new { success = false, message = "Please select a customer account.", data = (object)null });

            if (!Enum.IsDefined(typeof(FrontOfficeTransactionType), transactionModel.Type))
                return Json(new { success = false, message = "The selected transaction type is invalid.", data = (object)null });

            var SelectedTeller = GetCurrentTeller(serviceHeader);
            if (SelectedTeller == null)
                return Json(new { success = false, message = "Your user account is not linked to a teller profile.", data = (object)null });

            // Branch is an authenticated operator-context value. Never trust
            // a client-supplied BranchId for a teller transaction.
            transactionModel.BranchId = SelectedTeller.EmployeeBranchId;

            var SelectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(transactionModel.CreditCustomerAccountId, serviceHeader);

            if (SelectedCustomerAccount == null)
            {
                var response = new
                {
                    success = false,
                    message = "Please select a customer account",
                    data = (object)null
                };
                return Json(response);
            }

            var SelectedCustomer = _customerAppService.FindCustomer(SelectedCustomerAccount.CustomerId, serviceHeader);
            if (SelectedCustomer == null)
                return Json(new { success = false, message = "The customer linked to the selected account could not be found.", data = (object)null });

            if ((RecordStatus)SelectedCustomerAccount.RecordStatus != RecordStatus.Approved)
            {
                var response = new
                {
                    success = false,
                    message = "Sorry, account is not approved yet",
                    data = (object)null
                };

                return Json(response);
            }

            var SelectedBranch = _branchAppService.FindBranch(transactionModel.BranchId, serviceHeader);
            if (SelectedBranch == null)
                return Json(new { success = false, message = "The teller's branch could not be resolved. Check the employee/teller branch linkage.", data = (object)null });

            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
            if (postingPeriod == null)
                return Json(new { success = false, message = "No active posting period is configured for this transaction.", data = (object)null });
            transactionModel.PostingPeriodId = postingPeriod.Id;
            transactionModel.PrimaryDescription = "ok";
            transactionModel.SecondaryDescription = string.Format("B{0}/T{1}/#{2}", SelectedBranch.Code, SelectedTeller.Code, SelectedTeller.ItemsCount);

            // ChequeDeposit reads Reference back out as the cheque number
            // (NewExternalCheque.Number = transactionModel.Reference below) —
            // it must stay whatever the caller sent. Every other type's
            // Reference is derived from the customer's own reference.
            if ((FrontOfficeTransactionType)transactionModel.Type != FrontOfficeTransactionType.ChequeDeposit)
            {
                transactionModel.Reference = string.Format("{0}", SelectedCustomerAccount.CustomerReference1);
            }

            string productResolutionError;
            var targetProduct = ResolveTransactionProduct(SelectedCustomerAccount, transactionModel.BranchId, serviceHeader, out productResolutionError);
            if (targetProduct == null)
                return Json(new { success = false, message = productResolutionError, data = (object)null });
           

            transactionModel.Teller.Id = SelectedTeller.Id;
            transactionModel.BranchId = SelectedTeller.EmployeeBranchId;
    
            switch ((FrontOfficeTransactionType)transactionModel.Type)
            {
                case FrontOfficeTransactionType.CashDeposit:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashDeposit;

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.DebitChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
                        transactionModel.CreditChartOfAccountId = targetProduct.ChartOfAccountId;
                    }

                    break;

                case FrontOfficeTransactionType.ChequeDeposit:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.ChequeDeposit;


                    transactionModel.TransactionCode = (int)SystemTransactionCode.ChequeDeposit;

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.DebitChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;

                        // A cheque is not cash: unlike CashDeposit, the customer must not be
                        // credited to their real product GL yet — the funds aren't theirs to
                        // use until the cheque actually clears (ClearExternalCheque's Pay
                        // branch is what credits CustomerAccountTypeTargetProductChartOfAccountId,
                        // once). Until then this posts to the same ExternalChequesControl
                        // suspense account Pay/UnPay clearance already debits/credits — the
                        // customer link above keeps it visible on their CustomerAccountStatementType.ChequeDepositStatement
                        // mini-statement (see JournalEntryAppService) even though it's not yet
                        // their spendable balance.
                        var chequesControlChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesControl, serviceHeader);

                        if (chequesControlChartOfAccountId == Guid.Empty)
                        {
                            return Json(new
                            {
                                success = false,
                                message = "Sorry, but the external cheques control account has not been setup!",
                                data = (object)null
                            });
                        }

                        transactionModel.CreditChartOfAccountId = chequesControlChartOfAccountId;
                    }

                    break;

                case FrontOfficeTransactionType.CashWithdrawal:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashWithdrawal;

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
                        //transactionModel.DebitChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
                        transactionModel.DebitChartOfAccountId = targetProduct.ChartOfAccountId;
                    }

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

                    break;

                case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashWithdrawalPaymentVoucher;

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
                        // A withdrawal debits the customer's product control account and
                        // credits the teller cash account.  The payment-voucher path used
                        // to assign the product to CreditChartOfAccountId and immediately
                        // overwrite it with the teller below, leaving the debit G/L empty.
                        transactionModel.DebitChartOfAccountId = targetProduct.ChartOfAccountId;
                    }

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.CreditChartOfAccountId = (Guid)SelectedTeller.ChartOfAccountId;

                    break;
            }

            if ((FrontOfficeTransactionType)transactionModel.Type == FrontOfficeTransactionType.ChequeDeposit)
            {
                if (string.IsNullOrWhiteSpace(transactionModel.Reference) || !Regex.IsMatch(transactionModel.Reference, @"^\d{6}$"))
                    return Json(new { success = false, message = "Cheque number must contain exactly six digits.", data = (object)null });
                if (string.IsNullOrWhiteSpace(transactionModel.Drawer) || string.IsNullOrWhiteSpace(transactionModel.DrawerBank) || string.IsNullOrWhiteSpace(transactionModel.DrawerBankBranch))
                    return Json(new { success = false, message = "Cheque drawer, bank, and bank branch are required.", data = (object)null });
                if (!transactionModel.ChequeType.HasValue || transactionModel.ChequeType.Value == Guid.Empty)
                    return Json(new { success = false, message = "Cheque type is required so maturity can be calculated.", data = (object)null });
                if (transactionModel.WriteDate == default(DateTime) || transactionModel.WriteDate.Date > DateTime.Today)
                    return Json(new { success = false, message = "Enter a valid cheque write date that is not in the future.", data = (object)null });
            }

            if ((FrontOfficeTransactionType)transactionModel.Type == FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
            {
                if (transactionModel.PaymentVoucher == null || transactionModel.PaymentVoucher.Id == Guid.Empty || transactionModel.PaymentVoucher.ChequeBookId == Guid.Empty)
                    return Json(new { success = false, message = "Select an active payment voucher from the customer's cheque book.", data = (object)null });
                if (string.IsNullOrWhiteSpace(transactionModel.PaymentVoucher.Payee) || string.IsNullOrWhiteSpace(transactionModel.PaymentVoucher.Reference))
                    return Json(new { success = false, message = "Payment voucher payee and reference are required.", data = (object)null });
                if (!transactionModel.PaymentVoucher.WriteDate.HasValue || transactionModel.PaymentVoucher.WriteDate.Value.Date > DateTime.Today)
                    return Json(new { success = false, message = "Enter a valid payment voucher write date that is not in the future.", data = (object)null });

                var persistedVoucher = (_chequeBookAppService.FindPaymentVouchersByChequeBookId(transactionModel.PaymentVoucher.ChequeBookId, serviceHeader)
                    ?? new List<PaymentVoucherDTO>()).FirstOrDefault(x => x.Id == transactionModel.PaymentVoucher.Id);
                if (persistedVoucher == null || persistedVoucher.Status != (int)PaymentVoucherStatus.Active)
                    return Json(new { success = false, message = "The selected payment voucher does not exist, has already been paid, or has been flagged.", data = (object)null });
                if (persistedVoucher.ChequeBookCustomerAccountId != SelectedCustomerAccount.Id || !persistedVoucher.ChequeBookIsActive || persistedVoucher.ChequeBookIsLocked)
                    return Json(new { success = false, message = "The selected payment voucher is not from an active, unlocked cheque book for this customer account.", data = (object)null });

                transactionModel.PaymentVoucher.ChequeBookId = persistedVoucher.ChequeBookId;
                transactionModel.PaymentVoucher.Amount = transactionModel.TotalValue;
                transactionModel.PaymentVoucher.ValidateAll();
                if (transactionModel.PaymentVoucher.HasErrors)
                    return Json(new { success = false, message = string.Join("; ", transactionModel.PaymentVoucher.ErrorMessages), data = (object)null });
            }

            var authorityError = _journalAppService.ValidateTransactionAuthority(
                transactionModel.TotalValue,
                transactionModel.TransactionCode,
                serviceHeader);

            if (!string.IsNullOrWhiteSpace(authorityError))
                return Json(new { success = false, message = authorityError, data = (object)null });

            var increasesTellerBalance =
                (FrontOfficeTransactionType)transactionModel.Type == FrontOfficeTransactionType.CashDeposit ||
                (FrontOfficeTransactionType)transactionModel.Type == FrontOfficeTransactionType.ChequeDeposit;

            var tellerLimitError = _tellerAppService.ValidateCashMovement(
                SelectedTeller.Id,
                transactionModel.TotalValue,
                increasesTellerBalance,
                serviceHeader);

            if (!string.IsNullOrWhiteSpace(tellerLimitError))
                return Json(new { success = false, message = tellerLimitError, data = (object)null });

            transactionModel.ValidateAll();
            if (transactionModel.HasErrors)
            {
                var errorMessages = transactionModel.ErrorMessages
                    .Select(error => error)
                    .ToList();

                string combinedErrorMessage = string.Join("; ", errorMessages);
                //   ViewBag.TransactionTypeSelectList = GetFrontOfficeTransactionTypeSelectList(SelectedCustomerAccount.Type.ToString());


                var responseLast = new
                {
                    success = false,
                    message = $"Transaction Error: {combinedErrorMessage}",
                    data = (object)null
                };

                return Json(responseLast);
            }


            try
            {
                // Call the asynchronous method and check its result    
                var result = await ProcessCustomerTransactionAsync(transactionModel, SelectedTeller, SelectedCustomerAccount, SelectedCustomer, targetProduct);

                SelectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(transactionModel.CreditCustomerAccountId, serviceHeader);
        
                if (result.Success)
                {
                    var response = new
                    {
                        success = true,
                        message = "Operation Success",
                        data = result.TransactionJournal
                    };

                    return Json(response);
                }
                else if (!result.Success && result.Dialog)
                {
                    if (result.TransactionData.CreditCustomerAccountId != null && result.TransactionData.CreditCustomerAccountId != Guid.Empty)
                    {

                        var response = new
                        {
                            success = false,
                            message = result.Message,
                            data = new
                            {
                                isCashDepositRequest = true,
                                dialog = true,
                                selectedCustomerAccountId = result.TransactionData.CreditCustomerAccountId,
                                transactionTotalValue = result.TransactionData.TotalValue,
                                transactionReference = result.TransactionData.Reference,
                                cashTransactionRequestId = result.TransactionData.CashDepositRequestId,
                                transactionCategory = result.TransactionData.CashDepositCategory
                            }
                        };

                        return Json(response);

                    }

                    else if (result.TransactionData.DebitCustomerAccountId != null && result.TransactionData.DebitCustomerAccountId != Guid.Empty)
                    {
                        var response = new
                        {
                            success = false,
                            message = result.Message,
                            data = new
                            {
                                isCashWithdrawalRequest = true,
                                dialog = true,
                                selectedCustomerAccountId = result.TransactionData.DebitCustomerAccountId,
                                transactionTotalValue = result.TransactionData.TotalValue,
                                transactionReference = result.TransactionData.Reference,
                                cashTransactionRequestId = result.TransactionData.CashWithdrawalRequestId,
                                transactionCategory = result.TransactionData.CashWithdrawalCategory,
                                paymentVoucherId = result.TransactionData.PaymentVoucherId,
                                paymentVoucherPayee = result.TransactionData.PaymentVoucherPayee,
                                paymentVoucherChequeBookId = result.TransactionData.ChequeBookId,
                                paymentVoucherWriteDate = result.TransactionData.PaymentVoucherWriteDate
                            }
                        };

                        return Json(response);
                    }

                    // Default return for any path not covered by conditions
                    return Json(new
                    {
                        success = false,
                        message = "No valid transaction data found.",
                        data = new { dialog = false }
                    });
                }

                else
                {
                    var response = new
                    {

                        success = false,
                        message = result.Message,
                        data = (object)null
                    };

                    return Json(response);


                }
            }
            catch (InvalidOperationException exception)
            {
                // App-service validation and workflow-configuration failures are
                // expected business failures. Returning them through the normal
                // transaction envelope lets the teller correct the setup (for
                // example, a missing Cash Deposit Request Authorization role)
                // instead of having the global exception handler mask the reason
                // behind an HTTP 500 "An unexpected error occurred" response.
                return Json(new
                {
                    success = false,
                    message = exception.Message,
                    data = (object)null
                });
            }
            catch (Exception)
            {
                throw;
            }

        }


        // Checker approval for cash deposit/withdrawal requests goes through the generic
        // maker-checker engine now — POST /api/administration/workflows/items/approve —
        // not a front-office-specific endpoint. That route drives WorkflowProcessorAppService,
        // which calls AuthorizeCashDepositRequest/AuthorizeCashWithdrawalRequest itself with
        // correct multi-level approval counting and a WorkflowItem audit row, which this
        // endpoint never did. See WebApplication1/Areas/FrontOffice/WORKFLOW.md.

        [HttpPost]
        [Route("resend-approval")]
        public IHttpActionResult ResendApproval(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("A cash transaction request is required.");

            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
                var deposit = _cashDepositRequestAppService.FindCashDepositRequest(id, serviceHeader);
                var resent = deposit != null
                    ? _cashDepositRequestAppService.ResendCashDepositApprovalRequest(id, serviceHeader)
                    : _cashWithdrawalRequestAppService.ResendCashWithdrawalApprovalRequest(id, serviceHeader);

                if (!resent)
                    return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = "The approval request could not be resent.", data = (object)null });

                return Ok(new { success = true, message = "The transaction approval request was resent successfully.", data = (object)null });
            }
            catch (InvalidOperationException exception)
            {
                return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = exception.Message, data = (object)null });
            }
        }


        [HttpPost]
        [Route("post")]
        public async Task<IHttpActionResult> PostCashDepositRequest(Guid id)
        {

            Guid parseId;

            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                if (id == Guid.Empty || !Guid.TryParse(id.ToString(), out parseId))
                {
                    return BadRequest("Invalid Id");
                }



                var cashDepositRequestDTO = _cashDepositRequestAppService.FindCashDepositRequest(id, serviceHeader);

                if (cashDepositRequestDTO != null)
                {
                    if (cashDepositRequestDTO.Status == (int)CashDepositRequestAuthStatus.Authorized)
                    {
                        CustomerTransactionModel model = new CustomerTransactionModel();

                        var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(cashDepositRequestDTO.CustomerAccountId, serviceHeader);

                        var currentTellerDTO = GetCurrentTeller(serviceHeader);

                        if (customerAccount == null)
                            return BadRequest("The customer account linked to this authorized cash deposit could not be found.");

                        if (currentTellerDTO == null)
                            return BadRequest("Your user account is not linked to a teller profile.");

                        if (customerAccount != null && currentTellerDTO != null)
                        {
                            if (!currentTellerDTO.ChartOfAccountId.HasValue || currentTellerDTO.ChartOfAccountId.Value == Guid.Empty)
                                return BadRequest("The current teller does not have a cash G/L account configured.");

                            if (cashDepositRequestDTO.BranchId != currentTellerDTO.EmployeeBranchId)
                                return BadRequest("This authorized request belongs to a different branch and cannot be posted by the current teller.");

                            if ((RecordStatus)customerAccount.RecordStatus != RecordStatus.Approved)
                                return BadRequest("The customer account is no longer approved and cannot be posted.");

                            var tellerValidationError = _tellerAppService.ValidateCashMovement(currentTellerDTO.Id, cashDepositRequestDTO.Amount, true, serviceHeader);
                            if (!string.IsNullOrWhiteSpace(tellerValidationError))
                                return BadRequest(tellerValidationError);

                            model.TotalValue = cashDepositRequestDTO.Amount;
                            model.BranchId = currentTellerDTO.EmployeeBranchId;
                            model.CashDepositRequestId = cashDepositRequestDTO.Id;
                            model.DebitChartOfAccountId = currentTellerDTO.ChartOfAccountId.Value;


                            model.Type = (int)FrontOfficeTransactionType.CashDeposit;
                            model.TransactionCode = (int)SystemTransactionCode.CashDeposit;
                            model.CreditCustomerAccount = customerAccount;

                            model.DebitCustomerAccountId = customerAccount.Id;
                            model.DebitCustomerAccount = customerAccount;
                            model.CreditCustomerAccountId = customerAccount.Id;
                            model.CreditCustomerAccount = customerAccount;


                            model.ValueDate = DateTime.Today;


                            var SelectedCustomer = _customerAppService.FindCustomer(customerAccount.CustomerId, serviceHeader);
                            if (SelectedCustomer == null)
                                return BadRequest("The customer linked to this authorized request could not be found.");

                            var selectedBranch = _branchAppService.FindBranch(model.BranchId, serviceHeader);
                            if (selectedBranch == null)
                                return BadRequest("The teller's branch could not be resolved. Check the employee/teller branch linkage.");

                            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
                            if (postingPeriod == null)
                                return BadRequest("No active posting period is configured for this transaction.");

                            model.PostingPeriodId = postingPeriod.Id;
                            model.PrimaryDescription = "Authorized cash deposit";
                            model.SecondaryDescription = string.Format("B{0}/T{1}/#{2}", selectedBranch.Code, currentTellerDTO.Code, currentTellerDTO.ItemsCount);
                            model.Reference = customerAccount.CustomerReference1;

                            string productResolutionError;
                            var selectedProduct = ResolveTransactionProduct(customerAccount, model.BranchId, serviceHeader, out productResolutionError);

                            if (selectedProduct == null)
                                return BadRequest(productResolutionError);

                            if (selectedProduct.ChartOfAccountId == Guid.Empty)
                                return BadRequest("The selected account product does not have a control G/L account configured.");

                            model.CreditChartOfAccountId = selectedProduct.ChartOfAccountId;

                            var postResult = await ProcessCustomerTransactionAsync(model, currentTellerDTO, customerAccount, SelectedCustomer, selectedProduct);

                            if (!postResult.Success)
                                return BadRequest(postResult.Message);

                            return Ok(new { success = true, message = postResult.Message, data = postResult.TransactionJournal });
                        }


                        else
                        {

                            return BadRequest("Could not fetch a telleraccount, or/and customeraccount");
                        }
                    }
                }


                var cashWithdrawalRequestDTO = _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(id, serviceHeader);

                if (cashWithdrawalRequestDTO != null)
                {
                    if (cashWithdrawalRequestDTO.Status == (int)CashWithdrawalRequestAuthStatus.Authorized)
                    {
                        CustomerTransactionModel model = new CustomerTransactionModel();

                        var customerAccount = _customerAccountAppService.FindCustomerAccountDTO((Guid)cashWithdrawalRequestDTO.CustomerAccountId, serviceHeader);

                        var currentTellerDTO = GetCurrentTeller(serviceHeader);

                        if (customerAccount == null)
                            return BadRequest("The customer account linked to this authorized cash withdrawal could not be found.");

                        if (currentTellerDTO == null)
                            return BadRequest("Your user account is not linked to a teller profile.");

                        if (customerAccount != null && currentTellerDTO != null)
                        {
                            var isPaymentVoucherWithdrawal =
                                cashWithdrawalRequestDTO.TransactionType == (int)FrontOfficeTransactionType.CashWithdrawalPaymentVoucher
                                || cashWithdrawalRequestDTO.Category == (int)CashWithdrawalCategory.PaymentVoucher;
                            PaymentVoucherDTO paymentVoucher = null;

                            if (isPaymentVoucherWithdrawal)
                            {
                                if (cashWithdrawalRequestDTO.PaymentVoucherId == Guid.Empty)
                                    return BadRequest("The authorized withdrawal does not identify its payment voucher.");

                                paymentVoucher = _chequeBookAppService.FindPaymentVoucher(cashWithdrawalRequestDTO.PaymentVoucherId, serviceHeader);
                                if (paymentVoucher == null)
                                    return BadRequest("The payment voucher linked to this authorized withdrawal no longer exists.");
                                if (paymentVoucher.Status != (int)PaymentVoucherStatus.Active)
                                    return BadRequest("The payment voucher is no longer active. It may already be paid or flagged.");
                                if (paymentVoucher.ChequeBookCustomerAccountId != customerAccount.Id || !paymentVoucher.ChequeBookIsActive || paymentVoucher.ChequeBookIsLocked)
                                    return BadRequest("The payment voucher is not from an active, unlocked cheque book for this customer account.");
                                if (string.IsNullOrWhiteSpace(cashWithdrawalRequestDTO.PaymentVoucherPayee)
                                    || string.IsNullOrWhiteSpace(cashWithdrawalRequestDTO.PaymentVoucherReference)
                                    || !cashWithdrawalRequestDTO.PaymentVoucherWriteDate.HasValue)
                                    return BadRequest("The authorized payment-voucher request is missing its payee, reference, or write date. Re-initiate the request so it can be safely posted.");
                                if (cashWithdrawalRequestDTO.PaymentVoucherWriteDate.Value.Date > DateTime.Today)
                                    return BadRequest("The payment voucher write date cannot be in the future.");

                                paymentVoucher.Payee = cashWithdrawalRequestDTO.PaymentVoucherPayee.Trim();
                                paymentVoucher.Reference = cashWithdrawalRequestDTO.PaymentVoucherReference.Trim();
                                paymentVoucher.WriteDate = cashWithdrawalRequestDTO.PaymentVoucherWriteDate;
                                paymentVoucher.Amount = cashWithdrawalRequestDTO.Amount;
                                paymentVoucher.ValidateAll();
                                if (paymentVoucher.HasErrors)
                                    return BadRequest(string.Join("; ", paymentVoucher.ErrorMessages));
                            }

                            if (!currentTellerDTO.ChartOfAccountId.HasValue || currentTellerDTO.ChartOfAccountId.Value == Guid.Empty)
                                return BadRequest("The current teller does not have a cash G/L account configured.");

                            if (cashWithdrawalRequestDTO.BranchId != currentTellerDTO.EmployeeBranchId)
                                return BadRequest("This authorized request belongs to a different branch and cannot be posted by the current teller.");

                            if ((RecordStatus)customerAccount.RecordStatus != RecordStatus.Approved)
                                return BadRequest("The customer account is no longer approved and cannot be posted.");

                            var tellerValidationError = _tellerAppService.ValidateCashMovement(currentTellerDTO.Id, cashWithdrawalRequestDTO.Amount, false, serviceHeader);
                            if (!string.IsNullOrWhiteSpace(tellerValidationError))
                                return BadRequest(tellerValidationError);

                            model.TotalValue = cashWithdrawalRequestDTO.Amount;
                            model.BranchId = currentTellerDTO.EmployeeBranchId;
                            model.CashWithdrawalRequestId = cashWithdrawalRequestDTO.Id;
                            model.CreditChartOfAccountId = currentTellerDTO.ChartOfAccountId.Value;


                            model.Type = isPaymentVoucherWithdrawal ? (int)FrontOfficeTransactionType.CashWithdrawalPaymentVoucher : (int)FrontOfficeTransactionType.CashWithdrawal;
                            model.TransactionCode = isPaymentVoucherWithdrawal ? (int)SystemTransactionCode.CashWithdrawalPaymentVoucher : (int)SystemTransactionCode.CashWithdrawal;
                            model.PaymentVoucher = paymentVoucher;
                            model.CreditCustomerAccount = customerAccount;

                            model.DebitCustomerAccountId = customerAccount.Id;
                            model.DebitCustomerAccount = customerAccount;
                            model.CreditCustomerAccountId = customerAccount.Id;
                            model.CreditCustomerAccount = customerAccount;


                            model.ValueDate = DateTime.Today;


                            var SelectedCustomer = _customerAppService.FindCustomer(customerAccount.CustomerId, serviceHeader);
                            if (SelectedCustomer == null)
                                return BadRequest("The customer linked to this authorized request could not be found.");

                            var selectedBranch = _branchAppService.FindBranch(model.BranchId, serviceHeader);
                            if (selectedBranch == null)
                                return BadRequest("The teller's branch could not be resolved. Check the employee/teller branch linkage.");

                            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
                            if (postingPeriod == null)
                                return BadRequest("No active posting period is configured for this transaction.");

                            model.PostingPeriodId = postingPeriod.Id;
                            model.PrimaryDescription = isPaymentVoucherWithdrawal ? "Authorized payment voucher withdrawal" : "Authorized cash withdrawal";
                            model.SecondaryDescription = string.Format("B{0}/T{1}/#{2}", selectedBranch.Code, currentTellerDTO.Code, currentTellerDTO.ItemsCount);
                            model.Reference = isPaymentVoucherWithdrawal ? cashWithdrawalRequestDTO.PaymentVoucherReference : customerAccount.CustomerReference1;

                            string productResolutionError;
                            var selectedProduct = ResolveTransactionProduct(customerAccount, model.BranchId, serviceHeader, out productResolutionError);

                            if (selectedProduct == null)
                                return BadRequest(productResolutionError);

                            if (selectedProduct.ChartOfAccountId == Guid.Empty)
                                return BadRequest("The selected account product does not have a control G/L account configured.");

                            model.DebitChartOfAccountId = selectedProduct.ChartOfAccountId;

                            var postResult = await ProcessCustomerTransactionAsync(model, currentTellerDTO, customerAccount, SelectedCustomer, selectedProduct);

                            if (!postResult.Success)
                                return BadRequest(postResult.Message);

                            return Ok(new { success = true, message = postResult.Message, data = postResult.TransactionJournal });
                        }


                        else
                        {

                            return BadRequest("Could not fetch a telleraccount, or/and customeraccount");
                        }
                    }
                }


                    return BadRequest("The selected deposit is not authorized yet");
               
            }

            catch (InvalidOperationException exception)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = exception.Message,
                    data = (object)null
                });
            }
            catch (ArgumentException exception)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = exception.Message,
                    data = (object)null
                });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("markposted")]
        public async Task<IHttpActionResult> MarkAsPosted(Guid id)
        {
            Guid parseId;

            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
               
                if (id == Guid.Empty || !Guid.TryParse(id.ToString(), out parseId))
                {
                    return BadRequest("Invalid Id");
                }

                var cashDepositRequestDto = _cashDepositRequestAppService.FindCashDepositRequest(id, serviceHeader);

                if (cashDepositRequestDto != null) 
                {
                    if (cashDepositRequestDto.Status == (int)CashDepositRequestAuthStatus.Authorized)
                    {
                        CustomerTransactionModel model = new CustomerTransactionModel();

                        cashDepositRequestDto.Status = (int)CashDepositRequestAuthStatus.Posted;

                        _cashDepositRequestAppService.PostCashDepositRequest(cashDepositRequestDto, serviceHeader);

                        return Ok("the cash deposit was marked posted");
                    }
                }

                var cashWithdrawalRequestDto = _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(id, serviceHeader);

                if (cashWithdrawalRequestDto != null)
                {
                    if (cashWithdrawalRequestDto.Status == (int)CashWithdrawalRequestAuthStatus.Authorized)
                    {
                        if (cashWithdrawalRequestDto.PaymentVoucherId != Guid.Empty
                            || cashWithdrawalRequestDto.TransactionType == (int)FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
                            return BadRequest("Payment-voucher withdrawals must be posted through the transaction posting endpoint so the journal and voucher status are updated together.");

                        CustomerTransactionModel model = new CustomerTransactionModel();

                        cashWithdrawalRequestDto.Status = (int)CashWithdrawalRequestAuthStatus.Paid;

                        _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(cashWithdrawalRequestDto, null, serviceHeader);

                        return Ok("the cash withdrawal was marked posted");
                    }
                }
                
               
                
                    return BadRequest("The selected request is not authorized yet");
                
            }

            catch (InvalidOperationException exception)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = exception.Message,
                    data = (object)null
                });
            }
            catch (ArgumentException exception)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = exception.Message,
                    data = (object)null
                });
            }
            catch (Exception)
            {
                throw;
            }
        }


      

        private TransactionProduct ResolveTransactionProduct(CustomerAccountDTO account, Guid branchId, ServiceHeader serviceHeader, out string error)
        {
            error = null;
            if (account == null)
            {
                error = "Please select a customer account.";
                return null;
            }

            switch ((ProductCode)account.CustomerAccountTypeProductCode)
            {
                case ProductCode.Savings:
                    var savings = _savingsProductAppService.FindSavingsProduct(account.CustomerAccountTypeTargetProductId, branchId, serviceHeader);
                    if (savings == null) { error = "The savings product for the selected account is not configured for the teller's branch."; return null; }
                    if (savings.IsLocked) { error = "The savings product for the selected account is locked."; return null; }
                    return new TransactionProduct
                    {
                        ProductCode = ProductCode.Savings,
                        ChartOfAccountId = savings.ChartOfAccountId,
                        MinimumBalance = savings.MinimumBalance,
                        MaximumAllowedDeposit = savings.MaximumAllowedDeposit,
                        MaximumAllowedWithdrawal = savings.MaximumAllowedWithdrawal,
                        WithdrawalNoticePeriod = savings.WithdrawalNoticePeriod,
                        IsRefundable = true
                    };

                case ProductCode.Investment:
                    var investment = _investmentProductAppService.FindInvestmentProduct(account.CustomerAccountTypeTargetProductId, serviceHeader);
                    if (investment == null) { error = "The investment product for the selected account could not be found."; return null; }
                    if (investment.IsLocked) { error = "The investment product for the selected account is locked."; return null; }
                    return new TransactionProduct
                    {
                        ProductCode = ProductCode.Investment,
                        ChartOfAccountId = investment.ChartOfAccountId,
                        MinimumBalance = investment.MinimumBalance,
                        MaximumBalance = investment.MaximumBalance,
                        IsRefundable = investment.IsRefundable
                    };

                default:
                    error = "Only savings and investment accounts support teller deposits or withdrawals.";
                    return null;
            }
        }

        private sealed class TransactionProduct
        {
            public ProductCode ProductCode { get; set; }
            public Guid ChartOfAccountId { get; set; }
            public decimal MinimumBalance { get; set; }
            public decimal MaximumBalance { get; set; }
            public decimal MaximumAllowedDeposit { get; set; }
            public decimal MaximumAllowedWithdrawal { get; set; }
            public int WithdrawalNoticePeriod { get; set; }
            public bool IsRefundable { get; set; }
        }

        private async Task<OperationResult> ProcessCustomerTransactionAsync(CustomerTransactionModel transactionModel, TellerDTO selectedTellerDTO, CustomerAccountDTO customerAccountDTO, CustomerDTO selectedCustomer, TransactionProduct targetProduct)
        {
            bool IsBusy = false;
            var SelectedCustomerAccount = customerAccountDTO;
            var SelectedTeller = selectedTellerDTO;

            var SelectedCustomer = selectedCustomer;
            System.Globalization.NumberFormatInfo _nfi = new CultureInfo("en-US", false).NumberFormat;
            var time = System.DateTime.Now.ToString("dd/mm/yyyy");

            var serviceHeader = Utils.CreateServiceHeader();

            try
            {

                int frontOfficeTransactionType = transactionModel.Type;
                // Product charges are part of balance classification and the
                // resulting journal. They must be computed before deciding
                // whether a withdrawal is within limits, below minimum, or
                // an overdraw.
                var tariffs = _tellerAppService.ComputeCashTariffs(SelectedCustomerAccount, transactionModel.TotalValue, frontOfficeTransactionType, serviceHeader)
                    ?? new List<TariffWrapper>();
         
                switch ((FrontOfficeTransactionType)frontOfficeTransactionType)
                {
                    case FrontOfficeTransactionType.CashDeposit:
                        if (targetProduct.ProductCode == ProductCode.Investment
                            && targetProduct.MaximumBalance > 0m
                            && SelectedCustomerAccount.AvailableBalance + transactionModel.TotalValue > targetProduct.MaximumBalance)
                        {
                            return new OperationResult
                            {
                                Success = false,
                                Dialog = false,
                                Message = $"This deposit would exceed the investment product maximum balance of {targetProduct.MaximumBalance:N2}."
                            };
                        }

                        var cashDepositCategory = CashDepositCategory.WithinLimits;

                        if (targetProduct.ProductCode == ProductCode.Savings && transactionModel.TotalValue > targetProduct.MaximumAllowedDeposit)
                        {
                            cashDepositCategory = CashDepositCategory.AboveMaximumAllowed;
                        }

                        switch (cashDepositCategory)
                        {
                            case CashDepositCategory.WithinLimits:
                                var withinLimitsCashDepositJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);
                                
                                //transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
                                var updateWithinLimitResult = _customerAccountAppService.UpdateCustomerAccount(SelectedCustomerAccount, serviceHeader);


                                if (updateWithinLimitResult)
                                {
                                    if (transactionModel.CashDepositRequestId != Guid.Empty)
                                    {
                                        var originatingCashDepositRequest = _cashDepositRequestAppService.FindCashDepositRequest(transactionModel.CashDepositRequestId, serviceHeader);

                                        if (originatingCashDepositRequest != null)
                                            _cashDepositRequestAppService.PostCashDepositRequest(originatingCashDepositRequest, serviceHeader);
                                    }

                                    string message = $"Operation success: Customer's new balance is {SelectedCustomerAccount.NewAvailableBalance}";

                                    string cashDepositTextTemplate = "Dear customer, your account has been credited with a cash deposit of KES {0} at {1} Branch {2}.";
                                    await SendTextNotificationAsync(cashDepositTextTemplate, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription, _textAlertAppService);

                                    return new OperationResult
                                    {
                                        Success = true,
                                        Dialog = false,
                                        Message = message,
                                        TransactionJournal = new JournalDTO
                                        {

                                            Id = withinLimitsCashDepositJournal.Id,
                                            TransactionCode = withinLimitsCashDepositJournal.TransactionCode,
                                            SequentialId = withinLimitsCashDepositJournal.SequentialId,
                                            BranchDescription = withinLimitsCashDepositJournal.BranchDescription,
                                            PrimaryDescription = withinLimitsCashDepositJournal.PrimaryDescription,
                                            SecondaryDescription = withinLimitsCashDepositJournal.SecondaryDescription,
                                            PostingPeriodDescription = withinLimitsCashDepositJournal.PostingPeriodDescription,
                                            ApplicationUserName = withinLimitsCashDepositJournal.ApplicationUserName,
                                            CreatedDate = withinLimitsCashDepositJournal.CreatedDate,
                                            TotalValue = withinLimitsCashDepositJournal.TotalValue,
                                            Reference = withinLimitsCashDepositJournal.Reference
                                        }

                                    };

                                }

                                else
                                {
                                    return new OperationResult
                                    {

                                        Success = false,
                                        Dialog = false,
                                        Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

                                    };
                                }

                            case CashDepositCategory.AboveMaximumAllowed:
                                var createNewCashDepositRequest = default(bool);

                                var actionableCashDepositRequests = _cashDepositRequestAppService.FindActionableCashDepositRequestsByCustomerAccount(SelectedCustomerAccount, serviceHeader);

                                if (actionableCashDepositRequests != null && actionableCashDepositRequests.Any())
                                {
                                    var targetCashDepositRequest = actionableCashDepositRequests.Where(x => x.Id == transactionModel.CashDepositRequestId).FirstOrDefault();

                                    if (targetCashDepositRequest != null)
                                    {
                                        // Check if another operation is already in progress
                                        if (IsBusy)
                                        {
                                            return new OperationResult
                                            {

                                                Success = false,
                                                Dialog = false,
                                                Message = "Please wait until the current operation is complete.",

                                            };
                                        }

                                        // Set IsBusy to true to indicate an ongoing operation
                                        IsBusy = true;

                                        if (targetCashDepositRequest.Status == (int)CashDepositRequestAuthStatus.Authorized)
                                        {

                                            var authorizedCashDepositJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);


                                            //transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
                                            var updateAuhorizedResult = _customerAccountAppService.UpdateCustomerAccount(SelectedCustomerAccount, serviceHeader);


                                            if (updateAuhorizedResult)
                                            {

                                                _cashDepositRequestAppService.PostCashDepositRequest(targetCashDepositRequest, serviceHeader);


                                                string message = $"Operation success: Customer's new balance is {SelectedCustomerAccount.NewAvailableBalance}";

                                                string cashDepositTextTemplate = "Dear customer, your account has been credited with a cash deposit of KES {0} at {1} Branch {2}.";
                                                await SendTextNotificationAsync(cashDepositTextTemplate, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription, _textAlertAppService);

                                                return new OperationResult
                                                {
                                                    Success = true,
                                                    Dialog = false,
                                                    Message = message,
                                                    TransactionJournal = new JournalDTO
                                                    {

                                                        Id = authorizedCashDepositJournal.Id,
                                                        TransactionCode = authorizedCashDepositJournal.TransactionCode,
                                                        SequentialId = authorizedCashDepositJournal.SequentialId,
                                                        BranchDescription = authorizedCashDepositJournal.BranchDescription,
                                                        PrimaryDescription = authorizedCashDepositJournal.PrimaryDescription,
                                                        SecondaryDescription = authorizedCashDepositJournal.SecondaryDescription,
                                                        PostingPeriodDescription = authorizedCashDepositJournal.PostingPeriodDescription,
                                                        ApplicationUserName = authorizedCashDepositJournal.ApplicationUserName,
                                                        CreatedDate = authorizedCashDepositJournal.CreatedDate,
                                                        TotalValue = authorizedCashDepositJournal.TotalValue,
                                                        Reference = authorizedCashDepositJournal.Reference
                                                    }

                                                };

                                            }

                                            else
                                            {
                                                return new OperationResult
                                                {

                                                    Success = false,
                                                    Dialog = false,
                                                    Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

                                                };
                                            }
                                        }

                                     
                                    }
                                    else createNewCashDepositRequest = true;
                                }
                                else createNewCashDepositRequest = true;

                                if (createNewCashDepositRequest)
                                {
                                    CashDepositRequestDTO aboveMaxCashDepositRequestDTO = new CashDepositRequestDTO();



                                    aboveMaxCashDepositRequestDTO.Amount = transactionModel.TotalValue;
                                    aboveMaxCashDepositRequestDTO.BranchId = transactionModel.BranchId;
                                    aboveMaxCashDepositRequestDTO.CustomerAccountId = SelectedCustomerAccount.Id;
                                    aboveMaxCashDepositRequestDTO.CustomerName = SelectedCustomer.FullName;

                                    aboveMaxCashDepositRequestDTO.Status = (int)CashDepositRequestAuthStatus.Pending;

                                    aboveMaxCashDepositRequestDTO.Posted = false;
                                    aboveMaxCashDepositRequestDTO.TransactionType = transactionModel.Type;

                                    aboveMaxCashDepositRequestDTO.Remarks =
                                        "Cash deposit above the savings product maximum";

                                    aboveMaxCashDepositRequestDTO.ValidateAll();
                                    if (aboveMaxCashDepositRequestDTO.HasErrors)
                                    {
                                        return new OperationResult
                                        {
                                            Success = false,
                                            Dialog = false,
                                            Message = string.Join("; ", aboveMaxCashDepositRequestDTO.ErrorMessages)
                                        };
                                    }

                                    var cashDepositRequestDTO = _cashDepositRequestAppService.AddNewCashDepositRequestWithWorkflow(aboveMaxCashDepositRequestDTO, serviceHeader);


                                    string message = string.Format(
                                        "{0}.\nNew cash deposit authorization request placed",
                                        EnumHelper.GetDescription(cashDepositCategory)
                                    );


                                    // Success must be false here — no journal was posted, only a Pending
                                    // CashDepositRequest + Workflow item was created. Create()'s dispatch
                                    // checks `if (result.Success)` first and returns TransactionJournal
                                    // (null in this branch) with a hardcoded "Operation Success" message —
                                    // Success=true here previously made that always fire instead of the
                                    // documented "Authorization required" dialog response
                                    // (docs/api/frontoffice-api-spec.md §4.2), silently telling the caller
                                    // a deposit succeeded when it hadn't.
                                    return new OperationResult
                                    {
                                        Success = false,
                                        Dialog = true,
                                        Message = message,
                                        TransactionData = new CustomerTransactionModel
                                        {
                                            CreditCustomerAccountId = SelectedCustomerAccount.Id,
                                            TotalValue = transactionModel.TotalValue,
                                            Reference = transactionModel.Reference,
                                            CashDepositRequestId = cashDepositRequestDTO.Id,
                                            CashDepositCategory = (int)cashDepositCategory
                                        }
                                    };

                                }

                                break;

                            // Handle other categories if needed
                            default:
                                break;
                        }

                        break;

                    // Handle other transaction types if needed

                    case FrontOfficeTransactionType.CashWithdrawal:
                    case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:

                        if (targetProduct.ProductCode == ProductCode.Investment && !targetProduct.IsRefundable)
                        {
                            return new OperationResult
                            {
                                Success = false,
                                Dialog = false,
                                Message = "This investment product is not refundable and cannot be withdrawn through the teller."
                            };
                        }

                        if (selectedTellerDTO.BookBalance < transactionModel.TotalValue)
                        {

                            return new OperationResult
                            {

                                Success = false,
                                Message = "Sorry, but your teller G/L account has insufficient cash!"
                            };


                        }

                        else
                        {
                            var cashWithdrawalCategory = CashWithdrawalCategory.WithinLimits;

                            if ((FrontOfficeTransactionType)frontOfficeTransactionType == FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
                            {
                                cashWithdrawalCategory = CashWithdrawalCategory.PaymentVoucher;
                            }
                            else if (targetProduct.ProductCode == ProductCode.Savings && transactionModel.TotalValue > targetProduct.MaximumAllowedWithdrawal)
                            {
                                cashWithdrawalCategory = CashWithdrawalCategory.AboveMaximumAllowed;
                            }
                            else if (((transactionModel.TotalValue + tariffs.Where(x => x.ChargeBenefactor == (int)ChargeBenefactor.Customer).Sum(x => x.Amount)) > SelectedCustomerAccount.AvailableBalance) && ((transactionModel.TotalValue + tariffs.Sum(x => x.Amount)) <= (SelectedCustomerAccount.AvailableBalance + targetProduct.MinimumBalance)))
                            {
                                cashWithdrawalCategory = CashWithdrawalCategory.BelowMinimumBalance;
                            }

                            //TODO: maybe u want to Check for OverDraw earlier
                            else if ((transactionModel.TotalValue + tariffs.Where(x => x.ChargeBenefactor == (int)ChargeBenefactor.Customer).Sum(x => x.Amount)) > (SelectedCustomerAccount.AvailableBalance + targetProduct.MinimumBalance))
                            {
                                cashWithdrawalCategory = CashWithdrawalCategory.Overdraw;
                            }

                            switch (cashWithdrawalCategory)
                            {
                                case CashWithdrawalCategory.AboveMaximumAllowed:
                                case CashWithdrawalCategory.BelowMinimumBalance:
                                case CashWithdrawalCategory.PaymentVoucher:

                                    var createNewCashWithdrawalRequest = default(bool);

                                    var actionableCashWithdrawalRequests = _cashWithdrawalRequestAppService.FindMatureCashWithdrawalRequestsByCustomerAccountId(SelectedCustomerAccount, serviceHeader);

                                    if (actionableCashWithdrawalRequests != null && actionableCashWithdrawalRequests.Any())
                                    {
                                        // Scoped to the specific request this call is posting against — same
                                        // pattern as the deposit path above (targetCashDepositRequest via
                                        // transactionModel.CashDepositRequestId). Previously this iterated
                                        // every actionable request for the customer account and acted on the
                                        // first Authorized one found, regardless of whether it was the one
                                        // being posted — a customer with two pending withdrawal requests could
                                        // have an unrelated one silently marked Paid.
                                        var targetCashWithdrawalRequest = actionableCashWithdrawalRequests.Where(x => x.Id == transactionModel.CashWithdrawalRequestId).FirstOrDefault();

                                        if (targetCashWithdrawalRequest != null)
                                        {
                                            IsBusy = true;

                                            if (targetCashWithdrawalRequest.Status == (int)CashDepositRequestAuthStatus.Authorized)
                                            {

                                                var authorizedCashWithdrawalJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);
                                                transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
                                                var updateAuhorizedResult = _customerAccountAppService.UpdateCustomerAccount(SelectedCustomerAccount, serviceHeader);


                                                if (updateAuhorizedResult)
                                                {

                                                    _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(targetCashWithdrawalRequest, transactionModel.PaymentVoucher, serviceHeader);

                                                    string message = $"Operation success: Customer's new balance is {SelectedCustomerAccount.NewAvailableBalance}";

                                                    string cashDepositTextTemplate = "Dear customer, your account has been credited with a cash deposit of KES {0} at {1} Branch {2}.";
                                                    await SendTextNotificationAsync(cashDepositTextTemplate, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription, _textAlertAppService);

                                                    return new OperationResult
                                                    {
                                                        Success = true,
                                                        Dialog = false,
                                                        Message = message,
                                                        TransactionJournal = new JournalDTO
                                                        {

                                                            Id = authorizedCashWithdrawalJournal.Id,
                                                            TransactionCode = authorizedCashWithdrawalJournal.TransactionCode,
                                                            SequentialId = authorizedCashWithdrawalJournal.SequentialId,
                                                            BranchDescription = authorizedCashWithdrawalJournal.BranchDescription,
                                                            PrimaryDescription = authorizedCashWithdrawalJournal.PrimaryDescription,
                                                            SecondaryDescription = authorizedCashWithdrawalJournal.SecondaryDescription,
                                                            PostingPeriodDescription = authorizedCashWithdrawalJournal.PostingPeriodDescription,
                                                            ApplicationUserName = authorizedCashWithdrawalJournal.ApplicationUserName,
                                                            CreatedDate = authorizedCashWithdrawalJournal.CreatedDate,
                                                            TotalValue = authorizedCashWithdrawalJournal.TotalValue,
                                                            Reference = authorizedCashWithdrawalJournal.Reference
                                                        }

                                                    };

                                                }

                                                else
                                                {
                                                    return new OperationResult
                                                    {

                                                        Success = false,
                                                        Dialog = false,
                                                        Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

                                                    };
                                                }


                                            }
                                        }
                                        else createNewCashWithdrawalRequest = true;
                                    }
                                    else createNewCashWithdrawalRequest = true;

                                    if (createNewCashWithdrawalRequest)
                                    {
                                        var isPaymentVoucherWithdrawal = cashWithdrawalCategory == CashWithdrawalCategory.PaymentVoucher;
                                        CashWithdrawalRequestDTO aboveLimitsCashWithdrawalRequest = new CashWithdrawalRequestDTO();

                                        aboveLimitsCashWithdrawalRequest.Amount = transactionModel.TotalValue;
                                        aboveLimitsCashWithdrawalRequest.BranchId = transactionModel.BranchId;
                                        aboveLimitsCashWithdrawalRequest.CustomerName = SelectedCustomer.FullName;
                                        aboveLimitsCashWithdrawalRequest.CustomerAccountId = SelectedCustomerAccount.Id;
                                        aboveLimitsCashWithdrawalRequest.Category = (int)cashWithdrawalCategory;
                                        aboveLimitsCashWithdrawalRequest.Type = targetProduct.WithdrawalNoticePeriod > 0
                                            ? (int)CashWithdrawalRequestType.FutureNotice
                                            : (int)CashWithdrawalRequestType.ImmediateNotice;
                                        aboveLimitsCashWithdrawalRequest.CustomerAccountBranchId = SelectedCustomerAccount.BranchId;
                                        aboveLimitsCashWithdrawalRequest.CustomerAccountCustomerAccountTypeTargetProductId = SelectedCustomerAccount.CustomerAccountTypeTargetProductId;
                                        aboveLimitsCashWithdrawalRequest.Remarks = string.IsNullOrWhiteSpace(transactionModel.Reference)
                                            ? string.Format("Cash withdrawal initiated by teller {0}.", selectedTellerDTO.Description)
                                            : transactionModel.Reference.Trim();
                                        aboveLimitsCashWithdrawalRequest.TransactionType = frontOfficeTransactionType;
                                        aboveLimitsCashWithdrawalRequest.Status = (int)CashWithdrawalRequestAuthStatus.Pending;
                                        if (isPaymentVoucherWithdrawal)
                                        {
                                            aboveLimitsCashWithdrawalRequest.PaymentVoucherId = transactionModel.PaymentVoucher.Id;
                                            aboveLimitsCashWithdrawalRequest.PaymentVoucherPayee = transactionModel.PaymentVoucher.Payee;
                                            aboveLimitsCashWithdrawalRequest.PaymentVoucherReference = transactionModel.PaymentVoucher.Reference;
                                            aboveLimitsCashWithdrawalRequest.PaymentVoucherWriteDate = transactionModel.PaymentVoucher.WriteDate;
                                        }

                                        aboveLimitsCashWithdrawalRequest.ValidateAll();
                                        if (aboveLimitsCashWithdrawalRequest.HasErrors)
                                        {
                                            return new OperationResult
                                            {
                                                Success = false,
                                                Dialog = false,
                                                Message = string.Join("; ", aboveLimitsCashWithdrawalRequest.ErrorMessages)
                                            };
                                        }

                                        var cashWithdrawalRequestDTO = _cashWithdrawalRequestAppService.AddNewCashWithdrawalRequestWithWorkflow(aboveLimitsCashWithdrawalRequest, serviceHeader);
                                        if (cashWithdrawalRequestDTO == null || cashWithdrawalRequestDTO.Id == Guid.Empty)
                                        {
                                            return new OperationResult
                                            {
                                                Success = false,
                                                Dialog = false,
                                                Message = "The cash withdrawal authorization request could not be created."
                                            };
                                        }

                                        string message = string.Format("{0}.\nSuccessfully placed cash withdrawal authorization request", EnumHelper.GetDescription(cashWithdrawalCategory));


                                        // Success must be false / Dialog true here — no journal was posted,
                                        // only a Pending CashWithdrawalRequest + Workflow item. Same bug and
                                        // same fix as the CashDeposit "AboveMaximumAllowed" branch above:
                                        // Create()'s dispatch checks `if (result.Success)` first, so
                                        // Success=true/Dialog=false here previously returned a hardcoded
                                        // "Operation Success" with a null TransactionJournal instead of the
                                        // documented "Authorization required" dialog response.
                                        return new OperationResult
                                        {
                                            Success = false,
                                            Dialog = true,
                                            Message = message,
                                            TransactionData = new CustomerTransactionModel
                                            {
                                                DebitCustomerAccountId = SelectedCustomerAccount.Id,
                                                TotalValue = transactionModel.TotalValue,
                                                Reference = isPaymentVoucherWithdrawal ? transactionModel.PaymentVoucher.Reference : transactionModel.Reference,
                                                PaymentVoucherId = isPaymentVoucherWithdrawal ? transactionModel.PaymentVoucher.Id : Guid.Empty,
                                                PaymentVoucherPayee = isPaymentVoucherWithdrawal ? transactionModel.PaymentVoucher.Payee : null,
                                                CashWithdrawalCategory = (int)cashWithdrawalCategory,
                                                PaymentVoucherWriteDate = isPaymentVoucherWithdrawal ? transactionModel.PaymentVoucher.WriteDate : null,
                                                CashWithdrawalRequestId = cashWithdrawalRequestDTO.Id
                                            }
                                        }; 
                                    }
                                    break;

                                case CashWithdrawalCategory.WithinLimits:

                                    var withinLimitsJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);

                                    transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
                               
                                    var updateWithinLimitResult = _customerAccountAppService.UpdateCustomerAccount(SelectedCustomerAccount, serviceHeader);

                          
                                    if (updateWithinLimitResult)
                                    {
                                        if (transactionModel.CashWithdrawalRequestId != Guid.Empty)
                                        {
                                            var originatingCashWithdrawalRequest = _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(transactionModel.CashWithdrawalRequestId, serviceHeader);

                                            if (originatingCashWithdrawalRequest != null)
                                                _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(originatingCashWithdrawalRequest, null, serviceHeader);
                                        }
                                        else
                                        {
                                            CashWithdrawalRequestDTO withinLimitsCashWithdrawalRequest = new CashWithdrawalRequestDTO();

                                            withinLimitsCashWithdrawalRequest.Amount = transactionModel.TotalValue;
                                            withinLimitsCashWithdrawalRequest.BranchId = transactionModel.BranchId;
                                            withinLimitsCashWithdrawalRequest.AuthorizedDate = DateTime.Today;
                                            withinLimitsCashWithdrawalRequest.Status = (int)CashWithdrawalRequestAuthStatus.Paid;
                                            withinLimitsCashWithdrawalRequest.AuthorizedBy = SelectedTeller.Description;
                                            withinLimitsCashWithdrawalRequest.CustomerAccountId = SelectedCustomerAccount.Id;
                                            withinLimitsCashWithdrawalRequest.Category = (int)CashWithdrawalCategory.WithinLimits;
                                            withinLimitsCashWithdrawalRequest.Type = targetProduct.WithdrawalNoticePeriod > 0
                                                ? (int)CashWithdrawalRequestType.FutureNotice
                                                : (int)CashWithdrawalRequestType.ImmediateNotice;
                                            withinLimitsCashWithdrawalRequest.CustomerAccountBranchId = SelectedCustomerAccount.BranchId;
                                            withinLimitsCashWithdrawalRequest.CustomerAccountCustomerAccountTypeTargetProductId = SelectedCustomerAccount.CustomerAccountTypeTargetProductId;
                                            withinLimitsCashWithdrawalRequest.CustomerName = SelectedCustomer.FullName;
                                            withinLimitsCashWithdrawalRequest.TransactionType = (int)FrontOfficeTransactionType.CashWithdrawal;
                                            withinLimitsCashWithdrawalRequest.Remarks = string.IsNullOrWhiteSpace(transactionModel.Reference)
                                                ? string.Format("Cash withdrawal posted by teller {0}.", SelectedTeller.Description)
                                                : transactionModel.Reference.Trim();

                                            withinLimitsCashWithdrawalRequest.ValidateAll();
                                            if (withinLimitsCashWithdrawalRequest.HasErrors)
                                            {
                                                return new OperationResult
                                                {
                                                    Success = false,
                                                    Dialog = false,
                                                    Message = string.Join("; ", withinLimitsCashWithdrawalRequest.ErrorMessages)
                                                };
                                            }

                                            _cashWithdrawalRequestAppService.RecordPaidCashWithdrawal(withinLimitsCashWithdrawalRequest, serviceHeader);
                                        }

                                        string message = $"Operation success: Customer's new balance is {transactionModel.CustomerAccount.NewAvailableBalance}";

                                        string cashWithdrawalTextTemplate1 = "Dear customer, your account has been debited with KES {0} at {1} Branch {2}.";
                                        await SendTextNotificationAsync(cashWithdrawalTextTemplate1, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription, _textAlertAppService);

                                        return new OperationResult
                                        {
                                            Success = true,
                                            Dialog = false,
                                            Message = message,
                                            TransactionJournal = new JournalDTO
                                            {

                                                Id = withinLimitsJournal.Id,
                                                TransactionCode = withinLimitsJournal.TransactionCode,
                                                SequentialId = withinLimitsJournal.SequentialId,
                                                BranchDescription = withinLimitsJournal.BranchDescription,
                                                PrimaryDescription = withinLimitsJournal.PrimaryDescription,
                                                SecondaryDescription = withinLimitsJournal.SecondaryDescription,
                                                PostingPeriodDescription = withinLimitsJournal.PostingPeriodDescription,
                                                ApplicationUserName = withinLimitsJournal.ApplicationUserName,
                                                CreatedDate = withinLimitsJournal.CreatedDate,
                                                TotalValue = withinLimitsJournal.TotalValue,
                                                Reference = withinLimitsJournal.Reference
                                            }

                                        };

                                    }

                                    else
                                    {
                                        return new OperationResult
                                        {

                                            Success = false,
                                            Dialog = false,
                                            Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

                                        };
                                    }

                                case CashWithdrawalCategory.Overdraw:



                                    //ResetView();

                                    return new OperationResult
                                    {
                                        Success = false,
                                        Message = "Sorry, but the customer's account will be overdrawn!"
                                    };


                                //break;

                                default:
                                    break;
                            }
                        }

                        break;

                    case FrontOfficeTransactionType.ChequeDeposit:

                        ExternalChequeDTO NewExternalCheque = new ExternalChequeDTO();

                        NewExternalCheque.Amount = transactionModel.TotalValue;
                        NewExternalCheque.Number = transactionModel.Reference;
                        NewExternalCheque.TellerId = SelectedTeller.Id;

                        NewExternalCheque.Drawer = transactionModel.Drawer;
                        NewExternalCheque.DrawerBank = transactionModel.DrawerBank;
                        NewExternalCheque.DrawerBankBranch = transactionModel.DrawerBankBranch;

                        NewExternalCheque.ChequeTypeId = transactionModel.ChequeType;

                        NewExternalCheque.CustomerAccountId = SelectedCustomerAccount.Id;
                        NewExternalCheque.WriteDate = transactionModel.WriteDate;
                        //NewExternalCheque.ChequeTypeId = (int)ChequeBookType.External;

                        NewExternalCheque.ValidateAll();

                        if (NewExternalCheque.HasErrors)
                        {

                            string message = string.Join(Environment.NewLine, NewExternalCheque.ErrorMessages);
                            //string message = NewExternalCheque.ErrorMessages[0];
                            //MessageBox.Show(message, "ChequeDeposit Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            //_messageService.ShowExclamation(string.Join(Environment.NewLine, NewExternalCheque.ErrorMessages), this.DisplayName);

                            return new OperationResult
                            {
                                Success = false,
                                Dialog = false,
                                Message = message
                            };
                            //ResetView

                        }
                        else
                        {
                            transactionModel.PrimaryDescription = string.Format("{0} - {1}", transactionModel.PrimaryDescription, NewExternalCheque.Number);

                            var externalChequeResult = _externalChequeAppService.AddNewExternalCheque(NewExternalCheque, serviceHeader);

                            if (externalChequeResult == null)
                                return new OperationResult
                                {
                                    Success = false,
                                    Dialog = false,
                                    Message = "The cheque record could not be created. No deposit journal was posted."
                                };

                                var ExternalChequePayables = new List<ExternalChequePayableDTO>();





                                var externalChequePayable = new ExternalChequePayableDTO
                                {
                                    ExternalChequeId = externalChequeResult.Id,
                                    ExternalChequeNumber = externalChequeResult.Number,
                                    CustomerAccountId = (Guid)externalChequeResult.CustomerAccountId
                                };

                                ExternalChequePayables.Add(externalChequePayable);


                                var payablesUpdated = _externalChequeAppService.UpdateExternalChequePayables(externalChequeResult.Id, new List<ExternalChequePayableDTO>(ExternalChequePayables), serviceHeader);
                                if (!payablesUpdated)
                                    return new OperationResult
                                    {
                                        Success = false,
                                        Dialog = false,
                                        Message = "The cheque was captured, but its customer-account allocation could not be created. Do not resubmit it; ask a supervisor to review the captured cheque."
                                    };

                            //var chequeDepositJournal = await _channelService.AddJournalWithCustomerAccountAndTariffsAsync(transactionModel, tariffs, GetServiceHeader());

                            var chequeDepositJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);

                            if (chequeDepositJournal != null && !chequeDepositJournal.HasErrors)
                            {


                                await SendTextNotificationAsync(
                                    "Dear customer, {0} has been received by cheque on your account at {1}, {2}, on {3}. Reference: {4}. {5}. Funds remain unavailable until clearance.",
                                    SelectedCustomer,
                                    SelectedCustomerAccount,
                                    transactionModel.TotalValue,
                                    transactionModel.Reference,
                                    transactionModel.PrimaryDescription,
                                    _textAlertAppService);

                                SelectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(SelectedCustomerAccount.Id, serviceHeader);


                                var updatedTeller = GetCurrentTeller(serviceHeader);
                                string successmessage = $"Customer new balance is {SelectedCustomerAccount.AvailableBalance} and Teller's new balance is {updatedTeller.BookBalance}";


                                return new OperationResult
                                {
                                    Success = true,
                                    Dialog = false,
                                    Message = successmessage,
                                    TransactionJournal = new JournalDTO
                                    {

                                        Id = chequeDepositJournal.Id,
                                        TransactionCode = chequeDepositJournal.TransactionCode,
                                        SequentialId = chequeDepositJournal.SequentialId,
                                        BranchDescription = chequeDepositJournal.BranchDescription,
                                        PrimaryDescription = chequeDepositJournal.PrimaryDescription,
                                        SecondaryDescription = chequeDepositJournal.SecondaryDescription,
                                        PostingPeriodDescription = chequeDepositJournal.PostingPeriodDescription,
                                        ApplicationUserName = chequeDepositJournal.ApplicationUserName,
                                        CreatedDate = chequeDepositJournal.CreatedDate,
                                        TotalValue = chequeDepositJournal.TotalValue,
                                        Reference = chequeDepositJournal.Reference
                                    }

                                };
                            }

                            else
                            {

                                return new OperationResult
                                {

                                    Success = false,
                                    Dialog = false,
                                    Message = "Operation failed"
                                };

                            }
                        }
                    default:



                        return new OperationResult
                        {

                            Success = false,
                            Dialog = false,
                            Message = "You may have entered the wrong transaction typ"
                        };

                }
            }
            catch (Exception)
            {
                throw;
            }

            return new OperationResult
            {

                Success = false,
                Dialog = false,
                Message = "Operation failed. Please try again"
            };


        }


        FrontOfficeTransactionType _frontOfficeTransactionType;
        public FrontOfficeTransactionType FrontOfficeTransactionType
        {
            get { return _frontOfficeTransactionType; }
            set
            {
                if (_frontOfficeTransactionType != value)
                {
                    _frontOfficeTransactionType = value;

                }
            }
        }



        public static async Task SendTextNotificationAsync(string MessageTemplate, CustomerDTO Recipient, CustomerAccountDTO RecipientAccount, decimal Amount, string Reference, string PrimaryDescription, ITextAlertAppService textAlertAppService)
        {
            try
            {
                if (Recipient == null || RecipientAccount == null || textAlertAppService == null)
                    return;

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                if (!string.IsNullOrWhiteSpace(Recipient.AddressMobileLine) &&
                             Regex.IsMatch(Recipient.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") &&
                             Recipient.AddressMobileLine.Length >= 13)
                {
                    var smsBody = new StringBuilder();
                    smsBody.AppendFormat(
                        MessageTemplate,
                        Amount,
                        RecipientAccount.BranchDescription,
                        RecipientAccount.BranchCompanyDescription,
                        DateTime.Now.ToString("MMMM dd, yyyy"),
                        Reference,
                        PrimaryDescription
                    );

                    var textAlertDTO = new TextAlertDTO
                    {
                        BranchId = RecipientAccount.BranchId,
                        TextMessageOrigin = (int)MessageOrigin.Within,
                        TextMessageRecipient = Recipient.AddressMobileLine,
                        TextMessageBody = smsBody.ToString(),
                        MessageCategory = (int)MessageCategory.SMSAlert,
                        AppendSignature = false,
                        TextMessagePriority = (int)QueuePriority.Highest,
                    };

                    textAlertAppService.AddNewTextAlerts(new List<TextAlertDTO> { textAlertDTO }, serviceHeader);
                }
            }
            catch (Exception exception)
            {
                // Notification is a post-transaction side effect. It must not
                // turn a successfully posted journal into a client-visible
                // failure that encourages the teller to submit it again.
                System.Diagnostics.Trace.TraceError(
                    "Cash transaction posted but SMS notification creation failed: {0}",
                    exception);
            }
        }


        public class OperationResult
        {
            public bool Success { get; set; }

            public bool Dialog { get; set; }
            public string Message { get; set; }

            public CustomerTransactionModel TransactionData { get; set; }

            public JournalDTO TransactionJournal { get; set; }
        }


        private TellerDTO GetCurrentTeller(ServiceHeader serviceHeader)
        {
            var employeeIdClaim = (System.Web.HttpContext.Current?.User as ClaimsPrincipal)?.FindFirst("EmployeeId");

            if (employeeIdClaim == null || !Guid.TryParse(employeeIdClaim.Value, out var employeeId))
                throw new InvalidOperationException("Current user has no linked employee/teller record.");

            return _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);
        }


    }

}
