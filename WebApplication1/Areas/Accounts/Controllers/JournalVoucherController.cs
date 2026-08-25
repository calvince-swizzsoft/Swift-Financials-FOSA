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
    // Adapted from the reference MVC JournalVoucherController — the plain
    // unified one (Create/Edit/Verify/Authorize in one controller), not the
    // separately-split BatchOrigination_Voucher/BatchVerification_Voucher/
    // BatchAuthorization_Voucher screens — see BATCH-PROCEDURES-CONCEPTS.md §1
    // for why those three are a menu/role-routing artifact, not three
    // controllers here.
    //
    // What a Voucher actually posts as, confirmed by reading
    // AuthorizeJournalVoucher directly (not assumed from the DTO shape — see
    // the correction note in BATCH-PROCEDURES-CONCEPTS.md §5): it is **not** a
    // free-form N-line journal where each line independently chooses debit or
    // credit. It's one primary account (the header's own `chartOfAccountId` +
    // optional `customerAccountId`, at `totalValue`) on one side, and however
    // many entries you attach — each its own `chartOfAccountId` + optional
    // `customerAccountId` + own `amount` — collectively on the other side.
    // The header's single `type` (`JournalVoucherType`: 0=DebitGLAccount,
    // 1=CreditGLAccount, 2=DebitCustomerAccount, 3=CreditCustomerAccount) sets
    // the direction for *both* the header leg and every entry leg — there is
    // no per-entry direction control. Each `JournalVoucherEntryDTO` does carry
    // its own `type`/`entryType` fields, but they are **never read** by
    // `JournalVoucherAppService` anywhere (confirmed: `AddNewJournalVoucherEntry`'s
    // factory call doesn't even take them as parameters) — decorative DTO
    // fields only, don't build a per-entry debit/credit picker against them.
    //
    // Real things worth knowing:
    // - `AuthorizeJournalVoucher` posts synchronously, inline, in the same
    //   call — same as Refund, no async message-broker dispatch. Safe to
    //   assume `Posted` immediately after `Authorize` succeeds.
    // - It only posts at all if entries' `amount` sums to *exactly*
    //   `totalValue` (checked right before posting) — same equality
    //   requirement `Update`'s boolean return reflects (see below).
    // - `IJournalVoucherAppService` exposes two bulk-entry-replace methods,
    //   `UpdateJournalVoucherEntryCollection` and `UpdateJournalVoucherEntries`
    //   — read both; they are genuine duplicates, identical delete-then-recreate
    //   logic under two names. Only `UpdateJournalVoucherEntryCollection` is
    //   exposed here (matches what the reference controller actually called).
    //   Unlike Journal Reversal/Loan Disbursement Batch's insert-only bulk
    //   methods, this one is a real full replace.
    // - Fixed, not just documented: `AddNewJournalVoucher`'s out-of-range
    //   `ValueDate` guard used to set `ErrorMessageResult` via
    //   `string.Format("ValueDate", "Sorry, but value date is out of range!")`
    //   — with no `{0}` placeholder in the format string, that call just
    //   returned the literal text `"ValueDate"`, silently discarding the real
    //   message. Fixed to assign the real message directly.
    //
    // No `PostEntry`, no queueable browse, no single-entry lookup — consistent
    // with everything above: a voucher posts as one atomic unit on Authorize,
    // there's nothing to post or browse individually.
    [Authorize]
    [RoutePrefix("api/accounts/journalvouchers")]
    public class JournalVoucherController : ApiController
    {
        private readonly IJournalVoucherAppService _journalVoucherAppService;

        public JournalVoucherController(IJournalVoucherAppService journalVoucherAppService)
        {
            _journalVoucherAppService = journalVoucherAppService ?? throw new ArgumentNullException(nameof(journalVoucherAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var vouchers = _journalVoucherAppService.FindJournalVouchers(serviceHeader);

                return Ok(ApiResponse("", vouchers ?? new List<JournalVoucherDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int? status = null, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = status.HasValue
                    ? _journalVoucherAppService.FindJournalVouchers(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : (startDate.HasValue || endDate.HasValue)
                        ? _journalVoucherAppService.FindJournalVouchers(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                        : string.IsNullOrWhiteSpace(text)
                            ? _journalVoucherAppService.FindJournalVouchers(pageIndex, pageSize, serviceHeader)
                            : _journalVoucherAppService.FindJournalVouchers(text, pageIndex, pageSize, serviceHeader);

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

                var voucher = _journalVoucherAppService.FindJournalVoucher(id, serviceHeader);

                if (voucher == null)
                    return NotFound();

                return Ok(ApiResponse("", voucher));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Pending. VoucherNumber/Status/CreatedBy are all assigned
        // server-side. Fails with 400 if postingPeriodId doesn't resolve to a
        // real posting period, or if valueDate falls outside it (or is in the
        // future) — the latter comes back with data: the voucher (echoing what
        // you sent) rather than null; check errorMessageResult on it.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(JournalVoucherDTO journalVoucherDTO)
        {
            if (journalVoucherDTO == null)
                return ErrorResponse("Request body is required");

            journalVoucherDTO.ValidateAll();
            if (journalVoucherDTO.HasErrors)
                return ErrorResponse(string.Join("; ", journalVoucherDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _journalVoucherAppService.AddNewJournalVoucher(journalVoucherDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the journal voucher — the posting period could not be resolved");

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return ErrorResponse(created.ErrorMessageResult);

                return Ok(ApiResponse("Journal voucher created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Return value reflects whether entries now sum to exactly
        // totalValue (same "balanced, not success" semantics as Refund's
        // Update) — a false/non-error message here just means not balanced
        // yet, not that the save failed.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, JournalVoucherDTO journalVoucherDTO)
        {
            if (journalVoucherDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                journalVoucherDTO.Id = id;

                var balanced = _journalVoucherAppService.UpdateJournalVoucher(journalVoucherDTO, serviceHeader);

                var refreshed = _journalVoucherAppService.FindJournalVoucher(id, serviceHeader);

                if (refreshed == null)
                    return NotFound();

                return Ok(ApiResponse(balanced ? "Operation success" : "Saved, but entries do not yet sum to the voucher's total value", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify/Audit a Pending voucher. JournalVoucherAuthOption: 1=Post
        // (-> Audited), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] JournalVoucherActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _journalVoucherAppService.AuditJournalVoucher(dto, option, header));
        }

        // Authorize an Audited voucher. JournalVoucherAuthOption: 1=Post
        // (-> Posted; posts synchronously in this same call, see class-level
        // comment — only if entries sum to exactly totalValue), 2=Reject.
        // Refuses outright if the voucher isn't already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] JournalVoucherActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _journalVoucherAppService.AuthorizeJournalVoucher(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByVoucher(Guid id, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _journalVoucherAppService.FindJournalVoucherEntriesByJournalVoucherId(id, pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, JournalVoucherEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.JournalVoucherId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _journalVoucherAppService.AddNewJournalVoucherEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the journal voucher entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full replace: every existing entry is deleted and the given list
        // recreated in its place — unlike Journal Reversal/Loan Disbursement
        // Batch's insert-only bulk methods, this one really does replace the
        // whole collection.
        [HttpPut]
        [Route("{id:guid}/entries")]
        public IHttpActionResult ReplaceEntries(Guid id, List<JournalVoucherEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var replaced = _journalVoucherAppService.UpdateJournalVoucherEntryCollection(id, entries ?? new List<JournalVoucherEntryDTO>(), serviceHeader);

                if (!replaced)
                    return NotFound();

                var refreshed = _journalVoucherAppService.FindJournalVoucherEntriesByJournalVoucherId(id, serviceHeader);

                return Ok(ApiResponse("Entries replaced successfully", refreshed ?? new List<JournalVoucherEntryDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<JournalVoucherEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _journalVoucherAppService.RemoveJournalVoucherEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, JournalVoucherActionRequest request, Func<JournalVoucherDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _journalVoucherAppService.FindJournalVoucher(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Voucher is not in the right state for this action, entries don't sum to the total value, or the action option is invalid"));

                var updated = _journalVoucherAppService.FindJournalVoucher(id, serviceHeader);

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

    public class JournalVoucherActionRequest
    {
        // JournalVoucherAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }
}
