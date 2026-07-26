using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/frontoffice/endofday")]
    public class EndOfDayController : ApiController
    {
        private readonly ITellerAppService _tellerAppService;

        private readonly IEmployeeAppService _employeeAppService;

        private readonly IBranchAppService _branchAppService;

        private readonly IPostingPeriodAppService _postingPeriodAppService;

        private readonly ITreasuryAppService _treasuryAppService;

        private readonly ICashTransferRequestAppService _cashTransferRequestAppService;

        private readonly IFiscalCountAppService _fiscalCountAppService;

        private readonly IJournalAppService _journalAppService;

        public EndOfDayController (
            ITellerAppService tellerAppService,
            IEmployeeAppService employeeAppService,
            IBranchAppService branchAppService,
            IPostingPeriodAppService postingPeriodAppService,
            ITreasuryAppService treasuryAppService,
            IFiscalCountAppService fiscalCountAppService,
            IJournalAppService journalAppService)
        {

            _tellerAppService = tellerAppService;
            _employeeAppService = employeeAppService;
            _branchAppService = branchAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _treasuryAppService = treasuryAppService;
            _fiscalCountAppService = fiscalCountAppService;
            _journalAppService = journalAppService;
        }


        TellerDTO _selectedTeller;

        TreasuryDTO _selectedTreasury;

        PostingPeriodDTO _selectedPostingPeriod;

        BranchDTO _selectedBranch;

        EmployeeDTO _selectedEmployee;

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

        public PostingPeriodDTO SelectedPostingPeriod
        {
            get { return _selectedPostingPeriod; }

            set
            {
                if (_selectedPostingPeriod != value)
                {
                    _selectedPostingPeriod = value;
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

        public TreasuryDTO SelectedTreasury
        {
            get { return _selectedTreasury; }
            set
            {
                if (_selectedTreasury != value)
                {
                    _selectedTreasury = value;

                }


            }
        }

        private bool IsBusy { get; set; } // Property to indicate if an operation is in progress

        private string receiptContent;

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CashTransferRequestDTO cashTransferRequestDTO)
        {

            if (cashTransferRequestDTO.HasErrors)
                return BadRequest("Some validations failed - make sure all fields are included");


            var serviceHeader = new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                EnvironmentIPAddress = "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
            };

            _selectedTeller = _tellerAppService.FindTeller(cashTransferRequestDTO.TellerId, serviceHeader);
            
            cashTransferRequestDTO.EmployeeId = SelectedTeller.EmployeeId;
            _selectedTeller.TellerTotalCheques = cashTransferRequestDTO.UntransferredChequesValue;

            _selectedEmployee = _employeeAppService.FindEmployee((Guid)SelectedTeller.EmployeeId, serviceHeader);

            _selectedBranch = _branchAppService.FindBranch(SelectedEmployee.BranchId, serviceHeader);

            _selectedPostingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
            
 
            _selectedTreasury = _treasuryAppService.FindTreasuryByBranchId(SelectedBranch.Id, serviceHeader);

            try
            {

                var model = new TransactionModel();

                IsBusy = true;

                var proceedEndOfDayTransaction = default(bool);

                model.TransactionCode = (int)SystemTransactionCode.TellerEndOfDay;

                model.PrimaryDescription = SelectedTeller.Description;

                if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Balanced)
                    model.Reference = TellerCashBalanceStatus.Balanced.ToString();
                else if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Balanced)
                    model.Reference = TellerCashBalanceStatus.Shortage.ToString();
                else if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Excess)
                    model.Reference = TellerCashBalanceStatus.Excess.ToString();


                if (SelectedTeller != null && !SelectedTeller.IsLocked)
                    model.SecondaryDescription = SelectedTeller.Description;

                if (SelectedPostingPeriod != null)
                    model.PostingPeriodId = SelectedPostingPeriod.Id;

                if (SelectedBranch != null)
                    model.BranchId = SelectedBranch.Id;

                if (SelectedTreasury != null)
                {
                    model.DebitChartOfAccountId = SelectedTreasury.ChartOfAccountId;
                    model.ChartOfAccountId = SelectedTreasury.ChartOfAccountId;
                }

                if (SelectedTeller != null && !SelectedTeller.IsLocked)
                {
                    model.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;
                    model.ContraChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;
                }

                model.TotalValue = cashTransferRequestDTO.ClosingBalance;
                model.ValidateAll();

                if (model.HasErrors)
                {
                    IsBusy = false;

                    string errorMessages = string.Join(Environment.NewLine, model.ErrorMessages);

                    return Json(new { success = false, message = "Operation error: " + errorMessages });
                }
                else if (SelectedTeller.TellerTotalCheques != 0m)
                {
                    IsBusy = false;

                    return Json(new { success = false, message = "Operation error: " + "Sorry, but you need to first transfer your cheques!" });
                }
                else if (await _fiscalCountAppService.IsEndOfDayExecutedAsync(SelectedEmployee, serviceHeader))
                {
                    IsBusy = false;
                    return Json(new { success = false, message = "Operation error: " + "Sorry, but you have already closed your day!" });


                }
                else
                {
                    var NewFiscalCount = new FiscalCountDTO();

                    NewFiscalCount.TransactionCode = (int)SystemTransactionCode.TellerEndOfDay;
                    NewFiscalCount.PostingPeriodId = model.PostingPeriodId;
                    NewFiscalCount.BranchId = model.BranchId;
                    NewFiscalCount.ChartOfAccountId = model.DebitChartOfAccountId;

                    NewFiscalCount.PrimaryDescription = model.PrimaryDescription;
                    NewFiscalCount.SecondaryDescription = model.SecondaryDescription;
                    NewFiscalCount.Reference = model.Reference;

                    NewFiscalCount.TotalValue = model.TotalValue;


                    //NewFiscalCount.DestinationBranchId = Guid.NewGuid(); /*for passing validation*/

                    NewFiscalCount.DestinationBranchId = SelectedTreasury.BranchId;
                    NewFiscalCount.ValidateAll();

                    if (NewFiscalCount.HasErrors)
                    {
                        IsBusy = false;
                        return Json(new { success = false, message = "Operation error: " + NewFiscalCount.ErrorMessages });


                    }
                    else
                    {

                        proceedEndOfDayTransaction = true;

                        #region proceed with End Of Day Transaction?




                        var cashManagementResult = _journalAppService.AddNewJournal(null, NewFiscalCount.BranchId, null, model.TotalValue, model.PrimaryDescription, model.SecondaryDescription, model.Reference, 0, model.TransactionCode, model.ValueDate, model.CreditChartOfAccountId, model.DebitChartOfAccountId, serviceHeader, true);


                        //  var cashManagementResult = await _channelService.AddCashManagementJournalAsync(NewFiscalCount, model, GetServiceHeader());

                        if (cashManagementResult != null)
                        {
                            var postExcessOrShortage = default(bool);

                            switch ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue)
                            {
                                case TellerCashBalanceStatus.Balanced:
                                    break;
                                case TellerCashBalanceStatus.Shortage:
                                    model.TotalValue = cashTransferRequestDTO.BookBalance - cashTransferRequestDTO.ClosingBalance;
                                    model.CreditChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;
                                    model.DebitChartOfAccountId = SelectedTeller.ShortageChartOfAccountId ?? Guid.Empty;

                                    postExcessOrShortage = true;

                                    break;
                                case TellerCashBalanceStatus.Excess:
                                    model.TotalValue = cashTransferRequestDTO.ClosingBalance - cashTransferRequestDTO.BookBalance;
                                    model.CreditChartOfAccountId = SelectedTeller.ExcessChartOfAccountId ?? Guid.Empty;
                                    model.DebitChartOfAccountId = SelectedTeller.ChartOfAccountId ?? Guid.Empty;

                                    postExcessOrShortage = true;

                                    break;
                                default:
                                    break;
                            }

                            if (postExcessOrShortage)
                            {
                                model.PrimaryDescription = string.Format("{0}-{1}", "Transaction", EnumHelper.GetDescription((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue));


                                var resultJournal = _journalAppService.AddNewJournal(null, NewFiscalCount.BranchId, null, model.TotalValue, model.PrimaryDescription, model.SecondaryDescription, model.Reference, 0, model.TransactionCode, model.ValueDate, model.CreditChartOfAccountId, model.DebitChartOfAccountId, serviceHeader, true);

                               // var resultJournal = await _channelService.AddJournalAsync(model, null, GetServiceHeader());


                        #endregion
                                var response = new
                                {

                                    success = true,


                                    message = "Operation Success:" + "End of Day Operation Completed Successfully",

                                    journalId = resultJournal.Id,
                                    journalSequentialId = resultJournal.SequentialId,
                                    journalBranchDescription = resultJournal.BranchDescription,
                                    journalPrimaryDescription = resultJournal.PrimaryDescription,
                                    journalSecondaryDescription = resultJournal.SecondaryDescription,
                                    journalPostingPeriodDescription = resultJournal.PostingPeriodDescription,
                                    journalApplicationUserName = resultJournal.ApplicationUserName,
                                    journalCreatedDate = resultJournal.CreatedDate,
                                    journalTotalValue = resultJournal.TotalValue,
                                    journalReference = resultJournal.Reference
                                };

                                return Json(response);
                            }

                            else
                            {
                                return Json(new { success = false, message = "postExcessOrShortage boolean was false." });
                            }
                        }

                        else
                        {
                            return Json(new { success = false, message = "Failed to add a cash management journal. " });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation error: " + ex.Message });
            }
        }



        private async Task<TellerDTO> GetCurrentTeller()
        {

            var serviceHeader = new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                EnvironmentIPAddress = "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
            };

            // Get the current user
            //var user = await _applicationUserManager.FindByIdAsync(User.Identity.GetUserId());

            var teller =  _tellerAppService.FindTellerByEmployeeId(Guid.Parse("50BDE4A6-1F50-F111-9B87-C8E2651EF92A"), serviceHeader);
           
            return teller;

        }


        [HttpPost]
        public IHttpActionResult PrintReceipt(JournalDTO journal)
        {
            try
            {
                var printerName = ConfigurationManager.AppSettings["ReceiptPrinterName"];

                if (string.IsNullOrWhiteSpace(printerName))
                    return BadRequest("Printer name is not configured.");

                var receiptContent = BuildReceiptContent(journal);

                using (var printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings = new PrinterSettings
                    {
                        PrinterName = printerName
                    };

                    printDocument.PrintPage += (sender, e) =>
                    {
                        e.Graphics.DrawString(
                            receiptContent,
                            new Font("Courier New", 10),
                            Brushes.Black,
                            new RectangleF(0, 0, e.PageBounds.Width, e.PageBounds.Height)
                        );
                    };

                    printDocument.Print();
                }

                return Ok(new { success = true, message = "Receipt printed successfully." });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // Helper method to build the receipt content
        private string BuildReceiptContent(JournalDTO journal)
        {
            var builder = new StringBuilder();

            // Add headers
            builder.AppendLine("===== Transaction Receipt =====");
            builder.AppendLine($"Transaction ID: {journal.Id}");
            builder.AppendLine($"Sequential ID: {journal.SequentialId}");
            builder.AppendLine($"Branch: {journal.BranchDescription}");
            builder.AppendLine($"Posting Period: {journal.PostingPeriodDescription}");
            builder.AppendLine($"Total Value: {journal.TotalValue:C}"); // Format as currency
            builder.AppendLine($"Primary Description: {journal.PrimaryDescription}");
            builder.AppendLine($"Secondary Description: {journal.SecondaryDescription}");
            builder.AppendLine($"Reference: {journal.Reference}");

            //this format cld have issue
            builder.AppendLine($"Transaction Date: {journal.CreatedDate:yyyy-MM-dd HH:mm:ss}");

            // Add environment details
            builder.AppendLine("\n===== Environment Details =====");
            builder.AppendLine($"User: {journal.ApplicationUserName}");
            //builder.AppendLine($"Machine Name: {journal.EnvironmentMachineName}");
            //builder.AppendLine($"IP Address: {journal.EnvironmentIPAddress}");

            // Add a footer
            builder.AppendLine("\n===============================");
            builder.AppendLine("Thank you for using our services!");

            return builder.ToString();
        }



    }
}