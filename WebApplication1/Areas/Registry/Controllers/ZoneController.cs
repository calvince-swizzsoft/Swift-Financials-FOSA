using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.ApiErrors;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    [Authorize]
    [RoutePrefix("api/registry/zone")]
    public class ZoneController : ApiController
    {
        private readonly IZoneAppService _zoneAppService;

        public ZoneController(IZoneAppService zoneAppService)
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

        private IHttpActionResult ValidationError(string message)
        {
            return ResponseMessage(ApiErrorResponses.Create(
                Request,
                HttpStatusCode.BadRequest,
                ErrorCodes.ValidationFailed,
                message));
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
                    ? await _zoneAppService.FindZonesAsync(pageIndex, pageSize, serviceHeader)
                    : await _zoneAppService.FindZonesAsync(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Zones retrieved successfully", page);
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
                var zones = await _zoneAppService.FindZonesAsync(serviceHeader);
                return ApiResponse(true, "Zones retrieved successfully", zones);
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

                var zone = await _zoneAppService.FindZoneAsync(id, serviceHeader);
                if (zone == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Zone not found");

                return ApiResponse(true, "Zone retrieved successfully", zone);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}/stations")]
        public async Task<IHttpActionResult> GetStationsByZoneId(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var stations = await _zoneAppService.FindStationsByZoneIdAsync(id, serviceHeader);
                return ApiResponse(true, "Stations retrieved successfully", stations);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("stations")]
        public async Task<IHttpActionResult> GetStations()
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

        [HttpGet, Route("stations/{stationId:guid}")]
        public async Task<IHttpActionResult> GetStation(Guid stationId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var station = await _zoneAppService.FindStationAsync(stationId, serviceHeader);
                if (station == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Station not found");

                return ApiResponse(true, "Station retrieved successfully", station);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-employer/{employerId:guid}/stations")]
        public async Task<IHttpActionResult> GetStationsByEmployer(Guid employerId)
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

        [HttpGet, Route("by-division/{divisionId:guid}/stations")]
        public async Task<IHttpActionResult> GetStationsByDivision(Guid divisionId)
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

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateZoneRequest request)
        {
            try
            {
                if (request?.Zone == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid zone data");

                var serviceHeader = Utils.CreateServiceHeader();

                var zone = await _zoneAppService.AddNewZoneAsync(request.Zone, serviceHeader);

                if (zone == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create zone");

                if (request.Stations != null && request.Stations.Count > 0)
                    await _zoneAppService.UpdateStationsAsync(zone.Id, request.Stations, serviceHeader);

                var createdZone = await _zoneAppService.FindZoneAsync(zone.Id, serviceHeader);

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Zone created successfully",
                    data = createdZone
                });
            }
            catch (InvalidOperationException exception)
            {
                return ValidationError(exception.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] ZoneDTO zone)
        {
            try
            {
                if (zone == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid zone data");

                if (id != zone.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                var serviceHeader = Utils.CreateServiceHeader();

                var existingZone = await _zoneAppService.FindZoneAsync(id, serviceHeader);
                if (existingZone == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Zone not found");

                var updated = await _zoneAppService.UpdateZoneAsync(zone, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update zone");

                var updatedZone = await _zoneAppService.FindZoneAsync(id, serviceHeader);
                return ApiResponse(true, "Zone updated successfully", updatedZone);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}/stations")]
        public async Task<IHttpActionResult> UpdateStations(Guid id, [FromBody] List<StationDTO> stations)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existingZone = await _zoneAppService.FindZoneAsync(id, serviceHeader);
                if (existingZone == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Zone not found");

                var updated = await _zoneAppService.UpdateStationsAsync(id, stations ?? new List<StationDTO>(), serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update stations");

                var refreshed = await _zoneAppService.FindStationsByZoneIdAsync(id, serviceHeader);
                return ApiResponse(true, "Stations updated successfully", refreshed);
            }
            catch (InvalidOperationException exception)
            {
                return ValidationError(exception.Message);
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

                var existingZone = await _zoneAppService.FindZoneAsync(id, serviceHeader);
                if (existingZone == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Zone not found");

                var removed = await _zoneAppService.RemoveZoneAsync(id, serviceHeader);
                if (!removed)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove zone");

                return ApiResponse(true, "Zone removed successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete, Route("stations/{stationId:guid}")]
        public async Task<IHttpActionResult> DeleteStation(Guid stationId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existingStation = await _zoneAppService.FindStationAsync(stationId, serviceHeader);
                if (existingStation == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Station not found");

                var removed = await _zoneAppService.RemoveStationAsync(stationId, serviceHeader);
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

    public class CreateZoneRequest
    {
        public ZoneDTO Zone { get; set; }
        public List<StationDTO> Stations { get; set; }
    }
}
