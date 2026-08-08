using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{

    [Authorize]

    [RoutePrefix("api/frontoffice/cashmanagement")]
    public class CashManagementController : ApiController
    {

        private readonly IPostingPeriodAppService _postingPeriodAppService;

        private readonly ITreasuryAppService _treasuryAppService;

        private readonly IBankAppService _bankAppService;

        private readonly IBranchAppService _branchAppService;

        private readonly IBankLinkageAppService _bankLinkageAppService;

        private readonly ITellerAppService _tellerAppService;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly IFiscalCountAppService _fiscalCountAppService;

        private readonly IJournalAppService _journalAppService;


        public CashManagementController(
            IPostingPeriodAppService postingPeriodAppService,
            ITreasuryAppService treasuryAppService,
            IBankAppService bankAppService,
            ITellerAppService tellerAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            IFiscalCountAppService fiscalCountAppService,
           IJournalAppService journalAppService,
           IBranchAppService branchAppService,
           IBankLinkageAppService bankLinkageAppService

            )
        {
            _postingPeriodAppService = postingPeriodAppService;
            _treasuryAppService = treasuryAppService;
            _bankAppService = bankAppService;
            _tellerAppService = tellerAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _fiscalCountAppService = fiscalCountAppService;
            _journalAppService = journalAppService;
            _branchAppService = branchAppService;
            _bankLinkageAppService = bankLinkageAppService;
        }






        [HttpPost]

        [Route("")]
        public async Task<IHttpActionResult> Create(FiscalCountDTO fiscalCountDTO)
        {
           // bool IncludeBalance = false;
            fiscalCountDTO.ValidateAll();

            var serviceHeader = Utils.CreateServiceHeader();

            if (!fiscalCountDTO.HasErrors)
            {
                var countedTotal = Utils.SumDenominationValues(
                    fiscalCountDTO.DenominationOneThousandValue, fiscalCountDTO.DenominationFiveHundredValue,
                    fiscalCountDTO.DenominationTwoHundredValue, fiscalCountDTO.DenominationOneHundredValue,
                    fiscalCountDTO.DenominationFiftyValue, fiscalCountDTO.DenominationFourtyValue,
                    fiscalCountDTO.DenominationTwentyValue, fiscalCountDTO.DenominationTenValue,
                    fiscalCountDTO.DenominationFiveValue, fiscalCountDTO.DenominationOneValue,
                    fiscalCountDTO.DenominationFiftyCentValue);

                if (countedTotal != fiscalCountDTO.TotalValue)
                {
                    return Json(new { success = false, message = $"Operation Failed: Counted denominations ({countedTotal}) do not match the total value ({fiscalCountDTO.TotalValue})." });
                }

                int treasuryTransactionType = fiscalCountDTO.TransactionType;
                TransactionModel transactionModel = new TransactionModel();


                var CurrentPostingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                var ActiveTreasury = _treasuryAppService.FindTreasuryByBranchId(fiscalCountDTO.BranchId, serviceHeader);

                var missingParameters = new List<string>();

                if (CurrentPostingPeriod == null)
                {
                    missingParameters.Add("Posting Period");
                }
                else
                {
                    fiscalCountDTO.PostingPeriodId = CurrentPostingPeriod.Id;
                }

                if (ActiveTreasury == null)
                {
                    missingParameters.Add("Treasury");
                }
                else
                {
                    fiscalCountDTO.ChartOfAccountId = ActiveTreasury.ChartOfAccountId;
                    fiscalCountDTO.BranchId = ActiveTreasury.BranchId;
                }

                if (missingParameters.Any())
                {
                    var missingMessage = $"The transaction won't proceed. Unable to retrieve {string.Join(", ", missingParameters)}.";
                    return BadRequest(missingMessage);
                }


                transactionModel.TotalValue = fiscalCountDTO.TotalValue;
                transactionModel.PostingPeriodId = CurrentPostingPeriod.Id;
                transactionModel.PrimaryDescription = fiscalCountDTO.TransactionTypeDescription;
                transactionModel.ValueDate = DateTime.Today;

                try
                {
                    switch ((TreasuryTransactionType)treasuryTransactionType)
                    {
                        case TreasuryTransactionType.BankToTreasury:

                         
                            var sendingBank = _bankAppService.FindBank(fiscalCountDTO.Id, serviceHeader);
                            if (sendingBank == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Sending bank not found" });
                            }

                            var matchingBankLinkage = _bankLinkageAppService.FindBankLinkageByBankAccountId(fiscalCountDTO.Id, serviceHeader);
                           // var matchingBankLinkage = bankLinkages.FirstOrDefault(li => li.BankName == sendingBank.Description);
                            if (matchingBankLinkage == null)
                            {
                                return Json(new { success = false, message = "Operation Failed: No matching bank linkage found for selected bank account" });
                            }

                            transactionModel.CreditChartOfAccountId = matchingBankLinkage.ChartOfAccountId;
                            transactionModel.DebitChartOfAccountId = ActiveTreasury.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.BankToTreasury;
                            break;

                        case TreasuryTransactionType.TreasuryToTeller:
                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            var teller = _tellerAppService.FindTeller(fiscalCountDTO.TellerId, serviceHeader);

                            if (teller == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Teller Not Found" });
                            }
                            transactionModel.DebitChartOfAccountId = (Guid)teller.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToTeller;
                            break;

                        case TreasuryTransactionType.TreasuryToBank:
                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            var receivingBank = _bankAppService.FindBank(fiscalCountDTO.Id, serviceHeader);

                            if (receivingBank == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Receiving bank not found" });
                            }

                            var linkages = _bankLinkageAppService.FindBankLinkages(serviceHeader);
                            var linkage = linkages.FirstOrDefault(l => l.BankName == receivingBank.Description);
                            if (linkage == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: No matching bank linkage found for selected bank account." });

                            }
                            transactionModel.DebitChartOfAccountId = linkage.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToBank;

                            break;

                        case TreasuryTransactionType.TreasuryToTreasury:
                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            var treasury = _treasuryAppService.FindTreasury(fiscalCountDTO.Id, serviceHeader);

                            if (treasury == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Receiving treasury not found" });
                            }
                            transactionModel.DebitChartOfAccountId = treasury.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToTreasury;
                            break;
                    }

                    transactionModel.fiscalCountDTO = fiscalCountDTO;
                    //await ProcessTreasuryTransactionAsync(transactionModel);

                    switch ((TreasuryTransactionType)treasuryTransactionType)
                    {


                        case TreasuryTransactionType.BankToTreasury:


                            //var bankToTreasuryJournal = await _channelService.AddCashManagementJournalAsync(transactionModel.fiscalCountDTO, transactionModel, GetServiceHeader());
                           var bankToTreasuryJournal = DoSomething(transactionModel, transactionModel.fiscalCountDTO, serviceHeader, _branchAppService, _journalAppService);




                            //  var treasuryAccount = await _channelService.FindChartOfAccountAsync(transactionModel.CreditChartOfAccountId, GetServiceHeader());

                            var treasuryAccount = _chartOfAccountAppService.FindChartOfAccount(transactionModel.CreditChartOfAccountId, serviceHeader);

                            var updateTreasuryAccount = _chartOfAccountAppService.UpdateChartOfAccount(treasuryAccount, serviceHeader);

                            //var updateTreasuryAccount = await _channelService.UpdateChartOfAccountAsync(treasuryAccount, GetServiceHeader());

                            if (updateTreasuryAccount)
                            {

                                string message = $"Operation success";

                                return Json(new { success = true, message = message });

                            }

                            break;

                        case TreasuryTransactionType.TreasuryToTeller:

                            if (ActiveTreasury.BookBalance < transactionModel.fiscalCountDTO.TotalValue)
                            {


                                return Json(new { success = false, message = "Operation Failed: Insufficient Balance" });
                            }

                          //  var bankToTellerJournal = await _channelService.AddCashManagementJournalAsync(transactionModel.fiscalCountDTO, transactionModel, GetServiceHeader());

                            var bankToTellerJournal = DoSomething(transactionModel, transactionModel.fiscalCountDTO, serviceHeader, _branchAppService, _journalAppService);


                            var chartOfAccount = _chartOfAccountAppService.FindChartOfAccount(transactionModel.CreditChartOfAccountId, serviceHeader);
                            var updateChartOfAccount = _chartOfAccountAppService.UpdateChartOfAccount(chartOfAccount, serviceHeader);

                            if (updateChartOfAccount)
                            {


                                string message = $"Operation success";

                                return Json(new { success = true, message = message });

                            }


                            break;


                        case TreasuryTransactionType.TreasuryToBank:

                            if (ActiveTreasury.BookBalance < transactionModel.fiscalCountDTO.TotalValue)
                            {



                                return Json(new { success = false, message = "Operation Failed: Insufficient Balance" });
                            }


                           // var treasuryToBankJournal = await _channelService.AddCashManagementJournalAsync(transactionModel.fiscalCountDTO, transactionModel, GetServiceHeader());

                            var treasuryToBankJournal = DoSomething(transactionModel, transactionModel.fiscalCountDTO, serviceHeader, _branchAppService, _journalAppService);

                            var treasuryAcc = _chartOfAccountAppService.FindChartOfAccount(transactionModel.CreditChartOfAccountId, serviceHeader);

                            //var treasuryAcc = await _channelService.FindChartOfAccountAsync(transactionModel.CreditChartOfAccountId, GetServiceHeader());

                            //var updateTreasuryAcc = await _channelService.UpdateChartOfAccountAsync(treasuryAcc, GetServiceHeader());

                            var updateTreasuryAcc = _chartOfAccountAppService.UpdateChartOfAccount(treasuryAcc, serviceHeader);


                            if (updateTreasuryAcc)
                            {

                                string message = $"Operation success";

                                return Json(new { success = true, message = message });


                            }


                            break;


                        case TreasuryTransactionType.TreasuryToTreasury:

                            if (ActiveTreasury.BookBalance < transactionModel.fiscalCountDTO.TotalValue)
                            {


                                return Json(new { success = false, message = "Operation Failed: Insufficient Balance" });
                            }


                            //var treasuryToTreasuryJournal = await _channelService.AddCashManagementJournalAsync(transactionModel.fiscalCountDTO, transactionModel, GetServiceHeader());

                            var treasuryToTreasuryJournal = DoSomething(transactionModel, transactionModel.fiscalCountDTO, serviceHeader, _branchAppService, _journalAppService);

                           // var treasuryAc = await _channelService.FindChartOfAccountAsync(transactionModel.CreditChartOfAccountId, GetServiceHeader());

                            var treasuryAc = _chartOfAccountAppService.FindChartOfAccount(transactionModel.CreditChartOfAccountId, serviceHeader);

                            //var updateTreasuryAc = await _channelService.UpdateChartOfAccountAsync(treasuryAc, GetServiceHeader());

                            var updateTreasuryAc = _chartOfAccountAppService.UpdateChartOfAccount(treasuryAc, serviceHeader);



                            if (updateTreasuryAc)
                            {

                                string message = $"Operation success";

                                return Json(new { success = true, message = message });
                            }



                            break;
                    }

                    return Json(new { success = true, message = "Operation Success: Transaction processed successfully!" });

                }
                catch (Exception ex)
                {

                    return Json(new { success = false, message = "Operation Failed: " + ex.Message });
                }
            }
            else
            {

                return Json(new { success = false, message = "Operation Failed: There are errors in the form" });
            }
        }


        JournalDTO DoSomething(TransactionModel model, FiscalCountDTO fiscalCountDTO, ServiceHeader serviceHeader, IBranchAppService _branchAppService, IJournalAppService _journalAppService)
        {
            JournalDTO journalDTO = null;

            switch ((SystemTransactionCode)model.TransactionCode)
            {
                case SystemTransactionCode.TreasuryToTreasury:

                    var fiscalCountDTOs = new List<FiscalCountDTO>();

                    var sourceBranch = _branchAppService.FindBranch(fiscalCountDTO.BranchId, serviceHeader);

                    var destinationBranch = _branchAppService.FindBranch(fiscalCountDTO.DestinationBranchId, serviceHeader);

                    fiscalCountDTO.PrimaryDescription = string.Format("{0} (Source)", model.PrimaryDescription);
                    fiscalCountDTO.SecondaryDescription = string.Format("To {0}", destinationBranch.Description);

                    fiscalCountDTOs.Add(fiscalCountDTO);

                    var newFiscalCountDTO = new FiscalCountDTO();

                    newFiscalCountDTO.PostingPeriodId = fiscalCountDTO.PostingPeriodId;
                    newFiscalCountDTO.BranchId = fiscalCountDTO.DestinationBranchId;
                    // fiscalCountDTO.ChartOfAccountId is the *source* treasury's G/L account by
                    // this point (set in Create()) — the destination fiscal count needs the
                    // destination treasury's account, which is what got debited.
                    newFiscalCountDTO.ChartOfAccountId = model.DebitChartOfAccountId;
                    newFiscalCountDTO.DestinationBranchId = fiscalCountDTO.DestinationBranchId;
                    newFiscalCountDTO.PrimaryDescription = string.Format("{0} (Destination)", model.PrimaryDescription);
                    newFiscalCountDTO.SecondaryDescription = string.Format("From {0}", sourceBranch.Description);
                    newFiscalCountDTO.Reference = fiscalCountDTO.Reference;
                    newFiscalCountDTO.TransactionCode = fiscalCountDTO.TransactionCode;

                    newFiscalCountDTO.DenominationOneThousandValue = fiscalCountDTO.DenominationOneThousandValue;
                    newFiscalCountDTO.DenominationFiveHundredValue = fiscalCountDTO.DenominationFiveHundredValue;
                    newFiscalCountDTO.DenominationTwoHundredValue = fiscalCountDTO.DenominationTwoHundredValue;
                    newFiscalCountDTO.DenominationOneHundredValue = fiscalCountDTO.DenominationOneHundredValue;
                    newFiscalCountDTO.DenominationFiftyValue = fiscalCountDTO.DenominationFiftyValue;
                    newFiscalCountDTO.DenominationFourtyValue = fiscalCountDTO.DenominationFourtyValue;
                    newFiscalCountDTO.DenominationTwentyValue = fiscalCountDTO.DenominationTwentyValue;
                    newFiscalCountDTO.DenominationTenValue = fiscalCountDTO.DenominationTenValue;
                    newFiscalCountDTO.DenominationFiveValue = fiscalCountDTO.DenominationFiveValue;
                    newFiscalCountDTO.DenominationOneValue = fiscalCountDTO.DenominationOneValue;
                    newFiscalCountDTO.DenominationFiftyCentValue = fiscalCountDTO.DenominationFiftyCentValue;

                    fiscalCountDTOs.Add(newFiscalCountDTO);

                    if (_fiscalCountAppService.AddNewFiscalCounts(fiscalCountDTOs, serviceHeader))
                    {
                        journalDTO = _journalAppService.AddNewJournal(null, sourceBranch.Id, null, model.TotalValue, model.PrimaryDescription, model.SecondaryDescription, model.Reference, 0, model.TransactionCode, model.ValueDate, model.CreditChartOfAccountId, model.DebitChartOfAccountId, serviceHeader, true);
                    }
                    break;
                default:

                    if (_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { fiscalCountDTO }, serviceHeader))
                    {
                        //journalDTO = _journalAppService.AddNewJournal(null, fiscalCountDTO.BranchId, null, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, serviceHeader);
                        journalDTO = _journalAppService.AddNewJournal(null, fiscalCountDTO.BranchId, null, model.TotalValue, model.PrimaryDescription, model.SecondaryDescription, model.Reference, 0, model.TransactionCode, model.ValueDate, model.CreditChartOfAccountId, model.DebitChartOfAccountId, serviceHeader, true);
                    }

                    break;

            }

            return journalDTO;
        }
    }


}