using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.WhatsAppBanking.Controllers
{
    // The single highest-leverage missing piece flagged throughout WORKFLOW.md/docs/api/whatsapp-banking-api-spec.md:
    // an inbound REST endpoint a mobile money provider can actually call with a live C2B payment
    // confirmation. IMobileToBankRequestAppService.AddNewMobileToBankRequest already does real,
    // working GL posting (confirmed by reading it) and already matches by MSISDN against an
    // AlternateChannel link when MatchByMSISDN is set - nothing external could reach it before
    // this controller, only a legacy WCF passthrough no real provider posts to. This is the
    // deposit story not just for WhatsApp Banking but for every channel that wants C2B deposits.
    //
    // Deliberately NOT [Authorize] - a payment provider's server-to-server callback can't
    // participate in this system's staff/service JWT bearer scheme. Authenticated instead via a
    // shared secret header, configured by ops per DefaultSettings.Instance.MobileToBankWebhookSecret.
    // An unconfigured secret means every request is refused, not "no secret required" -
    // deliberately fails closed.
    [RoutePrefix("api/whatsappbanking/webhooks")]
    public class DepositWebhookController : ApiController
    {
        private readonly IMobileToBankRequestAppService _mobileToBankRequestAppService;

        public DepositWebhookController(IMobileToBankRequestAppService mobileToBankRequestAppService)
        {
            _mobileToBankRequestAppService = mobileToBankRequestAppService ?? throw new ArgumentNullException(nameof(mobileToBankRequestAppService));
        }

        [HttpPost]
        [Route("c2b-confirmation")]
        public async Task<IHttpActionResult> C2BConfirmation(C2BConfirmationRequest request)
        {
            try
            {
                var configuredSecret = DefaultSettings.Instance.MobileToBankWebhookSecret;

                if (string.IsNullOrEmpty(configuredSecret))
                    return InternalServerError(new InvalidOperationException("Inbound C2B webhook secret is not configured (DefaultSettings.Instance.MobileToBankWebhookSecret) - refusing every request until it is set."));

                if (!Request.Headers.TryGetValues("X-Webhook-Secret", out var secretValues) || !string.Equals(secretValues.FirstOrDefault(), configuredSecret, StringComparison.Ordinal))
                    return Content(HttpStatusCode.Unauthorized, new { success = false, message = "Invalid webhook secret", data = (object)null });

                if (request == null || string.IsNullOrWhiteSpace(request.MSISDN) || string.IsNullOrWhiteSpace(request.TransID) || request.TransAmount <= 0m)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "MSISDN, TransID and a positive TransAmount are required", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var mobileToBankRequestDTO = new MobileToBankRequestDTO
                {
                    MSISDN = request.MSISDN,
                    BusinessShortCode = request.BusinessShortCode,
                    TransID = request.TransID,
                    BillRefNumber = request.BillRefNumber,
                    TransAmount = request.TransAmount,
                    TransTime = request.TransTime,
                    OrgAccountBalance = request.OrgAccountBalance,
                    ThirdPartyTransID = request.ThirdPartyTransID,
                    InvoiceNumber = request.InvoiceNumber,
                    KYCInfo = request.KYCInfo,
                    Remarks = request.Remarks,
                    // The whole point of this webhook for WhatsApp Banking (and every other
                    // phone-identified channel): match the payer's MSISDN against the
                    // AlternateChannel link, not an encoded BillRefNumber account reference.
                    // MatchByMSISDN defaults false on the DTO - must be set explicitly, easy to
                    // silently get wrong (confirmed by reading MobileToBankRequestAppService.MatchCustomerAccount).
                    MatchByMSISDN = true
                };

                var result = _mobileToBankRequestAppService.AddNewMobileToBankRequest(mobileToBankRequestDTO, serviceHeader);

                if (result == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Could not record the confirmation", data = (object)null });

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = result.Status == (int)MobileToBankRequestStatus.AutoMatched ? "Matched and posted" : "Recorded, unmatched - queued for manual back-office reconciliation",
                    data = new { id = result.Id, status = result.StatusDescription }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class C2BConfirmationRequest
    {
        public string MSISDN { get; set; }
        public string BusinessShortCode { get; set; }
        public string TransID { get; set; }
        public string BillRefNumber { get; set; }
        public decimal TransAmount { get; set; }
        public string TransTime { get; set; }
        public decimal OrgAccountBalance { get; set; }
        public string ThirdPartyTransID { get; set; }
        public string InvoiceNumber { get; set; }
        public string KYCInfo { get; set; }
        public string Remarks { get; set; }
    }
}
