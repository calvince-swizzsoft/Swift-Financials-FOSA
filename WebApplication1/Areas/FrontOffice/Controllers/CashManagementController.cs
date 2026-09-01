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

        private readonly IBranchAppService _branchAppService;

        private readonly IBankLinkageAppService _bankLinkageAppService;

        private readonly ITellerAppService _tellerAppService;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly IFiscalCountAppService _fiscalCountAppService;

        private readonly IJournalAppService _journalAppService;


        public CashManagementController(
            IPostingPeriodAppService postingPeriodAppService,
            ITreasuryAppService treasuryAppService,
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
            if (fiscalCountDTO == null)
                return BadRequest("A cash-management request body is required.");

            if (fiscalCountDTO.BranchId == Guid.Empty)
                return BadRequest("The source branch is required.");

            if (fiscalCountDTO.TotalValue <= 0m)
                return BadRequest("The transaction amount must be greater than zero.");

            if (!string.IsNullOrWhiteSpace(fiscalCountDTO.Reference) && fiscalCountDTO.Reference.Trim().Length > 100)
                return BadRequest("The reference cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(fiscalCountDTO.Reference) && fiscalCountDTO.Reference.Any(char.IsControl))
                return BadRequest("The reference cannot contain control characters.");

            fiscalCountDTO.Reference = (fiscalCountDTO.Reference ?? string.Empty).Trim();

            var denominationError = ValidateDenominations(fiscalCountDTO);
            if (!string.IsNullOrWhiteSpace(denominationError))
                return BadRequest(denominationError);

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

                // TreasuryTransactionType.TellerToTreasury and .TellerCashTransfer are real
                // enum values, but they belong to EndOfDayController and TransfersController
                // respectively (each posts its own FiscalCount with that TransactionType) —
                // this endpoint only ever handles the four treasury-initiated movements below.
                // Without this guard, either of those (or any other unmapped value) fell
                // through both switches untouched and reached the "Operation Success" return
                // at the end with no journal or fiscal count ever posted.
                switch ((TreasuryTransactionType)treasuryTransactionType)
                {
                    case TreasuryTransactionType.BankToTreasury:
                    case TreasuryTransactionType.TreasuryToBank:
                    case TreasuryTransactionType.TreasuryToTeller:
                    case TreasuryTransactionType.TreasuryToTreasury:
                        break;
                    default:
                        return Json(new { success = false, message = "Operation Failed: Unsupported transaction type for treasury cash movement." });
                }

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

                    // FindTreasuryByBranchId doesn't populate BookBalance (Treasury has no
                    // balance column of its own) — without this, every outgoing-transfer
                    // check below (ActiveTreasury.BookBalance < TotalValue) sees 0 and
                    // rejects the transfer as "Insufficient Balance" regardless of the
                    // real GL balance.
                    _treasuryAppService.FetchTreasuryBalances(new List<TreasuryDTO> { ActiveTreasury }, serviceHeader);
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
                    Guid? destinationTreasuryId = null;

                    switch ((TreasuryTransactionType)treasuryTransactionType)
                    {
                        case TreasuryTransactionType.BankToTreasury:
                            if (fiscalCountDTO.Id == Guid.Empty)
                                return BadRequest("The bank linkage is required.");

                         
                            BankLinkageDTO matchingBankLinkage;
                            var bankToTreasuryLinkageError = _bankLinkageAppService.ValidateTreasuryCashMovementLinkage(
                                fiscalCountDTO.Id, ActiveTreasury.BranchId, out matchingBankLinkage, serviceHeader);
                            if (!string.IsNullOrWhiteSpace(bankToTreasuryLinkageError))
                                return Json(new { success = false, message = bankToTreasuryLinkageError });

                            transactionModel.CreditChartOfAccountId = matchingBankLinkage.ChartOfAccountId;
                            transactionModel.DebitChartOfAccountId = ActiveTreasury.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.BankToTreasury;
                            break;

                        case TreasuryTransactionType.TreasuryToTeller:
                            if (fiscalCountDTO.TellerId == Guid.Empty)
                                return BadRequest("The destination teller is required.");

                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            var teller = _tellerAppService.FindTeller(fiscalCountDTO.TellerId, serviceHeader);

                            if (teller == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Teller Not Found" });
                            }

                            if (teller.EmployeeBranchId != ActiveTreasury.BranchId)
                                return BadRequest("The selected teller does not belong to the source treasury branch.");

                            if (!teller.ChartOfAccountId.HasValue || teller.ChartOfAccountId.Value == Guid.Empty)
                                return BadRequest("The selected teller has no configured cash G/L account.");

                            transactionModel.DebitChartOfAccountId = (Guid)teller.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToTeller;

                            var tellerLimitError = _tellerAppService.ValidateCashMovement(
                                teller.Id,
                                transactionModel.TotalValue,
                                true,
                                serviceHeader);

                            if (!string.IsNullOrWhiteSpace(tellerLimitError))
                                return Json(new { success = false, message = tellerLimitError });
                            break;

                        case TreasuryTransactionType.TreasuryToBank:
                            if (fiscalCountDTO.Id == Guid.Empty)
                                return BadRequest("The bank linkage is required.");

                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            BankLinkageDTO linkage;
                            var treasuryToBankLinkageError = _bankLinkageAppService.ValidateTreasuryCashMovementLinkage(
                                fiscalCountDTO.Id, ActiveTreasury.BranchId, out linkage, serviceHeader);
                            if (!string.IsNullOrWhiteSpace(treasuryToBankLinkageError))
                                return Json(new { success = false, message = treasuryToBankLinkageError });
                            transactionModel.DebitChartOfAccountId = linkage.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToBank;

                            break;

                        case TreasuryTransactionType.TreasuryToTreasury:
                            if (fiscalCountDTO.Id == Guid.Empty)
                                return BadRequest("The destination treasury is required.");

                            transactionModel.CreditChartOfAccountId = ActiveTreasury.ChartOfAccountId;

                            var treasury = _treasuryAppService.FindTreasury(fiscalCountDTO.Id, serviceHeader);

                            if (treasury == null)
                            {

                                return Json(new { success = false, message = "Operation Failed: Receiving treasury not found" });
                            }

                            if (treasury.Id == ActiveTreasury.Id)
                                return BadRequest("The source and destination treasury must be different.");

                            // Never trust the request's destination branch. It is determined by
                            // the selected treasury and is used when the destination fiscal count
                            // is persisted.
                            fiscalCountDTO.DestinationBranchId = treasury.BranchId;
                            destinationTreasuryId = treasury.Id;
                            transactionModel.DebitChartOfAccountId = treasury.ChartOfAccountId;
                            transactionModel.TransactionCode = (int)SystemTransactionCode.TreasuryToTreasury;
                            break;
                    }

                    transactionModel.fiscalCountDTO = fiscalCountDTO;

                    var balanceLimitError = _treasuryAppService.ValidateCashMovement(
                        ActiveTreasury.Id,
                        destinationTreasuryId,
                        transactionModel.TotalValue,
                        treasuryTransactionType,
                        serviceHeader);

                    if (!string.IsNullOrWhiteSpace(balanceLimitError))
                        return Json(new { success = false, message = balanceLimitError });

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
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {

                return Json(new { success = false, message = "Operation Failed: There are errors in the form" });
            }
        }

        private static string ValidateDenominations(FiscalCountDTO fiscalCountDTO)
        {
            var denominations = new[]
            {
                new { Name = "1000", Value = fiscalCountDTO.DenominationOneThousandValue, Unit = 1000m },
                new { Name = "500", Value = fiscalCountDTO.DenominationFiveHundredValue, Unit = 500m },
                new { Name = "200", Value = fiscalCountDTO.DenominationTwoHundredValue, Unit = 200m },
                new { Name = "100", Value = fiscalCountDTO.DenominationOneHundredValue, Unit = 100m },
                new { Name = "50", Value = fiscalCountDTO.DenominationFiftyValue, Unit = 50m },
                new { Name = "40", Value = fiscalCountDTO.DenominationFourtyValue, Unit = 40m },
                new { Name = "20", Value = fiscalCountDTO.DenominationTwentyValue, Unit = 20m },
                new { Name = "10", Value = fiscalCountDTO.DenominationTenValue, Unit = 10m },
                new { Name = "5", Value = fiscalCountDTO.DenominationFiveValue, Unit = 5m },
                new { Name = "1", Value = fiscalCountDTO.DenominationOneValue, Unit = 1m },
                new { Name = "50c", Value = fiscalCountDTO.DenominationFiftyCentValue, Unit = 0.5m },
            };

            var negative = denominations.FirstOrDefault(item => item.Value < 0m);
            if (negative != null)
                return string.Format("The {0} denomination subtotal cannot be negative.", negative.Name);

            var invalidMultiple = denominations.FirstOrDefault(item => item.Value % item.Unit != 0m);
            if (invalidMultiple != null)
                return string.Format("The {0} denomination subtotal must represent a whole number of notes or coins.", invalidMultiple.Name);

            return null;
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
                    newFiscalCountDTO.TransactionType = fiscalCountDTO.TransactionType;

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
