using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC BatchOrigination_Refund /
    // BatchVerification_Refund / BatchAuthorization_Refund controllers —
    // folded into one, same reasoning as every other type in this module (see
    // BATCH-PROCEDURES-CONCEPTS.md §1).
    //
    // What this type actually is: refunding a prior over-collection (from a
    // Credit/Debit/CheckOff run) back to the affected member. An entry pairs a
    // debit side and a credit side (both real CustomerAccounts, unlike
    // General Ledger's mixed GL-or-customer pairing) plus `principal`/
    // `interest` — the amount to move, both real, both trustworthy (no
    // dead/unpopulated-amount trap like Credit/Debit have).
    //
    // The one big thing that makes this type different from every other one
    // built so far in this module: **Authorize posts every entry's journal(s)
    // synchronously, inline, in the same call** — there is no
    // BrokerService.ProcessXBatchEntries async dispatch anywhere in
    // OverDeductionBatchAppService. Unlike Credit/Debit/WireTransfer/Reversal,
    // it's safe to assume every entry is `Posted` immediately after
    // `Authorize` returns `success: true` here. Consistent with that, there is
    // no PostEntry/queueable/single-entry-lookup surface on this app service at
    // all — nothing to browse or post individually, because nothing is ever
    // left half-posted.
    //
    // Per-entry posting logic (for context, not reproduced here — it's inside
    // AuthorizeOverDeductionBatch): Savings/Investment debit accounts get one
    // journal for principal+interest; Loan debit accounts get three separate
    // journals (interest receivable, interest received/charged reversal,
    // principal) to properly unwind a loan repayment's interest recognition.
    //
    // Update genuinely works here (copies TotalValue) but its return value is
    // worth knowing: it reflects whether entries' principal+interest now sums
    // to *exactly* TotalValue (equality, not "does not exceed" like Credit) —
    // `false` doesn't mean the update failed, it means the batch is out of
    // balance and not yet ready to audit.
    //
    // Deliberately not exposed, consistent with the rest of this module: CSV
    // import (ParseOverDeductionBatchImport exists on the interface but no
    // controller in this project has a file-upload pattern yet).
    [Authorize]
    [RoutePrefix("api/accounts/overdeductionbatches")]
    public class OverDeductionBatchController : ApiController
    {
        private readonly IOverDeductionBatchAppService _overDeductionBatchAppService;

        public OverDeductionBatchController(IOverDeductionBatchAppService overDeductionBatchAppService)
        {
            _overDeductionBatchAppService = overDeductionBatchAppService ?? throw new ArgumentNullException(nameof(overDeductionBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var batches = _overDeductionBatchAppService.FindOverDeductionBatches(serviceHeader);

                return Ok(ApiResponse("", batches ?? new List<OverDeductionBatchDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // No status-less paged overload exists on this app service — status is
        // a real, required filter here, same as Wire Transfer/Reversal.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _overDeductionBatchAppService.FindOverDeductionBatches(status, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

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

                var batch = _overDeductionBatchAppService.FindOverDeductionBatch(id, serviceHeader);

                if (batch == null)
                    return NotFound();

                return Ok(ApiResponse("", batch));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned server-side.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(OverDeductionBatchDTO overDeductionBatchDTO)
        {
            if (overDeductionBatchDTO == null)
                return ErrorResponse("Request body is required");

            overDeductionBatchDTO.ValidateAll();
            if (overDeductionBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", overDeductionBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _overDeductionBatchAppService.AddNewOverDeductionBatch(overDeductionBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the refund batch");

                return Ok(ApiResponse("Refund batch created successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // See class-level comment: a `false`/Conflict here can mean either the
        // batch wasn't found, or (more likely, once entries exist) that
        // entries' principal+interest doesn't sum to exactly TotalValue yet —
        // not necessarily a hard failure, just "not balanced".
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, OverDeductionBatchDTO overDeductionBatchDTO)
        {
            if (overDeductionBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                overDeductionBatchDTO.Id = id;

                var balanced = _overDeductionBatchAppService.UpdateOverDeductionBatch(overDeductionBatchDTO, serviceHeader);

                var refreshed = _overDeductionBatchAppService.FindOverDeductionBatch(id, serviceHeader);

                if (refreshed == null)
                    return NotFound();

                return Ok(ApiResponse(balanced ? "Operation success" : "Saved, but entries do not yet sum to the batch's total value", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Verify/Audit a Pending batch. BatchAuthOption: 1=Post (-> Audited), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] OverDeductionBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _overDeductionBatchAppService.AuditOverDeductionBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted; every
        // entry's journal(s) are posted synchronously, in this same call — see
        // class-level comment), 2=Reject. Refuses outright if the batch isn't
        // already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] OverDeductionBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _overDeductionBatchAppService.AuthorizeOverDeductionBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _overDeductionBatchAppService.FindOverDeductionBatchEntriesByOverDeductionBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, OverDeductionBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.OverDeductionBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _overDeductionBatchAppService.AddNewOverDeductionBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the refund batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<OverDeductionBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _overDeductionBatchAppService.RemoveOverDeductionBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private IHttpActionResult RunTransition(Guid id, OverDeductionBatchActionRequest request, Func<OverDeductionBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _overDeductionBatchAppService.FindOverDeductionBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _overDeductionBatchAppService.FindOverDeductionBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", updated));
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

    public class OverDeductionBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }
}
