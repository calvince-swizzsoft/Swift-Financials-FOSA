using System;
using System.Net;
using System.Web.Http;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using WebApplication1.Helpers;
using WebApplication1.ApiErrors;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/mobile-to-bank")]
    public class MobileToBankController : ApiController
    {
        private readonly IMobileToBankRequestAppService _service;
        public MobileToBankController(IMobileToBankRequestAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet, Route("")]
        public IHttpActionResult Index(DateTime startDate, DateTime endDate, int? status = null,
            string text = "", int pageIndex = 0, int pageSize = 20)
        {
            if (!ModelState.IsValid || startDate == default(DateTime) || endDate == default(DateTime) || startDate.Date > endDate.Date || endDate.Year >= 9999 || pageIndex < 0 || pageSize < 1 || pageSize > 100 ||
                (status.HasValue && !Enum.IsDefined(typeof(MobileToBankRequestStatus), status.Value)))
                return Failure(HttpStatusCode.BadRequest, "VALIDATION_ERROR", "Supply a valid date range, matching status and page size (1–100).");
            var header = Utils.CreateServiceHeader();
            var page = status.HasValue
                ? _service.FindMobileToBankRequests(status.Value, startDate.Date, endDate.Date, text ?? "", pageIndex, pageSize, header)
                : _service.FindMobileToBankRequests(startDate.Date, endDate.Date, text ?? "", pageIndex, pageSize, header);
            return Ok(new { data = page });
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Details(Guid id)
        {
            var item = _service.FindMobileToBankRequest(id, Utils.CreateServiceHeader());
            if (item == null) return NotFound();
            return Ok(new { data = item });
        }

        public sealed class ReconcileRequest
        {
            public Guid CustomerAccountId { get; set; }
        }

        [HttpPut, Route("{id:guid}/reconcile")]
        public IHttpActionResult Reconcile(Guid id, ReconcileRequest request)
        {
            if (!ModelState.IsValid || request == null || request.CustomerAccountId == Guid.Empty)
                return Failure(HttpStatusCode.BadRequest, "VALIDATION_ERROR", "Select a customer account.");
            // Accept only the destination. Transaction amount, source references and audit fields
            // remain server-owned. The AppService enforces Unmatched + Pending before updating.
            var updated = _service.ReconcileMobileToBankRequest(new MobileToBankRequestDTO
            {
                Id = id, CustomerAccountId = request.CustomerAccountId,
                ReconciliationType = (int)ApportionTo.CustomerAccount
            }, Utils.CreateServiceHeader());
            if (!updated)
                return Failure(HttpStatusCode.Conflict, "RECONCILIATION_NOT_ALLOWED", "The transaction is no longer unmatched and pending, or the account could not be found. Refresh and try again.");
            return Ok(new { success = true, message = "Customer account matched. Pending verification; no funds were posted by this action." });
        }

        private IHttpActionResult Failure(HttpStatusCode status, string code, string message)
        {
            return ResponseMessage(ApiErrorResponses.Create(Request, status, code, message));
        }
    }
}