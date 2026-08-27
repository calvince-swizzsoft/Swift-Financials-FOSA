using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    [Authorize]
    [RoutePrefix("api/registry/division")]
    public class DivisionController : ApiController
    {
        private readonly IDivisionAppService _divisionAppService;
        private readonly IZoneAppService _zoneAppService;

        public DivisionController(IDivisionAppService divisionAppService, IZoneAppService zoneAppService)
        {
            _divisionAppService = divisionAppService ?? throw new ArgumentNullException(nameof(divisionAppService));
            _zoneAppService = zoneAppService ?? throw new ArgumentNullException(nameof(zoneAppService));
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
        public async Task<IHttpActionResult> Get(
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "")
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _divisionAppService.FindDivisionsAsync(pageIndex, pageSize, serviceHeader)
                    : await _divisionAppService.FindDivisionsAsync(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Divisions retrieved successfully", page);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var divisions = await _divisionAppService.FindDivisionsAsync(serviceHeader);
                return ApiResponse(true, "Divisions retrieved successfully", divisions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var division = await _divisionAppService.FindDivisionAsync(id, serviceHeader);
                if (division == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Division not found");

                return ApiResponse(true, "Division retrieved successfully", division);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}/zones")]
        public async Task<IHttpActionResult> GetZonesByDivisionId(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var zones = await _divisionAppService.FindZonesByDivisionIdAsync(id, serviceHeader);
                return ApiResponse(true, "Zones retrieved successfully", zones);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-employer/{employerId:guid}")]
        public async Task<IHttpActionResult> GetByEmployer(Guid employerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var divisions = await _divisionAppService.FindDivisionsByEmployerIdAsync(employerId, serviceHeader);
                return ApiResponse(true, "Divisions retrieved successfully", divisions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] DivisionDTO division)
        {
            try
            {
                if (division == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid division data");

                var serviceHeader = Utils.CreateServiceHeader();

                var createdDivision = await _divisionAppService.AddNewDivisionAsync(division, serviceHeader);

                if (createdDivision == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create division");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Division created successfully",
                    data = createdDivision
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] DivisionDTO division)
        {
            try
            {
                if (division == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid division data");

                if (id != division.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                var serviceHeader = Utils.CreateServiceHeader();

                var existingDivision = await _divisionAppService.FindDivisionAsync(id, serviceHeader);
                if (existingDivision == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Division not found");

                var updated = await _divisionAppService.UpdateDivisionAsync(division, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update division");

                var updatedDivision = await _divisionAppService.FindDivisionAsync(id, serviceHeader);
                return ApiResponse(true, "Division updated successfully", updatedDivision);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete, Route("{id:guid}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existingDivision = await _divisionAppService.FindDivisionAsync(id, serviceHeader);
                if (existingDivision == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Division not found");

                var removed = await _zoneAppService.RemoveDivisionAsync(id, serviceHeader);
                if (!removed)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove division");

                return ApiResponse(true, "Division removed successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
