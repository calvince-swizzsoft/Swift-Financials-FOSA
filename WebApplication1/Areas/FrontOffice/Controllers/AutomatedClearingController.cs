using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC AutomatedClearingController — image-based
    // (truncated) cheque clearing, routed through IElectronicJournalAppService
    // instead of the monolithic _channelService. The reference controller's
    // Index/Processing actions called FindElectronicJournalsByFilterInPageAsync
    // but never actually returned its result to the view (a bug — the query ran
    // and was discarded); this controller returns it.
    //
    // ParseElectronicJournalImport/CloseElectronicJournal need a server-side
    // file directory, blob-store connection string, and (for closing) PGP
    // encryption key paths/passphrase — these are server secrets/paths, sourced
    // from the existing serviceBrokerConfiguration section
    // (ConfigurationHelper.GetServiceBrokerConfigurationSettings, the same
    // helper the WCF ElectronicJournalService.svc uses) and the "BLOBStore"
    // connection string (same pattern as CustomerAccountStatementController's
    // PDF printing) — never accepted from the client.
    [Authorize]
    [RoutePrefix("api/frontoffice/automatedclearing")]
    public class AutomatedClearingController : ApiController
    {
        private readonly IElectronicJournalAppService _electronicJournalAppService;

        public AutomatedClearingController(IElectronicJournalAppService electronicJournalAppService)
        {
            _electronicJournalAppService = electronicJournalAppService ?? throw new ArgumentNullException(nameof(electronicJournalAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status = 0, DateTime? startDate = null, DateTime? endDate = null, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var journals = _electronicJournalAppService.FindElectronicJournals(status, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = journals });
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

                var journal = _electronicJournalAppService.FindElectronicJournal(id, serviceHeader);

                if (journal == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = journal });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/truncatedcheques")]
        public IHttpActionResult GetTruncatedCheques(Guid id, int? status, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheques = status.HasValue
                    ? _electronicJournalAppService.FindTruncatedCheques(id, status.Value, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _electronicJournalAppService.FindTruncatedCheques(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = cheques });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Upload a scanned cheque-image batch file (multipart/form-data, field
        // name "file") for parsing into an ElectronicJournal + its
        // TruncatedCheque entries.
        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> Upload()
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

                var blobDatabaseConnectionString = ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString;

                var result = _electronicJournalAppService.ParseElectronicJournalImport(settings.FileUploadDirectory, originalFileName, blobDatabaseConnectionString, serviceHeader);

                if (result == null)
                    return BadRequest("Failed to parse the uploaded electronic journal.");

                return Ok(new { success = true, message = "Electronic journal uploaded and parsed successfully.", data = result });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Finalizes/exports an electronic journal batch (PGP-encrypted export to
        // the configured file export directory) once its truncated cheques have
        // been processed.
        [HttpPost]
        [Route("{id:guid}/close")]
        public IHttpActionResult Close(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var journal = _electronicJournalAppService.FindElectronicJournal(id, serviceHeader);

                if (journal == null)
                    return NotFound();

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

                if (settings == null)
                    return InternalServerError(new InvalidOperationException("serviceBrokerConfiguration settings are not configured."));

                var result = _electronicJournalAppService.CloseElectronicJournal(journal, settings.EncryptionPublicKeyPath, settings.EncryptionPrivateKeyPath, settings.EncryptionPassPhrase, settings.FileExportDirectory, serviceHeader);

                if (!result)
                    return BadRequest("Failed to close the electronic journal.");

                return Ok(new { success = true, message = "Electronic journal closed successfully.", data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("truncatedcheques/{id:guid}/clear")]
        public IHttpActionResult ClearTruncatedCheque(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheque = _electronicJournalAppService.FindTruncatedCheque(id, serviceHeader);

                if (cheque == null)
                    return NotFound();

                var result = _electronicJournalAppService.ClearTruncatedCheque(cheque, serviceHeader);

                if (!result)
                    return BadRequest("Failed to clear the truncated cheque.");

                return Ok(new { success = true, message = "Truncated cheque cleared.", data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("truncatedcheques/{id:guid}/match-voucher")]
        public IHttpActionResult MatchPaymentVoucher(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheque = _electronicJournalAppService.FindTruncatedCheque(id, serviceHeader);

                if (cheque == null)
                    return NotFound();

                var result = _electronicJournalAppService.MatchTruncatedChequePaymentVoucher(cheque, serviceHeader);

                if (!result)
                    return BadRequest("Failed to match the truncated cheque to a payment voucher.");

                return Ok(new { success = true, message = "Payment voucher matched.", data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
