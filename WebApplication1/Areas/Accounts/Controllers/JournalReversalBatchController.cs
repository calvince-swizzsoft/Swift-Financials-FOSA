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
    // Adapted from the reference MVC BatchOrigination_ReversalController — the
    // only reference controller for this type; there is no
    // BatchVerification_Reversal/BatchAuthorization_Reversal despite
    // NavigationMenu.cs listing menu entries for both (dead nav links — the
    // single reference controller already covers Create/Verify/Authorize
    // itself, and is largely copy-pasted from the Disbursement/WireTransfer
    // controllers with the copy-paste leftovers still commented out in source).
    // That reference controller has no entry-adding UI at all, so the entry
    // shape/mechanics below come from reading JournalReversalBatchAppService
    // directly, not from the (incomplete) reference screen.
    //
    // What this type actually is: pick one or more already-posted Journals and
    // reverse them in a batch, under the same maker-checker-authorizer control
    // as every other type here — see BATCH-PROCEDURES-CONCEPTS.md. An entry is
    // just { JournalId, Remarks } — no amount, no tariffs; the amount to
    // reverse is implicitly the referenced Journal's own amount.
    //
    // Real things worth knowing:
    // - No TotalValue/control-total on this type (like Debit) — nothing to
    //   validate client-side against a declared batch total.
    // - JournalReversalBatchDTO.Remarks2 is [Required] by validation but has no
    //   backing column on the domain entity at all — a dead field. Send
    //   something to pass validation; don't expect it to persist or come back.
    // - UpdateJournalReversalBatch used to be a silent no-op in
    //   JournalReversalBatchAppService — it fetched the batch and saved without
    //   ever copying the DTO's fields onto it. Fixed (now persists Remarks/
    //   Priority, matching every sibling Update method) rather than ported
    //   forward as a trap.
    // - AuthorizeJournalReversalBatch strictly requires the batch to already be
    //   Audited (like Debit/WireTransfer, unlike Credit).
    // - Authorize queues every entry for async posting with no type filter
    //   (like Debit/WireTransfer — there's no sub-type here to filter on).
    // - Posting an entry (PostJournalReversalBatchEntry) calls the existing
    //   IJournalAppService.ReverseJournals for the one referenced Journal — no
    //   balance checks, no partial processing, no rejection path; it either
    //   reverses cleanly or the call fails outright.
    //
    // Deliberately not exposed, consistent with the rest of this module: CSV
    // import (there is no ParseJournalReversalBatchImport on this interface at
    // all — unlike Credit/Debit/WireTransfer, this type never had one).
    [Authorize]
    [RoutePrefix("api/accounts/journalreversalbatches")]
    public class JournalReversalBatchController : ApiController
    {
        private readonly IJournalReversalBatchAppService _journalReversalBatchAppService;

        public JournalReversalBatchController(IJournalReversalBatchAppService journalReversalBatchAppService)
        {
            _journalReversalBatchAppService = journalReversalBatchAppService ?? throw new ArgumentNullException(nameof(journalReversalBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var batches = _journalReversalBatchAppService.FindJournalReversalBatches(serviceHeader);

                return Ok(ApiResponse("", batches ?? new List<JournalReversalBatchDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // No status-less paged overload exists on this app service — status is
        // a real, required filter here, same as Wire Transfer.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _journalReversalBatchAppService.FindJournalReversalBatches(status, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
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

                var batch = _journalReversalBatchAppService.FindJournalReversalBatch(id, serviceHeader);

                if (batch == null)
                    return NotFound();

                return Ok(ApiResponse("", batch));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned server-side.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(JournalReversalBatchDTO journalReversalBatchDTO)
        {
            if (journalReversalBatchDTO == null)
                return ErrorResponse("Request body is required");

            journalReversalBatchDTO.ValidateAll();
            if (journalReversalBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", journalReversalBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _journalReversalBatchAppService.AddNewJournalReversalBatch(journalReversalBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the journal reversal batch");

                return Ok(ApiResponse("Journal reversal batch created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, JournalReversalBatchDTO journalReversalBatchDTO)
        {
            if (journalReversalBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                journalReversalBatchDTO.Id = id;

                var updated = _journalReversalBatchAppService.UpdateJournalReversalBatch(journalReversalBatchDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _journalReversalBatchAppService.FindJournalReversalBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify/Audit a Pending batch. BatchAuthOption: 1=Post (-> Audited), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] JournalReversalBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _journalReversalBatchAppService.AuditJournalReversalBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted; every
        // entry is queued for async posting — see class-level comment), 2=Reject.
        // Refuses outright if the batch isn't already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] JournalReversalBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _journalReversalBatchAppService.AuthorizeJournalReversalBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _journalReversalBatchAppService.FindJournalReversalBatchEntriesByJournalReversalBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Preview of the actual G/L lines (JournalEntryDTO, not batch entries)
        // that belong to every Journal this batch's entries point at — useful
        // for showing "here's exactly what will be reversed" before authorizing.
        [HttpGet]
        [Route("{id:guid}/journal-entries")]
        public IHttpActionResult GetJournalEntries(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _journalReversalBatchAppService.FindJournalEntriesByJournalReversalBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Entries ready to post, across all batches — no type restriction.
        [HttpGet]
        [Route("entries/queueable")]
        public IHttpActionResult GetQueueable(int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _journalReversalBatchAppService.FindQueableJournalReversalBatchEntries(pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
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

                var entry = _journalReversalBatchAppService.FindJournalReversalBatchEntry(entryId, serviceHeader);

                if (entry == null)
                    return NotFound();

                return Ok(ApiResponse("", entry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Pick a single Journal to reverse and attach it to this batch.
        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, JournalReversalBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.JournalReversalBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _journalReversalBatchAppService.AddNewJournalReversalBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the journal reversal batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Bulk-add convenience: diffs the given list against the batch's
        // existing entries by JournalId and inserts whatever's new. This is
        // insert-only — entries present now but missing from the list you send
        // are NOT removed, despite what a "replace the collection" name might
        // suggest; use entries/remove for that.
        [HttpPost]
        [Route("{id:guid}/entries/bulk")]
        public IHttpActionResult AddEntries(Guid id, List<JournalReversalBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var added = _journalReversalBatchAppService.UpdateJournalReversalBatchEntries(id, entries, serviceHeader);

                if (!added)
                    return ErrorResponse("Failed to add the journal reversal batch entries");

                return Ok(ApiResponse("Entries added successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<JournalReversalBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _journalReversalBatchAppService.RemoveJournalReversalBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Reverses the entry's referenced Journal via IJournalAppService.ReverseJournals.
        [HttpPost]
        [Route("entries/{entryId:guid}/post")]
        public IHttpActionResult PostEntry(Guid entryId, [FromBody] PostJournalReversalBatchEntryRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var posted = _journalReversalBatchAppService.PostJournalReversalBatchEntry(entryId, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be posted (already posted, or its referenced journal could not be resolved)"));

                var refreshed = _journalReversalBatchAppService.FindJournalReversalBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Entry posted successfully", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, JournalReversalBatchActionRequest request, Func<JournalReversalBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _journalReversalBatchAppService.FindJournalReversalBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _journalReversalBatchAppService.FindJournalReversalBatch(id, serviceHeader);

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
            return Content(HttpStatusCode.BadRequest, ErrorEnvelope(message));
        }
    }

    public class JournalReversalBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }

    public class PostJournalReversalBatchEntryRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
