using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Code 22009 ("Document", ControllerName:
    // EmployeeDocuments, AreaName: HumanResource, under Operations >
    // Employees) — same gap as Holidays (Code 22005): the domain/app-service
    // layer (IEmployeeDocumentAppService) was already fully built but had no
    // REST controller. Unlike Holidays, IEmployeeDocumentAppService has no
    // RemoveEmployeeDocument at all, so there is deliberately no DELETE
    // route here (same "don't fake a capability the backend doesn't have"
    // rule CostCenters/ChequeTypes already follow) — and no
    // FindEmployeeDocuments(employeeId, ...) overload either, so this
    // browses ALL employee documents flat (matching what the single
    // "Document" nav leaf actually models, not a per-employee drilldown),
    // filterable by search text, with each row's own
    // EmployeeCustomerFullName identifying whose document it is.
    //
    // AddNewEmployeeDocument/UpdateEmployeeDocument both require the file to
    // already exist on disk at Path.Combine(fileUploadDirectory, FileName)
    // *before* they run (they read it back into FileBuffer themselves) —
    // mirrors CreditBatchImportController/AutomatedClearingController's
    // upload-then-import two-step, using the same
    // ConfigurationHelper.GetServiceBrokerConfigurationSettings(...).FileUploadDirectory
    // (already configured in this project's own Web.config). Update never
    // accepts a replacement file — FileName is round-tripped from the
    // persisted record unchanged, so it always resolves to the same on-disk
    // file; attaching a different file means creating a new document, not
    // editing this one.
    [Authorize]
    [RoutePrefix("api/humanresource/employeedocuments")]
    public class EmployeeDocumentsController : ApiController
    {
        private readonly IEmployeeDocumentAppService _employeeDocumentAppService;

        public EmployeeDocumentsController(IEmployeeDocumentAppService employeeDocumentAppService)
        {
            _employeeDocumentAppService = employeeDocumentAppService ?? throw new ArgumentNullException(nameof(employeeDocumentAppService));
        }

        // FileBuffer is stripped from every row here — a paged browse list
        // has no business shipping every document's full binary content on
        // every page load. Use GET /{id}/download for the actual bytes.
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var documents = _employeeDocumentAppService.FindEmployeeDocuments(text, pageIndex, pageSize, serviceHeader);

                if (documents == null)
                    return NotFound();

                if (documents.PageCollection != null)
                {
                    foreach (var doc in documents.PageCollection)
                        doc.FileBuffer = null;
                }

                return Ok(documents);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/download")]
        public IHttpActionResult Download(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var document = _employeeDocumentAppService.FindEmployeeDocument(id, serviceHeader);

                if (document == null || document.FileBuffer == null)
                    return NotFound();

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(document.FileBuffer);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(document.FileMIMEType) ? "application/octet-stream" : document.FileMIMEType);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.IsNullOrWhiteSpace(document.FileTitle) ? document.FileName : document.FileTitle
                };

                return ResponseMessage(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // multipart/form-data: file part "file", plus text fields
        // "EmployeeId" (required GUID), "FileTitle", "FileDescription".
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create()
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

                Guid employeeId;
                if (!Guid.TryParse(provider.FormData["EmployeeId"], out employeeId) || employeeId == Guid.Empty)
                {
                    File.Delete(uploaded.LocalFileName);
                    return BadRequest("A valid EmployeeId is required.");
                }

                var originalFileName = uploaded.Headers.ContentDisposition.FileName?.Trim('"') ?? Path.GetFileName(uploaded.LocalFileName);
                var storedFileName = string.Format(
                    "{0}_{1:yyyyMMddHHmmssfff}_{2}{3}",
                    Path.GetFileNameWithoutExtension(originalFileName),
                    DateTime.UtcNow,
                    Guid.NewGuid().ToString("N"),
                    Path.GetExtension(originalFileName));
                var storedPath = Path.Combine(settings.FileUploadDirectory, storedFileName);
                File.Move(uploaded.LocalFileName, storedPath);

                var employeeDocumentDTO = new EmployeeDocumentDTO
                {
                    EmployeeId = employeeId,
                    FileName = storedFileName,
                    FileTitle = string.IsNullOrWhiteSpace(provider.FormData["FileTitle"]) ? originalFileName : provider.FormData["FileTitle"],
                    FileDescription = provider.FormData["FileDescription"] ?? "",
                };

                employeeDocumentDTO.ValidateAll();
                if (employeeDocumentDTO.HasErrors)
                {
                    File.Delete(storedPath);
                    return BadRequest(employeeDocumentDTO.ErrorMessages.ToString());
                }

                var created = _employeeDocumentAppService.AddNewEmployeeDocument(employeeDocumentDTO, settings.FileUploadDirectory, serviceHeader);

                if (created == null)
                {
                    File.Delete(storedPath);
                    return InternalServerError(new InvalidOperationException("Failed to save the employee document."));
                }

                created.FileBuffer = null;

                return Ok(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Metadata-only — Title/Description/employee reassignment. FileName
        // is round-tripped from the persisted record so
        // UpdateEmployeeDocument resolves the same already-uploaded file;
        // there's no way to swap the attached file through this action (see
        // class remarks).
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, EmployeeDocumentDTO employeeDocumentDTO)
        {
            try
            {
                if (employeeDocumentDTO == null)
                    return BadRequest("Employee document payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var persisted = _employeeDocumentAppService.FindEmployeeDocument(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                employeeDocumentDTO.Id = id;
                employeeDocumentDTO.FileName = persisted.FileName;

                employeeDocumentDTO.ValidateAll();
                if (employeeDocumentDTO.HasErrors)
                    return BadRequest(employeeDocumentDTO.ErrorMessages.ToString());

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);
                if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory))
                    return InternalServerError(new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration)."));

                var updated = _employeeDocumentAppService.UpdateEmployeeDocument(employeeDocumentDTO, settings.FileUploadDirectory, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(employeeDocumentDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
