using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using WebApplication1.Areas.Identity.Services;

namespace WebApplication1.Areas.Identity.Controllers
{

    [RoutePrefix("api/administration/users")]
    public class UsersController : ApiController
    {

        private readonly UserManagerService _userManagerService;
        private readonly IAuthorizationAppService _authorizationAppService;

       public UsersController(UserManagerService userManagerService, IAuthorizationAppService authorizationAppService)

        {

            _userManagerService = userManagerService;
            _authorizationAppService = authorizationAppService;

        }


        [HttpGet]
        [Route("roles")]
        public IHttpActionResult UserRoles(string user)
        {
            try
            {

                var users = _userManagerService.GetUserRoles(user);


                return Ok(users);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index ()
        {
            try
            {

                var users = _userManagerService.GetAllUsers();


                return Ok(users);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(UserDTO userDTO)
        {
            try
            {
                var success = _userManagerService.CreateUser(userDTO);   
                
                return Ok(success);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex); 
            }
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Update(UserDTO userDTO)
        {
            try
            {
                var success = _userManagerService.UpdateUser(userDTO);

                return Ok(success);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("{userName}/reset-password")]
        public IHttpActionResult ResetPassword(string userName)
        {
            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
                var allowedRoles = _authorizationAppService.GetRolesForSystemPermissionType(
                    (int)SystemPermissionType.UserPasswordReset,
                    serviceHeader) ?? new string[0];

                var permitted = serviceHeader.ApplicationUserRoles.Any(callerRole =>
                    allowedRoles.Any(allowedRole => string.Equals(callerRole, allowedRole, StringComparison.OrdinalIgnoreCase)));

                if (!permitted)
                    return StatusCode(HttpStatusCode.Forbidden);

                if (!_userManagerService.ResetUserPassword(userName))
                    return BadRequest("The password could not be reset.");

                return Ok(new
                {
                    message = "Password reset successfully. A temporary password has been queued to the user's email address."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class UserRoleRequest {

           public string UserName { get; set; }
        }


    }
}
