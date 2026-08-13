using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.WhatsAppBanking.Controllers
{
    // Bot-facing identity layer for WhatsApp Banking - see WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md
    // §5 for the functional design and docs/api/whatsapp-banking-api-spec.md for the full
    // client spec. [Authorize] here proves the caller is the legitimate bot/orchestrator
    // backend (a dedicated service account JWT), NOT the actual customer - OTP and PIN are the
    // separate, per-customer identity layers on top, exactly as WORKFLOW.md §5 designs them.
    [Authorize]
    [RoutePrefix("api/whatsappbanking")]
    public class IdentityController : ApiController
    {
        private readonly WhatsAppBankingTokenStore _tokenStore;
        private readonly ITextAlertAppService _textAlertAppService;
        private readonly IAlternateChannelAppService _alternateChannelAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public IdentityController(
            WhatsAppBankingTokenStore tokenStore,
            ITextAlertAppService textAlertAppService,
            IAlternateChannelAppService alternateChannelAppService,
            ICustomerAppService customerAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            _textAlertAppService = textAlertAppService ?? throw new ArgumentNullException(nameof(textAlertAppService));
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
        }

        // Used to start EITHER onboarding+linking (new number) OR PIN reset (already-linked
        // number) - the bot decides which flow it's in from otp/verify's response; this
        // endpoint's own response is identical either way (existence isn't revealed
        // pre-verification).
        [HttpPost]
        [Route("otp/request")]
        public async Task<IHttpActionResult> RequestOtp(OtpRequestRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneNumber is required", data = (object)null });

                var otp = _tokenStore.IssueOtp(request.PhoneNumber);

                var serviceHeader = Utils.CreateServiceHeader();

                // AddQuickTextAlert validates the recipient format itself (E.164, "+", >= 13
                // chars) and returns false rather than throwing if every recipient is rejected -
                // its BranchId param is only used for an optional signature append (looked up,
                // never required to exist), so DigitalChannelBranchId is safe to pass even
                // unconfigured (Guid.Empty).
                var sent = _textAlertAppService.AddQuickTextAlert(new QuickTextAlertDTO
                {
                    BranchId = DefaultSettings.Instance.DigitalChannelBranchId,
                    Recipients = request.PhoneNumber,
                    TextMessageBody = string.Format("Your WhatsApp Banking verification code is {0}. It expires in {1} minutes.", otp, _tokenStore.OtpTtlSeconds / 60),
                    AppendSignature = false
                }, serviceHeader);

                if (!sent)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Could not send OTP - check the phone number format (E.164, e.g. +254712345678)", data = (object)null });

                return Ok(new { success = true, message = "OTP sent", data = new { phoneNumber = request.PhoneNumber, expiresInSeconds = _tokenStore.OtpTtlSeconds } });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("otp/verify")]
        public async Task<IHttpActionResult> VerifyOtp(OtpVerifyRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Otp))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneNumber and otp are required", data = (object)null });

                if (!_tokenStore.VerifyAndConsumeOtp(request.PhoneNumber, request.Otp))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Incorrect or expired OTP", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var phoneVerifiedToken = _tokenStore.IssuePhoneVerifiedToken(request.PhoneNumber);

                var customerId = await WhatsAppBankingCustomerLookup.FindExistingCustomerIdAsync(request.PhoneNumber, _alternateChannelAppService, _customerAppService, serviceHeader);

                var whatsAppLinks = _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(request.PhoneNumber, (int)AlternateChannelType.WhatsAppBanking, serviceHeader);

                var hasApprovedLink = whatsAppLinks != null && whatsAppLinks.Any(x => x.RecordStatus == (int)RecordStatus.Approved && !x.IsLocked);

                object customerSummary = null;

                if (customerId.HasValue)
                {
                    var customer = _customerAppService.FindCustomer(customerId.Value, serviceHeader);
                    var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId.Value, serviceHeader) ?? new List<CustomerAccountDTO>();

                    customerSummary = new
                    {
                        id = customer?.Id,
                        firstName = customer?.IndividualFirstName,
                        lastName = customer?.IndividualLastName,
                        accounts = accounts.Select(a => new { id = a.Id, accountNumber = a.FullAccountNumber })
                    };
                }

                return Ok(new
                {
                    success = true,
                    message = "Verified",
                    data = new
                    {
                        phoneVerifiedToken,
                        expiresInSeconds = _tokenStore.PhoneVerifiedTokenTtlSeconds,
                        isExistingCustomer = customerId.HasValue,
                        hasApprovedLink,
                        customer = customerSummary
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Used at the start of every ordinary WhatsApp Banking session, not OTP - see WORKFLOW.md
        // §5 for why (MobilePIN, set during linking, is the ongoing-session credential; OTP is
        // step-up auth for linking/reset only).
        [HttpPost]
        [Route("pin/authenticate")]
        public async Task<IHttpActionResult> AuthenticateWithPin(PinAuthenticateRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Pin))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneNumber and pin are required", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var link = _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(request.PhoneNumber, (int)AlternateChannelType.WhatsAppBanking, serviceHeader)?.FirstOrDefault();

                if (link == null)
                    return NotFound();

                if (link.RecordStatus != (int)RecordStatus.Approved)
                    return Content(HttpStatusCode.Forbidden, new { success = false, message = "This channel link is not yet approved", data = (object)null });

                if (link.IsLocked)
                    return Content(HttpStatusCode.Forbidden, new { success = false, message = "This channel link is locked", data = (object)null });

                if (!_alternateChannelAppService.VerifyMobilePIN(link.Id, request.Pin, serviceHeader))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Incorrect PIN", data = (object)null });

                var sessionToken = _tokenStore.IssueSession(new WhatsAppBankingSession
                {
                    AlternateChannelId = link.Id,
                    CustomerAccountId = link.CustomerAccountId,
                    CustomerId = link.CustomerAccountCustomerId,
                    PhoneNumber = request.PhoneNumber
                });

                return Ok(new { success = true, message = "", data = new { sessionToken, expiresInSeconds = _tokenStore.SessionTtlSeconds } });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Deliberately step-up-authenticated with a fresh OTP, not the old PIN - a customer who
        // forgot their PIN can't prove they still control it.
        [HttpPost]
        [Route("pin/reset")]
        public async Task<IHttpActionResult> ResetPin(PinResetRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneVerifiedToken) || string.IsNullOrWhiteSpace(request.NewPin))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneVerifiedToken and newPin are required", data = (object)null });

                var phoneNumber = _tokenStore.GetPhoneForVerifiedToken(request.PhoneVerifiedToken);

                if (string.IsNullOrEmpty(phoneNumber))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid or expired phoneVerifiedToken", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var link = _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(phoneNumber, (int)AlternateChannelType.WhatsAppBanking, serviceHeader)?.FirstOrDefault();

                if (link == null)
                    return NotFound();

                // PINResetCharges (AlternateChannelKnownChargeType) - whether/how this fee
                // applies here is a pricing decision, not implemented (see
                // docs/api/whatsapp-banking-api-spec.md), flagged rather than guessed at; no
                // established one-line "charge this fee" primitive exists elsewhere in this
                // codebase to safely reuse.
                if (!_alternateChannelAppService.SetMobilePIN(link.Id, request.NewPin, serviceHeader))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Could not reset PIN", data = (object)null });

                var sessionToken = _tokenStore.IssueSession(new WhatsAppBankingSession
                {
                    AlternateChannelId = link.Id,
                    CustomerAccountId = link.CustomerAccountId,
                    CustomerId = link.CustomerAccountCustomerId,
                    PhoneNumber = phoneNumber
                });

                return Ok(new { success = true, message = "", data = new { sessionToken, expiresInSeconds = _tokenStore.SessionTtlSeconds } });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class OtpRequestRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class OtpVerifyRequest
    {
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
    }

    public class PinAuthenticateRequest
    {
        public string PhoneNumber { get; set; }
        public string Pin { get; set; }
    }

    public class PinResetRequest
    {
        public string PhoneVerifiedToken { get; set; }
        public string NewPin { get; set; }
    }
}
