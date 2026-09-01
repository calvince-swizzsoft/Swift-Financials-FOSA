using Application.MainBoundedContext.AccountsModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/standingorders/execution")]
    public class StandingOrderExecutionController : ApiController
    {
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        private readonly IStandingOrderAppService _standingOrderAppService;

        public StandingOrderExecutionController(
            IRecurringBatchAppService recurringBatchAppService,
            IStandingOrderAppService standingOrderAppService)
        {
            _recurringBatchAppService = recurringBatchAppService ?? throw new ArgumentNullException(nameof(recurringBatchAppService));
            _standingOrderAppService = standingOrderAppService ?? throw new ArgumentNullException(nameof(standingOrderAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        // Manual equivalent of SwiftFinancials.StandingOrderInvoker's StandingOrderJob (Dispatcher).
        [HttpPost, Route("execute")]
        public IHttpActionResult Execute([FromBody] ExecuteStandingOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _recurringBatchAppService.ExecuteStandingOrdersDetailed(
                    request.TargetDate ?? DateTime.Today,
                    request.TargetDateOption,
                    request.Priority,
                    request.MaximumStandingOrderExecuteAttemptCount,
                    request.PageSize,
                    serviceHeader);

                return ApiResponse(
                    true,
                    result.QueuedCount > 0 ? "Standing orders queued successfully" : result.Detail,
                    result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Manual equivalent of SwiftFinancials.StandingOrderInvoker's SkippedStandingOrderJob (Fixer).
        [HttpPost, Route("fix-skipped")]
        public IHttpActionResult FixSkipped([FromBody] FixSkippedStandingOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _standingOrderAppService.FixSkippedStandingOrders(
                    request.TargetDate ?? DateTime.Today.AddDays(-1),
                    request.PageSize,
                    serviceHeader);

                return ApiResponse(result, result ? "Skipped standing orders fixed successfully" : "No skipped standing orders were fixed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Manual equivalent of SwiftFinancials.StandingOrderInvoker's SweepingStandingOrderJob (Sweeper).
        [HttpPost, Route("sweep")]
        public IHttpActionResult Sweep([FromBody] SweepStandingOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _recurringBatchAppService.ExecuteSweepingStandingOrders(request.Priority, request.PageSize, serviceHeader);

                return ApiResponse(result, result ? "Sweeping standing orders executed successfully" : "No sweeping standing orders were executed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // On-demand payout trigger for a single benefactor account; no scheduled job mirrors this one.
        [HttpPost, Route("payout")]
        public IHttpActionResult Payout([FromBody] PayoutStandingOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _recurringBatchAppService.ExecutePayoutStandingOrders(request.BenefactorCustomerAccountId, request.Month, request.Priority, serviceHeader);

                return ApiResponse(result, result ? "Payout standing orders executed successfully" : "No payout standing orders were executed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public class ExecuteStandingOrdersRequest
        {
            public DateTime? TargetDate { get; set; }

            public int TargetDateOption { get; set; }

            public int Priority { get; set; }

            public int MaximumStandingOrderExecuteAttemptCount { get; set; }

            public int PageSize { get; set; } = 100;
        }

        public class FixSkippedStandingOrdersRequest
        {
            public DateTime? TargetDate { get; set; }

            public int PageSize { get; set; } = 100;
        }

        public class SweepStandingOrdersRequest
        {
            public int Priority { get; set; }

            public int PageSize { get; set; } = 100;
        }

        public class PayoutStandingOrdersRequest
        {
            public Guid BenefactorCustomerAccountId { get; set; }

            public int Month { get; set; }

            public int Priority { get; set; }
        }
    }
}
