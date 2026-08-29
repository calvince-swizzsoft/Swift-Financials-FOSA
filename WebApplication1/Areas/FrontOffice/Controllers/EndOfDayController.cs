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
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{

    [Authorize]
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

        private readonly IExternalChequeAppService _externalChequeAppService;

        public EndOfDayController (
            ITellerAppService tellerAppService,
            IEmployeeAppService employeeAppService,
            IBranchAppService branchAppService,
            IPostingPeriodAppService postingPeriodAppService,
            ITreasuryAppService treasuryAppService,
            IFiscalCountAppService fiscalCountAppService,
            IJournalAppService journalAppService,
            IExternalChequeAppService externalChequeAppService)
        {

            _tellerAppService = tellerAppService;
            _employeeAppService = employeeAppService;
            _branchAppService = branchAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _treasuryAppService = treasuryAppService;
            _fiscalCountAppService = fiscalCountAppService;
            _journalAppService = journalAppService;
            _externalChequeAppService = externalChequeAppService ?? throw new ArgumentNullException(nameof(externalChequeAppService));
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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CashTransferRequestDTO cashTransferRequestDTO)
        {

            if (cashTransferRequestDTO.HasErrors)
                return BadRequest("Some validations failed - make sure all fields are included");

            var countedTotal = Utils.SumDenominationValues(
                cashTransferRequestDTO.DenominationOneThousandValue, cashTransferRequestDTO.DenominationFiveHundredValue,
                cashTransferRequestDTO.DenominationTwoHundredValue, cashTransferRequestDTO.DenominationOneHundredValue,
                cashTransferRequestDTO.DenominationFiftyValue, cashTransferRequestDTO.DenominationFourtyValue,
                cashTransferRequestDTO.DenominationTwentyValue, cashTransferRequestDTO.DenominationTenValue,
                cashTransferRequestDTO.DenominationFiveValue, cashTransferRequestDTO.DenominationOneValue,
                cashTransferRequestDTO.DenominationFiftyCentValue);

            if (countedTotal != cashTransferRequestDTO.ClosingBalance)
            {
                return BadRequest($"Counted denominations ({countedTotal}) do not match the closing balance ({cashTransferRequestDTO.ClosingBalance}).");
            }

            var serviceHeader = Utils.CreateServiceHeader();

            // Resolve the teller closing out the day from the caller's own JWT identity,
            // not a client-supplied TellerId — otherwise any authenticated caller could
            // close out someone else's till by passing a different id.
            _selectedTeller = await GetCurrentTeller();

            if (_selectedTeller == null)
                return BadRequest("Current user has no linked employee/teller record.");

            cashTransferRequestDTO.TellerId = SelectedTeller.Id;
            cashTransferRequestDTO.EmployeeId = SelectedTeller.EmployeeId;

            // Independently verified server-side rather than trusting the client-supplied
            // UntransferredChequesValue — a teller could otherwise send 0 and bypass the
            // "transfer your cheques first" gate below regardless of reality.
            var untransferredCheques = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(SelectedTeller.Id, string.Empty, serviceHeader);
            _selectedTeller.TellerTotalCheques = untransferredCheques?.Sum(c => c.Amount) ?? 0m;

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
                else if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Shortage)
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
                    if (SelectedTreasury == null)
                    {
                        IsBusy = false;
                        return Json(new { success = false, message = "Operation error: No treasury is configured for the teller's branch." });
                    }

                    var balanceLimitError = _treasuryAppService.ValidateCashMovement(
                        SelectedTreasury.Id,
                        null,
                        model.TotalValue,
                        (int)TreasuryTransactionType.TellerToTreasury,
                        serviceHeader);

                    if (!string.IsNullOrWhiteSpace(balanceLimitError))
                    {
                        IsBusy = false;
                        return Json(new { success = false, message = "Operation error: " + balanceLimitError });
                    }

                    var NewFiscalCount = new FiscalCountDTO();

                    NewFiscalCount.TransactionCode = (int)SystemTransactionCode.TellerEndOfDay;
                    NewFiscalCount.TransactionType = (int)TreasuryTransactionType.TellerToTreasury;
                    NewFiscalCount.PostingPeriodId = model.PostingPeriodId;
                    NewFiscalCount.BranchId = model.BranchId;
                    NewFiscalCount.ChartOfAccountId = model.DebitChartOfAccountId;

                    NewFiscalCount.PrimaryDescription = model.PrimaryDescription;
                    NewFiscalCount.SecondaryDescription = model.SecondaryDescription;
                    NewFiscalCount.Reference = model.Reference;

                    NewFiscalCount.TotalValue = model.TotalValue;

                    NewFiscalCount.DenominationOneThousandValue = cashTransferRequestDTO.DenominationOneThousandValue;
                    NewFiscalCount.DenominationFiveHundredValue = cashTransferRequestDTO.DenominationFiveHundredValue;
                    NewFiscalCount.DenominationTwoHundredValue = cashTransferRequestDTO.DenominationTwoHundredValue;
                    NewFiscalCount.DenominationOneHundredValue = cashTransferRequestDTO.DenominationOneHundredValue;
                    NewFiscalCount.DenominationFiftyValue = cashTransferRequestDTO.DenominationFiftyValue;
                    NewFiscalCount.DenominationFourtyValue = cashTransferRequestDTO.DenominationFourtyValue;
                    NewFiscalCount.DenominationTwentyValue = cashTransferRequestDTO.DenominationTwentyValue;
                    NewFiscalCount.DenominationTenValue = cashTransferRequestDTO.DenominationTenValue;
                    NewFiscalCount.DenominationFiveValue = cashTransferRequestDTO.DenominationFiveValue;
                    NewFiscalCount.DenominationOneValue = cashTransferRequestDTO.DenominationOneValue;
                    NewFiscalCount.DenominationFiftyCentValue = cashTransferRequestDTO.DenominationFiftyCentValue;

                    NewFiscalCount.DestinationBranchId = SelectedTreasury.BranchId;
                    NewFiscalCount.ValidateAll();

                    if (NewFiscalCount.HasErrors)
                    {
                        IsBusy = false;
                        return Json(new { success = false, message = "Operation error: " + NewFiscalCount.ErrorMessages });


                    }
                    // Previously built but never saved — IsEndOfDayExecutedAsync (the
                    // "already closed your day" guard) queries for a FiscalCount row
                    // created today, so without this the guard could never trigger.
                    else if (!_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { NewFiscalCount }, serviceHeader))
                    {
                        IsBusy = false;
                        return Json(new { success = false, message = "Operation error: Failed to record the fiscal count." });
                    }
                    else
                    {

                        proceedEndOfDayTransaction = true;

                        #region proceed with End Of Day Transaction?




                        var cashManagementResult = _journalAppService.AddNewJournal(null, NewFiscalCount.BranchId, null, model.TotalValue, model.PrimaryDescription, model.SecondaryDescription, model.Reference, 0, model.TransactionCode, model.ValueDate, model.CreditChartOfAccountId, model.DebitChartOfAccountId, serviceHeader, true);

                        if (cashManagementResult != null)
                        {
                            var postExcessOrShortage = default(bool);

                            switch ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue)
                            {
                                case TellerCashBalanceStatus.Balanced:
                                    // The teller-to-treasury journal above is the complete
                                    // posting for a balanced till; no variance journal is
                                    // required. Return that journal as the successful receipt.
                                    return Json(new
                                    {
                                        success = true,
                                        message = "Operation Success: End of Day Operation Completed Successfully",
                                        data = cashManagementResult
                                    });
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

                        #endregion
                                // resultJournal carries everything a receipt needs (id, sequential id,
                                // branch/posting-period/user descriptions, amount, reference, date) — the
                                // React client renders/prints its own receipt from this instead of the
                                // server driving a local printer.
                                var response = new
                                {
                                    success = true,
                                    message = "Operation Success: End of Day Operation Completed Successfully",
                                    data = resultJournal
                                };

                                return Json(response);
                            }

                            else
                            {
                                return Json(new { success = false, message = "postExcessOrShortage boolean was false.", data = (object)null });
                            }
                        }

                        else
                        {
                            return Json(new { success = false, message = "Failed to add a cash management journal. ", data = (object)null });
                        }
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }
        }



        private async Task<TellerDTO> GetCurrentTeller()
        {
            var serviceHeader = Utils.CreateServiceHeader();

            var employeeIdClaim = (HttpContext.Current?.User as ClaimsPrincipal)?.FindFirst("EmployeeId");

            if (employeeIdClaim == null || !Guid.TryParse(employeeIdClaim.Value, out var employeeId))
                throw new InvalidOperationException("Current user has no linked employee/teller record.");

            var teller = _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);

            return teller;
        }


    }
}
