using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{

    [Authorize]
    [RoutePrefix("api/frontoffice/cheques")]
    public class ChequesController : ApiController
    {


        private readonly IExternalChequeAppService _externalChequeAppService;


        public ChequesController(IExternalChequeAppService externalChequeAppService)
        {
            _externalChequeAppService = externalChequeAppService;
        }
      
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheques = _externalChequeAppService.FindExternalCheques(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = cheques });
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("bank")]
        //public async Task<IHttpActionResult> BankSelectedCheques(List<Guid> selectedChequeIds, BankLinkageDTO bankLinkageDTO)
        public async Task<IHttpActionResult> BankSelectedCheques([FromBody] ChequeBankingRequest chequeBankingRequest)
        {
            if (chequeBankingRequest == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "A cheque-banking request is required.", data = (object)null });

            var selectedChequeIds = chequeBankingRequest.selectedChequeIds;
            var bankLinkageDTO = chequeBankingRequest.bankLinkageDTO;

            if (selectedChequeIds == null || !selectedChequeIds.Any())
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select at least one cheque to bank.", data = (object)null });
            }

            if (selectedChequeIds.Any(id => id == Guid.Empty) || selectedChequeIds.Distinct().Count() != selectedChequeIds.Count)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Selected cheque identifiers must be valid and unique.", data = (object)null });

            if (bankLinkageDTO == null || bankLinkageDTO.Id == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select the bank linkage where the cheques will be deposited.", data = (object)null });
            }

            if (chequeBankingRequest.ModuleNavigationItemCode <= 0)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "The cheque-banking navigation context is required.", data = (object)null });

            var serviceHeader = Utils.CreateServiceHeader();
            try
            {


                var pageCollectionInfo = _externalChequeAppService.FindUnBankedExternalCheques(string.Empty, 0, int.MaxValue, serviceHeader);

            

                if (pageCollectionInfo == null || !pageCollectionInfo.PageCollection.Any())
                {
                    return Json(new { success = false, message = "No uncleared cheques found." });
                }

                var selectedCheques = pageCollectionInfo.PageCollection
                    .Where(cheque => selectedChequeIds.Contains(cheque.Id))
                    .ToList();

                if (!selectedCheques.Any())
                {
                    return Json(new { success = false, message = "Selected cheques not found in unbanked cheques." });
                }

                if (selectedCheques.Count != selectedChequeIds.Count)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "One or more selected cheques are no longer available for banking. Refresh the list and try again.", data = (object)null });

                //foreach (var cheque in selectedCheques)
                //{
                //    cheque.BankLinkageChartOfAccountId = (Guid)TempData["BankLinkageChartOfAccountId"];
                //    cheque.ChartOfAccountAccountName = TempData["ChartOfAccountAccountName"].ToString();
                //}

                var externalChequeDTOs = new ObservableCollection<ExternalChequeDTO>(selectedCheques.Select(cheque => new ExternalChequeDTO
                {
                    Id = cheque.Id,
                    Number = cheque.Number,
                    Amount = cheque.Amount,
                    BankLinkageChartOfAccountId = cheque.BankLinkageChartOfAccountId,
                    BankLinkageChartOfAccountAccountName = cheque.ChartOfAccountAccountName,
                }).ToList());


                var result = _externalChequeAppService.BankExternalCheques(externalChequeDTOs.ToList(), bankLinkageDTO, chequeBankingRequest.ModuleNavigationItemCode, serviceHeader);
                
                if (!result)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "No cheque was banked. Refresh the list and verify the selected bank linkage.", data = (object)null });
                }
            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.TraceError("Cheque banking failed: {0}", exception);
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Cheque banking could not be completed. No cheque should be retried until its current banking status is refreshed.", data = (object)null });
            }

            return Ok(new { success = true, message = "Cheques banked successfully.", data = (object)null });
        }

      

        [HttpPost]
        [Route("clear")]
        //public async Task<IHttpActionResult> ClearSelectedCheques(List<Guid> selectedChequeIds, int clearingOption, string actionType, UnPayReasonDTO unPayReasonDTO = null)
        public async Task<IHttpActionResult> ClearSelectedCheques(ChequeClearingRequest chequeClearingRequest)
        {
            if (chequeClearingRequest == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "A cheque-clearing request is required.", data = (object)null });

            var selectedChequeIds = chequeClearingRequest.selectedChequeIds;
            var clearingOption = chequeClearingRequest.clearingOption;
            var unPayReasonDTO = chequeClearingRequest.unPayReasonDTO;


            if (selectedChequeIds == null || !selectedChequeIds.Any())
            {
                return Json(new { success = false, message = "No cheques selected." });
            }

            if (clearingOption != 1 && clearingOption != 2)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select either Pay or UnPay.", data = (object)null });

            if (clearingOption == 2 && (unPayReasonDTO == null || unPayReasonDTO.Id == Guid.Empty))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "An unpaid-cheque reason is required.", data = (object)null });

            var serviceHeader = Utils.CreateServiceHeader();

            bool isSuccess = true;
            string errorMessage = string.Empty;

            try
            {
                var pageCollectionInfo = _externalChequeAppService.FindUnClearedExternalCheques(
                    string.Empty,
                    0,
                    int.MaxValue,
                    serviceHeader
                );

                if (pageCollectionInfo == null || !pageCollectionInfo.PageCollection.Any())
                {
                    return Json(new { success = false, message = "No uncleared cheques found." });
                }

                var selectedCheques = pageCollectionInfo.PageCollection
                    .Where(cheque => selectedChequeIds.Contains(cheque.Id))
                    .ToList();

                if (!selectedCheques.Any())
                {
                    return Json(new { success = false, message = "Selected cheques not found in uncleared cheques." });
                }

                foreach (var cheque in selectedCheques)
                {
                    bool chequeProcessed = false;

                    if (clearingOption == 1)
                    {
                        var result = _externalChequeAppService.ClearExternalCheque(
                            cheque,
                            clearingOption,
                            chequeClearingRequest.ModuleNavigationItemCode,
                            null,
                            serviceHeader
                        );

                        if (result)
                            chequeProcessed = true;
                        else
                        {
                            isSuccess = false;
                            errorMessage += $"Failed to clear cheque #{cheque.Number}. ";
                        }
                    }
                    else
                    {
                        var result = _externalChequeAppService.ClearExternalCheque(
                            cheque,
                            clearingOption,
                            chequeClearingRequest.ModuleNavigationItemCode,
                            unPayReasonDTO,
                            serviceHeader
                        );

                        if (result)
                            chequeProcessed = true;
                        else
                        {
                            isSuccess = false;
                            errorMessage += $"Failed to unpay cheque #{cheque.Number}. ";
                        }
                    }

                    if (chequeProcessed)
                    {
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.TraceError("Cheque clearance failed: {0}", exception);
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Cheque clearance could not be completed. Refresh the cheque status before retrying.", data = (object)null });
            }

            return Json(new { success = isSuccess, message = isSuccess ? "Cheques processed successfully." : errorMessage });
        }

        [Route("untransfered")]
        public async Task<IHttpActionResult> GetTellerUntransferredCheques(Guid teller)
        {

            // Guid parseId;

            if (teller == Guid.Empty)
            {
                return BadRequest("teller Id cannot be null");
            }

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheques = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(teller, "", serviceHeader);
                return Ok(new { success = true, message = "", data = cheques });
            }

            catch (Exception)
            {
                throw;
            }

        }
    }


    public class ChequeBankingRequest
    {
        public List<Guid> selectedChequeIds { get; set; } = new List<Guid>();
        public BankLinkageDTO bankLinkageDTO { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }

    public class ChequeClearingRequest
    {
        public List<Guid> selectedChequeIds { get; set; } = new List<Guid>();

        public int clearingOption { get; set; }

        public UnPayReasonDTO unPayReasonDTO { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }

}
