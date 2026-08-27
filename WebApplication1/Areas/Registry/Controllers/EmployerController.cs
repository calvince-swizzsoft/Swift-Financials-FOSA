using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    [Authorize]
    [RoutePrefix("api/registry/employer")]
    public class EmployerController : ApiController
    {
        private readonly IEmployerAppService _employerAppService;
        private readonly IZoneAppService _zoneAppService;

        public EmployerController(IEmployerAppService employerAppService, IZoneAppService zoneAppService)
        {
            _employerAppService = employerAppService ?? throw new ArgumentNullException(nameof(employerAppService));
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
                    ? await _employerAppService.FindEmployersAsync(pageIndex, pageSize, serviceHeader)
                    : await _employerAppService.FindEmployersAsync(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Employers retrieved successfully", page);
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
                var employers = await _employerAppService.FindEmployersAsync(serviceHeader);
                return ApiResponse(true, "Employers retrieved successfully", employers);
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

                var employer = await _employerAppService.FindEmployerAsync(id, serviceHeader);
                if (employer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Employer not found");

                return ApiResponse(true, "Employer retrieved successfully", employer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}/divisions")]
        public async Task<IHttpActionResult> GetDivisions(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var divisions = await _employerAppService.FindDivisionsByEmployerIdAsync(id, serviceHeader);
                return ApiResponse(true, "Divisions retrieved successfully", divisions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("divisions/{divisionId:guid}")]
        public async Task<IHttpActionResult> GetDivision(Guid divisionId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var division = await _employerAppService.FindDivisionAsync(divisionId, serviceHeader);
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
        public async Task<IHttpActionResult> GetZones(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var zones = await _employerAppService.FindZonesAsync(id, serviceHeader);
                return ApiResponse(true, "Zones retrieved successfully", zones);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateEmployerRequest request)
        {
            try
            {
                if (request?.Employer == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid employer data");

                var serviceHeader = Utils.CreateServiceHeader();

                var createdEmployer = await _employerAppService.AddNewEmployerAsync(request.Employer, serviceHeader);

                if (createdEmployer == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create employer");

                var divisions = request.Divisions != null && request.Divisions.Count > 0
                    ? request.Divisions
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .Select(name => new DivisionDTO { EmployerId = createdEmployer.Id, Description = name })
                        .ToList()
                    : new List<DivisionDTO> { new DivisionDTO { EmployerId = createdEmployer.Id, Description = createdEmployer.Description } };

                await _employerAppService.UpdateDivisionsAsync(createdEmployer.Id, divisions, serviceHeader);

                var createdDivisions = await _employerAppService.FindDivisionsByEmployerIdAsync(createdEmployer.Id, serviceHeader);

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Employer created successfully",
                    data = new { employer = createdEmployer, divisions = createdDivisions }
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] EmployerDTO employer)
        {
            try
            {
                if (employer == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid employer data");

                if (id != employer.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                var serviceHeader = Utils.CreateServiceHeader();

                var existingEmployer = await _employerAppService.FindEmployerAsync(id, serviceHeader);
                if (existingEmployer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Employer not found");

                var updated = await _employerAppService.UpdateEmployerAsync(employer, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update employer");

                var updatedEmployer = await _employerAppService.FindEmployerAsync(id, serviceHeader);
                return ApiResponse(true, "Employer updated successfully", updatedEmployer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}/divisions")]
        public async Task<IHttpActionResult> UpdateDivisions(Guid id, [FromBody] List<DivisionDTO> divisions)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existingEmployer = await _employerAppService.FindEmployerAsync(id, serviceHeader);
                if (existingEmployer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Employer not found");

                var updated = await _employerAppService.UpdateDivisionsAsync(id, divisions ?? new List<DivisionDTO>(), serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update divisions");

                var refreshed = await _employerAppService.FindDivisionsByEmployerIdAsync(id, serviceHeader);
                return ApiResponse(true, "Divisions updated successfully", refreshed);
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

                var existingEmployer = await _employerAppService.FindEmployerAsync(id, serviceHeader);
                if (existingEmployer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Employer not found");

                var removed = await _zoneAppService.RemoveEmployerAsync(id, serviceHeader);
                if (!removed)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove employer");

                return ApiResponse(true, "Employer removed successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CreateEmployerRequest
    {
        public EmployerDTO Employer { get; set; }
        public List<string> Divisions { get; set; }
    }
}
