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
    [RoutePrefix("api/control/suppliers")]
    public class SupplierController : ApiController
    {
        private readonly ISupplierAppService _supplierAppService;

        public SupplierController(ISupplierAppService supplierAppService)
        {
            _supplierAppService = supplierAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _supplierAppService.FindSuppliersAsync(pageIndex, pageSize, serviceHeader)
                    : await _supplierAppService.FindSuppliersAsync(text, pageIndex, pageSize, serviceHeader);

                if (page == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var suppliers = await _supplierAppService.FindSuppliersAsync(serviceHeader);

                return Ok(new { success = true, message = "", data = suppliers ?? new System.Collections.Generic.List<SupplierDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetSupplier(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var supplier = await _supplierAppService.FindSupplierAsync(id, serviceHeader);

                if (supplier == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = supplier });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(SupplierDTO supplierDTO)
        {
            try
            {
                if (supplierDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid supplier data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _supplierAppService.AddNewSupplierAsync(supplierDTO, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the supplier could not be created.", data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, SupplierDTO supplierDTO)
        {
            try
            {
                if (supplierDTO == null || supplierDTO.Id != id)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid supplier data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = await _supplierAppService.FindSupplierAsync(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = await _supplierAppService.UpdateSupplierAsync(supplierDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update supplier", data = (object)null });
                }

                var refreshed = await _supplierAppService.FindSupplierAsync(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
