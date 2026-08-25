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
    [RoutePrefix("api/control/packagetypes")]
    public class PackageTypeController : ApiController
    {
        private readonly IPackageTypeAppService _packageTypeAppService;

        public PackageTypeController(IPackageTypeAppService packageTypeAppService)
        {
            _packageTypeAppService = packageTypeAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _packageTypeAppService.FindPackageTypesAsync(pageIndex, pageSize, serviceHeader)
                    : await _packageTypeAppService.FindPackageTypesAsync(text, pageIndex, pageSize, serviceHeader);

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

                var packageTypes = await _packageTypeAppService.FindPackageTypesAsync(serviceHeader);

                return Ok(new { success = true, message = "", data = packageTypes ?? new System.Collections.Generic.List<PackageTypeDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetPackageType(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var packageType = await _packageTypeAppService.FindPackageTypeAsync(id, serviceHeader);

                if (packageType == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = packageType });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(PackageTypeDTO packageTypeDTO)
        {
            try
            {
                if (packageTypeDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid package type data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _packageTypeAppService.AddNewPackageTypeAsync(packageTypeDTO, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the package type could not be created.", data = (object)null });
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
        public async Task<IHttpActionResult> Update(Guid id, PackageTypeDTO packageTypeDTO)
        {
            try
            {
                if (packageTypeDTO == null || packageTypeDTO.Id != id)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid package type data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = await _packageTypeAppService.FindPackageTypeAsync(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = await _packageTypeAppService.UpdatePackageTypeAsync(packageTypeDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update package type", data = (object)null });
                }

                var refreshed = await _packageTypeAppService.FindPackageTypeAsync(id, serviceHeader);

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
