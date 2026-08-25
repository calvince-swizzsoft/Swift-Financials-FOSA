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
    [RoutePrefix("api/registry/station")]
    public class StationController : ApiController
    {
        private readonly IZoneAppService _zoneAppService;

        public StationController(IZoneAppService zoneAppService)
        {
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
                    ? await _zoneAppService.FindStationsAsync(pageIndex, pageSize, serviceHeader)
                    : await _zoneAppService.FindStationsAsync(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Stations retrieved successfully", page);
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
                var stations = await _zoneAppService.FindStationsAsync(serviceHeader);
                return ApiResponse(true, "Stations retrieved successfully", stations);
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

                var station = await _zoneAppService.FindStationAsync(id, serviceHeader);
                if (station == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Station not found");

                return ApiResponse(true, "Station retrieved successfully", station);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-zone/{zoneId:guid}")]
        public async Task<IHttpActionResult> GetByZone(Guid zoneId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var stations = await _zoneAppService.FindStationsByZoneIdAsync(zoneId, serviceHeader);
                return ApiResponse(true, "Stations retrieved successfully", stations);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-division/{divisionId:guid}")]
        public async Task<IHttpActionResult> GetByDivision(Guid divisionId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var stations = await _zoneAppService.FindStationsByDivisionIdAsync(divisionId, serviceHeader);
                return ApiResponse(true, "Stations retrieved successfully", stations);
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
                var stations = await _zoneAppService.FindStationsByEmployerIdAsync(employerId, serviceHeader);
                return ApiResponse(true, "Stations retrieved successfully", stations);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] StationDTO station)
        {
            try
            {
                if (station == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid station data");

                var serviceHeader = Utils.CreateServiceHeader();

                var createdStation = await _zoneAppService.AddNewStationAsync(station, serviceHeader);

                if (createdStation == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create station");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Station created successfully",
                    data = createdStation
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] StationDTO station)
        {
            try
            {
                if (station == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid station data");

                if (id != station.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                var serviceHeader = Utils.CreateServiceHeader();

                var existingStation = await _zoneAppService.FindStationAsync(id, serviceHeader);
                if (existingStation == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Station not found");

                var updated = await _zoneAppService.UpdateStationAsync(station, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update station");

                var updatedStation = await _zoneAppService.FindStationAsync(id, serviceHeader);
                return ApiResponse(true, "Station updated successfully", updatedStation);
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

                var existingStation = await _zoneAppService.FindStationAsync(id, serviceHeader);
                if (existingStation == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Station not found");

                var removed = await _zoneAppService.RemoveStationAsync(id, serviceHeader);
                if (!removed)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove station");

                return ApiResponse(true, "Station removed successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
