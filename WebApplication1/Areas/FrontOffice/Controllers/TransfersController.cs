using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/frontoffice/transfers")]
    public class TransfersController : ApiController
    {

        private readonly ICashTransferRequestAppService _cashTransferRequestAppService;
        private readonly IExternalChequeAppService _externalChequeAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ITellerAppService _tellerAppService;
        private readonly IFiscalCountAppService _fiscalCountAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public TransfersController(
            ICashTransferRequestAppService cashTransferRequestAppService,
            IExternalChequeAppService externalChequeAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            ITellerAppService tellerAppService,
            IFiscalCountAppService fiscalCountAppService,
            IPostingPeriodAppService postingPeriodAppService
            )
        {
            _cashTransferRequestAppService = cashTransferRequestAppService;
            _externalChequeAppService = externalChequeAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _tellerAppService = tellerAppService;
            _fiscalCountAppService = fiscalCountAppService;
            _postingPeriodAppService = postingPeriodAppService;
        }


        TellerDTO _selectedTeller;
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



        // GET: FrontOffice/Transfers


        [HttpGet]
        [Route("cheques")]
        public async Task<IHttpActionResult> Cheques(Guid? TellerId)
        {
            var model = new CashTransferRequestDTO();

            var serviceHeader = Utils.CreateServiceHeader();

            _selectedTeller = await GetCurrentTeller();

            var untransferredChequesList = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId((Guid)TellerId, "", serviceHeader);

            var untransferredChequesValue = untransferredChequesList.Sum(cheque => cheque.Amount);

            model.EmployeeId = _selectedTeller.EmployeeId;
            model.TotalCredits = _selectedTeller.TotalCredits;
            model.TotalDebits = _selectedTeller.TotalDebits;
            model.BookBalance = _selectedTeller.BookBalance;

            model.OpeningBalance = _selectedTeller.OpeningBalance;
            model.ClosingBalance = _selectedTeller.ClosingBalance;

            model.UntransferredChequesValue = untransferredChequesValue;
            return Ok(model);
        }


        [HttpGet]
        [Route("cash")]
        public async Task<IHttpActionResult> cash()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cashTransferRequests = _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);
                return Ok(cashTransferRequests);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }




        [HttpPost]
        [Route("cash")]
        public async Task<IHttpActionResult> Create(CashTransferRequestDTO cashTransferRequestDTO)
        {

            var selectedTeller = await GetCurrentTeller();

            //get teller cash balance status



            var missingParameters = new List<string>();

            if (selectedTeller == null)
            {
                missingParameters.Add("Teller");
            }

            // Check if any parameter is missing
            if (missingParameters.Any())
            {
                var missingMessage = $"Some features may not work, you are missing {string.Join(", ", missingParameters)}";

                return Json(new { success = false, message = "Operation error: " + missingMessage });
            }

            cashTransferRequestDTO.EmployeeId = selectedTeller.EmployeeId;

            if (!cashTransferRequestDTO.HasErrors)
            {
                var countedTotal = Utils.SumDenominationValues(
                    cashTransferRequestDTO.DenominationOneThousandValue, cashTransferRequestDTO.DenominationFiveHundredValue,
                    cashTransferRequestDTO.DenominationTwoHundredValue, cashTransferRequestDTO.DenominationOneHundredValue,
                    cashTransferRequestDTO.DenominationFiftyValue, cashTransferRequestDTO.DenominationFourtyValue,
                    cashTransferRequestDTO.DenominationTwentyValue, cashTransferRequestDTO.DenominationTenValue,
                    cashTransferRequestDTO.DenominationFiveValue, cashTransferRequestDTO.DenominationOneValue,
                    cashTransferRequestDTO.DenominationFiftyCentValue);

                if (countedTotal != cashTransferRequestDTO.Amount)
                {
                    return Json(new { success = false, message = $"Operation Failed: Counted denominations ({countedTotal}) do not match the transfer amount ({cashTransferRequestDTO.Amount})." });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var successRequest = _cashTransferRequestAppService.AddNewCashTransferRequestAsync(cashTransferRequestDTO, serviceHeader);

                if (successRequest != null)
                {
                    // CashTransferRequest itself has nowhere to persist a denomination
                    // breakdown (no such columns on that aggregate) — record the physical
                    // count as a companion FiscalCount instead, same pattern used by
                    // treasury cash movement and End of Day close.
                    var currentPostingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                    var fiscalCount = new FiscalCountDTO
                    {
                        TransactionCode = (int)SystemTransactionCode.TellerCashTransfer,
                        TransactionType = (int)TreasuryTransactionType.TellerCashTransfer,
                        PostingPeriodId = currentPostingPeriod?.Id ?? Guid.Empty,
                        BranchId = selectedTeller.EmployeeBranchId,
                        ChartOfAccountId = selectedTeller.ChartOfAccountId ?? Guid.Empty,
                        PrimaryDescription = "Cash Transfer Request",
                        SecondaryDescription = selectedTeller.Description,
                        Reference = cashTransferRequestDTO.Reference,
                        TotalValue = cashTransferRequestDTO.Amount,
                        DenominationOneThousandValue = cashTransferRequestDTO.DenominationOneThousandValue,
                        DenominationFiveHundredValue = cashTransferRequestDTO.DenominationFiveHundredValue,
                        DenominationTwoHundredValue = cashTransferRequestDTO.DenominationTwoHundredValue,
                        DenominationOneHundredValue = cashTransferRequestDTO.DenominationOneHundredValue,
                        DenominationFiftyValue = cashTransferRequestDTO.DenominationFiftyValue,
                        DenominationFourtyValue = cashTransferRequestDTO.DenominationFourtyValue,
                        DenominationTwentyValue = cashTransferRequestDTO.DenominationTwentyValue,
                        DenominationTenValue = cashTransferRequestDTO.DenominationTenValue,
                        DenominationFiveValue = cashTransferRequestDTO.DenominationFiveValue,
                        DenominationOneValue = cashTransferRequestDTO.DenominationOneValue,
                        DenominationFiftyCentValue = cashTransferRequestDTO.DenominationFiftyCentValue
                    };

                    _fiscalCountAppService.AddNewFiscalCount(fiscalCount, serviceHeader);

                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Operation Failed" });
                }
            }
            else
            {
                var errorMessages = cashTransferRequestDTO.ErrorMessages;

                return Json(new { success = false, message = errorMessages });
            }
        }


        [HttpPost]
        [Route("cheques")]
        public async Task<IHttpActionResult> TransferSelectedChequesAsync(List<ExternalChequeDTO> cheques)
        {
            if (cheques == null || cheques.Count == 0)
            {
                return Json(new { success = false, message = "No cheques were selected for transfer." });
            }

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                List<ExternalChequeDTO> selectedCheques = new List<ExternalChequeDTO>(cheques);

                _selectedTeller = await GetCurrentTeller();


                var chequesInHandChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesInHand, serviceHeader);

                if (SelectedTeller != null && SelectedTeller.ChartOfAccountId.HasValue && chequesInHandChartOfAccountId != Guid.Empty)
                {

                    var transferred = _externalChequeAppService.TransferExternalCheques(selectedCheques, SelectedTeller, 0, serviceHeader);

                    if (!transferred)
                    {
                        return Json(new { success = false, message = "Transfer failed. Please try again." });
                    }


                    var untransferredCheques = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(SelectedTeller.Id, "", serviceHeader);

                    var untransferredChequesValue = untransferredCheques.Sum(cheque => cheque.Amount);

                    // Construct a JSON response directly
                    var response = new
                    {
                        success = true,
                        message = "Cheques transferred successfully.",
                        data = new
                        {
                            TotalCredits = SelectedTeller.TotalCredits,
                            TotalDebits = SelectedTeller.TotalDebits,
                            BookBalance = SelectedTeller.BookBalance,
                            OpeningBalance = SelectedTeller.OpeningBalance,
                            ClosingBalance = SelectedTeller.ClosingBalance,
                            UntransferredChequesValue = untransferredChequesValue
                        }
                    };

                    return Json(response);

                }

                else
                {

                    var message = "Sorry, but the requisite teller and / or external cheques in hand account has not been setup!";

                    return Json(new { success = false, message = "Operation error: " + message });
                }

            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new { success = false, message = "An error occurred while transferring cheques: " + ex.Message });
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


        [HttpPost]
        [Route("cash/acknowledge")]
        public async Task<IHttpActionResult> AcknowledgeCashTransferRequest(CashTransferRequestDTO cashTransferRequestDTO, int option)
        {

            var selectedTeller = await GetCurrentTeller();

            var missingParameters = new List<string>();

            var serviceHeader = Utils.CreateServiceHeader();


            if (selectedTeller == null)
            {
                missingParameters.Add("Teller");
            }

            // Check if any parameter is missing
            if (missingParameters.Any())
            {
                var missingMessage = $"Some features may not work, you are missing {string.Join(", ", missingParameters)}";

                return Json(new { success = false, message = "Operation error: " + missingMessage });
            }

            cashTransferRequestDTO.EmployeeId = selectedTeller.EmployeeId;

            if (!cashTransferRequestDTO.HasErrors)
            {
                var successRequest = await _cashTransferRequestAppService.AcknowledgeCashTransferRequestAsync(cashTransferRequestDTO, option, serviceHeader);

                if (successRequest)
                {
                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to Acknowledge cash transfer request" });
                }
            }
            else
            {
                var errorMessages = cashTransferRequestDTO.ErrorMessages;

                return Json(new { success = false, message = errorMessages });
            }
        }


        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetCashTransferRequests()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cashTransferRequests = await _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);

                return Ok(cashTransferRequests);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("cash/utilize")]
        public async Task<IHttpActionResult> UtilizeCashTransferRequest(Guid request)
        {

            if (request == Guid.Empty)
            {
                return BadRequest("Request Id is required for this operation");
            }


            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cashTransferRequest = await _cashTransferRequestAppService.FindCashTransferRequestAsync(request, serviceHeader);

                var successRequest = await _cashTransferRequestAppService.UtilizeCashTransferRequestAsync(request, serviceHeader);

                if (successRequest)
                {
                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to mark cash transfer request as Utilized" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex });
            }
        }
    }
}
