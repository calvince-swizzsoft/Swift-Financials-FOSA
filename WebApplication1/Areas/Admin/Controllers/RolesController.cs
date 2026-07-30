using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using WebApplication1.Services;

namespace WebApplication1.Areas.Roles
{

    [RoutePrefix("api/administration/roles")]
    public class RolesController : ApiController
    {


        private readonly RoleManagerService _roleManagerService;
        private readonly IEnumerationAppService _enumerationAppService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public RolesController(RoleManagerService roleMananagerService, IEnumerationAppService enumerationAppService, IAuthorizationAppService authorizationAppService)
        {
            _roleManagerService = roleMananagerService;
            _enumerationAppService = enumerationAppService;
            _authorizationAppService = authorizationAppService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index()
        {
            try
            {

                var roles = _roleManagerService.GetAllRoles();

                return Ok(roles);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] CreateRoleRequest request)

        {
            try
            {

                var result = _roleManagerService.CreateRole(request.Name);

                return Ok(result);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public IHttpActionResult AddUserToRoles(AddUserToRolesRequest request)

        {
            try
            {

                var result = _roleManagerService.AddUserToRoles(request.UserName, request.Roles);

                return Ok(result);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex); 
            }
        }

        [HttpPost]
        [Route("remove")]
        public IHttpActionResult RemoveUserFromRoles(AddUserToRolesRequest request)
        {

            try
            {
                var result = _roleManagerService.RemoveUserFromRoles(request.UserName, request.Roles);

                return Ok(result);
            }


            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllUsersInRole(string role)
        {

            try
            {
          
                var result = _roleManagerService.GetUsersInRole(role);

                return Ok(result); 
            }

            catch(Exception ex)
            {
                return InternalServerError(ex); 
            }
        }





        [HttpGet]
        [Route("permissiontypes")]
        public IHttpActionResult GetSystemPermissionTypes()

        {
            try
            {
                return Ok(Enum.GetNames(typeof(SystemPermissionType)));
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("GetRolesForPermissionType")]
        public IHttpActionResult GetPermissionTypeInRoles(string permissionType)
        {
            try
            {

                SystemPermissionType selectedType;

                Enum.TryParse(permissionType, out selectedType);

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                var rolesForPermissionType = _authorizationAppService.GetRolesListForSystemPermissionType((int)selectedType, serviceHeader);

                return Ok(rolesForPermissionType);

            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("RemoveRolesFromPermissionType")]
        public IHttpActionResult RemovePermissionTypeFromRoles(PermissionRoleRequest requestBody)
        {
            try
            {

                SystemPermissionType selectedType;

                Enum.TryParse(requestBody.SystemPermissionType, out selectedType);

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                List<string> list = new List<string>();

                foreach (var p in requestBody.permissionTypeinRoles)
                {
                    list.Add(p.RoleName);
                }

                string[] roleNames = list.ToArray();

                var success = _authorizationAppService.RemoveSystemPermissionTypeFromRoles((int)selectedType, list.ToArray(), serviceHeader);

                return Ok(success);

            }

            catch (Exception ex)
            {
                return InternalServerError(ex); 
            }
        }



        [HttpPost]
        [Route("addPermissionTypeToRoles")]
        public IHttpActionResult AddPermissionTypeToRoles(PermissionRoleRequest requestBody)
        {
            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                SystemPermissionType selectedType;
                Enum.TryParse(requestBody.SystemPermissionType, out selectedType);

                var result = _authorizationAppService.AddSystemPermissionTypeToRoles((int)selectedType, requestBody.permissionTypeinRoles, serviceHeader);

                return Ok(result);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
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


        public class PermissionRoleRequest {

            public string SystemPermissionType { get; set; }
            public List<SystemPermissionTypeInRoleDTO> permissionTypeinRoles { get; set; }
        }



    }
}