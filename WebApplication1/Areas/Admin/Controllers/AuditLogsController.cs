using Application.MainBoundedContext.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.ApiErrors;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Authorize, RoutePrefix("api/administration/auditlogs")]
    public class AuditLogsController : ApiController
    {
        private readonly IAuditLogAppService service;
        public AuditLogsController(IAuditLogAppService service) { this.service = service ?? throw new ArgumentNullException(nameof(service)); }

        [HttpGet, Route("")]
        public Task<IHttpActionResult> Index(int pageIndex = 0, int pageSize = 20, string text = null, DateTime? startDate = null, DateTime? endDate = null)
        { return Read(false, pageIndex, pageSize, text, startDate, endDate); }

        [HttpGet, Route("entries")]
        public Task<IHttpActionResult> Entries(int pageIndex = 0, int pageSize = 20, string text = null, DateTime? startDate = null, DateTime? endDate = null)
        { return Read(true, pageIndex, pageSize, text, startDate, endDate); }

        private async Task<IHttpActionResult> Read(bool entries, int pageIndex, int pageSize, string text, DateTime? startDate, DateTime? endDate)
        {
            if (!ModelState.IsValid) return Invalid("Check the page number and dates in the audit request.");
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100 || (long)pageIndex * pageSize > int.MaxValue)
                return Invalid("Choose a valid page number and a page size between 1 and 100.");
            if ((text ?? "").Length > 256) return Invalid("Search text must be 256 characters or fewer.");
            var start = (startDate ?? new DateTime(1753, 1, 1)).Date;
            var end = (endDate ?? new DateTime(9998, 12, 31)).Date;
            if (start.Year < 1753 || end.Year >= 9999 || start > end)
                return Invalid("Choose dates on or after 1753 and before 9999, with the start on or before the end.");
            var header = Helpers.Utils.CreateServiceHeader();
            if (entries) {
                var page = await service.FindAuditTrailsByDateRangeAndFilterAsync(pageIndex, pageSize, start, end, text?.Trim(), header);
                return Ok(new { success = true, data = page });
            }
            var logs = await service.FindAuditLogsByDateRangeAndFilterAsync(pageIndex, pageSize, start, end, text?.Trim(), header);
            return Ok(new { success = true, data = logs });
        }
        private IHttpActionResult Invalid(string message)
        { return ResponseMessage(ApiErrorResponses.Create(Request, HttpStatusCode.BadRequest, "audit.invalid_request", message)); }
    }
}
