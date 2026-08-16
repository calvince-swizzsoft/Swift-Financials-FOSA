using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // Adapted from the reference MVC LoanRequestController
    // (Areas/Loaning/Controllers/LoanRequestController.cs) — the optional
    // pre-case intake stage before a real LoanCase is registered (a member
    // expressing interest in a loan product/purpose/amount, no guarantor/
    // appraisal machinery involved yet). See Areas/BackOffice/WORKFLOW.md.
    //
    // The reference Create action is a four-screen Session-driven wizard
    // (Customers -> LoanProducts -> LoansPurpose -> Create submit, each
    // step stashing picks in Session) with no real behavior beyond
    // "populate these four ids onto one DTO" — collapsed into a single
    // Create call here, same as every other session-wizard adaptation in
    // this repo. Its Edit action has no POST counterpart at all (GET-only,
    // dead) — not reproduced. RegisterLoanRequest/CancelLoanRequest/
    // RemoveLoanRequest exist on ILoanRequestAppService but have no
    // reference MVC screen at all (the reference app never actually
    // converts a LoanRequest into a LoanCase, despite LoanRequestDTO
    // carrying LoanCaseId/LoanCaseNumber fields for exactly that) — exposed
    // here as real lifecycle actions since the app-service methods are
    // real and already there.
    [Authorize]
    [RoutePrefix("api/backoffice/loanrequests")]
    public class LoanRequestController : ApiController
    {
        private readonly ILoanRequestAppService _loanRequestAppService;

        public LoanRequestController(ILoanRequestAppService loanRequestAppService)
        {
            _loanRequestAppService = loanRequestAppService ?? throw new ArgumentNullException(nameof(loanRequestAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", int loanRequestFilter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanRequestAppService.FindLoanRequests(text ?? "", loanRequestFilter, pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanRequest = _loanRequestAppService.FindLoanRequest(id, serviceHeader);

                if (loanRequest == null)
                    return NotFound();

                return Ok(ApiResponse("", loanRequest));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("customers/{customerId:guid}/in-process")]
        public IHttpActionResult CustomerInProcess(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanRequests = _loanRequestAppService.FindLoanRequestsByCustomerIdInProcess(customerId, serviceHeader);

                return Ok(ApiResponse("", loanRequests ?? new List<LoanRequestDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LoanRequestDTO loanRequestDTO)
        {
            if (loanRequestDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                // AddNewLoanRequest sets Status/CreatedBy itself regardless
                // of what's on the DTO — not set here to avoid implying
                // otherwise. It does, however, throw InvalidOperationException
                // if the customer already has a pending (New) request for
                // the same loan product, caught below as a clean 400.
                loanRequestDTO.ValidateAll();
                if (loanRequestDTO.HasErrors)
                    return ErrorResponse(string.Join("; ", loanRequestDTO.ErrorMessages));

                var created = _loanRequestAppService.AddNewLoanRequest(loanRequestDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the loan request");

                return Ok(ApiResponse("Loan request created successfully", created));
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Converts a pending (New) LoanRequest into a Registered one —
        // real precondition enforced by RegisterLoanRequest itself. Takes
        // the resulting LoanCaseNumber once a real LoanCase has actually
        // been registered from this request (RegisterLoanRequest persists
        // whatever LoanCaseNumber is on the incoming DTO) — the reference
        // app never wires this link up itself (no screen calls
        // RegisterLoanRequest at all), so there's no reference behavior to
        // match beyond the app-service method's own real parameter.
        [HttpPost]
        [Route("{id:guid}/register")]
        public IHttpActionResult Register(Guid id, RegisterLoanRequestRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanRequestAppService.FindLoanRequest(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                existing.LoanCaseNumber = request?.LoanCaseNumber ?? existing.LoanCaseNumber;

                var registered = _loanRequestAppService.RegisterLoanRequest(existing, serviceHeader);

                if (!registered)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to register the loan request — it may not exist or is not in a New state"));

                var refreshed = _loanRequestAppService.FindLoanRequest(id, serviceHeader);

                return Ok(ApiResponse("Loan request registered successfully", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id:guid}/cancel")]
        public IHttpActionResult Cancel(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanRequestAppService.FindLoanRequest(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                existing.CancelledBy = serviceHeader.ApplicationUserName;
                existing.CancelledDate = DateTime.Now;

                var cancelled = _loanRequestAppService.CancelLoanRequest(existing, serviceHeader);

                if (!cancelled)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to cancel the loan request"));

                var refreshed = _loanRequestAppService.FindLoanRequest(id, serviceHeader);

                return Ok(ApiResponse("Loan request cancelled successfully", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _loanRequestAppService.RemoveLoanRequest(id, serviceHeader);

                if (!removed)
                    return NotFound();

                return Ok(ApiResponse("Loan request removed successfully", null));
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

        private IHttpActionResult ErrorResponse(string message)
        {
            return Content(HttpStatusCode.BadRequest, ErrorEnvelope(message));
        }
    }

    public class RegisterLoanRequestRequest
    {
        public int LoanCaseNumber { get; set; }
    }
}
