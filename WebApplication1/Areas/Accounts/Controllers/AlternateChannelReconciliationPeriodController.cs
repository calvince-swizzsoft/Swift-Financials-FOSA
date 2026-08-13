using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC AlternatePeriodsController (Areas/Accounts) — routed
    // through IAlternateChannelReconciliationPeriodAppService instead of the monolithic
    // _channelService. That app service was fully built already (real GL-aware reconciliation
    // logic: matches AlternateChannelLog entries against imported bank/processor files per
    // AlternateChannelType and SetDifferenceMode) with no controller anywhere - only reachable
    // via the legacy AlternateChannelReconciliationPeriodService.svc.cs WCF passthrough, same
    // "fully built, WCF-only" gap ChequeBook/UnPayReason had before their controllers.
    //
    // Real issues found reading the reference controller, not ported:
    // - Details(id) calls _channelService.FindAlternateChannelAsync(id, ...) - loads an
    //   AlternateChannel, not a reconciliation period, despite this being the reconciliation-
    //   period controller. Looks miswired/copy-pasted, same shape as RegisterController's
    //   Verify/Authorize-bound-to-DebitBatchDTO mixup found earlier. Not reproduced - Get
    //   below actually fetches the period.
    // - Processing(id) GET also calls FindAlternateChannelsByTypeAndFilterInPageAsync(64, 3,
    //   null, 2, 0, 10, true, ...) with unexplained magic numbers, unrelated to the period
    //   being viewed - looks like leftover debug/copy-paste. Not reproduced.
    // - Large blocks of commented-out field mapping (customer reference fields, cheque
    //   fields) throughout Index/Processing/Closing - dead code, not reproduced.
    // - Search largely duplicates Index's GET logic against a different DTO shape - not a
    //   distinct operation, not reproduced (same reasoning as AlternateChannelController's
    //   Create(id)/Linking(id)/History(id) GET overloads being pure MVC view-staging).
    //
    // Flagged, not fixed: IAlternateChannelReconciliationPeriodAppService.ParseAlternateChannelReconciliationImport
    // gates on `persisted.Status == (int)BatchStatus.Pending`, but Status is populated from a
    // different enum entirely (AlternateChannelReconciliationPeriodStatus - Open/Closed/
    // Suspended). It happens to work today only because BatchStatus.Pending and
    // AlternateChannelReconciliationPeriodStatus.Open are both numerically 1 - a coincidence,
    // not a correct type reference. Left as-is; not this controller's app-service logic to fix
    // unasked.
    [Authorize]
    [RoutePrefix("api/accounts/alternatechannelreconciliationperiods")]
    public class AlternateChannelReconciliationPeriodController : ApiController
    {
        private readonly IAlternateChannelReconciliationPeriodAppService _alternateChannelReconciliationPeriodAppService;

        public AlternateChannelReconciliationPeriodController(IAlternateChannelReconciliationPeriodAppService alternateChannelReconciliationPeriodAppService)
        {
            _alternateChannelReconciliationPeriodAppService = alternateChannelReconciliationPeriodAppService ?? throw new ArgumentNullException(nameof(alternateChannelReconciliationPeriodAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var periods = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(serviceHeader);

                return Ok(new { success = true, message = "", data = periods ?? new List<AlternateChannelReconciliationPeriodDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(pageIndex, pageSize, serviceHeader)
                    : _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // status is AlternateChannelReconciliationPeriodStatus (Open/Closed/Suspended) - the
        // reference controller's Create/Clos actions are this same query with status fixed to
        // Open/Closed respectively, so callers here get both by passing the status themselves
        // instead of needing two separate routes. startDate/endDate default to a 30-day-before/
        // after window, same default the reference controller used.
        [HttpGet]
        [Route("paged/status/{status:int}")]
        public async Task<IHttpActionResult> GetPagedByStatus(int status, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(status, startDate ?? DateTime.Now.AddDays(-30), endDate ?? DateTime.Now.AddDays(30), text ?? string.Empty, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var period = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriod(id, serviceHeader);

                if (period == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = period });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO)
        {
            try
            {
                if (alternateChannelReconciliationPeriodDTO == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid reconciliation period data", data = (object)null });

                alternateChannelReconciliationPeriodDTO.ValidateAll();

                if (alternateChannelReconciliationPeriodDTO.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelReconciliationPeriodDTO.ErrorMessages), data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _alternateChannelReconciliationPeriodAppService.AddNewAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the reconciliation period could not be created.", data = (object)null });

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO)
        {
            try
            {
                if (alternateChannelReconciliationPeriodDTO == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid reconciliation period data", data = (object)null });

                alternateChannelReconciliationPeriodDTO.Id = id;
                alternateChannelReconciliationPeriodDTO.ValidateAll();

                if (alternateChannelReconciliationPeriodDTO.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelReconciliationPeriodDTO.ErrorMessages), data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _alternateChannelReconciliationPeriodAppService.UpdateAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriod(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // AlternateChannelReconciliationPeriodAuthOption.Post - sets Status to Closed. Only
        // valid while the period is currently Open (enforced in the app service); a period
        // already Closed/Suspended returns false here, surfaced as 409.
        [HttpPost]
        [Route("{id:guid}/post")]
        public async Task<IHttpActionResult> Post(Guid id, CloseReconciliationPeriodRequest request)
        {
            return await Close(id, (int)AlternateChannelReconciliationPeriodAuthOption.Post, request);
        }

        // AlternateChannelReconciliationPeriodAuthOption.Reject - sets Status to Suspended, not
        // a rejection of the period's existence (it still exists, just no longer Open). Same
        // Open-only precondition as Post above.
        [HttpPost]
        [Route("{id:guid}/reject")]
        public async Task<IHttpActionResult> Reject(Guid id, CloseReconciliationPeriodRequest request)
        {
            return await Close(id, (int)AlternateChannelReconciliationPeriodAuthOption.Reject, request);
        }

        private async Task<IHttpActionResult> Close(Guid id, int authOption, CloseReconciliationPeriodRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var period = new AlternateChannelReconciliationPeriodDTO
                {
                    Id = id,
                    AuthorizationRemarks = request?.Remarks
                };

                var closed = _alternateChannelReconciliationPeriodAppService.CloseAlternateChannelReconciliationPeriod(period, authOption, serviceHeader);

                if (!closed)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "The reconciliation period could not be updated - it may not exist, or may no longer be Open.", data = (object)null });

                var refreshed = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriod(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> GetEntries(Guid id, int status = 0, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodId(id, status, text ?? string.Empty, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = entries });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> AddEntry(Guid id, AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO)
        {
            try
            {
                if (alternateChannelReconciliationEntryDTO == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid reconciliation entry data", data = (object)null });

                alternateChannelReconciliationEntryDTO.AlternateChannelReconciliationPeriodId = id;
                alternateChannelReconciliationEntryDTO.ValidateAll();

                if (alternateChannelReconciliationEntryDTO.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelReconciliationEntryDTO.ErrorMessages), data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _alternateChannelReconciliationPeriodAppService.AddNewAlternateChannelReconciliationEntry(alternateChannelReconciliationEntryDTO, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the reconciliation entry could not be added.", data = (object)null });

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST, not DELETE - the entries to remove are identified by a body list of DTOs (only
        // .Id is read), same "POST not DELETE" reasoning as AlternateChannelController.Delink.
        [HttpPost]
        [Route("{id:guid}/entries/remove")]
        public async Task<IHttpActionResult> RemoveEntries(Guid id, List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _alternateChannelReconciliationPeriodAppService.RemoveAlternateChannelReconciliationEntries(alternateChannelReconciliationEntryDTOs ?? new List<AlternateChannelReconciliationEntryDTO>(), serviceHeader);

                if (!removed)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The reconciliation entries could not be removed.", data = (object)null });

                return Ok(new { success = true, message = "Operation Success", data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Upload a bank/processor statement file (multipart/form-data, field name "file") to
        // reconcile against this period's AlternateChannelLog entries - same upload shape as
        // AutomatedClearingController.Upload (ElectronicJournal image batches), same server-side
        // file directory source (serviceBrokerConfiguration, never accepted from the client).
        // Response data is the list of unmatched/mismatched entries the import produced -
        // matched entries are already saved as reconciled AlternateChannelReconciliationEntry
        // rows by the app service, not returned again here.
        [HttpPost]
        [Route("{id:guid}/import")]
        public async Task<IHttpActionResult> Import(Guid id)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Expected multipart/form-data content with a 'file' part.");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

                if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory))
                    return InternalServerError(new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration)."));

                Directory.CreateDirectory(settings.FileUploadDirectory);

                var provider = new MultipartFormDataStreamProvider(settings.FileUploadDirectory);
                await Request.Content.ReadAsMultipartAsync(provider);

                var uploaded = provider.FileData.FirstOrDefault();
                if (uploaded == null)
                    return BadRequest("No file was uploaded.");

                var originalFileName = uploaded.Headers.ContentDisposition.FileName?.Trim('"') ?? Path.GetFileName(uploaded.LocalFileName);
                var targetPath = Path.Combine(settings.FileUploadDirectory, originalFileName);

                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                File.Move(uploaded.LocalFileName, targetPath);

                var unmatched = _alternateChannelReconciliationPeriodAppService.ParseAlternateChannelReconciliationImport(id, settings.FileUploadDirectory, originalFileName, serviceHeader);

                if (unmatched == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The file could not be parsed - check that the reconciliation period is Open and the file exists.", data = (object)null });

                return Ok(new { success = true, message = "Reconciliation file uploaded and parsed successfully.", data = unmatched });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class CloseReconciliationPeriodRequest
    {
        public string Remarks { get; set; }
    }
}
