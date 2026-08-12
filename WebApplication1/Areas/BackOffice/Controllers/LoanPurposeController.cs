using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // New — ILoanPurposeAppService was already fully built (reference
    // screen: Areas/Loaning/Controllers/LoanPurposeController.cs) but had no
    // controller anywhere. Built to unblock the loan-case registration
    // screen's loan-purpose picker — see Areas/BackOffice/WORKFLOW.md §15.2
    // and §13. Same CRUD shape as UnPayReasonController (Description
    // [Required], IsLocked toggle, duplicate-description reported via
    // ErrorMessageResult on the echoed-back DTO rather than a thrown
    // exception).
    [Authorize]
    [RoutePrefix("api/backoffice/loanpurposes")]
    public class LoanPurposeController : ApiController
    {
        private readonly ILoanPurposeAppService _loanPurposeAppService;

        public LoanPurposeController(ILoanPurposeAppService loanPurposeAppService)
        {
            _loanPurposeAppService = loanPurposeAppService ?? throw new ArgumentNullException(nameof(loanPurposeAppService));
        }

        // Unpaged — for pickers (e.g. loan-case registration's loan-purpose
        // picker). See GET /paged for an admin-screen listing.
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanPurposes = _loanPurposeAppService.FindLoanPurposes(serviceHeader);

                return Ok(new { success = true, message = "", data = loanPurposes ?? new List<LoanPurposeDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public IHttpActionResult Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _loanPurposeAppService.FindLoanPurposes(pageIndex, pageSize, serviceHeader)
                    : _loanPurposeAppService.FindLoanPurposes(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
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

                var loanPurpose = _loanPurposeAppService.FindLoanPurpose(id, serviceHeader);

                if (loanPurpose == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = loanPurpose });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LoanPurposeDTO loanPurposeDTO)
        {
            if (loanPurposeDTO == null)
                return ErrorResponse("Request body is required");

            loanPurposeDTO.ValidateAll();
            if (loanPurposeDTO.HasErrors)
                return ErrorResponse(string.Join("; ", loanPurposeDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _loanPurposeAppService.AddNewLoanPurpose(loanPurposeDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the loan purpose");

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope(created.ErrorMessageResult));

                return Ok(ApiResponse("Loan purpose created successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, LoanPurposeDTO loanPurposeDTO)
        {
            if (loanPurposeDTO == null)
                return ErrorResponse("Request body is required");

            loanPurposeDTO.Id = id;

            loanPurposeDTO.ValidateAll();
            if (loanPurposeDTO.HasErrors)
                return ErrorResponse(string.Join("; ", loanPurposeDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanPurposeAppService.UpdateLoanPurpose(loanPurposeDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _loanPurposeAppService.FindLoanPurpose(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
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
}
