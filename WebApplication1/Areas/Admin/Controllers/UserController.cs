using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.ApiErrors;
using WebApplication1.Areas.Identity.Services;

namespace WebApplication1.Areas.Identity.Controllers
{
    [Authorize]
    [RoutePrefix("api/administration/users")]
    public class UsersController : ApiController
    {
        private readonly UserManagerService _userManagerService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public UsersController(UserManagerService userManagerService,
            IAuthorizationAppService authorizationAppService)
        {
            _userManagerService = userManagerService;
            _authorizationAppService = authorizationAppService;
        }

        [HttpGet]
        [Route("roles")]
        public IHttpActionResult UserRoles(string user)
        {
            if (string.IsNullOrWhiteSpace(user))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "A username is required.");

            return Ok(_userManagerService.GetUserRoles(user));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index()
        {
            return Ok(_userManagerService.GetAllUsers());
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(UserDTO userDTO)
        {
            if (userDTO == null)
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "User data is required.");

            if (!_userManagerService.CreateUser(userDTO))
                return Error(HttpStatusCode.Conflict, ErrorCodes.UserCreateFailed,
                    "The user could not be created. The username may already exist or the password may not meet policy.");

            return Ok(true);
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Update(UserDTO userDTO)
        {
            if (userDTO == null || string.IsNullOrWhiteSpace(userDTO.UserName))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "User data and username are required.");

            if (!_userManagerService.UpdateUser(userDTO))
                return Error(HttpStatusCode.Conflict, ErrorCodes.UserUpdateFailed,
                    "The user could not be updated. Refresh the user list and try again.");

            return Ok(true);
        }

        [HttpPost]
        [Route("{userName}/reset-password")]
        public IHttpActionResult ResetPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "A username is required.");

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var allowedRoles = _authorizationAppService.GetRolesForSystemPermissionType(
                (int)SystemPermissionType.UserPasswordReset, serviceHeader) ?? new string[0];

            var permitted = serviceHeader.ApplicationUserRoles.Any(callerRole =>
                allowedRoles.Any(allowedRole => string.Equals(
                    callerRole, allowedRole, StringComparison.OrdinalIgnoreCase)));

            if (!permitted)
                return Error(HttpStatusCode.Forbidden, ErrorCodes.AccessDenied,
                    "You do not have permission to reset user passwords.");

            try
            {
                if (!_userManagerService.ResetUserPassword(userName))
                    return Error(HttpStatusCode.Conflict, ErrorCodes.PasswordResetFailed,
                        "The password could not be reset.");
            }
            catch (ArgumentException)
            {
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "A valid username is required.");
            }
            catch (InvalidOperationException)
            {
                return Error(HttpStatusCode.Conflict, ErrorCodes.PasswordResetFailed,
                    "The password reset could not be completed. Verify the user and email configuration.");
            }

            return Ok(new
            {
                message = "Password reset successfully. A temporary password has been queued to the user's email address."
            });
        }

        private IHttpActionResult Error(HttpStatusCode statusCode, string code, string message,
            IDictionary<string, string[]> validationErrors = null)
        {
            return ResponseMessage(ApiErrorResponses.Create(Request, statusCode, code, message, validationErrors));
        }

        public class UserRoleRequest
        {
            public string UserName { get; set; }
        }
    }
}
