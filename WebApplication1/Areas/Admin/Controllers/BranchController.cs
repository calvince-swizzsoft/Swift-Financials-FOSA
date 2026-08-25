using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Authorize]
    [RoutePrefix("api/administration/branches")]
    public class BranchController : ApiController
    {
        private readonly IBranchAppService _branchAppService;

        public BranchController(IBranchAppService branchAppService)
        {
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
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
        public IHttpActionResult Get([FromUri] int pageIndex = 0, [FromUri] int pageSize = 20, [FromUri] string text = "")
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _branchAppService.FindBranches(pageIndex, pageSize, serviceHeader)
                    : _branchAppService.FindBranches(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Branches retrieved successfully", page);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var branches = _branchAppService.FindBranches(serviceHeader);

                return ApiResponse(true, "Branches retrieved successfully", branches);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var branch = _branchAppService.FindBranch(id, serviceHeader);
                if (branch == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Branch not found");

                return ApiResponse(true, "Branch retrieved successfully", branch);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-code/{code:int}")]
        public IHttpActionResult GetByCode(int code)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var branch = _branchAppService.FindBranch(code, serviceHeader);
                if (branch == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Branch not found");

                return ApiResponse(true, "Branch retrieved successfully", branch);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-company/{companyId:guid}")]
        public IHttpActionResult GetByCompany(Guid companyId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var branches = _branchAppService.FindBranchesByCompanyId(companyId, serviceHeader);

                return ApiResponse(true, "Branches retrieved successfully", branches ?? Enumerable.Empty<BranchDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] BranchDTO branch)
        {
            try
            {
                if (branch == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid branch data");

                branch.ValidateAll();
                if (branch.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join(" ", branch.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var createdBranch = _branchAppService.AddNewBranch(branch, serviceHeader);
                if (createdBranch == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create branch");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Branch created successfully",
                    data = createdBranch
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] BranchDTO branch)
        {
            try
            {
                if (branch == null || branch.Id != id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid branch data");

                branch.ValidateAll();
                if (branch.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join(" ", branch.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var existingBranch = _branchAppService.FindBranch(id, serviceHeader);
                if (existingBranch == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Branch not found");

                var updated = _branchAppService.UpdateBranch(branch, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update branch");

                var updatedBranch = _branchAppService.FindBranch(id, serviceHeader);
                return ApiResponse(true, "Branch updated successfully", updatedBranch);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPatch, Route("{id:guid}/toggle-lock")]
        public IHttpActionResult ToggleLock(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var branch = _branchAppService.FindBranch(id, serviceHeader);
                if (branch == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Branch not found");

                branch.IsLocked = !branch.IsLocked;

                var updated = _branchAppService.UpdateBranch(branch, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update branch");

                var updatedBranch = _branchAppService.FindBranch(id, serviceHeader);
                return ApiResponse(true, $"Branch {(updatedBranch.IsLocked ? "locked" : "unlocked")} successfully", updatedBranch);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
