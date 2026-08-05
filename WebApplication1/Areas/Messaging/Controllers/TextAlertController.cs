using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Messaging.Controllers
{
    [RoutePrefix("api/messaging/textalert")]
    public class TextAlertController : ApiController
    {
        private readonly ITextAlertAppService _textAlertAppService;

        public TextAlertController(ITextAlertAppService textAlertAppService)
        {
            _textAlertAppService = textAlertAppService ?? throw new ArgumentNullException(nameof(textAlertAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int? dlrStatus = null)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = dlrStatus.HasValue
                    ? _textAlertAppService.FindTextAlerts(dlrStatus.Value, text ?? string.Empty, pageIndex, pageSize, 0, serviceHeader)
                    : _textAlertAppService.FindTextAlerts(pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Text alerts retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var textAlert = _textAlertAppService.FindTextAlert(id, serviceHeader);
                if (textAlert == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Text alert not found");

                return ApiResponse(true, "Text alert retrieved successfully", textAlert);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] TextAlertDTO textAlert)
        {
            try
            {
                if (textAlert == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid text alert data");

                textAlert.TextMessageSecurityCritical = true;
                textAlert.TextMessagePriority = (int)QueuePriority.High;
                textAlert.TextMessageDLRStatus = (int)DLRStatus.Pending;
                textAlert.TextMessageOrigin = (int)MessageOrigin.Within;
                textAlert.TextMessageSendRetry = 0;
                textAlert.ValidateAll();

                if (textAlert.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", textAlert.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var createdTextAlert = _textAlertAppService.AddNewTextAlert(textAlert, serviceHeader);

                if (createdTextAlert == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create text alert");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Text alert created successfully",
                    data = createdTextAlert
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
