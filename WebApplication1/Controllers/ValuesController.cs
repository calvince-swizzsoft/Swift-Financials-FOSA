using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

//using System.Web.Mvc;

namespace WebApplication1.Controllers
{

    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[AllowAnonymous]
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        //private readonly MasterController master;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;

        EmployeeDTO _selectedEmployee;

        CustomerAccountDTO _selectedCustomerAccount;

        BranchDTO _selectedBranch;

        TellerDTO _selectedTeller;

        PaymentVoucherDTO _selectedPaymentVoucher;

        CustomerDTO _selectedCustomer;

        PostingPeriodDTO _currentPostingPeriod;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly IBankLinkageAppService _bankLinkageAppService;

        private readonly IBankAppService _bankAppService;

        private readonly ISystemGeneralLedgerAccountMappingAppService _systemGeneralLedgerAccountMappingService;

        private readonly ICustomerAppService _customerAppService;

        private readonly ICustomerAccountAppService _customerAccountAppService;

        private readonly IPurchaseInvoiceAppService _purchaseInvoiceAppService;

        private readonly IPurchaseCreditMemoAppService _purchaseCreditMemoAppService;

        private readonly ISalesInvoiceAppService _salesInvoiceAppService;

        private readonly ISalesCreditMemoAppService _salesCreditMemoAppService;

        private readonly IJournalEntryAppService _journalEntryAppService;

        private readonly IJournalAppService _journalAppService;

        private readonly IPostingPeriodAppService _postingPeriodAppService;

        private readonly IBranchAppService _branchAppService;

        private readonly IInvestmentProductAppService _investmentProductAppService;

        private readonly ISavingsProductAppService _savingsProductAppService;

        private readonly ILoanProductAppService _loanProductAppService;

        private readonly IAdministrativeDivisionAppService _administrativeDivisionAppService;

        private readonly ICostCenterAppService _costCenterAppService;

        private readonly IDebitTypeAppService _debitTypeAppService;

        private readonly ICreditTypeAppService _creditTypeAppService;

        private readonly IJournalVoucherAppService _journalVoucherAppService;

        private readonly ICreditBatchAppService _creditBatchAppService;

        private readonly IGeneralLedgerAppService _generalLedgerAppService;

        private readonly IPaymentAppService _paymentAppService;

        private readonly IARCustomerAppService _arcustomerAppService;


        private readonly IReceiptAppService _receiptAppService;

        private readonly IInterAccountTransferBatchAppService _interAccountTransferBatchAppService;



        
        public ValuesController(IChartOfAccountAppService chartOfAccountAppService,
            IBankLinkageAppService bankLinkageAppService,
            IBankAppService bankAppService,
            ISystemGeneralLedgerAccountMappingAppService systemGeneralLedgerAccountMappingAppService,
            ICustomerAppService customerAppService,
            ICustomerAccountAppService customerAccountAppService,
            ISalesCreditMemoAppService salesCreditMemoAppService,
            IPurchaseCreditMemoAppService purchaseCreditMemoAppService,
            IPurchaseInvoiceAppService purchaseInvoiceAppService,
            ISalesInvoiceAppService salesInvoiceAppService,
            IJournalAppService journalAppService,
            IJournalEntryAppService journalEntryAppService,
            IPostingPeriodAppService postingPeriodAppService,
            IBranchAppService branchAppService,
            ISavingsProductAppService savingsProductAppService,
            IInvestmentProductAppService investmentProductAppService,
            ILoanProductAppService loanProductAppService,
            ICostCenterAppService costCenterAppService,
            IAdministrativeDivisionAppService administrativeDivisionAppService,
            IDebitTypeAppService debitTypeAppService,
            ICreditTypeAppService creditTypeAppService,
            IJournalVoucherAppService journalVoucherAppService,
            ICreditBatchAppService creditBatchAppService,
            IGeneralLedgerAppService generalLedgerAppService,
            IPaymentAppService paymentAppService,
            IARCustomerAppService arcustomerAppService,
            IReceiptAppService receiptAppService,
            IInterAccountTransferBatchAppService interAccountTransferBatchAppService
            )
        {
            _chartOfAccountAppService = chartOfAccountAppService;
            _bankAppService = bankAppService;
            _bankLinkageAppService = bankLinkageAppService;
            _systemGeneralLedgerAccountMappingService = systemGeneralLedgerAccountMappingAppService;
            _customerAppService = customerAppService;
            _customerAccountAppService = customerAccountAppService;
            _purchaseInvoiceAppService = purchaseInvoiceAppService;
            _salesInvoiceAppService = salesInvoiceAppService;
            _salesCreditMemoAppService = salesCreditMemoAppService;
            _purchaseCreditMemoAppService = purchaseCreditMemoAppService;
            _journalAppService = journalAppService;
            _journalEntryAppService = journalEntryAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _branchAppService = branchAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
            _costCenterAppService = costCenterAppService;
            _debitTypeAppService = debitTypeAppService;
            _creditTypeAppService = creditTypeAppService;
            _journalVoucherAppService = journalVoucherAppService;
            _creditBatchAppService = creditBatchAppService;
            _generalLedgerAppService = generalLedgerAppService;
            _paymentAppService = paymentAppService;
            _arcustomerAppService = arcustomerAppService;
            _receiptAppService = receiptAppService;
            _interAccountTransferBatchAppService = interAccountTransferBatchAppService;
        }

        //public ValuesController()
        //{

        //}

        //private readonly string _connectionString;






        private string receiptContent;
        decimal PreviousTellerBalance;
        decimal NewTellerBalance;
        private PageCollectionInfo<GeneralLedgerTransaction> TellerStatements;



        private bool IsBusy { get; set; } // Property to indicate if an operation is in progress

        //public CashDepositController()
        //{
        //    // Get connection string from Web.config
        //    _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        //}

        public class OperationResult
        {
            public bool Success { get; set; }

            public bool Dialog { get; set; }
            public string Message { get; set; }

            public CustomerTransactionModel TransactionData { get; set; }

            public JournalDTO TransactionJournal { get; set; }
        }

        public PostingPeriodDTO CurrentPostingPeriod
        {

            get { return _currentPostingPeriod; }

            set
            {
                if (_currentPostingPeriod != value)
                {
                    _currentPostingPeriod = value;
                }
            }
        }

        public CustomerDTO SelectedCustomer

        {

            get { return _selectedCustomer; }

            set
            {
                if (_selectedCustomer != value)
                {

                    _selectedCustomer = value;
                }
            }
        }



        public PaymentVoucherDTO SelectedPaymentVoucher
        {
            get { return _selectedPaymentVoucher; }

            set
            {
                if (_selectedPaymentVoucher != value)
                {

                    _selectedPaymentVoucher = value;
                }
            }

        }

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                if (_selectedEmployee != value)
                {
                    _selectedEmployee = value;

                }
            }
        }


        public CustomerAccountDTO SelectedCustomerAccount
        {
            get { return _selectedCustomerAccount; }
            set
            {
                if (_selectedCustomerAccount != value)
                {
                    _selectedCustomerAccount = value;

                }
            }
        }

        public BranchDTO SelectedBranch
        {
            get { return _selectedBranch; }
            set
            {
                if (_selectedBranch != value)
                {
                    _selectedBranch = value;

                }
            }
        }

        public TellerDTO SelectedTeller
        {
            get { return _selectedTeller; }
            set
            {
                if (_selectedTeller != value)
                {
                    _selectedTeller = value;

                }
            }
        }

        //public ValuesController()
        //{
        //    master = new MasterController();
        //}


        //[HttpGet]
        //[Route("GetCustomerJournal/{mobileNumber}")]
        //public IHttpActionResult GetCustomerJournal(string mobileNumber)
        //{
        //    // Keep only digits
        //    mobileNumber = new string(mobileNumber.Where(char.IsDigit).ToArray());

        //    // Convert local format to international (+254...)
        //    if (mobileNumber.StartsWith("0"))
        //    {
        //        mobileNumber = "+254" + mobileNumber.Substring(1);
        //    }

        //    CustomerJournalDTO result = null;

        //    string query = @"
        //SELECT 
        //    c.Individual_FirstName + ' ' + c.Individual_LastName AS FullName,
        //    c.Address_MobileLine,
        //    c.Address_Email,
        //    ca.Status,
        //    SUM(je.Amount) AS TotalAmount
        //FROM swiftfin_Customers c
        //INNER JOIN swiftfin_CustomerAccounts ca
        //    ON ca.CustomerId = c.Id
        //INNER JOIN swiftFin_JournalEntries je
        //    ON je.CustomerAccountId = ca.Id
        //WHERE c.Address_MobileLine = @MobileNumber
        //GROUP BY 
        //    c.Individual_FirstName, 
        //    c.Individual_LastName, 
        //    c.Address_MobileLine,
        //    c.Address_Email,
        //    ca.Status;";

        //    using (var conn = new SqlConnection(_connectionString))
        //    using (var cmd = new SqlCommand(query, conn))
        //    {
        //        cmd.Parameters.Add("@MobileNumber", SqlDbType.VarChar).Value = mobileNumber;

        //        conn.Open();
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                result = new CustomerJournalDTO
        //                {
        //                    FullName = reader["FullName"].ToString(),
        //                    Address_MobileLine = reader["Address_MobileLine"].ToString(),
        //                    Address_Email = reader["Address_Email"].ToString(),
        //                    Status = reader["Status"].ToString(),
        //                    Amount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
        //                };
        //            }
        //        }
        //    }

        //    // Return wrapped JSON response
        //    return Json(new ApiResponse<object>
        //    {
        //        Success = result != null,
        //        Message = result != null
        //            ? "Customer journal retrieved successfully."
        //            : "No records found for the given mobile number.",
        //        Data = result
        //    });
        //}



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetChartOfAccount")]
        public IHttpActionResult Get()
        {
            try
            {
                var serviceHeader = CreateServiceHeader();
               
                var chartOfAccountDTOs = _chartOfAccountAppService.FindChartOfAccounts(serviceHeader);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = chartOfAccountDTOs?.Count > 0 ? $"{chartOfAccountDTOs.Count} chart Of AccountDTOs retrieved." : "No chartOfAccountDTOs  found.",
                    Data = chartOfAccountDTOs
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving chartOfAccountDTOs.",
                    Data = ex.Message
                });
            }
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("UpdateChartOfAccount")]
        public async Task<IHttpActionResult> UpdateChartOfAccount([FromBody] ChartOfAccountDTO chartOfAccountDTO)
        {
            try
            {
                if (chartOfAccountDTO == null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Chart of account data is required.",
                        Data = null
                    });
                }

                // Validate the DTO using DataAnnotations
                var validationContext = new ValidationContext(chartOfAccountDTO);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(chartOfAccountDTO, validationContext, validationResults, true);

                if (!isValid)
                {
                    var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = errors
                    });
                }

                var serviceHeader = CreateServiceHeader();

                var result = _chartOfAccountAppService.UpdateChartOfAccount(chartOfAccountDTO, serviceHeader);
          
                return Json(new ApiResponse<object>
                {
                    Success = result,
                    Message = result ? "Chart of account updated successfully." : "Failed to update chart of account.",
                    Data = result ? chartOfAccountDTO : null
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating chart of account.",
                    Data = ex.Message
                });
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getBanks")]
        public IHttpActionResult getBanks()
        {
            try
            {
               

                var serviceHeader = CreateServiceHeader();

     
                //var pageCollectionInfo = await master._channelService.FindBanksInPageAsync((int)effectivePageSize, (int)effectivePageIndex, serviceHeader);
               
                var listOfBanks = _bankAppService.FindBanks(serviceHeader);
                //var sortedData = pageCollectionInfo.PageCollection.OrderByDescending(x => x.Id).ToList();

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = listOfBanks?.Count > 0 ? $"{listOfBanks.Count} Transactions Found." : "No Transaction  found.",
                    Data = listOfBanks
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetSystemMapItems")]
        public IHttpActionResult GetSystemMapItems()
        {

            try
            {

                var serviceHeader = CreateServiceHeader();

                var mapItems = GetSystemGeneralLedgerAccountCodeSelectList("Account Payables");

                return Json(mapItems);
            }

            catch (Exception ex)
            {

                return Json(ex.Message);


            }


        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetPaymentAccountTypeSelectList")]
        public async Task<IHttpActionResult> GetPaymentAccountTypes()
        {

            try
            {

                var serviceHeader = CreateServiceHeader();

            
                var accountTypes = GetPaymentAccountTypeSelectList("Vendor");

                return Json(accountTypes);
            }

            catch (Exception ex)
            {

                return Json(ex.Message);


            }


        }



        [HttpGet]
        [Route("GetPaymentDocumentTypeSelectList")]
        public IHttpActionResult GetPaymentDocumentTypes()
        {

            try
            {

                var serviceHeader = CreateServiceHeader();

                var accountTypes = GetPaymentDocumentTypeSelectList("Invoice");

                return Json(accountTypes);
            }

            catch (Exception ex)
            {

                return Json(ex.Message);


            }


        }





        [HttpGet]
        [Route("GetPurchaseInvoiceEntryTypes")]
        public async Task<IHttpActionResult> GetPurchaseInvoiceTypes()
        {

            try
            {

                var serviceHeader = CreateServiceHeader();

                var items = GetPurchaseInvoiceEntryTypeSelectList("G/L Account");
                return Json(items);
            }

            catch (Exception ex)
            {

                return Json(ex.Message);


            }


        }



        [HttpGet]
        [Route("getSystemMappings")]
        public async Task<IHttpActionResult> getSystemMappings()
        {

            try
            {

                var serviceHeader = CreateServiceHeader();

                var mappings = _chartOfAccountAppService.FindSystemGeneralLedgerAccountMappings(serviceHeader);


                var coas = _chartOfAccountAppService.FindChartOfAccounts(serviceHeader);
              

                foreach (SystemGeneralLedgerAccountMappingDTO mapping in mappings)
                {
                    var coa = coas.FirstOrDefault(c => c.Id == mapping.ChartOfAccountId);

                    if (coa != null)
                    {
                        mapping.ChartOfAccountAccountName = coa.AccountName;
                    }
                    // else: do nothing if not found
                }



                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = mappings?.Count > 0 ? $"{mappings.Count} Mappings Found." : "No Mapping  found.",
                    Data = mappings?.ToList()
                });

            }

            catch (Exception ex)
            {


                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                });
            }
        }


        [HttpPost]
        [Route("addSystemMapping")]
        public async Task<IHttpActionResult> addSystemMapping([FromBody] SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO)
        {

            try
            {

                var serviceHeader = CreateServiceHeader();


                systemGeneralLedgerAccountMappingDTO.ValidateAll();

                var result = _systemGeneralLedgerAccountMappingService.AddNewSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO, serviceHeader);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Mapping created successfully.",
                    Data = result
                });

            }

            catch (Exception ex)
            {



                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }


        [HttpPut]
        [Route("UpdateSystemMapping")]
        public async Task<IHttpActionResult> UpdateSystemMapping([FromBody] SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                systemGeneralLedgerAccountMappingDTO.ValidateAll();

                if (systemGeneralLedgerAccountMappingDTO.ErrorMessages.Count > 0)
                {

                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "failed, bad request"
                    });
                }

                else
                {


                    var result = _systemGeneralLedgerAccountMappingService.UpdateSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO, serviceHeader);


                    if (result)
                    {

                        return Json(new ApiResponse<object>
                        {
                            Success = true,
                            Message = "Success updating"

                        });
                    }


                    else
                    {

                        return Json(new ApiResponse<object>
                        {

                            Success = true,
                            Message = "Failed in updating"
                        });
                    }
                }

            }

            catch (Exception ex)
            {

                return Json(new ApiResponse<object>
                {

                    Success = false,
                    Message = "An error occurred"
                });
            }
        }




        [HttpGet]
        [Route("getBankWithLinkages")]
        public async Task<IHttpActionResult> getBankWithLinkages()
        {
            try
            {

                var serviceHeader = CreateServiceHeader();

                var bankLinkageDTOs = _bankLinkageAppService.FindBankLinkages(serviceHeader);

                var generalLedgerAccounts = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader, true);
         
                var balanceDict = generalLedgerAccounts
    .Where(x => x.Id != Guid.Empty)
    .GroupBy(x => x.Id)
    .ToDictionary(g => g.Key, g => g.First().Balance);

                foreach (var linkageDTO in bankLinkageDTOs)
                {


                    var relatedBank = _bankAppService.FindBank(linkageDTO.BankId, serviceHeader);
                    
                    if (relatedBank != null && relatedBank.Id != Guid.Empty)
                    {

                        linkageDTO.SwiftCode = relatedBank.SwiftCode;
                        linkageDTO.Address = relatedBank.Address;
                        linkageDTO.City = relatedBank.City;
                        linkageDTO.IbanNo = relatedBank.IbanNo;
                        linkageDTO.No = relatedBank.No;

                    }



                    if (linkageDTO.ChartOfAccountId != Guid.Empty && balanceDict.ContainsKey(linkageDTO.ChartOfAccountId))
                    {
                        linkageDTO.BankLinkageBalance = balanceDict[linkageDTO.ChartOfAccountId];
                    }
                    else
                    {
                        linkageDTO.BankLinkageBalance = 0m;
                    }
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = bankLinkageDTOs?.Count > 0 ? $"{bankLinkageDTOs.Count} Transactions Found." : "No Transaction  found.",
                    Data = bankLinkageDTOs
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("AddBankWithLinkages")]
        public async Task<IHttpActionResult> AddBankWithLinkages([FromBody] BankDTO bankDTO)
        {


            var serviceHeader = CreateServiceHeader();

            if (bankDTO == null || bankDTO == null)
                return Json(new ApiResponse<object> { Success = false, Message = "Invalid data.", Data = null });


            bankDTO.ValidateAll();
            if (bankDTO.HasErrors)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Bank validation failed.",
                    Data = bankDTO.ErrorMessages
                });
            }

            var result = _bankAppService.AddNewBank(bankDTO, serviceHeader);

            if (result.ErrorMessageResult != null)
            {

                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.ErrorMessageResult,
                    Data = null
                });
            }



            _bankAppService.UpdateBankBranches(result.Id, bankDTO.BankBranchesDTO.ToList(), serviceHeader);

           // await master._channelService.UpdateBankBranchesByBankIdAsync(result.Id, bankDTO.BankBranchesDTO, serviceHeader);


            var bankLinkageDTO = new BankLinkageDTO
            {
                BankAccountNumber = bankDTO.BankAccountNumber,
                BankId = result.Id,
                BankBranchName = bankDTO.BankBranchName,
                BankName = bankDTO.BankName,
                BranchDescription = bankDTO.BranchDescription,
                BranchId = bankDTO.BranchId,
                ChartOfAccountAccountCode = bankDTO.ChartOfAccountAccountCode,
                ChartOfAccountAccountName = bankDTO.ChartOfAccountAccountName,
                ChartOfAccountId = bankDTO.ChartOfAccountId,
                ChartOfAccountAccountType = bankDTO.ChartOfAccountAccountType,
                ChartOfAccountCostCenterId = bankDTO.ChartOfAccountCostCenterId,
                ChartOfAccountCostCenterDescription = bankDTO.ChartOfAccountCostCenterDescription
            };


            var linkageResult = _bankLinkageAppService.AddNewBankLinkage(bankLinkageDTO, serviceHeader);
            if (linkageResult.ErrorMessages.Count != 0)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = linkageResult.ErrorMessages.ToString(),
                    Data = null
                });
            }


            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Bank and linkages created successfully.",
                Data = bankDTO
            });
        }



        [HttpGet]
        [Route("GetGeneralLeadgersBalances")]
        public async Task<IHttpActionResult> GetGeneralLeadgers()
        {
            try
            {

                var serviceHeader = CreateServiceHeader();

                var chartOfAccountDTOs = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader, true);
              
                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = chartOfAccountDTOs?.Count > 0 ? $"{chartOfAccountDTOs.Count} Transactions Found." : "No Transaction  found.",
                    Data = chartOfAccountDTOs
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("GetGeneralLedgers")]
        public async Task<IHttpActionResult> GetGeneralLedgers([FromUri] string text = null, int? accountCategory = null, bool updateDepth = false)
        {
            try
            {

                var serviceHeader = CreateServiceHeader();

                var gls = _chartOfAccountAppService.FindGeneralLedgerAccountsWithCategoryAndText(text, accountCategory, serviceHeader, false);

                

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = gls != null && gls.Count > 0
                        ? $"{gls.Count} Accounts Found."
                        : "No accounts found.",
                    Data = gls
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the accounts.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("get-chart-of-accounts")]
        public async Task<HttpResponseMessage> GetChartOfAccounts()
        {
            try
            {
                var chartOfAccounts = new List<ChartOfAccountListDTO>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                SELECT 
                    Id,
                    ParentId,
                    CostCenterId,
                    AccountType,
                    AccountCategory,
                    AccountCode,
                    AccountName,
                    Depth,
                    IsControlAccount,
                    IsReconciliationAccount,
                    PostAutomaticallyOnly,
                    IsLocked,
                    SequentialId,
                    CreatedBy,
                    CreatedDate
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_ChartOfAccounts]
                ORDER BY AccountCode";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var account = new ChartOfAccountListDTO
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ParentId = reader["ParentId"] == DBNull.Value ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("ParentId")),
                                CostCenterId = reader["CostCenterId"] == DBNull.Value ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("CostCenterId")),
                                AccountType = Convert.ToInt32(reader["AccountType"]),
                                AccountCategory = Convert.ToInt32(reader["AccountCategory"]),
                                AccountCode = reader["AccountCode"]?.ToString() ?? "",
                                AccountName = reader["AccountName"]?.ToString() ?? "",
                                Depth = Convert.ToInt32(reader["Depth"]),
                                IsControlAccount = Convert.ToBoolean(reader["IsControlAccount"]),
                                IsReconciliationAccount = Convert.ToBoolean(reader["IsReconciliationAccount"]),
                                PostAutomaticallyOnly = Convert.ToBoolean(reader["PostAutomaticallyOnly"]),
                                IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                                SequentialId = reader["SequentialId"] == DBNull.Value ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("SequentialId")),
                                CreatedBy = reader["CreatedBy"]?.ToString() ?? "",
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };

                            chartOfAccounts.Add(account);
                        }
                    }
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Message = $"{chartOfAccounts.Count} record(s) retrieved successfully",
                        Data = chartOfAccounts,
                        TotalCount = chartOfAccounts.Count
                    }),
                    Encoding.UTF8,
                    "application/json");
                return response;
            }
            catch (SqlException ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "Database error occurred", ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "An error occurred", ex.Message);
            }
        }

        // DTO Class with unique name to avoid conflicts
        public class ChartOfAccountListDTO
        {
            public Guid Id { get; set; }
            public Guid? ParentId { get; set; }
            public Guid? CostCenterId { get; set; }
            public int AccountType { get; set; }
            public int AccountCategory { get; set; }
            public string AccountCode { get; set; }
            public string AccountName { get; set; }
            public int Depth { get; set; }
            public bool IsControlAccount { get; set; }
            public bool IsReconciliationAccount { get; set; }
            public bool PostAutomaticallyOnly { get; set; }
            public bool IsLocked { get; set; }
            public Guid? SequentialId { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedDate { get; set; }
        }





        [HttpGet]
        [Route("GetGeneralLeadgers")]
        public async Task<IHttpActionResult> GetGeneralLeadgers(int? pagesize, int? pageindex)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                var gls = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader);
                
                //var pageCollectionInfo = await master._channelService.FindGeneralLedgersInPageAsync((int)pagesize, (int)pageindex, serviceHeader);
                var sortedData = gls.OrderByDescending(x => x.Id).ToList();

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = sortedData?.Count > 0 ? $"{sortedData.Count} Transactions Found." : "No Transaction  found.",
                    Data = sortedData
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("test")]
        public IHttpActionResult Test()
        {
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Test GET endpoint is working!",
                Data = DateTime.UtcNow
            });
        }

        [HttpPost]
        [Route("chartofaccount")]
        public async Task<IHttpActionResult> CreateChartOfAccount([FromBody] ChartOfAccountDTO chartOfAccountDTO)
        {

            var serviceHeader = CreateServiceHeader();
           
            if (chartOfAccountDTO == null)
                return Json(new ApiResponse<object> { Success = false, Message = "Invalid data.", Data = null });

            chartOfAccountDTO.CostCenterId = Guid.Parse("A66AE3A0-AE25-F011-8982-28C63F4EECBE");


            chartOfAccountDTO.ValidateAll();

            if (!chartOfAccountDTO.HasErrors)
            {

                var result = _chartOfAccountAppService.AddNewChartOfAccount(chartOfAccountDTO, serviceHeader);
               
                if (result.ErrorMessageResult != null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = result.ErrorMessageResult,
                        Data = null
                    });
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Chart of account created successfully.",
                    Data = chartOfAccountDTO
                });
            }

            return Json(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed.",
                Data = chartOfAccountDTO.ErrorMessages
            });
        }

        [HttpGet]
        [Route("customers")]
        public async Task<IHttpActionResult> GetAllCustomers()
        {

            var serviceHeader = CreateServiceHeader();

            var customers = await _customerAppService.FindCustomersAsync(serviceHeader);

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = customers?.Count > 0 ? $"{customers.Count} customers found." : "No customers found.",
                Data = customers
            });
        }

        [HttpGet]
        [Route("invoicelines")]
        public async Task<IHttpActionResult> GetAllInvoiceLines()
        {
            var serviceHeader = CreateServiceHeader();
            //var customers = await master._channelService.FindCustomersAsync(serviceHeader);

            var invoiceLines = _purchaseInvoiceAppService.FindPurchaseInvoiceLines(serviceHeader);
           // var invoices = await master._channelService.FindPurchaseInvoiceLinesAsync(serviceHeader);

            //var invoices = await master._channelService.

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = invoiceLines?.Count > 0 ? $"{invoiceLines.Count} invoice lines found." : "No invoices found.",
                Data = invoiceLines
            });
        }

        [HttpGet]
        [Route("accounts")]
        public async Task<IHttpActionResult> GetAccounts([FromUri] int pageIndex, [FromUri] int pageSize, [FromUri] int? journalEntryFilter, [FromUri] DateTime? startDate, [FromUri] DateTime? endDate, [FromUri] string text = "")
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

        

                var accounts = await _journalEntryAppService.FindGeneralLedgerTransactionsInPageAsync(pageIndex, pageSize, (DateTime)startDate, (DateTime)endDate, text, (int)journalEntryFilter,serviceHeader);

                //var accounts = await master._channelService.FindGeneralLedgerTransactionsByDateRangeAndFilterInPageAsync(
                //    pageIndex, pageSize, (DateTime)startDate, (DateTime)endDate, text, (int)journalEntryFilter, serviceHeader);

                var sortedData = accounts.PageCollection.OrderByDescending(x => x.JournalCreatedDate).ToList();

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Transactions fetched successfully.",
                    Data = sortedData
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error fetching transactions.",
                    Data = ex.Message
                });
            }
        }


        //Posting Periods
        [HttpGet]
        [Route("GetPostingperiods")]
        public async Task<IHttpActionResult> GetPostingperiods()
        {

            var serviceHeader = CreateServiceHeader();

            var reports = _postingPeriodAppService.FindPostingPeriods(serviceHeader);
         
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = reports?.Count > 0 ? $"{reports.Count} POsting Periods found." : "No Posting periods found.",
                Data = reports
            });
        }



        [Route("AddPostingPeriod")]
        public async Task<IHttpActionResult> addPostingPeriod([FromBody] PostingPeriodDTO postingPeriodDTO)
        {

            var serviceHeader = CreateServiceHeader();

            var result = _postingPeriodAppService.AddNewPostingPeriod(postingPeriodDTO, serviceHeader);
       
          
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Posting period added successfully.",
                Data = result
            });
        }


        [HttpPut]
        [Route("UpdatePostingPeriod")]
        public async Task<IHttpActionResult> UpdatePostingPeriod([FromBody] PostingPeriodDTO postingPeriodDTO)
        {
            var serviceHeader = CreateServiceHeader();

            var result = _postingPeriodAppService.UpdatePostingPeriod(postingPeriodDTO, serviceHeader);
           
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Posting period updated successfully.",
                Data = result
            });
        }



        [HttpPost]
        [Route("ClosePostingPeriod")]
        public async Task<IHttpActionResult> ClosePostingPeriod([FromBody] PostingPeriodDTO postingPeriodDTO)
        {

            int moduleNavigationItemCode = 1; // Assuming 1 represents the Accounts module

            var serviceHeader = CreateServiceHeader();


            var result = _postingPeriodAppService.ClosePostingPeriod(postingPeriodDTO, moduleNavigationItemCode, serviceHeader);
          

            if (result == true)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Successfully closed posting period.",
                    Data = null
                });
            }

            else
            {

                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error closing posting period.",
                    Data = null
                });
            }

        }

        [HttpGet]
        [Route("branches")]
        public async Task<IHttpActionResult> GetBranches()
        {

            var serviceHeader = CreateServiceHeader();

            var branches = _branchAppService.FindBranches(serviceHeader);
         
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = branches?.Count > 0 ? $"{branches.Count} branches found." : "No branches found.",
                Data = branches
            });
        }

        //[HttpGet]
        //[Route("roles")]
        //public async Task<IHttpActionResult> GetRoles()
        //{

        //    var serviceHeader = CreateServiceHeader();

        //    var roles = _

        //    var roles = await master._channelService.GetAllRolesAsync(serviceHeader);

        //    return Json(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = roles?.Count > 0 ? $"{roles.Count} roles found." : "No roles found.",
        //        Data = roles
        //    });
      //  }

        [HttpGet]
        [Route("products")]
        public async Task<IHttpActionResult> GetProducts()
        {
            var serviceHeader = CreateServiceHeader();

            var products = _investmentProductAppService.FindInvestmentProducts(serviceHeader);
           
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = products?.Count > 0 ? $"{products.Count} products found." : "No products found.",
                Data = products
            });
        }

        [HttpGet]
        [Route("costcenters")]
        public async Task<IHttpActionResult> GetCostCenters()
        {

            
            var serviceHeader = CreateServiceHeader();

            var centers = _costCenterAppService.FindCostCenters(serviceHeader);
           
            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = centers?.Count > 0 ? $"{centers.Count} cost centers found." : "No cost centers found.",
                Data = centers
            });
        }

        //[HttpPost]
        //[Route("add-customer")]
        //public async Task<IHttpActionResult> AddCustomer([FromBody] CustomerBindingModel customerBindingModel)
        //{

        //    var serviceHeader = CreateServiceHeader();
         
        //    // Mandatory collections
        //    var mandatoryInvestmentProducts = new List<InvestmentProductDTO>();
        //    var mandatorySavingsProducts = new List<SavingsProductDTO>();
        //    var mandatoryDebitTypes = new ObservableCollection<DebitTypeDTO>();
        //    var mandatoryProducts = new ProductCollectionInfo();

        //    //  Get all savings products and find mandatory one

        //    var savingsDTO = _savingsProductAppService.FindSavingsProducts(serviceHeader);
            
        //    string mandatoryDescription = "M-WALLETACCOUNT";
        //    var savingsProductDTO = savingsDTO.FirstOrDefault(s => string.Equals(s.Description, mandatoryDescription, StringComparison.OrdinalIgnoreCase));

        //    if (savingsProductDTO == null)
        //    {
        //        return Json(new ApiResponse<object>
        //        {
        //            Success = false,
        //            Message = $"Mandatory savings product '{mandatoryDescription}' not found.",
        //            Data = null
        //        });
        //    }

        //    mandatorySavingsProducts.Add(savingsProductDTO);
        //    mandatoryProducts.SavingsProductCollection = mandatorySavingsProducts;

        //    var investmentDTO = _investmentProductAppService.FindInvestmentProducts(serviceHeader);
        //    string investmentDescription = "ENTRANCEFEE";

        //    var invest = investmentDTO.FirstOrDefault(s => string.Equals(s.Description, investmentDescription, StringComparison.OrdinalIgnoreCase));
        //    if (investmentDTO == null)
        //    {
        //        return Json(new ApiResponse<object>
        //        {
        //            Success = false,
        //            Message = "Mandatory investment product not found.",
        //            Data = null
        //        });
        //    }

        //    mandatoryInvestmentProducts.Add(invest);
        //    mandatoryProducts.InvestmentProductCollection = mandatoryInvestmentProducts;

        //    var debitTypeDTO = _debitTypeAppService.FindDebitTypes(serviceHeader);

        //    string debitTypeDTODescription = "Entrance Fees";

        //    var debitType = debitTypeDTO.FirstOrDefault(s => string.Equals(s.Description, debitTypeDTODescription, StringComparison.OrdinalIgnoreCase));
        //    if (debitTypeDTO == null)
        //    {
        //        return Json(new ApiResponse<object>
        //        {
        //            Success = false,
        //            Message = "Mandatory debit type not found.",
        //            Data = null
        //        });
        //    }

        //    mandatoryDebitTypes.Add(debitType);

        //    var customerDTO = customerBindingModel.MapTo<CustomerDTO>();

          
        //    //var customerDTO = new CustomerDTO(customerBindingModel);

        //    var result = _customerAppService.AddNewCustomerAsync(customerDTO, mandatoryDebitTypes.ToList(), mandatoryInvestmentProducts, mandatorySavingsProducts, mandatoryProducts);

           

        //    return Json(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Customer added successfully.",
        //        Data = result
        //    });
        //}


        [HttpPost]
        [Route("add-product")]
        public async Task<IHttpActionResult> AddProduct([FromBody] InvestmentProductDTO product)
        {

            var serviceHeader = CreateServiceHeader();

            var result = _investmentProductAppService.AddNewInvestmentProduct(product, serviceHeader);

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Product added successfully.",
                Data = result
            });
        }

        [HttpPost]
        [Route("add-branch")]
        public async Task<IHttpActionResult> AddBranch([FromBody] BranchDTO branch)
        {

            var serviceHeader = CreateServiceHeader();

            var result = _branchAppService.AddNewBranch(branch, serviceHeader);

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Branch added successfully.",
                Data = result
            });
        }

        [HttpPost]
        [Route("add-costcenter")]
        public async Task<IHttpActionResult> AddCostCenter([FromBody] CostCenterDTO center)
        {

            var serviceHeader = CreateServiceHeader();


            var result = _costCenterAppService.AddNewCostCenter(center, serviceHeader);

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Cost center added successfully.",
                Data = result
            });
        }

        //

        //IF ACCOUNT TYPE IS CUSTOMER, D0 THIS

        //IF ACCOUNT TYPE IS GL, DO THAT

        [HttpPost]
        [Route("PostJournal")]
        public async Task<IHttpActionResult> HandleDirectPosting([FromBody] List<TransactionModel> transactionModels)
        {
            var serviceHeader = CreateServiceHeader();

            if (transactionModels == null || !transactionModels.Any())
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No journal entries provided.",
                    Data = null
                });
            }

            var results = new List<object>();

            // ── PATH 1: Self-balanced (both ChartOfAccountId AND ContraChartOfAccountId provided) ──
            var selfBalanced = transactionModels
                .Where(t => t.ChartOfAccountId != Guid.Empty && t.ContraChartOfAccountId != Guid.Empty)
                .ToList();

            foreach (var transactionModel in selfBalanced)
            {

                // Single entry (self-balanced)
                if (transactionModel.ChartOfAccountId != Guid.Empty && transactionModel.ContraChartOfAccountId != Guid.Empty)
                    if (transactionModel.DebitAmount > 0)
                    {
                        transactionModel.DebitChartOfAccountId = transactionModel.ChartOfAccountId;
                        transactionModel.CreditChartOfAccountId = transactionModel.ContraChartOfAccountId;
                    }
                    else
                    {
                        transactionModel.CreditChartOfAccountId = transactionModel.ChartOfAccountId;
                        transactionModel.DebitChartOfAccountId = transactionModel.ContraChartOfAccountId;
                    }

                var result = _journalAppService.AddNewJournal(
                    null,
                    transactionModel.BranchId,
                    null,
                    transactionModel.TotalValue,
                    transactionModel.PrimaryDescription,
                    transactionModel.SecondaryDescription,
                    transactionModel.Reference,
                    transactionModel.ModuleNavigationItemCode,
                    transactionModel.TransactionCode,
                    transactionModel.ValueDate,
                    transactionModel.CreditChartOfAccountId,
                    transactionModel.DebitChartOfAccountId,
                    serviceHeader
                    );

               // var result = await master._channelService.AddJournalAsync(transactionModel, null, serviceHeader);
                results.Add(result);
            }

            // ── PATH 2: Multi-entry (each model carries only one side) ──
            var multiEntries = transactionModels
                .Where(t => !(t.ChartOfAccountId != Guid.Empty && t.ContraChartOfAccountId != Guid.Empty))
                .Where(t => t.CreditChartOfAccountId != Guid.Empty || t.DebitChartOfAccountId != Guid.Empty)
                .ToList();

            // Identify the two sides
            var debitEntries = multiEntries.Where(t => t.DebitChartOfAccountId != Guid.Empty && t.DebitAmount > 0).ToList();
            var creditEntries = multiEntries.Where(t => t.CreditChartOfAccountId != Guid.Empty && t.CreditAmount > 0).ToList();

            // ✅ KEY FIX: Cross-populate ContraChartOfAccountId BEFORE saving
            // Debit entry's contra = the credit side's account, and vice versa
            foreach (var debitEntry in debitEntries)
            {
                var creditMatch = creditEntries.FirstOrDefault(c => c.CreditAmount == debitEntry.DebitAmount
                                                                 && c.ContraChartOfAccountId == Guid.Empty);
                if (creditMatch != null)
                {
                    debitEntry.ContraChartOfAccountId = creditMatch.CreditChartOfAccountId; // A's contra = B
                    creditMatch.ContraChartOfAccountId = debitEntry.DebitChartOfAccountId;  // B's contra = A
                }
            }

            // Now save — contra is populated on both sides
            foreach (var transactionModel in multiEntries)
            {
                if (transactionModel.CreditChartOfAccountId != Guid.Empty && transactionModel.CreditAmount > 0)
                {
                    transactionModel.JournalType = (int)JournalVoucherType.CreditGLAccount;
                }
                else if (transactionModel.DebitChartOfAccountId != Guid.Empty && transactionModel.DebitAmount > 0)
                {
                    transactionModel.JournalType = (int)JournalVoucherType.DebitGLAccount;
                }

                var result = _journalAppService.AddNewJournalSingleEntry(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.ChartOfAccountId, transactionModel.ContraChartOfAccountId, transactionModel.JournalType, serviceHeader, true);
               
                
                //var result = await master._channelService.AddJournalSingleEntryAsync(transactionModel, null, serviceHeader);
                results.Add(result);
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = "Journal(s) added successfully.",
                Data = results
            });
        }

        [HttpGet]
        [Route("GeneralLedgerTransactions")]
        public async Task<IHttpActionResult> GetGeneralLedgerTransactions(
     Guid chartOfAccountId,
     int pageIndex = 0,
     int pageSize = 20,
     DateTime? fromDate = null,
     DateTime? toDate = null,
     string filter = "",
     int? transactionType = null,
     int? sortOrder = 1)
        {
            try
            {
                if (chartOfAccountId == Guid.Empty)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid chartOfAccountId",
                        Data = null
                    });

                if (pageSize < 1 || pageSize > 500)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "PageSize must be between 1 and 500",
                        Data = null
                    });

                var startDate = fromDate ?? new DateTime(1900, 1, 1);
                var endDate = toDate ?? DateTime.Today;
                int offset = pageIndex * pageSize;
                string orderDirection = (sortOrder ?? 1) == 1 ? "ASC" : "DESC";

                var transactions = new List<object>();
                int totalItems = 0;
                decimal totalCredits = 0;
                decimal totalDebits = 0;
                decimal bookBalanceBF = 0;
                decimal bookBalanceCF = 0;

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // ── Total count ───────────────────────────────────────────
                    using (var countCmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [dbo].[swiftFin_JournalEntries] je
                INNER JOIN [dbo].[swiftFin_Journals] j ON j.[Id] = je.[JournalId]
                WHERE je.[ChartOfAccountId] = @ChartOfAccountId
                  AND je.[ValueDate] >= @FromDate
                  AND je.[ValueDate] <  DATEADD(DAY, 1, @ToDate)
                  AND (@Filter = ''
                       OR j.[PrimaryDescription]   LIKE '%' + @Filter + '%'
                       OR j.[SecondaryDescription] LIKE '%' + @Filter + '%'
                       OR j.[Reference]            LIKE '%' + @Filter + '%')
                  AND (@TransactionType IS NULL
                       OR (@TransactionType = 1 AND je.[Amount] > 0)
                       OR (@TransactionType = 2 AND je.[Amount] < 0))", conn))
                    {
                        countCmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId;
                        countCmd.Parameters.Add("@FromDate", SqlDbType.DateTime2).Value = startDate;
                        countCmd.Parameters.Add("@ToDate", SqlDbType.DateTime2).Value = endDate;
                        countCmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 200).Value = filter ?? "";
                        countCmd.Parameters.Add("@TransactionType", SqlDbType.Int).Value =
                            transactionType.HasValue ? (object)transactionType.Value : DBNull.Value;

                        totalItems = (int)await countCmd.ExecuteScalarAsync();
                    }

                    // ── Totals ────────────────────────────────────────────────
                    using (var totalsCmd = new SqlCommand(@"
                SELECT
                    ISNULL(SUM(CASE WHEN je.[Amount] > 0 THEN  je.[Amount]  ELSE 0 END), 0) AS TotalDebits,
                    ISNULL(SUM(CASE WHEN je.[Amount] < 0 THEN -je.[Amount]  ELSE 0 END), 0) AS TotalCredits
                FROM [dbo].[swiftFin_JournalEntries] je
                WHERE je.[ChartOfAccountId] = @ChartOfAccountId
                  AND je.[ValueDate] >= @FromDate
                  AND je.[ValueDate] <  DATEADD(DAY, 1, @ToDate)", conn))
                    {
                        totalsCmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId;
                        totalsCmd.Parameters.Add("@FromDate", SqlDbType.DateTime2).Value = startDate;
                        totalsCmd.Parameters.Add("@ToDate", SqlDbType.DateTime2).Value = endDate;

                        using (var reader = await totalsCmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                totalDebits = Convert.ToDecimal(reader["TotalDebits"]);
                                totalCredits = Convert.ToDecimal(reader["TotalCredits"]);
                            }
                        }
                    }

                    // ── Book balance brought forward ───────────────────────────
                    using (var bfCmd = new SqlCommand(@"
                SELECT ISNULL(SUM(je.[Amount]), 0)
                FROM [dbo].[swiftFin_JournalEntries] je
                WHERE je.[ChartOfAccountId] = @ChartOfAccountId
                  AND je.[ValueDate] < @FromDate", conn))
                    {
                        bfCmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId;
                        bfCmd.Parameters.Add("@FromDate", SqlDbType.DateTime2).Value = startDate;
                        bookBalanceBF = Convert.ToDecimal(await bfCmd.ExecuteScalarAsync());
                    }

                    bookBalanceCF = bookBalanceBF + totalDebits - totalCredits;

                    // ── Fetch page with running balance ───────────────────────
                    using (var cmd = new SqlCommand($@"
                WITH Filtered AS (
                    SELECT
                        je.[Id],
                        je.[JournalId],
                        je.[ChartOfAccountId],
                        je.[ContraChartOfAccountId],
                        je.[CustomerAccountId],
                        je.[Amount],
                        je.[ValueDate],
                        je.[CreatedBy],
                        je.[CreatedDate],

                        coa.[AccountCode]         AS GLAccountCode,
                        coa.[AccountType]         AS GLAccountType,
                        coa.[AccountName]         AS GLAccountDescription,

                        contraCoa.[AccountCode]   AS ContraGLAccountCode,
                        contraCoa.[AccountType]   AS ContraGLAccountType,
                        contraCoa.[AccountName]   AS ContraGLAccountDescription,

                        j.[ParentId]              AS JournalParentId,
                        j.[BranchId],
                        j.[PrimaryDescription],
                        j.[SecondaryDescription],
                        j.[Reference],
                        j.[ApplicationUserName],
                        j.[EnvironmentUserName],
                        j.[EnvironmentMachineName],
                        j.[EnvironmentDomainName],
                        j.[EnvironmentOSVersion],
                        j.[EnvironmentMACAddress],
                        j.[EnvironmentMotherboardSerialNumber],
                        j.[EnvironmentProcessorId],
                        j.[EnvironmentIPAddress],
                        j.[TransactionCode],
                        j.[IsLocked]              AS JournalIsLocked,
                        j.[ValueDate]             AS JournalValueDate,
                        j.[CreatedDate]           AS JournalCreatedDate,

                        b.[Description]           AS BranchDescription,

                        c.[Individual_FirstName],
                        c.[Individual_LastName],
                        c.[Reference1]            AS CustomerReference1,
                        c.[Reference2]            AS CustomerReference2,
                        c.[Reference3]            AS CustomerReference3,

                        ca.[CustomerAccountType_ProductCode],
                        ca.[CustomerAccountType_TargetProductId],
                        ca.[CustomerAccountType_TargetProductCode],

                        SUM(je.[Amount]) OVER (
                            ORDER BY je.[ValueDate], je.[CreatedDate], je.[Id]
                        ) + @BookBalanceBF        AS RunningBalance

                    FROM [dbo].[swiftFin_JournalEntries] je
                    INNER JOIN [dbo].[swiftFin_Journals]         j         ON j.[Id]   = je.[JournalId]
                    LEFT  JOIN [dbo].[swiftFin_ChartOfAccounts]  coa       ON coa.[Id] = je.[ChartOfAccountId]
                    LEFT  JOIN [dbo].[swiftFin_ChartOfAccounts]  contraCoa ON contraCoa.[Id] = je.[ContraChartOfAccountId]
                    LEFT  JOIN [dbo].[swiftFin_Branches]         b         ON b.[Id]   = j.[BranchId]
                    LEFT  JOIN [dbo].[swiftFin_CustomerAccounts] ca        ON ca.[Id]  = je.[CustomerAccountId]
                    LEFT  JOIN [dbo].[swiftFin_Customers]        c         ON c.[Id]   = ca.[CustomerId]
                    WHERE je.[ChartOfAccountId] = @ChartOfAccountId
                      AND je.[ValueDate] >= @FromDate
                      AND je.[ValueDate] <  DATEADD(DAY, 1, @ToDate)
                      AND (@Filter = ''
                           OR j.[PrimaryDescription]   LIKE '%' + @Filter + '%'
                           OR j.[SecondaryDescription] LIKE '%' + @Filter + '%'
                           OR j.[Reference]            LIKE '%' + @Filter + '%')
                      AND (@TransactionType IS NULL
                           OR (@TransactionType = 1 AND je.[Amount] > 0)
                           OR (@TransactionType = 2 AND je.[Amount] < 0))
                )
                SELECT *
                FROM Filtered
                ORDER BY [ValueDate] {orderDirection}, [JournalCreatedDate] {orderDirection}, [Id] {orderDirection}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn))
                    {
                        cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId;
                        cmd.Parameters.Add("@FromDate", SqlDbType.DateTime2).Value = startDate;
                        cmd.Parameters.Add("@ToDate", SqlDbType.DateTime2).Value = endDate;
                        cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 200).Value = filter ?? "";
                        cmd.Parameters.Add("@TransactionType", SqlDbType.Int).Value =
                            transactionType.HasValue ? (object)transactionType.Value : DBNull.Value;
                        cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        var pBF = cmd.Parameters.Add("@BookBalanceBF", SqlDbType.Decimal);
                        pBF.Precision = 18; pBF.Scale = 2;
                        pBF.Value = bookBalanceBF;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                decimal amount = Convert.ToDecimal(reader["Amount"]);

                                int glAccountType = reader["GLAccountType"] != DBNull.Value
                                                        ? Convert.ToInt32(reader["GLAccountType"]) : 0;
                                int glAccountCode = reader["GLAccountCode"] != DBNull.Value
                                                        ? Convert.ToInt32(reader["GLAccountCode"]) : 0;
                                string glAccountDesc = reader["GLAccountDescription"]?.ToString() ?? "";

                                int contraGlAccountType = reader["ContraGLAccountType"] != DBNull.Value
                                                                ? Convert.ToInt32(reader["ContraGLAccountType"]) : 0;
                                int contraGlAccountCode = reader["ContraGLAccountCode"] != DBNull.Value
                                                                ? Convert.ToInt32(reader["ContraGLAccountCode"]) : 0;
                                string contraGlAccountDesc = reader["ContraGLAccountDescription"]?.ToString() ?? "";

                                decimal debit = amount > 0 ? amount : 0;
                                decimal credit = amount < 0 ? Math.Abs(amount) : 0;

                                string firstDigit(int type) =>
                                    type > 0 ? type.ToString().Substring(0, 1) : "";

                                string customerFirstName = reader["Individual_FirstName"]?.ToString();
                                string customerLastName = reader["Individual_LastName"]?.ToString();
                                string customerFullName =
                                    (!string.IsNullOrWhiteSpace(customerFirstName) ||
                                     !string.IsNullOrWhiteSpace(customerLastName))
                                        ? $"{customerFirstName} {customerLastName}".Trim()
                                        : null;

                                int transactionCode = reader["TransactionCode"] != DBNull.Value
                                                        ? Convert.ToInt32(reader["TransactionCode"]) : 0;
                                string transactionCodeDescription =
                                    Enum.IsDefined(typeof(SystemTransactionCode), transactionCode)
                                        ? EnumHelper.GetDescription((SystemTransactionCode)transactionCode)
                                        : string.Empty;

                                transactions.Add(new
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    BranchDescription = reader["BranchDescription"]?.ToString(),

                                    GLAccountId = reader.GetGuid(reader.GetOrdinal("ChartOfAccountId")),
                                    GLAccountCode = glAccountCode,
                                    GLAccountType = glAccountType,
                                    GLAccountDescription = glAccountDesc,
                                    GLAccountName = $"{firstDigit(glAccountType)}-{glAccountCode} {glAccountDesc}",

                                    CustomerFullName = customerFullName,
                                    CustomerReference1 = reader["CustomerReference1"]?.ToString(),
                                    CustomerReference2 = reader["CustomerReference2"]?.ToString(),
                                    CustomerReference3 = reader["CustomerReference3"]?.ToString(),

                                    CustomerAccountId = reader["CustomerAccountId"] != DBNull.Value
                                                            ? (Guid?)reader.GetGuid(reader.GetOrdinal("CustomerAccountId"))
                                                            : null,
                                    CustomerAccountAccountTypeProductCode = reader["CustomerAccountType_ProductCode"] != DBNull.Value
                                                            ? (int?)Convert.ToInt32(reader["CustomerAccountType_ProductCode"])
                                                            : null,
                                    CustomerAccountAccountTypeTargetProductId = reader["CustomerAccountType_TargetProductId"] != DBNull.Value
                                                            ? (Guid?)reader.GetGuid(reader.GetOrdinal("CustomerAccountType_TargetProductId"))
                                                            : null,
                                    CustomerAccountAccountTypeTargetProductCode = reader["CustomerAccountType_TargetProductCode"] != DBNull.Value
                                                            ? (int?)Convert.ToInt32(reader["CustomerAccountType_TargetProductCode"])
                                                            : null,
                                    CustomerAccountAccountTypeTargetProductDescription = (string)null,
                                    CustomerAccountNumber = (string)null,

                                    ContraGLAccountId = reader["ContraChartOfAccountId"] != DBNull.Value
                                                                    ? reader.GetGuid(reader.GetOrdinal("ContraChartOfAccountId"))
                                                                    : Guid.Empty,
                                    ContraGLAccountCode = contraGlAccountCode,
                                    ContraGLAccountType = contraGlAccountType,
                                    ContraGLAccountDescription = contraGlAccountDesc,
                                    ContraGLAccountName = $"{firstDigit(contraGlAccountType)}-{contraGlAccountCode} {contraGlAccountDesc}",

                                    JournalId = reader.GetGuid(reader.GetOrdinal("JournalId")),
                                    JournalParentId = reader["JournalParentId"] != DBNull.Value
                                                        ? (Guid?)reader.GetGuid(reader.GetOrdinal("JournalParentId"))
                                                        : null,
                                    JournalPrimaryDescription = reader["PrimaryDescription"]?.ToString(),
                                    JournalSecondaryDescription = reader["SecondaryDescription"]?.ToString(),
                                    JournalReference = reader["Reference"]?.ToString(),

                                    Debit = debit,
                                    Credit = credit,
                                    BookBalance = Convert.ToDecimal(reader["RunningBalance"]),
                                    AvailableBalance = 0m,
                                    RunningBalance = Convert.ToDecimal(reader["RunningBalance"]),

                                    JournalTransactionCode = transactionCode,
                                    JournalTransactionCodeDescription = transactionCodeDescription,

                                    JournalValueDate = reader["JournalValueDate"] != DBNull.Value
                                                            ? (DateTime?)Convert.ToDateTime(reader["JournalValueDate"])
                                                            : null,
                                    JournalCreatedDate = Convert.ToDateTime(reader["JournalCreatedDate"]),
                                    JournalIsLocked = reader["JournalIsLocked"] != DBNull.Value
                                                            && Convert.ToBoolean(reader["JournalIsLocked"]),

                                    ApplicationUserName = reader["ApplicationUserName"]?.ToString(),
                                    EnvironmentUserName = reader["EnvironmentUserName"]?.ToString(),
                                    EnvironmentMachineName = reader["EnvironmentMachineName"]?.ToString(),
                                    EnvironmentDomainName = reader["EnvironmentDomainName"]?.ToString(),
                                    EnvironmentOSVersion = reader["EnvironmentOSVersion"]?.ToString(),
                                    EnvironmentMACAddress = reader["EnvironmentMACAddress"]?.ToString(),
                                    EnvironmentMotherboardSerialNumber = reader["EnvironmentMotherboardSerialNumber"]?.ToString(),
                                    EnvironmentProcessorId = reader["EnvironmentProcessorId"]?.ToString(),
                                    EnvironmentIPAddress = reader["EnvironmentIPAddress"]?.ToString()
                                });
                            }
                        }
                    }
                }

                int totalPages = totalItems > 0
                    ? (int)Math.Ceiling((double)totalItems / pageSize)
                    : 0;

                var response = new
                {
                    ItemsCount = totalItems,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = pageIndex < totalPages - 1,
                    HasPreviousPage = pageIndex > 0,
                    DateRange = new { FromDate = startDate, ToDate = endDate },
                    Filters = new { FilterText = filter, TransactionType = transactionType },
                    Transactions = transactions,
                    BookBalanceBroughtFoward = bookBalanceBF,
                    BookBalanceCarriedForward = bookBalanceCF,
                    AvailableBalanceBroughtFoward = 0m,
                    AvailableBalanceCarriedForward = 0m,
                    TotalCredits = totalCredits,
                    TotalDebits = totalDebits,
                    TotalApportioned = 0m,
                    TotalShortage = 0m
                };

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = totalItems > 0
                        ? $"Showing page {pageIndex + 1} of {totalPages} ({totalItems} total transactions)"
                        : "No transactions found.",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                var realError = ex;
                while (realError.InnerException != null)
                    realError = realError.InnerException;

                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = realError.Message,
                    Data = realError.StackTrace
                });
            }
        }

        [HttpPost]
        [Route("PostJournalVoucher")]
        public async Task<IHttpActionResult> postJournalVoucher([FromBody] JournalVoucherDTO journalVoucherDTO)
        {
            try
            {
              
                var serviceHeader = CreateServiceHeader();



                // Validate that journal voucher entries balance (for batch posting)
                if (journalVoucherDTO.JournalVoucherEntries != null && journalVoucherDTO.JournalVoucherEntries.Any())
                {
                    var totalDebits = journalVoucherDTO.JournalVoucherEntries.Where(e => e.Amount > 0).Sum(e => e.Amount);
                    var totalCredits = journalVoucherDTO.JournalVoucherEntries.Where(e => e.Amount < 0).Sum(e => Math.Abs(e.Amount));

                    if (Math.Abs(totalDebits - totalCredits) > 0.01m)
                    {
                        return BadRequest(
                            "Journal entries are not balanced");
                    }

                    // Set total value to match entries for batch posting
                    journalVoucherDTO.TotalValue = totalDebits;
                }


                var createdVoucher = _journalVoucherAppService.AddNewJournalVoucher(journalVoucherDTO, serviceHeader);

            //    var createdVoucher = await master._channelService.AddJournalVoucherAsync(journalVoucherDTO, serviceHeader);

                if (createdVoucher != null)
                {
                    // If entries are provided, update the voucher entries collection
                    if (journalVoucherDTO.JournalVoucherEntries != null && journalVoucherDTO.JournalVoucherEntries.Any())
                    {

                        var entriesAdded = _journalVoucherAppService.UpdateJournalVoucherEntryCollection(createdVoucher.Id, journalVoucherDTO.JournalVoucherEntries.ToList(), serviceHeader);
                        
                        if (!entriesAdded)
                        {
                            return BadRequest("Failed to add journal entries");
                        }
                    }

                    // Auto-audit and authorize for direct posting (bypass workflow)
                    var auditResult = _journalVoucherAppService.AuditJournalVoucher(createdVoucher, (int)JournalVoucherAuthOption.Post, serviceHeader);



                    var authorizeResult = _journalVoucherAppService.AuthorizeJournalVoucher(createdVoucher, (int)JournalVoucherAuthOption.Post, 0, serviceHeader);
     

                    if (!auditResult || !authorizeResult)
                    {
                        return BadRequest("Failed to post journal voucher");
                    }

                    return Ok(new
                    {
                        Success = true,
                        VoucherId = createdVoucher.Id,
                        VoucherNumber = createdVoucher.VoucherNumber,
                        Message = "Journal voucher posted successfully",
                        TotalAmount = createdVoucher.TotalValue,
                        EntriesCount = journalVoucherDTO.JournalVoucherEntries?.Count ?? 0
                    });
                }
                else
                {
                    return BadRequest("Failed to create journal voucher");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Failed to post journal voucher: " + ex.Message));
            }
        }




        [HttpPost]
        [Route("AddGeneralLedgers")]
        public async Task<IHttpActionResult> AddGeneralLedgers([FromBody] GeneralLedgerDTO generalLedgerDTO)
        {

            var serviceHeader = CreateServiceHeader();


            if (generalLedgerDTO == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Batch cannot be null."
                });
            }

            // Make sure there are entries to process
            if (generalLedgerDTO.GeneralLedgerEntries == null || !generalLedgerDTO.GeneralLedgerEntries.Any())
            {



                return Json(new
                {
                    success = false,
                    message = "At least one ledger entry is required."
                });
            }

            decimal sumAmount = generalLedgerDTO.GeneralLedgerEntries.Sum(e => e.Amount);
            decimal totalValue = generalLedgerDTO.TotalValue;

            if (sumAmount != totalValue)
            {
                var balance = totalValue - sumAmount;
                return Json(new
                {
                    success = false,
                    message = $"The total value ({totalValue}) should be equal to the sum of the entries ({sumAmount}). Balance: {balance}"
                });
            }

            // Validate the DTO
            generalLedgerDTO.ValidateAll();
            if (generalLedgerDTO.ErrorMessages.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    message = generalLedgerDTO.ErrorMessages
                });
            }

            // Save the batch data
            var generalLedgerBatch = _generalLedgerAppService.AddNewGeneralLedger(generalLedgerDTO, serviceHeader);
            if (generalLedgerBatch.HasErrors)
            {
                return Json(new
                {
                    success = false,
                    message = generalLedgerBatch.ErrorMessages
                });
            }

            // Save each entry
            foreach (var entry in generalLedgerDTO.GeneralLedgerEntries)
            {
                entry.GeneralLedgerId = generalLedgerBatch.Id;
                entry.BranchId = generalLedgerBatch.BranchId;
                entry.BranchDescription = generalLedgerBatch.BranchDescription;

                _generalLedgerAppService.AddNewGeneralLedgerEntry(entry, serviceHeader);
            }

            return Json(new
            {
                success = true,
                message = "Successfully created refund batch."
            });
        }

        [HttpGet]
        [Route("GetPurchaseInvoices")]
        public async Task<IHttpActionResult> GetPurchaseInvoices(bool? posted = null)
        {
            var serviceHeader = CreateServiceHeader();

            var invoices = _purchaseInvoiceAppService.FindPurchaseInvoices(serviceHeader);

            // Apply filtering if 'posted' param is provided
            if (posted.HasValue)
            {
                invoices = invoices.Where(i => i.Posted == posted.Value).ToList();
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = invoices?.Count > 0 ? $"{invoices.Count} invoices found." : "No invoices found.",
                Data = invoices
            });
        }



        [HttpPost]
        [Route("AddPurchaseInvoice")]
        public async Task<IHttpActionResult> AddPurchaseInvoice([FromBody] PurchaseInvoiceDTO purchaseInvoiceDTO)
        {

            var serviceHeader = CreateServiceHeader();

            if (purchaseInvoiceDTO != null)
            {

                purchaseInvoiceDTO.PaidAmount = 0;
                purchaseInvoiceDTO.RemainingAmount = purchaseInvoiceDTO.TotalAmount;


                var linesTotal = 0.00m;
                //purchaseInvoiceDTO.RemainingAmount = purchaseInvoiceDTO.

                foreach (var gl in purchaseInvoiceDTO.PurchaseInvoiceLines)
                {

                    linesTotal = linesTotal + gl.Amount;

                    if (gl.DebitChartOfAccountId != Guid.Empty)
                    {

                        var debitGL = _chartOfAccountAppService.FindChartOfAccount(gl.DebitChartOfAccountId, serviceHeader);

                        gl.No = debitGL.AccountCode;

                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "YOU HAVE A LINE WITHOUT PROPERT DEBITCHARTOFAACCOUNTID"
                        });
                    }

                }

                if (purchaseInvoiceDTO.TotalAmount != linesTotal)
                {

                    return Json(new
                    {
                        success = false,
                        message = "Amounts in Lines dont add up to value of Total Amount"
                    });
                }

                purchaseInvoiceDTO.ValidateAll();


                if (purchaseInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseInvoiceDTO.ErrorMessages
                    });
                }

                var result = _purchaseInvoiceAppService.AddNewPurchaseInvoice(purchaseInvoiceDTO, serviceHeader);

                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully added Purchase header with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add Purchase header with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPut]
        [Route("UpdatePurchaseInvoice")]
        public async Task<IHttpActionResult> UpdatePurchaseInvoice([FromBody] PurchaseInvoiceDTO purchaseInvoiceDTO)
        {
            var serviceHeader = CreateServiceHeader();
        
            if (purchaseInvoiceDTO != null)
            {

                purchaseInvoiceDTO.ValidateAll();


                if (purchaseInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseInvoiceDTO.ErrorMessages
                    });
                }


                var result = _purchaseInvoiceAppService.UpdatePurchaseInvoice(purchaseInvoiceDTO, serviceHeader);


                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated Purchase header with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update Purchase header with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPost]
        [Route("PostPurchaseInvoice/{id}")]
        public async Task<IHttpActionResult> PostPurchaseInvoice(Guid id)
        {


            var serviceHeader = CreateServiceHeader();
          
            PurchaseInvoiceDTO purchaseInvoiceDTO = null;


            var purchaseInvoiceDTOs = _purchaseInvoiceAppService.FindPurchaseInvoices(serviceHeader);
           
            if (purchaseInvoiceDTOs != null)
            {

                purchaseInvoiceDTO = purchaseInvoiceDTOs.FirstOrDefault(p => p.Id == id);
            }


            if (purchaseInvoiceDTO != null)
            {

                var banks = _bankLinkageAppService.FindBankLinkages(serviceHeader);
            
                var bank = banks[0];



                purchaseInvoiceDTO.BranchId = bank.BranchId;
                purchaseInvoiceDTO.BankId = bank.Id;
                purchaseInvoiceDTO.BankBranchName = bank.BankBranchName;

                purchaseInvoiceDTO.ValidateAll();
                if (purchaseInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseInvoiceDTO.ErrorMessages
                    });
                }

                //var transactionModel = new TransactionModel();

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                //var result = await master._channelService.AddJournalAsync(transactionModel, tariffs, serviceHeader);


                var result = _purchaseInvoiceAppService.PostPurchaseInvoice(purchaseInvoiceDTO, moduleNavigationItemCode, serviceHeader);

                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }


        [HttpPost]
        [Route("PostPaymentVoucher")]
        public async Task<IHttpActionResult> PayVendorInvoice(PaymentDTO paymentDTO)
        {

            var serviceHeader = CreateServiceHeader();

            if (paymentDTO != null && paymentDTO.PaymentLines.Any())
            {

                decimal totalOfLines = paymentDTO.PaymentLines.Sum(x => x.Amount);

                if (paymentDTO.TotalAmount != totalOfLines)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Total mismatch: Header TotalAmount ({paymentDTO.TotalAmount:N2}) " +
                                  $"does not equal sum of PaymentLines ({totalOfLines:N2})."
                    });
                }

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                var result = _paymentAppService.PostPayment(paymentDTO, moduleNavigationItemCode, serviceHeader);

                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }

        [Route("GetPayments")]
        public async Task<IHttpActionResult> GetPayments()
        {


            var serviceHeader = CreateServiceHeader();

            //var salesInvoices = await master._channelService.FindSalesInvoicesAsync(serviceHeader);

            var payments = _paymentAppService.FindPayments(serviceHeader); ;

       

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = payments?.Count > 0 ? $"{payments.Count} payments found." : "No payments found.",
                Data = payments
            });
        }



        [Route("GetPurchaseCreditMemos")]
        public async Task<IHttpActionResult> GetPurchaseCreditMemos(bool? posted = null)
        {

            var serviceHeader = CreateServiceHeader();

            var creditMemos = _purchaseCreditMemoAppService.FindPurchaseCreditMemos(serviceHeader);
            
            // Apply filtering if 'posted' param is provided
            if (posted.HasValue)
            {
                creditMemos = creditMemos.Where(i => i.Posted == posted.Value).ToList();
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = creditMemos?.Count > 0 ? $"{creditMemos.Count} credit memos found." : "No credit memos found.",
                Data = creditMemos
            });
        }





        [HttpPost]
        [Route("AddPurchaseCreditMemo")]
        public async Task<IHttpActionResult> AddPurchaseCreditMemo([FromBody] PurchaseCreditMemoDTO purchaseCreditMemoDTO)
        {
            var serviceHeader = CreateServiceHeader();
          
            if (purchaseCreditMemoDTO != null)
            {
                foreach (var gl in purchaseCreditMemoDTO.PurchaseCreditMemoLines)
                {

                    if (gl.CreditChartOfAccountId != Guid.Empty)
                    {

                        var debitGl = _chartOfAccountAppService.FindChartOfAccount(gl.CreditChartOfAccountId, serviceHeader);

                        gl.No = debitGl.AccountCode;

                    }

                    else
                    {

                        return Json(new
                        {
                            success = false,
                            message = "YOU HAVE A LINE WITHOUT PROPERT CREDITCHARTOFAACCOUNTID"
                        });


                    }



                }

                purchaseCreditMemoDTO.ValidateAll();


                if (purchaseCreditMemoDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseCreditMemoDTO.ErrorMessages
                    });
                }


                var result = _purchaseCreditMemoAppService.AddNewPurchaseCreditMemo(purchaseCreditMemoDTO, serviceHeader);
                

                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully added Purchase CREDIT MEMO with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add Purchase CREDIT MEMO with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }



        [HttpPut]
        [Route("UpdatePurchaseCreditMemo")]
        public async Task<IHttpActionResult> UpdatePurchaseCreditMemo([FromBody] PurchaseCreditMemoDTO purchaseCreditMemoDTO)
        {

            var serviceHeader = CreateServiceHeader();
        
            if (purchaseCreditMemoDTO != null)
            {
                purchaseCreditMemoDTO.ValidateAll();

                if (purchaseCreditMemoDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseCreditMemoDTO.ErrorMessages
                    });
                }

                var result = _purchaseCreditMemoAppService.UpdatePurchaseCreditMemo(purchaseCreditMemoDTO, serviceHeader);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated Purchase Credit Memo header with lines."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update Purchase Credit Memo header with lines."
                    });
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });
            }
        }






        [HttpPost]
        [Route("PostPurchaseCreditMemo/{id}")]
        public async Task<IHttpActionResult> PostPurchaseCreditMemo(Guid id)
        {

            var serviceHeader = CreateServiceHeader();

            //PurchaseCreditMemoDTO purchaseCreditMemoDTO = null;

            var purchaseCreditMemoDTOs = _purchaseCreditMemoAppService.FindPurchaseCreditMemos(serviceHeader);
        
            var purchaseCreditMemoDTO = purchaseCreditMemoDTOs.FirstOrDefault(p => p.Id == id);
            if (purchaseCreditMemoDTO == null)
            {
                return Json(new { success = false, message = "Purchase Credit Memo not found" });
            }


            if (purchaseCreditMemoDTO != null)
            {

                var banks = _bankLinkageAppService.FindBankLinkages(serviceHeader);
                
                var bank = banks[0];



                purchaseCreditMemoDTO.BranchId = bank.BranchId;
                purchaseCreditMemoDTO.BankId = bank.Id;
                purchaseCreditMemoDTO.BankBranchName = bank.BankBranchName;

                purchaseCreditMemoDTO.ValidateAll();
                if (purchaseCreditMemoDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = purchaseCreditMemoDTO.ErrorMessages
                    });
                }

                //var transactionModel = new TransactionModel();

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                //var result = await master._channelService.AddJournalAsync(transactionModel, tariffs, serviceHeader);


                var result = _purchaseCreditMemoAppService.PostPurchaseCreditMemo(purchaseCreditMemoDTO, moduleNavigationItemCode, serviceHeader);
                
                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }



        [Route("GetSalesInvoices")]
        public async Task<IHttpActionResult> GetSalesInvoices(bool? posted = null)
        {

            var serviceHeader = CreateServiceHeader();

            var salesInvoices = _salesInvoiceAppService.FindSalesInvoices(serviceHeader);

            // Apply filtering if 'posted' param is provided
            if (posted.HasValue)
            {
                salesInvoices = salesInvoices.Where(i => i.Posted == posted.Value).ToList();
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = salesInvoices?.Count > 0 ? $"{salesInvoices.Count} sales invoices found." : "No sales invoices found.",
                Data = salesInvoices
            });
        }


        [HttpPost]
        [Route("AddSalesInvoice")]
        public async Task<IHttpActionResult> AddSalesInvoice([FromBody] SalesInvoiceDTO salesInvoiceDTO)
        {

         
            var serviceHeader = CreateServiceHeader();

            var totalLines = 0.00m;

            if (salesInvoiceDTO != null)
            {



                salesInvoiceDTO.PaidAmount = 0;
                salesInvoiceDTO.RemainingAmount = salesInvoiceDTO.TotalAmount;


                foreach (var gl in salesInvoiceDTO.SalesInvoiceLines)
                {

                    totalLines += gl.Amount;

                    if (gl.CreditChartOfAccountId != Guid.Empty)
                    {


                        var debitGl = _chartOfAccountAppService.FindChartOfAccount(gl.CreditChartOfAccountId, serviceHeader);
                       
                        gl.No = debitGl.AccountCode;

                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "YOU HAVE A LINE WITHOUT PROPERT CREDITCHARTOFAACCOUNTID"
                        });
                    }

                }

                if (salesInvoiceDTO.TotalAmount != totalLines)
                {

                    return Json(new
                    {

                        success = false,
                        message = "Amounts in lines do not add up to the totala mount"
                    });
                }
                salesInvoiceDTO.ValidateAll();


                if (salesInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = salesInvoiceDTO.ErrorMessages
                    });
                }

                var result = _salesInvoiceAppService.AddNewSalesInvoice(salesInvoiceDTO, serviceHeader);
             

                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully added Sales header with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add Sales header with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPut]
        [Route("UpdateSalesInvoice")]
        public async Task<IHttpActionResult> UpdateSalesInvoice([FromBody] SalesInvoiceDTO salesInvoiceDTO)
        {

            var serviceHeADER = CreateServiceHeader();

            if (salesInvoiceDTO != null)
            {

                salesInvoiceDTO.ValidateAll();


                if (salesInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = salesInvoiceDTO.ErrorMessages
                    });
                }

                var result = _salesInvoiceAppService.UpdateSalesInvoice(salesInvoiceDTO, serviceHeADER);

              

                if (result)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated Sales header with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update Sales header with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPost]
        [Route("PostSalesInvoice/{id}")]
        public async Task<IHttpActionResult> PostSalesInvoice(Guid id)
        {


            var serviceHeader = CreateServiceHeader();
      

            SalesInvoiceDTO salesInvoiceDTO = null;


            var salesInvoiceDTOs = _salesInvoiceAppService.FindSalesInvoices(serviceHeader);
          
            if (salesInvoiceDTOs != null)
            {

                salesInvoiceDTO = salesInvoiceDTOs.FirstOrDefault(p => p.Id == id);
            }


            if (salesInvoiceDTO != null && !salesInvoiceDTO.Posted)
            {

                var banks = _bankLinkageAppService.FindBankLinkages(serviceHeader);

                var bank = banks[0];



                salesInvoiceDTO.BranchId = bank.BranchId;
                salesInvoiceDTO.BankId = bank.Id;
                salesInvoiceDTO.BankBranchName = bank.BankBranchName;

                salesInvoiceDTO.ValidateAll();
                if (salesInvoiceDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = salesInvoiceDTO.ErrorMessages
                    });
                }

                //var transactionModel = new TransactionModel();

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                var result = _salesInvoiceAppService.PostSalesInvoice(salesInvoiceDTO, moduleNavigationItemCode, serviceHeader);

                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Target Invoice is already posted, or is missing"
                });
            }

        }






        [HttpPut]
        [Route("UpdateSalesCreditMemo")]
        public async Task<IHttpActionResult> UpdateSalesCreditMemo([FromBody] SalesCreditMemoDTO salesCreditMemoDTO)
        {

            var serviceHeader = CreateServiceHeader();
         
            if (salesCreditMemoDTO != null)
            {
                salesCreditMemoDTO.ValidateAll();

                if (salesCreditMemoDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = salesCreditMemoDTO.ErrorMessages
                    });
                }


                var result = _salesCreditMemoAppService.UpdateSalesCreditMemo(salesCreditMemoDTO, serviceHeader);
                
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated Sales Credit Memo header with lines."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update Sales Credit Memo header with lines."
                    });
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });
            }
        }


        [Route("GetSalesCreditMemos")]
        public async Task<IHttpActionResult> GetSalesCreditMemos(bool? posted = null)
        {
            var serviceHeader = CreateServiceHeader();


            var creditMemos = _salesCreditMemoAppService.FindSalesCreditMemos(serviceHeader);
            
            // Apply filtering if 'posted' param is provided
            if (posted.HasValue)
            {
                creditMemos = creditMemos.Where(i => i.Posted == posted.Value).ToList();
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = creditMemos?.Count > 0 ? $"{creditMemos.Count} credit memos found." : "No credit memos found.",
                Data = creditMemos
            });
        }





        //customer rcpt
        [HttpPost]
        [Route("PostSalesCreditMemo/{id}")]
        public async Task<IHttpActionResult> PostSalesCreditMemo(Guid id)
        {

            var serviceHeader = CreateServiceHeader();

            SalesCreditMemoDTO salesCreditMemoDTO = null;


            var salesCreditMemoDTOs = _salesCreditMemoAppService.FindSalesCreditMemos(serviceHeader);
     
            if (salesCreditMemoDTOs != null)
            {

                salesCreditMemoDTO = salesCreditMemoDTOs.FirstOrDefault(p => p.Id == id);
            }


            if (salesCreditMemoDTO != null && !salesCreditMemoDTO.Posted)
            {

                var banks = _bankLinkageAppService.FindBankLinkages(serviceHeader);

               
                var bank = banks[0];



                salesCreditMemoDTO.BranchId = bank.BranchId;
                salesCreditMemoDTO.BankId = bank.Id;
                salesCreditMemoDTO.BankBranchName = bank.BankBranchName;

                salesCreditMemoDTO.ValidateAll();
                if (salesCreditMemoDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = salesCreditMemoDTO.ErrorMessages
                    });
                }

                //var transactionModel = new TransactionModel();

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                //var result = await master._channelService.AddJournalAsync(transactionModel, tariffs, serviceHeader);

                var result = _salesCreditMemoAppService.PostSalesCreditMemo(salesCreditMemoDTO, moduleNavigationItemCode, serviceHeader);
                
                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Target Sales Credit Memo is already posted, or is missing"
                });
            }

        }



        [Route("GetARCustomers")]
        public async Task<IHttpActionResult> GetARCustomers()
        {

            var serviceHeader = CreateServiceHeader();


            var arCustomers = _arcustomerAppService.FindARCustomers(serviceHeader);

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = arCustomers?.Count > 0 ? $"{arCustomers.Count} customers found." : "No customers  found.",
                Data = arCustomers
            });
        }


        [HttpPut]
        [Route("UpdateARCustomer")]
        public async Task<IHttpActionResult> UpdateARCustomer([FromBody] ARCustomerDTO arCustomerDTO)
        {
            var serviceHeader = CreateServiceHeader();

            if (arCustomerDTO != null)
            {
                arCustomerDTO.ValidateAll();

                if (arCustomerDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = arCustomerDTO.ErrorMessages
                    });
                }


                var result = _arcustomerAppService.UpdateARCustomer(arCustomerDTO, serviceHeader);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated AR Customer."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update AR Customer"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });
            }
        }


        [HttpPost]
        [Route("AddARCustomer")]
        public async Task<IHttpActionResult> AddARCustomer([FromBody] ARCustomerDTO arCustomerDTO)
        {

            var serviceHeader = CreateServiceHeader();

            if (arCustomerDTO != null)
            {

                arCustomerDTO.ValidateAll();


                if (arCustomerDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = arCustomerDTO.ErrorMessages
                    });
                }


                var result = _arcustomerAppService.AddNewARCustomer(arCustomerDTO, serviceHeader);
               
                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully added Customer."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add customer."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }




        [HttpPost]
        [Route("AddNewReceipt")]
        public async Task<IHttpActionResult> AddNewReceipt(ReceiptDTO receiptDTO)
        {

            var serviceHeader = CreateServiceHeader();



            if (receiptDTO != null && receiptDTO.ReceiptLines.Any())
            {



                decimal totalOfLines = receiptDTO.ReceiptLines.Sum(x => x.Amount);

                if (receiptDTO.TotalAmount != totalOfLines)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Total mismatch: Header TotalAmount ({receiptDTO.TotalAmount:N2}) " +
                                  $"does not equal sum of PaymentLines ({totalOfLines:N2})."
                    });
                }

              //  int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                var result = _receiptAppService.AddNewReceipt(receiptDTO, serviceHeader);
              
                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully Added New Receipt",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to add receipt"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }


        [HttpPost]
        [Route("PostReceipt")]
        public async Task<IHttpActionResult> ReceiveCustomerPayment(ReceiptDTO receiptDTO)
        {

            var serviceHeader = CreateServiceHeader();



            if (receiptDTO != null && receiptDTO.ReceiptLines.Any())
            {



                decimal totalOfLines = receiptDTO.ReceiptLines.Sum(x => x.Amount);

                if (receiptDTO.TotalAmount != totalOfLines)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Total mismatch: Header TotalAmount ({receiptDTO.TotalAmount:N2}) " +
                                  $"does not equal sum of PaymentLines ({totalOfLines:N2})."
                    });
                }

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();


                var result = _receiptAppService.PostReceipt(receiptDTO, moduleNavigationItemCode, serviceHeader);

                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }

        [Route("GetReceipts")]
        public async Task<IHttpActionResult> GetReceipts()
        {
            var serviceHeader = CreateServiceHeader();

            var receipts = _receiptAppService.FindReceipts(serviceHeader);





            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = receipts?.Count > 0 ? $"{receipts.Count} receipts found." : "No receipts found.",
                Data = receipts
            });
        }



        // this applies for loan, investment or savings accounts
        [HttpGet]
        [Route("GetCustomerAccountHistory")]
        public async Task<IHttpActionResult> GetCustomerAccountHistory(Guid CustomerAccountId)
        {
            try


            {
                var serviceHeader = CreateServiceHeader();

               var history = _customerAccountAppService.FindCustomerAccountHistory(CustomerAccountId, serviceHeader);

                
                if (history == null || history.Count == 0)
                {
                    return NotFound(); // 404
                }

                return Ok(history); // 200 + JSON list
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // this applies for loan, investment or savings accounts
        [HttpGet]
        [Route("GetCustomerAccountById")]
        public async Task<IHttpActionResult> GetCustomerAccountById(Guid CustomerAccountId)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();



                var account = _customerAccountAppService.FindCustomerAccountDTO(CustomerAccountId, serviceHeader); 
               
                if (account == null)
                {
                    return NotFound(); // 404
                }

                return Ok(account); // 200 + JSON list
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        [HttpPost]
        [Route("api/CustomerReceipt/Create")]
        public async Task<IHttpActionResult> Create(CustomerTransactionModel transactionModel)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                // ── Fetch customer account ────────────────────────────────────

                var selectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(transactionModel.CustomerAccount.Id, serviceHeader);
                //var selectedCustomerAccount =
                //    await master._channelService.FindCustomerAccountAsync(
                //        transactionModel.CustomerAccount.Id,
                //        includeBalances: true,
                //        includeProductDescription: true,
                //        includeInterestBalanceForLoanAccounts: true,
                //        considerMaturityPeriodForInvestmentAccounts: true,
                //        serviceHeader);

                if (selectedCustomerAccount == null)
                    return Ok(new { success = false, message = "Please select a customer account" });

                if ((RecordStatus)selectedCustomerAccount.RecordStatus != RecordStatus.Approved)
                    return Ok(new { success = false, message = "Sorry, account is not approved yet" });

                // ── Fetch bank linkage ────────────────────────────────────────
                var selectedBankLinkage =

                    _bankLinkageAppService.FindBankLinkageByBankAccountId(transactionModel.BankAccountId, serviceHeader);


                if (selectedBankLinkage == null)
                    return Ok(new { success = false, message = "Bank Account is missing, please select a receiving bank" });

                // ── Fetch posting period ──────────────────────────────────────

                var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
              
                if (postingPeriod == null)
                    return Ok(new { success = false, message = "No active posting period found" });

                // ── Populate transaction model ────────────────────────────────
                transactionModel.PostingPeriodId = postingPeriod.Id;
                transactionModel.PrimaryDescription = "Customer Receipt";
                transactionModel.SecondaryDescription = $"BC {selectedBankLinkage.BankName}";
                transactionModel.Reference = $"{selectedCustomerAccount.CustomerReference1}";
                transactionModel.DebitChartOfAccountId = selectedBankLinkage.ChartOfAccountId;
                transactionModel.TransactionCode = (int)SystemTransactionCode.CashDeposit;

                transactionModel.CreditCustomerAccount = selectedCustomerAccount;
                transactionModel.CreditCustomerAccountId = selectedCustomerAccount.Id;
                transactionModel.DebitCustomerAccount = selectedCustomerAccount;
                transactionModel.DebitCustomerAccountId = selectedCustomerAccount.Id;

                // ── Resolve credit CoA from savings product ───────────────────

                var savingsProduct = _savingsProductAppService.FindSavingsProduct(selectedCustomerAccount.CustomerAccountTypeTargetProductId, transactionModel.BranchId, serviceHeader);
                //var savingsProduct =
                //    await master._channelService.FindSavingsProductAsync(
                //        selectedCustomerAccount.CustomerAccountTypeTargetProductId,
                //        serviceHeader);

                if (savingsProduct == null)
                    return Ok(new { success = false, message = "Savings product not found" });

                transactionModel.CreditChartOfAccountId = savingsProduct.ChartOfAccountId;

                // ── Validate ──────────────────────────────────────────────────
                transactionModel.ValidateAll();
                if (transactionModel.HasErrors)
                {
                    var combinedErrors = string.Join("; ", transactionModel.ErrorMessages);
                    return Ok(new { success = false, message = $"Transaction Error: {combinedErrors}" });
                }

                // ── Post transaction ──────────────────────────────────────────

                var journal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, serviceHeader, true);


                //var journal =
                //    await master._channelService.AddJournalWithCustomerAccountAsync(
                //        transactionModel, serviceHeader);

                if (journal == null || journal.HasErrors)
                    return Ok(new { success = false, message = "Journal posting failed." });

                // ── Update customer account balance ───────────────────────────
                selectedCustomerAccount.NewAvailableBalance =
                    selectedCustomerAccount.AvailableBalance + transactionModel.TotalValue;


                var updateResult = _customerAccountAppService.UpdateCustomerAccount(selectedCustomerAccount, serviceHeader);
                

                if (!updateResult)
                    return Ok(new { success = false, message = "Sorry, but the authorized transaction could not be posted!" });

                // ── Success ───────────────────────────────────────────────────
                return Ok(new
                {
                    success = true,
                    message = $"Operation success: Customer's new balance is {selectedCustomerAccount.NewAvailableBalance}",
                    journal = new
                    {
                        id = journal.Id,
                        sequentialId = journal.SequentialId,
                        branchDescription = journal.BranchDescription,
                        primaryDescription = journal.PrimaryDescription,
                        secondaryDescription = journal.SecondaryDescription,
                        postingPeriodDescription = journal.PostingPeriodDescription,
                        applicationUserName = journal.ApplicationUserName,
                        createdDate = journal.CreatedDate,
                        totalValue = journal.TotalValue,
                        reference = journal.Reference
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = $"Error processing receipt: {ex.Message}" });
            }
        }



        [HttpPost]
        [Route("PostCustomerReceipt")]
        public async Task<IHttpActionResult> PostCustomerReceipt(CustomerTransactionModel transactionModel)
        {
            try
            {
                // ── Service header ────────────────────────────────────────────
                var serviceHeader = CreateServiceHeader();

                // ── Validate request ──────────────────────────────────────────
                if (transactionModel == null)
                    return Ok(new { success = false, message = "Invalid transaction request" });

                if (transactionModel.CustomerAccount == null || transactionModel.CustomerAccount.Id == Guid.Empty)
                    return Ok(new { success = false, message = "Please select a customer account" });

                if (transactionModel.BankAccountId == Guid.Empty)
                    return Ok(new { success = false, message = "Please select a receiving bank account" });

                // ── Fetch customer account ────────────────────────────────────

                var selectedCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(transactionModel.CustomerAccount.Id, serviceHeader);

                //var selectedCustomerAccount =
                //    await master._channelService.FindCustomerAccountAsync(
                //        transactionModel.CustomerAccount.Id,
                //        includeBalances: true,
                //        includeProductDescription: true,
                //        includeInterestBalanceForLoanAccounts: true,
                //        considerMaturityPeriodForInvestmentAccounts: true,
                //        serviceHeader);

                if (selectedCustomerAccount == null)
                    return Ok(new { success = false, message = "Customer account not found" });

                if ((RecordStatus)selectedCustomerAccount.RecordStatus != RecordStatus.Approved)
                    return Ok(new { success = false, message = "Sorry, account is not approved yet" });

                // ── Set customer account references ───────────────────────────
                transactionModel.CreditCustomerAccount = selectedCustomerAccount;
                transactionModel.CreditCustomerAccountId = selectedCustomerAccount.Id;
                transactionModel.DebitCustomerAccount = selectedCustomerAccount;
                transactionModel.DebitCustomerAccountId = selectedCustomerAccount.Id;

                // ── Fetch savings product ─────────────────────────────────────

                var targetProduct = _savingsProductAppService.FindSavingsProduct(selectedCustomerAccount.CustomerAccountTypeTargetProductId, transactionModel.BranchId, serviceHeader);
                
             

                if (targetProduct == null)
                    return Ok(new { success = false, message = "Customer account product not found" });

                transactionModel.CreditChartOfAccountId = targetProduct.ChartOfAccountId;

                // ── Fetch bank linkage ────────────────────────────────────────

                var selectedBankLinkage = _bankLinkageAppService.FindBankLinkageByBankAccountId(transactionModel.BankAccountId, serviceHeader);
              
                if (selectedBankLinkage == null)
                    return Ok(new { success = false, message = "Bank Account is missing, please select a receiving bank" });

                // ── Fetch posting period ──────────────────────────────────────
                var postingPeriod = transactionModel.PostingPeriodId != Guid.Empty
                    ? _postingPeriodAppService.FindPostingPeriod(transactionModel.PostingPeriodId, serviceHeader)
                    : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                if (postingPeriod == null)
                    return Ok(new { success = false, message = "Posting period not found" });

                // ── Populate transaction model ────────────────────────────────
                transactionModel.PostingPeriodId = postingPeriod.Id;

                if (string.IsNullOrEmpty(transactionModel.PrimaryDescription))
                    transactionModel.PrimaryDescription = "Customer Receipt";

                if (string.IsNullOrEmpty(transactionModel.SecondaryDescription))
                    transactionModel.SecondaryDescription = $"BC {selectedBankLinkage.BankName}";

                if (string.IsNullOrEmpty(transactionModel.Reference))
                    transactionModel.Reference = selectedCustomerAccount.CustomerReference1;

                transactionModel.DebitChartOfAccountId = selectedBankLinkage.ChartOfAccountId;
                transactionModel.TransactionCode = (int)SystemTransactionCode.CashDeposit;

                // ── Validate business rules ───────────────────────────────────
                transactionModel.ValidateAll();

                if (transactionModel.HasErrors)
                {
                    var combinedErrors = string.Join("; ", transactionModel.ErrorMessages);
                    return Ok(new { success = false, message = $"Transaction Error: {combinedErrors}" });
                }

                // ── Post transaction ──────────────────────────────────────────
                var journal = _journalAppService.AddNewJournal(transactionModel.BranchId, null, transactionModel.TotalValue, transactionModel.PrimaryDescription, transactionModel.SecondaryDescription, transactionModel.Reference, transactionModel.ModuleNavigationItemCode, transactionModel.TransactionCode, transactionModel.ValueDate, transactionModel.CreditChartOfAccountId, transactionModel.DebitChartOfAccountId, serviceHeader, true);

                //var journal =
                //    await master._channelService.AddJournalWithCustomerAccountAsync(
                //        transactionModel, serviceHeader);

                if (journal == null || journal.HasErrors)
                    return Ok(new { success = false, message = "Journal posting failed." });

                // ── Update customer balance ───────────────────────────────────
                selectedCustomerAccount.NewAvailableBalance =
                    selectedCustomerAccount.AvailableBalance + transactionModel.TotalValue;

                var updateResult =
                    _customerAccountAppService.UpdateCustomerAccount(
                        selectedCustomerAccount, serviceHeader);

                if (!updateResult)
                    return Ok(new { success = false, message = "Sorry, but the authorized cash deposit could not be posted!" });

                // ── Success ───────────────────────────────────────────────────
                return Ok(new
                {
                    success = true,
                    message = $"Operation success: Customer's new balance is {selectedCustomerAccount.NewAvailableBalance}",
                    journal = new
                    {
                        id = journal.Id,
                        sequentialId = journal.SequentialId,
                        branchDescription = journal.BranchDescription,
                        primaryDescription = journal.PrimaryDescription,
                        secondaryDescription = journal.SecondaryDescription,
                        postingPeriodDescription = journal.PostingPeriodDescription,
                        applicationUserName = journal.ApplicationUserName,
                        createdDate = journal.CreatedDate,
                        totalValue = journal.TotalValue,
                        reference = journal.Reference
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
        // Add these controllers inside your ValuesController class

        // CREATE GENERAL LEDGER CONTROLLERS

        /// <summary>
        /// Creates a new general ledger with entries
        /// </summary>
        [HttpPost]
        [Route("GeneralLedgers")]
        public async Task<IHttpActionResult> CreateGeneralLedger([FromBody] GeneralLedgerDTO generalLedgerDTO)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                // Set default values
                generalLedgerDTO.Id = Guid.NewGuid();
                generalLedgerDTO.CreatedDate = DateTime.Now;
                generalLedgerDTO.Status = (int)GeneralLedgerStatus.Pending; // Using Pending = 1
                generalLedgerDTO.LedgerNumber = 0; // Will be auto-generated by system

                // Validate the DTO
                generalLedgerDTO.ValidateAll();

                if (generalLedgerDTO.HasErrors)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed for general ledger.",
                        Data = generalLedgerDTO.ErrorMessages
                    });
                }

                // Check if entries are provided and balanced
                if (generalLedgerDTO.GeneralLedgerEntries != null && generalLedgerDTO.GeneralLedgerEntries.Any())
                {
                    decimal totalAmount = 0;

                    foreach (var entry in generalLedgerDTO.GeneralLedgerEntries)
                    {
                        // Validate each entry
                        entry.GeneralLedgerId = generalLedgerDTO.Id;
                        entry.BranchId = generalLedgerDTO.BranchId;
                        entry.CreatedDate = DateTime.Now;
                        entry.Status = (int)GeneralLedgerEntryStatus.Pending;

                        entry.ValidateAll();

                        if (entry.HasErrors)
                        {
                            return Json(new ApiResponse<object>
                            {
                                Success = false,
                                Message = $"Entry validation failed: {string.Join(", ", entry.ErrorMessages)}",
                                Data = null
                            });
                        }

                        totalAmount += Math.Abs(entry.Amount);
                    }

                    // Set total value from entries (divided by 2 because double-entry)
                    generalLedgerDTO.TotalValue = totalAmount / 2;
                }

                // Create the general ledger

                var result = _generalLedgerAppService.AddNewGeneralLedger(generalLedgerDTO, serviceHeader);
            //    var result = await master._channelService.AddGeneralLedgerAsync(generalLedgerDTO, serviceHeader);

                if (result.ErrorMessageResult != null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = (string)result.ErrorMessageResult,
                        Data = null
                    });
                }

                // Add entries if provided
                if (generalLedgerDTO.GeneralLedgerEntries != null && generalLedgerDTO.GeneralLedgerEntries.Any())
                {
                    var entriesAdded = _generalLedgerAppService.UpdateGeneralLedgerEntryCollection(
                        result.Id,
                        generalLedgerDTO.GeneralLedgerEntries.ToList(),
                        serviceHeader
                    );

                    if (!entriesAdded)
                    {
                        return Json(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "General ledger created but failed to add entries.",
                            Data = result
                        });
                    }
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "General ledger created successfully with entries.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating general ledger.",
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a new general ledger entry for an existing ledger
        /// </summary>
        [HttpPost]
        [Route("GeneralLedgerEntries")]
        public async Task<IHttpActionResult> CreateGeneralLedgerEntry([FromBody] GeneralLedgerEntryDTO generalLedgerEntryDTO)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                // Set default values
                generalLedgerEntryDTO.Id = Guid.NewGuid();
                generalLedgerEntryDTO.CreatedDate = DateTime.Now;
                generalLedgerEntryDTO.Status = (int)GeneralLedgerEntryStatus.Pending;

                // Validate the DTO
                generalLedgerEntryDTO.ValidateAll();

                if (generalLedgerEntryDTO.HasErrors)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed for general ledger entry.",
                        Data = generalLedgerEntryDTO.ErrorMessages
                    });
                }

                // For double-entry (G/L to G/L), both accounts must be specified
                if (generalLedgerEntryDTO.ChartOfAccountId != Guid.Empty &&
                    generalLedgerEntryDTO.ContraChartOfAccountId != Guid.Empty)
                {
                    // This is a self-balanced entry
                    if (generalLedgerEntryDTO.Amount == 0)
                    {
                        return Json(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Amount must not be zero for double-entry transaction.",
                            Data = null
                        });
                    }
                }
                // For customer account entries, validate accordingly
                else if (generalLedgerEntryDTO.CustomerAccountId.HasValue ||
                         generalLedgerEntryDTO.ContraCustomerAccountId.HasValue)
                {
                    // Customer account entry logic
                    if (generalLedgerEntryDTO.Amount == 0)
                    {
                        return Json(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Amount must not be zero for customer account transaction.",
                            Data = null
                        });
                    }
                }
                else
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Either G/L accounts or Customer accounts must be specified.",
                        Data = null
                    });
                }


                var result = _generalLedgerAppService.AddNewGeneralLedgerEntry(generalLedgerEntryDTO, serviceHeader);
               
                if (result.ErrorMessageResult != null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = (string)result.ErrorMessageResult,
                        Data = null
                    });
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "General ledger entry created successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating general ledger entry.",
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a complete general ledger with balanced entries in one request
        /// </summary>
        [HttpPost]
        [Route("GeneralLedgers/Complete")]
        public async Task<IHttpActionResult> CreateCompleteGeneralLedger([FromBody] CreateCompleteGeneralLedgerRequest request)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();

                if (request == null || request.Entries == null || !request.Entries.Any())
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No ledger data provided.",
                        Data = null
                    });
                }

                // Create the general ledger header
                var generalLedgerDTO = new GeneralLedgerDTO
                {
                    Id = Guid.NewGuid(),
                    BranchId = request.BranchId,
                    PostingPeriodId = request.PostingPeriodId,
                    Remarks = request.Remarks,
                    CreatedDate = DateTime.Now,
                    Status = (int)GeneralLedgerStatus.Pending,
                    LedgerNumber = 0,
                    GeneralLedgerEntries = new ObservableCollection<GeneralLedgerEntryDTO>()
                };

                // Validate and prepare entries
                decimal totalDebits = 0;
                decimal totalCredits = 0;

                foreach (var entry in request.Entries)
                {
                    entry.Id = Guid.NewGuid();
                    entry.GeneralLedgerId = generalLedgerDTO.Id;
                    entry.BranchId = request.BranchId;
                    entry.CreatedDate = DateTime.Now;
                    entry.Status = (int)GeneralLedgerEntryStatus.Pending;

                    // Validate entry
                    entry.ValidateAll();
                    if (entry.HasErrors)
                    {
                        return Json(new ApiResponse<object>
                        {
                            Success = false,
                            Message = $"Entry validation failed: {string.Join(", ", entry.ErrorMessages)}",
                            Data = null
                        });
                    }

                    // Track debits and credits
                    if (entry.Amount > 0)
                    {
                        totalDebits += entry.Amount;
                    }
                    else
                    {
                        totalCredits += Math.Abs(entry.Amount);
                    }

                    generalLedgerDTO.GeneralLedgerEntries.Add(entry);
                }

                // Check if entries are balanced
                if (Math.Abs(totalDebits - totalCredits) > 0.01m)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Entries are not balanced. Debits: {totalDebits}, Credits: {totalCredits}",
                        Data = null
                    });
                }

                // Set total value
                generalLedgerDTO.TotalValue = totalDebits;

                // Validate general ledger
                generalLedgerDTO.ValidateAll();
                if (generalLedgerDTO.HasErrors)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"General ledger validation failed: {string.Join(", ", generalLedgerDTO.ErrorMessages)}",
                        Data = null
                    });
                }

                // Create the general ledger

                var ledgerResult = _generalLedgerAppService.AddNewGeneralLedger(generalLedgerDTO, serviceHeader);
                
                if (ledgerResult.ErrorMessageResult != null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = (string)ledgerResult.ErrorMessageResult,
                        Data = null
                    });
                }

                // Add entries to the ledger

       

                var entriesAdded = _generalLedgerAppService.UpdateGeneralLedgerEntryCollection(
                    ledgerResult.Id,
                    generalLedgerDTO.GeneralLedgerEntries.ToList(),
                    serviceHeader
                );

                if (!entriesAdded)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "General ledger created but failed to add entries.",
                        Data = ledgerResult
                    });
                }

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"General ledger created successfully with {request.Entries.Count} entries.",
                    Data = new
                    {
                        Ledger = ledgerResult,
                        TotalDebits = totalDebits,
                        TotalCredits = totalCredits,
                        EntryCount = request.Entries.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating complete general ledger.",
                    Data = ex.Message
                });
            }
        }

        // Request models
        public class CreateCompleteGeneralLedgerRequest
        {
            public Guid BranchId { get; set; }
            public Guid PostingPeriodId { get; set; }
            public string Remarks { get; set; }
            public List<GeneralLedgerEntryDTO> Entries { get; set; }
        }


        [HttpGet]
        [Route("GetGeneralLedgerTransactionsByDateRange")]
        public async Task<IHttpActionResult> GetGeneralLedgerTransactionsByDateRange(
    int pageIndex,
    int pageSize,
    DateTime startDate,
    DateTime endDate,
    string text = null,
    int journalEntryFilter = 0)
        {
            try
            {
                var serviceHeader = CreateServiceHeader();


                var result = await _journalEntryAppService.FindGeneralLedgerTransactionsInPageAsync(pageIndex, pageSize, startDate, endDate, text, journalEntryFilter, serviceHeader);


                if (result == null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "No transactions found for the specified date range.",
                        Data = null
                    });
                }

                var sortedData = result.PageCollection?.OrderByDescending(x => x.JournalCreatedDate).ToList();

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = sortedData?.Count > 0 ? $"{sortedData.Count} transactions found." : "No transactions found.",
                    Data = sortedData
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the transactions.",
                    Data = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("GetMembersWithDetails")]
        public async Task<IHttpActionResult> GetMembersWithDetails(
    [FromUri] bool includeNextOfKin = true,
    [FromUri] bool includeAccounts = true,
    [FromUri] string search = "",
    [FromUri] int pageIndex = 0,
    [FromUri] int pageSize = 50)
        {
            try
            {
                var members = new List<object>();
                int totalCount = 0;
                int offset = pageIndex * pageSize;

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // ── Total count ───────────────────────────────────────────
                    using (var countCmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
                WHERE c.RecordStatus IN (1, 2)
                  AND (@Search = ''
                       OR c.Individual_FirstName      LIKE '%' + @Search + '%'
                       OR c.Individual_LastName       LIKE '%' + @Search + '%'
                       OR c.Reference2               LIKE '%' + @Search + '%'
                       OR c.Reference3               LIKE '%' + @Search + '%'
                       OR c.Individual_PayrollNumbers LIKE '%' + @Search + '%'
                       OR c.Address_MobileLine        LIKE '%' + @Search + '%'
                       OR c.Individual_IdentityCardNumber LIKE '%' + @Search + '%')", conn))
                    {
                        countCmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = search ?? "";
                        totalCount = (int)await countCmd.ExecuteScalarAsync();
                    }

                    // ── Fetch members ─────────────────────────────────────────
                    var customerIds = new List<Guid>();
                    var memberMap = new Dictionary<Guid, dynamic>();

                    using (var cmd = new SqlCommand(@"
                SELECT
                    c.Id,
                    c.SerialNumber,
                    ISNULL(c.Individual_FirstName, '')          AS FirstName,
                    ISNULL(c.Individual_LastName,  '')          AS LastName,
                    c.Individual_IdentityCardType               AS IDCardType,
                    c.Individual_IdentityCardNumber             AS IDNumber,
                    c.Individual_PayrollNumbers                 AS PayrollNumber,
                    c.Individual_Salutation                     AS Salutation,
                    c.Individual_Gender                         AS Gender,
                    c.Individual_MaritalStatus                  AS MaritalStatus,
                    c.Individual_Nationality                    AS Nationality,
                    c.Individual_BirthDate                      AS BirthDate,
                    c.Individual_EmploymentDesignation          AS EmploymentDesignation,
                    c.Individual_EmploymentTermsOfService       AS EmploymentTerms,
                    c.Individual_EmploymentDate                 AS EmploymentDate,
                    c.Individual_Classification                 AS Classification,
                    c.Address_AddressLine1                      AS AddressLine1,
                    c.Address_AddressLine2                      AS AddressLine2,
                    c.Address_Street                            AS Street,
                    c.Address_PostalCode                        AS PostalCode,
                    c.Address_City                              AS City,
                    c.Address_Email                             AS Email,
                    c.Address_LandLine                          AS LandLine,
                    c.Address_MobileLine                        AS MobileLine,
                    c.Reference1                                AS Reference1,
                    c.Reference2                                AS MemberNumber,
                    c.Reference3                                AS PFNumber,
                    c.Remarks,
                    c.IsDefaulter,
                    c.IsLocked,
                    c.InhibitGuaranteeing,
                    c.RecruitedBy,
                    c.RecordStatus,
                    c.RegistrationDate,
                    c.BankName,
                    c.BranchName,
                    c.CreatedDate,
                    c.CreatedBy
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
                WHERE c.RecordStatus IN (1, 2)
                  AND (@Search = ''
                       OR c.Individual_FirstName      LIKE '%' + @Search + '%'
                       OR c.Individual_LastName       LIKE '%' + @Search + '%'
                       OR c.Reference2               LIKE '%' + @Search + '%'
                       OR c.Reference3               LIKE '%' + @Search + '%'
                       OR c.Individual_PayrollNumbers LIKE '%' + @Search + '%'
                       OR c.Address_MobileLine        LIKE '%' + @Search + '%'
                       OR c.Individual_IdentityCardNumber LIKE '%' + @Search + '%')
                ORDER BY c.Individual_FirstName ASC, c.Individual_LastName ASC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn))
                    {
                        cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = search ?? "";
                        cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            while (await r.ReadAsync())
                            {
                                Guid id = r.GetGuid(r.GetOrdinal("Id"));
                                customerIds.Add(id);

                                DateTime? birthDate = r["BirthDate"] != DBNull.Value
                                                        ? (DateTime?)Convert.ToDateTime(r["BirthDate"]) : null;

                                memberMap[id] = new
                                {
                                    id = id,
                                    serialNumber = r["SerialNumber"] != DBNull.Value ? Convert.ToInt32(r["SerialNumber"]) : 0,
                                    firstName = r["FirstName"]?.ToString(),
                                    lastName = r["LastName"]?.ToString(),
                                    fullName = $"{r["FirstName"]} {r["LastName"]}".Trim(),
                                    idCardType = r["IDCardType"] != DBNull.Value ? Convert.ToInt32(r["IDCardType"]) : 0,
                                    idNumber = r["IDNumber"]?.ToString(),
                                    payrollNumber = r["PayrollNumber"]?.ToString(),
                                    salutation = r["Salutation"] != DBNull.Value ? Convert.ToInt32(r["Salutation"]) : 0,
                                    gender = r["Gender"] != DBNull.Value ? Convert.ToInt32(r["Gender"]) : 0,
                                    maritalStatus = r["MaritalStatus"] != DBNull.Value ? Convert.ToInt32(r["MaritalStatus"]) : 0,
                                    nationality = r["Nationality"]?.ToString(),
                                    birthDate = birthDate,
                                    age = CalculateAgeFromBirthDate(birthDate),
                                    employmentDesignation = r["EmploymentDesignation"]?.ToString(),
                                    employmentTerms = r["EmploymentTerms"] != DBNull.Value ? Convert.ToInt32(r["EmploymentTerms"]) : 0,
                                    employmentDate = r["EmploymentDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["EmploymentDate"]) : null,
                                    classification = r["Classification"] != DBNull.Value ? Convert.ToInt32(r["Classification"]) : 0,
                                    address = new
                                    {
                                        addressLine1 = r["AddressLine1"]?.ToString(),
                                        addressLine2 = r["AddressLine2"]?.ToString(),
                                        street = r["Street"]?.ToString(),
                                        postalCode = r["PostalCode"]?.ToString(),
                                        city = r["City"]?.ToString(),
                                        email = r["Email"]?.ToString(),
                                        landLine = r["LandLine"]?.ToString(),
                                        mobileLine = r["MobileLine"]?.ToString()
                                    },
                                    reference1 = r["Reference1"]?.ToString(),
                                    memberNumber = r["MemberNumber"]?.ToString(),
                                    pfNumber = r["PFNumber"]?.ToString(),
                                    remarks = r["Remarks"]?.ToString(),
                                    isDefaulter = r["IsDefaulter"] != DBNull.Value && Convert.ToBoolean(r["IsDefaulter"]),
                                    isLocked = r["IsLocked"] != DBNull.Value && Convert.ToBoolean(r["IsLocked"]),
                                    inhibitGuaranteeing = r["InhibitGuaranteeing"] != DBNull.Value && Convert.ToBoolean(r["InhibitGuaranteeing"]),
                                    recruitedBy = r["RecruitedBy"]?.ToString(),
                                    recordStatus = r["RecordStatus"] != DBNull.Value ? Convert.ToInt32(r["RecordStatus"]) : 0,
                                    registrationDate = r["RegistrationDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["RegistrationDate"]) : null,
                                    bankName = r["BankName"]?.ToString(),
                                    branchName = r["BranchName"]?.ToString(),
                                    createdDate = r["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(r["CreatedDate"]) : DateTime.MinValue,
                                    createdBy = r["CreatedBy"]?.ToString(),
                                    nextOfKin = new List<object>(),
                                    accounts = new List<object>()
                                };
                            }
                        }
                    }

                    if (!customerIds.Any())
                    {
                        return Json(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "No members found.",
                            Data = null
                        });
                    }

                    // ── Build IN clause for batch queries ─────────────────────
                    var paramNames = customerIds.Select((_, i) => $"@CId{i}").ToList();
                    var inClause = string.Join(",", paramNames);

                    // ── Next of kin (one query for all members) ───────────────
                    if (includeNextOfKin)
                    {
                        using (var cmd = new SqlCommand($@"
                    SELECT
                        n.Id,
                        n.CustomerId,
                        ISNULL(n.Salutation, 0)     AS Salutation,
                        ISNULL(n.FirstName, '')      AS FirstName,
                        ISNULL(n.LastName,  '')      AS LastName,
                        n.IdentityCardNumber,
                        ISNULL(n.Gender, 0)          AS Gender,
                        ISNULL(n.Relationship, 0)    AS Relationship,
                        ISNULL(n.IdentityCardType, 0) AS IdentityCardType,
                        n.Address_AddressLine1,
                        n.Address_AddressLine2,
                        n.Address_Street,
                        n.Address_PostalCode,
                        n.Address_City,
                        n.Address_Email,
                        n.Address_LandLine,
                        n.Address_MobileLine,
                        ISNULL(n.NominatedPercentage, 0) AS NominatedPercentage,
                        n.Remarks,
                        n.CreatedDate
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_NextOfKin] n
                    WHERE n.CustomerId IN ({inClause})", conn))
                        {
                            for (int i = 0; i < customerIds.Count; i++)
                                cmd.Parameters.Add(paramNames[i], SqlDbType.UniqueIdentifier).Value = customerIds[i];

                            using (var r = await cmd.ExecuteReaderAsync())
                            {
                                while (await r.ReadAsync())
                                {
                                    Guid customerId = r.GetGuid(r.GetOrdinal("CustomerId"));
                                    if (!memberMap.ContainsKey(customerId)) continue;

                                    string nokFirst = r["FirstName"]?.ToString() ?? "";
                                    string nokLast = r["LastName"]?.ToString() ?? "";

                                    var nok = new
                                    {
                                        id = r.GetGuid(r.GetOrdinal("Id")),
                                        salutation = r["Salutation"] != DBNull.Value ? Convert.ToInt32(r["Salutation"]) : 0,
                                        firstName = nokFirst,
                                        lastName = nokLast,
                                        fullName = $"{nokFirst} {nokLast}".Trim(),
                                        identityCardNumber = r["IdentityCardNumber"]?.ToString(),
                                        gender = r["Gender"] != DBNull.Value ? Convert.ToInt32(r["Gender"]) : 0,
                                        relationship = r["Relationship"] != DBNull.Value ? Convert.ToInt32(r["Relationship"]) : 0,
                                        identityCardType = r["IdentityCardType"] != DBNull.Value ? Convert.ToInt32(r["IdentityCardType"]) : 0,
                                        address = new
                                        {
                                            addressLine1 = r["Address_AddressLine1"]?.ToString(),
                                            addressLine2 = r["Address_AddressLine2"]?.ToString(),
                                            street = r["Address_Street"]?.ToString(),
                                            postalCode = r["Address_PostalCode"]?.ToString(),
                                            city = r["Address_City"]?.ToString(),
                                            email = r["Address_Email"]?.ToString(),
                                            landLine = r["Address_LandLine"]?.ToString(),
                                            mobileLine = r["Address_MobileLine"]?.ToString()
                                        },
                                        nominatedPercentage = Convert.ToDecimal(r["NominatedPercentage"]),
                                        remarks = r["Remarks"]?.ToString(),
                                        createdDate = r["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(r["CreatedDate"]) : DateTime.MinValue
                                    };

                                    ((List<object>)memberMap[customerId].nextOfKin).Add(nok);
                                }
                            }
                        }
                    }

                    // ── Accounts (one query for all members) ──────────────────
                    if (includeAccounts)
                    {
                        using (var cmd = new SqlCommand($@"
                    SELECT
                        ca.Id                                       AS AccountId,
                        ca.CustomerId,
                        ca.BranchId,
                        ca.CustomerAccountType_ProductCode          AS ProductCode,
                        ca.CustomerAccountType_TargetProductId      AS TargetProductId,
                        ca.CustomerAccountType_TargetProductCode    AS TargetProductCode,
                        ISNULL(sp.Description, lp.Description)      AS ProductDescription,
                        ISNULL(sp.ChartOfAccountId, lp.ChartOfAccountId) AS ChartOfAccountId,
                        ISNULL(ca.Status, 0)                        AS AccountStatus,
                        ISNULL(ca.RecordStatus, 0)                  AS RecordStatus,
                        ca.Remarks,
                        ca.CreatedDate,
                        ca.CreatedBy,
                        -- Balance from journal entries
                        ISNULL(SUM(je.Amount), 0)                   AS Balance
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts]    sp ON sp.Id = ca.CustomerAccountType_TargetProductId
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts]       lp ON lp.Id = ca.CustomerAccountType_TargetProductId
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries]     je ON je.CustomerAccountId = ca.Id
                    WHERE ca.CustomerId IN ({inClause})
                    GROUP BY
                        ca.Id, ca.CustomerId, ca.BranchId,
                        ca.CustomerAccountType_ProductCode,
                        ca.CustomerAccountType_TargetProductId,
                        ca.CustomerAccountType_TargetProductCode,
                        sp.Description, lp.Description,
                        sp.ChartOfAccountId, lp.ChartOfAccountId,
                        ca.Status, ca.RecordStatus, ca.Remarks,
                        ca.CreatedDate, ca.CreatedBy", conn))
                        {
                            for (int i = 0; i < customerIds.Count; i++)
                                cmd.Parameters.Add(paramNames[i], SqlDbType.UniqueIdentifier).Value = customerIds[i];

                            using (var r = await cmd.ExecuteReaderAsync())
                            {
                                while (await r.ReadAsync())
                                {
                                    Guid customerId = r.GetGuid(r.GetOrdinal("CustomerId"));
                                    if (!memberMap.ContainsKey(customerId)) continue;

                                    var account = new
                                    {
                                        accountId = r.GetGuid(r.GetOrdinal("AccountId")),
                                        branchId = r["BranchId"] != DBNull.Value ? (Guid?)r.GetGuid(r.GetOrdinal("BranchId")) : null,
                                        productCode = r["ProductCode"] != DBNull.Value ? Convert.ToInt32(r["ProductCode"]) : 0,
                                        targetProductId = r["TargetProductId"] != DBNull.Value ? (Guid?)r.GetGuid(r.GetOrdinal("TargetProductId")) : null,
                                        targetProductCode = r["TargetProductCode"] != DBNull.Value ? Convert.ToInt32(r["TargetProductCode"]) : 0,
                                        productDescription = r["ProductDescription"]?.ToString(),
                                        chartOfAccountId = r["ChartOfAccountId"] != DBNull.Value ? (Guid?)r.GetGuid(r.GetOrdinal("ChartOfAccountId")) : null,
                                        accountStatus = r["AccountStatus"] != DBNull.Value ? Convert.ToInt32(r["AccountStatus"]) : 0,
                                        recordStatus = r["RecordStatus"] != DBNull.Value ? Convert.ToInt32(r["RecordStatus"]) : 0,
                                        balance = r["Balance"] != DBNull.Value ? Convert.ToDecimal(r["Balance"]) : 0m,
                                        remarks = r["Remarks"]?.ToString(),
                                        createdDate = r["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(r["CreatedDate"]) : DateTime.MinValue,
                                        createdBy = r["CreatedBy"]?.ToString()
                                    };

                                    ((List<object>)memberMap[customerId].accounts).Add(account);
                                }
                            }
                        }
                    }

                    // ── Build final list in sorted order ──────────────────────
                    members = customerIds.Select(id => (object)memberMap[id]).ToList();
                }

                int totalPages = totalCount > 0
                    ? (int)Math.Ceiling((double)totalCount / pageSize)
                    : 0;

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Successfully retrieved {members.Count} members.",
                    Data = new
                    {
                        totalCount = totalCount,
                        pageIndex = pageIndex,
                        pageSize = pageSize,
                        totalPages = totalPages,
                        hasNextPage = pageIndex < totalPages - 1,
                        hasPreviousPage = pageIndex > 0,
                        members = members
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving member details.",
                    Data = new { Error = ex.Message, StackTrace = ex.StackTrace }
                });
            }
        }
        private int CalculateAgeFromBirthDate(DateTime? birthDate)
        {
            if (!birthDate.HasValue || birthDate.Value == DateTime.MinValue)
                return -1;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            // Adjust age if birthday hasn't occurred yet this year
            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age >= 0 ? age : -1;
        }

        private async Task<object> GetNextOfKinForCustomer(Guid customerId, ServiceHeader serviceHeader)
        {
            try
            {

                var nextOfKins = await _customerAppService.FindNextOfKinCollectionAsync(customerId, serviceHeader);
                if (nextOfKins == null || !nextOfKins.Any())
                    return null;

                return nextOfKins;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error fetching next of kin for customer {customerId}: {ex.Message}");
                return null;
            }
        }




        // Helper method to get account statements using direct SQL queries
        public async Task<Dictionary<Guid, List<object>>> GetAccountStatementsAsync(
            List<Guid> accountIds,
            DateTime? startDate,
            DateTime? endDate)
        {
            var accountStatements = new Dictionary<Guid, List<object>>();

            try
            {
                // Create a connection to the database
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    // Build the SQL query to get journal entries and journal data
                    var sql = @"
                SELECT 
                    -- Journal Entry fields
                    je.Id as JournalEntryId,
                    je.JournalId,
                    je.ChartOfAccountId,
                    je.ContraChartOfAccountId,
                    je.CustomerAccountId,
                    je.Amount,
                    je.ValueDate as JournalEntryValueDate,
                    je.IntegrityHash as JournalEntryIntegrityHash,
                    je.SequentialId as JournalEntrySequentialId,
                    je.CreatedBy as JournalEntryCreatedBy,
                    je.CreatedDate as JournalEntryCreatedDate,
                    
                    -- Journal fields
                    j.Id as JournalId,
                    j.ParentId,
                    j.PostingPeriodId,
                    j.BranchId,
                    j.AlternateChannelLogId,
                    j.TotalValue,
                    j.PrimaryDescription,
                    j.SecondaryDescription,
                    j.Reference,
                    j.ApplicationUserName,
                    j.EnvironmentUserName,
                    j.EnvironmentMachineName,
                    j.EnvironmentDomainName,
                    j.EnvironmentOSVersion,
                    j.EnvironmentMACAddress,
                    j.EnvironmentMotherboardSerialNumber,
                    j.EnvironmentProcessorId,
                    j.EnvironmentIPAddress,
                    j.ModuleNavigationItemCode,
                    j.TransactionCode,
                    j.ValueDate as JournalValueDate,
                    j.SuppressAccountAlert,
                    j.IsLocked,
                    j.IntegrityHash as JournalIntegrityHash,
                    j.SequentialId as JournalSequentialId,
                    j.CreatedBy as JournalCreatedBy,
                    j.CreatedDate as JournalCreatedDate
                    
                FROM swiftFin_JournalEntries je
                INNER JOIN swiftFin_Journals j ON je.JournalId = j.Id
                WHERE je.CustomerAccountId IN ({0})";

                    // Add date filtering if provided
                    if (startDate.HasValue || endDate.HasValue)
                    {
                        sql += " AND (";
                        var conditions = new List<string>();

                        if (startDate.HasValue)
                        {
                            conditions.Add("(je.ValueDate >= @StartDate OR j.ValueDate >= @StartDate OR je.CreatedDate >= @StartDate OR j.CreatedDate >= @StartDate)");
                        }

                        if (endDate.HasValue)
                        {
                            conditions.Add("(je.ValueDate <= @EndDate OR j.ValueDate <= @EndDate OR je.CreatedDate <= @EndDate OR j.CreatedDate <= @EndDate)");
                        }

                        sql += string.Join(" AND ", conditions) + ")";
                    }

                    sql += " ORDER BY ISNULL(je.ValueDate, j.ValueDate) DESC, je.CreatedDate DESC, j.CreatedDate DESC";

                    // Create parameterized query
                    var accountIdParameters = string.Join(",", accountIds.Select((id, index) => $"@AccountId{index}"));
                    sql = string.Format(sql, accountIdParameters);

                    using (var command = new SqlCommand(sql, connection))
                    {
                        // Add account ID parameters
                        for (int i = 0; i < accountIds.Count; i++)
                        {
                            command.Parameters.AddWithValue($"@AccountId{i}", accountIds[i]);
                        }

                        // Add date parameters if provided
                        if (startDate.HasValue)
                        {
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);
                        }

                        if (endDate.HasValue)
                        {
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customerAccountId = reader.GetGuid(reader.GetOrdinal("CustomerAccountId"));

                                if (!accountStatements.ContainsKey(customerAccountId))
                                {
                                    accountStatements[customerAccountId] = new List<object>();
                                }

                                // Get journal entry value date
                                DateTime? journalEntryValueDate = null;
                                var journalEntryValueDateOrdinal = reader.GetOrdinal("JournalEntryValueDate");
                                if (!reader.IsDBNull(journalEntryValueDateOrdinal))
                                {
                                    journalEntryValueDate = reader.GetDateTime(journalEntryValueDateOrdinal);
                                }

                                // Get journal value date
                                DateTime? journalValueDate = null;
                                var journalValueDateOrdinal = reader.GetOrdinal("JournalValueDate");
                                if (!reader.IsDBNull(journalValueDateOrdinal))
                                {
                                    journalValueDate = reader.GetDateTime(journalValueDateOrdinal);
                                }

                                var statement = new
                                {
                                    // Journal Entry details
                                    JournalEntry = new
                                    {
                                        Id = reader.GetGuid(reader.GetOrdinal("JournalEntryId")),
                                        JournalId = reader.GetGuid(reader.GetOrdinal("JournalId")),
                                        ChartOfAccountId = reader.GetGuid(reader.GetOrdinal("ChartOfAccountId")),
                                        ContraChartOfAccountId = reader.GetGuid(reader.GetOrdinal("ContraChartOfAccountId")),
                                        CustomerAccountId = customerAccountId,
                                        Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                        ValueDate = journalEntryValueDate,
                                        IntegrityHash = reader.GetString(reader.GetOrdinal("JournalEntryIntegrityHash")),
                                        SequentialId = reader.GetGuid(reader.GetOrdinal("JournalEntrySequentialId")),
                                        CreatedBy = reader.GetString(reader.GetOrdinal("JournalEntryCreatedBy")),
                                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("JournalEntryCreatedDate"))
                                    },

                                    // Journal details
                                    Journal = new
                                    {
                                        Id = reader.GetGuid(reader.GetOrdinal("JournalId")),
                                        ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("ParentId")),
                                        PostingPeriodId = reader.GetGuid(reader.GetOrdinal("PostingPeriodId")),
                                        BranchId = reader.GetGuid(reader.GetOrdinal("BranchId")),
                                        AlternateChannelLogId = reader.IsDBNull(reader.GetOrdinal("AlternateChannelLogId")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("AlternateChannelLogId")),
                                        TotalValue = reader.GetDecimal(reader.GetOrdinal("TotalValue")),
                                        PrimaryDescription = reader.GetString(reader.GetOrdinal("PrimaryDescription")),
                                        SecondaryDescription = reader.GetString(reader.GetOrdinal("SecondaryDescription")),
                                        Reference = reader.GetString(reader.GetOrdinal("Reference")),
                                        ApplicationUserName = reader.GetString(reader.GetOrdinal("ApplicationUserName")),
                                        EnvironmentUserName = reader.GetString(reader.GetOrdinal("EnvironmentUserName")),
                                        EnvironmentMachineName = reader.GetString(reader.GetOrdinal("EnvironmentMachineName")),
                                        EnvironmentDomainName = reader.GetString(reader.GetOrdinal("EnvironmentDomainName")),
                                        EnvironmentOSVersion = reader.GetString(reader.GetOrdinal("EnvironmentOSVersion")),
                                        EnvironmentMACAddress = reader.GetString(reader.GetOrdinal("EnvironmentMACAddress")),
                                        EnvironmentMotherboardSerialNumber = reader.GetString(reader.GetOrdinal("EnvironmentMotherboardSerialNumber")),
                                        EnvironmentProcessorId = reader.GetString(reader.GetOrdinal("EnvironmentProcessorId")),
                                        EnvironmentIPAddress = reader.GetString(reader.GetOrdinal("EnvironmentIPAddress")),
                                        ModuleNavigationItemCode = reader.GetInt32(reader.GetOrdinal("ModuleNavigationItemCode")),
                                        TransactionCode = reader.GetInt32(reader.GetOrdinal("TransactionCode")),
                                        ValueDate = journalValueDate,
                                        SuppressAccountAlert = reader.GetBoolean(reader.GetOrdinal("SuppressAccountAlert")),
                                        IsLocked = reader.GetBoolean(reader.GetOrdinal("IsLocked")),
                                        IntegrityHash = reader.GetString(reader.GetOrdinal("JournalIntegrityHash")),
                                        SequentialId = reader.GetGuid(reader.GetOrdinal("JournalSequentialId")),
                                        CreatedBy = reader.GetString(reader.GetOrdinal("JournalCreatedBy")),
                                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("JournalCreatedDate"))
                                    },

                                    // Combined statement summary
                                    StatementSummary = new
                                    {
                                        TransactionId = reader.GetGuid(reader.GetOrdinal("JournalId")),
                                        TransactionDate = reader.GetDateTime(reader.GetOrdinal("JournalCreatedDate")),
                                        ValueDate = journalEntryValueDate ?? journalValueDate ?? reader.GetDateTime(reader.GetOrdinal("JournalCreatedDate")),
                                        Description = reader.GetString(reader.GetOrdinal("PrimaryDescription")),
                                        Reference = reader.GetString(reader.GetOrdinal("Reference")),
                                        Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                        TransactionCode = reader.GetInt32(reader.GetOrdinal("TransactionCode")),
                                        IsReversed = reader.GetBoolean(reader.GetOrdinal("IsLocked"))
                                    }
                                };

                                accountStatements[customerAccountId].Add(statement);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error in GetAccountStatementsAsync: {ex.Message}");
                System.Diagnostics.Trace.TraceError($"Stack Trace: {ex.StackTrace}");
                throw;
            }

            return accountStatements;
        }

        // Method to get connection string (you'll need to implement this based on your configuration)
        private string GetConnectionString()
        {
            // Get connection string from your configuration
            // This could be from web.config, appsettings.json, or other configuration source
            return System.Configuration.ConfigurationManager.ConnectionStrings["SwiftFinancialsDB_Live"].ConnectionString;
        }




        [HttpPost]
        [Route("creditbatch/add")]
        public async Task<IHttpActionResult> AddCreditBatch(CreditBatchDTO creditBatchDTO)
        {


            var serviceHeader = CreateServiceHeader();

            CreditBatchDTO creditBatchResponse = null;

            try
            {


                creditBatchResponse = _creditBatchAppService.AddNewCreditBatch(creditBatchDTO, serviceHeader);
             

                if (creditBatchResponse == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Sorry, but the batch could not be created!"
                    });
                }
                // -------------------- SUCCESS RESPONSE --------------------



                //await master._channelService.ParseCreditBatchImportAsync(creditBatchResponse.Id, creditBatchDTO.ImportFileName, serviceHeader);

                return Ok(creditBatchResponse);


            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

        }


        [HttpGet]
        [Route("creditbatches")]
        public async Task<IHttpActionResult> FindCreditBatches()
        {
            var serviceHeader = CreateServiceHeader();

            try
            {
                var batches = _creditBatchAppService.FindCreditBatches(serviceHeader); 
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("credittypes")]
        public async Task<IHttpActionResult> FindCreditTypes()
        {
            var serviceHeader = CreateServiceHeader();

            try
            {

                var credittypes = _creditTypeAppService.FindCreditTypes(serviceHeader);
            
                return Ok(credittypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("creditbatch/{batchId}/entries")]
        public async Task<IHttpActionResult> GetCreditBatchEntries(
     Guid batchId,
     [FromUri] int page = 1,
     [FromUri] int pageSize = 50,
     [FromUri] string search = "")
        {
            try
            {
                if (batchId == Guid.Empty)
                    return Ok(new ApiResponse<object> { Success = false, Message = "Invalid batch ID." });

                var members = new List<object>();
                int totalCount = 0;
                int offset = (page - 1) * pageSize;

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // ================== VERIFY BATCH EXISTS ==================
                    using (var checkCmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatches]
                WHERE Id = @BatchId", conn))
                    {
                        checkCmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        if ((int)await checkCmd.ExecuteScalarAsync() == 0)
                            return Ok(new ApiResponse<object> { Success = false, Message = "Batch not found." });
                    }

                    // ================== GRAND TOTALS (ALL entries in batch) ==================
                    decimal grandTotalPrincipal = 0m;
                    decimal grandTotalInterest = 0m;
                    decimal grandTotalAmount = 0m;
                    int grandTotalEntries = 0;
                    int grandTotalMembers = 0;

                    using (var totalsCmd = new SqlCommand(@"
                SELECT
                    COUNT(e.[Id])                                          AS TotalEntries,
                    COUNT(DISTINCT ca.[CustomerId])                        AS TotalMembers,
                    ISNULL(SUM(e.[Principal]), 0)                          AS TotalPrincipal,
                    ISNULL(SUM(e.[Interest]),  0)                          AS TotalInterest,
                    ISNULL(SUM(e.[Principal] + e.[Interest]), 0)           AS TotalAmount
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries] e
                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca ON ca.[Id] = e.[CustomerAccountId]
                WHERE e.[CreditBatchId] = @BatchId", conn))
                    {
                        totalsCmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        using (var r = await totalsCmd.ExecuteReaderAsync())
                        {
                            if (await r.ReadAsync())
                            {
                                grandTotalEntries = Convert.ToInt32(r["TotalEntries"]);
                                grandTotalMembers = Convert.ToInt32(r["TotalMembers"]);
                                grandTotalPrincipal = Convert.ToDecimal(r["TotalPrincipal"]);
                                grandTotalInterest = Convert.ToDecimal(r["TotalInterest"]);
                                grandTotalAmount = Convert.ToDecimal(r["TotalAmount"]);
                            }
                        }
                    }

                    // ================== TOTAL MEMBER COUNT (for pagination) ==================
                    using (var countCmd = new SqlCommand(@"
                SELECT COUNT(DISTINCT ca.[CustomerId])
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries] e
                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca ON ca.[Id] = e.[CustomerAccountId]
                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c          ON c.[Id]  = ca.[CustomerId]
                WHERE e.[CreditBatchId] = @BatchId
                  AND (@Search = ''
                       OR c.[Individual_FirstName]      LIKE '%' + @Search + '%'
                       OR c.[Individual_LastName]       LIKE '%' + @Search + '%'
                       OR c.[Individual_PayrollNumbers] LIKE '%' + @Search + '%'
                       OR c.[Reference3]               LIKE '%' + @Search + '%')", conn))
                    {
                        countCmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        countCmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = search ?? "";
                        totalCount = (int)await countCmd.ExecuteScalarAsync();
                    }

                    // ================== FETCH GROUPED BY MEMBER ==================
                    // Each row = one member with aggregated amounts + their individual account breakdown
                    using (var cmd = new SqlCommand(@"
                WITH MemberSummary AS (
                    SELECT
                        ca.[CustomerId],
                        c.[Individual_FirstName]            AS FirstName,
                        c.[Individual_LastName]             AS LastName,
                        c.[Individual_PayrollNumbers]       AS PayrollNumber,
                        c.[Reference2]                      AS MemberNumber,
                        c.[Reference3]                      AS PFNumber,
                        c.[Address_MobileLine]              AS Mobile,
                        COUNT(e.[Id])                       AS EntryCount,
                        ISNULL(SUM(e.[Principal]), 0)       AS TotalPrincipal,
                        ISNULL(SUM(e.[Interest]),  0)       AS TotalInterest,
                        ISNULL(SUM(e.[Principal] + e.[Interest]), 0) AS TotalAmount,
                        MAX(e.[Status])                     AS MaxStatus,
                        MAX(e.[CreatedDate])                AS LastEntryDate,
                        MAX(e.[Reference])                  AS LastReference
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries] e
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca ON ca.[Id] = e.[CustomerAccountId]
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c          ON c.[Id]  = ca.[CustomerId]
                    WHERE e.[CreditBatchId] = @BatchId
                      AND (@Search = ''
                           OR c.[Individual_FirstName]      LIKE '%' + @Search + '%'
                           OR c.[Individual_LastName]       LIKE '%' + @Search + '%'
                           OR c.[Individual_PayrollNumbers] LIKE '%' + @Search + '%'
                           OR c.[Reference3]               LIKE '%' + @Search + '%')
                    GROUP BY
                        ca.[CustomerId],
                        c.[Individual_FirstName],
                        c.[Individual_LastName],
                        c.[Individual_PayrollNumbers],
                        c.[Reference2],
                        c.[Reference3],
                        c.[Address_MobileLine]
                )
                SELECT *
                FROM MemberSummary
                ORDER BY FirstName ASC, LastName ASC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = search ?? "";
                        cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = offset;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Guid? customerId = reader["CustomerId"] != DBNull.Value
                                    ? (Guid?)reader.GetGuid(reader.GetOrdinal("CustomerId"))
                                    : null;

                                string firstName = reader["FirstName"]?.ToString() ?? "";
                                string lastName = reader["LastName"]?.ToString() ?? "";
                                int maxStatus = Convert.ToInt32(reader["MaxStatus"] == DBNull.Value ? 0 : reader["MaxStatus"]);

                                members.Add(new
                                {
                                    customerId = customerId,
                                    memberName = $"{firstName} {lastName}".Trim(),
                                    payrollNumber = reader["PayrollNumber"]?.ToString(),
                                    memberNumber = reader["MemberNumber"]?.ToString(),
                                    pfNumber = reader["PFNumber"]?.ToString(),
                                    mobile = reader["Mobile"]?.ToString(),
                                    entryCount = Convert.ToInt32(reader["EntryCount"]),
                                    totalPrincipal = Convert.ToDecimal(reader["TotalPrincipal"]),
                                    totalInterest = Convert.ToDecimal(reader["TotalInterest"]),
                                    totalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    lastReference = reader["LastReference"]?.ToString(),
                                    lastEntryDate = reader["LastEntryDate"] != DBNull.Value
                                                        ? (DateTime?)Convert.ToDateTime(reader["LastEntryDate"])
                                                        : null,
                                    status = maxStatus,
                                    statusDescription = maxStatus == 1 ? "Posted"
                                                      : maxStatus == 2 ? "Reversed"
                                                      : "Imported"
                                });
                            }
                        }
                    }

                    // ================== PER-MEMBER ACCOUNT BREAKDOWN ==================
                    // For each member on this page, load their individual entries
                    var memberIds = members
                        .Select(m => (Guid?)m.GetType().GetProperty("customerId").GetValue(m))
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList();

                    var breakdownMap = new Dictionary<Guid, List<object>>();

                    if (memberIds.Any())
                    {
                        // Build IN clause with parameters
                        var paramNames = memberIds.Select((_, i) => $"@Cid{i}").ToList();
                        var inClause = string.Join(",", paramNames);

                        using (var bdCmd = new SqlCommand($@"
                    SELECT
                        e.[Id],
                        ca.[CustomerId],
                        e.[CustomerAccountId],
                        e.[ChartOfAccountId],
                        e.[Principal],
                        e.[Interest],
                        e.[Balance],
                        e.[Reference],
                        e.[Status],
                        e.[CreatedDate],
                        coa.[AccountCode]   AS GLAccountCode,
                        coa.[AccountName]   AS GLAccountName,
                        ca.[CustomerAccountType_TargetProductCode] AS TargetProductCode,
                        CASE e.[Status]
                            WHEN 0 THEN 'Imported'
                            WHEN 1 THEN 'Posted'
                            WHEN 2 THEN 'Reversed'
                            ELSE 'Unknown'
                        END AS StatusDescription
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries] e
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca ON ca.[Id]  = e.[CustomerAccountId]
                    LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_ChartOfAccounts] coa  ON coa.[Id] = e.[ChartOfAccountId]
                    WHERE e.[CreditBatchId] = @BatchId
                      AND ca.[CustomerId] IN ({inClause})
                    ORDER BY ca.[CustomerId], e.[CreatedDate] ASC", conn))
                        {
                            bdCmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                            for (int i = 0; i < memberIds.Count; i++)
                                bdCmd.Parameters.Add(paramNames[i], SqlDbType.UniqueIdentifier).Value = memberIds[i];

                            using (var r = await bdCmd.ExecuteReaderAsync())
                            {
                                while (await r.ReadAsync())
                                {
                                    Guid custId = r.GetGuid(r.GetOrdinal("CustomerId"));
                                    if (!breakdownMap.ContainsKey(custId))
                                        breakdownMap[custId] = new List<object>();

                                    breakdownMap[custId].Add(new
                                    {
                                        id = r.GetGuid(r.GetOrdinal("Id")),
                                        customerAccountId = r["CustomerAccountId"] != DBNull.Value ? (Guid?)r.GetGuid(r.GetOrdinal("CustomerAccountId")) : null,
                                        chartOfAccountId = r["ChartOfAccountId"] != DBNull.Value ? (Guid?)r.GetGuid(r.GetOrdinal("ChartOfAccountId")) : null,
                                        glAccountCode = r["GLAccountCode"] != DBNull.Value ? Convert.ToInt32(r["GLAccountCode"]) : 0,
                                        glAccountName = r["GLAccountName"]?.ToString(),
                                        targetProductCode = r["TargetProductCode"] != DBNull.Value ? Convert.ToInt32(r["TargetProductCode"]) : 0,
                                        principal = Convert.ToDecimal(r["Principal"] == DBNull.Value ? 0 : r["Principal"]),
                                        interest = Convert.ToDecimal(r["Interest"] == DBNull.Value ? 0 : r["Interest"]),
                                        balance = Convert.ToDecimal(r["Balance"] == DBNull.Value ? 0 : r["Balance"]),
                                        amount = Convert.ToDecimal(r["Principal"] == DBNull.Value ? 0 : r["Principal"])
                                                          + Convert.ToDecimal(r["Interest"] == DBNull.Value ? 0 : r["Interest"]),
                                        reference = r["Reference"]?.ToString(),
                                        status = Convert.ToInt32(r["Status"] == DBNull.Value ? 0 : r["Status"]),
                                        statusDescription = r["StatusDescription"]?.ToString(),
                                        createdDate = r["CreatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["CreatedDate"]) : null
                                    });
                                }
                            }
                        }

                        // Attach breakdown to each member
                        members = members.Select(m => {
                            var type = m.GetType();
                            var custId = (Guid?)type.GetProperty("customerId").GetValue(m);
                            var breakdown = custId.HasValue && breakdownMap.ContainsKey(custId.Value)
                                                ? breakdownMap[custId.Value]
                                                : new List<object>();
                            return (object)new
                            {
                                customerId = type.GetProperty("customerId").GetValue(m),
                                memberName = type.GetProperty("memberName").GetValue(m),
                                payrollNumber = type.GetProperty("payrollNumber").GetValue(m),
                                memberNumber = type.GetProperty("memberNumber").GetValue(m),
                                pfNumber = type.GetProperty("pfNumber").GetValue(m),
                                mobile = type.GetProperty("mobile").GetValue(m),
                                entryCount = type.GetProperty("entryCount").GetValue(m),
                                totalPrincipal = type.GetProperty("totalPrincipal").GetValue(m),
                                totalInterest = type.GetProperty("totalInterest").GetValue(m),
                                totalAmount = type.GetProperty("totalAmount").GetValue(m),
                                lastReference = type.GetProperty("lastReference").GetValue(m),
                                lastEntryDate = type.GetProperty("lastEntryDate").GetValue(m),
                                status = type.GetProperty("status").GetValue(m),
                                statusDescription = type.GetProperty("statusDescription").GetValue(m),
                                accountBreakdown = breakdown
                            };
                        }).ToList();
                    }

                    int totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = $"Found {totalCount} members across {grandTotalEntries} entries",
                        Data = new
                        {
                            // ── Grand totals (whole batch, not just this page) ──
                            Summary = new
                            {
                                TotalMembers = grandTotalMembers,
                                TotalEntries = grandTotalEntries,
                                TotalPrincipal = grandTotalPrincipal,
                                TotalInterest = grandTotalInterest,
                                TotalAmount = grandTotalAmount
                            },

                            // ── Pagination ──
                            BatchId = batchId,
                            TotalCount = totalCount,
                            Page = page,
                            PageSize = pageSize,
                            TotalPages = totalPages,
                            HasNextPage = page < totalPages,
                            HasPreviousPage = page > 1,

                            // ── Member rows (grouped) ──
                            Members = members
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to fetch batch entries: {ex.Message} | Inner: {ex.InnerException?.Message}"
                });
            }
        }


        [HttpGet]
        [Route("entries/by-customer")]
        public async Task<IHttpActionResult> FindCreditBatchEntriesByCustomerId(Guid CustomerId, int CreditBatchType)
        {
            var serviceHeader = CreateServiceHeader();

            try
            {


                var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCustomerId(CreditBatchType, CustomerId, serviceHeader);
  
                return Ok(creditBatchEntries);
            }

            catch (Exception ex)
            {


                return InternalServerError(ex);
            }


        }


        [HttpPost]
        [Route("creditbatch/{batchId}/import")]
        public async Task<IHttpActionResult> CreditBatchImport(Guid batchId)
        {
            HttpContext.Current.Server.ScriptTimeout = 1800;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0)
                    return BadRequest("No file uploaded.");

                var postedFile = httpRequest.Files[0];
                if (postedFile == null || postedFile.ContentLength == 0)
                    return BadRequest("Invalid file.");

                var uploadDirectory = @"C:\swiftfin_file_uploads\";
                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                var fileName = Path.GetFileName(postedFile.FileName);
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
                var filePath = Path.Combine(uploadDirectory, uniqueFileName);
                postedFile.SaveAs(filePath);

                var matchedCount = 0;
                var mismatchEntries = new List<object>();
                var rowCount = 0;
                decimal totalAmountPosted = 0m;
                int postedCount = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    Guid branchId = Guid.Empty;
                    int batchType = 0;
                    bool fuzzyMatching = false;
                    Guid creditTypeId = Guid.Empty;
                    int batchStatus = 0;
                    DateTime batchValueDate = DateTime.Now;

                    using (var cmd = new SqlCommand(@"
                SELECT cb.BranchId, cb.Type, cb.FuzzyMatching, cb.CreditTypeId, cb.Status, cb.ValueDate
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatches] cb
                WHERE cb.Id = @BatchId", connection))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return NotFound();

                            branchId = reader.GetGuid(reader.GetOrdinal("BranchId"));
                            batchType = Convert.ToInt32(reader["Type"]);
                            fuzzyMatching = Convert.ToBoolean(reader["FuzzyMatching"]);
                            creditTypeId = reader.GetGuid(reader.GetOrdinal("CreditTypeId"));
                            batchStatus = Convert.ToInt32(reader["Status"]);
                            var vdOrd = reader.GetOrdinal("ValueDate");
                            if (!reader.IsDBNull(vdOrd))
                                batchValueDate = reader.GetDateTime(vdOrd);
                        }
                    }

                    if (batchStatus != 1)
                        return BadRequest($"Batch is not Pending (status: {batchStatus}). Only Pending batches can be imported.");

                    using (var streamReader = new StreamReader(filePath))
                    using (var csvReader = new CsvReader(streamReader, hasHeaders: true))
                    {
                        while (csvReader.ReadNextRecord())
                        {
                            rowCount++;

                            switch ((CreditBatchType)batchType)
                            {
                                // ─────────────────────────────────────────────────────────
                                case CreditBatchType.Payout:
                                    {
                                        if (csvReader.FieldCount != 5)
                                        {
                                            mismatchEntries.Add(new { Row = rowCount, Reason = "Expected 5 columns" });
                                            break;
                                        }

                                        string payrollNum = csvReader[0];
                                        string beneficiary = csvReader[1];
                                        string amountRaw = csvReader[2];
                                        string productCodeRaw = csvReader[3];
                                        string reference = csvReader[4];

                                        if (!decimal.TryParse(amountRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, beneficiary, amountRaw, productCodeRaw, null, reference, null, null,
                                                Truncate($"Row #{rowCount} ~ unable to parse amount {amountRaw}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"Cannot parse amount: {amountRaw}" });
                                            break;
                                        }

                                        if (!int.TryParse(productCodeRaw, out int productCode))
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, beneficiary, amountRaw, productCodeRaw, null, reference, null, null,
                                                Truncate($"Row #{rowCount} ~ unable to parse product code {productCodeRaw}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"Cannot parse product code: {productCodeRaw}" });
                                            break;
                                        }

                                        Guid savProductId = Guid.Empty;
                                        string savProductDesc = "";
                                        using (var cmd = new SqlCommand(@"
                                SELECT TOP 1 Id, Description
                                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts]
                                WHERE Code = @Code", connection))
                                        {
                                            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = productCode;
                                            using (var r = await cmd.ExecuteReaderAsync())
                                            {
                                                if (await r.ReadAsync())
                                                {
                                                    savProductId = r.GetGuid(r.GetOrdinal("Id"));
                                                    savProductDesc = r["Description"]?.ToString() ?? "";
                                                }
                                            }
                                        }

                                        if (savProductId == Guid.Empty)
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, beneficiary, amountRaw, productCodeRaw, null, reference, null, null,
                                                Truncate($"Row #{rowCount} ~ no match for savings product code {productCode}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"No savings product for code {productCode}" });
                                            break;
                                        }

                                        var accounts = await FindAccountsByProductAndPayroll(connection, savProductId, payrollNum);

                                        if (!accounts.Any())
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, beneficiary, amountRaw, productCodeRaw, null, reference, null, null,
                                                Truncate($"Row #{rowCount} ~ no customer account found for payroll {payrollNum}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"No account for payroll {payrollNum}" });
                                            break;
                                        }

                                        if (accounts.Count > 1)
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, beneficiary, amountRaw, productCodeRaw, null, reference, null, null,
                                                Truncate($"Row #{rowCount} ~ found {accounts.Count} accounts for payroll {payrollNum}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"Ambiguous: {accounts.Count} accounts for payroll {payrollNum}" });
                                            break;
                                        }

                                        var acc = accounts[0];
                                        await InsertCreditBatchEntry(connection, batchId, acc.Id, acc.ChartOfAccountId, amount, 0, 0, beneficiary, payrollNum);
                                        totalAmountPosted += amount;
                                        postedCount++;
                                        matchedCount++;
                                        break;
                                    }

                                // ─────────────────────────────────────────────────────────
                                case CreditBatchType.CheckOff:
                                    {
                                        if (string.IsNullOrWhiteSpace(csvReader[0])) break;
                                        if (csvReader.FieldCount != 6) break;

                                        string payrollNum = csvReader[0];
                                        string productTypeCode = csvReader[5].Trim();
                                        string rawAmount = csvReader[3].Replace(",", "").Trim();
                                        string csvReference = csvReader[4];

                                        if (!decimal.TryParse(rawAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal total))
                                        {
                                            await InsertBatchDiscrepancy(connection, batchId, payrollNum, null, rawAmount, productTypeCode, null, csvReference, null, null,
                                                Truncate($"Row #{rowCount} ~ unable to parse amount {rawAmount}"));
                                            mismatchEntries.Add(new { Row = rowCount, Reason = $"Cannot parse amount: {rawAmount}" });
                                            break;
                                        }

                                        bool ok = false;

                                        if (!fuzzyMatching)
                                        {
                                            if (productTypeCode == "DEF")
                                                ok = await ProcessCheckOffLoan(connection, batchId, payrollNum, total, 7, csvReference, rowCount, mismatchEntries, batchValueDate);
                                            else if (productTypeCode == "DEP")
                                                // FIX: DEP now uses split logic (entrance fee → share capital → deposit)
                                                ok = await ProcessCheckOffDepositWithSplit(connection, batchId, branchId, payrollNum, total, csvReference, rowCount, mismatchEntries);
                                            else if (productTypeCode == "RBF")
                                                ok = await ProcessCheckOffSavings(connection, batchId, branchId, payrollNum, total, 3, csvReference, rowCount, mismatchEntries);
                                            else if (productTypeCode == "SHP")
                                                ok = await ProcessCheckOffSavings(connection, batchId, branchId, payrollNum, total, 2, csvReference, rowCount, mismatchEntries);
                                            else
                                                ok = await ProcessMultiLoanCheckOff(connection, batchId, payrollNum, total, csvReference, rowCount, mismatchEntries, batchValueDate);
                                        }
                                        else
                                        {
                                            ok = await ProcessFuzzyCheckOff(connection, batchId, payrollNum, total, productTypeCode, csvReference, rowCount, mismatchEntries);
                                        }

                                        if (ok)
                                        {
                                            matchedCount++;
                                            totalAmountPosted += total;
                                            postedCount++;
                                        }

                                        break;
                                    }

                                // ─────────────────────────────────────────────────────────
                                case CreditBatchType.CashPickup:
                                case CreditBatchType.SundryPayments:
                                    {
                                        if (csvReader.FieldCount != 4)
                                        {
                                            mismatchEntries.Add(new { Row = rowCount, Reason = "Expected 4 columns" });
                                            break;
                                        }

                                        await InsertCreditBatchEntry(connection, batchId,
                                            customerAccountId: Guid.Empty,
                                            chartOfAccountId: Guid.Empty,
                                            principal: 0,
                                            interest: 0,
                                            balance: 0,
                                            beneficiary: csvReader[0],
                                            reference: csvReader[1]);

                                        matchedCount++;
                                        break;
                                    }
                            }
                        }
                    }

                    // ================== LOAD DISCREPANCIES (ERRORS ONLY) ==================
                    var discrepancies = new List<object>();
                    using (var cmd = new SqlCommand(@"
                SELECT
                    d.Column1   AS PayrollNumber,
                    d.Column2   AS Beneficiary,
                    d.Column3   AS Amount,
                    d.Column4   AS ProductCode,
                    d.Column6   AS Reference,
                    d.Remarks,
                    d.CreatedDate,
                    c.Individual_FirstName,
                    c.Individual_LastName,
                    c.Address_MobileLine
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchDiscrepancies] d
                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
                    ON c.Individual_PayrollNumbers = d.Column1
                    OR c.Reference3               = d.Column1
                WHERE d.CreditBatchId = @BatchId
                  AND (
                        d.Remarks LIKE '%no customer%'
                     OR d.Remarks LIKE '%no match%'
                     OR d.Remarks LIKE '%no account%'
                     OR d.Remarks LIKE '%no active%'
                     OR d.Remarks LIKE '%no loan%'
                     OR d.Remarks LIKE '%no savings%'
                     OR d.Remarks LIKE '%no standing%'
                     OR d.Remarks LIKE '%unable to parse%'
                     OR d.Remarks LIKE '%ambiguous%'
                     OR d.Remarks LIKE '%not found%'
                     OR d.Remarks LIKE '%missing%'
                     OR d.Remarks LIKE '%not configured%'
                     OR d.Remarks LIKE '%could not be posted%'
                     OR d.Remarks LIKE '%excess%'
                  )
                ORDER BY d.CreatedDate ASC", connection))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string remarks = reader["Remarks"]?.ToString() ?? "";
                                string payroll = reader["PayrollNumber"]?.ToString();
                                string productCode = reader["ProductCode"]?.ToString();
                                string amount = reader["Amount"]?.ToString();
                                string firstName = reader["Individual_FirstName"]?.ToString();
                                string lastName = reader["Individual_LastName"]?.ToString();
                                string memberName = (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
                                                        ? $"{firstName} {lastName}".Trim()
                                                        : null;

                                discrepancies.Add(new
                                {
                                    PayrollNumber = payroll,
                                    MemberName = memberName,
                                    MemberMobile = reader["Address_MobileLine"]?.ToString(),
                                    Amount = amount,
                                    ProductCode = productCode,
                                    Reference = reader["Reference"]?.ToString(),
                                    Remarks = remarks,
                                    ErrorCategory = GetErrorCategory(remarks),
                                    Solution = GetSolution(remarks, payroll, productCode, amount),
                                    Date = Convert.ToDateTime(reader["CreatedDate"])
                                });
                            }
                        }
                    }

                    return Ok(new
                    {
                        BatchId = batchId,
                        FileName = uniqueFileName,
                        SavedPath = filePath,
                        TotalRows = rowCount,
                        MatchedCount = matchedCount,
                        MismatchCount = mismatchEntries.Count,
                        PostedCount = postedCount,
                        TotalAmountPosted = totalAmountPosted,
                        Mismatches = mismatchEntries,
                        Errors = discrepancies
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // DEP SPLIT LOGIC
        // Priority:
        //   1. ENTRANCE FEE (code=4) — if total paid < 3000, deduct shortfall first
        //   2. SHARE CAPITAL (code=2) — if total paid < 30000, deduct 2000
        //   3. DEPOSITS (code=1) — remainder goes here
        // Balance = ABS(SUM(je.Amount)) since credits are stored as negative
        // ─────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────
        // DEP SPLIT LOGIC — only INSERT discrepancy on actual failure
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> ProcessCheckOffDepositWithSplit(
            SqlConnection connection, Guid batchId, Guid branchId,
            string payrollNum, decimal amount, string reference,
            int rowCount, List<object> mismatches)
        {
            Guid customerId = Guid.Empty;
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 Id
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers]
        WHERE Individual_PayrollNumbers = @PayrollNumber", connection))
            {
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                    customerId = (Guid)result;
            }

            if (customerId == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    amount.ToString(), "DEP", null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no customer found for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No customer for payroll {payrollNum}" });
                return false;
            }

            var products = new Dictionary<int, (Guid ProductId, Guid CoAId)>();
            using (var cmd = new SqlCommand(@"
        SELECT Code, Id, ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts]
        WHERE Code IN (1, 2, 4)
          AND ChartOfAccountId IS NOT NULL", connection))
            {
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        int code = Convert.ToInt32(r["Code"]);
                        Guid productId = r.GetGuid(r.GetOrdinal("Id"));
                        Guid coaId = r.GetGuid(r.GetOrdinal("ChartOfAccountId"));
                        products[code] = (productId, coaId);
                    }
                }
            }

            if (!products.ContainsKey(1))
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    amount.ToString(), "DEP", null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ DEPOSITS savings product (code=1) not found or missing CoA"));
                mismatches.Add(new { Row = rowCount, Reason = "DEPOSITS product (code=1) not configured" });
                return false;
            }

            async Task<Guid> GetOrCreateAccount(int productCode)
            {
                if (!products.ContainsKey(productCode)) return Guid.Empty;
                var (productId, _) = products[productCode];

                using (var cmd = new SqlCommand(@"
            SELECT TOP 1 Id
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
            WHERE CustomerId = @CustomerId
              AND CustomerAccountType_TargetProductId = @ProductId", connection))
                {
                    cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                    cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = productId;
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                        return (Guid)result;
                }

                if (productCode == 1) return Guid.Empty;

                var newId = Guid.NewGuid();
                using (var cmd = new SqlCommand(@"
            INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
                (Id, CustomerId, BranchId, CustomerAccountType_ProductCode,
                 CustomerAccountType_TargetProductId, CustomerAccountType_TargetProductCode,
                 ScoredLoanDisbursementProductCode, ScoredLoanLimit,
                 Status, RecordStatus, SequentialId, CreatedBy, CreatedDate)
            VALUES
                (@Id, @CustomerId, @BranchId, @ProdCode,
                 @TargetProductId, @TargetProductCode,
                 0, 0, 0, 2, @SeqId, 'System', @CreatedDate)", connection))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = newId;
                    cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                    cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = branchId;
                    cmd.Parameters.Add("@ProdCode", SqlDbType.TinyInt).Value = (byte)4;
                    cmd.Parameters.Add("@TargetProductId", SqlDbType.UniqueIdentifier).Value = productId;
                    cmd.Parameters.Add("@TargetProductCode", SqlDbType.SmallInt).Value = (short)productCode;
                    cmd.Parameters.Add("@SeqId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = DateTime.Now;
                    await cmd.ExecuteNonQueryAsync();
                }
                return newId;
            }

            async Task<decimal> GetAccountBalance(Guid accountId, Guid coaId)
            {
                if (accountId == Guid.Empty) return 0m;
                using (var cmd = new SqlCommand(@"
            SELECT ISNULL(ABS(SUM(je.Amount)), 0)
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je
            WHERE je.CustomerAccountId = @AccountId
              AND je.ChartOfAccountId  = @CoAId", connection))
                {
                    cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = accountId;
                    cmd.Parameters.Add("@CoAId", SqlDbType.UniqueIdentifier).Value = coaId;
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
                }
            }

            decimal remaining = amount;
            bool anyPosted = false;

            // ── STEP 1: ENTRANCE FEE (code=4) — threshold 3000 ────────────────────
            const decimal ENTRANCE_FEE_THRESHOLD = 3000m;

            if (products.ContainsKey(4))
            {
                Guid entranceFeeAccId = await GetOrCreateAccount(4);
                Guid entranceFeeCoA = products[4].CoAId;
                decimal entranceFeePaid = await GetAccountBalance(entranceFeeAccId, entranceFeeCoA);
                decimal entranceFeeShortfall = Math.Max(ENTRANCE_FEE_THRESHOLD - entranceFeePaid, 0m);

                if (entranceFeeShortfall > 0m && remaining > 0m)
                {
                    decimal toEntranceFee = Math.Min(entranceFeeShortfall, remaining);

                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: entranceFeeAccId,
                        chartOfAccountId: entranceFeeCoA,
                        principal: toEntranceFee,
                        interest: 0m,
                        balance: 0m,
                        beneficiary: amount.ToString(),
                        reference: $"ENTRANCE FEE - {reference}");

                    // ── audit to debug only, NOT to discrepancies table ──
                    System.Diagnostics.Debug.WriteLine(
                        $"[DEP SPLIT] {payrollNum} Row#{rowCount}: " +
                        $"{toEntranceFee:N2} → ENTRANCE FEE " +
                        $"(paid={entranceFeePaid:N2} shortfall={entranceFeeShortfall:N2})");

                    remaining -= toEntranceFee;
                    anyPosted = true;
                }
                // entrance fee fully paid — skip silently
            }
            else
            {
                // ── product missing is a WARNING, not a hard failure — log to debug ──
                System.Diagnostics.Debug.WriteLine(
                    $"[DEP SPLIT] {payrollNum} Row#{rowCount}: ENTRANCE FEE product (code=4) not found — skipping");
            }

            // ── STEP 2: SHARE CAPITAL (code=2) — threshold 30000, deduct 2000 ──────
            const decimal SHARE_CAPITAL_THRESHOLD = 30000m;
            const decimal SHARE_CAPITAL_DEDUCTION = 2000m;

            if (products.ContainsKey(2) && remaining > 0m)
            {
                Guid shareCapitalAccId = await GetOrCreateAccount(2);
                Guid shareCapitalCoA = products[2].CoAId;
                decimal shareCapitalPaid = await GetAccountBalance(shareCapitalAccId, shareCapitalCoA);

                if (shareCapitalPaid < SHARE_CAPITAL_THRESHOLD)
                {
                    decimal toShareCapital = Math.Min(SHARE_CAPITAL_DEDUCTION, remaining);

                    if (toShareCapital > 0m)
                    {
                        await InsertCreditBatchEntry(connection, batchId,
                            customerAccountId: shareCapitalAccId,
                            chartOfAccountId: shareCapitalCoA,
                            principal: toShareCapital,
                            interest: 0m,
                            balance: 0m,
                            beneficiary: amount.ToString(),
                            reference: $"SHARE CAPITAL - {reference}");

                        System.Diagnostics.Debug.WriteLine(
                            $"[DEP SPLIT] {payrollNum} Row#{rowCount}: " +
                            $"{toShareCapital:N2} → SHARE CAPITAL " +
                            $"(paid={shareCapitalPaid:N2} threshold={SHARE_CAPITAL_THRESHOLD:N2})");

                        remaining -= toShareCapital;
                        anyPosted = true;
                    }
                }
                // share capital fully paid — skip silently
            }
            else if (!products.ContainsKey(2))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[DEP SPLIT] {payrollNum} Row#{rowCount}: SHARE CAPITAL product (code=2) not found — skipping");
            }

            // ── STEP 3: DEPOSITS (code=1) — remainder ──────────────────────────────
            if (remaining > 0m)
            {
                Guid depositAccId = await GetOrCreateAccount(1);

                if (depositAccId == Guid.Empty)
                {
                    // ── REAL FAILURE → goes to discrepancies table ──
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        remaining.ToString(), "DEP", null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ no DEPOSITS account (code=1) found for payroll {payrollNum} — {remaining:N2} could not be posted. Create a deposits account for this member."));
                    mismatches.Add(new { Row = rowCount, Reason = $"No DEPOSITS account for payroll {payrollNum} — create one first" });
                }
                else
                {
                    Guid depositCoA = products[1].CoAId;

                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: depositAccId,
                        chartOfAccountId: depositCoA,
                        principal: remaining,
                        interest: 0m,
                        balance: 0m,
                        beneficiary: amount.ToString(),
                        reference: $"DEP - {reference}");

                    System.Diagnostics.Debug.WriteLine(
                        $"[DEP SPLIT] {payrollNum} Row#{rowCount}: " +
                        $"{remaining:N2} → DEPOSITS (split complete)");

                    anyPosted = true;
                }
            }

            return anyPosted;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // SAME-MONTH PAYMENT GUARD
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> HasPrincipalEntryThisMonth(
            SqlConnection conn, Guid customerAccountId, DateTime now)
        {
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 1
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
        WHERE CustomerAccountId = @CustomerAccountId
          AND Principal > 0
          AND YEAR(CreatedDate)  = @Year
          AND MONTH(CreatedDate) = @Month", conn))
            {
                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId;
                cmd.Parameters.Add("@Year", SqlDbType.Int).Value = now.Year;
                cmd.Parameters.Add("@Month", SqlDbType.Int).Value = now.Month;
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // TRUNCATION HELPER
        // ─────────────────────────────────────────────────────────────────────────────
        private static string Truncate(string value, int maxLen = 256)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLen)
                return value;
            return value.Substring(0, maxLen - 1) + "…";
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // INTEREST CALCULATION HELPER
        // ─────────────────────────────────────────────────────────────────────────────
        private static (decimal interest, decimal principal, decimal arrearsInterest, int monthsMissed)
            CalculateReducingBalancePayment(
                decimal outstandingBalance,
                decimal annualPercentageRate,
                decimal payment,
                DateTime? lastPaymentDate,
                DateTime? disbursedDate,
                DateTime now,
                bool isFirstPaymentAfterDisbursementThisMonth,
                bool isSameMonthRepayment)
        {
            if (isFirstPaymentAfterDisbursementThisMonth ||
                isSameMonthRepayment ||
                annualPercentageRate == 0m)
            {
                return (
                    interest: 0m,
                    principal: Math.Min(payment, outstandingBalance),
                    arrearsInterest: 0m,
                    monthsMissed: 0);
            }

            decimal monthlyRate = annualPercentageRate / 100m / 12m;

            DateTime referenceDate =
                lastPaymentDate.HasValue ? lastPaymentDate.Value :
                disbursedDate.HasValue ? disbursedDate.Value :
                now;

            int monthsMissed =
                ((now.Year - referenceDate.Year) * 12)
              + (now.Month - referenceDate.Month) - 1;

            if (monthsMissed < 0) monthsMissed = 0;

            decimal compoundedBalance = outstandingBalance;
            if (monthsMissed > 0)
            {
                compoundedBalance = Math.Round(
                    outstandingBalance * (decimal)Math.Pow((double)(1m + monthlyRate), monthsMissed),
                    2, MidpointRounding.AwayFromZero);
            }

            decimal arrearsInterest = Math.Round(
                compoundedBalance - outstandingBalance,
                2, MidpointRounding.AwayFromZero);

            decimal currentMonthInterest = Math.Round(
                compoundedBalance * monthlyRate,
                2, MidpointRounding.AwayFromZero);

            decimal totalInterest = arrearsInterest + currentMonthInterest;
            decimal principalPortion;

            if (totalInterest >= payment)
            {
                totalInterest = payment;
                principalPortion = 0m;
            }
            else
            {
                principalPortion = payment - totalInterest;
            }

            if (principalPortion > outstandingBalance)
            {
                principalPortion = outstandingBalance;
                decimal combined = principalPortion + totalInterest;
                if (combined > payment)
                {
                    totalInterest = payment - principalPortion;
                    if (totalInterest < 0m) totalInterest = 0m;
                }
            }

            return (
                interest: totalInterest,
                principal: principalPortion,
                arrearsInterest: arrearsInterest,
                monthsMissed: monthsMissed);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // CHECK OFF — single loan (DEF)
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> ProcessCheckOffLoan(
     SqlConnection connection, Guid batchId, string payrollNum,
     decimal contributionAmount, int productCode, string reference,
     int rowCount, List<object> mismatches, DateTime batchValueDate)
        {
            Guid loanProductId = Guid.Empty;
            decimal loanAPR = 0m;
            Guid interestReceivedCoA = Guid.Empty;
            Guid loanAssetCoA = Guid.Empty;

            using (var cmd = new SqlCommand(@"
        SELECT TOP 1
            lp.Id,
            lp.LoanInterest_AnnualPercentageRate,
            lp.ChartOfAccountId,
            lp.InterestReceivedChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp
        WHERE lp.Code = @Code", connection))
            {
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = productCode;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                    {
                        loanProductId = r.GetGuid(r.GetOrdinal("Id"));
                        loanAPR = Convert.ToDecimal(r["LoanInterest_AnnualPercentageRate"]);
                        if (!r.IsDBNull(r.GetOrdinal("ChartOfAccountId")))
                            loanAssetCoA = r.GetGuid(r.GetOrdinal("ChartOfAccountId"));
                        if (!r.IsDBNull(r.GetOrdinal("InterestReceivedChartOfAccountId")))
                            interestReceivedCoA = r.GetGuid(r.GetOrdinal("InterestReceivedChartOfAccountId"));
                    }
                }
            }

            if (loanProductId == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no match for loan product code {productCode}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No loan product for code {productCode}" });
                return false;
            }

            if (loanAssetCoA == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ loan product {productCode} has no ChartOfAccountId configured"));
                mismatches.Add(new { Row = rowCount, Reason = "Loan product missing Loan Asset Chart of Account" });
                return false;
            }

            if (interestReceivedCoA == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ loan product {productCode} has no InterestReceivedChartOfAccountId configured"));
                mismatches.Add(new { Row = rowCount, Reason = "Loan product missing Interest Received Chart of Account" });
                return false;
            }

            var accounts = await FindAccountsByProductAndReference3(connection, loanProductId, payrollNum);
            if (!accounts.Any())
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no loan account for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No loan account for payroll {payrollNum}" });
                return false;
            }

            var targetAccount = accounts[0];

            LoanCaseData loanCase = null;
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1
            lc.Id, lc.CustomerId, lc.LoanProductId,
            lc.DisbursedDate, lc.ReceivedDate,
            lc.TotalLoansBalance, lc.MonthlyPaybackAmount, lc.CaseNumber
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases] lc
        WHERE lc.CustomerId    = @CustomerId
          AND lc.DisbursedDate IS NOT NULL
          AND lc.TotalLoansBalance > 1
        ORDER BY ABS(DATEDIFF(SECOND, lc.DisbursedDate, GETDATE()))", connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = targetAccount.CustomerId;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                    {
                        loanCase = new LoanCaseData
                        {
                            Id = r.GetGuid(r.GetOrdinal("Id")),
                            CustomerId = r.GetGuid(r.GetOrdinal("CustomerId")),
                            DisbursedDate = r.IsDBNull(r.GetOrdinal("DisbursedDate")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("DisbursedDate")),
                            LastPaymentDate = r.IsDBNull(r.GetOrdinal("ReceivedDate")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("ReceivedDate")),
                            TotalLoansBalance = Convert.ToDecimal(r["TotalLoansBalance"]),
                            MonthlyPaybackAmount = Convert.ToDecimal(r["MonthlyPaybackAmount"]),
                            CaseNumber = Convert.ToInt32(r["CaseNumber"])
                        };
                    }
                }
            }

            if (loanCase == null)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no active loan case for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No active loan case for payroll {payrollNum}" });
                return false;
            }

            var now = batchValueDate;

            bool isFirstPayment = loanCase.LastPaymentDate == null;
            bool isSameMonthByDate = loanCase.LastPaymentDate.HasValue
                                        && loanCase.LastPaymentDate.Value.Year == now.Year
                                        && loanCase.LastPaymentDate.Value.Month == now.Month;
            bool isSameMonthByEntry = await HasPrincipalEntryThisMonth(connection, targetAccount.Id, now);
            bool isSameMonthPayment = isSameMonthByDate || isSameMonthByEntry;
            bool isDisbursedThisMonth = loanCase.DisbursedDate.HasValue
                                        && loanCase.DisbursedDate.Value.Year == now.Year
                                        && loanCase.DisbursedDate.Value.Month == now.Month;

            var (interestPortion, principalPortion, arrearsInterest, monthsMissed) =
                CalculateReducingBalancePayment(
                    outstandingBalance: loanCase.TotalLoansBalance,
                    annualPercentageRate: loanAPR,
                    payment: contributionAmount,
                    lastPaymentDate: loanCase.LastPaymentDate,
                    disbursedDate: loanCase.DisbursedDate,
                    now: now,
                    isFirstPaymentAfterDisbursementThisMonth: isDisbursedThisMonth && isFirstPayment,
                    isSameMonthRepayment: isSameMonthPayment);

            decimal newBalance = Math.Max(loanCase.TotalLoansBalance - principalPortion, 0m);

            if (principalPortion > 0m)
            {
                await InsertCreditBatchEntry(connection, batchId,
                    customerAccountId: targetAccount.Id,
                    chartOfAccountId: loanAssetCoA,
                    principal: principalPortion,
                    interest: 0m,
                    balance: newBalance,
                    beneficiary: contributionAmount.ToString(),
                    reference: $"{loanCase.CaseNumber} - PRINCIPAL - {reference}");
            }

            if (interestPortion > 0m)
            {
                await InsertCreditBatchEntry(connection, batchId,
                    customerAccountId: targetAccount.Id,
                    chartOfAccountId: interestReceivedCoA,
                    principal: 0m,
                    interest: interestPortion,
                    balance: 0m,
                    beneficiary: contributionAmount.ToString(),
                    reference: $"{loanCase.CaseNumber} - INTEREST - {reference}");
            }

            // ── audit to debug only, NOT to discrepancies table ──
            System.Diagnostics.Debug.WriteLine(
                $"[DEF] {payrollNum} Case#{loanCase.CaseNumber} Row#{rowCount}: " +
                $"APR={loanAPR}% Miss={monthsMissed} " +
                $"ArrInt={arrearsInterest} CurInt={interestPortion - arrearsInterest} " +
                $"TotInt={interestPortion} Prin={principalPortion} " +
                $"OldBal={loanCase.TotalLoansBalance} NewBal={newBalance} " +
                $"SameMonth={isSameMonthPayment}");

            if (principalPortion > 0m)
            {
                using (var cmd = new SqlCommand(@"
            UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases]
            SET ReceivedDate = @PaymentDate
            WHERE Id = @LoanCaseId", connection))
                {
                    cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value = now;
                    cmd.Parameters.Add("@LoanCaseId", SqlDbType.UniqueIdentifier).Value = loanCase.Id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return true;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // CHECK OFF — multi-loan split
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> ProcessMultiLoanCheckOff(
    SqlConnection connection, Guid batchId, string payrollNum,
    decimal total, string reference, int rowCount, List<object> mismatches,
    DateTime batchValueDate)
        {
            Guid customerId = Guid.Empty;
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 Id
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers]
        WHERE Reference3 = @PayrollNumber", connection))
            {
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                    customerId = (Guid)result;
            }

            if (customerId == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    total.ToString(), null, null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no customer found for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No customer for payroll {payrollNum}" });
                return false;
            }

            var loanCases = new List<LoanCaseData>();
            using (var cmd = new SqlCommand(@"
        SELECT
            lc.Id, lc.CustomerId, lc.LoanProductId,
            lc.DisbursedDate, lc.ReceivedDate,
            lc.TotalLoansBalance, lc.MonthlyPaybackAmount, lc.CaseNumber,
            lp.Code                             AS ProductCode,
            lp.LoanInterest_AnnualPercentageRate,
            lp.ChartOfAccountId                 AS LoanAssetCoA,
            lp.InterestReceivedChartOfAccountId AS InterestIncomeCoA
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases] lc
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp
            ON lc.LoanProductId = lp.Id
        WHERE lc.CustomerId    = @CustomerId
          AND lc.DisbursedDate IS NOT NULL
          AND lc.TotalLoansBalance > 1
        ORDER BY lc.DisbursedDate ASC", connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        loanCases.Add(new LoanCaseData
                        {
                            Id = r.GetGuid(r.GetOrdinal("Id")),
                            LoanProductId = r.GetGuid(r.GetOrdinal("LoanProductId")),
                            DisbursedDate = r.IsDBNull(r.GetOrdinal("DisbursedDate")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("DisbursedDate")),
                            LastPaymentDate = r.IsDBNull(r.GetOrdinal("ReceivedDate")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("ReceivedDate")),
                            TotalLoansBalance = Convert.ToDecimal(r["TotalLoansBalance"]),
                            MonthlyPaybackAmount = Convert.ToDecimal(r["MonthlyPaybackAmount"]),
                            CaseNumber = Convert.ToInt32(r["CaseNumber"]),
                            ProductCode = Convert.ToInt32(r["ProductCode"]),
                            AnnualPercentageRate = Convert.ToDecimal(r["LoanInterest_AnnualPercentageRate"]),
                            LoanAssetCoA = r.IsDBNull(r.GetOrdinal("LoanAssetCoA")) ? Guid.Empty : r.GetGuid(r.GetOrdinal("LoanAssetCoA")),
                            InterestIncomeCoA = r.IsDBNull(r.GetOrdinal("InterestIncomeCoA")) ? Guid.Empty : r.GetGuid(r.GetOrdinal("InterestIncomeCoA"))
                        });
                    }
                }
            }

            if (!loanCases.Any())
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    total.ToString(), null, null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no active loan cases for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No active loan cases for payroll {payrollNum}" });
                return false;
            }

            decimal remaining = total;
            bool anyPosted = false;
            var now = batchValueDate;

            foreach (var loanCase in loanCases)
            {
                if (loanCase.MonthlyPaybackAmount <= 0m)
                {
                    // ── skipped loan — debug only, not a discrepancy ──
                    System.Diagnostics.Debug.WriteLine(
                        $"[MULTI-LOAN] {payrollNum} Case#{loanCase.CaseNumber} Row#{rowCount}: skipped — MonthlyPaybackAmount=0");
                    continue;
                }

                decimal monthlyPayment = Math.Min(loanCase.MonthlyPaybackAmount, remaining);

                if (monthlyPayment <= 0m)
                {
                    // ── no remaining — debug only ──
                    System.Diagnostics.Debug.WriteLine(
                        $"[MULTI-LOAN] {payrollNum} Case#{loanCase.CaseNumber} Row#{rowCount}: no remaining (remaining={remaining:N2})");
                    continue;
                }

                if (loanCase.LoanAssetCoA == Guid.Empty)
                {
                    // ── config error — IS a real failure ──
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        total.ToString(), loanCase.ProductCode.ToString(), null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ loan case {loanCase.CaseNumber} has no Loan Asset CoA (ChartOfAccountId)"));
                    continue;
                }

                if (loanCase.InterestIncomeCoA == Guid.Empty)
                {
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        total.ToString(), loanCase.ProductCode.ToString(), null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ loan case {loanCase.CaseNumber} has no Interest Income CoA (InterestReceivedChartOfAccountId)"));
                    continue;
                }

                var loanAccounts = await FindAccountsByProductIdDirect(connection, loanCase.LoanProductId, customerId);
                if (!loanAccounts.Any())
                {
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        total.ToString(), loanCase.ProductCode.ToString(), null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ loan case {loanCase.CaseNumber}: no customer account for product {loanCase.ProductCode}"));
                    continue;
                }

                var acc = loanAccounts[0];

                decimal alreadyImportedPrincipal = 0m;
                using (var cmd = new SqlCommand(@"
            SELECT ISNULL(SUM(Principal), 0)
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
            WHERE CustomerAccountId = @AccountId
              AND Principal > 0
              AND Status = 0", connection))
                {
                    cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = acc.Id;
                    var r = await cmd.ExecuteScalarAsync();
                    if (r != null && r != DBNull.Value)
                        alreadyImportedPrincipal = Convert.ToDecimal(r);
                }
                decimal effectiveBalance = Math.Max(loanCase.TotalLoansBalance - alreadyImportedPrincipal, 0m);

                bool isFirstPayment = loanCase.LastPaymentDate == null;
                bool isSameMonthByDate = loanCase.LastPaymentDate.HasValue
                                            && loanCase.LastPaymentDate.Value.Year == now.Year
                                            && loanCase.LastPaymentDate.Value.Month == now.Month;
                bool isSameMonthByEntry = await HasPrincipalEntryThisMonth(connection, acc.Id, now);
                bool isSameMonthPayment = isSameMonthByDate || isSameMonthByEntry;
                bool isDisbursedThisMonth = loanCase.DisbursedDate.HasValue
                                            && loanCase.DisbursedDate.Value.Year == now.Year
                                            && loanCase.DisbursedDate.Value.Month == now.Month;

                var (interestPortion, principalPortion, arrearsInterest, monthsMissed) =
                    CalculateReducingBalancePayment(
                        outstandingBalance: effectiveBalance,
                        annualPercentageRate: loanCase.AnnualPercentageRate,
                        payment: monthlyPayment,
                        lastPaymentDate: loanCase.LastPaymentDate,
                        disbursedDate: loanCase.DisbursedDate,
                        now: now,
                        isFirstPaymentAfterDisbursementThisMonth: isDisbursedThisMonth && isFirstPayment,
                        isSameMonthRepayment: isSameMonthPayment);

                decimal newBalance = Math.Max(effectiveBalance - principalPortion, 0m);

                if (principalPortion > 0m)
                {
                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: acc.Id,
                        chartOfAccountId: loanCase.LoanAssetCoA,
                        principal: principalPortion,
                        interest: 0m,
                        balance: newBalance,
                        beneficiary: total.ToString(),
                        reference: $"{loanCase.CaseNumber} - PRINCIPAL - {reference}");
                }

                if (interestPortion > 0m)
                {
                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: acc.Id,
                        chartOfAccountId: loanCase.InterestIncomeCoA,
                        principal: 0m,
                        interest: interestPortion,
                        balance: 0m,
                        beneficiary: total.ToString(),
                        reference: $"{loanCase.CaseNumber} - INTEREST - {reference}");
                }

                // ── audit to debug only ──
                System.Diagnostics.Debug.WriteLine(
                    $"[MULTI-LOAN] {payrollNum} Case#{loanCase.CaseNumber} Row#{rowCount}: " +
                    $"APR={loanCase.AnnualPercentageRate}% Miss={monthsMissed} " +
                    $"ArrInt={arrearsInterest} CurInt={interestPortion - arrearsInterest} " +
                    $"TotInt={interestPortion} Prin={principalPortion} " +
                    $"OldBal={loanCase.TotalLoansBalance} NewBal={newBalance} " +
                    $"SameMonth={isSameMonthPayment}");

                if (principalPortion > 0m)
                {
                    using (var cmd = new SqlCommand(@"
                UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases]
                SET ReceivedDate = @PaymentDate
                WHERE Id = @LoanCaseId", connection))
                    {
                        cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value = now;
                        cmd.Parameters.Add("@LoanCaseId", SqlDbType.UniqueIdentifier).Value = loanCase.Id;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                remaining -= monthlyPayment;
                anyPosted = true;
            }

            // ── Excess — real failure if no investment account ──
            if (remaining > 0m)
            {
                Guid investAccId = Guid.Empty;
                Guid investCoAId = Guid.Empty;

                using (var cmd = new SqlCommand(@"
            SELECT TOP 1 ca.Id, ip.ChartOfAccountId
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
            INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_InvestmentProducts] ip
                ON ca.CustomerAccountType_TargetProductId = ip.Id
            WHERE ip.Code     = 222
              AND ca.CustomerId = @CustomerId", connection))
                {
                    cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (await r.ReadAsync())
                        {
                            investAccId = r.GetGuid(r.GetOrdinal("Id"));
                            var coaOrd = r.GetOrdinal("ChartOfAccountId");
                            if (!r.IsDBNull(coaOrd)) investCoAId = r.GetGuid(coaOrd);
                        }
                    }
                }

                if (investAccId != Guid.Empty && investCoAId != Guid.Empty)
                {
                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: investAccId,
                        chartOfAccountId: investCoAId,
                        principal: remaining,
                        interest: 0m,
                        balance: 0m,
                        beneficiary: total.ToString(),
                        reference: $"Excess - {reference}");
                    anyPosted = true;

                    System.Diagnostics.Debug.WriteLine(
                        $"[MULTI-LOAN] {payrollNum} Row#{rowCount}: excess {remaining:N2} → investment account");
                }
                else
                {
                    // ── real failure: excess with no investment account ──
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        remaining.ToString(), "222", null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ excess of {remaining:N2} could not be posted — no investment account (code 222) for payroll {payrollNum}"));
                    mismatches.Add(new { Row = rowCount, Reason = $"Excess {remaining:N2} — no investment account for payroll {payrollNum}" });
                }
            }

            return anyPosted;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // CHECK OFF — savings (RBF / SHP — NOT DEP, DEP uses split logic above)
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> ProcessCheckOffSavings(
            SqlConnection connection, Guid batchId, Guid branchId,
            string payrollNum, decimal amount, int productCode,
            string reference, int rowCount, List<object> mismatches)
        {
            Guid savProductId = Guid.Empty;
            Guid savProductCoA = Guid.Empty;
            int savProductTargetCode = 0;

            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 Id, Code, ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts]
        WHERE Code = @Code", connection))
            {
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = productCode;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                    {
                        savProductId = r.GetGuid(r.GetOrdinal("Id"));
                        savProductTargetCode = Convert.ToInt32(r["Code"]);
                        var coaOrd = r.GetOrdinal("ChartOfAccountId");
                        if (!r.IsDBNull(coaOrd)) savProductCoA = r.GetGuid(coaOrd);
                    }
                }
            }

            if (savProductId == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    amount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no savings product for code {productCode}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No savings product for code {productCode}" });
                return false;
            }

            if (savProductCoA == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    amount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ savings product {productCode} has no ChartOfAccountId configured"));
                mismatches.Add(new { Row = rowCount, Reason = "Savings product missing Chart of Account" });
                return false;
            }

            var accounts = await FindSavingsAccountsByProductAndPayroll(connection, savProductId, savProductTargetCode, payrollNum);

            if (accounts.Any())
            {
                var acc = accounts[0];
                await InsertCreditBatchEntry(connection, batchId, acc.Id, savProductCoA, amount, 0m, 0m, amount.ToString(), reference);
                return true;
            }

            Guid customerId = Guid.Empty;
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 Id
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers]
        WHERE Individual_PayrollNumbers = @PayrollNumber", connection))
            {
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                    customerId = (Guid)result;
            }

            if (customerId == Guid.Empty)
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    amount.ToString(), productCode.ToString(), null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no customer found for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No customer for payroll {payrollNum}" });
                return false;
            }

            Guid existingAccId = Guid.Empty;
            using (var cmd = new SqlCommand(@"
        SELECT TOP 1 Id
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
        WHERE CustomerId = @CustomerId
          AND CustomerAccountType_TargetProductCode = @ProductCode", connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                cmd.Parameters.Add("@ProductCode", SqlDbType.SmallInt).Value = (short)savProductTargetCode;
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                    existingAccId = (Guid)result;
            }

            if (existingAccId != Guid.Empty)
            {
                await InsertCreditBatchEntry(connection, batchId, existingAccId, savProductCoA, amount, 0m, 0m, amount.ToString(), reference);
                return true;
            }

            // Auto-create account
            Guid newAccountId = Guid.NewGuid();
            using (var cmd = new SqlCommand(@"
        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
            (Id, CustomerId, BranchId, CustomerAccountType_ProductCode,
             CustomerAccountType_TargetProductId, CustomerAccountType_TargetProductCode,
             ScoredLoanDisbursementProductCode, ScoredLoanLimit,
             Status, RecordStatus, SequentialId, CreatedBy, CreatedDate)
        VALUES
            (@Id, @CustomerId, @BranchId, @ProductCode,
             @TargetProductId, @TargetProductCode,
             0, 0, 0, 2, @SeqId, 'System', @CreatedDate)", connection))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = newAccountId;
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = branchId;
                cmd.Parameters.Add("@ProductCode", SqlDbType.TinyInt).Value = (byte)4;
                cmd.Parameters.Add("@TargetProductId", SqlDbType.UniqueIdentifier).Value = savProductId;
                cmd.Parameters.Add("@TargetProductCode", SqlDbType.SmallInt).Value = (short)savProductTargetCode;
                cmd.Parameters.Add("@SeqId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = DateTime.Now;
                await cmd.ExecuteNonQueryAsync();
            }

            await InsertCreditBatchEntry(connection, batchId, newAccountId, savProductCoA, amount, 0m, 0m, amount.ToString(), reference);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // CHECK OFF FUZZY
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<bool> ProcessFuzzyCheckOff(
            SqlConnection connection, Guid batchId, string payrollNum,
            decimal contributionAmount, string productTypeCode,
            string reference, int rowCount, List<object> mismatches)
        {
            var standingOrders = new List<StandingOrderData>();

            using (var cmd = new SqlCommand(@"
        SELECT so.Id, so.BenefactorCustomerAccountId, so.BeneficiaryCustomerAccountId,
               so.Principal, so.Interest, so.Charge_FixedAmount, so.PaymentPerPeriod,
               so.IsLocked,
               bca.CustomerAccountType_ProductCode AS BeneficiaryProductCode
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_StandingOrders] so
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] bca
            ON so.BeneficiaryCustomerAccountId = bca.Id
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] fca
            ON so.BenefactorCustomerAccountId = fca.Id
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
            ON fca.CustomerId = c.Id
        WHERE so.Trigger = @Trigger
          AND c.Individual_PayrollNumbers = @PayrollNumber
          AND so.IsLocked = 0", connection))
            {
                cmd.Parameters.Add("@Trigger", SqlDbType.Int).Value = (int)StandingOrderTrigger.CheckOff;
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        standingOrders.Add(new StandingOrderData
                        {
                            Id = r.GetGuid(r.GetOrdinal("Id")),
                            BenefactorCustomerAccountId = r.GetGuid(r.GetOrdinal("BenefactorCustomerAccountId")),
                            BeneficiaryCustomerAccountId = r.GetGuid(r.GetOrdinal("BeneficiaryCustomerAccountId")),
                            Principal = Convert.ToDecimal(r["Principal"]),
                            Interest = Convert.ToDecimal(r["Interest"]),
                            ChargeFixedAmount = Convert.ToDecimal(r["Charge_FixedAmount"]),
                            PaymentPerPeriod = Convert.ToDecimal(r["PaymentPerPeriod"]),
                            BeneficiaryProductCode = Convert.ToInt32(r["BeneficiaryProductCode"]),
                            BeneficiaryCoA = Guid.Empty
                        });
                    }
                }
            }

            if (!standingOrders.Any())
            {
                await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                    contributionAmount.ToString(), productTypeCode, null, reference, null, null,
                    Truncate($"Row #{rowCount} ~ no standing orders found for payroll {payrollNum}"));
                mismatches.Add(new { Row = rowCount, Reason = $"No standing orders for payroll {payrollNum}" });
                return false;
            }

            bool anyPosted = false;

            if (productTypeCode == "DEP" || productTypeCode == "RBF" || productTypeCode == "SHP")
            {
                var matchedSOs = standingOrders
                    .Where(s => s.BeneficiaryProductCode == (int)ProductCode.Savings
                             && s.ChargeFixedAmount == contributionAmount)
                    .ToList();

                if (!matchedSOs.Any())
                {
                    matchedSOs = standingOrders
                        .Where(s => s.BeneficiaryProductCode == (int)ProductCode.Investment
                                 && s.ChargeFixedAmount == contributionAmount)
                        .ToList();
                }

                foreach (var so in matchedSOs)
                {
                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: so.BeneficiaryCustomerAccountId,
                        chartOfAccountId: Guid.Empty,
                        principal: contributionAmount,
                        interest: 0m,
                        balance: 0m,
                        beneficiary: payrollNum,
                        reference: reference);
                    anyPosted = true;
                }

                if (!anyPosted)
                {
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        contributionAmount.ToString(), productTypeCode, null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ no standing order match for ChargeFixedAmount {contributionAmount}"));
                    mismatches.Add(new { Row = rowCount, Reason = $"No standing order match for amount {contributionAmount}" });
                }
            }
            else
            {
                var matchedSOs = standingOrders
                    .Where(s => s.BeneficiaryProductCode == (int)ProductCode.Loan
                             && (s.Principal == contributionAmount || s.PaymentPerPeriod == contributionAmount))
                    .ToList();

                foreach (var so in matchedSOs)
                {
                    var loanAccount = await FindAccountById(connection, so.BeneficiaryCustomerAccountId);
                    await InsertCreditBatchEntry(connection, batchId,
                        customerAccountId: so.BeneficiaryCustomerAccountId,
                        chartOfAccountId: Guid.Empty,
                        principal: contributionAmount,
                        interest: 0m,
                        balance: loanAccount?.TotalBalance ?? 0m,
                        beneficiary: payrollNum,
                        reference: reference);
                    anyPosted = true;
                }

                if (!anyPosted)
                {
                    await InsertBatchDiscrepancy(connection, batchId, payrollNum, null,
                        contributionAmount.ToString(), productTypeCode, null, reference, null, null,
                        Truncate($"Row #{rowCount} ~ no loan standing order match for amount {contributionAmount}"));
                    mismatches.Add(new { Row = rowCount, Reason = $"No loan standing order match for amount {contributionAmount}" });
                }
            }

            return anyPosted;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // QUERY HELPERS
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task<List<CustomerAccountData>> FindSavingsAccountsByProductAndPayroll(
            SqlConnection conn, Guid savingsProductId, int targetProductCode, string payrollNum)
        {
            var result = new List<CustomerAccountData>();
            using (var cmd = new SqlCommand(@"
        SELECT ca.Id, ca.CustomerId, ca.BranchId,
               ca.CustomerAccountType_ProductCode,
               ca.CustomerAccountType_TargetProductId,
               ca.CustomerAccountType_TargetProductCode,
               sp.ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
            ON ca.CustomerId = c.Id
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts] sp
            ON sp.Id   = @ProductId
           AND sp.Code = ca.CustomerAccountType_TargetProductCode
        WHERE c.Individual_PayrollNumbers = @PayrollNumber", conn))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = savingsProductId;
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var acc = MapAccount(r);
                        var coaOrd = r.GetOrdinal("ChartOfAccountId");
                        acc.ChartOfAccountId = r.IsDBNull(coaOrd) ? Guid.Empty : r.GetGuid(coaOrd);
                        result.Add(acc);
                    }
                }
            }
            return result;
        }

        private async Task<List<CustomerAccountData>> FindAccountsByProductAndPayroll(
            SqlConnection conn, Guid productId, string payrollNum)
        {
            var result = new List<CustomerAccountData>();
            using (var cmd = new SqlCommand(@"
        SELECT ca.Id, ca.CustomerId, ca.BranchId,
               ca.CustomerAccountType_ProductCode,
               ca.CustomerAccountType_TargetProductId,
               ca.CustomerAccountType_TargetProductCode,
               sp.ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
            ON ca.CustomerId = c.Id
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts] sp
            ON sp.Id   = @ProductId
           AND sp.Code = ca.CustomerAccountType_TargetProductCode
        WHERE c.Reference3 = @PayrollNumber", conn))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = productId;
                cmd.Parameters.Add("@PayrollNumber", SqlDbType.NVarChar).Value = payrollNum;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var acc = MapAccount(r);
                        var coaOrd = r.GetOrdinal("ChartOfAccountId");
                        acc.ChartOfAccountId = r.IsDBNull(coaOrd) ? Guid.Empty : r.GetGuid(coaOrd);
                        result.Add(acc);
                    }
                }
            }
            return result;
        }

        private async Task<List<CustomerAccountData>> FindAccountsByProductAndReference3(
            SqlConnection conn, Guid productId, string reference3)
        {
            var result = new List<CustomerAccountData>();
            using (var cmd = new SqlCommand(@"
        SELECT ca.Id, ca.CustomerId, ca.BranchId,
               ca.CustomerAccountType_ProductCode,
               ca.CustomerAccountType_TargetProductId,
               ca.CustomerAccountType_TargetProductCode,
               lp.ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
            ON ca.CustomerId = c.Id
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp
            ON lp.Id   = @ProductId
           AND lp.Code = ca.CustomerAccountType_TargetProductCode
        WHERE c.Reference3 = @Reference3", conn))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = productId;
                cmd.Parameters.Add("@Reference3", SqlDbType.NVarChar).Value = reference3;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var acc = MapAccount(r);
                        var coaOrd = r.GetOrdinal("ChartOfAccountId");
                        acc.ChartOfAccountId = r.IsDBNull(coaOrd) ? Guid.Empty : r.GetGuid(coaOrd);
                        result.Add(acc);
                    }
                }
            }
            return result;
        }

        private async Task<List<CustomerAccountData>> FindAccountsByProductIdDirect(
            SqlConnection conn, Guid productId, Guid customerId)
        {
            var result = new List<CustomerAccountData>();
            using (var cmd = new SqlCommand(@"
        SELECT ca.Id, ca.CustomerId, ca.BranchId,
               ca.CustomerAccountType_ProductCode,
               ca.CustomerAccountType_TargetProductId,
               ca.CustomerAccountType_TargetProductCode,
               lp.ChartOfAccountId
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp
            ON lp.Id   = @ProductId
           AND lp.Code = ca.CustomerAccountType_TargetProductCode
        WHERE ca.CustomerId = @CustomerId", conn))
            {
                cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = productId;
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var acc = MapAccount(r);
                        var coaOrd = r.GetOrdinal("ChartOfAccountId");
                        acc.ChartOfAccountId = r.IsDBNull(coaOrd) ? Guid.Empty : r.GetGuid(coaOrd);
                        result.Add(acc);
                    }
                }
            }
            return result;
        }

        private async Task<CustomerAccountData> FindAccountById(SqlConnection conn, Guid accountId)
        {
            using (var cmd = new SqlCommand(@"
        SELECT ca.Id, ca.CustomerId, ca.BranchId,
               ca.CustomerAccountType_ProductCode,
               ca.CustomerAccountType_TargetProductId,
               ca.CustomerAccountType_TargetProductCode
        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
        WHERE ca.Id = @Id", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = accountId;
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                        return MapAccount(r);
                }
            }
            return null;
        }

        // FIX: MapAccount now safely handles DBNull on BranchId
        private CustomerAccountData MapAccount(SqlDataReader r)
        {
            var branchOrd = r.GetOrdinal("BranchId");
            return new CustomerAccountData
            {
                Id = r.GetGuid(r.GetOrdinal("Id")),
                CustomerId = r.GetGuid(r.GetOrdinal("CustomerId")),
                BranchId = r.IsDBNull(branchOrd) ? Guid.Empty : r.GetGuid(branchOrd),
                CustomerAccountTypeProductCode = Convert.ToInt32(r["CustomerAccountType_ProductCode"]),
                CustomerAccountTypeTargetProductId = r.GetGuid(r.GetOrdinal("CustomerAccountType_TargetProductId")),
                CustomerAccountTypeTargetProductCode = Convert.ToInt32(r["CustomerAccountType_TargetProductCode"])
            };
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // INSERT HELPERS
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task InsertCreditBatchEntry(
     SqlConnection conn, Guid batchId,
     Guid customerAccountId, Guid chartOfAccountId,
     decimal principal, decimal interest, decimal balance,
     string beneficiary, string reference)
        {
            // ================== DUPLICATE GUARD ==================
            // Prevent double-posting same account + same reference in same batch
            if (customerAccountId != Guid.Empty && chartOfAccountId != Guid.Empty)
            {
                using (var checkCmd = new SqlCommand(@"
            SELECT COUNT(1)
            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
            WHERE CreditBatchId       = @CreditBatchId
              AND CustomerAccountId   = @CustomerAccountId
              AND ChartOfAccountId    = @ChartOfAccountId
              AND Reference           = @Reference", conn))
                {
                    checkCmd.Parameters.Add("@CreditBatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                    checkCmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId;
                    checkCmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId;
                    checkCmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = reference ?? (object)DBNull.Value;

                    var exists = (int)await checkCmd.ExecuteScalarAsync();
                    if (exists > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[DUPLICATE GUARD] Skipped duplicate entry: " +
                            $"BatchId={batchId} AccountId={customerAccountId} CoA={chartOfAccountId} Ref={reference}");
                        return; // skip — already posted
                    }
                }
            }

            // ================== INSERT ==================
            using (var cmd = new SqlCommand(@"
        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
            (Id, CreditBatchId, CustomerAccountId, ChartOfAccountId,
             Principal, Interest, Balance, Beneficiary, Reference,
             Status, SequentialId, CreatedBy, CreatedDate)
        VALUES
            (@Id, @CreditBatchId, @CustomerAccountId, @ChartOfAccountId,
             @Principal, @Interest, @Balance, @Beneficiary, @Reference,
             @Status, @SequentialId, @CreatedBy, @CreatedDate)", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreditBatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId != Guid.Empty ? (object)customerAccountId : DBNull.Value;
                cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = chartOfAccountId != Guid.Empty ? (object)chartOfAccountId : DBNull.Value;
                cmd.Parameters.Add("@Principal", SqlDbType.Decimal).Value = principal;
                cmd.Parameters.Add("@Interest", SqlDbType.Decimal).Value = interest;
                cmd.Parameters.Add("@Balance", SqlDbType.Decimal).Value = balance;
                cmd.Parameters.Add("@Beneficiary", SqlDbType.NVarChar).Value = beneficiary ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = reference ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = "System";
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertBatchDiscrepancy(
            SqlConnection conn, Guid batchId,
            string col1, string col2, string col3, string col4,
            string col5, string col6, string col7, string col8, string remarks)
        {
            col1 = Truncate(col1, 256);
            col2 = Truncate(col2, 256);
            col3 = Truncate(col3, 256);
            col4 = Truncate(col4, 256);
            col5 = Truncate(col5, 256);
            col6 = Truncate(col6, 256);
            col7 = Truncate(col7, 256);
            col8 = Truncate(col8, 256);
            remarks = Truncate(remarks, 256);

            using (var cmd = new SqlCommand(@"
        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchDiscrepancies]
            (Id, CreditBatchId, Column1, Column2, Column3, Column4, Column5, Column6, Column7, Column8,
             Remarks, Status, PostedBy, PostedDate, SequentialId, CreatedBy, CreatedDate)
        VALUES
            (@Id, @CreditBatchId, @C1, @C2, @C3, @C4, @C5, @C6, @C7, @C8,
             @Remarks, @Status, @PostedBy, @PostedDate, @SequentialId, @CreatedBy, @CreatedDate)", conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreditBatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                cmd.Parameters.Add("@C1", SqlDbType.NVarChar).Value = col1 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C2", SqlDbType.NVarChar).Value = col2 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C3", SqlDbType.NVarChar).Value = col3 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C4", SqlDbType.NVarChar).Value = col4 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C5", SqlDbType.NVarChar).Value = col5 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C6", SqlDbType.NVarChar).Value = col6 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C7", SqlDbType.NVarChar).Value = col7 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@C8", SqlDbType.NVarChar).Value = col8 ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = remarks ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@PostedBy", SqlDbType.NVarChar).Value = "System";
                cmd.Parameters.Add("@PostedDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = "System";
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // DATA CLASSES
        // ─────────────────────────────────────────────────────────────────────────────
        private class CustomerAccountData
        {
            public Guid Id { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BranchId { get; set; }
            public int CustomerAccountTypeProductCode { get; set; }
            public Guid CustomerAccountTypeTargetProductId { get; set; }
            public int CustomerAccountTypeTargetProductCode { get; set; }
            public Guid ChartOfAccountId { get; set; }
            public decimal TotalBalance { get; set; }
        }

        private class LoanCaseData
        {
            public Guid Id { get; set; }
            public Guid CustomerId { get; set; }
            public Guid LoanProductId { get; set; }
            public DateTime? DisbursedDate { get; set; }
            public DateTime? LastPaymentDate { get; set; }
            public decimal TotalLoansBalance { get; set; }
            public decimal MonthlyPaybackAmount { get; set; }
            public int CaseNumber { get; set; }
            public int ProductCode { get; set; }
            public decimal AnnualPercentageRate { get; set; }
            public Guid LoanAssetCoA { get; set; }
            public Guid InterestIncomeCoA { get; set; }
        }

        private class StandingOrderData
        {
            public Guid Id { get; set; }
            public Guid BenefactorCustomerAccountId { get; set; }
            public Guid BeneficiaryCustomerAccountId { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal ChargeFixedAmount { get; set; }
            public decimal PaymentPerPeriod { get; set; }
            public int BeneficiaryProductCode { get; set; }
            public Guid BeneficiaryCoA { get; set; }
        }
     
        [HttpPost]
        [Route("creditbatch/{batchId}/post")]
        public async Task<IHttpActionResult> PostCreditBatch(Guid batchId)
        {
            try
            {
                if (batchId == Guid.Empty)
                    return BadRequest("Invalid batch ID.");

                var serviceHeader = CreateServiceHeader();


                var creditBatchDTO = _creditBatchAppService.FindCreditBatch(batchId, serviceHeader);

                if (creditBatchDTO == null)
                    return NotFound();

                var authorized = _creditBatchAppService.AuthorizeCreditBatch(creditBatchDTO, 1, 0, serviceHeader);

     
                if (!authorized)
                    return BadRequest("Authorization failed for credit batch.");

                var creditBatchEntries = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchId(batchId,serviceHeader);
                if (creditBatchEntries == null || !creditBatchEntries.Any())
                    return BadRequest("No entries found for this credit batch.");

                var unpostedEntries = creditBatchEntries.Where(e => e.Status == 0).ToList();
                if (!unpostedEntries.Any())
                    return BadRequest("All entries have already been posted.");

                var postedCount = 0;
                var skippedCount = creditBatchEntries.Count - unpostedEntries.Count;
                var discrepancies = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // ─────────────────────────────────────────────────────────
                    // 1. Load batch header
                    // ─────────────────────────────────────────────────────────
                    Guid branchId = Guid.Empty;
                    Guid creditTypeCoAId = Guid.Empty;
                    string batchReference = "";
                    int batchType = 0;
                    string createdBy = "System";
                    DateTime batchValueDate = DateTime.Now;

                    using (var cmd = new SqlCommand(@"
                SELECT cb.BranchId, cb.Reference, cb.Type, cb.CreatedBy, cb.ValueDate,
                       ct.ChartOfAccountId AS CreditTypeChartOfAccountId
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatches] cb
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditTypes] ct
                    ON cb.CreditTypeId = ct.Id
                WHERE cb.Id = @BatchId", connection))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return NotFound();

                            branchId = reader.GetGuid(reader.GetOrdinal("BranchId"));
                            batchReference = reader["Reference"]?.ToString() ?? "";
                            batchType = Convert.ToInt32(reader["Type"]);
                            createdBy = reader["CreatedBy"]?.ToString() ?? "System";

                            var vdOrd = reader.GetOrdinal("ValueDate");
                            if (!reader.IsDBNull(vdOrd))
                                batchValueDate = reader.GetDateTime(vdOrd);

                            var coaOrd = reader.GetOrdinal("CreditTypeChartOfAccountId");
                            if (!reader.IsDBNull(coaOrd))
                                creditTypeCoAId = reader.GetGuid(coaOrd);
                        }
                    }

                    if (creditTypeCoAId == Guid.Empty)
                        return BadRequest("Credit type chart of account not configured.");

                    // ─────────────────────────────────────────────────────────
                    // 2. Get active posting period
                    // ─────────────────────────────────────────────────────────
                    Guid postingPeriodId = Guid.Empty;

                    using (var cmd = new SqlCommand(@"
                SELECT TOP 1 Id
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_PostingPeriods]
                WHERE GETDATE() BETWEEN Duration_StartDate AND Duration_EndDate
                  AND IsActive = 1", connection))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                            postingPeriodId = (Guid)result;
                    }

                    if (postingPeriodId == Guid.Empty)
                        return BadRequest("No active posting period found for today's date.");

                    // ─────────────────────────────────────────────────────────
                    // 3. SQL templates
                    // ─────────────────────────────────────────────────────────
                    const string journalInsert = @"
                INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_Journals]
                    (Id, PostingPeriodId, BranchId, TotalValue,
                     PrimaryDescription, SecondaryDescription, Reference,
                     ValueDate, IsLocked, SequentialId, CreatedBy, CreatedDate)
                VALUES
                    (@Id, @PostingPeriodId, @BranchId, @TotalValue,
                     @PrimaryDescription, @SecondaryDescription, @Reference,
                     @ValueDate, @IsLocked, @SequentialId, @CreatedBy, @CreatedDate)";

                    const string journalEntryInsert = @"
                INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries]
                    (Id, JournalId, ChartOfAccountId, ContraChartOfAccountId, CustomerAccountId,
                     Amount, ValueDate, SequentialId, CreatedBy, CreatedDate)
                VALUES
                    (@Id, @JournalId, @ChartOfAccountId, @ContraChartOfAccountId, @CustomerAccountId,
                     @Amount, @ValueDate, @SequentialId, @CreatedBy, @CreatedDate)";

                    // ─────────────────────────────────────────────────────────
                    // 4. Process each unposted entry
                    // ─────────────────────────────────────────────────────────
                    foreach (var entry in unpostedEntries)
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                // ── DB-level double-post guard ────────────────
                                int currentStatus = 0;
                                using (var cmd = new SqlCommand(@"
                            SELECT Status
                            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
                            WHERE Id = @EntryId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@EntryId", SqlDbType.UniqueIdentifier).Value = entry.Id;
                                    var s = await cmd.ExecuteScalarAsync();
                                    if (s != null && s != DBNull.Value)
                                        currentStatus = Convert.ToInt32(s);
                                }

                                if (currentStatus == 1)
                                {
                                    transaction.Commit();
                                    skippedCount++;
                                    continue;
                                }

                                if (!entry.CustomerAccountId.HasValue || entry.CustomerAccountId == Guid.Empty)
                                {
                                    await InsertDiscrepancy(connection, transaction, batchId, entry,
                                        "Entry skipped: CustomerAccountId is required for all entries.");
                                    transaction.Commit();
                                    continue;
                                }

                                DateTime now = batchValueDate;
                                string primaryDesc = batchReference;
                                string secondaryDesc = $"{creditBatchDTO.TypeDescription}~{creditBatchDTO.MonthDescription}";
                                string reference = entry.Reference ?? batchReference;

                                // ── Determine account type ────────────────────
                                int accountTypeProductCode = 0;
                                Guid targetProductId = Guid.Empty;

                                using (var cmd = new SqlCommand(@"
                            SELECT CustomerAccountType_ProductCode,
                                   CustomerAccountType_TargetProductId
                            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
                            WHERE Id = @CustomerAccountId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = entry.CustomerAccountId.Value;
                                    using (var r = await cmd.ExecuteReaderAsync())
                                    {
                                        if (await r.ReadAsync())
                                        {
                                            accountTypeProductCode = Convert.ToInt32(r["CustomerAccountType_ProductCode"]);
                                            targetProductId = r.GetGuid(r.GetOrdinal("CustomerAccountType_TargetProductId"));
                                        }
                                    }
                                }

                                // ── Is this a loan product? ───────────────────
                                bool isLoanProduct = false;
                                if (targetProductId != Guid.Empty)
                                {
                                    using (var cmd = new SqlCommand(@"
                                SELECT TOP 1 1
                                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts]
                                WHERE Id = @ProductId", connection, transaction))
                                    {
                                        cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = targetProductId;
                                        var r = await cmd.ExecuteScalarAsync();
                                        isLoanProduct = r != null && r != DBNull.Value;
                                    }
                                }

                                // ─────────────────────────────────────────────
                                // PATH 1: LOAN ACCOUNT
                                //
                                // Principal:
                                //   DR Control Account (creditTypeCoAId)  +Principal
                                //   CR Loan Asset CoA  (loanAssetCoAId)   -Principal
                                //
                                // Interest:
                                //   DR Control Account (creditTypeCoAId)       +Interest
                                //   CR Interest Received CoA (interestCoAId)   -Interest
                                // ─────────────────────────────────────────────
                                if (isLoanProduct)
                                {
                                    Guid loanAssetCoAId = Guid.Empty;
                                    Guid interestReceivedCoAId = Guid.Empty;

                                    using (var cmd = new SqlCommand(@"
                                SELECT ChartOfAccountId,
                                       InterestReceivedChartOfAccountId
                                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts]
                                WHERE Id = @ProductId", connection, transaction))
                                    {
                                        cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = targetProductId;
                                        using (var r = await cmd.ExecuteReaderAsync())
                                        {
                                            if (await r.ReadAsync())
                                            {
                                                var coaOrd = r.GetOrdinal("ChartOfAccountId");
                                                var irCoaOrd = r.GetOrdinal("InterestReceivedChartOfAccountId");
                                                if (!r.IsDBNull(coaOrd)) loanAssetCoAId = r.GetGuid(coaOrd);
                                                if (!r.IsDBNull(irCoaOrd)) interestReceivedCoAId = r.GetGuid(irCoaOrd);
                                            }
                                        }
                                    }

                                    if (loanAssetCoAId == Guid.Empty)
                                    {
                                        await InsertDiscrepancy(connection, transaction, batchId, entry,
                                            "Entry skipped: could not resolve loan product ChartOfAccountId (Loan Asset CoA).");
                                        transaction.Commit();
                                        continue;
                                    }

                                    // ── Principal journal ─────────────────────
                                    if (entry.Principal > 0)
                                    {
                                        Guid principalJournalId = Guid.NewGuid();

                                        await InsertJournalHeader(connection, transaction, journalInsert,
                                            principalJournalId, postingPeriodId, branchId, entry.Principal,
                                            "Principal Repayment", "Check Off",
                                            entry.Reference ?? reference, now, createdBy);

                                        // DR Control Account +principal
                                        await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                            principalJournalId,
                                            creditTypeCoAId,   // CoA      = control (cash in)
                                            loanAssetCoAId,    // ContraCoA = loan asset
                                            entry.CustomerAccountId,
                                            entry.Principal,   // POSITIVE = Debit
                                            now, createdBy);

                                        // CR Loan Asset CoA -principal
                                        await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                            principalJournalId,
                                            loanAssetCoAId,    // CoA      = loan asset
                                            creditTypeCoAId,   // ContraCoA = control
                                            entry.CustomerAccountId,
                                            -entry.Principal,  // NEGATIVE = Credit
                                            now, createdBy);

                                        // ── Update loan balance at post time ──
                                        string caseNumPart = (entry.Reference ?? "").Split('-')[0].Trim();
                                        if (int.TryParse(caseNumPart, out int caseNumber))
                                        {
                                            using (var cmd = new SqlCommand(@"
                                        UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanCases]
                                        SET TotalLoansBalance = CASE
                                                WHEN TotalLoansBalance - @Principal < 0 THEN 0
                                                ELSE TotalLoansBalance - @Principal END,
                                            ReceivedDate = @PaymentDate
                                        WHERE CaseNumber = @CaseNumber", connection, transaction))
                                            {
                                                cmd.Parameters.Add("@Principal", SqlDbType.Decimal).Value = entry.Principal;
                                                cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value = now;
                                                cmd.Parameters.Add("@CaseNumber", SqlDbType.Int).Value = caseNumber;
                                                await cmd.ExecuteNonQueryAsync();
                                            }
                                        }
                                    }

                                    // ── Interest journal ──────────────────────
                                    if (entry.Interest > 0 && interestReceivedCoAId != Guid.Empty)
                                    {
                                        Guid interestJournalId = Guid.NewGuid();

                                        await InsertJournalHeader(connection, transaction, journalInsert,
                                            interestJournalId, postingPeriodId, branchId, entry.Interest,
                                            "Interest Payment", "Check Off",
                                            entry.Reference ?? reference, now, createdBy);

                                        // DR Control Account +interest
                                        await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                            interestJournalId,
                                            creditTypeCoAId,        // CoA      = control
                                            interestReceivedCoAId,  // ContraCoA = interest income
                                            entry.CustomerAccountId,
                                            entry.Interest,         // POSITIVE = Debit
                                            now, createdBy);

                                        // CR Interest Received CoA -interest
                                        await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                            interestJournalId,
                                            interestReceivedCoAId,  // CoA      = interest income
                                            creditTypeCoAId,        // ContraCoA = control
                                            entry.CustomerAccountId,
                                            -entry.Interest,        // NEGATIVE = Credit
                                            now, createdBy);
                                    }
                                }

                                // ─────────────────────────────────────────────
                                // PATH 2: Non-loan with explicit ChartOfAccountId
                                // (DEPOSITS, ENTRANCE FEE, SHARE CAPITAL, RBF, SHP)
                                //
                                //   DR Control Account (creditTypeCoAId)   +Amount
                                //   CR Product GL (entry.ChartOfAccountId) -Amount
                                // ─────────────────────────────────────────────
                                else if (entry.ChartOfAccountId != Guid.Empty)
                                {
                                    decimal totalAmount = entry.Principal + entry.Interest;
                                    if (totalAmount <= 0)
                                    {
                                        await InsertDiscrepancy(connection, transaction, batchId, entry,
                                            "Entry skipped: total amount is zero or negative.");
                                        transaction.Commit();
                                        continue;
                                    }

                                    Guid journalId = Guid.NewGuid();

                                    await InsertJournalHeader(connection, transaction, journalInsert,
                                        journalId, postingPeriodId, branchId, totalAmount,
                                        primaryDesc, secondaryDesc, reference, now, createdBy);

                                    // DR Control Account +amount (POSITIVE = Debit)
                                    await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                        journalId,
                                        creditTypeCoAId,               // CoA      = control account
                                        (Guid)entry.ChartOfAccountId,  // ContraCoA = member's product GL
                                        entry.CustomerAccountId,
                                        totalAmount,                    // POSITIVE = Debit on control
                                        now, createdBy);

                                    // CR Member's Product CoA -amount (NEGATIVE = Credit)
                                    await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                        journalId,
                                        (Guid)entry.ChartOfAccountId,  // CoA      = member's product GL
                                        creditTypeCoAId,               // ContraCoA = control account
                                        entry.CustomerAccountId,
                                        -totalAmount,                   // NEGATIVE = Credit on product CoA
                                        now, createdBy);
                                }

                                // ─────────────────────────────────────────────
                                // PATH 3: Savings (4) or Investment (2/3)
                                // CoA resolved via product join
                                //
                                //   DR Control Account (creditTypeCoAId) +Amount
                                //   CR Product CoA    (productCoAId)     -Amount
                                // ─────────────────────────────────────────────
                                else if (accountTypeProductCode == 4 ||
                                         accountTypeProductCode == 2 ||
                                         accountTypeProductCode == 3)
                                {
                                    decimal totalAmount = entry.Principal + entry.Interest;
                                    if (totalAmount <= 0)
                                    {
                                        await InsertDiscrepancy(connection, transaction, batchId, entry,
                                            "Entry skipped: total amount is zero or negative.");
                                        transaction.Commit();
                                        continue;
                                    }

                                    Guid productCoAId = Guid.Empty;

                                    using (var cmd = new SqlCommand(@"
                                SELECT COALESCE(sp.ChartOfAccountId, ip.ChartOfAccountId) AS ResolvedCoA
                                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts] sp
                                    ON ca.CustomerAccountType_TargetProductId = sp.Id
                                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_InvestmentProducts] ip
                                    ON ca.CustomerAccountType_TargetProductId = ip.Id
                                WHERE ca.Id = @CustomerAccountId", connection, transaction))
                                    {
                                        cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = entry.CustomerAccountId.Value;
                                        var r = await cmd.ExecuteScalarAsync();
                                        if (r != null && r != DBNull.Value)
                                            productCoAId = (Guid)r;
                                    }

                                    if (productCoAId == Guid.Empty)
                                    {
                                        await InsertDiscrepancy(connection, transaction, batchId, entry,
                                            "Entry skipped: could not resolve savings/investment chart of account.");
                                        transaction.Commit();
                                        continue;
                                    }

                                    Guid journalId = Guid.NewGuid();

                                    await InsertJournalHeader(connection, transaction, journalInsert,
                                        journalId, postingPeriodId, branchId, totalAmount,
                                        primaryDesc, secondaryDesc, reference, now, createdBy);

                                    // DR Control Account +amount (POSITIVE = Debit)
                                    await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                        journalId,
                                        creditTypeCoAId,  // CoA      = control account
                                        productCoAId,     // ContraCoA = member's product GL
                                        entry.CustomerAccountId,
                                        totalAmount,       // POSITIVE = Debit on control
                                        now, createdBy);

                                    // CR Product CoA -amount (NEGATIVE = Credit)
                                    await InsertJournalEntry(connection, transaction, journalEntryInsert,
                                        journalId,
                                        productCoAId,     // CoA      = member's product GL
                                        creditTypeCoAId,  // ContraCoA = control account
                                        entry.CustomerAccountId,
                                        -totalAmount,      // NEGATIVE = Credit on product CoA
                                        now, createdBy);
                                }
                                else
                                {
                                    await InsertDiscrepancy(connection, transaction, batchId, entry,
                                        $"Entry skipped: unrecognised account type code {accountTypeProductCode}.");
                                    transaction.Commit();
                                    continue;
                                }

                                // ── Mark entry as posted ──────────────────────
                                using (var cmd = new SqlCommand(@"
                            UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchEntries]
                            SET Status = 1
                            WHERE Id = @EntryId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@EntryId", SqlDbType.UniqueIdentifier).Value = entry.Id;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                transaction.Commit();
                                postedCount++;
                            }
                            catch (Exception entryEx)
                            {
                                transaction.Rollback();

                                using (var fallbackTx = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        await InsertDiscrepancy(connection, fallbackTx, batchId, entry,
                                            $"Posting failed: {entryEx.Message}");
                                        fallbackTx.Commit();
                                    }
                                    catch
                                    {
                                        fallbackTx.Rollback();
                                    }
                                }

                                discrepancies.Add(new { EntryId = entry.Id, Reason = entryEx.Message });
                            }
                        }
                    }

                    // ── Update batch status to Posted (2) ─────────────────────
                    using (var cmd = new SqlCommand(@"
                UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatches]
                SET Status = 2
                WHERE Id   = @BatchId", connection))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new
                {
                    Success = true,
                    BatchId = batchId,
                    TotalEntries = creditBatchEntries.Count,
                    Posted = postedCount,
                    Skipped = skippedCount,
                    Failed = discrepancies.Count,
                    Discrepancies = discrepancies
                });
            }
            catch (SqlException ex)
            {
                return InternalServerError(new Exception($"Database error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // JOURNAL HELPERS
        // ─────────────────────────────────────────────────────────────────────────────
        private async Task InsertJournalHeader(
            SqlConnection conn, SqlTransaction tx, string sql,
            Guid journalId, Guid postingPeriodId, Guid branchId, decimal totalValue,
            string primaryDesc, string secondaryDesc, string reference,
            DateTime now, string createdBy)
        {
            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = journalId;
                cmd.Parameters.Add("@PostingPeriodId", SqlDbType.UniqueIdentifier).Value = postingPeriodId;
                cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = branchId;
                cmd.Parameters.Add("@TotalValue", SqlDbType.Decimal).Value = totalValue;
                cmd.Parameters.Add("@PrimaryDescription", SqlDbType.NVarChar).Value = primaryDesc ?? (object)DBNull.Value;
                cmd.Parameters.Add("@SecondaryDescription", SqlDbType.NVarChar).Value = secondaryDesc ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = reference ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ValueDate", SqlDbType.Date).Value = now.Date;
                cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = false;
                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = createdBy ?? "System";
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = now;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertJournalEntry(
            SqlConnection conn, SqlTransaction tx, string sql,
            Guid journalId, Guid coaId, Guid contraCoAId,
            Guid? customerAccountId, decimal amount,
            DateTime now, string createdBy)
        {
            if (!customerAccountId.HasValue || customerAccountId == Guid.Empty)
                throw new InvalidOperationException("CustomerAccountId is required for all journal entries.");

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = journalId;
                cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = coaId;
                cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = contraCoAId;
                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId.Value;
                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;
                cmd.Parameters.Add("@ValueDate", SqlDbType.Date).Value = now.Date;
                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = createdBy ?? "System";
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = now;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static string Cap256(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= 256)
                return s;
            return s.Substring(0, 255) + "…";
        }

        private async Task InsertDiscrepancy(
            SqlConnection connection, SqlTransaction transaction,
            Guid batchId, CreditBatchEntryDTO entry, string remarks)
        {
            using (var cmd = new SqlCommand(@"
        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchDiscrepancies]
            (Id, CreditBatchId, Column1, Column2, Column3, Column4,
             Column5, Column6, Column7, Column8,
             Remarks, Status, PostedBy, PostedDate,
             SequentialId, CreatedBy, CreatedDate)
        VALUES
            (@Id, @CreditBatchId, @Column1, @Column2, @Column3, @Column4,
             @Column5, @Column6, @Column7, @Column8,
             @Remarks, @Status, @PostedBy, @PostedDate,
             @SequentialId, @CreatedBy, @CreatedDate)",
                connection, transaction))
            {
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreditBatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                cmd.Parameters.Add("@Column1", SqlDbType.NVarChar).Value = Cap256(entry.Id.ToString());
                cmd.Parameters.Add("@Column2", SqlDbType.NVarChar).Value = Cap256(entry.Beneficiary) ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Column3", SqlDbType.NVarChar).Value = Cap256(entry.Principal.ToString("N2"));
                cmd.Parameters.Add("@Column4", SqlDbType.NVarChar).Value = Cap256(entry.Interest.ToString("N2"));
                cmd.Parameters.Add("@Column5", SqlDbType.NVarChar).Value = Cap256(entry.Balance.ToString("N2"));
                cmd.Parameters.Add("@Column6", SqlDbType.NVarChar).Value = Cap256(entry.Reference) ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Column7", SqlDbType.NVarChar).Value = Cap256(entry.CustomerAccountId?.ToString()) ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Column8", SqlDbType.NVarChar).Value = entry.ChartOfAccountId != Guid.Empty
                                                                                    ? Cap256(entry.ChartOfAccountId.ToString())
                                                                                    : (object)DBNull.Value;
                cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Cap256(remarks) ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@PostedBy", SqlDbType.NVarChar).Value = "System";
                cmd.Parameters.Add("@PostedDate", SqlDbType.DateTime2).Value = DateTime.Now;
                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = "System";
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = DateTime.Now;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static string ExtractLoanCaseLabel(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return reference;
            if (reference.Contains(" - PRINCIPAL - "))
                return reference.Split(new[] { " - PRINCIPAL - " }, StringSplitOptions.None)[0].Trim();
            if (reference.Contains(" - INTEREST - "))
                return reference.Split(new[] { " - INTEREST - " }, StringSplitOptions.None)[0].Trim();
            return reference;
        }
        [HttpGet]
        [Route("creditbatch/{batchId}/discrepancies")]
        public async Task<IHttpActionResult> GetBatchDiscrepancies(
    Guid batchId,
    [FromUri] string type = "all") // "errors" | "audit" | "all"
        {
            try
            {
                if (batchId == Guid.Empty)
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid batch ID."
                    });

                var discrepancies = new List<object>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // ================== VERIFY BATCH EXISTS ==================
                    using (var checkCmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatches]
                WHERE Id = @BatchId", conn))
                    {
                        checkCmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;
                        var exists = (int)await checkCmd.ExecuteScalarAsync();
                        if (exists == 0)
                            return Ok(new ApiResponse<object>
                            {
                                Success = false,
                                Message = "Batch not found."
                            });
                    }

                    // ================== FETCH DISCREPANCIES ==================
                    // OUTER APPLY TOP 1 prevents duplicate rows when a payroll number
                    // matches BOTH Individual_PayrollNumbers AND Reference3 on different
                    // customer records. LEFT JOIN would return one row per match.
                    using (var cmd = new SqlCommand(@"
                SELECT
                    d.[Id],
                    d.[CreditBatchId],
                    d.[Column1]  AS PayrollNumber,
                    d.[Column2]  AS Beneficiary,
                    d.[Column3]  AS Amount,
                    d.[Column4]  AS ProductCode,
                    d.[Column6]  AS Reference,
                    d.[Remarks],
                    d.[Status],
                    d.[PostedBy],
                    d.[PostedDate],
                    d.[CreatedDate],
                    c.[Individual_FirstName],
                    c.[Individual_LastName],
                    c.[Address_MobileLine]
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CreditBatchDiscrepancies] d
                OUTER APPLY (
                    SELECT TOP 1
                        cx.[Individual_FirstName],
                        cx.[Individual_LastName],
                        cx.[Address_MobileLine]
                    FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] cx
                    WHERE cx.[Individual_PayrollNumbers] = d.[Column1]
                       OR cx.[Reference3]               = d.[Column1]
                    ORDER BY
                        -- prefer Individual_PayrollNumbers match over Reference3
                        CASE WHEN cx.[Individual_PayrollNumbers] = d.[Column1] THEN 0 ELSE 1 END
                ) c
                WHERE d.[CreditBatchId] = @BatchId
                ORDER BY d.[CreatedDate] ASC", conn))
                    {
                        cmd.Parameters.Add("@BatchId", SqlDbType.UniqueIdentifier).Value = batchId;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string remarks = reader["Remarks"]?.ToString() ?? "";
                                string payroll = reader["PayrollNumber"]?.ToString();
                                string productCode = reader["ProductCode"]?.ToString();
                                string amount = reader["Amount"]?.ToString();

                                bool isError = IsErrorRemark(remarks);

                                // ── Skip based on filter ──
                                if (type == "errors" && !isError) continue;
                                if (type == "audit" && isError) continue;

                                string firstName = reader["Individual_FirstName"]?.ToString();
                                string lastName = reader["Individual_LastName"]?.ToString();
                                string fullName = (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
                                    ? $"{firstName} {lastName}".Trim()
                                    : null;

                                discrepancies.Add(new
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    creditBatchId = reader.GetGuid(reader.GetOrdinal("CreditBatchId")),
                                    payrollNumber = payroll,
                                    beneficiary = reader["Beneficiary"]?.ToString(),
                                    amount = amount,
                                    productCode = productCode,
                                    reference = reader["Reference"]?.ToString(),
                                    remarks = remarks,
                                    status = Convert.ToInt32(reader["Status"] == DBNull.Value ? 0 : reader["Status"]),
                                    postedBy = reader["PostedBy"]?.ToString(),
                                    postedDate = reader["PostedDate"] != DBNull.Value
                                                        ? (DateTime?)Convert.ToDateTime(reader["PostedDate"])
                                                        : null,
                                    createdDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    isError = isError,

                                    // ── Member details ──
                                    memberName = fullName,
                                    memberMobile = reader["Address_MobileLine"]?.ToString(),

                                    // ── Derived fields ──
                                    errorCategory = GetErrorCategory(remarks),
                                    solution = GetSolution(remarks, payroll, productCode, amount)
                                });
                            }
                        }
                    }
                }

                int errorCount = discrepancies.Count(d => (bool)d.GetType().GetProperty("isError").GetValue(d));
                int auditCount = discrepancies.Count - errorCount;

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Found {discrepancies.Count} discrepancy records ({errorCount} errors, {auditCount} audit logs).",
                    Data = new
                    {
                        BatchId = batchId,
                        TotalCount = discrepancies.Count,
                        ErrorCount = errorCount,
                        AuditCount = auditCount,
                        Records = discrepancies
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to fetch discrepancies: {ex.Message} | Inner: {ex.InnerException?.Message}"
                });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // IsErrorRemark — audit log keywords checked FIRST to prevent false positives
        // ─────────────────────────────────────────────────────────────────────────────
        private static bool IsErrorRemark(string remarks)
        {
            if (string.IsNullOrWhiteSpace(remarks)) return false;

            var lower = remarks.ToLower();

            // ── Audit log keywords take priority — these are NEVER errors ──
            var auditKeywords = new[]
            {
        "dep split:",
        "dep split complete",
        "compound reducing balance",
        "allocated to entrance fee",
        "allocated to share capital",
        "allocated to deposits",
        "posted to deposits",
        "to entrance fee",
        "to share capital",
        "split complete"
    };

            if (auditKeywords.Any(k => lower.Contains(k)))
                return false;

            // ── Error keywords ──
            var errorKeywords = new[]
            {
        "no customer",
        "no match",
        "no account",
        "no active",
        "no loan",
        "no savings",
        "no standing",
        "unable to parse",
        "cannot parse",
        "ambiguous",
        "failed",
        "not found",
        "missing",
        "no remaining",
        "excess",
        "could not be posted",
        "not configured",
        "expected",
        "no deposits account",
        "no investment account",
        "monthly payback"
    };

            return errorKeywords.Any(k => lower.Contains(k));
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // GetErrorCategory
        // ─────────────────────────────────────────────────────────────────────────────
        private static string GetErrorCategory(string remarks)
        {
            if (string.IsNullOrWhiteSpace(remarks)) return "Unknown";

            var lower = remarks.ToLower();

            // ── Audit logs first — prevent misclassification ──
            if (lower.Contains("dep split:") ||
                lower.Contains("split complete") ||
                lower.Contains("compound reducing") ||
                lower.Contains("allocated to") ||
                lower.Contains("posted to deposits"))
                return "Audit Log";

            if (lower.Contains("no customer") || lower.Contains("not found"))
                return "Member Not Found";

            if (lower.Contains("no account") ||
                lower.Contains("no deposits account") ||
                lower.Contains("no savings") ||
                lower.Contains("no loan account"))
                return "Account Not Found";

            if (lower.Contains("no active loan") || lower.Contains("no loan case"))
                return "No Active Loan";

            if (lower.Contains("unable to parse") || lower.Contains("cannot parse"))
                return "Invalid Data";

            if (lower.Contains("ambiguous") || lower.Contains("accounts for payroll"))
                return "Duplicate Records";

            if (lower.Contains("no standing order"))
                return "No Standing Order";

            if (lower.Contains("excess"))
                return "Excess Payment";

            if (lower.Contains("missing") ||
                lower.Contains("not configured") ||
                (lower.Contains("monthly payback") && lower.Contains("0")))
                return "Configuration Error";

            return "Other Error";
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // GetSolution
        // ─────────────────────────────────────────────────────────────────────────────
        private static string GetSolution(string remarks, string payrollNumber, string productCode, string amount)
        {
            if (string.IsNullOrWhiteSpace(remarks)) return "Review the record manually.";

            var lower = remarks.ToLower();

            // ── Audit logs — no action needed ──
            if (lower.Contains("dep split:") ||
                lower.Contains("split complete") ||
                lower.Contains("allocated to") ||
                lower.Contains("posted to deposits"))
                return "This is an audit log entry, not an error. No action required.";

            if (lower.Contains("compound reducing"))
                return "This is an interest calculation audit log. No action required.";

            // ── Member errors ──
            if (lower.Contains("no customer") || (lower.Contains("not found") && lower.Contains("customer")))
                return $"Register member with payroll number '{payrollNumber}' in the system, then re-import.";

            // ── Account errors ──
            if (lower.Contains("no deposits account"))
                return $"Create a DEPOSITS savings account for member with payroll '{payrollNumber}', then re-import.";

            if (lower.Contains("no savings product") || (lower.Contains("savings product") && lower.Contains("not found")))
                return $"Configure savings product code '{productCode}' with a valid Chart of Account, then re-import.";

            if (lower.Contains("no loan account"))
                return $"Ensure member '{payrollNumber}' has a loan account for product code '{productCode}'. Check swiftFin_CustomerAccounts.";

            // ── Loan errors ──
            if (lower.Contains("no active loan") || lower.Contains("no loan case"))
                return $"Member '{payrollNumber}' has no active disbursed loan. Verify the loan has been disbursed before importing repayments.";

            if (lower.Contains("no match for loan product"))
                return $"Loan product code '{productCode}' does not exist. Verify the product code in swiftFin_LoanProducts.";

            // ── Configuration errors ──
            if (lower.Contains("chartofaccountid") || (lower.Contains("missing") && lower.Contains("chart")))
                return $"Product '{productCode}' is missing a Chart of Account mapping. Configure ChartOfAccountId on the product.";

            if (lower.Contains("interestreceivedchartofaccountid") || lower.Contains("missing interest"))
                return $"Loan product '{productCode}' is missing InterestReceivedChartOfAccountId. Configure it on the loan product.";

            if (lower.Contains("monthly payback") && lower.Contains("0"))
                return $"Loan case for '{payrollNumber}' has MonthlyPaybackAmount = 0. Recalculate and update the loan's repayment schedule.";

            // ── Data errors ──
            if (lower.Contains("unable to parse amount") || lower.Contains("cannot parse amount"))
                return $"Amount '{amount}' in the CSV is not a valid number. Correct the value in the spreadsheet and re-import.";

            if (lower.Contains("unable to parse product code") || lower.Contains("cannot parse product code"))
                return $"Product code '{productCode}' in the CSV is not a valid number. Correct the value in the spreadsheet and re-import.";

            // ── Duplicate records ──
            if (lower.Contains("ambiguous") || lower.Contains("accounts for payroll"))
                return $"Multiple accounts found for payroll '{payrollNumber}'. Deduplicate customer accounts in the system.";

            // ── Standing orders ──
            if (lower.Contains("no standing order"))
                return $"Set up a standing order for member '{payrollNumber}' before using fuzzy matching, or disable fuzzy matching for this batch.";

            // ── Excess / investment ──
            if (lower.Contains("excess") && lower.Contains("investment"))
                return $"Member '{payrollNumber}' has excess funds but no investment account (code 222). Create an investment account for this member.";

            // ── Remaining amount ──
            if (lower.Contains("no remaining amount"))
                return $"CSV total for '{payrollNumber}' was fully consumed by earlier loans. Verify the total amount in the CSV covers all active loans.";

            return "Review this record manually and re-import after correcting the data.";
        }



        // DEBUG QUERY — run this to check what ProductCode the loan accounts have:
        // SELECT ca.Id, ca.CustomerAccountType_ProductCode, ca.CustomerAccountType_TargetProductCode
        // FROM swiftFin_CustomerAccounts ca
        // WHERE ca.Id IN (
        //     '062688AD-C58F-4281-A4C6-5A62BFD9024F',  -- loan 1005 account
        //     'E8670239-7CE3-444C-BEA3-EFEA426ADBE2'   -- loan 1007 account
        // )
        [HttpPost]
        [Route("InterAccountTransferBatch")]
        public async Task<IHttpActionResult> InterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO)
        {
            // Retrieve the DTO stored in session
            var serviceHeader = CreateServiceHeader();

            if (interAccountTransferBatchDTO == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Batch cannot be null."
                });
            }
            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(interAccountTransferBatchDTO.CustomerAccountId, serviceHeader);
            interAccountTransferBatchDTO.CustomerId = customerAccount.CustomerId;

            // If there are no entries, initialize a new list

            if (interAccountTransferBatchDTO.interAccountBatchEntries != null)
            {


                decimal SumAmount = interAccountTransferBatchDTO.interAccountBatchEntries.Sum(e => e.Principal + e.Interest);
                decimal totalValue = interAccountTransferBatchDTO?.AvailableBalance ?? 0;

                if (SumAmount != totalValue)
                {
                    var balance = totalValue - SumAmount;
                    return Json(new { success = false, message = $"The total value ({totalValue}) should be equal to the sum of the entries ({SumAmount}). Balance: {balance}" });
                }
            }

            // Validate the DTO
            interAccountTransferBatchDTO.ValidateAll();
            if (interAccountTransferBatchDTO.ErrorMessages.Count != 0)
            {
                return Json(new
                {
                    success = false,
                    message = interAccountTransferBatchDTO.ErrorMessages
                });
            }

            // Save the batch data
            var interAccountTransferBatch = _interAccountTransferBatchAppService.AddNewInterAccountTransferBatch(interAccountTransferBatchDTO, serviceHeader);
          
            if (interAccountTransferBatchDTO.HasErrors)
            {
                return Json(new
                {
                    success = false,
                    message = interAccountTransferBatchDTO.ErrorMessages
                });
            }

            // Save each batch entry
            foreach (var InterAccountTransferBatchEntry in interAccountTransferBatchDTO.interAccountBatchEntries)
            {
                InterAccountTransferBatchEntry.InterAccountTransferBatchId = interAccountTransferBatch.Id;
                _interAccountTransferBatchAppService.AddNewInterAccountTransferBatchEntry(InterAccountTransferBatchEntry, serviceHeader);
            }

            // Return success message in JSON
            return Json(new
            {
                success = true,
                message = "Successfully created refund batch."
            });
        }



        [HttpGet]
        [Route("FindInterTransferBatches")]
        public async Task<IHttpActionResult> FindInterTransferBatches(int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = CreateServiceHeader();

            if (pageIndex < 0) pageIndex = 0;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20; // guardrails

            try
            {
                var pageCollectionInfo =
                
                    _interAccountTransferBatchAppService.FindInterAccountTransferBatches(serviceHeader);

                return Ok(new
                {
                    success = true,
                    data = pageCollectionInfo,
                    paging = new
                    {
                        pageIndex,
                        pageSize,
                       // totalCount = pageCollectionInfo.TotalCount,
                        //totalPages = pageCollectionInfo.TotalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("FindInterTransferBatchesEntries")]
        public async Task<IHttpActionResult> FindInterTransferBatchesEntries(Guid batchId)
        {
            var serviceHeader = CreateServiceHeader();

            try
            {
                var pageCollectionInfo =  _interAccountTransferBatchAppService.FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(batchId, serviceHeader);

                return Ok(new
                {
                    success = true,
                    Data = pageCollectionInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        [HttpPost]
        [Route("PostinterAccount")]
        public async Task<IHttpActionResult> PostinterAccount(Guid batchId)
        {
            try
            {
                var serviceHeader =CreateServiceHeader();
                var interAccountTransferBatchDTO = _interAccountTransferBatchAppService.FindInterAccountTransferBatch(batchId, serviceHeader);
                var success = _interAccountTransferBatchAppService.AuthorizeInterAccountTransferBatch(interAccountTransferBatchDTO, (int)BatchAuthOption.Post, 0, serviceHeader);
                if (success)
                {
                    return Ok(interAccountTransferBatchDTO);
                }
                else
                {
                    return BadRequest("post failed");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }











        [HttpGet]
        [Route("GetCustomerShareStatement/{customerAccountId}")]
        public async Task<HttpResponseMessage> GetCustomerShareStatement(Guid customerAccountId, DateTime? startDate = null, DateTime? endDate = null, bool downloadPdf = false)
        {
            try
            {
                // Create a simple model for SQL results
                var customerData = new
                {
                    FirstName = "",
                    LastName = "",
                    Mobile = "",
                    Email = "",
                    Reference2 = "",
                    Reference3 = "",
                    BranchCode = 0,
                    CustomerSerialNumber = 0,
                    ProductCode = 0,
                    TargetProductCode = 0
                };

                var statementRows = new List<CustomerShareStatementRow>();
                decimal totalContribution = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get customer and account information - FIXED query
                    string customerQuery = @"
               SELECT TOP 1 
                   c.Individual_FirstName,
                   c.Individual_LastName,
                   c.Address_MobileLine,
                   c.Address_Email,
                   c.Reference2,
                   c.Reference3,
                   b.Code as BranchCode,
                   c.SerialNumber as CustomerSerialNumber,
                   ca.CustomerAccountType_ProductCode,
                   ca.CustomerAccountType_TargetProductCode
               FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
               INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c 
                   ON ca.CustomerId = c.Id
               INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Branches] b
                   ON ca.BranchId = b.Id
               WHERE ca.Id = @CustomerAccountId";

                    using (var cmd = new SqlCommand(customerQuery, connection))
                    {
                        cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                customerData = new
                                {
                                    FirstName = reader["Individual_FirstName"]?.ToString() ?? "",
                                    LastName = reader["Individual_LastName"]?.ToString() ?? "",
                                    Mobile = reader["Address_MobileLine"]?.ToString() ?? "",
                                    Email = reader["Address_Email"]?.ToString() ?? "",
                                    Reference2 = reader["Reference2"]?.ToString() ?? "",
                                    Reference3 = reader["Reference3"]?.ToString() ?? "",
                                    BranchCode = Convert.ToInt32(reader["BranchCode"]),
                                    CustomerSerialNumber = Convert.ToInt32(reader["CustomerSerialNumber"]),
                                    ProductCode = Convert.ToInt32(reader["CustomerAccountType_ProductCode"]),
                                    TargetProductCode = Convert.ToInt32(reader["CustomerAccountType_TargetProductCode"])
                                };
                            }
                            else
                            {
                                // Customer not found
                                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                                response.Content = new StringContent(
                                    JsonConvert.SerializeObject(new ApiResponse<object>
                                    {
                                        Success = false,
                                        Message = "Customer account not found.",
                                        Data = null
                                    }),
                                    Encoding.UTF8,
                                    "application/json");
                                return response;
                            }
                        }
                    }

                    // Now get share statement
                    using (var command = new SqlCommand("usp_GetCustomerShareStatement", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId;
                        command.Parameters.Add("@StartDate", SqlDbType.Date).Value = (object)startDate ?? DBNull.Value;
                        command.Parameters.Add("@EndDate", SqlDbType.Date).Value = (object)endDate ?? DBNull.Value;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // First result set: Statement rows
                            while (await reader.ReadAsync())
                            {
                                var row = new CustomerShareStatementRow
                                {
                                    Date = reader["Date"].ToString(),
                                    ShareContribution = Convert.ToDecimal(reader["Share Contribution"]),
                                    Cumulative = Convert.ToDecimal(reader["Cumulative"]),
                                    Description = reader["Description"].ToString()
                                };
                                statementRows.Add(row);
                            }

                            // Move to second result set: Total contribution
                            if (await reader.NextResultAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    totalContribution = reader["TotalContribution"] != DBNull.Value ?
                                        Convert.ToDecimal(reader["TotalContribution"]) : 0;
                                }
                            }
                        }
                    }
                }

                // Build the full account number using the same logic as in the DTO
                string fullAccountNumber = string.Format("{0}-{1}-{2}-{3}",
                    customerData.BranchCode.ToString().PadLeft(3, '0'),
                    customerData.CustomerSerialNumber.ToString().PadLeft(7, '0'),
                    customerData.ProductCode.ToString().PadLeft(3, '0'),
                    customerData.TargetProductCode.ToString().PadLeft(3, '0'));

                // Create the result object with customer info
                var shareStatementResult = new
                {
                    Customer = new
                    {
                        FullName = $"{customerData.FirstName} {customerData.LastName}".Trim(),
                        AccountNumber = fullAccountNumber,
                        StaffNo = customerData.Reference2,
                        PFNumber = customerData.Reference3,
                        Mobile = customerData.Mobile,
                        Email = customerData.Email
                    },
                    Statement = statementRows,
                    TotalContribution = totalContribution
                };

                // If PDF download is requested
                if (downloadPdf)
                {
                    byte[] pdfBytes = GenerateShareStatementPdf(customerData, fullAccountNumber, statementRows, totalContribution, startDate, endDate);

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(pdfBytes)
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    string customerName = $"{customerData.FirstName}_{customerData.LastName}".Replace(" ", "_");
                    response.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"ShareStatement_{customerName}_{DateTime.Now:yyyyMMdd}.pdf"
                        };

                    return response;
                }
                else
                {
                    // Return JSON response
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(
                        JsonConvert.SerializeObject(new ApiResponse<object>
                        {
                            Success = true,
                            Message = statementRows.Count > 0 ?
                                $"Share statement retrieved successfully. Total: {totalContribution:C}" :
                                "No transactions found for the given period.",
                            Data = shareStatementResult
                        }),
                        Encoding.UTF8,
                        "application/json");
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while retrieving customer share statement.",
                        Data = ex.Message
                    }),
                    Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
        [HttpPost]
        [Route("create-withdrawal-notification")]
        public async Task<HttpResponseMessage> CreateWithdrawalNotification([FromBody] CreateWithdrawalNotificationRequest request)
        {
            try
            {
                // -------------------------------------------------------------------------
                // Validate the request
                // -------------------------------------------------------------------------
                if (request == null)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request", "Request body cannot be empty");
                }

                if (request.CustomerId == Guid.Empty)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Customer ID is required", "Please provide a valid Customer ID");
                }

                if (request.BranchId == Guid.Empty)
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Branch ID is required", "Please provide a valid Branch ID");
                }

                if (string.IsNullOrWhiteSpace(request.Remarks))
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Remarks are required", "Please provide withdrawal remarks/reason");
                }

                // Validate category
                var validCategories = new[] { 0x700, 0x701, 0x702 }; // Deceased, Voluntary, Retiree
                if (!validCategories.Contains((int)request.Category))
                {
                    return CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid withdrawal category",
                        "Category must be Deceased (1792), Voluntary (1793), or Retiree (1794)");
                }

                // -------------------------------------------------------------------------
                // Calculate MaturityDate server-side — never trust the client to send this.
                //
                // Deceased         → today (immediate settlement)
                // Voluntary/Retiree → 60 business days from today (notice period)
                //
                // If the client explicitly sends a MaturityDate, use it only for Deceased
                // (e.g. backdating a death claim). For Voluntary/Retiree always calculate.
                // -------------------------------------------------------------------------
                DateTime maturityDate;

                switch (request.Category)
                {
                    case WithdrawalNotificationCategory.Deceased:
                        // Use today or whatever the client sent (allows backdating for death claims)
                        maturityDate = request.MaturityDate?.Date ?? DateTime.Today;
                        break;

                    case WithdrawalNotificationCategory.Voluntary:
                    case WithdrawalNotificationCategory.Retiree:
                        // Always calculate — 60 business days from today, ignore client value
                        maturityDate = AddBusinessDays(DateTime.Today, 60);
                        break;

                    default:
                        maturityDate = DateTime.Today;
                        break;
                }

                // -------------------------------------------------------------------------
                // Verify customer exists and get their name
                // -------------------------------------------------------------------------
                var withdrawalNotificationId = Guid.NewGuid();
                var sequentialId = Guid.NewGuid();
                string customerFullName = "";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // -------------------------------------------------------------------------
                    // Check if customer exists
                    // -------------------------------------------------------------------------
                    string customerCheckQuery = @"
                SELECT Individual_FirstName, Individual_LastName 
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] 
                WHERE Id = @CustomerId AND RecordStatus != 2";

                    using (var cmd = new SqlCommand(customerCheckQuery, connection))
                    {
                        cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = request.CustomerId;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                return CreateErrorResponse(HttpStatusCode.NotFound, "Customer not found",
                                    $"No active customer found with ID: {request.CustomerId}");
                            }

                            var firstName = reader["Individual_FirstName"]?.ToString() ?? "";
                            var lastName = reader["Individual_LastName"]?.ToString() ?? "";
                            customerFullName = $"{firstName} {lastName}".Trim();
                        }
                    }

                    // -------------------------------------------------------------------------
                    // Check for an active withdrawal notification — prevent duplicates
                    // -------------------------------------------------------------------------
                    string duplicateCheckQuery = @"
                SELECT COUNT(1) 
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications]
                WHERE CustomerId = @CustomerId 
                AND Status NOT IN (@SettledStatus, @RejectedStatus)";

                    using (var cmd = new SqlCommand(duplicateCheckQuery, connection))
                    {
                        cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = request.CustomerId;
                        cmd.Parameters.Add("@SettledStatus", SqlDbType.Int).Value = (int)WithdrawalNotificationStatus.WithdrawalSettled;
                        cmd.Parameters.Add("@RejectedStatus", SqlDbType.Int).Value = (int)WithdrawalNotificationStatus.Deferred;

                        var existingCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        if (existingCount > 0)
                        {
                            return CreateErrorResponse(HttpStatusCode.Conflict,
                                "Active withdrawal notification already exists",
                                $"Customer {customerFullName} already has a withdrawal notification in progress. " +
                                $"Please complete or cancel the existing notification before creating a new one.");
                        }
                    }

                    // -------------------------------------------------------------------------
                    // Insert the withdrawal notification
                    // -------------------------------------------------------------------------
                    string insertQuery = @"
                INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications]
                (
                    Id,
                    CustomerId,
                    BranchId,
                    Category,
                    Status,
                    Remarks,
                    MaturityDate,
                    SettlementType,
                    IsLocked,
                    SequentialId,
                    CreatedBy,
                    CreatedDate
                )
                VALUES
                (
                    @Id,
                    @CustomerId,
                    @BranchId,
                    @Category,
                    @Status,
                    @Remarks,
                    @MaturityDate,
                    @SettlementType,
                    @IsLocked,
                    @SequentialId,
                    @CreatedBy,
                    @CreatedDate
                )";

                    using (var cmd = new SqlCommand(insertQuery, connection))
                    {
                        var settlementType = request.SettlementType ?? MembershipWithdrawalSettlementType.Normal;

                        cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = withdrawalNotificationId;
                        cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = request.CustomerId;
                        cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = request.BranchId;
                        cmd.Parameters.Add("@Category", SqlDbType.Int).Value = (int)request.Category;
                        cmd.Parameters.Add("@Status", SqlDbType.Int).Value = (int)WithdrawalNotificationStatus.Registered;
                        cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = request.Remarks;
                        cmd.Parameters.Add("@MaturityDate", SqlDbType.DateTime).Value = maturityDate; // always a value now
                        cmd.Parameters.Add("@SettlementType", SqlDbType.TinyInt).Value = (byte)settlementType;
                        cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = sequentialId;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "Swiftfin_Dev";
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return CreateErrorResponse(HttpStatusCode.InternalServerError,
                                "Failed to create withdrawal notification",
                                "Database insert returned 0 rows affected");
                        }
                    }
                }

                // -------------------------------------------------------------------------
                // Return success response
                // -------------------------------------------------------------------------
                var responseData = new
                {
                    Id = withdrawalNotificationId,
                    SequentialId = sequentialId,
                    CustomerId = request.CustomerId,
                    CustomerName = customerFullName,
                    BranchId = request.BranchId,
                    Category = request.Category.ToString(),
                    CategoryValue = (int)request.Category,
                    Status = WithdrawalNotificationStatus.Registered.ToString(),
                    StatusValue = (int)WithdrawalNotificationStatus.Registered,
                    Remarks = request.Remarks,
                    MaturityDate = maturityDate,
                    SettlementType = (request.SettlementType ?? MembershipWithdrawalSettlementType.Normal).ToString(),
                    SettlementTypeValue = (int)(request.SettlementType ?? MembershipWithdrawalSettlementType.Normal),
                    CreatedDate = DateTime.Now,
                    NextSteps = new
                    {
                        Approval = "Submit for approval using approve-withdrawal endpoint",
                        Audit = "After approval, audit using audit-withdrawal endpoint",
                        Settlement = "After audit, settle using settle-withdrawal endpoint"
                    }
                };

                var successResponse = Request.CreateResponse(HttpStatusCode.OK);
                successResponse.Content = new StringContent(
                    JsonConvert.SerializeObject(new ApiResponse<object>
                    {
                        Success = true,
                        Message = $"Withdrawal notification created successfully for {customerFullName}",
                        Data = responseData
                    }),
                    Encoding.UTF8,
                    "application/json");

                return successResponse;
            }
            catch (SqlException ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Database error occurred while creating withdrawal notification",
                    $"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "An error occurred while creating withdrawal notification",
                    ex.Message);
            }
        }

        // -------------------------------------------------------------------------
        // Adds the given number of business days (Mon–Fri) to a start date.
        // Skips weekends. Does not account for public holidays.
        // -------------------------------------------------------------------------
        private static DateTime AddBusinessDays(DateTime startDate, int businessDays)
        {
            var date = startDate;
            var daysAdded = 0;

            while (daysAdded < businessDays)
            {
                date = date.AddDays(1);

                // Skip Saturday and Sunday
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    daysAdded++;
            }

            return date;
        }

        // -------------------------------------------------------------------------
        // Request model
        // -------------------------------------------------------------------------
        public class CreateWithdrawalNotificationRequest
        {
            public Guid CustomerId { get; set; }
            public Guid BranchId { get; set; }
            public WithdrawalNotificationCategory Category { get; set; }
            public string Remarks { get; set; }

            /// <summary>
            /// Only used for Deceased category to allow backdating a death claim date.
            /// For Voluntary and Retiree this is always ignored — MaturityDate is
            /// calculated server-side as 60 business days from today.
            /// </summary>
            public DateTime? MaturityDate { get; set; }

            public MembershipWithdrawalSettlementType? SettlementType { get; set; }
            public string CreatedBy { get; set; }
        }

        // -------------------------------------------------------------------------
        // Helper method for error responses
        // -------------------------------------------------------------------------
        private HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string message, string errors)
        {
            var response = Request.CreateResponse(statusCode);
            response.Content = new StringContent(
                JsonConvert.SerializeObject(new ApiResponse<object>
                {
                    Success = false,
                    Message = message,
                    Data = null,
                    Errors = errors
                }),
                Encoding.UTF8,
                "application/json");
            return response;
        }
        [HttpPost]
        [Route("settle-withdrawal-notification/{notificationId}")]
        public async Task<HttpResponseMessage> SettleWithdrawalNotification(Guid notificationId, [FromBody] SettleWithdrawalRequest request)
        {
            try
            {
                if (request == null)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Invalid request", "Request body cannot be empty");

                if (notificationId == Guid.Empty)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Notification ID is required", "Please provide a valid notification ID");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // -------------------------------------------------------------------------
                            // 1. Get the withdrawal notification
                            // -------------------------------------------------------------------------
                            WithdrawalNotificationData notification = null;

                            using (var cmd = new SqlCommand(@"
                        SELECT Id, CustomerId, BranchId, Category, Status, Remarks,
                               MaturityDate, SettlementType, CreatedBy, CreatedDate
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications]
                        WHERE Id = @NotificationId", connection, transaction))
                            {
                                cmd.Parameters.Add("@NotificationId", SqlDbType.UniqueIdentifier).Value = notificationId;
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (!await reader.ReadAsync())
                                        return BuildErrorResponse(HttpStatusCode.NotFound, "Notification not found",
                                            $"No withdrawal notification found with ID: {notificationId}");

                                    notification = new WithdrawalNotificationData
                                    {
                                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                        CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId")),
                                        BranchId = reader.GetGuid(reader.GetOrdinal("BranchId")),
                                        Category = Convert.ToInt32(reader["Category"]),
                                        Status = Convert.ToInt32(reader["Status"]),
                                        Remarks = reader["Remarks"]?.ToString() ?? "",
                                        SettlementType = reader["SettlementType"] == DBNull.Value
                                            ? (byte?)null
                                            : Convert.ToByte(reader["SettlementType"])
                                    };
                                }
                            }

                            if (notification.Status != 4)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Cannot settle notification",
                                    $"Notification is in status '{notification.Status}'. Only Audited (4) notifications can be settled.");

                            // -------------------------------------------------------------------------
                            // 2. Get customer details
                            // -------------------------------------------------------------------------
                            CustomerInformation customer = null;

                            using (var cmd = new SqlCommand(@"
                        SELECT Id, Individual_FirstName, Individual_LastName, Reference2, Reference3
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers]
                        WHERE Id = @CustomerId", connection, transaction))
                            {
                                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = notification.CustomerId;
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        customer = new CustomerInformation
                                        {
                                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                            FirstName = reader["Individual_FirstName"]?.ToString() ?? "",
                                            LastName = reader["Individual_LastName"]?.ToString() ?? "",
                                            Reference2 = reader["Reference2"]?.ToString() ?? "",
                                            Reference3 = reader["Reference3"]?.ToString() ?? ""
                                        };
                                    }
                                }
                            }

                            if (customer == null)
                                return BuildErrorResponse(HttpStatusCode.NotFound, "Customer not found",
                                    "No customer record found for this notification.");

                            // -------------------------------------------------------------------------
                            // 3. Get required configurations
                            // -------------------------------------------------------------------------

                            // DEPOSITS savings product (Code = 1)
                            Guid depositsProductId = Guid.Empty;
                            Guid depositsChartOfAccountId = Guid.Empty;

                            using (var cmd = new SqlCommand(@"
                        SELECT TOP 1 Id, ChartOfAccountId
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts]
                        WHERE Code = 1", connection, transaction))
                            {
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        depositsProductId = reader.GetGuid(reader.GetOrdinal("Id"));
                                        depositsChartOfAccountId = reader.GetGuid(reader.GetOrdinal("ChartOfAccountId"));
                                    }
                                }
                            }

                            // Excess deposit chart of account (AccountCode = '31700006')
                            Guid excessDepositChartOfAccountId = Guid.Empty;

                            using (var cmd = new SqlCommand(@"
                        SELECT TOP 1 Id
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_ChartOfAccounts]
                        WHERE AccountCode = '31700006'", connection, transaction))
                            {
                                var result = await cmd.ExecuteScalarAsync();
                                if (result != null && result != DBNull.Value)
                                    excessDepositChartOfAccountId = (Guid)result;
                            }

                            // Current active posting period
                            Guid postingPeriodId = Guid.Empty;

                            using (var cmd = new SqlCommand(@"
                        SELECT TOP 1 Id
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_PostingPeriods]
                        WHERE GETDATE() BETWEEN Duration_StartDate AND Duration_EndDate
                        AND IsActive = 1", connection, transaction))
                            {
                                var result = await cmd.ExecuteScalarAsync();
                                if (result != null && result != DBNull.Value)
                                    postingPeriodId = (Guid)result;
                            }

                            if (depositsProductId == Guid.Empty || depositsChartOfAccountId == Guid.Empty)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Configuration missing",
                                    "DEPOSITS savings product (Code=1) not found in swiftFin_SavingsProducts.");

                            if (excessDepositChartOfAccountId == Guid.Empty)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Configuration missing",
                                    "Excess deposit chart of account (AccountCode=31700006) not found.");

                            if (postingPeriodId == Guid.Empty)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Configuration missing",
                                    "No active posting period found for today's date.");

                            // -------------------------------------------------------------------------
                            // 4. Get member's DEPOSITS account + correct balance
                            // -------------------------------------------------------------------------
                            DepositAccountData depositAccount = null;

                            string getDepositAccountQuery = @"
                        SELECT
                            ca.Id,
                            ISNULL(SUM(
                                CASE
                                    WHEN je.Amount < 0 AND wj.JournalId IS NULL
                                        THEN ABS(je.Amount)
                                    WHEN je.Amount > 0 AND wj.JournalId IS NOT NULL
                                        THEN -je.Amount
                                    ELSE 0
                                END
                            ), 0) AS BookBalance
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                        LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je
                            ON ca.Id = je.CustomerAccountId
                        LEFT JOIN (
                            SELECT DISTINCT j.Id AS JournalId
                            FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_Journals] j
                            INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je2
                                ON j.Id = je2.JournalId
                            INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca2
                                ON je2.CustomerAccountId = ca2.Id
                            WHERE ca2.CustomerId = @CustomerId
                              AND ca2.CustomerAccountType_ProductCode = 1
                              AND ca2.CustomerAccountType_TargetProductId = @ProductId
                              AND ca2.RecordStatus = 2
                              AND j.PrimaryDescription IN (
                                  'Withdrawals', 'Transfer', 'Withdrawal',
                                  'Cash Withdrawal', 'Bank Transfer', 'EFT'
                              )
                        ) wj ON je.JournalId = wj.JournalId
                        WHERE ca.CustomerId = @CustomerId
                          AND ca.CustomerAccountType_ProductCode = 1
                          AND ca.CustomerAccountType_TargetProductId = @ProductId
                          AND ca.RecordStatus = 2
                        GROUP BY ca.Id";

                            using (var cmd = new SqlCommand(getDepositAccountQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = notification.CustomerId;
                                cmd.Parameters.Add("@ProductId", SqlDbType.UniqueIdentifier).Value = depositsProductId;

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        depositAccount = new DepositAccountData
                                        {
                                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                            ChartOfAccountId = depositsChartOfAccountId,
                                            BookBalance = Convert.ToDecimal(reader["BookBalance"])
                                        };
                                    }
                                }
                            }

                            if (depositAccount == null)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "No deposit account found",
                                    $"No approved DEPOSITS account found for this member. " +
                                    $"(CustomerId={notification.CustomerId}, TargetProductId={depositsProductId})");

                            decimal availableDeposit = depositAccount.BookBalance;

                            if (availableDeposit <= 0)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Zero deposit balance",
                                    $"Member deposit balance is {availableDeposit:N2} KSH. Cannot process exit with zero or negative balance.");

                            // -------------------------------------------------------------------------
                            // 5. Get member's LOAN accounts with correct balances
                            // -------------------------------------------------------------------------
                            List<LoanAccountData> loanAccounts = new List<LoanAccountData>();
                            decimal totalLoanBalance = 0;

                            string getLoanAccountsQuery = @"
                        SELECT
                            ca.Id,
                            lp.ChartOfAccountId,
                            lp.Description AS ProductDescription,
                            lp.Code AS LoanProductCode,
                            ISNULL(SUM(je.Amount), 0) AS RawBalance
                        FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                        INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_LoanProducts] lp
                            ON ca.CustomerAccountType_TargetProductId = lp.Id
                        LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries] je
                            ON ca.Id = je.CustomerAccountId
                        WHERE ca.CustomerId = @CustomerId
                          AND ca.CustomerAccountType_ProductCode = 1
                          AND ca.RecordStatus = 2
                          AND lp.Code IN (5, 7, 9, 11, 13, 15)
                        GROUP BY ca.Id, lp.ChartOfAccountId, lp.Description, lp.Code";

                            using (var cmd = new SqlCommand(getLoanAccountsQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = notification.CustomerId;

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        decimal rawBalance = Convert.ToDecimal(reader["RawBalance"]);

                                        if (rawBalance < 0)
                                        {
                                            var loan = new LoanAccountData
                                            {
                                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                                ChartOfAccountId = reader.GetGuid(reader.GetOrdinal("ChartOfAccountId")),
                                                ProductDescription = reader["ProductDescription"]?.ToString() ?? "",
                                                LoanProductCode = Convert.ToInt32(reader["LoanProductCode"]),
                                                TotalBalance = Math.Abs(rawBalance)
                                            };
                                            totalLoanBalance += loan.TotalBalance;
                                            loanAccounts.Add(loan);
                                        }
                                    }
                                }
                            }

                            // -------------------------------------------------------------------------
                            // 6. Validate deposit can cover all outstanding loans
                            // -------------------------------------------------------------------------
                            if (loanAccounts.Count > 0 && availableDeposit < totalLoanBalance)
                            {
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Insufficient deposit to cover loans",
                                    $"Deposit balance {availableDeposit:N2} KSH is less than total loan balance {totalLoanBalance:N2} KSH. " +
                                    $"Loans: {string.Join(", ", loanAccounts.Select(l => $"{l.ProductDescription} ({l.TotalBalance:N2} KSH)"))}. " +
                                    "Please clear outstanding loan balances before settling.");
                            }

                            // -------------------------------------------------------------------------
                            // 7. Calculate amounts
                            // -------------------------------------------------------------------------
                            decimal amountToTransferToExcess = availableDeposit - totalLoanBalance;
                            string transferDescription = loanAccounts.Count > 0
                                ? "Excess Deposit After Loan Clearance"
                                : "Full Deposit Transfer (No Loans)";

                            List<Guid> createdJournals = new List<Guid>();
                            string journalReference = $"{customer.Reference2} - {notification.Remarks}";
                            string primaryDesc = $"Membership Termination - {GetWithdrawalCategoryName(notification.Category)}";

                            const string entryQuery = @"
                        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries]
                        (Id, JournalId, ChartOfAccountId, ContraChartOfAccountId, CustomerAccountId,
                         Amount, ValueDate, CreatedBy, CreatedDate)
                        VALUES
                        (@Id, @JournalId, @ChartOfAccountId, @ContraChartOfAccountId, @CustomerAccountId,
                         @Amount, @ValueDate, @CreatedBy, @CreatedDate)";

                            const string journalQuery = @"
                        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_Journals]
                        (Id, PostingPeriodId, BranchId, TotalValue, PrimaryDescription,
                         SecondaryDescription, Reference, ModuleNavigationItemCode,
                         TransactionCode, ValueDate, IsLocked, CreatedBy, CreatedDate)
                        VALUES
                        (@Id, @PostingPeriodId, @BranchId, @TotalValue, @PrimaryDescription,
                         @SecondaryDescription, @Reference, @ModuleNavigationItemCode,
                         @TransactionCode, @ValueDate, @IsLocked, @CreatedBy, @CreatedDate)";

                            // -------------------------------------------------------------------------
                            // 8. For each loan: debit deposit, credit loan, freeze loan account
                            // -------------------------------------------------------------------------
                            foreach (var loan in loanAccounts)
                            {
                                Guid journalId = Guid.NewGuid();

                                using (var cmd = new SqlCommand(journalQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = journalId;
                                    cmd.Parameters.Add("@PostingPeriodId", SqlDbType.UniqueIdentifier).Value = postingPeriodId;
                                    cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = notification.BranchId;
                                    cmd.Parameters.Add("@TotalValue", SqlDbType.Decimal).Value = loan.TotalBalance;
                                    cmd.Parameters.Add("@PrimaryDescription", SqlDbType.NVarChar).Value = primaryDesc;
                                    cmd.Parameters.Add("@SecondaryDescription", SqlDbType.NVarChar).Value = $"Loan Clearance - {loan.ProductDescription}";
                                    cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = journalReference;
                                    cmd.Parameters.Add("@ModuleNavigationItemCode", SqlDbType.Int).Value = request.ModuleNavigationItemCode;
                                    cmd.Parameters.Add("@TransactionCode", SqlDbType.Int).Value = 6;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = false;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                // Debit: deposit account (reduces deposit)
                                using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = journalId;
                                    cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.ChartOfAccountId;
                                    cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = loan.ChartOfAccountId;
                                    cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.Id;
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = -loan.TotalBalance;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                // Credit: loan account (clears the loan)
                                using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = journalId;
                                    cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = loan.ChartOfAccountId;
                                    cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.ChartOfAccountId;
                                    cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = loan.Id;
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = loan.TotalBalance;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                createdJournals.Add(journalId);

                                // Freeze loan account
                                using (var cmd = new SqlCommand(@"
                            UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
                            SET RecordStatus = 3
                            WHERE Id = @AccountId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = loan.Id;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }

                            // -------------------------------------------------------------------------
                            // 9. Transfer excess/full deposit to account 31700006
                            // -------------------------------------------------------------------------
                            if (amountToTransferToExcess > 0)
                            {
                                Guid transferJournalId = Guid.NewGuid();

                                using (var cmd = new SqlCommand(journalQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = transferJournalId;
                                    cmd.Parameters.Add("@PostingPeriodId", SqlDbType.UniqueIdentifier).Value = postingPeriodId;
                                    cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = notification.BranchId;
                                    cmd.Parameters.Add("@TotalValue", SqlDbType.Decimal).Value = amountToTransferToExcess;
                                    cmd.Parameters.Add("@PrimaryDescription", SqlDbType.NVarChar).Value = primaryDesc;
                                    cmd.Parameters.Add("@SecondaryDescription", SqlDbType.NVarChar).Value = transferDescription;
                                    cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = journalReference;
                                    cmd.Parameters.Add("@ModuleNavigationItemCode", SqlDbType.Int).Value = request.ModuleNavigationItemCode;
                                    cmd.Parameters.Add("@TransactionCode", SqlDbType.Int).Value = 6;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = false;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                // Debit: deposit account
                                using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = transferJournalId;
                                    cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.ChartOfAccountId;
                                    cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = excessDepositChartOfAccountId;
                                    cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.Id;
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = -amountToTransferToExcess;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                // Credit: excess account 31700006
                                using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                                {
                                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = transferJournalId;
                                    cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = excessDepositChartOfAccountId;
                                    cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.ChartOfAccountId;
                                    cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
                                    cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amountToTransferToExcess;
                                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                createdJournals.Add(transferJournalId);
                            }

                            // -------------------------------------------------------------------------
                            // 10. Freeze deposit account
                            // -------------------------------------------------------------------------
                            using (var cmd = new SqlCommand(@"
                        UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts]
                        SET RecordStatus = 3
                        WHERE Id = @AccountId", connection, transaction))
                            {
                                cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.Id;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // -------------------------------------------------------------------------
                            // 11. Update notification to Settled (Status = 8)
                            // -------------------------------------------------------------------------
                            using (var cmd = new SqlCommand(@"
                        UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications]
                        SET Status = 8,
                            SettledBy = @SettledBy,
                            SettledDate = @SettledDate,
                            SettlementRemarks = @SettlementRemarks,
                            SettlementType = @SettlementType
                        WHERE Id = @NotificationId", connection, transaction))
                            {
                                cmd.Parameters.Add("@NotificationId", SqlDbType.UniqueIdentifier).Value = notificationId;
                                cmd.Parameters.Add("@SettledBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                cmd.Parameters.Add("@SettledDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@SettlementRemarks", SqlDbType.NVarChar).Value = request.SettlementRemarks ?? notification.Remarks;
                                cmd.Parameters.Add("@SettlementType", SqlDbType.TinyInt).Value = notification.SettlementType ?? (byte)1;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // -------------------------------------------------------------------------
                            // 12. Update customer record — mark as exited (RecordStatus = 1)
                            // -------------------------------------------------------------------------
                            using (var cmd = new SqlCommand(@"
                        UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers]
                        SET RecordStatus = 1,
                            Remarks = @Remarks
                        WHERE Id = @CustomerId", connection, transaction))
                            {
                                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = notification.CustomerId;
                                cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value =
                                    $"Member exited on {DateTime.Now:yyyy-MM-dd}. {notification.Remarks}";
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // -------------------------------------------------------------------------
                            // 12b. Stage settlement summary record
                            //      SequentialId is a uniqueidentifier — generate a new Guid
                            // -------------------------------------------------------------------------
                            using (var cmd = new SqlCommand(@"
                        INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalSettlements]
                        (Id, WithdrawalNotificationId, CustomerAccountId, Principal, Interest, CarryForwards,
                         Reference, SequentialId, CreatedBy, CreatedDate)
                        VALUES
                        (@Id, @WithdrawalNotificationId, @CustomerAccountId, @Principal, @Interest, @CarryForwards,
                         @Reference, @SequentialId, @CreatedBy, @CreatedDate)", connection, transaction))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@WithdrawalNotificationId", SqlDbType.UniqueIdentifier).Value = notificationId;
                                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = depositAccount.Id;
                                cmd.Parameters.Add("@Principal", SqlDbType.Decimal).Value = availableDeposit;
                                cmd.Parameters.Add("@Interest", SqlDbType.Decimal).Value = 0m;
                                cmd.Parameters.Add("@CarryForwards", SqlDbType.Decimal).Value = amountToTransferToExcess;
                                cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = journalReference;
                                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();

                            // -------------------------------------------------------------------------
                            // 13. Return success
                            // -------------------------------------------------------------------------
                            var responseData = new
                            {
                                NotificationId = notificationId,
                                CustomerName = $"{customer.FirstName} {customer.LastName}",
                                InitialDepositBalance = availableDeposit,
                                TotalLoanBalance = totalLoanBalance,
                                LoansCleared = loanAccounts.Count,
                                AmountTransferredTo31700006 = amountToTransferToExcess,
                                TransferDescription = transferDescription,
                                JournalsPosted = createdJournals.Count,
                                SettlementDate = DateTime.Now,
                                Notes = loanAccounts.Count > 0
                                    ? $"Deposit of {availableDeposit:N2} KSH used to clear {loanAccounts.Count} loan(s) totalling {totalLoanBalance:N2} KSH. " +
                                      $"Excess {amountToTransferToExcess:N2} KSH transferred to account 31700006."
                                    : $"No loans found. Full deposit of {amountToTransferToExcess:N2} KSH transferred to account 31700006."
                            };

                            var successResponse = Request.CreateResponse(HttpStatusCode.OK);
                            successResponse.Content = new StringContent(
                                JsonConvert.SerializeObject(new
                                {
                                    Success = true,
                                    Message = "Withdrawal notification settled successfully",
                                    Data = responseData,
                                    Errors = (string)null
                                }),
                                Encoding.UTF8, "application/json");

                            return successResponse;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "Database error occurred", ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "An error occurred", ex.Message);
            }
        }

        private string GetWithdrawalCategoryName(int category)
        {
            switch (category)
            {
                case 1792: return "Deceased";
                case 1793: return "Voluntary";
                case 1794: return "Retiree";
                default: return "Unknown";
            }
        }

        private HttpResponseMessage BuildErrorResponse(HttpStatusCode statusCode, string message, string errors)
        {
            var response = Request.CreateResponse(statusCode);
            response.Content = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Message = message,
                    Data = (object)null,
                    Errors = errors
                }),
                Encoding.UTF8, "application/json");
            return response;
        }

        private class WithdrawalNotificationData
        {
            public Guid Id { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BranchId { get; set; }
            public int Category { get; set; }
            public int Status { get; set; }
            public string Remarks { get; set; }
            public byte? SettlementType { get; set; }
        }

        private class CustomerInformation
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Reference2 { get; set; }
            public string Reference3 { get; set; }
        }

        private class DepositAccountData
        {
            public Guid Id { get; set; }
            public Guid ChartOfAccountId { get; set; }
            public decimal BookBalance { get; set; }
        }

        private class LoanAccountData
        {
            public Guid Id { get; set; }
            public Guid ChartOfAccountId { get; set; }
            public decimal TotalBalance { get; set; }
            public string ProductDescription { get; set; }
            public int LoanProductCode { get; set; }
        }

        public class SettleWithdrawalRequest
        {
            public int ModuleNavigationItemCode { get; set; }
            public string SettlementRemarks { get; set; }
            public string CreatedBy { get; set; }
        }

        [HttpGet]
        [Route("get-pending-payouts")]
        public async Task<HttpResponseMessage> GetPendingPayouts()
        {
            try
            {
                var pendingPayouts = new List<PendingPayoutDTO>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                SELECT 
                    wns.Id                              AS SettlementId,
                    wns.WithdrawalNotificationId,
                    wns.CustomerAccountId,
                    wns.Principal,
                    wns.Interest,
                    wns.CarryForwards,
                    wns.Reference                       AS SettlementReference,
                    wns.SequentialId,
                    wns.CreatedDate                     AS SettlementDate,
                    wn.Status,
                    wn.SettledDate,
                    wn.SettlementRemarks,
                    c.Id                                AS CustomerId,
                    c.Individual_FirstName,
                    c.Individual_LastName,
                    c.Reference2                        AS MemberNumber,
                    c.Reference3                        AS PFNumber,
                    sp.Code                             AS AccountCode,
                    sp.Description                      AS AccountDescription
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalSettlements] wns
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications] wn
                    ON wns.WithdrawalNotificationId = wn.Id
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
                    ON wn.CustomerId = c.Id
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                    ON wns.CustomerAccountId = ca.Id
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_SavingsProducts] sp
                    ON ca.CustomerAccountType_TargetProductId = sp.Id
                WHERE wn.Status IN (8, 32)
                ORDER BY wn.SettledDate DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pendingPayouts.Add(new PendingPayoutDTO
                            {
                                SettlementId = reader.GetGuid(reader.GetOrdinal("SettlementId")),
                                WithdrawalNotificationId = reader.GetGuid(reader.GetOrdinal("WithdrawalNotificationId")),
                                CustomerAccountId = reader.GetGuid(reader.GetOrdinal("CustomerAccountId")),
                                CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId")),
                                SequentialId = reader.GetGuid(reader.GetOrdinal("SequentialId")),
                                CustomerName = $"{reader["Individual_FirstName"]} {reader["Individual_LastName"]}".Trim(),
                                MemberNumber = reader["MemberNumber"]?.ToString() ?? "",
                                PFNumber = reader["PFNumber"]?.ToString() ?? "",
                                AccountCode = reader["AccountCode"]?.ToString() ?? "",
                                AccountDescription = reader["AccountDescription"]?.ToString() ?? "",
                                SettlementReference = reader["SettlementReference"]?.ToString() ?? "",
                                Principal = Convert.ToDecimal(reader["Principal"]),
                                Interest = Convert.ToDecimal(reader["Interest"]),
                                CarryForwards = Convert.ToDecimal(reader["CarryForwards"]),
                                SettlementDate = reader["SettlementDate"] != DBNull.Value
                                                            ? Convert.ToDateTime(reader["SettlementDate"])
                                                            : (DateTime?)null,
                                SettledDate = reader["SettledDate"] != DBNull.Value
                                                            ? Convert.ToDateTime(reader["SettledDate"])
                                                            : (DateTime?)null,
                                Remarks = reader["SettlementRemarks"]?.ToString() ?? ""
                            });
                        }
                    }
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Message = pendingPayouts.Count > 0
                                        ? $"{pendingPayouts.Count} record(s) retrieved successfully"
                                        : "No records found",
                        Data = pendingPayouts,
                        TotalCount = pendingPayouts.Count
                    }),
                    Encoding.UTF8,
                    "application/json");

                return response;
            }
            catch (SqlException ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "Database error occurred", ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "An error occurred", ex.Message);
            }
        }

        public class PendingPayoutDTO
        {
            public Guid SettlementId { get; set; }
            public Guid WithdrawalNotificationId { get; set; }
            public Guid CustomerAccountId { get; set; }
            public Guid CustomerId { get; set; }
            public Guid SequentialId { get; set; }
            public string CustomerName { get; set; }
            public string MemberNumber { get; set; }
            public string PFNumber { get; set; }
            public string AccountCode { get; set; }
            public string AccountDescription { get; set; }
            public string SettlementReference { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal CarryForwards { get; set; }
            public DateTime? SettlementDate { get; set; }
            public DateTime? SettledDate { get; set; }
            public string Remarks { get; set; }
        }

        [HttpPost]
        [Route("process-payout/{settlementId}")]
        public async Task<HttpResponseMessage> ProcessMemberPayout(Guid settlementId, [FromBody] MemberPayoutRequest request)
        {
            try
            {
                if (request == null)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Invalid request", "Request body cannot be empty");

                if (settlementId == Guid.Empty)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Settlement ID is required", "Please provide a valid settlement ID");

                if (request.PayoutAmount <= 0)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Invalid payout amount", "Payout amount must be greater than zero");

                if (request.PaymentChartOfAccountId == Guid.Empty)
                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Payment account required", "Please provide the chart of account to pay from");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // -------------------------------------------------------------------------
                            // 1. Get settlement + notification + customer + current principal/carryforwards
                            // -------------------------------------------------------------------------
                            Guid withdrawalNotificationId = Guid.Empty;
                            Guid branchId = Guid.Empty;
                            string settlementReference = "";
                            string firstName = "";
                            string lastName = "";
                            string memberNumber = "";
                            int notificationStatus = 0;
                            int notificationCategory = 0;
                            decimal currentPrincipal = 0;
                            decimal currentCarryForwards = 0;
                            decimal currentInterest = 0;

                            using (var cmd = new SqlCommand(@"
                SELECT
                    wns.WithdrawalNotificationId,
                    wns.Reference       AS SettlementReference,
                    wns.Principal,
                    wns.Interest,
                    wns.CarryForwards,
                    wn.BranchId,
                    wn.Status,
                    wn.Category,
                    c.Reference2        AS MemberNumber,
                    c.Individual_FirstName,
                    c.Individual_LastName
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalSettlements] wns
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications] wn
                    ON wns.WithdrawalNotificationId = wn.Id
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
                    ON wn.CustomerId = c.Id
                WHERE wns.Id = @SettlementId", connection, transaction))
                            {
                                cmd.Parameters.Add("@SettlementId", SqlDbType.UniqueIdentifier).Value = settlementId;
                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    if (!await reader.ReadAsync())
                                        return BuildErrorResponse(HttpStatusCode.NotFound, "Settlement not found",
                                            $"No settlement record found with ID: {settlementId}");

                                    withdrawalNotificationId = reader.GetGuid(reader.GetOrdinal("WithdrawalNotificationId"));
                                    branchId = reader.GetGuid(reader.GetOrdinal("BranchId"));
                                    settlementReference = reader["SettlementReference"]?.ToString() ?? "";
                                    notificationStatus = Convert.ToInt32(reader["Status"]);
                                    notificationCategory = Convert.ToInt32(reader["Category"]);
                                    memberNumber = reader["MemberNumber"]?.ToString() ?? "";
                                    firstName = reader["Individual_FirstName"]?.ToString() ?? "";
                                    lastName = reader["Individual_LastName"]?.ToString() ?? "";
                                    currentPrincipal = Convert.ToDecimal(reader["Principal"]);
                                    currentInterest = Convert.ToDecimal(reader["Interest"]);
                                    currentCarryForwards = Convert.ToDecimal(reader["CarryForwards"]);
                                }
                            }

                            if (notificationStatus != 8 && notificationStatus != 32)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Cannot process payout",
                                    $"Notification status is '{notificationStatus}'. Only Settled (8) or Death Claim Settled (32) are eligible.");

                            // Determine the remaining amount (principal or carryforwards)
                            decimal remainingAmount = currentCarryForwards > 0 ? currentCarryForwards : currentPrincipal;

                            if (request.PayoutAmount > remainingAmount)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Insufficient balance",
                                    $"Payout of {request.PayoutAmount:N2} KSH exceeds available balance of {remainingAmount:N2} KSH.");

                            // -------------------------------------------------------------------------
                            // 2. Get 31700006 CoA Id (holding account)
                            // -------------------------------------------------------------------------
                            Guid excessCoAId = Guid.Empty;

                            using (var cmd = new SqlCommand(@"
                SELECT Id
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_ChartOfAccounts] coa
                WHERE coa.AccountCode = '31700006'", connection, transaction))
                            {
                                var result = await cmd.ExecuteScalarAsync();
                                if (result != null && result != DBNull.Value)
                                    excessCoAId = (Guid)result;
                                else
                                    return BuildErrorResponse(HttpStatusCode.BadRequest, "Configuration missing",
                                        "Chart of account 31700006 not found.");
                            }

                            // -------------------------------------------------------------------------
                            // 3. Get active posting period
                            // -------------------------------------------------------------------------
                            Guid postingPeriodId = Guid.Empty;

                            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 Id
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_PostingPeriods]
                WHERE GETDATE() BETWEEN Duration_StartDate AND Duration_EndDate
                AND IsActive = 1", connection, transaction))
                            {
                                var result = await cmd.ExecuteScalarAsync();
                                if (result != null && result != DBNull.Value)
                                    postingPeriodId = (Guid)result;
                            }

                            if (postingPeriodId == Guid.Empty)
                                return BuildErrorResponse(HttpStatusCode.BadRequest, "Configuration missing",
                                    "No active posting period found for today's date.");

                            // -------------------------------------------------------------------------
                            // 4. Update the settlement record (reduce Principal or CarryForwards)
                            // -------------------------------------------------------------------------
                            decimal newRemainingAmount = remainingAmount - request.PayoutAmount;
                            bool isFullPayout = newRemainingAmount == 0;

                            if (currentCarryForwards > 0)
                            {
                                using (var cmd = new SqlCommand(@"
                    UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalSettlements]
                    SET CarryForwards = @NewCarryForwards
                    WHERE Id = @SettlementId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@NewCarryForwards", SqlDbType.Decimal).Value = newRemainingAmount;
                                    cmd.Parameters.Add("@SettlementId", SqlDbType.UniqueIdentifier).Value = settlementId;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                using (var cmd = new SqlCommand(@"
                    UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalSettlements]
                    SET Principal = @NewPrincipal
                    WHERE Id = @SettlementId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@NewPrincipal", SqlDbType.Decimal).Value = newRemainingAmount;
                                    cmd.Parameters.Add("@SettlementId", SqlDbType.UniqueIdentifier).Value = settlementId;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }

                            // -------------------------------------------------------------------------
                            // 5. Post journal entries
                            //    DR: 31700006  → Amount = -PayoutAmount  (debit holding, reduces liability)
                            //    CR: PaymentChartOfAccountId → Amount = -PayoutAmount  (credit payment account, funds flow out)
                            // -------------------------------------------------------------------------
                            Guid payoutJournalId = Guid.NewGuid();
                            Guid journalSequentialId = Guid.NewGuid();
                            string primaryDesc = $"Member Payout - {GetWithdrawalCategoryName(notificationCategory)}";
                            string secondaryDesc = isFullPayout ? "Full Payout" : $"Partial Payout - Remaining: {newRemainingAmount:N2} KSH";

                            // Journal header
                            using (var cmd = new SqlCommand(@"
                INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_Journals]
                (Id, PostingPeriodId, BranchId, TotalValue, PrimaryDescription,
                 SecondaryDescription, Reference, ModuleNavigationItemCode,
                 TransactionCode, ValueDate, IsLocked, SequentialId, CreatedBy, CreatedDate)
                VALUES
                (@Id, @PostingPeriodId, @BranchId, @TotalValue, @PrimaryDescription,
                 @SecondaryDescription, @Reference, @ModuleNavigationItemCode,
                 @TransactionCode, @ValueDate, @IsLocked, @SequentialId, @CreatedBy, @CreatedDate)",
                                connection, transaction))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = payoutJournalId;
                                cmd.Parameters.Add("@PostingPeriodId", SqlDbType.UniqueIdentifier).Value = postingPeriodId;
                                cmd.Parameters.Add("@BranchId", SqlDbType.UniqueIdentifier).Value = branchId;
                                cmd.Parameters.Add("@TotalValue", SqlDbType.Decimal).Value = request.PayoutAmount;
                                cmd.Parameters.Add("@PrimaryDescription", SqlDbType.NVarChar).Value = primaryDesc;
                                cmd.Parameters.Add("@SecondaryDescription", SqlDbType.NVarChar).Value = secondaryDesc;
                                cmd.Parameters.Add("@Reference", SqlDbType.NVarChar).Value = settlementReference;
                                cmd.Parameters.Add("@ModuleNavigationItemCode", SqlDbType.Int).Value = request.ModuleNavigationItemCode;
                                cmd.Parameters.Add("@TransactionCode", SqlDbType.Int).Value = 6;
                                cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = false;
                                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = journalSequentialId;
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            const string entryQuery = @"
                INSERT INTO [SwiftFinancialsDB_Live].[dbo].[swiftFin_JournalEntries]
                (Id, JournalId, ChartOfAccountId, ContraChartOfAccountId, CustomerAccountId,
                 Amount, ValueDate, SequentialId, CreatedBy, CreatedDate)
                VALUES
                (@Id, @JournalId, @ChartOfAccountId, @ContraChartOfAccountId, @CustomerAccountId,
                 @Amount, @ValueDate, @SequentialId, @CreatedBy, @CreatedDate)";

                            // ✅ DR: 31700006 — negative, debits/reduces the holding account
                            using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = payoutJournalId;
                                cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = excessCoAId;
                                cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = request.PaymentChartOfAccountId;
                                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
                                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = -request.PayoutAmount;
                                cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // ✅ CR: Payment account — ALSO negative, credits/reduces the payment account (funds flow out)
                            using (var cmd = new SqlCommand(entryQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@JournalId", SqlDbType.UniqueIdentifier).Value = payoutJournalId;
                                cmd.Parameters.Add("@ChartOfAccountId", SqlDbType.UniqueIdentifier).Value = request.PaymentChartOfAccountId;
                                cmd.Parameters.Add("@ContraChartOfAccountId", SqlDbType.UniqueIdentifier).Value = excessCoAId;
                                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = DBNull.Value;
                                cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = -request.PayoutAmount; // ← FIXED: was +, now -
                                cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@SequentialId", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = request.CreatedBy ?? "System";
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // -------------------------------------------------------------------------
                            // 6. If full payout — update notification to Paid Out (Status = 9)
                            // -------------------------------------------------------------------------
                            if (isFullPayout)
                            {
                                using (var cmd = new SqlCommand(@"
                    UPDATE [SwiftFinancialsDB_Live].[dbo].[swiftFin_MembershipWithdrawalNotifications]
                    SET Status = 9
                    WHERE Id = @NotificationId", connection, transaction))
                                {
                                    cmd.Parameters.Add("@NotificationId", SqlDbType.UniqueIdentifier).Value = withdrawalNotificationId;
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }

                            transaction.Commit();

                            // -------------------------------------------------------------------------
                            // 7. Return success response
                            // -------------------------------------------------------------------------
                            var successResponse = Request.CreateResponse(HttpStatusCode.OK);
                            successResponse.Content = new StringContent(
                                JsonConvert.SerializeObject(new
                                {
                                    Success = true,
                                    Message = isFullPayout
                                        ? "Full payout processed successfully"
                                        : $"Partial payout of {request.PayoutAmount:N2} KSH processed successfully. Remaining balance: {newRemainingAmount:N2} KSH",
                                    Data = new
                                    {
                                        SettlementId = settlementId,
                                        JournalId = payoutJournalId,
                                        CustomerName = $"{firstName} {lastName}".Trim(),
                                        MemberNumber = memberNumber,
                                        PayoutAmount = request.PayoutAmount,
                                        PreviousBalance = remainingAmount,
                                        RemainingBalance = newRemainingAmount,
                                        IsFullPayout = isFullPayout,
                                        PayoutDate = DateTime.Now
                                    },
                                    Errors = (string)null
                                }),
                                Encoding.UTF8, "application/json");

                            return successResponse;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "Database error occurred", ex.Message);
            }
            catch (Exception ex)
            {
                return BuildErrorResponse(HttpStatusCode.InternalServerError, "An error occurred", ex.Message);
            }
        }

        public class MemberPayoutRequest
        {
            public decimal PayoutAmount { get; set; }
            public Guid PaymentChartOfAccountId { get; set; }
            public int ModuleNavigationItemCode { get; set; }
            public string CreatedBy { get; set; }
        }


        #region GenerateShareStatementPdf
        private byte[] GenerateShareStatementPdf(dynamic customerData, string fullAccountNumber,
 List<CustomerShareStatementRow> statementRows, decimal totalContribution,
 DateTime? startDate = null, DateTime? endDate = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create document with smaller margins for better space utilization
                Document document = new Document(PageSize.A4, 30, 30, 50, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // ===== RUBANI SACCO COLOR THEME =====
                BaseColor SkyBlue = new BaseColor(0, 174, 239); // #00AEEF
                BaseColor Red = new BaseColor(255, 0, 0);       // #FF0000
                BaseColor DarkGray = new BaseColor(26, 26, 26); // #1A1A1A
                BaseColor LightGray = new BaseColor(217, 217, 217); // #D9D9D9
                BaseColor White = BaseColor.WHITE;
                BaseColor TableHeaderBlue = new BaseColor(173, 216, 230); // Light blue for table headers

                // Fonts using Rubani Sacco theme
                Font titleFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, DarkGray));
                Font headerFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, DarkGray));
                Font subHeaderFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, White));
                Font normalFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 9));
                Font boldFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9));
                Font smallFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, DarkGray));
                Font amountFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, DarkGray));

                // ===== CUSTOM HEADER WITH RUBANI SACCO LOGO =====
                try
                {
                    // Create a table with 1 column for logo on top, then company info below
                    PdfPTable headerTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    // Row 1: Logo centered at top
                    PdfPCell logoCell = new PdfPCell();
                    logoCell.Border = Rectangle.NO_BORDER;
                    logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    logoCell.PaddingBottom = 5f;

                    // Try to load logo from local path
                    string logoPath = @"C:/Users/Karenju/Desktop/testapidebug/Assets/Images/rubani-logo.jpeg";
                    if (File.Exists(logoPath))
                    {
                        try
                        {
                            Image logo = Image.GetInstance(logoPath);
                            logo.ScaleToFit(100, 100); // Increased size for better visibility
                            logoCell.AddElement(logo);
                        }
                        catch (Exception)
                        {
                            // Fallback to text if image fails to load
                            logoCell.AddElement(new Paragraph("RUBANI SACCO",
                                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, SkyBlue))
                            {
                                Alignment = Element.ALIGN_CENTER
                            });
                        }
                    }
                    else
                    {
                        // Use text if no logo file
                        logoCell.AddElement(new Paragraph("RUBANI SACCO",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, SkyBlue))
                        {
                            Alignment = Element.ALIGN_CENTER
                        });
                    }

                    headerTable.AddCell(logoCell);

                    // Row 2: Company Info - LEFT ALIGNED BELOW LOGO
                    PdfPCell infoCell = new PdfPCell();
                    infoCell.Border = Rectangle.NO_BORDER;
                    infoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    infoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    infoCell.PaddingTop = 5f;

                    // Company name - LEFT ALIGNED
                    var companyNamePara = new Paragraph("RUBANI SACCO",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, SkyBlue))
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(companyNamePara);

                    // Address - LEFT ALIGNED
                    var address = new Paragraph("Rubani House, Off Airport North Embakasi",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10))
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(address);

                    // Email - LEFT ALIGNED
                    var email = new Paragraph("rubanisacco@gmail.com",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10))
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(email);

                    headerTable.AddCell(infoCell);

                    document.Add(headerTable);

                    // Add decorative line (Blue-Red-Blue)
                    var lineTable = new PdfPTable(3)
                    {
                        WidthPercentage = 100,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        SpacingAfter = 10f
                    };
                    lineTable.SetWidths(new float[] { 33, 34, 33 });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = Red,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = iTextSharp.text.Rectangle.NO_BORDER
                    });

                    document.Add(lineTable);
                }
                catch (Exception)
                {
                    // Fallback header if anything goes wrong
                    var fallbackPara = new Paragraph("RUBANI SACCO\nRubani House, Off Airport North Embakasi\nrubanisacco@gmail.com",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10))
                    {
                        Alignment = Element.ALIGN_LEFT,
                        SpacingAfter = 15f,
                        IndentationLeft = 0f
                    };
                    document.Add(fallbackPara);
                }

                // ===== STATEMENT TITLE =====
                document.Add(new Paragraph("SHARES STATEMENT", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                });

                // ===== MEMBER INFORMATION SECTION =====
                string fullName = $"{customerData.FirstName} {customerData.LastName}".Trim().ToUpper();
                string staffNo = customerData.Reference2;
                string pfNumber = customerData.Reference3;

                // Use Paragraphs for member info (like in loan statement)
                Paragraph memberInfo = new Paragraph();
                memberInfo.Alignment = Element.ALIGN_LEFT;

                // Add member info with proper formatting
                memberInfo.Add(new Chunk("Name: ", boldFont));
                memberInfo.Add(new Chunk(fullName, normalFont));
                memberInfo.Add(new Chunk("   MemberNo: ", boldFont));
                memberInfo.Add(new Chunk(staffNo ?? "N/A", normalFont));
                memberInfo.Add(Chunk.NEWLINE);

                memberInfo.Add(new Chunk("Staff No: ", boldFont));
                memberInfo.Add(new Chunk(pfNumber ?? "N/A", normalFont));
                memberInfo.Add(new Chunk("   Account No: ", boldFont));
                memberInfo.Add(new Chunk(fullAccountNumber, normalFont));

                memberInfo.SpacingAfter = 15f;
                document.Add(memberInfo);

                // ===== STATEMENT PERIOD SECTION =====
                if (startDate.HasValue || endDate.HasValue)
                {
                    string periodText = "Statement Period: ";
                    if (startDate.HasValue && endDate.HasValue)
                        periodText += $"{startDate.Value:dd/MM/yyyy} to {endDate.Value:dd/MM/yyyy}";
                    else if (startDate.HasValue)
                        periodText += $"From {startDate.Value:dd/MM/yyyy}";
                    else if (endDate.HasValue)
                        periodText += $"To {endDate.Value:dd/MM/yyyy}";

                    var periodPara = new Paragraph(periodText, boldFont)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        SpacingAfter = 10f
                    };
                    document.Add(periodPara);
                }

                // ===== SUMMARY SECTION =====
                PdfPTable summaryTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingAfter = 15f
                };
                summaryTable.SetWidths(new float[] { 50, 50 });

                // Summary header
                var summaryHeader = new PdfPCell(new Phrase("SUMMARY", subHeaderFont))
                {
                    BackgroundColor = DarkGray,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 8,
                    Colspan = 2,
                    Border = Rectangle.NO_BORDER,
                    BorderWidthBottom = 2f,
                    BorderColorBottom = SkyBlue
                };
                summaryTable.AddCell(summaryHeader);

                // Total Contribution row
                summaryTable.AddCell(new PdfPCell(new Phrase("Total Share Contribution:", boldFont))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 8,
                    BackgroundColor = TableHeaderBlue
                });

                summaryTable.AddCell(new PdfPCell(new Phrase(totalContribution.ToString("N2"), amountFont))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = TableHeaderBlue
                });

                document.Add(summaryTable);

                // ===== TRANSACTIONS SECTION =====
                if (statementRows != null && statementRows.Count > 0)
                {
                    // Section header
                    var sectionHeaderPara = new Paragraph("TRANSACTION DETAILS",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, White))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 5f
                    };

                    // Create a background for the header
                    PdfPTable headerBgTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    PdfPCell headerCell = new PdfPCell(sectionHeaderPara)
                    {
                        BackgroundColor = DarkGray,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 8,
                        Border = Rectangle.NO_BORDER,
                        BorderWidthBottom = 2f,
                        BorderColorBottom = SkyBlue
                    };
                    headerBgTable.AddCell(headerCell);
                    document.Add(headerBgTable);

                    // Transactions table with 4 columns - NO BORDERS
                    PdfPTable transTable = new PdfPTable(4)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 5f
                    };
                    transTable.SetWidths(new float[] { 20, 40, 20, 20 });

                    // Table headers - NO BORDERS
                    string[] headers = { "Date", "Description", "Share Contribution", "Cumulative" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        PdfPCell headerCellItem = new PdfPCell(new Phrase(headers[i], headerFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5,
                            // NO BORDERS - remove all border widths
                            BorderWidthTop = 0f,
                            BorderWidthBottom = 0f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f
                        };
                        transTable.AddCell(headerCellItem);
                    }

                    // Add transactions with NO BORDERS between rows
                    for (int rowIndex = 0; rowIndex < statementRows.Count; rowIndex++)
                    {
                        var row = statementRows[rowIndex];

                        // Date cell - NO BORDERS
                        PdfPCell dateCell = new PdfPCell(new Phrase(row.Date, normalFont));
                        dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        dateCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        dateCell.BorderWidthTop = 0f;
                        dateCell.BorderWidthBottom = 0f;
                        dateCell.BorderWidthLeft = 0f;
                        dateCell.BorderWidthRight = 0f;
                        transTable.AddCell(dateCell);

                        // Description cell - NO BORDERS
                        PdfPCell descCell = new PdfPCell(new Phrase(row.Description ?? "", normalFont));
                        descCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        descCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        descCell.BorderWidthTop = 0f;
                        descCell.BorderWidthBottom = 0f;
                        descCell.BorderWidthLeft = 0f;
                        descCell.BorderWidthRight = 0f;
                        transTable.AddCell(descCell);

                        // Share Contribution cell - NO BORDERS
                        PdfPCell shareCell = new PdfPCell(new Phrase(row.ShareContribution.ToString("N2"), normalFont));
                        shareCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        shareCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        shareCell.BorderWidthTop = 0f;
                        shareCell.BorderWidthBottom = 0f;
                        shareCell.BorderWidthLeft = 0f;
                        shareCell.BorderWidthRight = 0f;
                        transTable.AddCell(shareCell);

                        // Cumulative cell - NO BORDERS
                        PdfPCell cumulativeCell = new PdfPCell(new Phrase(row.Cumulative.ToString("N2"), normalFont));
                        cumulativeCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cumulativeCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        cumulativeCell.BorderWidthTop = 0f;
                        cumulativeCell.BorderWidthBottom = 0f;
                        cumulativeCell.BorderWidthLeft = 0f;
                        cumulativeCell.BorderWidthRight = 0f;
                        transTable.AddCell(cumulativeCell);
                    }

                    document.Add(transTable);
                }
                else
                {
                    var noTransPara = new Paragraph("No transactions found for the selected period.", normalFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20f,
                        SpacingAfter = 20f
                    };
                    document.Add(noTransPara);
                }

                // ===== GRAND TOTAL SECTION =====
                document.Add(new Paragraph("\n"));
                PdfPTable grandTotalTable = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingAfter = 20f
                };
                grandTotalTable.SetWidths(new float[] { 70, 30 });

                grandTotalTable.AddCell(new PdfPCell(new Phrase("GRAND TOTAL SHARE CONTRIBUTION:",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, DarkGray)))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 10,
                    BackgroundColor = TableHeaderBlue
                });

                grandTotalTable.AddCell(new PdfPCell(new Phrase(totalContribution.ToString("N2"),
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, Red)))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 10,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = TableHeaderBlue
                });

                document.Add(grandTotalTable);

                // ===== CUSTOM FOOTER =====
                document.Add(new Paragraph("\n"));
                var footerPara = new Paragraph(
                    $"Statement Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Page: 1",
                    smallFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 10f
                };
                document.Add(footerPara);

                // ===== FOOTER NOTES =====
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("This is a system generated statement.", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph("For any queries, contact: rubanisacco@gmail.com", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        // Updated helper method with NO borders at all
        private PdfPCell CreateShareStyledCell(string text, Font font, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = alignment;
            cell.Padding = 5f;

            // REMOVE ALL BORDERS
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthBottom = 0f;

            return cell;
        }

        public class SasraForm6Row
        {
            public string ReportSection { get; set; }
            public string LineItem { get; set; }
            public decimal? Amount { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class SasraForm6Meta
        {
            public string SaccoName { get; set; }
            public DateTime FiscalStartDate { get; set; }
            public DateTime PeriodEndingDate { get; set; }
            public DateTime GeneratedDate { get; set; }
        }

        public class SasraReportRow
        {
            public string ReportSection { get; set; }
            public string LineItem { get; set; }
            public decimal? Amount { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class SasraReportMeta
        {
            public string SaccoName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime GeneratedDate { get; set; }
        }

        public class SasraForm5Row
        {
            public string RefNo { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
        }

        public class SasraFormMeta
        {
            public string SaccoName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime GeneratedDate { get; set; }
        }


        [HttpGet]
        [Route("GenerateSasraForm6Pdf")]
        public HttpResponseMessage GenerateSasraForm6Pdf(DateTime startDate, DateTime endDate)
        {
            var reportRows = new List<SasraForm6Row>();
            SasraForm6Meta meta = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_GenerateSASRAForm6_Report", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // 🔹 First result set (report body)
                    while (reader.Read())
                    {
                        reportRows.Add(new SasraForm6Row
                        {
                            ReportSection = reader["ReportSection"].ToString(),
                            LineItem = reader["LineItem"].ToString(),
                            Amount = reader["Amount"] == DBNull.Value
                                ? (decimal?)null
                                : Convert.ToDecimal(reader["Amount"]),
                            DisplayOrder = Convert.ToInt32(reader["DisplayOrder"])
                        });
                    }

                    // 🔹 Second result set (metadata)
                    if (reader.NextResult() && reader.Read())
                    {
                        meta = new SasraForm6Meta
                        {
                            SaccoName = reader["SaccoName"].ToString(),
                            FiscalStartDate = Convert.ToDateTime(reader["FiscalStartDate"]),
                            PeriodEndingDate = Convert.ToDateTime(reader["PeriodEndingDate"]),
                            GeneratedDate = Convert.ToDateTime(reader["GeneratedDate"])
                        };
                    }
                }
            }

            // ================= PDF GENERATION =================
            var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 36, 36, 36, 36);
            PdfWriter.GetInstance(document, ms);

            document.Open();

            // 🔹 Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var amountFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // 🔹 Header
            document.Add(new Paragraph(meta.SaccoName, titleFont));
            document.Add(new Paragraph("SASRA FORM 6 – CAPITAL ADEQUACY REPORT", titleFont));
            document.Add(new Paragraph(
                $"Period: {meta.FiscalStartDate:dd-MMM-yyyy} to {meta.PeriodEndingDate:dd-MMM-yyyy}",
                normalFont));
            document.Add(new Paragraph($"Generated: {meta.GeneratedDate:dd-MMM-yyyy HH:mm}", normalFont));
            document.Add(new Paragraph(" "));

            // 🔹 Table with NO BORDERS
            PdfPTable table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            // Set border width to 0 to remove all grid lines
            table.DefaultCell.BorderWidth = 0;
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.SetWidths(new float[] { 70, 30 });

            string currentSection = null;

            foreach (var row in reportRows.OrderBy(r => r.DisplayOrder))
            {
                // Section Header
                if (row.ReportSection != currentSection)
                {
                    currentSection = row.ReportSection;

                    var sectionCell = new PdfPCell(new Phrase(row.LineItem, sectionFont))
                    {
                        Colspan = 2,
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        Padding = 5,
                        Border = Rectangle.NO_BORDER,  // Remove border from section cells too
                        BorderWidth = 0
                    };
                    table.AddCell(sectionCell);
                    continue;
                }

                // Regular row cells
                var descriptionCell = new PdfPCell(new Phrase(row.LineItem, normalFont))
                {
                    Border = Rectangle.NO_BORDER,
                    BorderWidth = 0,
                    PaddingBottom = 3  // Add some spacing between rows
                };
                table.AddCell(descriptionCell);

                var amountCell = new PdfPCell(new Phrase(
                    row.Amount.HasValue ? row.Amount.Value.ToString("N2") : "",
                    amountFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = Rectangle.NO_BORDER,
                    BorderWidth = 0,
                    PaddingBottom = 3
                };
                table.AddCell(amountCell);
            }

            document.Add(table);
            document.Close();

            // ================= RESPONSE =================
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"SASRA_Form6_{endDate:yyyyMMdd}.pdf"
                };

            return result;
        }


        [HttpGet]
        [Route("GenerateSasraForm2LiquidityPdf")]
        public HttpResponseMessage GenerateSasraForm2LiquidityPdf(
     DateTime startDate,
     DateTime endDate)
        {
            var rows = new List<SasraReportRow>();
            SasraReportMeta meta = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "dbo.sp_GenerateSASRAForm2_Liquidity_Report", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // ===== Result Set 1: Report Body =====
                    while (reader.Read())
                    {
                        rows.Add(new SasraReportRow
                        {
                            ReportSection = reader["ReportSection"].ToString(),
                            LineItem = reader["LineItem"].ToString(),
                            Amount = reader["Amount"] == DBNull.Value
                                ? (decimal?)null
                                : Convert.ToDecimal(reader["Amount"]),
                            DisplayOrder = Convert.ToInt32(reader["DisplayOrder"])
                        });
                    }

                    // ===== Result Set 2: Metadata =====
                    if (reader.NextResult() && reader.Read())
                    {
                        meta = new SasraReportMeta
                        {
                            SaccoName = reader["SaccoName"].ToString(),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            GeneratedDate = Convert.ToDateTime(reader["GeneratedDate"])
                        };
                    }
                }
            }

            // ================= PDF =================
            var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 36, 36, 36, 36);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // ===== Header =====
            doc.Add(new Paragraph(meta.SaccoName, titleFont));
            doc.Add(new Paragraph("SASRA FORM 2 – LIQUIDITY RETURN", titleFont));
            doc.Add(new Paragraph(
                $"Period: {meta.StartDate:dd-MMM-yyyy} to {meta.EndDate:dd-MMM-yyyy}",
                normalFont));
            doc.Add(new Paragraph(
                $"Generated: {meta.GeneratedDate:dd-MMM-yyyy HH:mm}",
                normalFont));
            doc.Add(new Paragraph(" "));

            // ===== Table =====
            PdfPTable table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 70, 30 });

            // Remove all table borders
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            string currentSection = null;

            foreach (var row in rows.OrderBy(r => r.DisplayOrder))
            {
                // Section headers
                if (row.Amount == null)
                {
                    var sectionCell = new PdfPCell(
                        new Phrase(row.LineItem, sectionFont))
                    {
                        Colspan = 2,
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        Padding = 6,
                        Border = Rectangle.NO_BORDER  // Remove border for section cells too
                    };
                    table.AddCell(sectionCell);
                    continue;
                }

                // Create normal cells with NO_BORDER
                var lineItemCell = new PdfPCell(
                    new Phrase(row.LineItem, normalFont))
                {
                    Border = Rectangle.NO_BORDER
                };

                var amountCell = new PdfPCell(
                    new Phrase(row.Amount.Value.ToString("N2"), normalFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = Rectangle.NO_BORDER
                };

                table.AddCell(lineItemCell);
                table.AddCell(amountCell);
            }

            doc.Add(table);
            doc.Close();

            // ================= RESPONSE =================
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"SASRA_Form2_Liquidity_{endDate:yyyyMMdd}.pdf"
                };

            return response;
        }

        [HttpGet]
        [Route("GenerateSasraForm5InvestmentPdf")]
        public HttpResponseMessage GenerateSasraForm5InvestmentPdf(
    DateTime startDate,
    DateTime endDate)
        {
            var rows = new List<SasraForm5Row>();
            SasraFormMeta meta = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "dbo.sp_GenerateSASRAForm5_Investment_Return", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // ===== Result Set 1: Form 5 Rows =====
                    while (reader.Read())
                    {
                        rows.Add(new SasraForm5Row
                        {
                            RefNo = reader["RefNo"].ToString(),
                            Description = reader["Description"].ToString(),
                            Amount = Convert.ToDecimal(reader["Amount"])
                        });
                    }

                    // ===== Result Set 2: Metadata =====
                    if (reader.NextResult() && reader.Read())
                    {
                        meta = new SasraFormMeta
                        {
                            SaccoName = reader["SaccoName"].ToString(),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            GeneratedDate = Convert.ToDateTime(reader["GeneratedDate"])
                        };
                    }
                }
            }

            // ================= PDF GENERATION =================
            var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 36, 36, 36, 36);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // ===== Header =====
            doc.Add(new Paragraph(meta.SaccoName, titleFont));
            doc.Add(new Paragraph("SASRA FORM 5 – INVESTMENT RETURN", titleFont));
            doc.Add(new Paragraph(
                $"Period: {meta.StartDate:dd-MMM-yyyy} to {meta.EndDate:dd-MMM-yyyy}",
                normalFont));
            doc.Add(new Paragraph(
                $"Generated: {meta.GeneratedDate:dd-MMM-yyyy HH:mm}",
                normalFont));
            doc.Add(new Paragraph(" "));

            // ===== Table =====
            PdfPTable table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 15, 55, 30 });

            // Remove all table borders
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.DefaultCell.Padding = 4;
            table.DefaultCell.PaddingBottom = 6;

            // Table header - create cells with NO_BORDER
            var refHeaderCell = new PdfPCell(new Phrase("Ref", headerFont))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 8
            };

            var descHeaderCell = new PdfPCell(new Phrase("Description", headerFont))
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 8
            };

            var amountHeaderCell = new PdfPCell(new Phrase("Amount / %", headerFont))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 8
            };

            table.AddCell(refHeaderCell);
            table.AddCell(descHeaderCell);
            table.AddCell(amountHeaderCell);

            // Optional: Add a separator line after headers (if you want some visual separation)
            // If you want a clean look without any lines, remove this section
            var separatorCell = new PdfPCell(new Phrase(""))
            {
                Colspan = 3,
                Border = Rectangle.BOTTOM_BORDER,
                BorderWidthBottom = 0.5f,
                BorderColorBottom = BaseColor.LIGHT_GRAY,
                Padding = 0,
                FixedHeight = 1
            };
            table.AddCell(separatorCell);

            foreach (var row in rows)
            {
                var refCell = new PdfPCell(new Phrase(row.RefNo, normalFont))
                {
                    Border = Rectangle.NO_BORDER
                };

                var descCell = new PdfPCell(new Phrase(row.Description, normalFont))
                {
                    Border = Rectangle.NO_BORDER
                };

                var amountCell = new PdfPCell(
                    new Phrase(row.Amount.ToString("N2"), normalFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = Rectangle.NO_BORDER
                };

                table.AddCell(refCell);
                table.AddCell(descCell);
                table.AddCell(amountCell);
            }

            doc.Add(table);
            doc.Close();

            // ================= HTTP RESPONSE =================
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"SASRA_Form5_Investment_{endDate:yyyyMMdd}.pdf"
                };

            return response;
        }

        [HttpGet]
        [Route("GenerateSasraForm7IncomeStatementPdf")]
        public HttpResponseMessage GenerateSasraForm7IncomeStatementPdf(
    DateTime startDate,
    DateTime endDate,
    string csCode = null)
        {
            var rows = new List<SasraForm7Row>();
            SasraForm7Meta meta = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_GenerateSASRAForm7_Report", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;
                cmd.Parameters.Add("@CSCode", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrEmpty(csCode) ? DBNull.Value : (object)csCode;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // ===== Result Set 1: Form 7 Rows =====
                    while (reader.Read())
                    {
                        rows.Add(new SasraForm7Row
                        {
                            RefNo = reader["RefNo"].ToString(),
                            LineItem = reader["LineItem"].ToString(),
                            AmountInThousands = reader["AmountInThousands"].ToString(),
                            DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                            IsBold = Convert.ToBoolean(reader["IsBold"]),
                            IsSubTotal = Convert.ToBoolean(reader["IsSubTotal"]),
                            IsTotal = Convert.ToBoolean(reader["IsTotal"])
                        });
                    }

                    // ===== Result Set 2: Metadata =====
                    if (reader.NextResult() && reader.Read())
                    {
                        meta = new SasraForm7Meta
                        {
                            SaccoName = reader["SaccoName"].ToString(),
                            CSCode = reader["CSCode"].ToString(),
                            PeriodStartDate = Convert.ToDateTime(reader["PeriodStartDate"]),
                            PeriodEndDate = Convert.ToDateTime(reader["PeriodEndDate"]),
                            FinalNetIncome = Convert.ToDecimal(reader["FinalNetIncome"]),
                            TotalIncome = Convert.ToDecimal(reader["TotalIncome"]),
                            TotalExpense = Convert.ToDecimal(reader["TotalExpense"]),
                            GeneratedDate = Convert.ToDateTime(reader["GeneratedDate"])
                        };
                    }
                }
            }

            // ================= PDF GENERATION =================
            var ms = new MemoryStream();
            var doc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25); // Landscape for better fit
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            var italicFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9);

            // ===== Header Section =====
            // Main title
            var titleParagraph = new Paragraph("FORM 7 SASRA/007", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 5
            };
            doc.Add(titleParagraph);

            var subtitleParagraph = new Paragraph("STATEMENT OF COMPREHENSIVE INCOME", subtitleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            doc.Add(subtitleParagraph);

            // SACCO Information - Borderless table
            var saccoInfoTable = new PdfPTable(4)
            {
                WidthPercentage = 100,
                SpacingBefore = 5,
                SpacingAfter = 10
            };
            saccoInfoTable.SetWidths(new float[] { 30, 20, 30, 20 });
            saccoInfoTable.DefaultCell.Border = Rectangle.NO_BORDER; // Remove all borders

            saccoInfoTable.AddCell(CreateBorderlessCell("SACCO SOCIETY:", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell(meta.SaccoName, normalFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell("CS NUMBER:", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell(meta.CSCode ?? "N/A", normalFont, Element.ALIGN_LEFT));

            saccoInfoTable.AddCell(CreateBorderlessCell("FINANCIAL YEAR:", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell($"{meta.PeriodStartDate:yyyy} - {meta.PeriodEndDate:yyyy}", normalFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell("REPORTING PERIOD:", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell($"{meta.PeriodStartDate:dd-MMM-yyyy} to {meta.PeriodEndDate:dd-MMM-yyyy}", normalFont, Element.ALIGN_LEFT));

            saccoInfoTable.AddCell(CreateBorderlessCell("GENERATED:", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell(meta.GeneratedDate.ToString("dd-MMM-yyyy HH:mm"), normalFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell("", boldFont, Element.ALIGN_LEFT));
            saccoInfoTable.AddCell(CreateBorderlessCell("", normalFont, Element.ALIGN_LEFT));

            doc.Add(saccoInfoTable);

            // ===== Income Statement Table =====
            var incomeTable = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 10
            };
            incomeTable.SetWidths(new float[] { 15, 70, 15 });
            incomeTable.DefaultCell.Border = Rectangle.NO_BORDER; // Remove all borders

            // Table header - also borderless
            incomeTable.AddCell(CreateBorderlessCell("Ref No.", boldFont, Element.ALIGN_CENTER));
            incomeTable.AddCell(CreateBorderlessCell("", boldFont, Element.ALIGN_CENTER));
            incomeTable.AddCell(CreateBorderlessCell("KShs.'000'", boldFont, Element.ALIGN_CENTER));

            incomeTable.AddCell(CreateBorderlessCell("", boldFont, Element.ALIGN_CENTER));
            incomeTable.AddCell(CreateBorderlessCell("Year to Date", boldFont, Element.ALIGN_CENTER));
            incomeTable.AddCell(CreateBorderlessCell("", boldFont, Element.ALIGN_CENTER));

            // Add borderless rows
            foreach (var row in rows.Where(r => r.RefNo != "HEADER" && r.RefNo != "AUTH" && r.RefNo != "NOTE"))
            {
                // Determine font style based on row properties
                var cellFont = row.IsBold ? boldFont :
                              (row.IsSubTotal || row.IsTotal) ? boldFont : normalFont;

                // Reference Number cell
                incomeTable.AddCell(CreateBorderlessCell(row.RefNo, cellFont,
                    row.IsBold ? Element.ALIGN_LEFT : Element.ALIGN_LEFT));

                // Description cell
                incomeTable.AddCell(CreateBorderlessCell(row.LineItem, cellFont,
                    row.IsBold ? Element.ALIGN_LEFT : Element.ALIGN_LEFT));

                // Amount cell
                if (!string.IsNullOrEmpty(row.AmountInThousands))
                {
                    if (decimal.TryParse(row.AmountInThousands, out decimal amount))
                    {
                        incomeTable.AddCell(CreateBorderlessCell(amount.ToString("N2"), cellFont,
                            row.IsBold ? Element.ALIGN_RIGHT : Element.ALIGN_RIGHT));
                    }
                    else
                    {
                        incomeTable.AddCell(CreateBorderlessCell(row.AmountInThousands, cellFont,
                            row.IsBold ? Element.ALIGN_CENTER : Element.ALIGN_CENTER));
                    }
                }
                else
                {
                    incomeTable.AddCell(CreateBorderlessCell("", cellFont, Element.ALIGN_RIGHT));
                }
            }

            doc.Add(incomeTable);

            // ===== Summary Section =====
            doc.Add(new Paragraph(" "));

            var summaryTable = new PdfPTable(2)
            {
                WidthPercentage = 50,
                SpacingBefore = 20,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };
            summaryTable.SetWidths(new float[] { 60, 40 });
            summaryTable.DefaultCell.Border = Rectangle.NO_BORDER; // Remove all borders

            summaryTable.AddCell(CreateBorderlessCell("Total Income:", boldFont, Element.ALIGN_LEFT));
            summaryTable.AddCell(CreateBorderlessCell((meta.TotalIncome / 1000).ToString("N2"), boldFont, Element.ALIGN_RIGHT));

            summaryTable.AddCell(CreateBorderlessCell("Total Expense:", boldFont, Element.ALIGN_LEFT));
            summaryTable.AddCell(CreateBorderlessCell((meta.TotalExpense / 1000).ToString("N2"), boldFont, Element.ALIGN_RIGHT));

            summaryTable.AddCell(CreateBorderlessCell("Net Income (After Tax & Donations):", boldFont, Element.ALIGN_LEFT));
            summaryTable.AddCell(CreateBorderlessCell((meta.FinalNetIncome / 1000).ToString("N2"), boldFont, Element.ALIGN_RIGHT));

            doc.Add(summaryTable);

            // ===== Notes and Authorization =====
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph(" "));

            // Notes
            var notes = rows.Where(r => r.RefNo == "NOTE").ToList();
            foreach (var note in notes)
            {
                doc.Add(new Paragraph(note.LineItem, smallFont));
            }

            // Authorization section
            var authRows = rows.Where(r => r.RefNo == "AUTH").OrderBy(r => r.DisplayOrder).ToList();

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("AUTHORIZATION:", boldFont));

            foreach (var auth in authRows.Skip(1)) // Skip the "AUTHORIZATION:" header since we already added it
            {
                if (auth.DisplayOrder == 106) // Declaration text
                {
                    doc.Add(new Paragraph(auth.LineItem, italicFont) { SpacingBefore = 10 });
                }
                else
                {
                    var font = auth.DisplayOrder >= 107 ? normalFont : smallFont;
                    var alignment = auth.LineItem.Contains("sign") || auth.LineItem.Contains("Name") ?
                                  Element.ALIGN_LEFT : Element.ALIGN_LEFT;
                    doc.Add(new Paragraph(auth.LineItem, font) { Alignment = alignment });
                }
            }

            doc.Close();

            // ================= HTTP RESPONSE =================
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"SASRA_Form7_IncomeStatement_{endDate:yyyyMMdd}.pdf"
                };

            return response;
        }

        // Helper method to create borderless table cells
        private PdfPCell CreateBorderlessCell(string text, Font font, int alignment)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = alignment,
                Padding = 6,
                PaddingBottom = 8,
                Border = Rectangle.NO_BORDER // This removes all border lines
            };

            return cell;
        }

        // Model classes
        public class SasraForm7Row
        {
            public string RefNo { get; set; }
            public string LineItem { get; set; }
            public string AmountInThousands { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsBold { get; set; }
            public bool IsSubTotal { get; set; }
            public bool IsTotal { get; set; }
        }

        public class SasraForm7Meta
        {
            public string SaccoName { get; set; }
            public string CSCode { get; set; }
            public DateTime PeriodStartDate { get; set; }
            public DateTime PeriodEndDate { get; set; }
            public decimal FinalNetIncome { get; set; }
            public decimal TotalIncome { get; set; }
            public decimal TotalExpense { get; set; }
            public DateTime GeneratedDate { get; set; }
        }



        [HttpGet]
        [Route("GetLoanStatement/{loanCaseId}")]
        public async Task<HttpResponseMessage> GetLoanStatement(Guid loanCaseId, bool downloadPdf = false)
        {
            try
            {
                // Create a simple model for SQL results
                var customerData = new
                {
                    FirstName = "",
                    LastName = "",
                    Mobile = "",
                    Email = "",
                    Reference2 = "",
                    Reference3 = "",
                    BranchCode = 0,
                    CustomerSerialNumber = 0,
                    ProductCode = 0,
                    TargetProductCode = 0
                };

                var loanHeader = new
                {
                    LoanNumber = "",
                    LoanProductType = "",
                    AppliedLoanAmount = 0m,  // Changed from ApprovedLoanAmount
                    MonthlyRepayment = 0m,
                    CustomerAccountId = Guid.Empty,
                    MemberNumber = ""
                };

                var statementRows = new List<LoanStatementRow>();
                var summary = new LoanSummary();
                var recentTransactions = new List<RecentTransaction>(); // Not used in updated SP

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_GenerateMemberLoanStatement", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LoanCaseId", SqlDbType.UniqueIdentifier).Value = loanCaseId;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Result Set 1: Loan Header
                            if (await reader.ReadAsync())
                            {
                                loanHeader = new
                                {
                                    LoanNumber = reader["LoanNumber"]?.ToString() ?? "",
                                    LoanProductType = reader["LoanProductType"]?.ToString() ?? "",
                                    AppliedLoanAmount = reader["AppliedLoanAmount"] != DBNull.Value ? Convert.ToDecimal(reader["AppliedLoanAmount"]) : 0m, // Changed
                                    MonthlyRepayment = reader["MonthlyRepayment"] != DBNull.Value ? Convert.ToDecimal(reader["MonthlyRepayment"]) : 0m,
                                    CustomerAccountId = reader["CustomerAccountId"] != DBNull.Value ? (Guid)reader["CustomerAccountId"] : Guid.Empty,
                                    MemberNumber = reader["MemberNumber"]?.ToString() ?? ""
                                };
                            }
                            else
                            {
                                // If no loan header found, return not found
                                var errorResponse = Request.CreateResponse(HttpStatusCode.NotFound);
                                errorResponse.Content = new StringContent(
                                    JsonConvert.SerializeObject(new ApiResponse<object>
                                    {
                                        Success = false,
                                        Message = "Loan case not found.",
                                        Data = null
                                    }),
                                    Encoding.UTF8,
                                    "application/json");
                                return errorResponse;
                            }

                            // Result Set 2: Statement rows
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var row = new LoanStatementRow
                                    {
                                        PostingDate = reader["PostingDate"] != DBNull.Value ? Convert.ToDateTime(reader["PostingDate"]).ToString("yyyy-MM-dd") : "",
                                        TransactionType = reader["TransactionType"]?.ToString() ?? "",
                                        DocumentNo = reader["DocumentNo"]?.ToString() ?? "",
                                        Description = reader["Description"]?.ToString() ?? "",
                                        Debit = reader["Debit"] != DBNull.Value ? Convert.ToDecimal(reader["Debit"]) : 0m,
                                        Credit = reader["Credit"] != DBNull.Value ? Convert.ToDecimal(reader["Credit"]) : 0m,
                                        Balance = reader["RunningBalance"] != DBNull.Value ? Convert.ToDecimal(reader["RunningBalance"]) : 0m // Changed from Balance to RunningBalance
                                    };
                                    statementRows.Add(row);
                                }
                            }

                            // Result Set 3: Summary
                            if (await reader.NextResultAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    summary = new LoanSummary
                                    {
                                        TotalDisbursed = reader["TotalDisbursed"] != DBNull.Value ? Convert.ToDecimal(reader["TotalDisbursed"]) : 0m,
                                        TotalPrincipalRepaid = reader["TotalPrincipalPaid"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPrincipalPaid"]) : 0m, // Changed from TotalPrincipalRepaid
                                        TotalInterestPaid = reader["TotalInterestPaid"] != DBNull.Value ? Convert.ToDecimal(reader["TotalInterestPaid"]) : 0m,
                                        TotalInterestAccrued = reader["TotalInterestCharged"] != DBNull.Value ? Convert.ToDecimal(reader["TotalInterestCharged"]) : 0m, // Changed from TotalInterestAccrued
                                        OutstandingLoanAmount = reader["OutstandingPrincipal"] != DBNull.Value ? Convert.ToDecimal(reader["OutstandingPrincipal"]) : 0m, // Changed from OutstandingLoanAmount
                                        OutstandingLoanInterest = reader["OutstandingInterest"] != DBNull.Value ? Convert.ToDecimal(reader["OutstandingInterest"]) : 0m, // Changed from OutstandingLoanInterest
                                        TotalOutstandingBalance = reader["TotalOutstandingBalance"] != DBNull.Value ? Convert.ToDecimal(reader["TotalOutstandingBalance"]) : 0m
                                    };
                                }
                            }

                            // Note: The updated stored procedure only returns 3 result sets, not 4
                            // So we don't need to read the 4th result set (RecentTransactions)
                        }
                    }

                    // Get customer details using CustomerAccountId from loan header
                    if (loanHeader.CustomerAccountId != Guid.Empty)
                    {
                        string customerQuery = @"
                SELECT TOP 1 
                    c.Individual_FirstName,
                    c.Individual_LastName,
                    c.Address_MobileLine,
                    c.Address_Email,
                    c.Reference2,
                    c.Reference3,
                    ISNULL(b.Code, 0) as BranchCode,
                    ISNULL(c.SerialNumber, 0) as CustomerSerialNumber,
                    ISNULL(ca.CustomerAccountType_ProductCode, 0) as ProductCode,
                    ISNULL(ca.CustomerAccountType_TargetProductCode, 0) as TargetProductCode
                FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
                INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c 
                    ON ca.CustomerId = c.Id
                LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Branches] b
                    ON ca.BranchId = b.Id
                WHERE ca.Id = @CustomerAccountId";

                        using (var cmd = new SqlCommand(customerQuery, connection))
                        {
                            cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = loanHeader.CustomerAccountId;

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    customerData = new
                                    {
                                        FirstName = reader["Individual_FirstName"]?.ToString() ?? "",
                                        LastName = reader["Individual_LastName"]?.ToString() ?? "",
                                        Mobile = reader["Address_MobileLine"]?.ToString() ?? "",
                                        Email = reader["Address_Email"]?.ToString() ?? "",
                                        Reference2 = reader["Reference2"]?.ToString() ?? "",
                                        Reference3 = reader["Reference3"]?.ToString() ?? "",
                                        BranchCode = reader["BranchCode"] != DBNull.Value ? Convert.ToInt32(reader["BranchCode"]) : 0,
                                        CustomerSerialNumber = reader["CustomerSerialNumber"] != DBNull.Value ? Convert.ToInt32(reader["CustomerSerialNumber"]) : 0,
                                        ProductCode = reader["ProductCode"] != DBNull.Value ? Convert.ToInt32(reader["ProductCode"]) : 0,
                                        TargetProductCode = reader["TargetProductCode"] != DBNull.Value ? Convert.ToInt32(reader["TargetProductCode"]) : 0
                                    };
                                }
                            }
                        }
                    }
                }

                // Build the full account number
                string fullAccountNumber = string.Format("{0}-{1}-{2}-{3}",
                    customerData.BranchCode.ToString().PadLeft(3, '0'),
                    customerData.CustomerSerialNumber.ToString().PadLeft(7, '0'),
                    customerData.ProductCode.ToString().PadLeft(3, '0'),
                    customerData.TargetProductCode.ToString().PadLeft(3, '0'));

                // Create the result object
                var loanStatementResult = new
                {
                    Customer = new
                    {
                        FullName = $"{customerData.FirstName} {customerData.LastName}".Trim(),
                        AccountNumber = fullAccountNumber,
                        StaffNo = customerData.Reference2,
                        PFNumber = customerData.Reference3,
                        Mobile = customerData.Mobile,
                        Email = customerData.Email
                    },
                    LoanDetails = new
                    {
                        LoanNumber = loanHeader.LoanNumber,
                        LoanProductType = loanHeader.LoanProductType,
                        AppliedAmount = loanHeader.AppliedLoanAmount, // Changed from ApprovedAmount
                        MonthlyRepayment = loanHeader.MonthlyRepayment,
                        MemberNumber = loanHeader.MemberNumber
                    },
                    Statement = statementRows,
                    Summary = summary
                    // Note: RecentTransactions is not included as it's not returned by the updated SP
                };

                // If PDF download is requested
                if (downloadPdf)
                {
                    byte[] pdfBytes = GenerateLoanStatementPdf(customerData, loanHeader, fullAccountNumber,
                        statementRows, summary, recentTransactions);

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(pdfBytes)
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    string customerName = $"{customerData.FirstName}_{customerData.LastName}".Replace(" ", "_");
                    response.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"LoanStatement_{loanHeader.LoanNumber}_{customerName}_{DateTime.Now:yyyyMMdd}.pdf"
                        };

                    return response;
                }
                else
                {
                    // Return JSON response
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(
                        JsonConvert.SerializeObject(new ApiResponse<object>
                        {
                            Success = true,
                            Message = statementRows.Count > 0 ?
                                $"Loan statement retrieved successfully. Outstanding Balance: {summary.TotalOutstandingBalance:C}" :
                                "No transactions found for this loan.",
                            Data = loanStatementResult
                        }),
                        Encoding.UTF8,
                        "application/json");
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while retrieving loan statement.",
                        Data = ex.Message + " | Inner: " + (ex.InnerException?.Message ?? "None")
                    }),
                    Encoding.UTF8,
                    "application/json");
                return response;
            }
        }

        private byte[] GenerateLoanStatementPdf(dynamic customerData, dynamic loanHeader, string fullAccountNumber,
    List<LoanStatementRow> statementRows, LoanSummary summary, List<RecentTransaction> recentTransactions)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create document
                Document document = new Document(PageSize.A4, 30, 30, 50, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // ===== RUBANI SACCO COLOR THEME =====
                BaseColor SkyBlue = new BaseColor(0, 174, 239); // #00AEEF
                BaseColor Red = new BaseColor(255, 0, 0);       // #FF0000
                BaseColor DarkGray = new BaseColor(26, 26, 26); // #1A1A1A
                BaseColor LightGray = new BaseColor(217, 217, 217); // #D9D9D9
                BaseColor White = BaseColor.WHITE;

                // Fonts
                Font titleFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, DarkGray));
                Font headerFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, DarkGray));
                Font subHeaderFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, White));
                Font normalFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 9));
                Font boldFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9));
                Font smallFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 8, DarkGray));
                Font companyNameFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, SkyBlue));
                Font companyInfoFont = new Font(FontFactory.GetFont(FontFactory.HELVETICA, 10));

                // ===== CUSTOM HEADER WITH RUBANI SACCO LOGO =====
                try
                {
                    // Create a table with 1 column for left-aligned content
                    PdfPTable headerTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    // Row 1: Logo left-aligned at top
                    PdfPCell logoCell = new PdfPCell();
                    logoCell.Border = Rectangle.NO_BORDER;
                    logoCell.HorizontalAlignment = Element.ALIGN_LEFT; // Changed from CENTER to LEFT
                    logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    logoCell.PaddingBottom = 5f;

                    // Try to load logo from local path
                    string logoPath = @"C:\Users\ADMIN\source\repos\SwiftFinancialsNew\SwiftFinancialsSolution\TestApis\Assets\Images\rubani-logo.jpeg";
                    if (File.Exists(logoPath))
                    {
                        try
                        {
                            Image logo = Image.GetInstance(logoPath);
                            logo.ScaleToFit(100, 100); // Increased size for better visibility
                            logoCell.AddElement(logo);
                        }
                        catch (Exception)
                        {
                            // Fallback to text if image fails to load
                            logoCell.AddElement(new Paragraph("RUBANI SACCO", companyNameFont)
                            {
                                Alignment = Element.ALIGN_LEFT // Changed from CENTER to LEFT
                            });
                        }
                    }
                    else
                    {
                        // Use text if no logo file
                        logoCell.AddElement(new Paragraph("RUBANI SACCO", companyNameFont)
                        {
                            Alignment = Element.ALIGN_LEFT // Changed from CENTER to LEFT
                        });
                    }

                    headerTable.AddCell(logoCell);

                    // Row 2: Company Info - LEFT ALIGNED
                    PdfPCell infoCell = new PdfPCell();
                    infoCell.Border = Rectangle.NO_BORDER;
                    infoCell.HorizontalAlignment = Element.ALIGN_LEFT; // Changed from CENTER to LEFT
                    infoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    infoCell.PaddingTop = 5f;

                    // Company name - LEFT ALIGNED
                    var companyNamePara = new Paragraph("RUBANI SACCO", companyNameFont)
                    {
                        Alignment = Element.ALIGN_LEFT // Changed from CENTER to LEFT
                    };
                    infoCell.AddElement(companyNamePara);

                    // Address - LEFT ALIGNED
                    var address = new Paragraph("Rubani House, Off Airport North Embakasi", companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT // Changed from CENTER to LEFT
                    };
                    infoCell.AddElement(address);

                    // Email - LEFT ALIGNED
                    var email = new Paragraph("rubanisacco@gmail.com", companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT // Changed from CENTER to LEFT
                    };
                    infoCell.AddElement(email);

                    headerTable.AddCell(infoCell);

                    document.Add(headerTable);

                    // Add decorative line (Blue-Red-Blue) - Still centered for visual appeal
                    var lineTable = new PdfPTable(3)
                    {
                        WidthPercentage = 100,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        SpacingAfter = 10f
                    };
                    lineTable.SetWidths(new float[] { 33, 34, 33 });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = Red,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    document.Add(lineTable);
                }
                catch (Exception)
                {
                    // Fallback header with left alignment
                    var fallbackPara = new Paragraph("RUBANI SACCO\nRubani House, Off Airport North Embakasi\nrubanisacco@gmail.com",
                        companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT, // Changed from CENTER to LEFT
                        SpacingAfter = 15f
                    };
                    document.Add(fallbackPara);
                }

                // ===== STATEMENT TITLE =====
                // Keep title centered for emphasis
                document.Add(new Paragraph("LOAN STATEMENT", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                });

                // ===== MEMBER INFORMATION SECTION =====
                string fullName = $"{customerData.FirstName} {customerData.LastName}".Trim().ToUpper();
                string staffNo = customerData.Reference2;
                string pfNumber = customerData.Reference3;

                // Member info - LEFT ALIGNED
                Paragraph memberInfo = new Paragraph();
                memberInfo.Alignment = Element.ALIGN_LEFT; // Changed from CENTER to LEFT

                // Add member info with proper formatting
                memberInfo.Add(new Chunk("Name: ", boldFont));
                memberInfo.Add(new Chunk(fullName, normalFont));
                memberInfo.Add(new Chunk("   MemberNo: ", boldFont));
                memberInfo.Add(new Chunk(staffNo ?? "N/A", normalFont));
                memberInfo.Add(Chunk.NEWLINE);

                memberInfo.Add(new Chunk("Account No: ", boldFont));
                memberInfo.Add(new Chunk(fullAccountNumber, normalFont));
                memberInfo.Add(new Chunk("   Company: ", boldFont));
                memberInfo.Add(new Chunk("RUBANI SACCO", normalFont));

                memberInfo.SpacingAfter = 15f;
                document.Add(memberInfo);

                // ===== LOAN DETAILS SECTION =====
                // Loan details - LEFT ALIGNED
                Paragraph loanDetails = new Paragraph();
                loanDetails.Alignment = Element.ALIGN_LEFT; // Changed from CENTER to LEFT

                loanDetails.Add(new Chunk("Loan Number: ", boldFont));
                loanDetails.Add(new Chunk(loanHeader.LoanNumber, normalFont));
                loanDetails.Add(new Chunk("   Loan Product: ", boldFont));
                loanDetails.Add(new Chunk(loanHeader.LoanProductType, normalFont));
                loanDetails.Add(Chunk.NEWLINE);

                loanDetails.Add(new Chunk("Applied Amount: ", boldFont));
                loanDetails.Add(new Chunk(loanHeader.AppliedLoanAmount.ToString("N2"), normalFont));
                loanDetails.Add(new Chunk("   Monthly Repayment: ", boldFont));
                loanDetails.Add(new Chunk(loanHeader.MonthlyRepayment.ToString("N2"), normalFont));

                loanDetails.SpacingAfter = 5f;
                document.Add(loanDetails);

                // ===== ADD TOTAL OUTSTANDING BALANCE BELOW MONTHLY REPAYMENT =====
                Paragraph outstandingBalance = new Paragraph();
                outstandingBalance.Alignment = Element.ALIGN_LEFT; // Changed from CENTER to LEFT

                outstandingBalance.Add(new Chunk("TOTAL OUTSTANDING BALANCE: ",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, DarkGray)));
                outstandingBalance.Add(new Chunk(summary.OutstandingLoanAmount.ToString("N2"),
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, Red)));

                outstandingBalance.SpacingAfter = 15f;
                document.Add(outstandingBalance);

                // ===== LOAN TRANSACTIONS SECTION =====
                if (statementRows != null && statementRows.Count > 0)
                {
                    // Section header - Keep centered for visual hierarchy
                    var sectionHeader = new Paragraph("LOAN TRANSACTIONS",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, White))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 5f
                    };

                    // Create a background for the header
                    PdfPTable headerBgTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    PdfPCell headerCell = new PdfPCell(sectionHeader)
                    {
                        BackgroundColor = DarkGray,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 8,
                        Border = Rectangle.NO_BORDER,
                        BorderWidthBottom = 2f,
                        BorderColorBottom = SkyBlue
                    };
                    headerBgTable.AddCell(headerCell);
                    document.Add(headerBgTable);

                    // Transactions table
                    PdfPTable transTable = new PdfPTable(6)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 5f
                    };
                    // Column widths: Date, Type, Description, Debit, Credit, RunningBalance
                    transTable.SetWidths(new float[] { 12, 12, 40, 18, 18, 20 });

                    // Table headers
                    string[] headers = { "Date", "Type", "Description", "Debit", "Credit", "Running Balance" };

                    // Add header cells with NO BORDERS
                    for (int i = 0; i < headers.Length; i++)
                    {
                        PdfPCell headerCellItem = new PdfPCell(new Phrase(headers[i], headerFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5,
                            // NO BORDERS - remove all border widths
                            BorderWidthTop = 0f,
                            BorderWidthBottom = 0f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f
                        };
                        transTable.AddCell(headerCellItem);
                    }

                    // Add transactions with NO BORDERS between rows
                    for (int rowIndex = 0; rowIndex < statementRows.Count; rowIndex++)
                    {
                        var row = statementRows[rowIndex];

                        // Date cell - NO BORDERS
                        PdfPCell dateCell = new PdfPCell(new Phrase(row.PostingDate, normalFont));
                        dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        dateCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        dateCell.BorderWidthTop = 0f;
                        dateCell.BorderWidthBottom = 0f;
                        dateCell.BorderWidthLeft = 0f;
                        dateCell.BorderWidthRight = 0f;
                        transTable.AddCell(dateCell);

                        // Type cell - NO BORDERS
                        PdfPCell typeCell = new PdfPCell(new Phrase(row.TransactionType, normalFont));
                        typeCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        typeCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        typeCell.BorderWidthTop = 0f;
                        typeCell.BorderWidthBottom = 0f;
                        typeCell.BorderWidthLeft = 0f;
                        typeCell.BorderWidthRight = 0f;
                        transTable.AddCell(typeCell);

                        // Description cell - NO BORDERS
                        PdfPCell descCell = new PdfPCell(new Phrase(row.Description ?? "", normalFont));
                        descCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        descCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        descCell.BorderWidthTop = 0f;
                        descCell.BorderWidthBottom = 0f;
                        descCell.BorderWidthLeft = 0f;
                        descCell.BorderWidthRight = 0f;
                        transTable.AddCell(descCell);

                        // Debit cell - NO BORDERS
                        PdfPCell debitCell = new PdfPCell(new Phrase(row.Debit.ToString("N2"), normalFont));
                        debitCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        debitCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        debitCell.BorderWidthTop = 0f;
                        debitCell.BorderWidthBottom = 0f;
                        debitCell.BorderWidthLeft = 0f;
                        debitCell.BorderWidthRight = 0f;
                        transTable.AddCell(debitCell);

                        // Credit cell - NO BORDERS
                        PdfPCell creditCell = new PdfPCell(new Phrase(row.Credit.ToString("N2"), normalFont));
                        creditCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        creditCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        creditCell.BorderWidthTop = 0f;
                        creditCell.BorderWidthBottom = 0f;
                        creditCell.BorderWidthLeft = 0f;
                        creditCell.BorderWidthRight = 0f;
                        transTable.AddCell(creditCell);

                        // Running Balance cell - NO BORDERS
                        // Handle NULL values for Interest Paid rows
                        string balanceText = row.Balance == 0 && row.TransactionType == "Interest Paid"
                            ? ""  // Empty string for Interest Paid rows
                            : row.Balance.ToString("N2");

                        PdfPCell balanceCell = new PdfPCell(new Phrase(balanceText, normalFont));
                        balanceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        balanceCell.Padding = 5f;
                        // NO BORDERS - remove all border widths
                        balanceCell.BorderWidthTop = 0f;
                        balanceCell.BorderWidthBottom = 0f;
                        balanceCell.BorderWidthLeft = 0f;
                        balanceCell.BorderWidthRight = 0f;
                        transTable.AddCell(balanceCell);
                    }

                    document.Add(transTable);
                }
                else
                {
                    var noTransPara = new Paragraph("No transactions found", normalFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20f,
                        SpacingAfter = 20f
                    };
                    document.Add(noTransPara);
                }

                // ===== CUSTOM FOOTER =====
                document.Add(new Paragraph("\n"));
                var footerPara = new Paragraph(
                    $"Statement Date: {DateTime.Now:dd/MM/yyyy} | Printed on: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Page: 1",
                    smallFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 10f
                };
                document.Add(footerPara);

                // ===== FOOTER NOTES =====
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("This is a system generated statement.", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph("For any queries, contact: rubanisacco@gmail.com", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        // Helper method to add summary rows
        private void AddSummaryRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.Padding = 5f;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            valueCell.Padding = 5f;
            table.AddCell(valueCell);
        }









        [HttpGet]
        [Route("GetMemberStatement/{customerId}")]
        public async Task<HttpResponseMessage> GetMemberStatement(
     Guid customerId,
     DateTime? startDate = null,
     DateTime? endDate = null,
     bool downloadPdf = false)
        {
            try
            {
                var memberStatement = new MemberStatementResult
                {
                    CustomerId = customerId,
                    StartDate = startDate,
                    EndDate = endDate
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // ===== GET LOANS INFORMATION =====
                    var allLoanStatements = new List<LoanStatementResult>();

                    using (var loanCommand = new SqlCommand("sp_GenerateMemberLoanStatement", connection))
                    {
                        loanCommand.CommandType = CommandType.StoredProcedure;
                        loanCommand.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                        loanCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate.HasValue ? startDate.Value.Date : (object)DBNull.Value;
                        loanCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate.HasValue ? endDate.Value.Date : (object)DBNull.Value;

                        using (var reader = await loanCommand.ExecuteReaderAsync())
                        {
                            bool hasLoans = false;

                            // Process each loan - each loan has 3 result sets
                            do
                            {
                                // RESULT SET 1: Loan Header
                                if (!await reader.ReadAsync())
                                    break;

                                // *** FIX: Stop if SP returned a message instead of loan data ***
                                if (reader.FieldCount == 1 && reader.GetName(0) == "Message")
                                    break;

                                hasLoans = true;

                                // Read loan header
                                var loanNumber = reader["LoanNumber"]?.ToString() ?? "";
                                var loanProductType = reader["LoanProductType"]?.ToString() ?? "";
                                var appliedLoanAmount = GetSafeValue<decimal>(reader, "AppliedLoanAmount", 0m);
                                var monthlyRepayment = GetSafeValue<decimal>(reader, "MonthlyRepayment", 0m);
                                var customerAccountId = GetSafeValue<Guid>(reader, "CustomerAccountId", Guid.Empty);
                                var memberNumber = reader["MemberNumber"]?.ToString() ?? "";
                                var disbursedDate = reader["DisbursedDate"] != DBNull.Value ? Convert.ToDateTime(reader["DisbursedDate"]).ToString("yyyy-MM-dd") : "";

                                var statementRows = new List<LoanStatementRow>();
                                var summary = new LoanSummary();
                                DateTime? statementStartDate = null;
                                DateTime? statementEndDate = null;

                                // RESULT SET 2: Statement rows for this loan
                                if (await reader.NextResultAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        // Skip if it's a message result set (has only Message column)
                                        if (reader.FieldCount == 1 && reader.GetName(0) == "Message")
                                            continue;

                                        var row = new LoanStatementRow
                                        {
                                            TransDate = reader["TransDate"] != DBNull.Value ? Convert.ToDateTime(reader["TransDate"]).ToString("yyyy-MM-dd") : "",
                                            OpeningBalance = GetSafeValue<decimal>(reader, "OpeningBalance", 0m),
                                            Principle = GetSafeValue<decimal>(reader, "Principle", 0m),
                                            Interest = GetSafeValue<decimal>(reader, "Interest", 0m),
                                            Amount = GetSafeValue<decimal>(reader, "Amount", 0m),
                                            LoanBalance = GetSafeValue<decimal>(reader, "LoanBalance", 0m),
                                            PostingDate = reader["TransDate"] != DBNull.Value ? Convert.ToDateTime(reader["TransDate"]).ToString("yyyy-MM-dd") : "",
                                            Balance = GetSafeValue<decimal>(reader, "LoanBalance", 0m),
                                            TransactionType = GetSafeValue<string>(reader, "TransactionType", ""),
                                            Debit = 0m,
                                            Credit = 0m
                                        };
                                        statementRows.Add(row);
                                    }
                                }

                                // RESULT SET 3: Summary for this loan
                                if (await reader.NextResultAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        // Skip if it's a message
                                        if (!(reader.FieldCount == 1 && reader.GetName(0) == "Message"))
                                        {
                                            summary = new LoanSummary
                                            {
                                                TotalDisbursed = GetSafeValue<decimal>(reader, "TotalDisbursed", 0m),
                                                TotalPrincipalRepaid = GetSafeValue<decimal>(reader, "TotalPrincipalPaid", 0m),
                                                TotalInterestPaid = GetSafeValue<decimal>(reader, "TotalInterestPaid", 0m),
                                                TotalInterestAccrued = GetSafeValue<decimal>(reader, "TotalInterestAccrued", 0m),
                                                OutstandingLoanAmount = GetSafeValue<decimal>(reader, "OutstandingPrincipal", 0m),
                                                OutstandingLoanInterest = GetSafeValue<decimal>(reader, "OutstandingInterest", 0m),
                                                TotalOutstandingBalance = GetSafeValue<decimal>(reader, "OutstandingPrincipal", 0m),
                                                OpeningBalance = GetSafeValue<decimal>(reader, "OpeningBalance", 0m)
                                            };

                                            statementStartDate = GetSafeValue<DateTime?>(reader, "StartDate", null);
                                            statementEndDate = GetSafeValue<DateTime?>(reader, "EndDate", null);
                                        }
                                    }
                                }

                                // Get customer details
                                var customerData = await GetCustomerDetails(connection, customerAccountId, customerId);

                                // Build full account number
                                string fullAccountNumber = string.Format("{0}-{1}-{2}-{3}",
                                    customerData.BranchCode.ToString().PadLeft(3, '0'),
                                    customerData.CustomerSerialNumber.ToString().PadLeft(7, '0'),
                                    customerData.ProductCode.ToString().PadLeft(3, '0'),
                                    customerData.TargetProductCode.ToString().PadLeft(3, '0'));

                                // Create loan statement result
                                var loanStatementResult = new LoanStatementResult
                                {
                                    LoanNumber = loanNumber,
                                    Customer = new CustomerInfo
                                    {
                                        FullName = $"{customerData.FirstName} {customerData.LastName}".Trim(),
                                        AccountNumber = fullAccountNumber,
                                        StaffNo = customerData.Reference2,
                                        PFNumber = customerData.Reference3,
                                        Mobile = customerData.Mobile,
                                        Email = customerData.Email
                                    },
                                    LoanDetails = new LoanDetails
                                    {
                                        LoanNumber = loanNumber,
                                        LoanProductType = loanProductType,
                                        AppliedAmount = appliedLoanAmount,
                                        MonthlyRepayment = monthlyRepayment,
                                        MemberNumber = memberNumber,
                                        DisbursedDate = disbursedDate
                                    },
                                    Statement = statementRows,
                                    Summary = summary,
                                    StartDate = statementStartDate,
                                    EndDate = statementEndDate
                                };

                                allLoanStatements.Add(loanStatementResult);

                            } while (await reader.NextResultAsync());

                            // If no loans found
                            if (!hasLoans)
                            {
                                // Continue with empty loan list - shares will still be processed
                            }
                        }
                    }

                    // ===== GET SHARES INFORMATION =====
                    var allSharesStatements = new List<SharesStatementResult>();

                    using (var sharesCommand = new SqlCommand("sp_GenerateAllSharesStatement", connection))
                    {
                        sharesCommand.CommandType = CommandType.StoredProcedure;
                        sharesCommand.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                        sharesCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate.HasValue ? startDate.Value.Date : (object)DBNull.Value;
                        sharesCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate.HasValue ? endDate.Value.Date : (object)DBNull.Value;

                        using (var reader = await sharesCommand.ExecuteReaderAsync())
                        {
                            var accountTransactions = new Dictionary<Guid, List<SharesTransaction>>();
                            var accountDetails = new Dictionary<Guid, (string ProductName, decimal TotalContribution)>();
                            bool hasSharesData = false;

                            // First result set: Account Header (skip)
                            if (await reader.NextResultAsync())
                            {
                                // Second result set: Detailed Statement
                                while (await reader.ReadAsync())
                                {
                                    if (reader.FieldCount == 1 && reader.GetName(0) == "Message")
                                        continue;

                                    hasSharesData = true;

                                    var customerAccountId = GetSafeValue<Guid>(reader, "CustomerAccountId", Guid.Empty);

                                    var transaction = new SharesTransaction
                                    {
                                        TransactionDate = reader["Date"]?.ToString() ?? "",
                                        Description = reader["Description"]?.ToString() ?? "",
                                        DepositAmount = GetSafeValue<decimal>(reader, "Share Contribution", 0m),
                                        WithdrawalAmount = 0m,
                                        RunningBalance = GetSafeValue<decimal>(reader, "Cumulative", 0m)
                                    };

                                    if (!accountTransactions.ContainsKey(customerAccountId))
                                        accountTransactions[customerAccountId] = new List<SharesTransaction>();

                                    accountTransactions[customerAccountId].Add(transaction);
                                }
                            }

                            // Third result set: Summary
                            if (hasSharesData && await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    if (reader.FieldCount == 1 && reader.GetName(0) == "Message")
                                        continue;

                                    var customerAccountId = GetSafeValue<Guid>(reader, "CustomerAccountId", Guid.Empty);
                                    var productName = reader["ProductName"]?.ToString() ?? "";
                                    var totalContribution = GetSafeValue<decimal>(reader, "TotalContribution", 0m);

                                    accountDetails[customerAccountId] = (productName, totalContribution);
                                }
                            }

                            // Create shares statement results
                            foreach (var account in accountDetails)
                            {
                                var transactions = accountTransactions.ContainsKey(account.Key)
                                    ? accountTransactions[account.Key]
                                    : new List<SharesTransaction>();

                                decimal closingBalance = transactions.Any() ? transactions.Last().RunningBalance : 0m;

                                var sharesStatementResult = new SharesStatementResult
                                {
                                    StatementType = "SHARES/SAVINGS STATEMENT",
                                    ProductName = account.Value.ProductName,
                                    AccountType = "Share Account",
                                    ProductCode = 0,
                                    Period = $"{(startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : "Beginning")} to {(endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"))}",
                                    OpeningBalance = 0m,
                                    TotalDeposits = account.Value.TotalContribution,
                                    TotalWithdrawals = 0m,
                                    ClosingBalance = closingBalance,
                                    Transactions = transactions,
                                    Summary = new SharesAccountSummary
                                    {
                                        AccountName = account.Value.ProductName,
                                        AccountType = "Share Account",
                                        OpeningBalance = 0m,
                                        TotalDeposits = account.Value.TotalContribution,
                                        TotalWithdrawals = 0m,
                                        ClosingBalance = closingBalance,
                                        NetMovement = account.Value.TotalContribution
                                    }
                                };

                                allSharesStatements.Add(sharesStatementResult);
                            }
                        }
                    }

                    // ===== GET CUSTOMER INFO =====
                    CustomerInfo customerInfo = null;

                    if (allLoanStatements.Count > 0)
                    {
                        customerInfo = allLoanStatements.First().Customer;
                    }
                    else
                    {
                        customerInfo = new CustomerInfo
                        {
                            FullName = await GetCustomerName(connection, customerId),
                            AccountNumber = "N/A",
                            StaffNo = await GetCustomerStaffNo(connection, customerId),
                            Mobile = await GetCustomerMobile(connection, customerId),
                            Email = await GetCustomerEmail(connection, customerId),
                            PFNumber = await GetCustomerPFNumber(connection, customerId)
                        };
                    }

                    // ===== POPULATE MEMBER STATEMENT =====
                    memberStatement.Customer = customerInfo;
                    memberStatement.LoanStatements = allLoanStatements;
                    memberStatement.SharesStatements = allSharesStatements;
                    memberStatement.TotalLoanBalance = allLoanStatements.Sum(l => l.Summary?.TotalOutstandingBalance ?? 0);
                    memberStatement.TotalSharesBalance = allSharesStatements.Sum(s => s.ClosingBalance);
                    memberStatement.TotalAccounts = allLoanStatements.Count + allSharesStatements.Count;

                    // ===== RETURN RESPONSE =====
                    if (downloadPdf)
                    {
                        byte[] pdfBytes = GenerateMemberStatementPdf(memberStatement, startDate, endDate);
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(pdfBytes)
                        };
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                        string customerName = memberStatement.Customer?.FullName?.Replace(" ", "_") ?? "Customer";
                        string dateRange = "";
                        if (startDate.HasValue && endDate.HasValue)
                            dateRange = $"{startDate.Value:yyyyMMdd}_{endDate.Value:yyyyMMdd}";

                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = $"MemberStatement_{customerName}_{dateRange}_{DateTime.Now:yyyyMMdd}.pdf"
                        };
                        return response;
                    }
                    else
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        string message = $"Found {memberStatement.LoanStatements.Count} loan(s) and {memberStatement.SharesStatements.Count} shares/savings account(s).";
                        response.Content = new StringContent(
                            JsonConvert.SerializeObject(new ApiResponse<object>
                            {
                                Success = true,
                                Message = message,
                                Data = memberStatement
                            }),
                            Encoding.UTF8,
                            "application/json");
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while retrieving member statement.",
                        Data = ex.Message + " | Inner: " + (ex.InnerException?.Message ?? "None")
                    }),
                    Encoding.UTF8,
                    "application/json");
                return response;
            }
        }


        private T GetSafeValue<T>(SqlDataReader reader, string columnName, T defaultValue = default(T))
        {
            try
            {
                int columnIndex = reader.GetOrdinal(columnName);
                if (columnIndex >= 0 && !reader.IsDBNull(columnIndex))
                {
                    object value = reader[columnName];
                    if (value is T)
                        return (T)value;

                    // Handle type conversions
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                // Column doesn't exist in result set
            }
            catch (Exception)
            {
                // Other exception occurred
            }
            return defaultValue;
        }



        private async Task<CustomerData> GetCustomerDetails(SqlConnection connection, Guid customerAccountId, Guid customerId)
        {
            string customerQuery = @"
SELECT TOP 1
    c.Individual_FirstName,
    c.Individual_LastName,
    c.Address_MobileLine,
    c.Address_Email,
    c.Reference2,
    c.Reference3,
    ISNULL(b.Code, 0) as BranchCode,
    ISNULL(c.SerialNumber, 0) as CustomerSerialNumber,
    ISNULL(ca.CustomerAccountType_ProductCode, 0) as ProductCode,
    ISNULL(ca.CustomerAccountType_TargetProductCode, 0) as TargetProductCode
FROM [SwiftFinancialsDB_Live].[dbo].[swiftFin_CustomerAccounts] ca
INNER JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Customers] c
    ON ca.CustomerId = c.Id
LEFT JOIN [SwiftFinancialsDB_Live].[dbo].[swiftFin_Branches] b
    ON ca.BranchId = b.Id
WHERE (ca.Id = @CustomerAccountId OR c.Id = @CustomerId)
ORDER BY ca.CreatedDate DESC";

            using (var cmd = new SqlCommand(customerQuery, connection))
            {
                cmd.Parameters.Add("@CustomerAccountId", SqlDbType.UniqueIdentifier).Value = customerAccountId;
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new CustomerData
                        {
                            FirstName = reader["Individual_FirstName"]?.ToString() ?? "",
                            LastName = reader["Individual_LastName"]?.ToString() ?? "",
                            Mobile = reader["Address_MobileLine"]?.ToString() ?? "",
                            Email = reader["Address_Email"]?.ToString() ?? "",
                            Reference2 = reader["Reference2"]?.ToString() ?? "",
                            Reference3 = reader["Reference3"]?.ToString() ?? "",
                            BranchCode = reader["BranchCode"] != DBNull.Value ? Convert.ToInt32(reader["BranchCode"]) : 0,
                            CustomerSerialNumber = reader["CustomerSerialNumber"] != DBNull.Value ? Convert.ToInt32(reader["CustomerSerialNumber"]) : 0,
                            ProductCode = reader["ProductCode"] != DBNull.Value ? Convert.ToInt32(reader["ProductCode"]) : 0,
                            TargetProductCode = reader["TargetProductCode"] != DBNull.Value ? Convert.ToInt32(reader["TargetProductCode"]) : 0
                        };
                    }
                }
            }

            // Return default if no customer found
            return new CustomerData();
        }


        private async Task<string> GetCustomerName(SqlConnection connection, Guid customerId)
        {
            string query = @"
SELECT 
    CASE 
        WHEN Type = 1
        THEN CONCAT(ISNULL(Individual_FirstName, ''), ' ', ISNULL(Individual_LastName, ''))
        ELSE ISNULL(NonIndividual_Description, '')
    END AS FullName
FROM swiftFin_Customers
WHERE Id = @CustomerId";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;

                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString()?.Trim() ?? "Customer";
            }
        }


        private async Task<string> GetCustomerStaffNo(SqlConnection connection, Guid customerId)
        {
            string query = @"
SELECT Reference2 
FROM swiftFin_Customers 
WHERE Id = @CustomerId";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                return (await cmd.ExecuteScalarAsync())?.ToString() ?? "N/A";
            }
        }
        private async Task<string> GetCustomerMobile(SqlConnection connection, Guid customerId)
        {
            string query = @"
SELECT Address_MobileLine 
FROM swiftFin_Customers 
WHERE Id = @CustomerId";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                return (await cmd.ExecuteScalarAsync())?.ToString() ?? "N/A";
            }
        }

        private async Task<string> GetCustomerEmail(SqlConnection connection, Guid customerId)
        {
            string query = @"
SELECT Address_Email 
FROM swiftFin_Customers 
WHERE Id = @CustomerId";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                return (await cmd.ExecuteScalarAsync())?.ToString() ?? "N/A";
            }
        }

        private async Task<string> GetCustomerPFNumber(SqlConnection connection, Guid customerId)
        {
            string query = @"
SELECT Reference3 
FROM swiftFin_Customers 
WHERE Id = @CustomerId";

            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.UniqueIdentifier).Value = customerId;
                return (await cmd.ExecuteScalarAsync())?.ToString() ?? "N/A";
            }
        }

        public class MemberStatementResult
        {
            public Guid CustomerId { get; set; }
            public CustomerInfo Customer { get; set; }
            public List<LoanStatementResult> LoanStatements { get; set; } = new List<LoanStatementResult>();
            public List<SharesStatementResult> SharesStatements { get; set; } = new List<SharesStatementResult>();
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal TotalLoanBalance { get; set; }
            public decimal TotalSharesBalance { get; set; }
            public int TotalAccounts { get; set; }
        }

        public class SharesStatementResult
        {
            public string StatementType { get; set; }
            public string ProductName { get; set; }
            public string AccountType { get; set; }
            public int ProductCode { get; set; }
            public string Period { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal TotalDeposits { get; set; }
            public decimal TotalWithdrawals { get; set; }
            public decimal ClosingBalance { get; set; }
            public List<SharesTransaction> Transactions { get; set; } = new List<SharesTransaction>();
            public SharesAccountSummary Summary { get; set; }
        }

        public class SharesTransaction
        {
            public string TransactionDate { get; set; }
            public string Description { get; set; }
            public decimal DepositAmount { get; set; }
            public decimal WithdrawalAmount { get; set; }
            public decimal RunningBalance { get; set; }
        }

        public class SharesAccountSummary
        {
            public string AccountName { get; set; }
            public string AccountType { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal TotalDeposits { get; set; }
            public decimal TotalWithdrawals { get; set; }
            public decimal ClosingBalance { get; set; }
            public decimal NetMovement { get; set; }
        }

        private byte[] GenerateMemberStatementPdf(MemberStatementResult memberStatement, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create document with same margins as individual statements
                Document document = new Document(PageSize.A4, 30, 30, 50, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // ===== RUBANI SACCO COLOR THEME =====
                BaseColor SkyBlue = new BaseColor(0, 174, 239); // #00AEEF
                BaseColor Red = new BaseColor(255, 0, 0);       // #FF0000
                BaseColor Green = new BaseColor(0, 150, 0);     // #009600
                BaseColor DarkGray = new BaseColor(26, 26, 26); // #1A1A1A
                BaseColor LightGray = new BaseColor(217, 217, 217); // #D9D9D9
                BaseColor MediumGray = new BaseColor(128, 128, 128); // #808080 - Added for section headers
                BaseColor White = BaseColor.WHITE;

                // ===== FONTS - BOOK ANTIQUA FONT WITH SIZE 11 =====
                // Load Book Antiqua font (make sure it's available on your system)
                // You may need to install Book Antiqua font or use a different serif font
                string bookAntiquaFontName = "Book Antiqua";

                // Try to create Book Antiqua fonts, fallback to Times if not available
                Font titleFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 16f, Font.BOLD, DarkGray);
                Font normalFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.NORMAL, DarkGray);
                Font boldFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.BOLD, DarkGray);
                Font smallFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 9f, Font.NORMAL, DarkGray);
                Font companyNameFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 14f, Font.BOLD, SkyBlue);
                Font companyInfoFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.NORMAL, DarkGray);
                Font sectionHeaderFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12f, Font.BOLD, SkyBlue);
                Font tableHeaderFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 9f, Font.BOLD, DarkGray);
                Font tableCellFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 10f, Font.NORMAL, DarkGray);

                // Fallback fonts if Book Antiqua is not available
                try
                {
                    // Test if Book Antiqua is available
                    var testFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f);
                }
                catch
                {
                    // Fallback to Times New Roman if Book Antiqua is not available
                    bookAntiquaFontName = "Times New Roman";
                    titleFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 16f, Font.BOLD, DarkGray);
                    normalFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.NORMAL, DarkGray);
                    boldFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.BOLD, DarkGray);
                    smallFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 9f, Font.NORMAL, DarkGray);
                    companyNameFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 14f, Font.BOLD, SkyBlue);
                    companyInfoFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.NORMAL, DarkGray);
                    sectionHeaderFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12f, Font.BOLD, SkyBlue);
                    tableHeaderFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 9f, Font.BOLD, DarkGray);
                    tableCellFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 10f, Font.NORMAL, DarkGray);
                }

                // ===== CUSTOM HEADER WITH RUBANI SACCO LOGO =====
                try
                {
                    // Create a table with 1 column for left-aligned content
                    PdfPTable headerTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 8f
                    };

                    // Row 1: Logo left-aligned at top
                    PdfPCell logoCell = new PdfPCell();
                    logoCell.Border = Rectangle.NO_BORDER;
                    logoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    logoCell.PaddingBottom = 3f;

                    // Try to load logo from local path
                    string logoPath = @"C:\Users\ADMIN\source\repos\SwiftFinancialsNew\SwiftFinancialsSolution\TestApis\Assets\Images\rubani-logo.jpeg";
                    if (File.Exists(logoPath))
                    {
                        try
                        {
                            Image logo = Image.GetInstance(logoPath);
                            logo.ScaleToFit(100, 100);
                            logoCell.AddElement(logo);
                        }
                        catch (Exception)
                        {
                            logoCell.AddElement(new Paragraph("RUBANI SACCO", companyNameFont)
                            {
                                Alignment = Element.ALIGN_LEFT
                            });
                        }
                    }
                    else
                    {
                        logoCell.AddElement(new Paragraph("RUBANI SACCO", companyNameFont)
                        {
                            Alignment = Element.ALIGN_LEFT
                        });
                    }

                    headerTable.AddCell(logoCell);

                    // Row 2: Company Info - LEFT ALIGNED
                    PdfPCell infoCell = new PdfPCell();
                    infoCell.Border = Rectangle.NO_BORDER;
                    infoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    infoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    infoCell.PaddingTop = 3f;

                    var companyNamePara = new Paragraph("RUBANI SACCO", companyNameFont)
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(companyNamePara);

                    var address = new Paragraph("Rubani House, Off Airport North Embakasi", companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(address);

                    var email = new Paragraph("rubanisacco@gmail.com", companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    infoCell.AddElement(email);

                    headerTable.AddCell(infoCell);
                    document.Add(headerTable);

                    // Add decorative line (Blue-Red-Blue)
                    var lineTable = new PdfPTable(3)
                    {
                        WidthPercentage = 100,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        SpacingAfter = 8f
                    };
                    lineTable.SetWidths(new float[] { 33, 34, 33 });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = Red,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    lineTable.AddCell(new PdfPCell()
                    {
                        BackgroundColor = SkyBlue,
                        FixedHeight = 2f,
                        Border = Rectangle.NO_BORDER
                    });

                    document.Add(lineTable);
                }
                catch (Exception)
                {
                    var fallbackPara = new Paragraph("RUBANI SACCO\nRubani House, Off Airport North Embakasi\nrubanisacco@gmail.com",
                        companyInfoFont)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        SpacingAfter = 10f
                    };
                    document.Add(fallbackPara);
                }

                // ===== MEMBER DETAILED STATEMENT TITLE =====
                string titleText = "MEMBER DETAILED STATEMENT";
                if (startDate.HasValue || endDate.HasValue)
                {
                    titleText = "MEMBER DETAILED STATEMENT";
                    string dateRangeText = "";

                    if (startDate.HasValue && endDate.HasValue)
                        dateRangeText = $"{startDate.Value:dd/MM/yyyy} to {endDate.Value:dd/MM/yyyy}";
                    else if (startDate.HasValue)
                        dateRangeText = $"From {startDate.Value:dd/MM/yyyy}";
                    else if (endDate.HasValue)
                        dateRangeText = $"To {endDate.Value:dd/MM/yyyy}";

                    if (!string.IsNullOrEmpty(dateRangeText))
                    {
                        document.Add(new Paragraph(titleText, titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 3f
                        });

                        document.Add(new Paragraph(dateRangeText,
                            FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12f, Font.BOLD, DarkGray))
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 8f
                        });
                    }
                    else
                    {
                        document.Add(new Paragraph(titleText, titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 8f
                        });
                    }
                }
                else
                {
                    document.Add(new Paragraph(titleText, titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 8f
                    });
                }

                // ===== MEMBER INFORMATION SECTION =====
                if (memberStatement.Customer != null)
                {
                    // Create a 2-column table for better alignment
                    PdfPTable memberInfoTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };
                    memberInfoTable.SetWidths(new float[] { 40, 60 });

                    // Left column: Name, Staff No, Mobile
                    Paragraph leftColumn = new Paragraph();
                    leftColumn.Add(new Chunk("Name: ", boldFont));
                    leftColumn.Add(new Chunk(memberStatement.Customer.FullName, normalFont));
                    leftColumn.Add(Chunk.NEWLINE);
                    leftColumn.Add(new Chunk("Staff No: ", boldFont));
                    leftColumn.Add(new Chunk(memberStatement.Customer.PFNumber ?? "N/A", normalFont));
                    leftColumn.Add(Chunk.NEWLINE);
                    leftColumn.Add(new Chunk("Mobile: ", boldFont));
                    leftColumn.Add(new Chunk(memberStatement.Customer.Mobile ?? "N/A", normalFont));

                    PdfPCell leftCell = new PdfPCell(leftColumn)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Padding = 3
                    };
                    memberInfoTable.AddCell(leftCell);

                    // Right column: MemberNo, Account No, Email
                    Paragraph rightColumn = new Paragraph();
                    rightColumn.Add(new Chunk("MemberNo: ", boldFont));
                    rightColumn.Add(new Chunk(memberStatement.Customer.StaffNo ?? "N/A", normalFont));
                    rightColumn.Add(Chunk.NEWLINE);
                    rightColumn.Add(new Chunk("Account No: ", boldFont));
                    rightColumn.Add(new Chunk(memberStatement.Customer.AccountNumber, normalFont));
                    rightColumn.Add(Chunk.NEWLINE);
                    rightColumn.Add(new Chunk("Email: ", boldFont));
                    rightColumn.Add(new Chunk(memberStatement.Customer.Email ?? "N/A", normalFont));

                    PdfPCell rightCell = new PdfPCell(rightColumn)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Padding = 3
                    };
                    memberInfoTable.AddCell(rightCell);

                    document.Add(memberInfoTable);
                }

                // ===== STATEMENT PERIOD SECTION =====
                if (startDate.HasValue || endDate.HasValue)
                {
                    string periodText = "Statement Period: ";
                    if (startDate.HasValue && endDate.HasValue)
                        periodText += $"{startDate.Value:dd/MM/yyyy} to {endDate.Value:dd/MM/yyyy}";
                    else if (startDate.HasValue)
                        periodText += $"From {startDate.Value:dd/MM/yyyy}";
                    else if (endDate.HasValue)
                        periodText += $"To {endDate.Value:dd/MM/yyyy}";

                    var periodPara = new Paragraph(periodText, boldFont)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        SpacingAfter = 8f
                    };
                    document.Add(periodPara);
                }

                // In the PDF generation method, update the shares section:

                // ===== SHARES/SAVINGS DETAILED SECTION =====
                if (memberStatement.SharesStatements.Count > 0)
                {
                    var sharesHeader = new Paragraph("SHARES/SAVINGS STATEMENT",
                        FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12f, Font.BOLD, White))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 3f
                    };

                    PdfPTable sharesHeaderTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    PdfPCell sharesHeaderCell = new PdfPCell(sharesHeader)
                    {
                        BackgroundColor = MediumGray,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 6,
                        Border = Rectangle.NO_BORDER,
                        BorderWidthBottom = 2f,
                        BorderColorBottom = SkyBlue
                    };
                    sharesHeaderTable.AddCell(sharesHeaderCell);
                    document.Add(sharesHeaderTable);

                    int sharesCounter = 1;
                    foreach (var shares in memberStatement.SharesStatements)
                    {
                        // Account Header - Show product name only
                        var accountHeaderPara = new Paragraph($"ACCOUNT #{sharesCounter}: {shares.ProductName}", sectionHeaderFont)
                        {
                            Alignment = Element.ALIGN_LEFT,
                            SpacingAfter = 8f
                        };
                        document.Add(accountHeaderPara);

                        // Transactions Section - Show ALL transactions (no limit)
                        if (shares.Transactions.Count > 0)
                        {
                            // Use all transactions, not just first 5
                            var allTransactions = shares.Transactions;

                            PdfPTable transTable = new PdfPTable(5)
                            {
                                WidthPercentage = 100,
                                SpacingAfter = 5f
                            };
                            transTable.SetWidths(new float[] { 20, 35, 15, 15, 15 });

                            // Table headers - using tableHeaderFont
                            PdfPCell dateHeaderCell = new PdfPCell(new Phrase("Date", tableHeaderFont))
                            {
                                BackgroundColor = LightGray,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 4,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthTop = 1f,
                                BorderWidthBottom = 1f,
                                BorderWidthLeft = 1f,
                                BorderWidthRight = 0f,
                                BorderColorTop = DarkGray,
                                BorderColorBottom = DarkGray,
                                BorderColorLeft = DarkGray
                            };
                            transTable.AddCell(dateHeaderCell);

                            PdfPCell descHeaderCell = new PdfPCell(new Phrase("Description", tableHeaderFont))
                            {
                                BackgroundColor = LightGray,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 4,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthTop = 1f,
                                BorderWidthBottom = 1f,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderColorTop = DarkGray,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(descHeaderCell);

                            PdfPCell depositHeaderCell = new PdfPCell(new Phrase("Deposit", tableHeaderFont))
                            {
                                BackgroundColor = LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 4,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthTop = 1f,
                                BorderWidthBottom = 1f,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderColorTop = DarkGray,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(depositHeaderCell);

                            // Withdrawal header - kept for consistency but will be empty for shares
                            PdfPCell withdrawalHeaderCell = new PdfPCell(new Phrase("Withdrawal", tableHeaderFont))
                            {
                                BackgroundColor = LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 4,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthTop = 1f,
                                BorderWidthBottom = 1f,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderColorTop = DarkGray,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(withdrawalHeaderCell);

                            PdfPCell balanceHeaderCell = new PdfPCell(new Phrase("Balance", tableHeaderFont))
                            {
                                BackgroundColor = LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 4,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthTop = 1f,
                                BorderWidthBottom = 1f,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 1f,
                                BorderColorTop = DarkGray,
                                BorderColorBottom = DarkGray,
                                BorderColorRight = DarkGray
                            };
                            transTable.AddCell(balanceHeaderCell);

                            // Add ALL transactions - using tableCellFont
                            for (int i = 0; i < allTransactions.Count; i++)
                            {
                                var transaction = allTransactions[i];
                                bool isLastRow = (i == allTransactions.Count - 1);

                                PdfPCell dateCell = new PdfPCell(new Phrase(transaction.TransactionDate, tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 1f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorLeft = DarkGray,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(dateCell);

                                PdfPCell descCell = new PdfPCell(new Phrase(transaction.Description ?? "", tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(descCell);

                                PdfPCell depositCell = new PdfPCell(new Phrase(transaction.DepositAmount.ToString("N2"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(depositCell);

                                // Withdrawal cell - always empty for shares
                                PdfPCell withdrawalCell = new PdfPCell(new Phrase("", tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(withdrawalCell);

                                PdfPCell balanceCell = new PdfPCell(new Phrase(transaction.RunningBalance.ToString("N2"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 1f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorRight = DarkGray,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(balanceCell);
                            }

                            document.Add(transTable);

                        }

                        // Account Summary - Show net movement only
                        if (shares.Summary != null)
                        {
                            var summaryPara = new Paragraph();
                            summaryPara.Alignment = Element.ALIGN_LEFT;

                            string accountTypeName = shares.ProductName ?? shares.AccountType;
                            string totalLabel = accountTypeName.Contains("Share") ?
                                               "Total Share Capital" : $"Total {accountTypeName}";

                            summaryPara.Add(new Chunk($"{totalLabel}: ", boldFont));
                            summaryPara.Add(new Chunk(shares.Summary.NetMovement.ToString("N2"),
                                FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.BOLD, shares.Summary.NetMovement >= 0 ? Green : Red)));

                            summaryPara.SpacingAfter = 8f;
                            document.Add(summaryPara);
                        }

                        sharesCounter++;
                    }
                }
                // ===== LOANS DETAILED SECTION =====
                if (memberStatement.LoanStatements.Count > 0)
                {
                    var loansHeader = new Paragraph("LOANS STATEMENT",
                        FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 12f, Font.BOLD, White))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 3f
                    };

                    PdfPTable loansHeaderTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        SpacingAfter = 10f
                    };

                    PdfPCell loansHeaderCell = new PdfPCell(loansHeader)
                    {
                        BackgroundColor = MediumGray,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 6,
                        Border = Rectangle.NO_BORDER,
                        BorderWidthBottom = 2f,
                        BorderColorBottom = SkyBlue
                    };
                    loansHeaderTable.AddCell(loansHeaderCell);
                    document.Add(loansHeaderTable);

                    int loanCounter = 1;
                    foreach (var loan in memberStatement.LoanStatements)
                    {
                        // Loan Header
                        var loanHeaderPara = new Paragraph($"LOAN #{loanCounter}: {loan.LoanNumber}", sectionHeaderFont)
                        {
                            Alignment = Element.ALIGN_LEFT,
                            SpacingAfter = 3f
                        };
                        document.Add(loanHeaderPara);

                        // Loan Details - 3 column layout with Disbursed Date centered
                        PdfPTable loanDetailsTable = new PdfPTable(3)
                        {
                            WidthPercentage = 100,
                            SpacingAfter = 3f  // Reduced from 5f
                        };
                        loanDetailsTable.SetWidths(new float[] { 33, 34, 33 });

                        // Format the disbursed date
                        string mainDisbursedDateDisplay = "N/A";
                        if (!string.IsNullOrEmpty(loan.LoanDetails.DisbursedDate))
                        {
                            DateTime disbursedDate;
                            if (DateTime.TryParse(loan.LoanDetails.DisbursedDate, out disbursedDate))
                            {
                                mainDisbursedDateDisplay = disbursedDate.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                mainDisbursedDateDisplay = loan.LoanDetails.DisbursedDate;
                            }
                        }

                        // Column 1: Loan Product - Left aligned
                        Paragraph col1Details = new Paragraph();
                        col1Details.Add(new Chunk("Loan Product: ", boldFont));
                        col1Details.Add(new Chunk(loan.LoanDetails.LoanProductType, normalFont));

                        PdfPCell col1Cell = new PdfPCell(col1Details)
                        {
                            Border = Rectangle.NO_BORDER,
                            Padding = 3,
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        loanDetailsTable.AddCell(col1Cell);

                        // Column 2: Disbursed Date - CENTERED
                        Paragraph col2Details = new Paragraph();
                        col2Details.Add(new Chunk("Disbursed Date: ", boldFont));
                        col2Details.Add(new Chunk(mainDisbursedDateDisplay, normalFont));

                        PdfPCell col2Cell = new PdfPCell(col2Details)
                        {
                            Border = Rectangle.NO_BORDER,
                            Padding = 3,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        loanDetailsTable.AddCell(col2Cell);

                        // Column 3: Issued Amount - Right aligned
                        Paragraph col3Details = new Paragraph();
                        col3Details.Add(new Chunk("Issued Amount: ", boldFont));
                        col3Details.Add(new Chunk(loan.LoanDetails.AppliedAmount.ToString("N0"), normalFont));

                        PdfPCell col3Cell = new PdfPCell(col3Details)
                        {
                            Border = Rectangle.NO_BORDER,
                            Padding = 3,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        loanDetailsTable.AddCell(col3Cell);

                        document.Add(loanDetailsTable);

                        // CURRENT OUTSTANDING - Single line, left and right aligned
                        if (loan.Summary != null)
                        {
                            PdfPTable outstandingTable = new PdfPTable(2)
                            {
                                WidthPercentage = 100,
                                SpacingAfter = 5f  // Reduced from 10f
                            };
                            outstandingTable.SetWidths(new float[] { 50, 50 });

                            // Left cell: "CURRENT OUTSTANDING:" label
                            PdfPCell labelCell = new PdfPCell(new Paragraph("CURRENT OUTSTANDING:",
                                FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.BOLD, DarkGray)))
                            {
                                Border = Rectangle.NO_BORDER,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 3,
                                PaddingTop = 0
                            };
                            outstandingTable.AddCell(labelCell);

                            // Right cell: Value
                            PdfPCell valueCell = new PdfPCell(new Paragraph(loan.Summary.TotalOutstandingBalance.ToString("N0"),
                                FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 11f, Font.BOLD, Red)))
                            {
                                Border = Rectangle.NO_BORDER,
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 3,
                                PaddingTop = 0
                            };
                            outstandingTable.AddCell(valueCell);

                            document.Add(outstandingTable);
                        }

                        // Transaction Table
                        PdfPTable transTable = new PdfPTable(6)
                        {
                            WidthPercentage = 100,
                            SpacingAfter = 5f  // Reduced from 10f
                        };
                        transTable.SetWidths(new float[] { 15, 18, 15, 15, 15, 22 });

                        // Table headers - using tableHeaderFont
                        PdfPCell dateHeaderCell = new PdfPCell(new Phrase("Date", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 1f,
                            BorderWidthRight = 0f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray,
                            BorderColorLeft = DarkGray
                        };
                        transTable.AddCell(dateHeaderCell);

                        PdfPCell openingBalanceHeaderCell = new PdfPCell(new Phrase("Opening Balance", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray
                        };
                        transTable.AddCell(openingBalanceHeaderCell);

                        PdfPCell principleHeaderCell = new PdfPCell(new Phrase("Principle", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray
                        };
                        transTable.AddCell(principleHeaderCell);

                        PdfPCell interestHeaderCell = new PdfPCell(new Phrase("Interest", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray
                        };
                        transTable.AddCell(interestHeaderCell);

                        PdfPCell amountHeaderCell = new PdfPCell(new Phrase("Amount", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 0f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray
                        };
                        transTable.AddCell(amountHeaderCell);

                        PdfPCell loanBalanceHeaderCell = new PdfPCell(new Phrase("Loan Balance", tableHeaderFont))
                        {
                            BackgroundColor = LightGray,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Padding = 4,
                            PaddingTop = 6,
                            PaddingBottom = 6,
                            BorderWidthTop = 1f,
                            BorderWidthBottom = 1f,
                            BorderWidthLeft = 0f,
                            BorderWidthRight = 1f,
                            BorderColorTop = DarkGray,
                            BorderColorBottom = DarkGray,
                            BorderColorRight = DarkGray
                        };
                        transTable.AddCell(loanBalanceHeaderCell);

                        // Add transactions if they exist - using tableCellFont
                        if (loan.Statement != null && loan.Statement.Count > 0)
                        {
                            for (int i = 0; i < loan.Statement.Count; i++)
                            {
                                var row = loan.Statement[i];
                                bool isLastRow = (i == loan.Statement.Count - 1);

                                // Format date
                                string transDate = "";
                                if (!string.IsNullOrEmpty(row.TransDate))
                                {
                                    DateTime date;
                                    if (DateTime.TryParse(row.TransDate, out date))
                                        transDate = date.ToString("dd/MM/yyyy");
                                    else
                                        transDate = row.TransDate;
                                }

                                // Date cell
                                PdfPCell dateCell = new PdfPCell(new Phrase(transDate, tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 1f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorLeft = DarkGray,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(dateCell);

                                // Opening Balance cell
                                PdfPCell openingBalanceCell = new PdfPCell(new Phrase(row.OpeningBalance.ToString("N0"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(openingBalanceCell);

                                // Principle cell
                                PdfPCell principleCell = new PdfPCell(new Phrase(row.Principle.ToString("N0"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(principleCell);

                                // Interest cell
                                PdfPCell interestCell = new PdfPCell(new Phrase(row.Interest.ToString("N0"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(interestCell);

                                // Amount cell - with color coding
                                Font amountCellFont = FontFactory.GetFont(bookAntiquaFontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 10f, Font.BOLD, row.Amount > 0 ? Green : DarkGray);
                                PdfPCell amountCell = new PdfPCell(new Phrase(row.Amount.ToString("N0"), amountCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 0f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(amountCell);

                                // Loan Balance cell
                                PdfPCell balanceCell = new PdfPCell(new Phrase(row.LoanBalance.ToString("N0"), tableCellFont))
                                {
                                    HorizontalAlignment = Element.ALIGN_RIGHT,
                                    Padding = 3,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderWidthLeft = 0f,
                                    BorderWidthRight = 1f,
                                    BorderWidthTop = 0f,
                                    BorderWidthBottom = isLastRow ? 1f : 0f,
                                    BorderColorRight = DarkGray,
                                    BorderColorBottom = isLastRow ? DarkGray : BaseColor.WHITE
                                };
                                transTable.AddCell(balanceCell);
                            }

                            //if (loan.Statement.Count > 10)
                            //{
                            //    var moreTransPara = new Paragraph($"... and {loan.Statement.Count - 10} more transactions", smallFont)
                            //    {
                            //        Alignment = Element.ALIGN_CENTER,
                            //        SpacingBefore = 3f,
                            //        SpacingAfter = 3f  // Reduced from 8f
                            //    };
                            //    document.Add(moreTransPara);
                            //}
                        }
                        else
                        {
                            // No transactions - show initial disbursement
                            decimal issuedAmount = loan.LoanDetails.AppliedAmount;

                            // Use a different variable name for the "else" block
                            string noTransactionsDateDisplay = "N/A";
                            if (!string.IsNullOrEmpty(loan.LoanDetails.DisbursedDate))
                            {
                                DateTime noTransactionsDate;
                                if (DateTime.TryParse(loan.LoanDetails.DisbursedDate, out noTransactionsDate))
                                {
                                    noTransactionsDateDisplay = noTransactionsDate.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    noTransactionsDateDisplay = loan.LoanDetails.DisbursedDate;
                                }
                            }

                            // Date cell
                            PdfPCell dateCell = new PdfPCell(new Phrase(noTransactionsDateDisplay, tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 1f,
                                BorderWidthRight = 0f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorLeft = DarkGray,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(dateCell);

                            // Opening Balance cell
                            PdfPCell openingBalanceCell = new PdfPCell(new Phrase(issuedAmount.ToString("N0"), tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(openingBalanceCell);

                            // Principle cell
                            PdfPCell principleCell = new PdfPCell(new Phrase("0", tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(principleCell);

                            // Interest cell
                            PdfPCell interestCell = new PdfPCell(new Phrase("0", tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(interestCell);

                            // Amount cell
                            PdfPCell amountCell = new PdfPCell(new Phrase("0", tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 0f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(amountCell);

                            // Loan Balance cell
                            PdfPCell balanceCell = new PdfPCell(new Phrase(issuedAmount.ToString("N0"), tableCellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_RIGHT,
                                Padding = 3,
                                PaddingTop = 6,
                                PaddingBottom = 6,
                                BorderWidthLeft = 0f,
                                BorderWidthRight = 1f,
                                BorderWidthTop = 0f,
                                BorderWidthBottom = 1f,
                                BorderColorRight = DarkGray,
                                BorderColorBottom = DarkGray
                            };
                            transTable.AddCell(balanceCell);
                        }

                        document.Add(transTable);

                        // Loan Summary - Moved closer to table
                        //if (loan.Summary != null)
                        //{
                        //    // Removed document.Add(new Paragraph("\n")); to eliminate blank line

                        //    var summaryPara = new Paragraph();
                        //    summaryPara.Alignment = Element.ALIGN_LEFT;

                        //    string periodText = "";
                        //    if (startDate.HasValue && endDate.HasValue)
                        //        periodText = $"for period {startDate.Value:dd/MM/yyyy} - {endDate.Value:dd/MM/yyyy}";
                        //    else if (startDate.HasValue)
                        //        periodText = $"from {startDate.Value:dd/MM/yyyy}";
                        //    else if (endDate.HasValue)
                        //        periodText = $"up to {endDate.Value:dd/MM/yyyy}";

                        //    if (!string.IsNullOrEmpty(periodText))
                        //    {
                        //        summaryPara.Add(new Chunk($"Summary {periodText}: ", boldFont));
                        //    }
                        //    else
                        //    {
                        //        summaryPara.Add(new Chunk("Loan Summary: ", boldFont));
                        //    }

                        //    summaryPara.Add(new Chunk($"Principal Paid: {loan.Summary.TotalPrincipalRepaid:N0}", normalFont));
                        //    summaryPara.Add(new Chunk(" | ", normalFont));
                        //    summaryPara.Add(new Chunk($"Interest Accrued: {loan.Summary.TotalInterestAccrued:N0}", normalFont));
                        //    summaryPara.Add(new Chunk(" | ", normalFont));
                        //    summaryPara.Add(new Chunk($"Interest Paid: {loan.Summary.TotalInterestPaid:N0}", normalFont));

                        //    summaryPara.SpacingAfter = 8f;  // Reduced from 15f
                        //    document.Add(summaryPara);
                        //}

                        loanCounter++;
                    }
                }

                // ===== FOOTER =====
                document.Add(new Paragraph("\n"));
                string footerText = $"Statement Generated on: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                if (startDate.HasValue || endDate.HasValue)
                {
                    string dateRangeInfo = "";
                    if (startDate.HasValue && endDate.HasValue)
                        dateRangeInfo = $" | Period: {startDate.Value:dd/MM/yyyy} - {endDate.Value:dd/MM/yyyy}";
                    else if (startDate.HasValue)
                        dateRangeInfo = $" | From: {startDate.Value:dd/MM/yyyy}";
                    else if (endDate.HasValue)
                        dateRangeInfo = $" | Up to: {endDate.Value:dd/MM/yyyy}";

                    footerText += dateRangeInfo;
                }

                footerText += $" | Total Accounts: {memberStatement.TotalAccounts}";

                var footerPara = new Paragraph(footerText, smallFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 8f
                };
                document.Add(footerPara);

                // ===== FOOTER NOTES =====
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("This is a system generated detailed statement for all member accounts.", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                document.Add(new Paragraph("For any queries, contact: rubanisacco@gmail.com", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        // Helper method to create cells with NO BORDERS (not used anymore, but kept for backward compatibility)
        private PdfPCell CreateStyledCell(string text, Font font, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = alignment;
            cell.Padding = 4f;

            // REMOVE ALL BORDERS
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthBottom = 0f;

            return cell;
        }


        public class LoanPaymentRow
        {
            public string TransactionDate { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal Principal { get; set; }
            public decimal Interest { get; set; }
            public decimal Amount { get; set; }
            public decimal LoanBalance { get; set; }
            public string TransactionType { get; set; }
            public string Description { get; set; }
        }

        public class LoanDetails
        {
            public string LoanNumber { get; set; }
            public string LoanProductType { get; set; }
            public decimal AppliedAmount { get; set; }
            public decimal MonthlyRepayment { get; set; }
            public string MemberNumber { get; set; }
            public string DisbursedDate { get; set; }
        }

        public class LoanStatementResult
        {
            public string LoanNumber { get; set; }
            public CustomerInfo Customer { get; set; }
            public LoanDetails LoanDetails { get; set; }
            public List<LoanStatementRow> Statement { get; set; }
            public LoanSummary Summary { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
        public class CustomerData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Reference2 { get; set; }
            public string Reference3 { get; set; }
            public int BranchCode { get; set; }
            public int CustomerSerialNumber { get; set; }
            public int ProductCode { get; set; }
            public int TargetProductCode { get; set; }
        }

        public class CustomerInfo
        {
            public string FullName { get; set; }
            public string AccountNumber { get; set; }
            public string StaffNo { get; set; }
            public string PFNumber { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }



        // Helper method to create cells WITH BORDERS for loan table alignment
        private PdfPCell CreateTableCell(string text, Font font, int alignment = Element.ALIGN_LEFT, BaseColor borderColor = null, bool showBorders = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = alignment;
            cell.Padding = 5f;
            cell.PaddingTop = 8f;
            cell.PaddingBottom = 8f;

            if (showBorders && borderColor != null)
            {
                // Add borders for better alignment
                cell.BorderWidthLeft = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 1f;
                cell.BorderWidthBottom = 1f;
                cell.BorderColor = borderColor;
            }
            else
            {
                // No borders for shares table
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthRight = 0f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 0f;
            }

            return cell;
        }




        #endregion

        public class CustomerShareStatementRow
        {
            public string Date { get; set; }
            public decimal ShareContribution { get; set; }
            public decimal Cumulative { get; set; }
            public string Description { get; set; }
        }

        public class CustomerShareStatementResult
        {
            public List<CustomerShareStatementRow> Statement { get; set; }
            public decimal TotalContribution { get; set; }
        }

        public class LoanStatementRow
        {
            public string PostingDate { get; set; }
            public string TransactionType { get; set; }
            public string DocumentNo { get; set; }
            public string Description { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal Balance { get; set; }
            public decimal Amount { get; set; }

            public string TransDate { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal Principle { get; set; }
            public decimal Interest { get; set; }
            public decimal LoanBalance { get; set; }
        }

        public class LoanSummary
        {
            public decimal TotalDisbursed { get; set; }
            public decimal TotalPrincipalRepaid { get; set; }
            public decimal TotalInterestPaid { get; set; }
            public decimal TotalInterestAccrued { get; set; }
            public decimal OutstandingLoanAmount { get; set; }
            public decimal OutstandingLoanInterest { get; set; }
            public decimal TotalOutstandingBalance { get; set; }
            public decimal OpeningBalance { get; set; }

        }

        public class RecentTransaction
        {
            public string PostingDate { get; set; }
            public string TransactionType { get; set; }
            public string DocumentNo { get; set; }
            public string Description { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal Balance { get; set; }
        }


        public class CreditBatchImportRequest
        {
            public string FileName { get; set; }
        }

        private ServiceHeader CreateServiceHeader()
        {
            return new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                EnvironmentIPAddress = "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
            };
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public object Errors { get; set; }
        }


        public List<System.Web.Mvc.SelectListItem> GetSystemGeneralLedgerAccountCodeSelectList(string selectedValue)
        {
            List<System.Web.Mvc.SelectListItem> systemGeneralLedgerAccountCodes = new List<System.Web.Mvc.SelectListItem>();

            var items = Enum.GetValues(typeof(SystemGeneralLedgerAccountCode)).Cast<SystemGeneralLedgerAccountCode>().Select(v => new System.Web.Mvc.SelectListItem
            {
                Text = GetEnumDescription(v),
                Value = ((int)v).ToString(),
                Selected = ((int)v).ToString() == selectedValue,
            }).ToList();

            systemGeneralLedgerAccountCodes.AddRange(items);

            return systemGeneralLedgerAccountCodes;
        }


        protected static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }



        //select lists


        public List<System.Web.Mvc.SelectListItem> GetPaymentAccountTypeSelectList(string selectedValue)
        {
            List<System.Web.Mvc.SelectListItem> accountTypes = new List<System.Web.Mvc.SelectListItem>();

            var items = Enum.GetValues(typeof(PaymentAccountType)).Cast<PaymentAccountType>().Select(v => new System.Web.Mvc.SelectListItem
            {
                Text = GetEnumDescription(v),
                Value = ((int)v).ToString(),
                Selected = ((int)v).ToString() == selectedValue,
            }).ToList();

            accountTypes.AddRange(items);

            return accountTypes;

        }


        public List<System.Web.Mvc.SelectListItem> GetPaymentDocumentTypeSelectList(string selectedValue)
        {
            List<System.Web.Mvc.SelectListItem> documentTypes = new List<System.Web.Mvc.SelectListItem>();

            var items = Enum.GetValues(typeof(PaymentDocumentType)).Cast<PaymentDocumentType>().Select(v => new System.Web.Mvc.SelectListItem
            {
                Text = GetEnumDescription(v),
                Value = ((int)v).ToString(),
                Selected = ((int)v).ToString() == selectedValue,
            }).ToList();

            documentTypes.AddRange(items);

            return documentTypes;

        }


        public List<System.Web.Mvc.SelectListItem> GetPurchaseInvoiceEntryTypeSelectList(string selectedValue)
        {
            List<System.Web.Mvc.SelectListItem> purchaseInvoiceSelectList = new List<System.Web.Mvc.SelectListItem>();

            var items = Enum.GetValues(typeof(PurchaseInvoiceEntryType)).Cast<PurchaseInvoiceEntryType>().Select(v => new System.Web.Mvc.SelectListItem
            {
                Text = GetEnumDescription(v),
                Value = ((int)v).ToString(),
                Selected = ((int)v).ToString() == selectedValue,
            }).ToList();

            purchaseInvoiceSelectList.AddRange(items);

            return purchaseInvoiceSelectList;
        }





    }
}
