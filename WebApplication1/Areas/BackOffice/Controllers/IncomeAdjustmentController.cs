using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // New — IIncomeAdjustmentAppService was already fully built (reference
    // screen: Areas/Loaning/Controllers/IncomeAdjustmentsController.cs) but
    // had no controller anywhere. Built to unblock the loan-case appraisal
    // screen's income-adjustment picker — see
    // Areas/BackOffice/WORKFLOW.md §15.2 and §13. Same CRUD shape as
    // LoanPurposeController/LoaningRemarkController; `Type`
    // (`IncomeAdjustmentType`: Allowance/Deduction) is the one extra field
    // versus those two.
    [Authorize]
    [RoutePrefix("api/backoffice/incomeadjustments")]
    public class IncomeAdjustmentController : ApiController
    {
        private readonly IIncomeAdjustmentAppService _incomeAdjustmentAppService;

        public IncomeAdjustmentController(IIncomeAdjustmentAppService incomeAdjustmentAppService)
        {
            _incomeAdjustmentAppService = incomeAdjustmentAppService ?? throw new ArgumentNullException(nameof(incomeAdjustmentAppService));
        }

        // Unpaged — for pickers (e.g. loan-case appraisal's income-adjustment
        // picker). See GET /paged for an admin-screen listing.
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var incomeAdjustments = _incomeAdjustmentAppService.FindIncomeAdjustments(serviceHeader);

                return Ok(new { success = true, message = "", data = incomeAdjustments ?? new List<IncomeAdjustmentDTO>() });
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
                    ? _incomeAdjustmentAppService.FindIncomeAdjustments(pageIndex, pageSize, serviceHeader)
                    : _incomeAdjustmentAppService.FindIncomeAdjustments(text, pageIndex, pageSize, serviceHeader);

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

                var incomeAdjustment = _incomeAdjustmentAppService.FindIncomeAdjustment(id, serviceHeader);

                if (incomeAdjustment == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = incomeAdjustment });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(IncomeAdjustmentDTO incomeAdjustmentDTO)
        {
            if (incomeAdjustmentDTO == null)
                return ErrorResponse("Request body is required");

            incomeAdjustmentDTO.ValidateAll();
            if (incomeAdjustmentDTO.HasErrors)
                return ErrorResponse(string.Join("; ", incomeAdjustmentDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _incomeAdjustmentAppService.AddNewIncomeAdjustment(incomeAdjustmentDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the income adjustment");

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope(created.ErrorMessageResult));

                return Ok(ApiResponse("Income adjustment created successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, IncomeAdjustmentDTO incomeAdjustmentDTO)
        {
            if (incomeAdjustmentDTO == null)
                return ErrorResponse("Request body is required");

            incomeAdjustmentDTO.Id = id;

            incomeAdjustmentDTO.ValidateAll();
            if (incomeAdjustmentDTO.HasErrors)
                return ErrorResponse(string.Join("; ", incomeAdjustmentDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _incomeAdjustmentAppService.UpdateIncomeAdjustment(incomeAdjustmentDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _incomeAdjustmentAppService.FindIncomeAdjustment(id, serviceHeader);

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
