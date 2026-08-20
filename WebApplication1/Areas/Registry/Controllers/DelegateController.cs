using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // NavigationMenu.cs Code 21011 ("Delegates", ControllerName "Delegate",
    // under Registry > Operations > Customers). IDelegateAppService was
    // already fully built and registered in DI — this is a thin wrapper,
    // same envelope/helper shape as ZoneController.cs. Note two things the
    // app service does NOT do, confirmed by reading it directly:
    // - AddNewDelegate enforces one delegate per customer, but signals it
    //   by returning null rather than throwing — Create below turns that
    //   into a proper Conflict response instead of a bare 500.
    // - Areas/Registry/Delegates.md's "a customer must belong to a zone in
    //   order to be created as a delegate" is NOT enforced anywhere
    //   server-side (not here, not in DelegateFactory, no DB constraint) —
    //   left as-is rather than inventing a new validation rule that wasn't
    //   asked for; it's presented as informational guidance on the create
    //   screen instead.
    [Authorize]
    [RoutePrefix("api/registry/delegate")]
    public class DelegateController : ApiController
    {
        private readonly IDelegateAppService _delegateAppService;

        public DelegateController(IDelegateAppService delegateAppService)
        {
            _delegateAppService = delegateAppService ?? throw new ArgumentNullException(nameof(delegateAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "")
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _delegateAppService.FindDelegates(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Delegates retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var delegates = _delegateAppService.FindDelegates(serviceHeader);
                return ApiResponse(true, "Delegates retrieved successfully", delegates);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var item = _delegateAppService.FindDelegate(id, serviceHeader);
                if (item == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Delegate not found");

                return ApiResponse(true, "Delegate retrieved successfully", item);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] DelegateDTO delegateDto)
        {
            try
            {
                if (delegateDto == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid delegate data");

                if (delegateDto.ZoneId == Guid.Empty)
                    return ErrorResponse(HttpStatusCode.BadRequest, "A zone is required");

                if (delegateDto.CustomerId == Guid.Empty)
                    return ErrorResponse(HttpStatusCode.BadRequest, "A customer is required");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _delegateAppService.AddNewDelegate(delegateDto, serviceHeader);

                if (created == null)
                    return ErrorResponse(HttpStatusCode.Conflict, "Sorry, this customer is already registered as a delegate.");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Delegate created successfully",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] DelegateDTO delegateDto)
        {
            try
            {
                if (delegateDto == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid delegate data");

                delegateDto.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _delegateAppService.FindDelegate(id, serviceHeader);
                if (existing == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Delegate not found");

                var updated = _delegateAppService.UpdateDelegate(delegateDto, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update delegate");

                var refreshed = _delegateAppService.FindDelegate(id, serviceHeader);
                return ApiResponse(true, "Delegate updated successfully", refreshed);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
