using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // NavigationMenu.cs Code 21015 ("Conditional Lendging" [sic],
    // ControllerName "ConditionalLending", under Registry > Operations >
    // Customers). IConditionalLendingAppService was already fully built and
    // registered in DI — same "group + entries" shape as
    // ChargeExemptionsController.cs (a ConditionalLending is a named group
    // tied to one loan product — which set of customers may take that
    // product; a ConditionalLendingEntry is one customer's membership).
    //
    // One real bug fixed alongside this: ConditionalLendingDTO extends
    // BindingModelBase<T> ([DataContract]) but had zero [DataMember] tags on
    // its own properties — same silent-blank-JSON bug CustomerDocumentDTO
    // and NextOfKinDTO had, fixed the same way (added [DataMember] to every
    // scalar property).
    //
    // AddNewConditionalLendingAsync signals "this loan product already has a
    // conditional lending group" by setting DTO.ErrorMessageResult (a plain
    // field, not a [DataMember] — never serializes) rather than throwing or
    // returning null — Create below checks it server-side and turns it into
    // a proper Conflict response.
    [Authorize]
    [RoutePrefix("api/registry/conditionallendings")]
    public class ConditionalLendingController : ApiController
    {
        private readonly IConditionalLendingAppService _conditionalLendingAppService;

        public ConditionalLendingController(IConditionalLendingAppService conditionalLendingAppService)
        {
            _conditionalLendingAppService = conditionalLendingAppService ?? throw new ArgumentNullException(nameof(conditionalLendingAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var items = await _conditionalLendingAppService.FindConditionalLendingsAsync(serviceHeader) ?? new List<ConditionalLendingDTO>();

                return Ok(new { success = true, message = "", data = items });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = await _conditionalLendingAppService.FindConditionalLendingsAsync(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var item = await _conditionalLendingAppService.FindConditionalLendingAsync(id, serviceHeader);

                if (item == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = item });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(ConditionalLendingDTO conditionalLending)
        {
            try
            {
                if (conditionalLending == null)
                    return BadRequest("Conditional lending payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _conditionalLendingAppService.AddNewConditionalLendingAsync(conditionalLending, serviceHeader);

                if (created == null)
                    throw new ApplicationException("Failed to create the conditional lending.");

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, new { success = false, message = created.ErrorMessageResult });

                return Ok(new { success = true, message = "", data = created });
            }
            catch (InvalidOperationException)
            {
                return BadRequest("The request could not be completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, ConditionalLendingDTO conditionalLending)
        {
            try
            {
                if (conditionalLending == null)
                    return BadRequest("Conditional lending payload is required.");

                conditionalLending.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _conditionalLendingAppService.UpdateConditionalLendingAsync(conditionalLending, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(new { success = true, message = "", data = await _conditionalLendingAppService.FindConditionalLendingAsync(id, serviceHeader) });
            }
            catch (InvalidOperationException)
            {
                return BadRequest("The request could not be completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> GetEntries(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = await _conditionalLendingAppService.FindConditionalLendingEntriesByConditionalLendingIdAsync(id, serviceHeader) ?? new List<ConditionalLendingEntryDTO>();

                return Ok(new { success = true, message = "", data = entries });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full replace on save — same stage-locally-then-commit pattern as
        // ChargeExemptionsController's entries endpoint, matching Areas/Registry/
        // Conditional Lendings.md's "Add button... repeat... then click create".
        [HttpPut]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> ReplaceEntries(Guid id, List<ConditionalLendingEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _conditionalLendingAppService.UpdateConditionalLendingEntryCollectionAsync(id, entries ?? new List<ConditionalLendingEntryDTO>(), serviceHeader);

                if (!updated)
                    return NotFound();

                var current = await _conditionalLendingAppService.FindConditionalLendingEntriesByConditionalLendingIdAsync(id, serviceHeader) ?? new List<ConditionalLendingEntryDTO>();

                return Ok(new { success = true, message = "", data = current });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
