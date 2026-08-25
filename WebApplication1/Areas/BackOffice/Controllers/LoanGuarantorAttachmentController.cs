using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // Consolidates three reference MVC controllers that all operate on the
    // same LoanGuarantorAttachmentHistory-family resource — post-
    // registration guarantor attachment, browsing that history, relieving
    // an attachment, and substituting one guarantor for another — into one
    // controller per this repo's "one controller per resource" convention
    // (see LoanCaseController's own header comment). References:
    // Areas/Loaning/Controllers/GuarantorAttachmentController.cs,
    // GuarantorRelievingController.cs, GuarantorSubstitutionController.cs.
    //
    // Not ported: GuarantorAttachmentController's raw ADO.NET
    // GetLoanGuarantorsAsync/GetCustomerNamesAsync (hand-rolled SQL against
    // swiftFin_LoanGuarantors/swiftFin_Customers — the real equivalent is
    // LoanGuarantorController's own endpoints); GuarantorManagementController
    // entirely (its Add() accumulates guarantors in Session with real
    // validation, but its Create(LoanCaseDTO) POST that should commit them
    // does nothing — Session["LoanProductId"] = null; return View(); with
    // no persistence call at all — dead/non-functional in the reference
    // itself, and a near-duplicate of LoanCaseController's own
    // EnrichAndValidateGuarantors besides).
    [Authorize]
    [RoutePrefix("api/backoffice/loanguarantorattachments")]
    public class LoanGuarantorAttachmentController : ApiController
    {
        private readonly ILoanCaseAppService _loanCaseAppService;

        public LoanGuarantorAttachmentController(ILoanCaseAppService loanCaseAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
        }

        // Attaches one or more guarantors to an already-registered loan
        // (post-registration guarantor attachment — distinct from the
        // guarantors submitted with LoanCaseController.Create). Mirrors
        // the reference GuarantorAttachmentController.Create's real call:
        // AttachLoanGuarantorsAsync(sourceCustomerAccountId, attachToId
        // [a loan product id], guarantor details, moduleNavigationItemCode).
        [HttpPost]
        [Route("")]
        public IHttpActionResult Attach(AttachLoanGuarantorsRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (request.SourceCustomerAccountId == Guid.Empty)
                return ErrorResponse("SourceCustomerAccountId is required");

            if (request.DestinationLoanProductId == Guid.Empty)
                return ErrorResponse("DestinationLoanProductId is required");

            var guarantors = request.LoanGuarantors ?? new List<LoanGuarantorDTO>();
            if (!guarantors.Any())
                return ErrorResponse("At least one loan guarantor is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var attached = _loanCaseAppService.AttachLoanGuarantors(request.SourceCustomerAccountId, request.DestinationLoanProductId, guarantors, request.ModuleNavigationItemCode, serviceHeader);

                if (!attached)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to attach the loan guarantors"));

                return Ok(ApiResponse("Loan guarantors attached successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Browses attachment history — status defaults to Attached (0), the
        // reference Index grid's own default (LoanGuarantorAttachmentHistoryStatus.Attached).
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status = (int)LoanGuarantorAttachmentHistoryStatus.Attached, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
                var effectiveEndDate = endDate ?? DateTime.Today;

                var page = _loanCaseAppService.FindLoanGuarantorAttachmentHistoryByStatus(status, effectiveStartDate, effectiveEndDate, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _loanCaseAppService.FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(id, serviceHeader);

                return Ok(ApiResponse("", entries ?? new List<LoanGuarantorAttachmentHistoryEntryDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult GetEntry(Guid entryId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entry = _loanCaseAppService.FindLoanGuarantorAttachmentHistoryEntry(entryId, serviceHeader);

                if (entry == null)
                    return NotFound();

                return Ok(ApiResponse("", entry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Reference: GuarantorRelievingController.Update — relieves every
        // guarantee under one attachment history record in one call.
        [HttpPost]
        [Route("{id:guid}/relieve")]
        public IHttpActionResult Relieve(Guid id, ModuleNavigationRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var relieved = _loanCaseAppService.RelieveLoanGuarantors(id, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!relieved)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to relieve the loan guarantors — the attachment history record may not exist or is already relieved"));

                return Ok(ApiResponse("Loan guarantors relieved successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Reference: GuarantorSubstitutionController.Create — replaces the
        // guarantor on each of the given existing LoanGuarantorDTO records
        // with SubstituteGuarantorCustomerId. The reference resolves each
        // picked guarantor id via FindLoanGuarantorAsync before calling
        // SubstituteLoanGuarantorsAsync; reproduced the same way here so a
        // caller only has to supply the ids being substituted, not full
        // guarantor payloads.
        [HttpPost]
        [Route("substitute")]
        public IHttpActionResult Substitute(SubstituteLoanGuarantorsRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (request.SubstituteGuarantorCustomerId == Guid.Empty)
                return ErrorResponse("SubstituteGuarantorCustomerId is required");

            var loanGuarantorIds = request.LoanGuarantorIds ?? new List<Guid>();
            if (!loanGuarantorIds.Any())
                return ErrorResponse("At least one LoanGuarantorId is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loansGuaranteed = new List<LoanGuarantorDTO>();
                foreach (var loanGuarantorId in loanGuarantorIds)
                {
                    var loanGuarantor = _loanCaseAppService.FindLoanGuarantor(loanGuarantorId, serviceHeader);
                    if (loanGuarantor == null)
                        return ErrorResponse($"Loan guarantor {loanGuarantorId} not found");

                    loansGuaranteed.Add(loanGuarantor);
                }

                var substituted = _loanCaseAppService.SubstituteLoanGuarantors(request.SubstituteGuarantorCustomerId, loansGuaranteed, request.ModuleNavigationItemCode, serviceHeader);

                if (!substituted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to substitute the loan guarantors"));

                return Ok(ApiResponse("Loan guarantors substituted successfully", null));
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

    public class AttachLoanGuarantorsRequest
    {
        public Guid SourceCustomerAccountId { get; set; }
        public Guid DestinationLoanProductId { get; set; }
        public List<LoanGuarantorDTO> LoanGuarantors { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }

    public class SubstituteLoanGuarantorsRequest
    {
        public Guid SubstituteGuarantorCustomerId { get; set; }
        public List<Guid> LoanGuarantorIds { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }

    public class ModuleNavigationRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
