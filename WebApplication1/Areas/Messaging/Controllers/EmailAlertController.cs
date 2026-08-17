using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Messaging.Controllers
{
    // Faithful REST adaptation of the useful behavior split between the
    // reference MVC Messaging/EmailAlert and Dashboard/EmailAlerts
    // controllers: compose an alert, inspect one alert, and monitor the
    // delivery log with status/text/date filters. SMTP delivery remains the
    // responsibility of the MSMQ-backed EmailAlertDispatcher Windows plugin.
    [Authorize]
    [RoutePrefix("api/messaging/emailalerts")]
    public class EmailAlertController : ApiController
    {
        private const int MaximumPageSize = 1000;
        private readonly IEmailAlertAppService _emailAlertAppService;

        public EmailAlertController(IEmailAlertAppService emailAlertAppService)
        {
            _emailAlertAppService = emailAlertAppService ?? throw new ArgumentNullException(nameof(emailAlertAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(
            int dlrStatus = (int)DLRStatus.Delivered,
            string text = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageIndex = 0,
            int pageSize = 20)
        {
            if (!Enum.IsDefined(typeof(DLRStatus), dlrStatus))
                return ErrorResponse(HttpStatusCode.BadRequest, "Invalid delivery status");
            if (pageIndex < 0)
                return ErrorResponse(HttpStatusCode.BadRequest, "pageIndex cannot be negative");
            if (pageSize < 1 || pageSize > MaximumPageSize)
                return ErrorResponse(HttpStatusCode.BadRequest, $"pageSize must be between 1 and {MaximumPageSize}");
            if (startDate.HasValue != endDate.HasValue)
                return ErrorResponse(HttpStatusCode.BadRequest, "startDate and endDate must be supplied together");
            if (startDate.HasValue && startDate.Value > endDate.Value)
                return ErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be later than endDate");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var page = startDate.HasValue
                    ? _emailAlertAppService.FindEmailAlerts(dlrStatus, startDate.Value, endDate.Value, text ?? string.Empty, pageIndex, pageSize, serviceHeader)
                    : _emailAlertAppService.FindEmailAlerts(dlrStatus, text ?? string.Empty, pageIndex, pageSize, int.MaxValue, serviceHeader);

                return Ok(ApiResponse("Email alerts retrieved successfully", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var emailAlert = _emailAlertAppService.FindEmailAlert(id, serviceHeader);
                if (emailAlert == null)
                    return Content(HttpStatusCode.NotFound, ErrorEnvelope("Email alert not found"));

                return Ok(ApiResponse("Email alert retrieved successfully", emailAlert));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(EmailAlertDTO emailAlert)
        {
            if (emailAlert == null)
                return ErrorResponse(HttpStatusCode.BadRequest, "Request body is required");

            // These are transport lifecycle fields, not caller decisions.
            // The dispatcher replaces MailMessageFrom with its configured
            // SMTP identity when it sends, matching the reference behavior.
            emailAlert.Id = Guid.Empty;
            emailAlert.MailMessageDLRStatus = (int)DLRStatus.Pending;
            emailAlert.MailMessageOrigin = (int)MessageOrigin.Within;
            emailAlert.MailMessageSendRetry = 0;
            emailAlert.MailMessageAttachments = string.Empty;
            emailAlert.CreatedBy = null;
            emailAlert.CreatedDate = default(DateTime);

            if (!Enum.IsDefined(typeof(QueuePriority), emailAlert.MailMessagePriority))
                return ErrorResponse(HttpStatusCode.BadRequest, "Invalid message priority");

            emailAlert.ValidateAll();
            if (emailAlert.HasErrors)
                return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", emailAlert.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var created = _emailAlertAppService.AddNewEmailAlert(emailAlert, serviceHeader);
                if (created == null)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("The email alert could not be queued"));

                return Content(HttpStatusCode.Created, ApiResponse("Email alert queued successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private object ApiResponse(string message, object data)
        {
            return new { success = true, message, data };
        }

        private object ErrorEnvelope(string message)
        {
            return new { success = false, message, data = (object)null };
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, ErrorEnvelope(message));
        }
    }
}
