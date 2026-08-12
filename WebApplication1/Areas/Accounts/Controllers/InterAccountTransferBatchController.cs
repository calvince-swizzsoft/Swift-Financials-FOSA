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
    // Adapted from the reference MVC InterAccountTransferController — the
    // plain unified one (Create/Verify/Authorize in one controller); the only
    // split reference controller for this type,
    // BatchVerification_InterAccountTransferController.cs, is an empty
    // scaffold (no real actions), so nothing was lost by not reading it as a
    // second source. Ninth and last of the "Batch Procedures" module — see
    // BATCH-PROCEDURES-CONCEPTS.md §1/§2.
    //
    // What this actually is: one **source** customer account (the header's
    // own `customerAccountId`) transfers its balance out to however many
    // **entries** you attach, each targeting *either* a customer account or a
    // raw G/L account (`apportionTo`: `1`=CustomerAccount, `2`=
    // GeneralLedgerAccount — genuinely consulted server-side, unlike
    // Voucher's equivalent-looking dead fields; `AddNewInterAccountTransferBatchEntry`
    // nulls out whichever of `chartOfAccountId`/`customerAccountId` doesn't
    // apply). Each entry carries its own `principal`+`interest` (the amount
    // to move to it) — no single shared journal or per-entry-own-journal
    // pattern like Voucher/General Ledger; here `AddNewJournal` is called
    // once per entry, synchronously, inline in `Authorize`, the same
    // high-level journal-posting entry point the rest of the front office
    // uses (not `BulkSave` the way Voucher/General Ledger/Refund batch their
    // journals).
    //
    // Real things worth knowing:
    // - **Fixed, not just documented — a real control-bypass bug**:
    //   `AuthorizeInterAccountTransferBatch` used to set
    //   `persisted.Status = Audited` *before* checking `persisted.Status !=
    //   Audited` (and before even null-checking `persisted`). That made the
    //   "must already be Audited" guard tautologically always true — a batch
    //   could be authorized (and its journals posted, real money moved)
    //   straight from `Pending`, skipping the Audit step entirely, and would
    //   throw a `NullReferenceException` instead of a clean `false` for an
    //   unknown id. Fixed to match every sibling type's guard: check first,
    //   don't mutate before checking.
    // - **No control-total validation exists anywhere for this type** —
    //   `AvailableBalance` on `InterAccountTransferBatchDTO` has no backing
    //   column at all (populated client-side only, in the reference app, by
    //   looking up the source account's real balance) and is never
    //   re-verified server-side. Nothing here stops entries' `principal +
    //   interest` from exceeding what the source account can actually cover,
    //   or from dipping it below its minimum balance — that check, if it's
    //   needed, has to happen client-side today. Flagged, not fixed —
    //   fixing it means adding real balance-checking business logic, a
    //   bigger change than this pass's scope.
    // - `DynamicCharge`s attached via `PUT /{id}/dynamiccharges` are real and
    //   consulted during posting (fed into the same `AddNewJournal` call as
    //   transfer-fee tariffs) — not decorative.
    // - `Authorize` posts synchronously, inline — same as Refund/Voucher/
    //   General Ledger, no async broker dispatch.
    // - `InterAccountTransferBatchDTO` has no `Priority` field at all, unlike
    //   every other batch type in this module.
    //
    // No `PostEntry`, no queueable browse, no CSV import (`ParseXImport`
    // doesn't exist on this interface at all).
    [Authorize]
    [RoutePrefix("api/accounts/interaccounttransferbatches")]
    public class InterAccountTransferBatchController : ApiController
    {
        private readonly IInterAccountTransferBatchAppService _interAccountTransferBatchAppService;

        public InterAccountTransferBatchController(IInterAccountTransferBatchAppService interAccountTransferBatchAppService)
        {
            _interAccountTransferBatchAppService = interAccountTransferBatchAppService ?? throw new ArgumentNullException(nameof(interAccountTransferBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var batches = _interAccountTransferBatchAppService.FindInterAccountTransferBatches(serviceHeader);

                return Ok(ApiResponse("", batches ?? new List<InterAccountTransferBatchDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // status is optional here (same four-overload dispatch as Journal
        // Voucher/General Ledger) — omit it and supply startDate/endDate for
        // a date-range search, or neither for a plain paged list.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int? status = null, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = status.HasValue
                    ? _interAccountTransferBatchAppService.FindInterAccountTransferBatches(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : (startDate.HasValue || endDate.HasValue)
                        ? _interAccountTransferBatchAppService.FindInterAccountTransferBatches(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                        : _interAccountTransferBatchAppService.FindInterAccountTransferBatches(pageIndex, pageSize, serviceHeader);

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

                var batch = _interAccountTransferBatchAppService.FindInterAccountTransferBatch(id, serviceHeader);

                if (batch == null)
                    return NotFound();

                return Ok(ApiResponse("", batch));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned
        // server-side. Only branchId/customerAccountId/reference are actually
        // persisted — everything else on the DTO (availableBalance,
        // startDate/endDate, denormalized customer fields) is display-only.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(InterAccountTransferBatchDTO interAccountTransferBatchDTO)
        {
            if (interAccountTransferBatchDTO == null)
                return ErrorResponse("Request body is required");

            interAccountTransferBatchDTO.ValidateAll();
            if (interAccountTransferBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", interAccountTransferBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _interAccountTransferBatchAppService.AddNewInterAccountTransferBatch(interAccountTransferBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the inter account transfer batch");

                return Ok(ApiResponse("Inter account transfer batch created successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Only updates reference — branchId/customerAccountId are immutable
        // post-creation, same as every sibling.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, InterAccountTransferBatchDTO interAccountTransferBatchDTO)
        {
            if (interAccountTransferBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                interAccountTransferBatchDTO.Id = id;

                var updated = _interAccountTransferBatchAppService.UpdateInterAccountTransferBatch(interAccountTransferBatchDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _interAccountTransferBatchAppService.FindInterAccountTransferBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Verify/Audit a Pending batch. BatchAuthOption: 1=Post (-> Audited), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] InterAccountTransferBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _interAccountTransferBatchAppService.AuditInterAccountTransferBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted;
        // posts synchronously in this same call, one Journal per entry — see
        // class-level comment), 2=Reject. Refuses outright if the batch isn't
        // already Audited (see the fixed bug noted at class level).
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] InterAccountTransferBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _interAccountTransferBatchAppService.AuthorizeInterAccountTransferBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _interAccountTransferBatchAppService.FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, InterAccountTransferBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.InterAccountTransferBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _interAccountTransferBatchAppService.AddNewInterAccountTransferBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the inter account transfer batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Full replace: every existing entry is deleted and the given list
        // recreated in its place — same real full-replace semantics as
        // General Ledger's equivalent.
        [HttpPut]
        [Route("{id:guid}/entries")]
        public IHttpActionResult ReplaceEntries(Guid id, List<InterAccountTransferBatchEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var replaced = _interAccountTransferBatchAppService.UpdateInterAccountTransferBatchEntryCollection(id, entries ?? new List<InterAccountTransferBatchEntryDTO>(), serviceHeader);

                if (!replaced)
                    return NotFound();

                var refreshed = _interAccountTransferBatchAppService.FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(id, serviceHeader);

                return Ok(ApiResponse("Entries replaced successfully", refreshed ?? new List<InterAccountTransferBatchEntryDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<InterAccountTransferBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _interAccountTransferBatchAppService.RemoveInterAccountTransferBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Transfer-fee DynamicCharges attached to this batch — fed into the
        // per-entry journal posting at Authorize time (real, not decorative).
        [HttpGet]
        [Route("{id:guid}/dynamiccharges")]
        public IHttpActionResult GetDynamicCharges(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var charges = _interAccountTransferBatchAppService.FindDynamicCharges(id, serviceHeader);

                return Ok(ApiResponse("", charges ?? new List<DynamicChargeDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Full replace of the attached DynamicCharge set (id references only).
        [HttpPut]
        [Route("{id:guid}/dynamiccharges")]
        public IHttpActionResult ReplaceDynamicCharges(Guid id, List<DynamicChargeDTO> dynamicCharges)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var replaced = _interAccountTransferBatchAppService.UpdateDynamicCharges(id, dynamicCharges ?? new List<DynamicChargeDTO>(), serviceHeader);

                if (!replaced)
                    return NotFound();

                var refreshed = _interAccountTransferBatchAppService.FindDynamicCharges(id, serviceHeader);

                return Ok(ApiResponse("Dynamic charges replaced successfully", refreshed ?? new List<DynamicChargeDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private IHttpActionResult RunTransition(Guid id, InterAccountTransferBatchActionRequest request, Func<InterAccountTransferBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _interAccountTransferBatchAppService.FindInterAccountTransferBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _interAccountTransferBatchAppService.FindInterAccountTransferBatch(id, serviceHeader);

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

    public class InterAccountTransferBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }
}
