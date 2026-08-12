using Application.MainBoundedContext.AccountsModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Batch triggers for ElectronicStatementOrder, split out of
    // ElectronicStatementOrderController the same way StandingOrderExecutionController
    // is split from StandingOrderController — Execute wraps IRecurringBatchAppService
    // (the actual generate-and-send batch job, run on a schedule elsewhere; this is the
    // manual/on-demand equivalent), FixSkipped wraps
    // IElectronicStatementOrderAppService.FixSkippedElectronicStatementOrders — grouped
    // here by concept (both are "run this batch process now" actions) rather than by
    // which app service happens to implement them, matching the Standing Order precedent.
    [RoutePrefix("api/accounts/electronicstatementorders/execution")]
    public class ElectronicStatementOrderExecutionController : ApiController
    {
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        private readonly IElectronicStatementOrderAppService _electronicStatementOrderAppService;

        public ElectronicStatementOrderExecutionController(
            IRecurringBatchAppService recurringBatchAppService,
            IElectronicStatementOrderAppService electronicStatementOrderAppService)
        {
            _recurringBatchAppService = recurringBatchAppService ?? throw new ArgumentNullException(nameof(recurringBatchAppService));
            _electronicStatementOrderAppService = electronicStatementOrderAppService ?? throw new ArgumentNullException(nameof(electronicStatementOrderAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpPost, Route("execute")]
        public IHttpActionResult Execute([FromBody] ExecuteElectronicStatementOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _recurringBatchAppService.ExecuteElectronicStatementOrders(
                    request.TargetDate ?? DateTime.Today,
                    request.TargetDateOption,
                    request.Sender,
                    request.Priority,
                    request.PageSize,
                    serviceHeader);

                return ApiResponse(result, result ? "E-statement orders executed successfully" : "No e-statement orders were executed");
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("fix-skipped")]
        public IHttpActionResult FixSkipped([FromBody] FixSkippedElectronicStatementOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _electronicStatementOrderAppService.FixSkippedElectronicStatementOrders(
                    request.TargetDate ?? DateTime.Today.AddDays(-1),
                    serviceHeader);

                return ApiResponse(result, result ? "Skipped e-statement orders fixed successfully" : "No skipped e-statement orders were fixed");
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public class ExecuteElectronicStatementOrdersRequest
        {
            public DateTime? TargetDate { get; set; }

            public int TargetDateOption { get; set; }

            public string Sender { get; set; }

            public int Priority { get; set; }

            public int PageSize { get; set; } = 100;
        }

        public class FixSkippedElectronicStatementOrdersRequest
        {
            public DateTime? TargetDate { get; set; }
        }
    }
}
