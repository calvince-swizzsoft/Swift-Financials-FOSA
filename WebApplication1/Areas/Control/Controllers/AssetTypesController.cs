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
    [RoutePrefix("api/control/assettypes")]
    public class AssetTypesController : ApiController
    {
        private readonly IAssetTypeAppService _assetTypeAppService;

        public AssetTypesController(IAssetTypeAppService assetTypeAppService)
        {
            _assetTypeAppService = assetTypeAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _assetTypeAppService.FindAssetTypesAsync(pageIndex, pageSize, serviceHeader)
                    : await _assetTypeAppService.FindAssetTypesAsync(text, pageIndex, pageSize, serviceHeader);

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

                var assetTypes = await _assetTypeAppService.FindAssetTypesAsync(serviceHeader);

                return Ok(new { success = true, message = "", data = assetTypes ?? new System.Collections.Generic.List<AssetTypeDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetAssetType(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var assetType = await _assetTypeAppService.FindAssetTypeAsync(id, serviceHeader);

                if (assetType == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = assetType });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(AssetTypeDTO assetTypeDTO)
        {
            try
            {
                if (assetTypeDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid asset type data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _assetTypeAppService.AddNewAssetTypeAsync(assetTypeDTO, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the asset type could not be created.", data = (object)null });
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
        public async Task<IHttpActionResult> Update(Guid id, AssetTypeDTO assetTypeDTO)
        {
            try
            {
                if (assetTypeDTO == null || assetTypeDTO.Id != id)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid asset type data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = await _assetTypeAppService.FindAssetTypeAsync(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = await _assetTypeAppService.UpdateAssetTypeAsync(assetTypeDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update asset type", data = (object)null });
                }

                var refreshed = await _assetTypeAppService.FindAssetTypeAsync(id, serviceHeader);

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
