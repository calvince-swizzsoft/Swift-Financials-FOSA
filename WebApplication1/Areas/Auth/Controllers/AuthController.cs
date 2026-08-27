using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using WebApplication1.Areas.Admin.Services;
using WebApplication1.Areas.Identity;
using WebApplication1.ApiErrors;

namespace WebApplication1.Areas.Auth
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        //private readonly ITokenService _tokenService


        //[HttpPost, Route("register")]
        //public async Task<IHttpActionResult> Register(UserDTO userDto)
        //{

        //    var user = new ApplicationUser
        //    {
        //        UserName = userDto.UserName,
        //        Email = userDto.Email, 
        //        EmployeeId = userDto.EmployeeId
        //    };

        //    var result = await _userManager.CreateAsync(user, userDto.Password);

        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors); 
        //    }

        //    return Ok();
        //}



        [AllowAnonymous, HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]LoginRequest userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.UserName) ||
                string.IsNullOrWhiteSpace(userDto.Password))
            {
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "Username and password are required.",
                    new Dictionary<string, string[]>
                    {
                        { "userName", string.IsNullOrWhiteSpace(userDto?.UserName) ? new[] { "Username is required." } : new string[0] },
                        { "password", string.IsNullOrWhiteSpace(userDto?.Password) ? new[] { "Password is required." } : new string[0] }
                    }.Where(x => x.Value.Length > 0).ToDictionary(x => x.Key, x => x.Value));
            }

            var user = await _userManager.FindByNameAsync(userDto.UserName);

            if ( user == null)
            {
                return InvalidCredentials();
            }

            var valid = await _userManager.CheckPasswordAsync(user, userDto.Password);


            if (!valid) 
            
            {
                return InvalidCredentials();
            }

            if (!user.LastPasswordChangedDate.HasValue)
            {
                return Ok(new
                {
                    requiresPasswordChange = true,
                    user.UserName
                });
            }


            var roles = await _userManager.GetRolesAsync(user.Id);
            var token = JwtTokenService.GenerateToken(user, roles);

            return Ok(new
            {
                token,
                user.UserName,
                roles
            });
        }

        [AllowAnonymous, HttpPost, Route("change-initial-password")]
        public async Task<IHttpActionResult> ChangeInitialPassword([FromBody] ChangeInitialPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "Username, current password, and new password are required.");

            if (request.NewPassword != request.ConfirmPassword)
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                    "The new password and confirmation password do not match.",
                    new Dictionary<string, string[]>
                    {
                        { "confirmPassword", new[] { "The passwords do not match." } }
                    });

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.LastPasswordChangedDate.HasValue)
                return Error(HttpStatusCode.Conflict, ErrorCodes.InitialPasswordChangeNotAllowed,
                    "This initial password-change request is no longer valid.");

            var result = await _userManager.ChangePasswordAsync(user.Id, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return Error(HttpStatusCode.BadRequest, ErrorCodes.PasswordChangeFailed,
                    "The password could not be changed. Verify the current password and password requirements.");

            user.LastPasswordChangedDate = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ApiException(HttpStatusCode.ServiceUnavailable,
                    ErrorCodes.PasswordChangeOutcomeUnknown,
                    "The password change could not be confirmed. Do not retry automatically; contact support with the correlation ID.");

            var roles = await _userManager.GetRolesAsync(user.Id);
            var token = JwtTokenService.GenerateToken(user, roles);

            return Ok(new { token, user.UserName, roles, requiresPasswordChange = false });
        }

        private IHttpActionResult InvalidCredentials()
        {
            return Error(HttpStatusCode.Unauthorized, ErrorCodes.InvalidCredentials,
                "The username or password is incorrect.");
        }

        private IHttpActionResult Error(HttpStatusCode statusCode, string code, string message,
            IDictionary<string, string[]> validationErrors = null)
        {
            return ResponseMessage(ApiErrorResponses.Create(Request, statusCode, code, message, validationErrors));
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ChangeInitialPasswordRequest
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
