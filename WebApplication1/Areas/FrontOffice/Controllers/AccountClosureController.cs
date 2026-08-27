using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC AccountClosureController
    // (SwiftFinancials.Web/Areas/FrontOffice/Controllers/AccountClosureController.cs),
    // which routed everything through the monolithic _channelService — this uses
    // IAccountClosureRequestAppService directly instead.
    //
    // The reference controller's action order (Create -> Verify -> Approve -> Settle)
    // does not match what AccountClosureRequestAppService actually enforces: Approve
    // only accepts a Registered/Deferred request, Audit ("Verify") only accepts an
    // Approved request, and Settle only accepts an Audited request. The real sequence
    // is Create -> Approve -> Verify -> Settle; this controller follows the app
    // service, not the reference controller's naming order.
    //
    // The reference controller's Create/Details/Edit/Verify/Approve/Settle GET actions
    // also each independently recomputed loan balance, investment balance, and
    // guarantor summaries for display via three unrelated app services (loan,
    // investment, guarantor) — that's a customer/accounts-module concern, not an
    // account-closure one, and every one of those endpoints is already documented
    // separately (see docs/api/customer-accounts-api-spec.md and friends). It is not
    // reproduced here; the client composes that view from the existing endpoints.
    [Authorize]
    [RoutePrefix("api/frontoffice/accountclosures")]
    public class AccountClosureController : ApiController
    {
        private readonly IAccountClosureRequestAppService _accountClosureRequestAppService;

        public AccountClosureController(IAccountClosureRequestAppService accountClosureRequestAppService)
        {
            _accountClosureRequestAppService = accountClosureRequestAppService ?? throw new ArgumentNullException(nameof(accountClosureRequestAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int? status, string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                PageCollectionInfo<AccountClosureRequestDTO> requests;

                if (status.HasValue)
                {
                    requests = _accountClosureRequestAppService.FindAccountClosureRequests(
                        startDate ?? DateTime.MinValue,
                        endDate ?? DateTime.MaxValue,
                        status.Value,
                        text ?? "",
                        0,
                        pageIndex,
                        pageSize,
                        serviceHeader);
                }
                else
                {
                    requests = string.IsNullOrWhiteSpace(text)
                        ? _accountClosureRequestAppService.FindAccountClosureRequests(pageIndex, pageSize, serviceHeader)
                        : _accountClosureRequestAppService.FindAccountClosureRequests(text, 0, pageIndex, pageSize, serviceHeader);
                }

                return Ok(ApiResponse("", requests));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var request = _accountClosureRequestAppService.FindAccountClosureRequest(id, serviceHeader);

                if (request == null)
                    return NotFound();

                return Ok(ApiResponse("", request));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("customer-account/{customerAccountId:guid}")]
        public IHttpActionResult GetByCustomerAccount(Guid customerAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var requests = _accountClosureRequestAppService.FindAccountClosureRequestsByCustomerAccountId(customerAccountId, serviceHeader);

                return Ok(ApiResponse("", requests));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Registered. Blocked (returns 409) if the customer account already
        // has a request in Registered/Approved/Audited/Deferred — enforced by the app
        // service itself (AddNewAccountClosureRequest), surfaced here via its
        // errormassage field rather than a thrown exception.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(AccountClosureRequestDTO accountClosureRequestDTO)
        {
            if (accountClosureRequestDTO == null)
                return ErrorResponse("Request body is required");

            accountClosureRequestDTO.ValidateAll();
            if (accountClosureRequestDTO.HasErrors)
                return ErrorResponse(string.Join("; ", accountClosureRequestDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _accountClosureRequestAppService.AddNewAccountClosureRequest(accountClosureRequestDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the account closure request");

                if (!string.IsNullOrWhiteSpace(created.errormassage))
                    return Content(System.Net.HttpStatusCode.Conflict, ErrorEnvelope(created.errormassage));

                return Ok(ApiResponse("Account closure request created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Approve/Defer a Registered (or previously Deferred) request.
        [HttpPost]
        [Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id, [FromBody] AccountClosureActionRequest request)
        {
            return RunTransition(id, request, (dto, opt, header) => _accountClosureRequestAppService.ApproveAccountClosureRequest(dto, opt, header));
        }

        // Verify/Audit an Approved request.
        [HttpPost]
        [Route("{id:guid}/verify")]
        public IHttpActionResult Verify(Guid id, [FromBody] AccountClosureActionRequest request)
        {
            return RunTransition(id, request, (dto, opt, header) => _accountClosureRequestAppService.AuditAccountClosureRequest(dto, opt, header));
        }

        // Settle/Defer an Audited (verified) request — pays out remaining balance and
        // closes the account.
        [HttpPost]
        [Route("{id:guid}/settle")]
        public IHttpActionResult Settle(Guid id, [FromBody] AccountClosureActionRequest request)
        {
            return RunTransition(id, request, (dto, opt, header) => _accountClosureRequestAppService.SettleAccountClosureRequest(dto, opt, header));
        }

        private IHttpActionResult RunTransition(Guid id, AccountClosureActionRequest request, Func<AccountClosureRequestDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _accountClosureRequestAppService.FindAccountClosureRequest(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.ApprovalRemarks = request?.Remarks ?? existing.ApprovalRemarks;
                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(System.Net.HttpStatusCode.Conflict, ErrorEnvelope("Request is not in the right state for this action, or the action option is invalid"));

                var updated = _accountClosureRequestAppService.FindAccountClosureRequest(id, serviceHeader);

                return Ok(ApiResponse("Operation success", updated));
            }
            catch (Exception)
            {
                throw;
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

        private IHttpActionResult ErrorResponse(string message)
        {
            return BadRequest(message);
        }
    }

    public class AccountClosureActionRequest
    {
        // AccountClosureApprovalOption / AccountClosureAuditOption / AccountClosureSettlementOption
        // all share the same shape: 1 = act (Approve/Audit/Settle), 2 = Defer.
        public int Option { get; set; }

        public string Remarks { get; set; }
    }
}
