using Application.MainBoundedContext.AccountsModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/recurringbatches")]
    public class RecurringBatchController : ApiController
    {
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        public RecurringBatchController(IRecurringBatchAppService recurringBatchAppService)
        {
            _recurringBatchAppService = recurringBatchAppService ?? throw new ArgumentNullException(nameof(recurringBatchAppService));
        }

        private IHttpActionResult ApiResponse(string message, object data)
        {
            return Ok(new { success = true, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get([FromUri] int? type = null, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200)
                return ErrorResponse(HttpStatusCode.BadRequest, "pageIndex must be zero or greater and pageSize must be between 1 and 200");

            if (type.HasValue && !Enum.IsDefined(typeof(Infrastructure.Crosscutting.Framework.Utils.RecurringBatchType), type.Value))
                return ErrorResponse(HttpStatusCode.BadRequest, "Select a valid recurring batch type");

            var page = _recurringBatchAppService.FindRecurringBatches(type, pageIndex, pageSize, Utils.CreateServiceHeader());
            return ApiResponse("Recurring batches retrieved successfully", page);
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            var batch = _recurringBatchAppService.FindRecurringBatch(id, Utils.CreateServiceHeader());
            return batch == null
                ? ErrorResponse(HttpStatusCode.NotFound, "Recurring batch not found")
                : ApiResponse("Recurring batch retrieved successfully", batch);
        }

        [HttpGet, Route("{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, [FromUri] string text = "", [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200)
                return ErrorResponse(HttpStatusCode.BadRequest, "pageIndex must be zero or greater and pageSize must be between 1 and 200");

            var serviceHeader = Utils.CreateServiceHeader();
            if (_recurringBatchAppService.FindRecurringBatch(id, serviceHeader) == null)
                return ErrorResponse(HttpStatusCode.NotFound, "Recurring batch not found");

            var entries = _recurringBatchAppService.FindRecurringBatchEntriesByRecurringBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);
            return ApiResponse("Recurring batch entries retrieved successfully", entries);
        }

        [HttpGet, Route("queueable")]
        public IHttpActionResult GetQueueable([FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200)
                return ErrorResponse(HttpStatusCode.BadRequest, "pageIndex must be zero or greater and pageSize must be between 1 and 200");

            var entries = _recurringBatchAppService.FindQueableRecurringBatchEntries(pageIndex, pageSize, Utils.CreateServiceHeader());
            return ApiResponse("Queueable recurring batch entries retrieved successfully", entries);
        }
    }
}
