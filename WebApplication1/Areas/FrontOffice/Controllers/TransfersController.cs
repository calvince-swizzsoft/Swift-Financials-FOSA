using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
        public async Task<IHttpActionResult> Cheques()
        {
            try
            {
                var model = new CashTransferRequestDTO();

                var serviceHeader = Utils.CreateServiceHeader();

                _selectedTeller = await GetCurrentTeller();

                if (_selectedTeller == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Current teller could not be resolved.", data = (object)null });

            var untransferredChequesList = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(_selectedTeller.Id, "", serviceHeader);

            // FindUnTransferredExternalChequesByTellerId returns null (not an empty list)
            // when a teller has no pending cheques — the normal/clean state — which
            // previously crashed this endpoint with an unhandled ArgumentNullException.
            var untransferredChequesValue = untransferredChequesList?.Sum(cheque => cheque.Amount) ?? 0m;

            model.EmployeeId = _selectedTeller.EmployeeId;
            model.TotalCredits = _selectedTeller.TotalCredits;
            model.TotalDebits = _selectedTeller.TotalDebits;
            model.BookBalance = _selectedTeller.BookBalance;

            model.OpeningBalance = _selectedTeller.OpeningBalance;
            model.ClosingBalance = _selectedTeller.ClosingBalance;

            model.UntransferredChequesValue = untransferredChequesValue;
            model.TransactionType = (int)TreasuryTransactionType.TellerToTreasury;
                return Ok(model);
            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
        }


        [HttpGet]
        [Route("cash")]
        public async Task<IHttpActionResult> cash()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cashTransferRequests = await _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);
                return Ok(cashTransferRequests);
            }

            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
        }




        [HttpPost]
        [Route("cash")]
        public async Task<IHttpActionResult> Create(CashTransferRequestDTO cashTransferRequestDTO)
        {

            try
            {
                var selectedTeller = await GetCurrentTeller();
                if (selectedTeller == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Current teller could not be resolved.", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();
                var created = await _cashTransferRequestAppService.CreateCashTransferAsync(cashTransferRequestDTO, selectedTeller, serviceHeader);
                return Json(new { success = created != null, message = created != null ? "Operation Success" : "Operation Failed", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }


        [HttpPost]
        [Route("cheques")]
        public async Task<IHttpActionResult> TransferSelectedChequesAsync(List<ExternalChequeDTO> cheques)
        {
            if (cheques == null || cheques.Count == 0)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "No cheques were selected for transfer.", data = (object)null });
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

                    // FindUnTransferredExternalChequesByTellerId returns null (not an empty
                    // list) when there are no matches — e.g. right after this transfer clears
                    // out the teller's last pending cheque — which previously crashed the
                    // Sum() call below with an unhandled ArgumentNullException.
                    var untransferredChequesValue = untransferredCheques?.Sum(cheque => cheque.Amount) ?? 0m;

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

                    return Content(HttpStatusCode.BadRequest, new { success = false, message = message, data = (object)null });
                }

            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
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


        [HttpPost]
        [Route("cash/acknowledge")]
        public async Task<IHttpActionResult> AcknowledgeCashTransferRequest(CashTransferRequestDTO cashTransferRequestDTO, int option)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            try
            {
                var successRequest = await _cashTransferRequestAppService.AcknowledgeCashTransferRequestAsync(cashTransferRequestDTO, option, serviceHeader);
                return Json(new { success = successRequest, message = successRequest ? "Operation Success" : "Cash transfer is no longer actionable." });
            }
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("cash/actionable")]
        public async Task<IHttpActionResult> GetActionableCashTransferRequests()
        {
            try
            {
                var requests = await _cashTransferRequestAppService.FindActionableCashTransferRequestsAsync(Utils.CreateServiceHeader());
                return Ok(requests);
            }
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, new { success = false, message = ex.Message });
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

            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
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
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = ex.Message });
            }
        }
    }
}
