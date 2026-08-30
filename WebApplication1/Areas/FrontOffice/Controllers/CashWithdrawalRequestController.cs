using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/frontoffice/cash-withdrawal-requests")]
    public class CashWithdrawalRequestController : ApiController
    {
        private readonly ICashWithdrawalRequestAppService _cashWithdrawalRequestAppService;

        public CashWithdrawalRequestController(ICashWithdrawalRequestAppService cashWithdrawalRequestAppService)
        {
            _cashWithdrawalRequestAppService = cashWithdrawalRequestAppService ?? throw new ArgumentNullException(nameof(cashWithdrawalRequestAppService));
        }

        [HttpGet, Route("")]
        public IHttpActionResult Index(DateTime? startDate = null, DateTime? endDate = null,
            int status = (int)CashWithdrawalRequestAuthStatus.Pending, string text = "",
            int customerFilter = 0, int pageIndex = 0, int pageSize = 20)
        {
            if (!Enum.IsDefined(typeof(CashWithdrawalRequestAuthStatus), status))
                return BadRequest("The selected request status is invalid.");
            if (!Enum.IsDefined(typeof(CustomerFilter), customerFilter))
                return BadRequest("The selected customer search field is invalid.");
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100)
                return BadRequest("Page index must be zero or greater and page size must be between 1 and 100.");

            var result = _cashWithdrawalRequestAppService.FindCashWithdrawalRequests(
                startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, status,
                text ?? string.Empty, customerFilter, pageIndex, pageSize, Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "Cash withdrawal requests retrieved successfully.", data = result });
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var request = _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(id, Utils.CreateServiceHeader());
            if (request == null)
                return Content(HttpStatusCode.NotFound, new { success = false, message = "Cash withdrawal request not found." });
            return Ok(new { success = true, message = "Cash withdrawal request retrieved successfully.", data = request });
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(CashWithdrawalRequestDTO request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            // These are server-owned lifecycle values. Category 1 is a manually
            // lodged, within-limits notice; exception categories are created by
            // the teller transaction AppService path, not accepted from this UI.
            request.Status = (int)CashWithdrawalRequestAuthStatus.Pending;
            request.Category = (int)CashWithdrawalCategory.WithinLimits;
            request.TransactionType = (int)FrontOfficeTransactionType.CashWithdrawal;
            request.ValidateAll();
            if (request.HasErrors)
                return BadRequest(string.Join("; ", request.ErrorMessages));
            if (!request.CustomerAccountId.HasValue || request.CustomerAccountId == Guid.Empty)
                return BadRequest("A customer savings account is required.");

            try
            {
                var created = _cashWithdrawalRequestAppService.AddNewCashWithdrawalRequestWithWorkflow(request, Utils.CreateServiceHeader());
                if (created == null)
                    return BadRequest("The cash withdrawal request could not be created. Verify the branch and account details.");
                return Content(HttpStatusCode.Created, new { success = true, message = "Cash withdrawal request created and is pending authorization.", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("{id:guid}/resend-approval")]
        public IHttpActionResult ResendApproval(Guid id)
        {
            try
            {
                var resent = _cashWithdrawalRequestAppService.ResendCashWithdrawalApprovalRequest(id, Utils.CreateServiceHeader());
                if (!resent)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "The approval request could not be resent." });

                return Ok(new { success = true, message = "The cash withdrawal approval request was resent successfully." });
            }
            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.Conflict, new { success = false, message = exception.Message });
            }
        }

    }
}
