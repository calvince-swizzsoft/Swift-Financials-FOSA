using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // New — ILoaningRemarkAppService was already fully built (reference
    // screen: Areas/Loaning/Controllers/LoaningRemarkController.cs) but had
    // no controller anywhere. Built to unblock the loan-case registration
    // screen's registration-remark picker — see
    // Areas/BackOffice/WORKFLOW.md §15.2 and §13. Same CRUD shape as
    // LoanPurposeController/UnPayReasonController.
    [Authorize]
    [RoutePrefix("api/backoffice/loaningremarks")]
    public class LoaningRemarkController : ApiController
    {
        private readonly ILoaningRemarkAppService _loaningRemarkAppService;

        public LoaningRemarkController(ILoaningRemarkAppService loaningRemarkAppService)
        {
            _loaningRemarkAppService = loaningRemarkAppService ?? throw new ArgumentNullException(nameof(loaningRemarkAppService));
        }

        // Unpaged — for pickers (e.g. loan-case registration's
        // registration-remark picker). See GET /paged for an admin-screen
        // listing.
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loaningRemarks = _loaningRemarkAppService.FindLoaningRemarks(serviceHeader);

                return Ok(new { success = true, message = "", data = loaningRemarks ?? new List<LoaningRemarkDTO>() });
            }
            catch (Exception)
            {
                throw;
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
                    ? _loaningRemarkAppService.FindLoaningRemarks(pageIndex, pageSize, serviceHeader)
                    : _loaningRemarkAppService.FindLoaningRemarks(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
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

                var loaningRemark = _loaningRemarkAppService.FindLoaningRemark(id, serviceHeader);

                if (loaningRemark == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = loaningRemark });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LoaningRemarkDTO loaningRemarkDTO)
        {
            if (loaningRemarkDTO == null)
                return ErrorResponse("Request body is required");

            loaningRemarkDTO.ValidateAll();
            if (loaningRemarkDTO.HasErrors)
                return ErrorResponse(string.Join("; ", loaningRemarkDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _loaningRemarkAppService.AddNewLoaningRemark(loaningRemarkDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the loaning remark");

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope(created.ErrorMessageResult));

                return Ok(ApiResponse("Loaning remark created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, LoaningRemarkDTO loaningRemarkDTO)
        {
            if (loaningRemarkDTO == null)
                return ErrorResponse("Request body is required");

            loaningRemarkDTO.Id = id;

            loaningRemarkDTO.ValidateAll();
            if (loaningRemarkDTO.HasErrors)
                return ErrorResponse(string.Join("; ", loaningRemarkDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loaningRemarkAppService.UpdateLoaningRemark(loaningRemarkDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _loaningRemarkAppService.FindLoaningRemark(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
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
            return Content(HttpStatusCode.BadRequest, ErrorEnvelope(message));
        }
    }
}
