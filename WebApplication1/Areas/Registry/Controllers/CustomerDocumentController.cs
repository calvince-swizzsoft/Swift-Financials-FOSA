using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // ICustomerDocumentAppService was already fully built but had no
    // controller anywhere. GetByCustomer/Get below were added first,
    // read-only, to unblock the loan-case registration screen's
    // collateral-document picker (see Areas/BackOffice/WORKFLOW.md §15.2 and
    // §13) — the reference LoanRegistrationController resolves a customer's
    // *released* collateral documents via
    // FindCustomerDocumentsByCustomerIdAndTypeAsync(customerId,
    // CustomerDocumentType.Collateral) then filters client-side on
    // CollateralStatus.Released, and the loan-case Create endpoint already
    // takes a list of document ids resolved through this same app service.
    //
    // Browse/Create/Update/Download added afterwards to back the standalone
    // Registry > Operations > Customers > Documents screen (NavigationMenu.cs
    // Code 21006, ControllerName "Document") — the reference app's own
    // Areas/Registry/Documents.md documents a real create/upload flow
    // (type picker, customer lookup, file browse, title, description) that
    // this was missing entirely. Modeled directly on
    // Areas/HumanResource/Controllers/EmployeeDocumentsController.cs, the
    // same upload-to-configured-directory-then-persist-metadata shape.
    [Authorize]
    [RoutePrefix("api/registry/customerdocuments")]
    public class CustomerDocumentController : ApiController
    {
        private readonly ICustomerDocumentAppService _customerDocumentAppService;

        public CustomerDocumentController(ICustomerDocumentAppService customerDocumentAppService)
        {
            _customerDocumentAppService = customerDocumentAppService ?? throw new ArgumentNullException(nameof(customerDocumentAppService));
        }

        // customerId + type (CustomerDocumentType: 0=General, 1=Collateral)
        // are both required — this is a picker endpoint, not a general
        // browse. Client-side, filter the result to CollateralStatus.Released
        // (0) for a collateral picker, same as the reference screen did.
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetByCustomer(Guid customerId, int type)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var documents = _customerDocumentAppService.FindCustomerDocuments(customerId, type, serviceHeader);

                return Ok(new { success = true, message = "", data = documents ?? new List<CustomerDocumentDTO>() });
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

                var document = _customerDocumentAppService.FindCustomerDocument(id, serviceHeader);

                if (document == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = document });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Paged, flat, searchable browse across ALL customer documents —
        // backs the standalone Documents list screen. A distinct route from
        // the bare GET "" picker above (which requires customerId+type and
        // is relied on by the loan-case collateral picker) — two GET actions
        // can't share one route template in attribute routing. No FileBuffer
        // stripping needed here (unlike EmployeeDocumentsController.Index) —
        // RegistryModuleProfile's CustomerDocument -> CustomerDocumentDTO map
        // ignores FileBuffer unconditionally, so it's never populated by any
        // Find*/Get* read path; Download below reads the bytes off disk
        // directly instead.
        [HttpGet]
        [Route("browse")]
        public IHttpActionResult Browse(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var documents = _customerDocumentAppService.FindCustomerDocuments(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = documents ?? new PageCollectionInfo<CustomerDocumentDTO> { PageCollection = new List<CustomerDocumentDTO>(), ItemsCount = 0 } });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // FileBuffer is never populated on read (see Browse's remarks above)
        // — bytes are read straight off disk instead, the same
        // fileUploadDirectory/FileName convention Create() stores under.
        [HttpGet]
        [Route("{id:guid}/download")]
        public IHttpActionResult Download(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var document = _customerDocumentAppService.FindCustomerDocument(id, serviceHeader);

                if (document == null || string.IsNullOrWhiteSpace(document.FileName))
                    return NotFound();

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);
                if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory))
                    throw new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration).");

                var path = Path.Combine(settings.FileUploadDirectory, document.FileName);
                if (!File.Exists(path))
                    return NotFound();

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(File.ReadAllBytes(path));
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
        // "CustomerId" (required GUID), "Type" (required, 0=General,
        // 1=Collateral), "FileTitle", "FileDescription". Mirrors
        // EmployeeDocumentsController.Create — upload to the configured
        // directory first, then AddNewCustomerDocument(dto,
        // fileUploadDirectory, serviceHeader) reads the file back off disk
        // into FileBuffer itself.
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
                    throw new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration).");

                Directory.CreateDirectory(settings.FileUploadDirectory);

                var provider = new MultipartFormDataStreamProvider(settings.FileUploadDirectory);
                await Request.Content.ReadAsMultipartAsync(provider);

                var uploaded = provider.FileData.FirstOrDefault();
                if (uploaded == null)
                    return BadRequest("No file was uploaded.");

                Guid customerId;
                if (!Guid.TryParse(provider.FormData["CustomerId"], out customerId) || customerId == Guid.Empty)
                {
                    File.Delete(uploaded.LocalFileName);
                    return BadRequest("A valid CustomerId is required.");
                }

                int type;
                if (!int.TryParse(provider.FormData["Type"], out type) || !Enum.IsDefined(typeof(CustomerDocumentType), type))
                {
                    File.Delete(uploaded.LocalFileName);
                    return BadRequest("A valid document Type (0=General, 1=Collateral) is required.");
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

                var customerDocumentDTO = new CustomerDocumentDTO
                {
                    CustomerId = customerId,
                    Type = type,
                    FileName = storedFileName,
                    FileTitle = string.IsNullOrWhiteSpace(provider.FormData["FileTitle"]) ? originalFileName : provider.FormData["FileTitle"],
                    FileDescription = provider.FormData["FileDescription"] ?? "",
                };

                customerDocumentDTO.ValidateAll();
                if (customerDocumentDTO.HasErrors)
                {
                    File.Delete(storedPath);
                    return BadRequest(customerDocumentDTO.ErrorMessages.ToString());
                }

                var created = _customerDocumentAppService.AddNewCustomerDocument(customerDocumentDTO, settings.FileUploadDirectory, serviceHeader);

                if (created == null)
                {
                    File.Delete(storedPath);
                    throw new InvalidOperationException("Failed to save the customer document.");
                }

                created.FileBuffer = null;

                return Ok(new { success = true, message = "", data = created });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Metadata-only — Title/Description/Type/Customer reassignment.
        // FileName is round-tripped from the persisted record so
        // UpdateCustomerDocument resolves the same already-uploaded file;
        // there's no way to swap the attached file through this action
        // (mirrors EmployeeDocumentsController.Update).
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, CustomerDocumentDTO customerDocumentDTO)
        {
            try
            {
                if (customerDocumentDTO == null)
                    return BadRequest("Customer document payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var persisted = _customerDocumentAppService.FindCustomerDocument(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                customerDocumentDTO.Id = id;
                customerDocumentDTO.FileName = persisted.FileName;

                customerDocumentDTO.ValidateAll();
                if (customerDocumentDTO.HasErrors)
                    return BadRequest(customerDocumentDTO.ErrorMessages.ToString());

                var settings = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);
                if (settings == null || string.IsNullOrWhiteSpace(settings.FileUploadDirectory))
                    throw new InvalidOperationException("File upload directory is not configured (serviceBrokerConfiguration).");

                var updated = _customerDocumentAppService.UpdateCustomerDocument(customerDocumentDTO, settings.FileUploadDirectory, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(new { success = true, message = "", data = customerDocumentDTO });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
