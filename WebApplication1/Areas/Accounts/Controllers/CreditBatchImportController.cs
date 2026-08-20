using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Dedicated adapter for the original CreditBatchAppService import pipeline.
    // Parsing, matching, loan allocation, tariffs, fuzzy matching, entry replacement,
    // and discrepancy persistence remain in the application layer; this controller
    // owns only HTTP upload validation and configured file storage.
    [Authorize]
    [RoutePrefix("api/accounts/creditbatches")]
    public class CreditBatchImportController : ApiController
    {
        private readonly ICreditBatchAppService _creditBatchAppService;

        public CreditBatchImportController(ICreditBatchAppService creditBatchAppService)
        {
            _creditBatchAppService = creditBatchAppService ?? throw new ArgumentNullException(nameof(creditBatchAppService));
        }

        [HttpPost]
        [Route("{id:guid}/import")]
        public async Task<IHttpActionResult> Import(Guid id)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return ErrorResponse(HttpStatusCode.UnsupportedMediaType, "Expected multipart/form-data content with a 'file' part.");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var batch = _creditBatchAppService.FindCreditBatch(id, serviceHeader);

                if (batch == null)
                    return NotFound();

                if (batch.Status != (int)BatchStatus.Pending)
                    return ErrorResponse(HttpStatusCode.Conflict, "Only a Pending credit batch can be imported.");

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);
                if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory))
                    return InternalServerError(new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration)."));

                Directory.CreateDirectory(settings.FileUploadDirectory);
                var provider = new MultipartFormDataStreamProvider(settings.FileUploadDirectory);
                await Request.Content.ReadAsMultipartAsync(provider);

                var uploaded = provider.FileData.FirstOrDefault();
                if (uploaded == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "No file was uploaded.");

                var suppliedName = uploaded.Headers.ContentDisposition.FileName?.Trim('"');
                var originalFileName = Path.GetFileName(suppliedName ?? "credit-batch.csv");
                if (!string.Equals(Path.GetExtension(originalFileName), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(uploaded.LocalFileName);
                    return ErrorResponse(HttpStatusCode.BadRequest, "Credit-batch imports must be CSV files exported from Excel.");
                }

                var storedFileName = string.Format(
                    "{0}_{1:yyyyMMddHHmmssfff}_{2}.csv",
                    Path.GetFileNameWithoutExtension(originalFileName),
                    DateTime.UtcNow,
                    Guid.NewGuid().ToString("N"));
                var storedPath = Path.Combine(settings.FileUploadDirectory, storedFileName);
                File.Move(uploaded.LocalFileName, storedPath);

                var mismatches = _creditBatchAppService.ParseCreditBatchImport(
                    id,
                    settings.FileUploadDirectory,
                    storedFileName,
                    serviceHeader);

                if (mismatches == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "The file contained no valid rows, had an unsupported layout, or the batch could not be imported.");

                var entries = _creditBatchAppService.FindCreditBatchEntriesByCreditBatchId(id, serviceHeader);
                var data = new
                {
                    BatchId = id,
                    FileName = originalFileName,
                    MatchedCount = entries?.Count ?? 0,
                    MismatchCount = mismatches.Count,
                    Mismatches = mismatches
                };

                return Ok(new { success = true, message = "Credit batch imported and reconciled.", data });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode status, string message)
        {
            return Content(status, new { success = false, message, data = (object)null });
        }
    }
}
