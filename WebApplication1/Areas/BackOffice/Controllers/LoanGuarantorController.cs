using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // Adapted from the reference MVC LoanGuarantorController
    // (Areas/Loaning/Controllers/LoanGuarantorController.cs) — standalone
    // CRUD/search over LoanGuarantorDTO records, independent of the
    // enrichment/validation LoanCaseController.EnrichAndValidateGuarantors
    // does at case-registration time. See Areas/BackOffice/WORKFLOW.md.
    //
    // Not ported: the reference Edit GET view has its POST counterpart
    // entirely commented out (no UpdateLoanGuarantorAsync call exists,
    // dead code) — matching that, there is no Update action here either.
    [Authorize]
    [RoutePrefix("api/backoffice/loanguarantors")]
    public class LoanGuarantorController : ApiController
    {
        private readonly ILoanCaseAppService _loanCaseAppService;

        public LoanGuarantorController(ILoanCaseAppService loanCaseAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanCaseAppService.FindLoanGuarantors(text ?? "", pageIndex, pageSize, serviceHeader);

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

                var guarantor = _loanCaseAppService.FindLoanGuarantor(id, serviceHeader);

                if (guarantor == null)
                    return NotFound();

                return Ok(ApiResponse("", guarantor));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Every loan a customer is currently guaranteeing, across all loan
        // cases — mirrors the "committed shares" figure LoanCaseController
        // computes internally (ComputeCommittedShares), exposed here as a
        // real read of its own.
        [HttpGet]
        [Route("customer/{customerId:guid}")]
        public IHttpActionResult ByCustomer(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var guarantors = _loanCaseAppService.FindLoanGuarantorsByCustomerId(customerId, serviceHeader);

                return Ok(ApiResponse("", guarantors ?? new List<LoanGuarantorDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LoanGuarantorDTO loanGuarantorDTO)
        {
            if (loanGuarantorDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                loanGuarantorDTO.CreatedBy = serviceHeader.ApplicationUserName;

                loanGuarantorDTO.ValidateAll();
                if (loanGuarantorDTO.HasErrors)
                    return ErrorResponse(string.Join("; ", loanGuarantorDTO.ErrorMessages));

                var created = _loanCaseAppService.AddNewLoanGuarantor(loanGuarantorDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the loan guarantor record");

                return Ok(ApiResponse("Loan guarantor created successfully", created));
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
            return Content(System.Net.HttpStatusCode.BadRequest, ErrorEnvelope(message));
        }
    }
}
