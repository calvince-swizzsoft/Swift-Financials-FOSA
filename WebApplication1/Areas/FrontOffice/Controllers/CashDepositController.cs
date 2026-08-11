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
            IChartOfAccountAppService chartOfAccountAppService
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
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(int? type, int? status, string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
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

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

   


        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CustomerTransactionModel transactionModel)
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            var SelectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(transactionModel.CreditCustomerAccountId, serviceHeader);
            var SelectedCustomer = _customerAppService.FindCustomer(SelectedCustomerAccount.CustomerId, serviceHeader);

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

            var SelectedTeller = GetCurrentTeller(serviceHeader);
            //var SelectedTeller = _tellerAppService.FindTeller((Guid)transactionModel.CurrentTellerId, serviceHeader);



            if (SelectedTeller == null)
            {
                var response = new
                {
                    success = false,
                    message = "Teller is missing",
                    data = (object)null
                };
                return Json(response);
            }

            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
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

            var targetSavingsProduct = _savingsProductAppService.FindSavingsProduct(SelectedCustomerAccount.CustomerAccountTypeTargetProductId, transactionModel.BranchId, serviceHeader);
           

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
                        transactionModel.CreditChartOfAccountId = targetSavingsProduct.ChartOfAccountId;
                    }

                    break;

                case FrontOfficeTransactionType.ChequeDeposit:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.ChequeDeposit;


                    if ((SelectedTeller.BookBalance - transactionModel.TotalValue) < SelectedTeller.RangeLowerLimit)
                    {
                        var response = new
                        {
                            success = false,
                            message = "Sorry, the transaction will reduce teller's balance below limit",
                            data = (object)null
                        };

                        return Json(response);
                    }

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

                    // is it available balance or book balance
                    if ((SelectedCustomerAccount.AvailableBalance) - transactionModel.TotalValue < targetSavingsProduct.MinimumBalance)
                    {

                        var response = new
                        {

                            success = false,
                            message = "Sorry, this transaction will reduce the customer's balance below the minimum balance for product",
                            data = (object)null

                        };

                        return Json(response);

                    }

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
                        //transactionModel.DebitChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
                        transactionModel.DebitChartOfAccountId = targetSavingsProduct.ChartOfAccountId;
                    }

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

                    break;

                case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:

                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashWithdrawalPaymentVoucher;

                    if (SelectedCustomerAccount.BookBalance - transactionModel.TotalValue < targetSavingsProduct.MinimumBalance)
                    {

                        var response = new
                        {

                            success = false,
                            message = "Sorry, this transaction will reduce the customer balance below the allowed minimum balance for product",
                            data = (object)null

                        };

                        return Json(response);

                    }

                    if (Math.Abs(SelectedTeller.BookBalance) - transactionModel.TotalValue < SelectedTeller.RangeLowerLimit)
                    {
                        var response = new
                        {
                            success = false,
                            message = "Sorry, the transaction will reduce teller's balance below limit",
                            data = (object)null
                        };

                        return Json(response);
                    }

                    if (SelectedCustomerAccount != null)
                    {
                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
                        transactionModel.CreditChartOfAccountId = targetSavingsProduct.ChartOfAccountId;
                        //transactionModel.DebitChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
                    }

                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
                        transactionModel.CreditChartOfAccountId = (Guid)SelectedTeller.ChartOfAccountId;

                    break;
            }

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
                var result = await ProcessCustomerTransactionAsync(transactionModel, SelectedTeller, SelectedCustomerAccount, SelectedCustomer, targetSavingsProduct);

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
            catch (Exception ex)
            {

                var response = new
                {

                    success = false,
                    message = ex.Message,
                    data = (object)null

                };

                return Json(response);
            }

        }


        // Checker approval for cash deposit/withdrawal requests goes through the generic
        // maker-checker engine now — POST /api/administration/workflows/items/approve —
        // not a front-office-specific endpoint. That route drives WorkflowProcessorAppService,
        // which calls AuthorizeCashDepositRequest/AuthorizeCashWithdrawalRequest itself with
        // correct multi-level approval counting and a WorkflowItem audit row, which this
        // endpoint never did. See WebApplication1/Areas/FrontOffice/WORKFLOW.md.


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

                        if (customerAccount != null && currentTellerDTO != null)
                        {

                            model.TotalValue = cashDepositRequestDTO.Amount;
                            model.BranchId = cashDepositRequestDTO.BranchId;
                            model.CashDepositRequestId = cashDepositRequestDTO.Id;
                            model.DebitChartOfAccountId = (Guid)currentTellerDTO.ChartOfAccountId;


                            model.Type = (int)FrontOfficeTransactionType.CashDeposit;
                            model.CreditCustomerAccount = customerAccount;

                            model.DebitCustomerAccountId = customerAccount.Id;
                            model.DebitCustomerAccount = customerAccount;
                            model.CreditCustomerAccountId = customerAccount.Id;
                            model.CreditCustomerAccount = customerAccount;


                            model.ValueDate = DateTime.Today;


                            var SelectedCustomer = _customerAppService.FindCustomer(customerAccount.CustomerId, serviceHeader);

                            var selectedProduct = _savingsProductAppService.FindSavingsProduct(customerAccount.CustomerAccountTypeTargetProductId, model.BranchId, serviceHeader);

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
                    if (cashWithdrawalRequestDTO.Status == (int)CashDepositRequestAuthStatus.Authorized)
                    {
                        CustomerTransactionModel model = new CustomerTransactionModel();

                        var customerAccount = _customerAccountAppService.FindCustomerAccountDTO((Guid)cashWithdrawalRequestDTO.CustomerAccountId, serviceHeader);

                        var currentTellerDTO = GetCurrentTeller(serviceHeader);

                        if (customerAccount != null && currentTellerDTO != null)
                        {

                            model.TotalValue = cashWithdrawalRequestDTO.Amount;
                            model.BranchId = cashWithdrawalRequestDTO.BranchId;
                            model.CashWithdrawalRequestId = cashWithdrawalRequestDTO.Id;
                            model.CreditChartOfAccountId = (Guid)currentTellerDTO.ChartOfAccountId;


                            model.Type = (int)FrontOfficeTransactionType.CashWithdrawal;
                            model.CreditCustomerAccount = customerAccount;

                            model.DebitCustomerAccountId = customerAccount.Id;
                            model.DebitCustomerAccount = customerAccount;
                            model.CreditCustomerAccountId = customerAccount.Id;
                            model.CreditCustomerAccount = customerAccount;


                            model.ValueDate = DateTime.Today;


                            var SelectedCustomer = _customerAppService.FindCustomer(customerAccount.CustomerId, serviceHeader);

                            var selectedProduct = _savingsProductAppService.FindSavingsProduct(customerAccount.CustomerAccountTypeTargetProductId, model.BranchId, serviceHeader);

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

            catch (Exception ex)
            {

                return InternalServerError(ex);
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
                        CustomerTransactionModel model = new CustomerTransactionModel();

                        cashWithdrawalRequestDto.Status = (int)CashWithdrawalRequestAuthStatus.Paid;

                        _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(cashWithdrawalRequestDto, null, serviceHeader);

                        return Ok("the cash withdrawal was marked posted");
                    }
                }
                
               
                
                    return BadRequest("The selected request is not authorized yet");
                
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


      

        private async Task<OperationResult> ProcessCustomerTransactionAsync(CustomerTransactionModel transactionModel, TellerDTO selectedTellerDTO, CustomerAccountDTO customerAccountDTO, CustomerDTO selectedCustomer, SavingsProductDTO targetProduct)
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
                //disable tariff computation for now,, missing sp 
                var tariffs = new List<TariffWrapper>();
                //var tariffs = _tellerAppService.ComputeCashTariffs(SelectedCustomerAccount, transactionModel.TotalValue, frontOfficeTransactionType,  serviceHeader);
         
                switch ((FrontOfficeTransactionType)frontOfficeTransactionType)
                {
                    case FrontOfficeTransactionType.CashDeposit:
                        var cashDepositCategory = CashDepositCategory.WithinLimits;

                        if (transactionModel.TotalValue > targetProduct.MaximumAllowedDeposit)
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

                                    aboveMaxCashDepositRequestDTO.Remarks = SelectedTeller.Id.ToString();

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

                                    var cashDepositRequestDTO = _cashDepositRequestAppService.AddNewCashDepositRequest(aboveMaxCashDepositRequestDTO, serviceHeader);

                                    //var systemPermissionTypeInRoles = _authorizationAppService.GetRolesAndApprovalPriorityByPermissionType((int)SystemPermissionType.CashDepositRequestAuthorization, serviceHeader);
                                    var rolesList = _authorizationAppService.GetRolesListForSystemPermissionType((int)SystemPermissionType.CashDepositRequestAuthorization, serviceHeader);

                                    WorkflowDTO workflowDto = new WorkflowDTO
                                    {
                                        RecordId = cashDepositRequestDTO.Id, 
                                        BranchId = transactionModel.BranchId,
                                        Status = (int)WorkflowRecordStatus.Pending,
                                        SystemPermissionType = (int)SystemPermissionType.CashDepositRequestAuthorization,
                                        RequiredApprovals = rolesList.Sum(x => x.RequiredApprovers)
                                    };

                                  
                                    _workflowAppService.AddNewWorkflow(workflowDto, rolesList, serviceHeader);


                                    string message = string.Format(
                                        "{0}.\nNew cash deposit authorization request placed",
                                        EnumHelper.GetDescription(cashDepositCategory)
                                    );


                                    return new OperationResult
                                    {
                                        Success = true,
                                        Dialog = true,
                                        Message = message,
                                        TransactionData = new CustomerTransactionModel
                                        {
                                            CreditCustomerAccountId = SelectedCustomerAccount.Id,
                                            TotalValue = transactionModel.TotalValue,
                                            Reference = transactionModel.Reference
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
                            else if (transactionModel.TotalValue > targetProduct.MaximumAllowedWithdrawal)
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

                                                    _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(targetCashWithdrawalRequest, null, serviceHeader);

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
                                        CashWithdrawalRequestDTO aboveLimitsCashWithdrawalRequest = new CashWithdrawalRequestDTO();

                                        aboveLimitsCashWithdrawalRequest.Amount = transactionModel.TotalValue;
                                        aboveLimitsCashWithdrawalRequest.BranchId = transactionModel.BranchId;
                                        aboveLimitsCashWithdrawalRequest.CustomerName = SelectedCustomer.FullName;
                                        aboveLimitsCashWithdrawalRequest.CustomerAccountId = SelectedCustomerAccount.Id;
                                        aboveLimitsCashWithdrawalRequest.Remarks = selectedTellerDTO.Id.ToString();
                                        aboveLimitsCashWithdrawalRequest.TransactionType = (int)FrontOfficeTransactionType.CashWithdrawal;
                                        aboveLimitsCashWithdrawalRequest.Status = (int)CashWithdrawalRequestAuthStatus.Pending;

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

                                        var cashWithdrawalRequestDTO = _cashWithdrawalRequestAppService.AddNewCashWithdrawalRequest(aboveLimitsCashWithdrawalRequest, serviceHeader);

                                        var withdrawalRolesList = _authorizationAppService.GetRolesListForSystemPermissionType((int)SystemPermissionType.CashWithdrawalRequestAuthorization, serviceHeader);

                                        WorkflowDTO withdrawalWorkflowDto = new WorkflowDTO
                                        {
                                            RecordId = cashWithdrawalRequestDTO.Id,
                                            BranchId = transactionModel.BranchId,
                                            Status = (int)WorkflowRecordStatus.Pending,
                                            SystemPermissionType = (int)SystemPermissionType.CashWithdrawalRequestAuthorization,
                                            RequiredApprovals = withdrawalRolesList.Sum(x => x.RequiredApprovers)
                                        };

                                        _workflowAppService.AddNewWorkflow(withdrawalWorkflowDto, withdrawalRolesList, serviceHeader);

                                        string message = string.Format("{0}.\nSuccessfully placed cash withdrawal authorization request", EnumHelper.GetDescription(cashWithdrawalCategory));


                                        return new OperationResult
                                        {
                                            Success = true,
                                            Dialog = false,
                                            Message = message,
                                            TransactionData = new CustomerTransactionModel
                                            {
                                                DebitCustomerAccountId = SelectedCustomerAccount.Id,
                                                //TotalValue = (cashWithdrawalCategory == CashWithdrawalCategory.PaymentVoucher) ? transactionModel.PaymentVoucher.Amount : transactionModel.TotalValue,
                                                TotalValue = transactionModel.TotalValue,
                                                Reference = transactionModel.PaymentVoucher.Reference,
                                                PaymentVoucherId = transactionModel.PaymentVoucher.Id,
                                                PaymentVoucherPayee = transactionModel.PaymentVoucher.Payee,
                                                CashWithdrawalCategory = (int)cashWithdrawalCategory,
                                                PaymentVoucherWriteDate = transactionModel.PaymentVoucher.WriteDate
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
                                            withinLimitsCashWithdrawalRequest.CustomerName = SelectedCustomer.FullName;
                                            withinLimitsCashWithdrawalRequest.TransactionType = (int)FrontOfficeTransactionType.CashWithdrawal;
                                            withinLimitsCashWithdrawalRequest.Remarks = SelectedTeller.Id.ToString();

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

                                            _cashWithdrawalRequestAppService.AddNewCashWithdrawalRequest(withinLimitsCashWithdrawalRequest, serviceHeader);
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


                            if (externalChequeResult != null)
                            {

                                var ExternalChequePayables = new List<ExternalChequePayableDTO>();





                                var externalChequePayable = new ExternalChequePayableDTO
                                {
                                    ExternalChequeId = externalChequeResult.Id,
                                    ExternalChequeNumber = externalChequeResult.Number,
                                    CustomerAccountId = (Guid)externalChequeResult.CustomerAccountId
                                };

                                ExternalChequePayables.Add(externalChequePayable);


                                if (ExternalChequePayables != null)
                                    _externalChequeAppService.UpdateExternalChequePayables(externalChequeResult.Id, new List<ExternalChequePayableDTO>(ExternalChequePayables), serviceHeader);
                                    
                            }

                            //var chequeDepositJournal = await _channelService.AddJournalWithCustomerAccountAndTariffsAsync(transactionModel, tariffs, GetServiceHeader());

                            var chequeDepositJournal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, transactionModel.CreditCustomerAccount, transactionModel.DebitCustomerAccount, tariffs, serviceHeader);

                            if (chequeDepositJournal != null && !chequeDepositJournal.HasErrors)
                            {


                                #region Send Text Notification

                                if (!string.IsNullOrWhiteSpace(SelectedCustomer.AddressMobileLine) &&
                           Regex.IsMatch(SelectedCustomer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") &&
                           SelectedCustomer.AddressMobileLine.Length >= 13)
                                {

                                    var smsBody = new StringBuilder();
                                    smsBody.AppendFormat(
                                        "Dear customer, {0} of {1} has been effected on your fosa account at {2} at Branch {3}",
                                        transactionModel.Reference,
                                        transactionModel.TotalValue,
                                        SelectedCustomerAccount.BranchDescription,
                                        SelectedCustomerAccount.BranchCompanyDescription,
                                        DateTime.Now.ToString("MMMM dd, yyyy")
                                    );


                                    var textAlertDTO = new TextAlertDTO
                                    {
                                        BranchId = SelectedCustomerAccount.BranchId,
                                        TextMessageOrigin = (int)MessageOrigin.Within,
                                        TextMessageRecipient = SelectedCustomer.AddressMobileLine,
                                        TextMessageBody = smsBody.ToString(),
                                        MessageCategory = (int)MessageCategory.SMSAlert,
                                        AppendSignature = false,
                                        TextMessagePriority = (int)QueuePriority.Highest,
                                    };


                                    var textAlertDTOs = new List<TextAlertDTO>{ textAlertDTO };


                                    _textAlertAppService.AddNewTextAlerts(textAlertDTOs, serviceHeader);
                                }

                                #endregion

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
            catch (Exception ex)
            {
                return new OperationResult
                {

                    Success = false,
                    Dialog = false,
                    Message = $"An error occurred: {ex.Message}"
                };

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

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
          
            if (!string.IsNullOrWhiteSpace(Recipient.AddressMobileLine) &&
                         Regex.IsMatch(Recipient.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") &&
                         Recipient.AddressMobileLine.Length >= 13)
            {
                // Build the SMS body message
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


                var textAlertDTOs = new List<TextAlertDTO> { textAlertDTO };

               

                textAlertAppService.AddNewTextAlerts(textAlertDTOs, serviceHeader);
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