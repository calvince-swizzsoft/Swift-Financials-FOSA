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
    // Adapted from the reference MVC CreditBatchController (Areas/Accounts) — that
    // controller only covered the batch header lifecycle (Create/Edit/Verify/
    // Authorize/Index) through the monolithic _channelService; entry browsing for
    // a pickup queue (what the FrontOffice "Sundry Receipts/Payments -> Cash
    // Pickup" screen needs) was never exposed anywhere in the reference app as its
    // own endpoint — the reference SundryPaymentsController had its own private
    // FetchCreditBatchEntriesTable action instead. This controller exposes
    // ICreditBatchAppService directly (batch CRUD/audit/authorize + entry
    // CRUD/browse/post) rather than reproducing either shape.
    //
    // Deliberately not ported/exposed:
    // - CreditBatchDiscrepancy browsing/matching (FindCreditBatchDiscrepancies*,
    //   MatchCreditBatchDiscrepancy* overloads) — a separate Payout/CheckOff
    //   reconciliation workflow, not needed to unblock Cash Pickup.
    // - ParseCreditBatchImport (CSV batch import) — needs file-upload handling
    //   the rest of this project doesn't have a pattern for yet.
    // Both can be added later behind their own routes without touching what's here.
    //
    // IMPORTANT for the entry-browsing endpoints (GetByType, GetByBatch): the
    // returned CreditBatchEntryDTO.Amount is NOT populated — CreditBatchEntry has
    // no Amount column on the domain entity, so AutoMapper's convention-based
    // projection always leaves it at 0 (this is a preexisting gap in the domain
    // model, not something introduced here). Use Principal + Interest as the pay
    // amount instead (Interest is 0 for CashPickup/SundryPayments batches, so in
    // practice that's just Principal), and CreditBatchCreditTypeChartOfAccountId
    // as the GL account — see docs/api/frontoffice-api-spec.md §13.2.
    [Authorize]
    [RoutePrefix("api/accounts/creditbatches")]
    public class CreditBatchController : ApiController
    {
        private readonly ICreditBatchAppService _creditBatchAppService;

        public CreditBatchController(ICreditBatchAppService creditBatchAppService)
        {
            _creditBatchAppService = creditBatchAppService ?? throw new ArgumentNullException(nameof(creditBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var creditBatches = _creditBatchAppService.FindCreditBatches(serviceHeader);

                return Ok(ApiResponse("", creditBatches ?? new List<CreditBatchDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
                    ? _creditBatchAppService.FindCreditBatches(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : string.IsNullOrWhiteSpace(text)
                        ? _creditBatchAppService.FindCreditBatches(pageIndex, pageSize, serviceHeader)
                        : _creditBatchAppService.FindCreditBatches(text, pageIndex, pageSize, serviceHeader);

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

                var creditBatch = _creditBatchAppService.FindCreditBatch(id, serviceHeader);

                if (creditBatch == null)
                    return NotFound();

                return Ok(ApiResponse("", creditBatch));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned server-side.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreditBatchDTO creditBatchDTO)
        {
            if (creditBatchDTO == null)
                return ErrorResponse("Request body is required");

            creditBatchDTO.ValidateAll();
            if (creditBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", creditBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _creditBatchAppService.AddNewCreditBatch(creditBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the credit batch");

                return Ok(ApiResponse("Credit batch created successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Only touches the batch's own fields (TotalValue, concession, recovery
        // flags, reference, priority, value date) — same shape UpdateCreditBatch
        // itself supports. Entries have their own sub-resource endpoints below.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, CreditBatchDTO creditBatchDTO)
        {
            if (creditBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                creditBatchDTO.Id = id;

                var updated = _creditBatchAppService.UpdateCreditBatch(creditBatchDTO, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch not found, or entries total exceeds the batch's total value"));

                var refreshed = _creditBatchAppService.FindCreditBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Verify/Audit a Pending batch. BatchAuthOption: 1=Post (-> Audited, only if
        // entries total <= batch TotalValue), 2=Reject (-> Rejected).
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] CreditBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _creditBatchAppService.AuditCreditBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted, entries
        // become eligible for pickup/payout), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] CreditBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _creditBatchAppService.AuthorizeCreditBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchId(id, text ?? "", filter, pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // The Cash Pickup (and Sundry Payments batch) picker: browse entries for a
        // given CreditBatchType. Not filtered by entry status server-side — the
        // underlying spec only filters by date range/type/text — so the client must
        // filter for status == 1 (Pending) to show only entries not yet paid out.
        [HttpGet]
        [Route("entries/type/{creditBatchType:int}")]
        public IHttpActionResult GetByType(int creditBatchType, DateTime? startDate = null, DateTime? endDate = null, string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchType(
                    creditBatchType,
                    startDate ?? DateTime.MinValue,
                    endDate ?? DateTime.MaxValue,
                    text ?? "",
                    filter,
                    pageIndex,
                    pageSize,
                    serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("entries/customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomer(Guid customerId, int creditBatchType)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _creditBatchAppService.FindCreditBatchEntriesByCustomerId(creditBatchType, customerId, serviceHeader);

                return Ok(ApiResponse("", entries ?? new List<CreditBatchEntryDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult GetEntry(Guid entryId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entry = _creditBatchAppService.FindCreditBatchEntry(entryId, serviceHeader);

                if (entry == null)
                    return NotFound();

                return Ok(ApiResponse("", entry));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, CreditBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.CreditBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _creditBatchAppService.AddNewCreditBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the credit batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Status can only move forward (Pending -> Posted/Rejected) — enforced by
        // UpdateCreditBatchEntry itself, not re-validated here.
        [HttpPut]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult UpdateEntry(Guid entryId, CreditBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.Id = entryId;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _creditBatchAppService.UpdateCreditBatchEntry(entryDTO, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry not found, or the status change is not forward-only"));

                var refreshed = _creditBatchAppService.FindCreditBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<CreditBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _creditBatchAppService.RemoveCreditBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Marks a single entry Posted (and, for Payout/CheckOff, posts its GL
        // journal — see CreditBatchAppService.PostCreditBatchEntry). For
        // CashPickup/SundryPayments entries this only flips the entry's status; the
        // actual cash journal for those two types is posted by
        // SundryPaymentsController, which calls this endpoint itself right after a
        // successful Cash Pickup payment so the picked entry can't be paid twice —
        // see api/frontoffice/sundrypayments.
        [HttpPost]
        [Route("entries/{entryId:guid}/post")]
        public IHttpActionResult PostEntry(Guid entryId, [FromBody] PostCreditBatchEntryRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var posted = _creditBatchAppService.PostCreditBatchEntry(entryId, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be posted (already posted, or its batch/posting period could not be resolved)"));

                var refreshed = _creditBatchAppService.FindCreditBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Entry posted successfully", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private IHttpActionResult RunTransition(Guid id, CreditBatchActionRequest request, Func<CreditBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _creditBatchAppService.FindCreditBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _creditBatchAppService.FindCreditBatch(id, serviceHeader);

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

    public class CreditBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize (AuthorizeCreditBatch posts Payout/CheckOff
        // journals inline on Post; ignored by Audit).
        public int ModuleNavigationItemCode { get; set; }
    }

    public class PostCreditBatchEntryRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
