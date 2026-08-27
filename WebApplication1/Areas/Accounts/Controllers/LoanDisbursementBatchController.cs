using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC BatchOrigination_Disbursement /
    // BatchVerification_Disbursement / BatchAuthorization_Disbursement
    // controllers — folded into one, same reasoning as every other type in
    // this module (BATCH-PROCEDURES-CONCEPTS.md §1). The app service lives in
    // BackOfficeModule (ILoanDisbursementBatchAppService), but this controller
    // stays under Areas/Accounts, matching where the reference screens live.
    //
    // Two things in the reference controllers deliberately NOT ported:
    // - All three reference controllers hand-roll raw ADO.NET SQL
    //   (`UpdateLoanCaseBatchNumber`) directly against the `swiftFin_LoanCases`
    //   table to stamp a loan case's batch number, bypassing the domain layer
    //   entirely. Not needed — the real app service already does this
    //   correctly: AddNewLoanDisbursementBatchEntry/UpdateLoanDisbursementBatchEntries
    //   flip LoanCase.IsBatched/BatchNumber/BatchedBy through the domain layer,
    //   and refuse a loan case that's already batched.
    // - BatchAuthorization_Disbursement's Authorize action, after calling the
    //   real Authorize, loops every entry in the MVC controller itself and (a)
    //   sends an SMS, (b) calls an MPESA B2C helper with a phone number
    //   variable that is declared, never assigned, and passed in as `""`, and
    //   (c) sets `.Status = Disbursed` on a local in-memory LoanCaseDTO that's
    //   never saved. None of that is trustworthy or real — it's dead/broken
    //   code living in the presentation layer, not domain logic. Real
    //   disbursement work happens entirely server-side, in
    //   PostLoanDisbursementBatchEntry (see below). If real SMS/MPESA
    //   notification on disbursement is actually wanted, that needs a product
    //   decision and a real implementation — not a port of this.
    //
    // What Authorize actually does here: same shape as Debit/WireTransfer/
    // Reversal — requires the batch to already be Audited, and on Post queues
    // every entry onto an async message broker
    // (BrokerService.ProcessLoanDisbursementBatchEntries) with no type filter,
    // despite DisbursementType (Normal/Express/Waiver) existing as a header
    // field. Posting is not synchronous with Authorize.
    //
    // What posting one entry (PostLoanDisbursementBatchEntry) actually does —
    // for context, not reproduced here, it's substantial: resolves (creating
    // if missing) the customer's loan and savings accounts, posts the
    // disbursement journal moving approved principal from the loan account to
    // the savings account, recovers any upfront dynamic charges configured on
    // the loan product, marks the LoanCase Disbursed, and creates/updates a
    // StandingOrder for the loan's repayment schedule. This is real,
    // business-critical domain logic — treat it as a black box from the API's
    // perspective, don't try to precompute or second-guess its result
    // client-side.
    //
    // Deliberately not exposed:
    // - DisburseMicroLoan — a separate real-time/alternate-channel (USSD/API)
    //   disbursement path (its `alternateChannelLogId` parameter gives it
    //   away), unrelated to this batch screen. Needs its own product decision
    //   if/when an alternate-channel disbursement API is wanted.
    // - CSV import — there is no ParseLoanDisbursementBatchImport on this
    //   interface at all, unlike Credit/Debit/WireTransfer (nothing was
    //   excluded here, there was never anything to expose).
    [Authorize]
    [RoutePrefix("api/accounts/loandisbursementbatches")]
    public class LoanDisbursementBatchController : ApiController
    {
        private readonly ILoanDisbursementBatchAppService _loanDisbursementBatchAppService;

        public LoanDisbursementBatchController(ILoanDisbursementBatchAppService loanDisbursementBatchAppService)
        {
            _loanDisbursementBatchAppService = loanDisbursementBatchAppService ?? throw new ArgumentNullException(nameof(loanDisbursementBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var batches = _loanDisbursementBatchAppService.FindLoanDisbursementBatches(serviceHeader);

                return Ok(ApiResponse("", batches ?? new List<LoanDisbursementBatchDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // No status-less paged overload exists on this app service — status is
        // a real, required filter here, same as Wire Transfer/Reversal/Refund.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanDisbursementBatchAppService.FindLoanDisbursementBatches(status, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

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

                var batch = _loanDisbursementBatchAppService.FindLoanDisbursementBatch(id, serviceHeader);

                if (batch == null)
                    return NotFound();

                return Ok(ApiResponse("", batch));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned
        // server-side. Note: `batchTotal`/`startDate`/`endDate` on the DTO have
        // no backing column on the domain entity at all — send whatever you
        // like, but don't expect them to persist or come back on a GET.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LoanDisbursementBatchDTO loanDisbursementBatchDTO)
        {
            if (loanDisbursementBatchDTO == null)
                return ErrorResponse("Request body is required");

            loanDisbursementBatchDTO.ValidateAll();
            if (loanDisbursementBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", loanDisbursementBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _loanDisbursementBatchAppService.AddNewLoanDisbursementBatch(loanDisbursementBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the loan disbursement batch");

                return Ok(ApiResponse("Loan disbursement batch created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Only updates reference/priority — branchId/type/loanProductCategory
        // are immutable post-creation, same as every sibling.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, LoanDisbursementBatchDTO loanDisbursementBatchDTO)
        {
            if (loanDisbursementBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                loanDisbursementBatchDTO.Id = id;

                var updated = _loanDisbursementBatchAppService.UpdateLoanDisbursementBatch(loanDisbursementBatchDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _loanDisbursementBatchAppService.FindLoanDisbursementBatch(id, serviceHeader);

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
        public IHttpActionResult Audit(Guid id, [FromBody] LoanDisbursementBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _loanDisbursementBatchAppService.AuditLoanDisbursementBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted; every
        // entry is queued for async posting — see class-level comment), 2=Reject.
        // Refuses outright if the batch isn't already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] LoanDisbursementBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _loanDisbursementBatchAppService.AuthorizeLoanDisbursementBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        // Whether any entry in this batch exceeds the transaction threshold
        // configured for the given Designation (role) — an AML/compliance
        // pre-authorization check, not a hard gate enforced elsewhere in this
        // flow. `transactionThresholdType` matches whatever
        // TransactionThresholdType values IDesignationAppService's threshold
        // collection uses.
        [HttpGet]
        [Route("{id:guid}/exceeds-threshold")]
        public IHttpActionResult ExceedsThreshold(Guid id, Guid designationId, int transactionThresholdType)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var exceeds = _loanDisbursementBatchAppService.ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(id, designationId, transactionThresholdType, serviceHeader);

                return Ok(ApiResponse("", exceeds));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Entries across all batches of a DisbursementType (1=Normal,
        // 2=Express, 4=Waiver) — cross-batch browse, same shape as Credit's
        // entries/type/{creditBatchType}.
        [HttpGet]
        [Route("entries/type/{disbursementType:int}")]
        public IHttpActionResult GetByType(int disbursementType, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByLoanDisbursementBatchType(disbursementType, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("entries/customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomer(Guid customerId, int disbursementType)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByCustomerId(disbursementType, customerId, serviceHeader);

                return Ok(ApiResponse("", entries ?? new List<LoanDisbursementBatchEntryDTO>()));
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

                var page = _loanDisbursementBatchAppService.FindQueableLoanDisbursementBatchEntries(pageIndex, pageSize, serviceHeader);

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

                var entry = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntry(entryId, serviceHeader);

                if (entry == null)
                    return NotFound();

                return Ok(ApiResponse("", entry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Picks one Audited, not-yet-batched LoanCase (browse those via the
        // existing loan-case endpoints, filtered client-side to
        // LoanCaseStatus.Audited && !isBatched) and attaches it to this batch.
        // Refuses (500, InvalidOperationException) if the loan case is already
        // batched elsewhere.
        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, LoanDisbursementBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.LoanDisbursementBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _loanDisbursementBatchAppService.AddNewLoanDisbursementBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the loan disbursement batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Bulk-add convenience: diffs the given list against the batch's
        // existing entries by LoanCaseId and inserts whatever's new — same
        // insert-only caveat as Journal Reversal Batch's equivalent (entries
        // missing from the list are NOT removed). Also silently skips any
        // LoanCase already batched, or whose LoanProductCategory doesn't match
        // this batch's own category.
        [HttpPost]
        [Route("{id:guid}/entries/bulk")]
        public IHttpActionResult AddEntries(Guid id, List<LoanDisbursementBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var added = _loanDisbursementBatchAppService.UpdateLoanDisbursementBatchEntries(id, entries, serviceHeader);

                if (!added)
                    return ErrorResponse("Failed to add the loan disbursement batch entries");

                return Ok(ApiResponse("Entries added successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Status can only move forward (Pending -> Posted/Rejected) — enforced
        // by UpdateLoanDisbursementBatchEntry itself, not re-validated here.
        [HttpPut]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult UpdateEntry(Guid entryId, LoanDisbursementBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.Id = entryId;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanDisbursementBatchAppService.UpdateLoanDisbursementBatchEntry(entryDTO, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry not found, or the status change is not forward-only"));

                var refreshed = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Removing an entry also un-flags the underlying LoanCase
        // (isBatched=false, batchNumber=0) so it becomes eligible to be picked
        // into a different batch again.
        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<LoanDisbursementBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _loanDisbursementBatchAppService.RemoveLoanDisbursementBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Disburses one entry — see the class-level comment for what actually
        // happens (customer account resolution, disbursement journal, upfront
        // dynamic charges, standing order setup).
        [HttpPost]
        [Route("entries/{entryId:guid}/post")]
        public IHttpActionResult PostEntry(Guid entryId, [FromBody] PostLoanDisbursementBatchEntryRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var posted = _loanDisbursementBatchAppService.PostLoanDisbursementBatchEntry(entryId, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be posted (already posted, or its batch/posting period/loan case could not be resolved)"));

                var refreshed = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Entry posted successfully", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, LoanDisbursementBatchActionRequest request, Func<LoanDisbursementBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanDisbursementBatchAppService.FindLoanDisbursementBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _loanDisbursementBatchAppService.FindLoanDisbursementBatch(id, serviceHeader);

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

    public class LoanDisbursementBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }

    public class PostLoanDisbursementBatchEntryRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
