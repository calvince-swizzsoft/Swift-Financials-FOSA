using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using WebApplication1.Areas.Admin.Services;
using WebApplication1.Areas.Identity;

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



        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]LoginRequest userDto)
        {

            var user = await _userManager.FindByNameAsync(userDto.UserName);

            if ( user == null)
            {
                return Unauthorized();
            }

            var valid = await _userManager.CheckPasswordAsync(user, userDto.Password);


            if (!valid) 
            
            {
                return Unauthorized();     
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

        [HttpPost, Route("change-initial-password")]
        public async Task<IHttpActionResult> ChangeInitialPassword([FromBody] ChangeInitialPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Username, current password, and new password are required.");

            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest("The new password and confirmation password do not match.");

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.LastPasswordChangedDate.HasValue)
                return BadRequest("This initial password-change request is no longer valid.");

            var result = await _userManager.ChangePasswordAsync(user.Id, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return BadRequest(string.Join("; ", result.Errors));

            user.LastPasswordChangedDate = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(string.Join("; ", updateResult.Errors));

            var roles = await _userManager.GetRolesAsync(user.Id);
            var token = JwtTokenService.GenerateToken(user, roles);

            return Ok(new { token, user.UserName, roles, requiresPasswordChange = false });
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
