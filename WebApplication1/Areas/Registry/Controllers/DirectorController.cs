using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // NavigationMenu.cs Code 21012 ("Directors", ControllerName "Director",
    // under Registry > Operations > Customers). IDirectorAppService was
    // already fully built and registered in DI — same gap and same fix
    // shape as DelegateController.cs (sibling nav entry, same pattern):
    // AddNewDirector enforces one director per customer but signals it by
    // returning null rather than throwing — Create below turns that into a
    // proper Conflict response instead of a bare 500.
    [Authorize]
    [RoutePrefix("api/registry/director")]
    public class DirectorController : ApiController
    {
        private readonly IDirectorAppService _directorAppService;

        public DirectorController(IDirectorAppService directorAppService)
        {
            _directorAppService = directorAppService ?? throw new ArgumentNullException(nameof(directorAppService));
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

                var page = _directorAppService.FindDirectors(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Directors retrieved successfully", page);
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
                var directors = _directorAppService.FindDirectors(serviceHeader);
                return ApiResponse(true, "Directors retrieved successfully", directors);
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

                var item = _directorAppService.FindDirector(id, serviceHeader);
                if (item == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Director not found");

                return ApiResponse(true, "Director retrieved successfully", item);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] DirectorDTO directorDto)
        {
            try
            {
                if (directorDto == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid director data");

                if (directorDto.DivisionId == Guid.Empty)
                    return ErrorResponse(HttpStatusCode.BadRequest, "A division is required");

                if (directorDto.CustomerId == Guid.Empty)
                    return ErrorResponse(HttpStatusCode.BadRequest, "A customer is required");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _directorAppService.AddNewDirector(directorDto, serviceHeader);

                if (created == null)
                    return ErrorResponse(HttpStatusCode.Conflict, "Sorry, this customer is already registered as a director.");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Director created successfully",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] DirectorDTO directorDto)
        {
            try
            {
                if (directorDto == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid director data");

                directorDto.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _directorAppService.FindDirector(id, serviceHeader);
                if (existing == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Director not found");

                var updated = _directorAppService.UpdateDirector(directorDto, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update director");

                var refreshed = _directorAppService.FindDirector(id, serviceHeader);
                return ApiResponse(true, "Director updated successfully", refreshed);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
