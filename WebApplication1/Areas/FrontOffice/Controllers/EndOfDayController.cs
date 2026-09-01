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
using System.Net;
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
            if (cashTransferRequestDTO == null)
                return BadRequest("An end-of-day request body is required.");

            if (cashTransferRequestDTO.ClosingBalance < 0m)
                return BadRequest("The closing balance cannot be negative.");

            if (!string.IsNullOrWhiteSpace(cashTransferRequestDTO.Reference) && cashTransferRequestDTO.Reference.Trim().Length > 100)
                return BadRequest("The reference cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(cashTransferRequestDTO.Remarks) && cashTransferRequestDTO.Remarks.Trim().Length > 500)
                return BadRequest("Remarks cannot exceed 500 characters.");

            if ((!string.IsNullOrWhiteSpace(cashTransferRequestDTO.Reference) && cashTransferRequestDTO.Reference.Any(char.IsControl))
                || (!string.IsNullOrWhiteSpace(cashTransferRequestDTO.Remarks) && cashTransferRequestDTO.Remarks.Any(char.IsControl)))
                return BadRequest("Reference and remarks cannot contain control characters.");

            cashTransferRequestDTO.Reference = (cashTransferRequestDTO.Reference ?? string.Empty).Trim();
            cashTransferRequestDTO.Remarks = (cashTransferRequestDTO.Remarks ?? string.Empty).Trim();

            var denominationError = ValidateDenominations(cashTransferRequestDTO);
            if (!string.IsNullOrWhiteSpace(denominationError))
                return BadRequest(denominationError);

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

            if (_selectedTeller.IsLocked)
                return BadRequest("The current teller is locked and cannot run end of day.");

            if (!SelectedTeller.EmployeeId.HasValue || SelectedTeller.EmployeeId.Value == Guid.Empty)
                return BadRequest("The current teller has no linked employee record.");

            _tellerAppService.FetchTellerBalances(new List<TellerDTO> { _selectedTeller }, serviceHeader);

            cashTransferRequestDTO.TellerId = SelectedTeller.Id;
            cashTransferRequestDTO.EmployeeId = SelectedTeller.EmployeeId;
            // Book balance and balance status are accounting facts. Never trust values
            // supplied by the browser for either of them.
            cashTransferRequestDTO.BookBalance = SelectedTeller.BookBalance;
            cashTransferRequestDTO.TellerCashBalanceStatusValue = cashTransferRequestDTO.ClosingBalance == SelectedTeller.BookBalance
                ? (int)TellerCashBalanceStatus.Balanced
                : cashTransferRequestDTO.ClosingBalance < SelectedTeller.BookBalance
                    ? (int)TellerCashBalanceStatus.Shortage
                    : (int)TellerCashBalanceStatus.Excess;

            // Independently verified server-side rather than trusting the client-supplied
            // UntransferredChequesValue — a teller could otherwise send 0 and bypass the
            // "transfer your cheques first" gate below regardless of reality.
            var untransferredCheques = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(SelectedTeller.Id, string.Empty, serviceHeader);
            _selectedTeller.TellerTotalCheques = untransferredCheques?.Sum(c => c.Amount) ?? 0m;

            _selectedEmployee = _employeeAppService.FindEmployee(SelectedTeller.EmployeeId.Value, serviceHeader);

            if (_selectedEmployee == null)
                return BadRequest("The teller's linked employee record could not be found.");

            _selectedBranch = _branchAppService.FindBranch(SelectedEmployee.BranchId, serviceHeader);

            if (_selectedBranch == null)
                return BadRequest("The teller's branch could not be found.");

            _selectedPostingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (_selectedPostingPeriod == null)
                return BadRequest("No current posting period is configured.");


            _selectedTreasury = _treasuryAppService.FindTreasuryByBranchId(SelectedBranch.Id, serviceHeader);

            if (_selectedTreasury == null)
                return BadRequest("No treasury is configured for the teller's branch.");

            if (!SelectedTeller.ChartOfAccountId.HasValue || SelectedTeller.ChartOfAccountId.Value == Guid.Empty)
                return BadRequest("The teller has no configured cash G/L account.");

            if (SelectedTreasury.ChartOfAccountId == Guid.Empty)
                return BadRequest("The branch treasury has no configured cash G/L account.");

            if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Shortage
                && (!SelectedTeller.ShortageChartOfAccountId.HasValue || SelectedTeller.ShortageChartOfAccountId.Value == Guid.Empty))
                return BadRequest("The teller has no configured shortage G/L account.");

            if ((TellerCashBalanceStatus)cashTransferRequestDTO.TellerCashBalanceStatusValue == TellerCashBalanceStatus.Excess
                && (!SelectedTeller.ExcessChartOfAccountId.HasValue || SelectedTeller.ExcessChartOfAccountId.Value == Guid.Empty))
                return BadRequest("The teller has no configured excess G/L account.");

            try
            {

                var model = new TransactionModel();

                IsBusy = true;

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

                var authorityError = _journalAppService.ValidateTransactionAuthority(
                    model.TotalValue,
                    model.TransactionCode,
                    serviceHeader);

                if (!string.IsNullOrWhiteSpace(authorityError))
                {
                    IsBusy = false;
                    return Content(HttpStatusCode.Forbidden, new
                    {
                        success = false,
                        message = authorityError,
                        data = (object)null
                    });
                }

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
                    else
                    {

                        #region proceed with End Of Day Transaction?
                        // A zero-cash balanced till is a valid close. There is no monetary
                        // journal to post, but the fiscal-count marker is still required to
                        // prevent the teller from closing the same business date twice.
                        if (model.TotalValue == 0m)
                        {
                            if (!_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { NewFiscalCount }, serviceHeader))
                                return Json(new { success = false, message = "Operation error: Failed to record the fiscal count." });

                            return Json(new
                            {
                                success = true,
                                message = "Operation Success: End of Day completed with a zero cash balance.",
                                data = (object)null
                            });
                        }

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
                                    if (!_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { NewFiscalCount }, serviceHeader))
                                        return Json(new { success = false, message = "Operation error: Cash was posted, but the fiscal close marker could not be recorded. Do not retry; escalate for reconciliation." });

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

                                if (resultJournal == null)
                                    return Json(new { success = false, message = "Operation error: Cash was transferred to treasury, but the shortage/excess journal failed. Do not retry; escalate for reconciliation." });

                                // Only mark the teller closed after every required journal has
                                // posted. The old ordering wrote this marker first, which could
                                // leave a teller permanently closed despite a failed posting.
                                if (!_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { NewFiscalCount }, serviceHeader))
                                    return Json(new { success = false, message = "Operation error: Journals posted, but the fiscal close marker could not be recorded. Do not retry; escalate for reconciliation." });

                        #endregion
                                // resultJournal carries everything a receipt needs (id, sequential id,
                                // branch/posting-period/user descriptions, amount, reference, date) — the
                                // React client renders/prints its own receipt from this instead of the
                                // server driving a local printer.
                                var response = new
                                {
                                    success = true,
                                    message = "Operation Success: End of Day Operation Completed Successfully",
                                    data = cashManagementResult,
                                    varianceJournal = resultJournal
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

        private static string ValidateDenominations(CashTransferRequestDTO request)
        {
            var denominations = new[]
            {
                new { Name = "1000", Value = request.DenominationOneThousandValue, Unit = 1000m },
                new { Name = "500", Value = request.DenominationFiveHundredValue, Unit = 500m },
                new { Name = "200", Value = request.DenominationTwoHundredValue, Unit = 200m },
                new { Name = "100", Value = request.DenominationOneHundredValue, Unit = 100m },
                new { Name = "50", Value = request.DenominationFiftyValue, Unit = 50m },
                new { Name = "40", Value = request.DenominationFourtyValue, Unit = 40m },
                new { Name = "20", Value = request.DenominationTwentyValue, Unit = 20m },
                new { Name = "10", Value = request.DenominationTenValue, Unit = 10m },
                new { Name = "5", Value = request.DenominationFiveValue, Unit = 5m },
                new { Name = "1", Value = request.DenominationOneValue, Unit = 1m },
                new { Name = "50c", Value = request.DenominationFiftyCentValue, Unit = 0.5m },
            };

            var negative = denominations.FirstOrDefault(item => item.Value < 0m);
            if (negative != null)
                return string.Format("The {0} denomination subtotal cannot be negative.", negative.Name);

            var invalidMultiple = denominations.FirstOrDefault(item => item.Value % item.Unit != 0m);
            if (invalidMultiple != null)
                return string.Format("The {0} denomination subtotal must represent a whole number of notes or coins.", invalidMultiple.Name);

            return null;
        }


    }
}
