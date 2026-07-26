//using Application.MainBoundedContext.DTO;
//using Application.MainBoundedContext.DTO.AccountsModule;
//using Application.MainBoundedContext.DTO.FrontOfficeModule;
//using Application.MainBoundedContext.DTO.MessagingModule;
//using Application.MainBoundedContext.DTO.RegistryModule;
//using Infrastructure.Crosscutting.Framework.Utils;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Globalization;
//using System.Linq;
//using System.Runtime.Remoting.Channels;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using static WebApplication1.Controllers.CashDepositController;

//namespace WebApplication1.Controllers
//{

//    [RoutePrefix("api/withdrawal")]
//    public class CashWithdrawalController : ApiController
//    {


//        [HttpGet]
//        [Route("vouchers")]
//        public async Task<IHttpActionResult> GetPaymentVouchers(Guid chequeBookId)
//        {
//            try
//            {

//                var serviceHeader = new ServiceHeader
//                {
//                    ApplicationDomainName = "SwiftApis",
//                    ApplicationUserName = "Admin",
//                    EnvironmentDomainName = "SwiftApis",
//                    //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
//                    EnvironmentIPAddress = "",
//                    EnvironmentMACAddress = "",
//                    EnvironmentMachineName = Environment.MachineName,
//                    EnvironmentMotherboardSerialNumber = "",
//                    EnvironmentOSVersion = Environment.OSVersion.ToString(),
//                    EnvironmentProcessorId = "",
//                    EnvironmentUserName = Environment.UserName
//                };
//                //var vouchers = await _channelService.FindPaymentVouchersByChequeBookIdAsync();
//                var vouchers = await _channelService.FindPaymentVouchersByChequeBookIdAsync(chequeBookId, GetServiceHeader());
//                return Ok(vouchers);
//            }

//            catch (Exception ex)
//            {


//                return InternalServerError(ex);

//            }
//        }

//        [HttpGet]
//        [Route("")]
//        public async Task<IHttpActionResult> Index()
//        {
//            try
//            {

//                var cashwithdrawals = await _channelService.FindCashWithdrawalRequestsAsync(GetServiceHeader());

//                if (cashwithdrawals == null)
//                {
//                    return NotFound();
//                }

//                foreach (var cwr in cashwithdrawals)
//                {

//                    var customeracc = await _channelService.FindCustomerAccountAsync((Guid)cwr.CustomerAccountId, false, false, false, false, GetServiceHeader());

//                    var customer = await _channelService.FindCustomerAsync(customeracc.CustomerId, GetServiceHeader());

//                    cwr.CustomerName = customer.IndividualFirstName + " " + customer.IndividualLastName;

//                }

//                return Ok(cashwithdrawals);
//            }

//            catch (Exception ex)
//            {

//                return InternalServerError(ex);
//            }
//        }


//        [HttpPost]
//        [Route("")]
//        public async Task<IHttpActionResult> Create(CustomerTransactionModel transactionModel)
//        {
//            bool includeBalances = true;
//            bool includeProductDescription = true;
//            bool includeInterestBalanceForLoanAccounts = true;
//            bool considerMaturityPeriodForInvestmentAccounts = true;

//            var SelectedCustomerAccount = await _channelService.FindCustomerAccountAsync(transactionModel.DebitCustomerAccountId, includeBalances, includeProductDescription, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts, GetServiceHeader());

//            var SelectedCustomer = await _channelService.FindCustomerAsync(SelectedCustomerAccount.CustomerId, GetServiceHeader());


//            if (SelectedCustomerAccount == null)
//            {

//                var response = new
//                {

//                    success = false,
//                    message = "Please select a customer account",

//                };

//                return Json(response);
//            }

//            if ((RecordStatus)SelectedCustomerAccount.RecordStatus != RecordStatus.Approved)
//            {

//                var response = new
//                {

//                    success = false,
//                    message = "Sorry, account is not approved yet",

//                };

//                return Json(response);
//            }

//            var SelectedBranch = await _channelService.FindBranchAsync(transactionModel.BranchId, GetServiceHeader());
//            var SelectedTeller = await _channelService.FindTellerAsync((Guid)transactionModel.CurrentTellerId, true, GetServiceHeader());
//            //var SelectedTeller = await GetCurrentTeller();

//            if (SelectedTeller == null)
//            {
//                var response = new
//                {

//                    success = false,
//                    message = "Teller is missing",

//                };
//                return Json(response);

//            }

//            var postingPeriod = await _channelService.FindCurrentPostingPeriodAsync(GetServiceHeader());
//            transactionModel.PostingPeriodId = postingPeriod.Id;
//            transactionModel.PrimaryDescription = "ok";
//            transactionModel.SecondaryDescription = string.Format("B{0}/T{1}/#{2}", SelectedBranch.Code, SelectedTeller.Code, SelectedTeller.ItemsCount);
//            transactionModel.Reference = string.Format("{0}", SelectedCustomerAccount.CustomerReference1);
//            transactionModel.CreditChartOfAccountId = (Guid)SelectedTeller.ChartOfAccountId;

//            transactionModel.Teller = SelectedTeller;

//            switch ((FrontOfficeTransactionType)transactionModel.Type)
//            {
//                case FrontOfficeTransactionType.CashDeposit:

//                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashDeposit;

//                    if (Math.Abs(SelectedTeller.BookBalance) - Math.Abs(transactionModel.TotalValue) < SelectedTeller.RangeLowerLimit)
//                    {
//                        var response = new
//                        {
//                            success = false,
//                            message = "Sorry, the transaction will reduce teller's balance below limit",
//                        };

//                        return Json(response);
//                    }


//                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
//                        transactionModel.DebitChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

//                    if (SelectedCustomerAccount != null)
//                    {
//                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.CreditChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
//                    }

//                    break;

//                case FrontOfficeTransactionType.ChequeDeposit:

//                    transactionModel.TransactionCode = (int)SystemTransactionCode.ChequeDeposit;

//                    if (SelectedTeller.BookBalance - transactionModel.TotalValue < SelectedTeller.RangeLowerLimit)
//                    {
//                        var response = new
//                        {
//                            success = false,
//                            message = "Sorry, the transaction will reduce teller's balance below limit",
//                        };

//                        return Json(response);
//                    }

//                    transactionModel.TransactionCode = (int)SystemTransactionCode.ChequeDeposit;

//                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
//                        transactionModel.DebitChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

//                    if (SelectedCustomerAccount != null)
//                    {
//                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.CreditChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
//                    }

//                    break;

//                case FrontOfficeTransactionType.CashWithdrawal:

//                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashWithdrawal;

//                    if (SelectedCustomerAccount.BookBalance - transactionModel.TotalValue < SelectedCustomerAccount.CustomerAccountTypeTargetProductMinimumBalance)
//                    {

//                        var response = new
//                        {

//                            success = false,
//                            message = "Sorry, this transaction will reduce the customer's balance below the minimum balance for product",

//                        };

//                        return Json(response);

//                    }

//                    if (SelectedCustomerAccount != null)
//                    {
//                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.DebitChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
//                    }

//                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
//                        transactionModel.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

//                    break;

//                case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:

//                    transactionModel.TransactionCode = (int)SystemTransactionCode.CashWithdrawalPaymentVoucher;

//                    if (SelectedCustomerAccount.BookBalance - transactionModel.TotalValue < SelectedCustomerAccount.CustomerAccountTypeTargetProductMinimumBalance)
//                    {

//                        var response = new
//                        {

//                            success = false,
//                            message = "Sorry, this transaction will reduce the customer balance below the allowed minimum balance for product",

//                        };

//                        return Json(response);

//                    }

//                    if (Math.Abs(SelectedTeller.BookBalance) - transactionModel.TotalValue < SelectedTeller.RangeLowerLimit)
//                    {
//                        var response = new
//                        {
//                            success = false,
//                            message = "Sorry, the transaction will reduce teller's balance below limit",
//                        };

//                        return Json(response);
//                    }

//                    if (SelectedCustomerAccount != null)
//                    {
//                        transactionModel.DebitCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.DebitCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccountId = SelectedCustomerAccount.Id;
//                        transactionModel.CreditCustomerAccount = SelectedCustomerAccount;
//                        transactionModel.DebitChartOfAccountId = SelectedCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId;
//                    }

//                    if (SelectedTeller != null && !SelectedTeller.IsLocked)
//                        transactionModel.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

//                    break;
//            }

//            transactionModel.ValidateAll();
//            if (transactionModel.HasErrors)
//            {
//                var errorMessages = transactionModel.ErrorMessages
//                    .Select(error => error)
//                    .ToList();

//                string combinedErrorMessage = string.Join("; ", errorMessages);
//                //   ViewBag.TransactionTypeSelectList = GetFrontOfficeTransactionTypeSelectList(SelectedCustomerAccount.Type.ToString());


//                var responseLast = new
//                {
//                    success = false,
//                    message = $"Transaction Error: {combinedErrorMessage}"
//                };

//                return Json(responseLast);
//            }


//            try
//            {
//                // Call the asynchronous method and check its result
//                var result = await ProcessCustomerTransactionAsync(transactionModel, SelectedTeller, SelectedCustomerAccount, SelectedCustomer);

//                SelectedCustomerAccount = await _channelService.FindCustomerAccountAsync(transactionModel.CreditCustomerAccountId, true, true, false, false, GetServiceHeader());
//                //  var SelectedCustomer = await _channelService.FindCustomerAsync(SelectedCustomerAccount.CustomerId);


//                if (result.Success)
//                {


//                    var response = new
//                    {
//                        success = true,
//                        message = "Operation Success"
//                        //TellerBookBalance = Teller.BookBalance,
//                        //CustomerAvailableBalance = SelectedCustomerAccount.AvailableBalance,
//                        //journalId = result.TransactionJournal.Id,
//                        //journalSequentialId = result.TransactionJournal.SequentialId,
//                        //journalBranchDescription = result.TransactionJournal.BranchDescription,
//                        //journalPrimaryDescription = result.TransactionJournal.PrimaryDescription,
//                        //journalSecondaryDescription = result.TransactionJournal.SecondaryDescription,
//                        //journalPostingPeriodDescription = result.TransactionJournal.PostingPeriodDescription,
//                        //journalApplicationUserName = result.TransactionJournal.ApplicationUserName,
//                        //journalCreatedDate = result.TransactionJournal.CreatedDate,
//                        //journalTotalValue = result.TransactionJournal.TotalValue,
//                        //journalReference = result.TransactionJournal.Reference

//                    };

//                    return Json(response);
//                }
//                else if (!result.Success && result.Dialog)
//                {
//                    if (result.TransactionData.CreditCustomerAccountId != null && result.TransactionData.CreditCustomerAccountId != Guid.Empty)
//                    {

//                        var response = new
//                        {
//                            isCashDepositRequest = true,
//                            success = false,
//                            dialog = true,
//                            message = result.Message,
//                            selectedCustomerAccountId = result.TransactionData.CreditCustomerAccountId,
//                            transactionTotalValue = result.TransactionData.TotalValue,
//                            transactionReference = result.TransactionData.Reference,
//                            cashTransactionRequestId = result.TransactionData.CashDepositRequestId,
//                            transactionCategory = result.TransactionData.CashDepositCategory
//                        };

//                        return Json(response);

//                    }

//                    else if (result.TransactionData.DebitCustomerAccountId != null && result.TransactionData.DebitCustomerAccountId != Guid.Empty)
//                    {
//                        var response = new
//                        {
//                            isCashWithdrawalRequest = true,
//                            success = false,
//                            dialog = true,
//                            message = result.Message,
//                            selectedCustomerAccountId = result.TransactionData.DebitCustomerAccountId,
//                            transactionTotalValue = result.TransactionData.TotalValue,
//                            transactionReference = result.TransactionData.Reference,
//                            cashTransactionRequestId = result.TransactionData.CashWithdrawalRequestId,
//                            transactionCategory = result.TransactionData.CashWithdrawalCategory,
//                            paymentVoucherId = result.TransactionData.PaymentVoucherId,
//                            paymentVoucherPayee = result.TransactionData.PaymentVoucherPayee,
//                            paymentVoucherChequeBookId = result.TransactionData.ChequeBookId,
//                            paymentVoucherWriteDate = result.TransactionData.PaymentVoucherWriteDate
//                        };

//                        return Json(response);
//                    }

//                    // Default return for any path not covered by conditions
//                    return Json(new
//                    {
//                        success = false,
//                        dialog = false,
//                        message = "No valid transaction data found."
//                    });
//                }

//                else
//                {

//                    //ViewBag.TransactionTypeSelectList = GetFrontOfficeTransactionTypeSelectList(SelectedCustomerAccount.Type.ToString());

//                    var response = new
//                    {

//                        success = false,
//                        message = result.Message,
//                        //selectedCustomerAccountId = result.TransactionData.CreditCustomerAccountId,
//                        //transactionTotalValue = result.TransactionData.TotalValue,
//                        //transactionReference = result.TransactionData.Reference

//                    };

//                    return Json(response);


//                }
//            }
//            catch (Exception ex)
//            {

//                var response = new
//                {

//                    success = false,
//                    message = ex.Message

//                };

//                return Json(response);
//            }

//        }




//        //[HttpPut]
//        //[Route("{id}")]
//        //public async Task<IHttpActionResult> UpdateCashWithdrawalRequestDTO(CashWithdrawalRequestDTO cashWithdrawalRequestDTO)
//        //{
//        //    try
//        //    {

//        //        var updatedCashWithdrawalRequestDTO = await _channelService.AddCashWithdrawalRequestAsync(cashWithdrawalRequestDTO, GetServiceHeader());
//        //        return Ok(cashWithdrawalRequestDTO);
//        //    }

//        //    catch (Exception ex)
//        //    {
//        //        return InternalServerError(ex);
//        //    }
//        //}

//        [HttpPost]
//        [Route("authorize")]
//        public async Task<IHttpActionResult> AuthorizeCashWithdrawalRequest(Guid cashWithdrawalRequestId, int customerTransactionAuthOption)
//        {

//            Guid parseId;

//            try
//            {
//                if (cashWithdrawalRequestId == Guid.Empty || !Guid.TryParse(cashWithdrawalRequestId.ToString(), out parseId))
//                {
//                    return BadRequest("Invalid Id");
//                }

//                var cashWithdrawalRequestDTO = await _channelService.FindCashWithdrawalRequestAsync(cashWithdrawalRequestId, GetServiceHeader());

//                var AuthorizedCashWithdrawalRequest = await _channelService.AuthorizeCashWithdrawalRequestAsync(cashWithdrawalRequestDTO, customerTransactionAuthOption, GetServiceHeader());

//                return Ok(AuthorizedCashWithdrawalRequest);

//            }

//            catch (Exception ex)
//            {

//                return InternalServerError(ex);
//            }
//        }



//        [HttpPost]
//        [Route("post")]
//        public async Task<IHttpActionResult> PostCashWithdrawalRequest(Guid cashWithdrawalRequestId)
//        {

//            Guid parseId;

//            try
//            {
//                if (cashWithdrawalRequestId == Guid.Empty || !Guid.TryParse(cashWithdrawalRequestId.ToString(), out parseId))
//                {
//                    return BadRequest("Invalid Id");
//                }

//                var cashWithdrawalRequestDTO = await _channelService.FindCashWithdrawalRequestAsync(cashWithdrawalRequestId, GetServiceHeader());

//                var paymentVoucherDTO = new PaymentVoucherDTO();

//                paymentVoucherDTO.Amount = cashWithdrawalRequestDTO.Amount;
//                paymentVoucherDTO.WriteDate = DateTime.UtcNow;
//                paymentVoucherDTO.ChequeBookCustomerAccountId = (Guid)cashWithdrawalRequestDTO.CustomerAccountId;
//                paymentVoucherDTO.Payee = cashWithdrawalRequestDTO.CustomerAccountCustomerFullName;


//                var PostedCashWithdrawalRequest = await _channelService.PayCashWithdrawalRequestAsync(cashWithdrawalRequestDTO, paymentVoucherDTO, GetServiceHeader());

//                return Ok(PostedCashWithdrawalRequest);

//            }

//            catch (Exception ex)
//            {

//                return InternalServerError(ex);
//            }
//        }



//        private async Task<TellerDTO> GetCurrentTeller()
//        {


//            bool includeBalance = true;
//            // Get the current user
//            //var user = await _applicationUserManager.FindByIdAsync(User.Identity.GetUserId());

//            var teller = await _channelService.FindTellerByEmployeeIdAsync(Guid.Parse("50BDE4A6-1F50-F111-9B87-C8E2651EF92A"), includeBalance, GetServiceHeader());

//            return teller;

//        }


//        private async Task<OperationResult> ProcessCustomerTransactionAsync(CustomerTransactionModel transactionModel, TellerDTO selectedTellerDTO, CustomerAccountDTO customerAccountDTO, CustomerDTO selectedCustomer)
//        {

//            bool includeBalances = true;
//            bool includeProductDescription = true;
//            bool includeInterestBalanceForLoanAccounts = true;
//            bool considerMaturityPeriodForInvestmentAccounts = true;

//            bool IsBusy = false;

//            var SelectedCustomerAccount = customerAccountDTO;

//            var SelectedTeller = selectedTellerDTO;

//            var SelectedCustomer = selectedCustomer;

//            System.Globalization.NumberFormatInfo _nfi = new CultureInfo("en-US", false).NumberFormat;
//            var time = System.DateTime.Now.ToString("dd/mm/yyyy");

//            try
//            {

//                int frontOfficeTransactionType = transactionModel.Type;
//                var tariffs = await _channelService.ComputeTellerCashTariffsAsync(SelectedCustomerAccount, transactionModel.TotalValue, frontOfficeTransactionType, GetServiceHeader());

//                switch ((FrontOfficeTransactionType)frontOfficeTransactionType)
//                {
//                    case FrontOfficeTransactionType.CashDeposit:
//                        var cashDepositCategory = CashDepositCategory.WithinLimits;

//                        if (transactionModel.TotalValue > SelectedCustomerAccount.CustomerAccountTypeTargetProductMaximumAllowedDeposit)
//                        {
//                            cashDepositCategory = CashDepositCategory.AboveMaximumAllowed;
//                        }

//                        switch (cashDepositCategory)
//                        {
//                            case CashDepositCategory.WithinLimits:


//                                // cashDepositRequestDTO.Remarks = transactionModel.Reference;

//                                var withinLimitsCashDepositJournal = await _channelService.AddJournalWithCustomerAccountAndTariffsAsync(transactionModel, tariffs, GetServiceHeader());

//                                transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
//                                var updateWithinLimitResult = await _channelService.UpdateCustomerAccountAsync(SelectedCustomerAccount, GetServiceHeader());


//                                if (updateWithinLimitResult)
//                                {
//                                    CashDepositRequestDTO withinLimitCashDepositRequestDTO = new CashDepositRequestDTO();

//                                    withinLimitCashDepositRequestDTO.Amount = transactionModel.TotalValue;
//                                    withinLimitCashDepositRequestDTO.BranchId = transactionModel.BranchId;
//                                    withinLimitCashDepositRequestDTO.CustomerAccountId = SelectedCustomerAccount.Id;
//                                    withinLimitCashDepositRequestDTO.CustomerName = SelectedCustomer.FullName;

//                                    withinLimitCashDepositRequestDTO.AuthorizedBy = SelectedTeller.Description;
//                                    withinLimitCashDepositRequestDTO.AuthorizedDate = DateTime.Today;

//                                    withinLimitCashDepositRequestDTO.Posted = true;

//                                    withinLimitCashDepositRequestDTO.Type = transactionModel.Type;


//                                    _channelService.AddCashDepositRequestAsync(withinLimitCashDepositRequestDTO, GetServiceHeader());


//                                    string message = $"Operation success: Customer's new balance is {SelectedCustomerAccount.NewAvailableBalance}";

//                                    string cashDepositTextTemplate = "Dear customer, your account has been credited with a cash deposit of KES {0} at {1} Branch {2}.";
//                                    await SendTextNotificationAsync(cashDepositTextTemplate, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription);

//                                    return new OperationResult
//                                    {
//                                        Success = true,
//                                        Dialog = false,
//                                        Message = message,
//                                        TransactionJournal = new JournalDTO
//                                        {

//                                            Id = withinLimitsCashDepositJournal.Id,
//                                            SequentialId = withinLimitsCashDepositJournal.SequentialId,
//                                            BranchDescription = withinLimitsCashDepositJournal.BranchDescription,
//                                            PrimaryDescription = withinLimitsCashDepositJournal.PrimaryDescription,
//                                            SecondaryDescription = withinLimitsCashDepositJournal.SecondaryDescription,
//                                            PostingPeriodDescription = withinLimitsCashDepositJournal.PostingPeriodDescription,
//                                            ApplicationUserName = withinLimitsCashDepositJournal.ApplicationUserName,
//                                            CreatedDate = withinLimitsCashDepositJournal.CreatedDate,
//                                            TotalValue = withinLimitsCashDepositJournal.TotalValue,
//                                            Reference = withinLimitsCashDepositJournal.Reference
//                                        }

//                                    };

//                                }

//                                else
//                                {
//                                    return new OperationResult
//                                    {

//                                        Success = false,
//                                        Dialog = false,
//                                        Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

//                                    };
//                                }

//                            case CashDepositCategory.AboveMaximumAllowed:

//                                CashDepositRequestDTO aboveMaxCashDepositRequestDTO = new CashDepositRequestDTO();

//                                _channelService.AddCashDepositRequestAsync(aboveMaxCashDepositRequestDTO, GetServiceHeader());

//                                aboveMaxCashDepositRequestDTO.Amount = transactionModel.TotalValue;
//                                aboveMaxCashDepositRequestDTO.BranchId = transactionModel.BranchId;
//                                aboveMaxCashDepositRequestDTO.CustomerAccountId = SelectedCustomerAccount.Id;
//                                aboveMaxCashDepositRequestDTO.CustomerName = SelectedCustomer.FullName;

//                                aboveMaxCashDepositRequestDTO.Posted = false;
//                                aboveMaxCashDepositRequestDTO.Type = transactionModel.Type;


//                                var createNewCashDepositRequest = default(bool);

//                                var actionableCashDepositRequests = await _channelService.FindActionableCashDepositRequestsByCustomerAccountAsync(SelectedCustomerAccount, GetServiceHeader());

//                                if (actionableCashDepositRequests != null && actionableCashDepositRequests.Any())
//                                {
//                                    var targetCashDepositRequest = actionableCashDepositRequests.Where(x => x.Id == transactionModel.CashDepositRequest.Id).FirstOrDefault();

//                                    if (targetCashDepositRequest != null)
//                                    {
//                                        // Check if another operation is already in progress
//                                        if (IsBusy)
//                                        {
//                                            return new OperationResult
//                                            {

//                                                Success = false,
//                                                Dialog = false,
//                                                Message = "Please wait until the current operation is complete.",

//                                            };
//                                        }

//                                        // Set IsBusy to true to indicate an ongoing operation
//                                        IsBusy = true;

//                                        // Format the message
//                                        string message = string.Format(
//                                            "Txn Request of {1} is {2} for this customer account.\n\nDo you want to proceed?",
//                                            EnumHelper.GetDescription(CashDepositCategory.AboveMaximumAllowed),
//                                            string.Format(_nfi, "{0:C}", targetCashDepositRequest.Amount),
//                                            targetCashDepositRequest.StatusDescription
//                                        );


//                                        return new OperationResult
//                                        {
//                                            Success = false,
//                                            Dialog = true,
//                                            Message = message,
//                                            TransactionData = new CustomerTransactionModel
//                                            {

//                                                CreditCustomerAccountId = SelectedCustomerAccount.Id,
//                                                CashDepositRequestId = targetCashDepositRequest.Id

//                                            }
//                                        };
//                                    }
//                                    else createNewCashDepositRequest = true;
//                                }
//                                else createNewCashDepositRequest = true;

//                                if (createNewCashDepositRequest)
//                                {
//                                    string message = string.Format(
//                                        "{0}.\nDo you want to proceed and place a cash deposit authorization request?",
//                                        EnumHelper.GetDescription(cashDepositCategory)
//                                    );


//                                    return new OperationResult
//                                    {
//                                        Success = false,
//                                        Dialog = true,
//                                        Message = message,
//                                        TransactionData = new CustomerTransactionModel
//                                        {
//                                            CreditCustomerAccountId = SelectedCustomerAccount.Id,
//                                            TotalValue = transactionModel.TotalValue,
//                                            Reference = transactionModel.Reference
//                                        }
//                                    };

//                                }

//                                break;

//                            // Handle other categories if needed
//                            default:
//                                break;
//                        }

//                        break;

//                    // Handle other transaction types if needed

//                    case FrontOfficeTransactionType.CashWithdrawal:
//                    case FrontOfficeTransactionType.CashWithdrawalPaymentVoucher:

//                        if (Math.Abs(transactionModel.Teller.BookBalance) < transactionModel.TotalValue)
//                        {

//                            return new OperationResult
//                            {

//                                Success = false,
//                                Message = "Sorry, but your teller G/L account has insufficient cash!"
//                            };


//                        }

//                        else
//                        {
//                            var cashWithdrawalCategory = CashWithdrawalCategory.WithinLimits;

//                            if ((FrontOfficeTransactionType)frontOfficeTransactionType == FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
//                            {
//                                cashWithdrawalCategory = CashWithdrawalCategory.PaymentVoucher;
//                            }
//                            else if (transactionModel.TotalValue > SelectedCustomerAccount.CustomerAccountTypeTargetProductMaximumAllowedWithdrawal)
//                            {
//                                cashWithdrawalCategory = CashWithdrawalCategory.AboveMaximumAllowed;
//                            }
//                            else if (((transactionModel.TotalValue + tariffs.Where(x => x.ChargeBenefactor == (int)ChargeBenefactor.Customer).Sum(x => x.Amount)) > SelectedCustomerAccount.AvailableBalance) && ((transactionModel.TotalValue + tariffs.Sum(x => x.Amount)) <= (SelectedCustomerAccount.AvailableBalance + SelectedCustomerAccount.CustomerAccountTypeTargetProductMinimumBalance)))
//                            {
//                                cashWithdrawalCategory = CashWithdrawalCategory.BelowMinimumBalance;
//                            }

//                            //TODO: maybe u want to Check for OverDraw earlier 
//                            else if ((transactionModel.TotalValue + tariffs.Where(x => x.ChargeBenefactor == (int)ChargeBenefactor.Customer).Sum(x => x.Amount)) > (SelectedCustomerAccount.AvailableBalance + SelectedCustomerAccount.CustomerAccountTypeTargetProductMinimumBalance))
//                            {
//                                cashWithdrawalCategory = CashWithdrawalCategory.Overdraw;
//                            }

//                            switch (cashWithdrawalCategory)
//                            {
//                                case CashWithdrawalCategory.AboveMaximumAllowed:
//                                case CashWithdrawalCategory.BelowMinimumBalance:
//                                case CashWithdrawalCategory.PaymentVoucher:

//                                    var createNewCashWithdrawalRequest = default(bool);

//                                    var actionableCashWithdrawalRequests = await _channelService.FindMatureCashWithdrawalRequestsByCustomerAccountIdAsync
//                                        (SelectedCustomerAccount, GetServiceHeader());

//                                    //var actionableCashWithdrawalRequests = await _channelService.FindActionableCashWithdrawalRequestsByCustomerAccountAsync(SelectedCustomerAccount, GetServiceHeader());

//                                    if (actionableCashWithdrawalRequests != null && actionableCashWithdrawalRequests.Any())
//                                    {
//                                        if ((actionableCashWithdrawalRequests.Where(x => x.Category == (int)cashWithdrawalCategory)).Any())
//                                        {
//                                            var targetCashWithdrawalRequest = actionableCashWithdrawalRequests.Where(x => x.Category == (int)cashWithdrawalCategory && x.Amount == transactionModel.TotalValue).FirstOrDefault();

//                                            if (targetCashWithdrawalRequest != null)
//                                            {
//                                                string message = string.Format(
//                                                "Txn Request of {1} is {2} for this customer account.\n\nDo you want to proceed?",
//                                                EnumHelper.GetDescription(CashWithdrawalCategory.AboveMaximumAllowed),
//                                                string.Format(_nfi, "{0:C}", targetCashWithdrawalRequest.Amount),
//                                                targetCashWithdrawalRequest.StatusDescription
//                                                );

//                                                var result = new OperationResult
//                                                {

//                                                    Success = false,
//                                                    Dialog = true,
//                                                    Message = message,

//                                                };

//                                                if (cashWithdrawalCategory == CashWithdrawalCategory.PaymentVoucher)
//                                                {

//                                                    result.TransactionData = new CustomerTransactionModel
//                                                    {

//                                                        DebitCustomerAccountId = SelectedCustomerAccount.Id,
//                                                        CashWithdrawalCategory = (int)cashWithdrawalCategory,
//                                                        PaymentVoucherId = transactionModel.PaymentVoucher.Id,
//                                                        ChequeBookId = transactionModel.PaymentVoucher.ChequeBookId,
//                                                        TotalValue = transactionModel.TotalValue,
//                                                        Reference = transactionModel.Reference,
//                                                        PaymentVoucherWriteDate = transactionModel.PaymentVoucher.WriteDate,
//                                                        PaymentVoucherPayee = transactionModel.PaymentVoucher.Payee,
//                                                        CashWithdrawalRequestId = targetCashWithdrawalRequest.Id
//                                                    };

//                                                }

//                                                else
//                                                {

//                                                    result.TransactionData = new CustomerTransactionModel
//                                                    {


//                                                        DebitCustomerAccountId = SelectedCustomerAccount.Id,
//                                                        CashWithdrawalCategory = (int)cashWithdrawalCategory,
//                                                        TotalValue = transactionModel.TotalValue,
//                                                        Reference = transactionModel.Reference,
//                                                        CashWithdrawalRequestId = targetCashWithdrawalRequest.Id

//                                                    };


//                                                }


//                                                return result;

//                                            }

//                                            else createNewCashWithdrawalRequest = true;
//                                        }
//                                        else createNewCashWithdrawalRequest = true;
//                                    }
//                                    else createNewCashWithdrawalRequest = true;

//                                    if (createNewCashWithdrawalRequest)
//                                    {



//                                        CashWithdrawalRequestDTO aboveLimitsCashWithdrawalRequest = new CashWithdrawalRequestDTO();

//                                        aboveLimitsCashWithdrawalRequest.Amount = transactionModel.TotalValue;
//                                        aboveLimitsCashWithdrawalRequest.BranchId = transactionModel.BranchId;
//                                        aboveLimitsCashWithdrawalRequest.CustomerName = SelectedCustomer.FullName;
//                                        aboveLimitsCashWithdrawalRequest.CustomerAccountId = SelectedCustomerAccount.Id;
//                                        //   aboveLimitsCashWithdrawalRequest.
//                                        //   aboveLimitsCashWithdrawalRequest.AuthorizedDate = DateTime.Today;
//                                        //aboveLimitsCashWithdrawalRequest.Status = (int)CustomerTransactionAuthOption.;
//                                        //  aboveLimitsCashWithdrawalRequest.AuthorizedBy = SelectedTeller.Description;


//                                        _channelService.AddCashWithdrawalRequestAsync(aboveLimitsCashWithdrawalRequest, GetServiceHeader());


//                                        string message = string.Format(
//                 "{0}.\nDo you want to proceed and place a cash withdrawal authorization request?",
//                 EnumHelper.GetDescription(cashWithdrawalCategory)
//             );


//                                        return new OperationResult
//                                        {
//                                            Success = false,
//                                            Dialog = true,
//                                            Message = message,
//                                            TransactionData = new CustomerTransactionModel
//                                            {
//                                                DebitCustomerAccountId = SelectedCustomerAccount.Id,
//                                                TotalValue = (cashWithdrawalCategory == CashWithdrawalCategory.PaymentVoucher) ? transactionModel.PaymentVoucher.Amount : transactionModel.TotalValue,
//                                                Reference = transactionModel.PaymentVoucher.Reference,
//                                                PaymentVoucherId = transactionModel.PaymentVoucher.Id,
//                                                PaymentVoucherPayee = transactionModel.PaymentVoucher.Payee,
//                                                CashWithdrawalCategory = (int)cashWithdrawalCategory,
//                                                PaymentVoucherWriteDate = transactionModel.PaymentVoucher.WriteDate
//                                            }
//                                        };
//                                    }
//                                    break;

//                                case CashWithdrawalCategory.WithinLimits:

//                                    //if (SelectedCustomer.BiometricFingerprintTemplateBuffer != null && SelectedBranch.CompanyEnforceBiometricsForCashWithdrawal)
//                                    //{
//                                    //SendCustomerDetailsToAwaitVerification();

//                                    //if (!(await
//                                    //WaitCustomerVerification())) return;
//                                    //}

//                                    var withinLimitsJournal = await _channelService.AddJournalWithCustomerAccountAndTariffsAsync(transactionModel, tariffs, GetServiceHeader());


//                                    transactionModel.CustomerAccount.NewAvailableBalance = transactionModel.CustomerAccount.AvailableBalance + transactionModel.TotalValue;
//                                    var updateWithinLimitResult = await _channelService.UpdateCustomerAccountAsync(SelectedCustomerAccount, GetServiceHeader());


//                                    if (updateWithinLimitResult)
//                                    {

//                                        //CashWithdrawalRequestDTO withinLimitsCashWithdrawalRequest = new CashWithdrawalRequestDTO();

//                                        //withinLimitsCashWithdrawalRequest.Amount = transactionModel.TotalValue;
//                                        //withinLimitsCashWithdrawalRequest.BranchId = transactionModel.BranchId;
//                                        //withinLimitsCashWithdrawalRequest.AuthorizedDate = DateTime.Today;
//                                        //withinLimitsCashWithdrawalRequest.Status = (int)CustomerTransactionAuthOption.Authorize;
//                                        //withinLimitsCashWithdrawalRequest.AuthorizedBy = SelectedTeller.Description;


//                                        // _channelService.AddCashWithdrawalRequestAsync(withinLimitsCashWithdrawalRequest, GetServiceHeader());



//                                        string message = $"Operation success: Customer's new balance is {transactionModel.CustomerAccount.NewAvailableBalance}";

//                                        string cashWithdrawalTextTemplate1 = "Dear customer, your account has been debited with KES {0} at {1} Branch {2}.";
//                                        await SendTextNotificationAsync(cashWithdrawalTextTemplate1, SelectedCustomer, SelectedCustomerAccount, transactionModel.TotalValue, transactionModel.Reference, transactionModel.PrimaryDescription);

//                                        return new OperationResult
//                                        {
//                                            Success = true,
//                                            Dialog = false,
//                                            Message = message,
//                                            TransactionJournal = new JournalDTO
//                                            {

//                                                Id = withinLimitsJournal.Id,
//                                                SequentialId = withinLimitsJournal.SequentialId,
//                                                BranchDescription = withinLimitsJournal.BranchDescription,
//                                                PrimaryDescription = withinLimitsJournal.PrimaryDescription,
//                                                SecondaryDescription = withinLimitsJournal.SecondaryDescription,
//                                                PostingPeriodDescription = withinLimitsJournal.PostingPeriodDescription,
//                                                ApplicationUserName = withinLimitsJournal.ApplicationUserName,
//                                                CreatedDate = withinLimitsJournal.CreatedDate,
//                                                TotalValue = withinLimitsJournal.TotalValue,
//                                                Reference = withinLimitsJournal.Reference
//                                            }

//                                        };

//                                    }

//                                    else
//                                    {
//                                        return new OperationResult
//                                        {

//                                            Success = false,
//                                            Dialog = false,
//                                            Message = "Sorry, but the authorized cash deposit request could not be marked as posted!",

//                                        };
//                                    }

//                                case CashWithdrawalCategory.Overdraw:



//                                    //ResetView();

//                                    return new OperationResult
//                                    {
//                                        Success = false,
//                                        Message = "Sorry, but the customer's account will be overdrawn!"
//                                    };


//                                //break;

//                                default:
//                                    break;
//                            }
//                        }

//                        break;

//                    case FrontOfficeTransactionType.ChequeDeposit:

//                        var NewExternalCheque = transactionModel.ChequeDeposit;
//                        NewExternalCheque.TellerId = SelectedTeller.Id;

//                        NewExternalCheque.CustomerAccountId = SelectedCustomerAccount.Id;

//                        NewExternalCheque.ValidateAll();

//                        if (NewExternalCheque.HasErrors)
//                        {

//                            string message = string.Join(Environment.NewLine, NewExternalCheque.ErrorMessages);
//                            //string message = NewExternalCheque.ErrorMessages[0];
//                            //MessageBox.Show(message, "ChequeDeposit Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
//                            //_messageService.ShowExclamation(string.Join(Environment.NewLine, NewExternalCheque.ErrorMessages), this.DisplayName);

//                            return new OperationResult
//                            {
//                                Success = false,
//                                Dialog = false,
//                                Message = message
//                            };
//                            //ResetView

//                        }
//                        else
//                        {
//                            transactionModel.PrimaryDescription = string.Format("{0} - {1}", transactionModel.PrimaryDescription, NewExternalCheque.Number);

//                            var externalChequeResult = await _channelService.AddExternalChequeAsync(NewExternalCheque, GetServiceHeader());


//                            if (externalChequeResult != null)
//                            {

//                                var ExternalChequePayables = new List<ExternalChequePayableDTO>();


//                                foreach (Guid payableId in transactionModel.ChequePayableCustomerAccountIds)
//                                {


//                                    var externalChequePayable = new ExternalChequePayableDTO
//                                    {
//                                        ExternalChequeId = externalChequeResult.Id,
//                                        ExternalChequeNumber = externalChequeResult.Number,
//                                        CustomerAccountId = payableId
//                                    };

//                                    ExternalChequePayables.Add(externalChequePayable);
//                                }




//                                if (ExternalChequePayables != null)
//                                    await _channelService.UpdateExternalChequePayablesByExternalChequeIdAsync(externalChequeResult.Id, new ObservableCollection<ExternalChequePayableDTO>(ExternalChequePayables), GetServiceHeader());

//                            }

//                            var chequeDepositJournal = await _channelService.AddJournalWithCustomerAccountAndTariffsAsync(transactionModel, tariffs, GetServiceHeader());


//                            if (chequeDepositJournal != null && !chequeDepositJournal.HasErrors)
//                            {


//                                #region Send Text Notification

//                                if (!string.IsNullOrWhiteSpace(SelectedCustomer.AddressMobileLine) &&
//                           Regex.IsMatch(SelectedCustomer.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") &&
//                           SelectedCustomer.AddressMobileLine.Length >= 13)
//                                {

//                                    var smsBody = new StringBuilder();
//                                    smsBody.AppendFormat(
//                                        "Dear customer, {0} of {1} has been effected on your fosa account at {2} at Branch {3}",
//                                        transactionModel.Reference,
//                                        transactionModel.TotalValue,
//                                        SelectedCustomerAccount.BranchDescription,
//                                        SelectedCustomerAccount.BranchCompanyDescription,
//                                        DateTime.Now.ToString("MMMM dd, yyyy")
//                                    );


//                                    var textAlertDTO = new TextAlertDTO
//                                    {
//                                        BranchId = SelectedCustomerAccount.BranchId,
//                                        TextMessageOrigin = (int)MessageOrigin.Within,
//                                        TextMessageRecipient = SelectedCustomer.AddressMobileLine,
//                                        TextMessageBody = smsBody.ToString(),
//                                        MessageCategory = (int)MessageCategory.SMSAlert,
//                                        AppendSignature = false,
//                                        TextMessagePriority = (int)QueuePriority.Highest,
//                                    };


//                                    var textAlertDTOs = new ObservableCollection<TextAlertDTO> { textAlertDTO };


//                                    await _channelService.AddTextAlertsAsync(textAlertDTOs, GetServiceHeader());
//                                }

//                                #endregion

//                                SelectedCustomerAccount = await _channelService.FindCustomerAccountAsync(SelectedCustomerAccount.Id, includeBalances, includeProductDescription, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts, GetServiceHeader());


//                                var updatedTeller = await GetCurrentTeller();
//                                string successmessage = $"Customer new balance is {SelectedCustomerAccount.AvailableBalance} and Teller's new balance is {updatedTeller.BookBalance}";


//                                return new OperationResult
//                                {
//                                    Success = true,
//                                    Dialog = false,
//                                    Message = successmessage,
//                                    TransactionJournal = new JournalDTO
//                                    {

//                                        Id = chequeDepositJournal.Id,
//                                        SequentialId = chequeDepositJournal.SequentialId,
//                                        BranchDescription = chequeDepositJournal.BranchDescription,
//                                        PrimaryDescription = chequeDepositJournal.PrimaryDescription,
//                                        SecondaryDescription = chequeDepositJournal.SecondaryDescription,
//                                        PostingPeriodDescription = chequeDepositJournal.PostingPeriodDescription,
//                                        ApplicationUserName = chequeDepositJournal.ApplicationUserName,
//                                        CreatedDate = chequeDepositJournal.CreatedDate,
//                                        TotalValue = chequeDepositJournal.TotalValue,
//                                        Reference = chequeDepositJournal.Reference
//                                    }

//                                };
//                            }

//                            else
//                            {

//                                return new OperationResult
//                                {

//                                    Success = false,
//                                    Dialog = false,
//                                    Message = "Operation failed"
//                                };

//                            }
//                        }
//                    default:



//                        return new OperationResult
//                        {

//                            Success = false,
//                            Dialog = false,
//                            Message = "You may have entered the wrong transaction typ"
//                        };

//                }
//            }
//            catch (Exception ex)
//            {
//                return new OperationResult
//                {

//                    Success = false,
//                    Dialog = false,
//                    Message = $"An error occurred: {ex.Message}"
//                };

//            }

//            return new OperationResult
//            {

//                Success = false,
//                Dialog = false,
//                Message = "Operation failed. Please try again"
//            };


//        }


//        FrontOfficeTransactionType _frontOfficeTransactionType;
//        public FrontOfficeTransactionType FrontOfficeTransactionType
//        {
//            get { return _frontOfficeTransactionType; }
//            set
//            {
//                if (_frontOfficeTransactionType != value)
//                {
//                    _frontOfficeTransactionType = value;

//                }
//            }
//        }



//        public static async Task SendTextNotificationAsync(string MessageTemplate, CustomerDTO Recipient, CustomerAccountDTO RecipientAccount, decimal Amount, string Reference, string PrimaryDescription)
//        {

//            if (!string.IsNullOrWhiteSpace(Recipient.AddressMobileLine) &&
//                         Regex.IsMatch(Recipient.AddressMobileLine, @"^\+(?:[0-9]??){6,14}[0-9]$") &&
//                         Recipient.AddressMobileLine.Length >= 13)
//            {
//                // Build the SMS body message
//                var smsBody = new StringBuilder();
//                smsBody.AppendFormat(
//                    MessageTemplate,
//                    Amount,
//                    RecipientAccount.BranchDescription,
//                    RecipientAccount.BranchCompanyDescription,
//                    DateTime.Now.ToString("MMMM dd, yyyy"),
//                    Reference,
//                    PrimaryDescription
//                );


//                var textAlertDTO = new TextAlertDTO
//                {
//                    BranchId = RecipientAccount.BranchId,
//                    TextMessageOrigin = (int)MessageOrigin.Within,
//                    TextMessageRecipient = Recipient.AddressMobileLine,
//                    TextMessageBody = smsBody.ToString(),
//                    MessageCategory = (int)MessageCategory.SMSAlert,
//                    AppendSignature = false,
//                    TextMessagePriority = (int)QueuePriority.Highest,
//                };


//                var textAlertDTOs = new ObservableCollection<TextAlertDTO> { textAlertDTO };

//                var masterController = new MasterController();

//                await masterController._channelService.AddTextAlertsAsync(textAlertDTOs, masterController.GetServiceHeader());
//            }


//        }



//    }
//}