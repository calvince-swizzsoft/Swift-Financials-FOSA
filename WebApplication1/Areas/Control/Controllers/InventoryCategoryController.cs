using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.InventoryModule.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Control.Controllers
{
    [Authorize]
    [RoutePrefix("api/control/inventory-categories")]
    public class InventoryCategoryController : ApiController
    {
        private readonly ICategoryAppService _categoryAppService;

        public InventoryCategoryController(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService ?? throw new ArgumentNullException(nameof(categoryAppService));
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? await _categoryAppService.FindCategoriesAsync(pageIndex, pageSize, serviceHeader)
                : await _categoryAppService.FindCategoriesAsync(text, pageIndex, pageSize, serviceHeader);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            var categories = await _categoryAppService.FindCategoriesAsync(Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "", data = categories ?? new List<CategoryDTO>() });
        }

        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var category = await _categoryAppService.FindCategoryAsync(id, Utils.CreateServiceHeader());
            return category == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data = category });
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(CategoryDTO category)
        {
            if (category == null) return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid inventory category data.", data = (object)null });
            try
            {
                var created = await _categoryAppService.AddNewCategoryAsync(category, Utils.CreateServiceHeader());
                return created == null
                    ? Content(HttpStatusCode.BadRequest, new { success = false, message = "The inventory category could not be created.", data = (object)null })
                    : (IHttpActionResult)Ok(new { success = true, message = "Inventory category created successfully.", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, CategoryDTO category)
        {
            if (category == null || category.Id != id) return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid inventory category data.", data = (object)null });
            if (await _categoryAppService.FindCategoryAsync(id, Utils.CreateServiceHeader()) == null) return NotFound();
            try
            {
                var updated = await _categoryAppService.UpdateCategoryAsync(category, Utils.CreateServiceHeader());
                if (!updated) return Content(HttpStatusCode.BadRequest, new { success = false, message = "The inventory category could not be updated.", data = (object)null });
                var refreshed = await _categoryAppService.FindCategoryAsync(id, Utils.CreateServiceHeader());
                return Ok(new { success = true, message = "Inventory category updated successfully.", data = refreshed });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }
}
