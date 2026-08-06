using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("bank")]
        //public async Task<IHttpActionResult> BankSelectedCheques(List<Guid> selectedChequeIds, BankLinkageDTO bankLinkageDTO)
        public async Task<IHttpActionResult> BankSelectedCheques([FromBody] ChequeBankingRequest chequeBankingRequest)
        {
            var selectedChequeIds = chequeBankingRequest.selectedChequeIds;
            var bankLinkageDTO = chequeBankingRequest.bankLinkageDTO;

            if (selectedChequeIds == null || !selectedChequeIds.Any())
            {
                return Json(new { success = false, message = "No cheques selected." });
            }

            if (bankLinkageDTO == null || bankLinkageDTO.Id == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    message = "Bank linkage is required. Please enter the bank linkage details and try again."
                });
            }

            var serviceHeader = Utils.CreateServiceHeader();
            bool isSuccess = true;
            string errorMessage = string.Empty;

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


                var result = _externalChequeAppService.BankExternalCheques(externalChequeDTOs.ToList(), bankLinkageDTO, 123, serviceHeader);
                
                if (!result)
                {
                    isSuccess = false;
                    errorMessage = "Failed to bank the cheques. Ensure valid data and try again.";
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error occurred: {ex.Message}" });
            }

            return Json(new { success = isSuccess, message = isSuccess ? "Cheques processed successfully." : errorMessage });
        }

      

        [HttpPost]
        [Route("clear")]
        //public async Task<IHttpActionResult> ClearSelectedCheques(List<Guid> selectedChequeIds, int clearingOption, string actionType, UnPayReasonDTO unPayReasonDTO = null)
        public async Task<IHttpActionResult> ClearSelectedCheques(ChequeClearingRequest chequeClearingRequest)
        {
            var selectedChequeIds = chequeClearingRequest.selectedChequeIds;
            var clearingOption = chequeClearingRequest.clearingOption;
            var actionType = chequeClearingRequest.actionType;
            var unPayReasonDTO = chequeClearingRequest.unPayReasonDTO;


            if (selectedChequeIds == null || !selectedChequeIds.Any())
            {
                return Json(new { success = false, message = "No cheques selected." });
            }

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

                    if (actionType.ToLower() == "clear")
                    {
                        var result = _externalChequeAppService.ClearExternalCheque(
                            cheque,
                            clearingOption,
                            /* ModuleNavigationItemCode */ 1,
                            null,
                            serviceHeader
                        );

                        if (result)
                        {
                            var markClearedResult = _externalChequeAppService.MarkExternalChequeCleared(cheque.Id, serviceHeader);
                            if (markClearedResult)
                            {
                                chequeProcessed = true;
                            }
                            else
                            {
                                isSuccess = false;
                                errorMessage += $"Failed to mark cheque with ID {cheque.ChequeTypeDescription} as cleared. ";
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage += $"Failed to clear cheque with ID {cheque.ChequeTypeDescription}. ";
                        }
                    }
                    else if (actionType.ToLower() == "unpay")
                    {
                        if (unPayReasonDTO == null)
                        {
                            return Json(new { success = false, message = "UnPay reason is required." });
                        }

                        var result = _externalChequeAppService.ClearExternalCheque(
                            cheque,
                            clearingOption,
                            /* ModuleNavigationItemCode */ 1,
                            unPayReasonDTO,
                            serviceHeader
                        );

                        if (result)
                        {
                            var markClearedResult = _externalChequeAppService.MarkExternalChequeCleared(cheque.Id, serviceHeader);
                            if (markClearedResult)
                            {
                                chequeProcessed = true;
                            }
                            else
                            {
                                isSuccess = false;
                                errorMessage += $"Failed to mark cheque with ID {cheque.ChequeTypeDescription} as cleared. ";
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage += $"Failed to unpay cheque with ID {cheque.ChequeTypeDescription}. ";
                        }
                    }

                    if (chequeProcessed)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error occurred: {ex.Message}" });
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }


    public class ChequeBankingRequest
    {
        public List<Guid> selectedChequeIds { get; set; } = new List<Guid>();
        public BankLinkageDTO bankLinkageDTO { get; set; }
    }

    public class ChequeClearingRequest
    {
        public List<Guid> selectedChequeIds { get; set; } = new List<Guid>();

        public int clearingOption { get; set; }

        public string actionType { get; set; }

        public UnPayReasonDTO unPayReasonDTO { get; set; }
    }

}