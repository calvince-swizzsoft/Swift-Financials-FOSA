using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApplication1.Services;

namespace WebApplication1.Areas.Roles
{

    [RoutePrefix("api/administration/roles")]
    public class RolesController : ApiController
    {


        private readonly RoleManagerService _roleManagerService;

        public RolesController(RoleManagerService roleMananagerService)
        {
            _roleManagerService = roleMananagerService;
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

   

        public class CreateRoleRequest
        {
            public string Name { get; set; }
        }


        public class AddUserToRolesRequest
        {
            public string UserName { get; set; }

            public string[] Roles { get; set; }
        }



    }
}