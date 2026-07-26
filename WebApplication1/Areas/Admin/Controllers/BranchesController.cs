using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

using WebApplication1.Services;

namespace WebApplication1.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/administration/branches")]
    public class BranchesController : ApiController
    {
        private readonly BranchService _service = new BranchService();

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var branches = _service.GetAll();
                return ApiResponse(true, "Branches retrieved successfully", branches);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var branch = _service.GetById(id);
                if (branch == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Branch not found" });

                return ApiResponse(true, "Branch retrieved successfully", branch);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-company/{companyId:guid}")]
        public IHttpActionResult GetByCompany(Guid companyId)
        {
            try
            {
                var branches = _service.GetByCompanyId(companyId);
                return ApiResponse(true, "Branches retrieved successfully", branches);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-code/{code:int}")]
        public IHttpActionResult GetByCode(int code)
        {
            try
            {
                var branch = _service.GetByCode(code);
                if (branch == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Branch not found" });

                return ApiResponse(true, "Branch retrieved successfully", branch);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] BranchDTO branch)
        {
            try
            {
                if (branch == null)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Invalid branch data" });

                // Validate CompanyId exists
                if (!_service.CompanyExists(branch.CompanyId))
                {
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Company does not exist" });
                }

                var createdBranch = _service.Create(branch);

                return Content(System.Net.HttpStatusCode.Created,
                               new
                               {
                                   success = true,
                                   message = "Branch created successfully",
                                   data = createdBranch
                               });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] BranchDTO branch)
        {
            try
            {
                if (branch == null || branch.Id != id)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Invalid branch data" });

                // Validate CompanyId exists if being changed
                if (!_service.CompanyExists(branch.CompanyId))
                {
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Company does not exist" });
                }

                _service.Update(branch);
                return ApiResponse(true, "Branch updated successfully", branch);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpDelete, Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
                return ApiResponse(true, "Branch deleted successfully");
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpPatch, Route("{id:guid}/toggle-lock")]
        public IHttpActionResult ToggleLock(Guid id)
        {
            try
            {
                var branch = _service.GetById(id);
                if (branch == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Branch not found" });

                branch.IsLocked = !branch.IsLocked;
                _service.Update(branch);

                return ApiResponse(true, $"Branch {(branch.IsLocked ? "locked" : "unlocked")} successfully", branch);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }
    }
}