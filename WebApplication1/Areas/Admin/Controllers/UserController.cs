using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Linq;
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

       public UsersController(UserManagerService userManagerService)

        {

            _userManagerService = userManagerService;

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



        public class UserRoleRequest {

           public string UserName { get; set; }
        }


    }
}