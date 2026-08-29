using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;
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

            var creationResult = _userManagerService.CreateUser(userDTO);
            if (!creationResult.Succeeded)
                return Error(HttpStatusCode.Conflict, ErrorCodes.UserCreateFailed,
                    creationResult.Errors.Any()
                        ? string.Join(" ", creationResult.Errors)
                        : "The user could not be created.",
                    new Dictionary<string, string[]>
                    {
                        { "user", creationResult.Errors.ToArray() }
                    });

            return Ok(true);
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Update(UpdateUserRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "User data and username are required.");

            // EmailConfirmed is deliberately absent from this administrator-edit contract. It is
            // identity-system state and must never be set by ordinary user-profile maintenance.
            var userDTO = new UserDTO
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                OtherNames = request.OtherNames,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                BranchId = request.BranchId,
                EmployeeId = request.EmployeeId,
                CustomerId = request.CustomerId,
                TwoFactorEnabled = request.TwoFactorEnabled,
                LockoutEnabled = request.LockoutEnabled
            };

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
            catch (InvalidOperationException exception)
            {
                return Error(HttpStatusCode.Conflict, ErrorCodes.PasswordResetFailed,
                    exception.Message);
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

        [HttpPost]
        [Route("{userName}/resend-email-confirmation")]
        public IHttpActionResult ResendEmailConfirmation(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed, "A username is required.");

            try
            {
                if (!_userManagerService.ResendEmailConfirmation(userName))
                    return Error(HttpStatusCode.Conflict, ErrorCodes.InvalidEmailConfirmation,
                        "A confirmation message was not queued. The user may already be confirmed, may not have an email address, or the message-queue configuration may be unavailable.");
            }
            catch (Exception exception)
            {
                var correlationId = CorrelationIdHandler.GetCorrelationId(Request);
                Trace.TraceError("Email confirmation resend failed. CorrelationId={0} UserName={1} Exception={2}", correlationId, userName, exception);
                return ResponseMessage(ApiErrorResponses.Create(Request, HttpStatusCode.ServiceUnavailable,
                    ErrorCodes.DependencyUnavailable,
                    "The confirmation link could not be queued. Verify that MSMQ and the Account Alert Dispatcher are running, then retry."));
            }

            return Ok(new { message = "A new confirmation email was created and queued for delivery." });
        }

        public class UpdateUserRequest
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string OtherNames { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public Guid? BranchId { get; set; }
            public Guid? EmployeeId { get; set; }
            public Guid? CustomerId { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public bool LockoutEnabled { get; set; }
        }
    }
}
