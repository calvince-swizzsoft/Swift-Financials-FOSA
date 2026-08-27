using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.ApiErrors;
using WebApplication1.Services;

namespace WebApplication1.Areas.Roles
{
    [Authorize]
    [RoutePrefix("api/administration/roles")]
    public class RolesController : ApiController
    {
        private readonly RoleManagerService _roleManagerService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public RolesController(RoleManagerService roleMananagerService,
            Application.MainBoundedContext.Services.IEnumerationAppService enumerationAppService,
            IAuthorizationAppService authorizationAppService)
        {
            _roleManagerService = roleMananagerService;
            _authorizationAppService = authorizationAppService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index()
        {
            return Ok(_roleManagerService.GetAllRoles());
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] CreateRoleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "A role name is required.");

            if (!_roleManagerService.CreateRole(request.Name.Trim()))
                return Error(HttpStatusCode.Conflict, ErrorCodes.RoleCreateFailed,
                    "The role could not be created. A role with that name may already exist.");

            return Ok(true);
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddUserToRoles(AddUserToRolesRequest request)
        {
            if (!IsValidRoleRequest(request))
                return InvalidRoleRequest();

            if (!_roleManagerService.AddUserToRoles(request.UserName, request.Roles))
                return Error(HttpStatusCode.Conflict, ErrorCodes.RoleAssignmentFailed,
                    "The roles could not be assigned. Verify that the user and roles still exist.");

            return Ok(true);
        }

        [HttpPost]
        [Route("remove")]
        public IHttpActionResult RemoveUserFromRoles(AddUserToRolesRequest request)
        {
            if (!IsValidRoleRequest(request))
                return InvalidRoleRequest();

            if (!_roleManagerService.RemoveUserFromRoles(request.UserName, request.Roles))
                return Error(HttpStatusCode.Conflict, ErrorCodes.RoleAssignmentFailed,
                    "The roles could not be removed. Verify that the user and roles still exist.");

            return Ok(true);
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllUsersInRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "A role name is required.");

            return Ok(_roleManagerService.GetUsersInRole(role));
        }

        [HttpGet]
        [Route("permissiontypes")]
        public IHttpActionResult GetSystemPermissionTypes()
        {
            return Ok(Enum.GetNames(typeof(SystemPermissionType)));
        }

        [HttpGet]
        [Route("GetRolesForPermissionType")]
        public IHttpActionResult GetPermissionTypeInRoles(string permissionType)
        {
            SystemPermissionType selectedType;
            if (!Enum.TryParse(permissionType, true, out selectedType))
                return InvalidPermissionType();

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            return Ok(_authorizationAppService.GetRolesListForSystemPermissionType(
                (int)selectedType, serviceHeader));
        }

        [HttpPost]
        [Route("RemoveRolesFromPermissionType")]
        public IHttpActionResult RemovePermissionTypeFromRoles(PermissionRoleRequest requestBody)
        {
            SystemPermissionType selectedType;
            if (!TryValidatePermissionRequest(requestBody, out selectedType))
                return InvalidPermissionType();

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var roleNames = requestBody.PermissionTypeInRoles.Select(item => item.RoleName).ToArray();
            return Ok(_authorizationAppService.RemoveSystemPermissionTypeFromRoles(
                (int)selectedType, roleNames, serviceHeader));
        }

        [HttpPost]
        [Route("addPermissionTypeToRoles")]
        public IHttpActionResult AddPermissionTypeToRoles(PermissionRoleRequest requestBody)
        {
            SystemPermissionType selectedType;
            if (!TryValidatePermissionRequest(requestBody, out selectedType))
                return InvalidPermissionType();

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            return Ok(_authorizationAppService.AddSystemPermissionTypeToRoles(
                (int)selectedType, requestBody.PermissionTypeInRoles, serviceHeader));
        }

        private static bool IsValidRoleRequest(AddUserToRolesRequest request)
        {
            return request != null && !string.IsNullOrWhiteSpace(request.UserName) &&
                request.Roles != null && request.Roles.Any(role => !string.IsNullOrWhiteSpace(role));
        }

        private static bool TryValidatePermissionRequest(PermissionRoleRequest request,
            out SystemPermissionType selectedType)
        {
            selectedType = default(SystemPermissionType);
            return request != null &&
                Enum.TryParse(request.SystemPermissionType, true, out selectedType) &&
                request.PermissionTypeInRoles != null &&
                request.PermissionTypeInRoles.All(item => item != null && !string.IsNullOrWhiteSpace(item.RoleName));
        }

        private IHttpActionResult InvalidRoleRequest()
        {
            return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                "Username and at least one role are required.");
        }

        private IHttpActionResult InvalidPermissionType()
        {
            return Error(HttpStatusCode.BadRequest, ErrorCodes.InvalidPermissionType,
                "A valid system permission type and role list are required.");
        }

        private IHttpActionResult Error(HttpStatusCode statusCode, string code, string message)
        {
            return ResponseMessage(ApiErrorResponses.Create(Request, statusCode, code, message));
        }

        public class CreateRoleRequest
        {
            public string Name { get; set; }
        }

        public class AddUserToRolesRequest
        {
            public string UserName { get; set; }
            public string[] Roles { get; set; }
        }

        public class PermissionRoleRequest
        {
            public string SystemPermissionType { get; set; }

            // Preserve the existing JSON property used by the frontend.
            [Newtonsoft.Json.JsonProperty("permissionTypeinRoles")]
            public List<SystemPermissionTypeInRoleDTO> PermissionTypeInRoles { get; set; }
        }
    }
}
