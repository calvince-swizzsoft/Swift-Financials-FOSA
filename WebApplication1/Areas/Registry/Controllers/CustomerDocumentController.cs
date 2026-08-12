using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // New, read-only — ICustomerDocumentAppService was already fully built
    // but had no controller anywhere. Built specifically to unblock the
    // loan-case registration screen's collateral-document picker (see
    // Areas/BackOffice/WORKFLOW.md §15.2 and §13): the reference
    // LoanRegistrationController resolves a customer's *released* collateral
    // documents via
    // FindCustomerDocumentsByCustomerIdAndTypeAsync(customerId,
    // CustomerDocumentType.Collateral) then filters client-side on
    // CollateralStatus.Released, and the loan-case Create endpoint already
    // takes a list of document ids resolved through this same app service —
    // this just exposes the picker's own read path.
    //
    // Deliberately NOT exposed: AddNewCustomerDocument/UpdateCustomerDocument
    // take a fileUploadDirectory parameter and are a real photo/document
    // upload feature (passport photos, ID scans, etc., alongside the raw-SQL
    // specimen-capture reads seen elsewhere in the reference app) — a
    // separate, larger piece of work, not needed to let a loan case pick an
    // already-existing collateral document. If document upload is wanted as
    // an API, that's its own pass.
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

                var document = _customerDocumentAppService.FindCustomerDocument(id, serviceHeader);

                if (document == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = document });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
