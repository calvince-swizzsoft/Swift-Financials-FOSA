using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Areas.Identity;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/administration/modules")]
    public class ModulesController : ApiController
    {

        private readonly IAuthorizationAppService _authorizationAppService;

        private readonly INavigationItemAppService _navigationItemAppService;

        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;

        private readonly ApplicationUserManager _applicationUserManager;


        public ModulesController(IAuthorizationAppService authorizationAppService,
            INavigationItemAppService navigationItemAppService,
            INavigationItemInRoleAppService navigationItemInRoleAppService,
            ApplicationUserManager applicationUserManager
            )
        {
            _authorizationAppService = authorizationAppService;
            _navigationItemAppService = navigationItemAppService;
            _navigationItemInRoleAppService = navigationItemInRoleAppService;
            _applicationUserManager = applicationUserManager;
        }


        [HttpGet]

        [Route("")]
        public async Task<IHttpActionResult> GetModuleNavigationNodes()
        {
            // var nodes = new List<JsTreeModel>();

            //var navigationItems = await _channelService.FindNavigationItemsAsync(GetServiceHeader());

            try
            {

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                var navigationItems = await _navigationItemAppService.FindNavigationItemsAsync(serviceHeader);

                return Ok(navigationItems);

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

        }


  

        // The role to populate modules for is resolved server-side from the caller's validated identity -
        // a client-supplied role name is never trusted here, even though one may still be present on the
        // querystring for older callers.
        [HttpGet, Route("by-role")]
        public async Task<IHttpActionResult> GetNavigationItemsByRole(string role = null)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();

                var itemsByRole = await Task.WhenAll(callerRoles.Select(r => _navigationItemInRoleAppService.GetNavigationItemsInRoleAsync(r, serviceHeader)));

                var result = itemsByRole
                    .Where(items => items != null)
                    .SelectMany(items => items)
                    .GroupBy(item => item.NavigationItemId)
                    .Select(group => group.First())
                    .ToList();

                return Json(result);
            }

            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

        }




        [HttpPost, Route("remove-from-role")]
        public async Task<IHttpActionResult> RemoveNavigationItemFromRole(NavigationItemToRoleViewModel NavigationItemToRoleViewModel)
        {


            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            
            List<Guid> navigationItemIds = new List<Guid>();


            foreach (var navId in NavigationItemToRoleViewModel.NavigationItemId)
            {
                if (navId == null || navId == Guid.Empty)
                {
                    return (BadRequest("Invalid Module Id sent"));
                }

                if (NavigationItemToRoleViewModel.RoleName == null || NavigationItemToRoleViewModel.RoleName == "")
                {
                    return BadRequest("rolename cannot be empty");
                }

                navigationItemIds.Add(navId);
            }


            try
            {

                var result = await _navigationItemInRoleAppService.RemoveNavigationItemsInRoleAsync(navigationItemIds, NavigationItemToRoleViewModel.RoleName, serviceHeader); 

            
                //if (result)
                //    await LoadModuleAccessRights(HttpContext.User.Identity.Name);

                return Json(result);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }



        [HttpPost, Route("add-to-role")]
        public async Task<IHttpActionResult> AddNavigationItemToRole(NavigationItemToRoleViewModel NavigationItemToRoleViewModel)
        {


            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            List<NavigationItemInRoleDTO> navigationItemInRoleDTOs = new List<NavigationItemInRoleDTO>();


            foreach (var navId in NavigationItemToRoleViewModel.NavigationItemId)
            {
              
                var navigationItemInRoleDTO = new NavigationItemInRoleDTO
                {
                    NavigationItemId = navId,
                    RoleName = NavigationItemToRoleViewModel.RoleName
                };

                navigationItemInRoleDTO.ValidateAll();

                if (navigationItemInRoleDTO.HasErrors)
                {

                    return BadRequest(string.Join("\n", navigationItemInRoleDTO.ErrorMessages));
                }

                navigationItemInRoleDTOs.Add(navigationItemInRoleDTO);
            }


            try
            {

                var result = await _navigationItemInRoleAppService.AddNavigationItemsToRoleAsync(navigationItemInRoleDTOs, serviceHeader);


                //if (result)
                //    await LoadModuleAccessRights(HttpContext.User.Identity.Name);

                return Json(result);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }



        public async Task LoadModuleAccessRights(string username)
        {

            var currentUser = await _applicationUserManager.FindByNameAsync(username);

            var roles = await _applicationUserManager.GetRolesAsync(currentUser.Id);

            if (roles.Any())
            {

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

           
                //var navigationItems = new List<NavigationItemDTO>();

                foreach (var role in roles)
                {
                    var items = await _navigationItemInRoleAppService
                        .GetNavigationItemsInRoleAsync(role, serviceHeader);

                  //  navigationItems.AddRange(items);
                }
            }
        }



    }
    public class NavigationItemToRoleViewModel
    {
        public List<Guid> NavigationItemId { get; set; }

        public string RoleName { get; set; }
    }


}