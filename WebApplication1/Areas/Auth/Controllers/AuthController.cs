using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private readonly ApplicationUserManager _userManager;

        public AuthController(ApplicationUserManager userManager)
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

            // Usernames are normalized at account creation. Ignore accidental surrounding
            // whitespace in the username while preserving the password exactly as entered.
            var normalizedUserName = userDto.UserName.Trim();
            var user = await _userManager.FindByNameAsync(normalizedUserName);

            if ( user == null)
            {
                return InvalidCredentials();
            }

            var valid = await _userManager.CheckPasswordAsync(user, userDto.Password);


            if (!valid) 
            
            {
                return InvalidCredentials();
            }

            // Rollout compatibility: accounts created before confirmation was introduced never
            // received a token. After their password has been verified above, confirm them once.
            // Accounts created on/after the cutoff must always complete the emailed-token flow.
            DateTime confirmationCutoffUtc;
            var hasConfirmationCutoff = DateTime.TryParse(
                ConfigurationManager.AppSettings["Identity:EmailConfirmationEnforcedFromUtc"],
                out confirmationCutoffUtc);
            var predatesConfirmationRollout = !user.EmailConfirmed && hasConfirmationCutoff &&
                user.CreatedDate.ToUniversalTime() < confirmationCutoffUtc.ToUniversalTime();
            if (predatesConfirmationRollout)
            {
                user.EmailConfirmed = true;
                var legacyUpdate = await _userManager.UpdateAsync(user);
                if (!legacyUpdate.Succeeded)
                    throw new ApiException(HttpStatusCode.ServiceUnavailable, ErrorCodes.InternalError,
                        "The legacy user account could not be activated. Contact support with the correlation ID.");
            }

            if (!user.EmailConfirmed)
                return Error(HttpStatusCode.Forbidden, ErrorCodes.EmailNotConfirmed,
                    "Your email address has not been confirmed. Open the confirmation link sent when your user account was created.");

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

            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.PasswordChangeFailed,
                    "The current temporary password is incorrect.",
                    new Dictionary<string, string[]>
                    {
                        { "currentPassword", new[] { "Enter the temporary password issued when the user account was created." } }
                    });

            var passwordValidation = await _userManager.PasswordValidator.ValidateAsync(request.NewPassword);
            if (!passwordValidation.Succeeded)
            {
                var passwordErrors = passwordValidation.Errors
                    .Where(error => !string.IsNullOrWhiteSpace(error))
                    .Distinct()
                    .ToArray();
                return Error(HttpStatusCode.BadRequest, ErrorCodes.PasswordChangeFailed,
                    passwordErrors.Length > 0
                        ? string.Join(" ", passwordErrors)
                        : "The new password does not meet the password requirements.",
                    new Dictionary<string, string[]>
                    {
                        { "newPassword", passwordErrors.Length > 0 ? passwordErrors : new[] { "Choose a different password that meets every requirement." } }
                    });
            }

            // Password changes must not revalidate unrelated legacy profile data. In
            // particular, ChangePasswordAsync calls UpdateAsync internally and can reject
            // an otherwise valid password because an old account shares this email address.
            // Store all password workflow fields together so the change is atomic.
            try
            {
                using (var context = new ApplicationDbContext("AuthStore"))
                {
                    var persistedUser = context.Users.SingleOrDefault(item => item.Id == user.Id);
                    if (persistedUser == null)
                        throw new InvalidOperationException("The user no longer exists in AuthStore.");
                    persistedUser.PasswordHash = _userManager.PasswordHasher.HashPassword(request.NewPassword);
                    persistedUser.SecurityStamp = Guid.NewGuid().ToString();
                    persistedUser.LastPasswordChangedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                throw new ApiException(HttpStatusCode.ServiceUnavailable,
                    ErrorCodes.PasswordChangeOutcomeUnknown,
                    "The password change could not be recorded. Do not retry automatically; contact support with the correlation ID.",
                    innerException: exception);
            }

            var roles = await _userManager.GetRolesAsync(user.Id);
            var token = JwtTokenService.GenerateToken(user, roles);

            return Ok(new { token, user.UserName, roles, requiresPasswordChange = false });
        }

        [AllowAnonymous, HttpPost, Route("confirm-email")]
        public IHttpActionResult ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Code))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.InvalidEmailConfirmation,
                    "The email confirmation link is incomplete.");

            using (var context = new ApplicationDbContext("AuthStore"))
            {
                var user = context.Users.SingleOrDefault(item => item.Id == request.UserId);
                if (user == null)
                    return Error(HttpStatusCode.BadRequest, ErrorCodes.InvalidEmailConfirmation,
                        "The email confirmation link is invalid or has expired.");

                if (user.EmailConfirmed)
                    return Ok(new { message = "This email address is already confirmed." });

                if (!EmailConfirmationTokens.ValidateAndConsume(context, user, request.Code))
                    return Error(HttpStatusCode.BadRequest, ErrorCodes.InvalidEmailConfirmation,
                        "The email confirmation link is invalid or has expired. Ask an administrator to issue a new confirmation message.");
            }

            return Ok(new { message = "Your email address has been confirmed. You can now sign in." });
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

    public class ConfirmEmailRequest
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}
