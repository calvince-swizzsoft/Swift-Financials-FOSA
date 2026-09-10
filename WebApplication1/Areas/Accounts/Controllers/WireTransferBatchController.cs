using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC BatchOrigination_WireTransferController /
    // BatchVerification_WireTransferController / BatchAuthorization_WireTransferController
    // (there is no plain unified WireTransferBatchController in the reference app,
    // unlike Credit/Debit) — folded into one controller here, same reasoning as
    // both of those: the three-way split is a menu/role-routing artifact, see
    // BATCH-PROCEDURES-CONCEPTS.md §1.
    //
    // Where this sits relative to Credit/Debit:
    // - Has a real TotalValue control-total (entries must not exceed it), same as
    //   Credit — checked on Update/Audit/Authorize.
    // - Authorize strictly requires the batch to already be Audited, same as
    //   Debit (Credit's equivalent guard is commented out in source).
    // - Authorize posts every entry through PostWireTransferBatchEntry. Unlike
    //   Debit, this is synchronous because this solution has no Wire Transfer
    //   posting-service plugin to consume the configured MSMQ queue.
    // - Entries DO carry a real, trustworthy Amount (unlike Debit) — no
    //   tariff-basis computation needed to know what an entry is worth before
    //   it posts.
    //
    // What PostEntry actually does — worth reading before wiring a UI to this:
    // it debits the customer's product account (Amount + wire-transfer-type
    // tariffs) and credits the WireTransferType's own G/L account (a
    // suspense/clearing account) with ONE journal — same shape as Credit's
    // Payout posting. Despite the MPESA B2C/B2B/EFT type naming, **no external
    // gateway call happens here** — WireTransferBatchEntryDTO.ThirdPartyResponse
    // exists on the DTO but nothing in WireTransferBatchAppService ever sets it.
    // Actually dispatching to Mpesa/an EFT/SWIFT network, if that's a real
    // requirement, is a separate integration this API does not perform. If the
    // customer's available balance can't cover Amount + tariffs, the entry is
    // auto-**rejected** outright (not partially processed the way Debit caps and
    // partially deducts).
    //
    // Deliberately not exposed, consistent with Credit/Debit: CSV import
    // (ParseWireTransferBatchImport).
    [Authorize]
    [RoutePrefix("api/accounts/wiretransferbatches")]
    public class WireTransferBatchController : ApiController
    {
        private readonly IWireTransferBatchAppService _wireTransferBatchAppService;

        public WireTransferBatchController(IWireTransferBatchAppService wireTransferBatchAppService)
        {
            _wireTransferBatchAppService = wireTransferBatchAppService ?? throw new ArgumentNullException(nameof(wireTransferBatchAppService));
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var batches = _wireTransferBatchAppService.FindWireTransferBatches(serviceHeader);

                return Ok(ApiResponse("", batches ?? new List<WireTransferBatchDTO>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // No status-less paged overload exists on this app service (unlike
        // Credit/Debit) — status is a real, required filter here, not optional.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _wireTransferBatchAppService.FindWireTransferBatches(status, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

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

                var batch = _wireTransferBatchAppService.FindWireTransferBatch(id, serviceHeader);

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
        public IHttpActionResult Create(WireTransferBatchDTO wireTransferBatchDTO)
        {
            if (wireTransferBatchDTO == null)
                return ErrorResponse("Request body is required");

            wireTransferBatchDTO.ValidateAll();
            if (wireTransferBatchDTO.HasErrors)
                return ErrorResponse(string.Join("; ", wireTransferBatchDTO.ErrorMessages));
            if (!Enum.IsDefined(typeof(WireTransferBatchType), wireTransferBatchDTO.Type))
                return ErrorResponse("A valid batch type is required");
            if (!Enum.IsDefined(typeof(QueuePriority), wireTransferBatchDTO.Priority))
                return ErrorResponse("A valid priority is required");
            if (wireTransferBatchDTO.TotalValue <= 0m)
                return ErrorResponse("Total value must be greater than zero");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _wireTransferBatchAppService.AddNewWireTransferBatch(wireTransferBatchDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to create the wire transfer batch");

                return Ok(ApiResponse("Wire transfer batch created successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, WireTransferBatchDTO wireTransferBatchDTO)
        {
            if (wireTransferBatchDTO == null)
                return ErrorResponse("Request body is required");
            if (string.IsNullOrWhiteSpace(wireTransferBatchDTO.Reference))
                return ErrorResponse("Reference is required");
            if (wireTransferBatchDTO.TotalValue <= 0m)
                return ErrorResponse("Total value must be greater than zero");
            if (!Enum.IsDefined(typeof(QueuePriority), wireTransferBatchDTO.Priority))
                return ErrorResponse("A valid priority is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                wireTransferBatchDTO.Id = id;

                var updated = _wireTransferBatchAppService.UpdateWireTransferBatch(wireTransferBatchDTO, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Batch not found, or entries total exceeds the batch's total value"));

                var refreshed = _wireTransferBatchAppService.FindWireTransferBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify/Audit a Pending batch. BatchAuthOption: 1=Post (-> Audited, only
        // if entries total <= batch TotalValue), 2=Reject.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, [FromBody] WireTransferBatchActionRequest request)
        {
            var precondition = ValidateAuditRequest(id, request);
            if (precondition != null) return precondition;
            return RunTransition(id, request, (dto, option, header) => _wireTransferBatchAppService.AuditWireTransferBatch(dto, option, header));
        }

        // Authorize an Audited batch. BatchAuthOption: 1=Post (-> Posted; every
        // entry is posted synchronously — see class-level comment), 2=Reject.
        // Refuses outright if the batch isn't already Audited.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] WireTransferBatchActionRequest request)
        {
            var precondition = ValidateAuthorizationRequest(id, request);
            if (precondition != null) return precondition;
            return RunTransition(id, request, (dto, option, header) => _wireTransferBatchAppService.AuthorizeWireTransferBatch(dto, option, request?.ModuleNavigationItemCode ?? 0, header));
        }

        private IHttpActionResult ValidateAuditRequest(Guid id, WireTransferBatchActionRequest request)
        {
            if (request == null || !Enum.IsDefined(typeof(BatchAuthOption), request.Option))
                return ErrorResponse("Select a valid verification action: Post or Reject.");

            var header = Utils.CreateServiceHeader();
            var batch = _wireTransferBatchAppService.FindWireTransferBatch(id, header);
            if (batch == null) return NotFound();
            if (batch.Status != (int)BatchStatus.Pending)
                return Content(HttpStatusCode.Conflict, ErrorEnvelope(string.Format("This batch cannot be verified because its current status is {0}; only Pending batches can be verified.", batch.StatusDescription)));
            return null;
        }

        private IHttpActionResult ValidateAuthorizationRequest(Guid id, WireTransferBatchActionRequest request)
        {
            if (request == null || !Enum.IsDefined(typeof(BatchAuthOption), request.Option))
                return ErrorResponse("Select a valid authorization action: Post or Reject.");

            var header = Utils.CreateServiceHeader();
            var batch = _wireTransferBatchAppService.FindWireTransferBatch(id, header);
            if (batch == null) return NotFound();
            if (batch.Status != (int)BatchStatus.Audited)
                return Content(HttpStatusCode.Conflict, ErrorEnvelope(string.Format("This batch cannot be authorized because its current status is {0}; it must be verified and Audited first.", batch.StatusDescription)));
            return null;
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetByBatch(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _wireTransferBatchAppService.FindWireTransferBatchEntriesByWireTransferBatchId(id, text ?? "", pageIndex, pageSize, serviceHeader);

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

                var page = _wireTransferBatchAppService.FindQueableWireTransferBatchEntries(pageIndex, pageSize, serviceHeader);

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

                var entry = _wireTransferBatchAppService.FindWireTransferBatchEntry(entryId, serviceHeader);

                if (entry == null)
                    return NotFound();

                return Ok(ApiResponse("", entry));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, WireTransferBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.WireTransferBatchId = id;
                entryDTO.ValidateAll();
                if (entryDTO.HasErrors)
                    return ErrorResponse(string.Join("; ", entryDTO.ErrorMessages));
                if (entryDTO.Amount <= 0m)
                    return ErrorResponse("Amount must be greater than zero");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _wireTransferBatchAppService.AddNewWireTransferBatchEntry(entryDTO, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be added. The batch must be Pending and owned by the current maker, and the entry must not exceed the batch total."));

                return Ok(ApiResponse("Entry added successfully", created));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Status can only move forward (Pending -> Posted/Rejected) — enforced by
        // UpdateWireTransferBatchEntry itself, not re-validated here.
        [HttpPut]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult UpdateEntry(Guid entryId, WireTransferBatchEntryDTO entryDTO)
        {
            if (entryDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                entryDTO.Id = entryId;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _wireTransferBatchAppService.UpdateWireTransferBatchEntry(entryDTO, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry not found, or the status change is not forward-only"));

                var refreshed = _wireTransferBatchAppService.FindWireTransferBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries(List<WireTransferBatchEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return ErrorResponse("At least one entry is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _wireTransferBatchAppService.RemoveWireTransferBatchEntries(entries, serviceHeader);

                if (!removed)
                    return ErrorResponse("Failed to remove the selected entries");

                return Ok(ApiResponse("Entries removed successfully", null));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Posts one entry: debits the customer (Amount + wire-transfer-type
        // tariffs) and credits the WireTransferType's G/L account in one
        // journal, or auto-rejects the entry outright if the customer's
        // available balance can't cover it — see class-level comment.
        [HttpPost]
        [Route("entries/{entryId:guid}/post")]
        public IHttpActionResult PostEntry(Guid entryId, [FromBody] PostWireTransferBatchEntryRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var posted = _wireTransferBatchAppService.PostWireTransferBatchEntry(entryId, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Entry could not be posted (already posted, its batch/posting period could not be resolved, or it was auto-rejected for insufficient available balance)"));

                var refreshed = _wireTransferBatchAppService.FindWireTransferBatchEntry(entryId, serviceHeader);

                return Ok(ApiResponse("Entry posted successfully", refreshed));
            }
            catch (TransactionAuthorityException ex)
            {
                return ResponseMessage(WebApplication1.ApiErrors.ApiErrorResponses.Create(
                    Request, HttpStatusCode.Forbidden, "TRANSACTION_AUTHORITY_DENIED", ex.Message));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IHttpActionResult RunTransition(Guid id, WireTransferBatchActionRequest request, Func<WireTransferBatchDTO, int, ServiceHeader, bool> transition)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _wireTransferBatchAppService.FindWireTransferBatch(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks ?? existing.AuditRemarks;
                existing.AuthorizationRemarks = request?.Remarks ?? existing.AuthorizationRemarks;

                var result = transition(existing, request?.Option ?? 0, serviceHeader);

                if (!result)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("The batch could not advance. When posting, it must contain at least one entry and the entries total must not exceed the batch total value."));

                var updated = _wireTransferBatchAppService.FindWireTransferBatch(id, serviceHeader);

                return Ok(ApiResponse("Operation success", updated));
            }
            catch (TransactionAuthorityException ex)
            {
                return ResponseMessage(WebApplication1.ApiErrors.ApiErrorResponses.Create(
                    Request, HttpStatusCode.Forbidden, "TRANSACTION_AUTHORITY_DENIED", ex.Message));
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

    public class WireTransferBatchActionRequest
    {
        // BatchAuthOption: 1 = Post, 2 = Reject.
        public int Option { get; set; }

        public string Remarks { get; set; }

        // Only read by Authorize; ignored by Audit.
        public int ModuleNavigationItemCode { get; set; }
    }

    public class PostWireTransferBatchEntryRequest
    {
        public int ModuleNavigationItemCode { get; set; }
    }
}
