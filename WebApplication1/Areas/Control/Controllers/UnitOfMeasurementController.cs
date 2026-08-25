using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.InventoryModule.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Control.Controllers
{
    [Authorize]
    [RoutePrefix("api/control/unitsofmeasurement")]
    public class UnitOfMeasurementController : ApiController
    {
        private readonly IUnitOfMeasurementAppService _unitOfMeasurementAppService;

        public UnitOfMeasurementController(IUnitOfMeasurementAppService unitOfMeasurementAppService)
        {
            _unitOfMeasurementAppService = unitOfMeasurementAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _unitOfMeasurementAppService.FindUnitOfMeasurementsAsync(pageIndex, pageSize, serviceHeader)
                    : await _unitOfMeasurementAppService.FindUnitOfMeasurementsAsync(text, pageIndex, pageSize, serviceHeader);

                if (page == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var units = await _unitOfMeasurementAppService.FindUnitOfMeasurementsAsync(serviceHeader);

                return Ok(new { success = true, message = "", data = units ?? new System.Collections.Generic.List<UnitOfMeasurementDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetUnitOfMeasurement(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var unit = await _unitOfMeasurementAppService.FindUnitOfMeasurementAsync(id, serviceHeader);

                if (unit == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = unit });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(UnitOfMeasurementDTO unitOfMeasurementDTO)
        {
            try
            {
                if (unitOfMeasurementDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid unit of measure data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _unitOfMeasurementAppService.AddNewUnitOfMeasurementAsync(unitOfMeasurementDTO, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the unit of measure could not be created.", data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (InvalidOperationException)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "The request could not be completed.", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, UnitOfMeasurementDTO unitOfMeasurementDTO)
        {
            try
            {
                if (unitOfMeasurementDTO == null || unitOfMeasurementDTO.Id != id)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid unit of measure data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = await _unitOfMeasurementAppService.FindUnitOfMeasurementAsync(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = await _unitOfMeasurementAppService.UpdateUnitOfMeasurementAsync(unitOfMeasurementDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update unit of measure", data = (object)null });
                }

                var refreshed = await _unitOfMeasurementAppService.FindUnitOfMeasurementAsync(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (InvalidOperationException)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "The request could not be completed.", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
