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
    // Adapted from the reference MVC DebitBatchController (Areas/Accounts) — that
    // one is already unified (Index/Details/Create/Edit/Verify/Authorize in one
    // controller, same shape CreditBatchController's reference had), not the
    // separately-split BatchOrigination_Debit/BatchVerification_Debit/
    // BatchAuthorization_Debit screens — see BATCH-PROCEDURES-CONCEPTS.md §1 for
    // why those three are a menu/role-routing artifact, not three controllers here.
    //
    // Real differences from CreditBatchController worth knowing before touching
    // this:
    // - DebitBatchDTO has no TotalValue field at all, so unlike Credit there is no
    //   "entries total must not exceed the batch total" control-total check
    //   anywhere in DebitBatchAppService (Add/Update/Audit/Authorize). Nothing
    //   missing here — the reference app genuinely doesn't gate on one for Debit.
    // - AuthorizeDebitBatch strictly requires the batch to already be Audited
    //   (persisted.Status != BatchStatus.Audited -> refuses). CreditBatch's
    //   equivalent guard is commented out in AuthorizeCreditBatch — a real,
    //   pre-existing asymmetry between the two services, not something to
    //   normalize here.
    // - DebitBatchEntryDTO has no Amount field — entries carry Multiplier +
    //   BasisValue instead. The actual deduction amount (possibly several tariff
    //   lines) is only computed inside PostDebitBatchEntry, via
    //   ComputeTariffsByDebitType(DebitTypeId, BasisValue, Multiplier,
    //   customerAccount), and is capped against the customer's available balance
    //   unless BranchCompanyAllowDebitBatchToOverdrawAccount is set. There is no
    //   "amount" to show a client before an entry actually posts.
    // - Authorizing a batch with option 1 (Post) enqueues every one of its entries
    //   onto an async message queue (BrokerService.ProcessDebitBatchEntries ->
    //   DebitBatchPostingQueuePath) for background processing — GL posting is
    //   NOT synchronous with this call, and unlike CreditBatch's equivalent
    //   dispatch (which only fires for Payout/CheckOff types), every Debit batch
    //   gets queued since Debit has no sub-type to filter on. PostEntry below is
    //   what that background consumer calls per entry; it's also exposed here as
    //   a manual retry path for any entry that didn't post.
    //
    // Deliberately not exposed, consistent with CreditBatchController: CSV import
    // (ParseDebitBatchImport — no file-upload pattern in this project yet).
    [Authorize]
    [RoutePrefix("api/accounts/debitbatches")]
    public class DebitBatchController : ApiController
    {
        private readonly IDebitBatchAppService _debitBatchAppService;

        public DebitBatchController(IDebitBatchAppService debitBatchAppService)
        {
            _debitBatchAppService = debitBatchAppService ?? throw new ArgumentNullException(nameof(debitBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var debitBatches = _debitBatchAppService.FindDebitBatches(serviceHeader);

                return Ok(ApiResponse("", debitBatches ?? new List<DebitBatchDTO>()));
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
                    ? _debitBatchAppService.FindDebitBatches(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : string.IsNullOrWhiteSpace(text)
                        ? _debitBatchAppService.FindDebitBatches(pageIndex, pageSize, serviceHeader)
                        : _debitBatchAppService.FindDebitBatches(text, pageIndex, pageSize, serviceHeader);

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

                var debitBatch = _debitBatchAppService.FindDebitBatch(id, serviceHeader);

                if (debitBatch == null)
                    return NotFound();

                return Ok(ApiResponse("", debitBatch));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create -> Pending. BatchNumber/Status/CreatedBy are all assigned server-side.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(DebitBatchDTO debitBatchDTO)
        {
            if (debitBatchDTO == null)
                return ErrorResponse("Request body is required");

            debitBatchDTO.ValidateAll();
            if (debitBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", debitBatchDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _debitBatchAppService.AddNewDebitBatch(debitBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the debit batch");

                return Ok(ApiResponse("Debit batch created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, DebitBatchDTO debitBatchDTO)
        {
            if (debitBatchDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                debitBatchDTO.Id = id;

                var updated = _debitBatchAppService.UpdateDebitBatch(debitBatchDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _debitBatchAppService.FindDebitBatch(id, serviceHeader);

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
        public IHttpActionResult Audit(Guid id, [FromBody] DebitBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _debitBatchAppService.AuditDebitBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted; every
        // entry is queued for async background posting — see class-level comment),
        // 2=Reject. Unlike CreditBatch, this refuses outright if the batch isn't
        // already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] DebitBatchActionRequest request)
        {
            return RunTransition(id, request, (dto, option, header) => _debitBatchAppService.AuthorizeDebitBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _debitBatchAppService.FindDebitBatchEntriesByDebitBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // The general "entries ready to post" browse — unlike CreditBatch's
        // equivalent (hardcoded to Payout/CheckOff types only, so unusable for
        // CashPickup), this one has no type restriction since Debit has no
        // sub-type. Still only useful as a manual/ops view: normal processing
        // happens automatically off the message queue once a batch is authorized.
        [HttpGet]
        [Route("entries/queueable")]
        public IHttpActionResult GetQueueable(int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _debitBatchAppService.FindQueableDebitBatchEntries(pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("entries/customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomer(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _debitBatchAppService.FindDebitBatchEntriesByCustomerId(customerId, serviceHeader);

                return Ok(ApiResponse("", entries ?? new List<DebitBatchEntryDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, DebitBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.DebitBatchId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _debitBatchAppService.AddNewDebitBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to add the debit batch entry");

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<DebitBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _debitBatchAppService.RemoveDebitBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Computes the entry's actual deduction (possibly several tariff lines,
        // capped against available balance) and posts it — see class-level
        // comment. This is what the async queue consumer calls per entry after
        // Authorize; exposed here too as a manual retry for any entry that didn't
        // post on its own. No single-entry lookup exists on this service (unlike
        // Credit's FindCreditBatchEntry), so there's nothing to refresh and
        // return here beyond a success message — the caller can re-browse via
        // GetByBatch/GetQueueable/GetByCustomer to see the entry's new status.
        [HttpPost]
        [Route("entries/{entryId:guid}/post")]
        public IHttpActionResult PostEntry(Guid entryId, [FromBody] PostDebitBatchEntryRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var posted = _debitBatchAppService.PostDebitBatchEntry(entryId, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be posted (already posted, or its batch/posting period/customer account could not be resolved)"));

                return Ok(ApiResponse("Entry posted successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, DebitBatchActionRequest request, Func<DebitBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _debitBatchAppService.FindDebitBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch is not in the right state for this action, or the action option is invalid"));

                var updated = _debitBatchAppService.FindDebitBatch(id, serviceHeader);

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

    public class DebitBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }

    public class PostDebitBatchEntryRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
