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
    // Adapted from the reference MVC AddGeneralLedgerController — the plain
    // unified one (Create/Add/Remove/Edit/Verify/Authorize in one
    // controller), matching the shape JournalVoucherController.cs (this
    // project) adapted from its own plain reference. Second and last of
    // Group B — see BATCH-PROCEDURES-CONCEPTS.md §5 for the settled
    // Voucher-vs-General-Ledger distinction. Not to be confused with
    // GeneralLedgerStatementController (`api/accounts/statements/gl-account`)
    // — that's a read-only reporting view over IJournalEntryAppService, this
    // is the maker-checker-authorizer batch that actually posts entries.
    //
    // Confirmed by reading AuthorizeGeneralLedger directly (same discipline
    // as the Voucher correction — don't trust the DTO shape alone): each
    // entry really is a self-contained double-entry transfer, exactly as
    // GeneralLedgerEntryDTO's `chartOfAccountId`/`contraChartOfAccountId` +
    // optional `customerAccountId`/`contraCustomerAccountId` pairing
    // suggests. The one thing worth knowing that isn't obvious from the DTO:
    // **every entry posts as its own separate Journal** — unlike Voucher
    // (one shared journal, N legs) or Credit/Debit/WireTransfer (one journal
    // per entry too, but async). Here, on Authorize, the app service loops
    // entries and calls `PerformDoubleEntry` once per entry, building N
    // distinct Journals in one synchronous `BulkSave` call.
    //
    // Real things worth knowing:
    // - `AuthorizeGeneralLedger` posts synchronously, inline, same as
    //   Refund/Voucher — no async broker dispatch. Safe to assume `Posted`
    //   immediately after `Authorize` succeeds.
    // - Unlike every sibling type, an out-of-balance batch doesn't fail
    //   quietly here — `AuthorizeGeneralLedger` throws
    //   `InvalidOperationException` if entries' `amount` doesn't sum to
    //   exactly `totalValue`, instead of just returning `false`. Caught here
    //   and turned into the same `409` shape every other type in this module
    //   uses, so the client doesn't need to special-case this one.
    // - `GeneralLedgerDTO` carries no chart-of-account/customer-account
    //   fields of its own — unlike Voucher's header, General Ledger has no
    //   "primary" account. It's purely a container (branch, posting period,
    //   totalValue, remarks) for entries that are each already
    //   self-balancing.
    //
    // No `PostEntry`, no queueable browse, no single-entry lookup — a ledger
    // posts as one atomic unit on `Authorize`, same as Voucher/Refund.
    [Authorize]
    [RoutePrefix("api/accounts/generalledgers")]
    public class GeneralLedgerController : ApiController
    {
        private readonly IGeneralLedgerAppService _generalLedgerAppService;

        public GeneralLedgerController(IGeneralLedgerAppService generalLedgerAppService)
        {
            _generalLedgerAppService = generalLedgerAppService ?? throw new ArgumentNullException(nameof(generalLedgerAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var ledgers = _generalLedgerAppService.FindGeneralLedgers(serviceHeader);

                return Ok(ApiResponse("", ledgers ?? new List<GeneralLedgerDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // status is optional here (unlike Wire Transfer/Reversal/Refund/
        // Disbursement) — omit it and supply startDate/endDate for a
        // date-range search, or neither for a plain paged/text list. Same
        // four-overload dispatch as Journal Voucher.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int? status = null, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = status.HasValue
                    ? _generalLedgerAppService.FindGeneralLedgers(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : (startDate.HasValue || endDate.HasValue)
                        ? _generalLedgerAppService.FindGeneralLedgers(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                        : _generalLedgerAppService.FindGeneralLedgers(pageIndex, pageSize, serviceHeader);

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

                var ledger = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

                if (ledger == null)
                    return NotFound();

                return Ok(ApiResponse("", ledger));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Pending. LedgerNumber/Status/CreatedBy are all assigned server-side.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(GeneralLedgerDTO generalLedgerDTO)
        {
            if (generalLedgerDTO == null)
                return ErrorResponse("Request body is required");

            generalLedgerDTO.ValidateAll();
            if (generalLedgerDTO.HasErrors)
                return ErrorResponse(string.Join("; ", generalLedgerDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _generalLedgerAppService.AddNewGeneralLedger(generalLedgerDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the general ledger");

                return Ok(ApiResponse("General ledger created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Return value reflects whether entries now sum to exactly
        // totalValue (same "balanced, not success" semantics as Refund/
        // Journal Voucher's Update) — a false/non-error message here just
        // means not balanced yet, not that the save failed.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, GeneralLedgerDTO generalLedgerDTO)
        {
            if (generalLedgerDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                generalLedgerDTO.Id = id;

                var balanced = _generalLedgerAppService.UpdateGeneralLedger(generalLedgerDTO, serviceHeader);

                var refreshed = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

                if (refreshed == null)
                    return NotFound();

                return Ok(ApiResponse(balanced ? "Operation success" : "Saved, but entries do not yet sum to the ledger's total value", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify/Audit a Pending ledger. GeneralLedgerAuthOption: 1=Post
        // (-> Audited), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] GeneralLedgerActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _generalLedgerAppService.AuditGeneralLedger(dto, option, header));
        }

        // Authorize an Audited ledger. GeneralLedgerAuthOption: 1=Post
        // (-> Posted; posts synchronously in this same call, one Journal per
        // entry — see class-level comment), 2=Reject. Refuses outright if the
        // ledger isn't already Audited. Unlike every sibling type, an
        // out-of-balance Post throws server-side rather than just returning
        // false — caught here and normalized to the same 409 shape.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] GeneralLedgerActionRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                bool result;

                try
                {
                    result = _generalLedgerAppService.AuthorizeGeneralLedger(existing, request?.Option ?? 0, request?.ModuleNavigationItemCode ?? 0, serviceHeader);
                }
                catch (InvalidOperationException)
                {
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("The general ledger cannot be authorized until its entries balance to the batch total."));
                }

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Ledger is not in the right state for this action, or the action option is invalid"));

                var updated = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

                return Ok(ApiResponse("Operation success", updated));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByLedger(Guid id, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _generalLedgerAppService.FindGeneralLedgerEntriesByGeneralLedgerId(id, pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, GeneralLedgerEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.GeneralLedgerId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _generalLedgerAppService.AddNewGeneralLedgerEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the general ledger entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full replace: every existing entry is deleted and the given list
        // recreated in its place — same real full-replace semantics as
        // Journal Voucher's equivalent (no duplicate method here, unlike
        // Voucher's two identical ones).
        [HttpPut]
        [Route("{id:guid}/entries")]
        public IHttpActionResult ReplaceEntries(Guid id, List<GeneralLedgerEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var replaced = _generalLedgerAppService.UpdateGeneralLedgerEntryCollection(id, entries ?? new List<GeneralLedgerEntryDTO>(), serviceHeader);

                if (!replaced)
                    return NotFound();

                var refreshed = _generalLedgerAppService.FindGeneralLedgerEntriesByGeneralLedgerId(id, serviceHeader);

                return Ok(ApiResponse("Entries replaced successfully", refreshed ?? new List<GeneralLedgerEntryDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<GeneralLedgerEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _generalLedgerAppService.RemoveGeneralLedgerEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, GeneralLedgerActionRequest request, Func<GeneralLedgerDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Ledger is not in the right state for this action, or the action option is invalid"));

                var updated = _generalLedgerAppService.FindGeneralLedger(id, serviceHeader);

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

    public class GeneralLedgerActionRequest
    {
        // GeneralLedgerAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }
}
