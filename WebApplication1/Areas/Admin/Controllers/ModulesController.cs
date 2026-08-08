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

                var grantedRows = itemsByRole
                    .Where(items => items != null)
                    .SelectMany(items => items)
                    .GroupBy(item => item.NavigationItemId)
                    .Select(group => group.First())
                    .ToList();

                // Role grants only ever cover the concrete items an admin explicitly checked -
                // never their ancestor "IsArea" folder nodes automatically. But the client
                // rebuilds its tree purely from each item's own Code/AreaCode links within
                // whatever array it's handed (it never trusts a server-computed Children field),
                // so a granted leaf whose ancestor folder was never itself granted has nothing to
                // attach to and renders as a false top-level root - the client-side tree only
                // ever cascades correctly when the array it's given is a subset of the FULL
                // Code/AreaCode graph, not a disconnected bag of leaves.
                //
                // Fix: treat the grant list as a prune over the full catalogue rather than the
                // final answer. Walk every granted item's ParentId chain up through the full
                // catalogue and splice in every ancestor it passes through, so what's returned
                // is exactly the full list's own graph with everything NOT on a path to a
                // granted item removed - "as if it was the whole list, only pruned."
                var allItems = await _navigationItemAppService.FindNavigationItemsAsync(serviceHeader);
                var byId = allItems.ToDictionary(i => i.Id);

                var includedIds = new HashSet<Guid>(grantedRows.Select(g => g.NavigationItemId));
                var ancestorQueue = new Queue<Guid>(includedIds);

                while (ancestorQueue.Count > 0)
                {
                    var id = ancestorQueue.Dequeue();
                    if (!byId.TryGetValue(id, out var item) || !item.ParentId.HasValue) continue;

                    var parentId = item.ParentId.Value;
                    if (includedIds.Add(parentId)) ancestorQueue.Enqueue(parentId);
                }

                var grantedById = grantedRows.ToDictionary(g => g.NavigationItemId);

                var result = includedIds
                    .Where(id => byId.ContainsKey(id))
                    .Select(id => grantedById.TryGetValue(id, out var granted) ? granted : ToAncestorRow(byId[id]))
                    .ToList();

                return Json(result);
            }

            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // An ancestor folder pulled in only to keep the tree connected, not an actual
        // grant - no real NavigationItemInRole row backs it, so Id/RoleName/CreatedBy
        // are synthetic. The client only ever reads the NavigationItem* fields to
        // rebuild its tree (Code/AreaCode/Description/IsArea/ControllerName/ActionName),
        // never these, so their placeholder values are safe.
        private static NavigationItemInRoleDTO ToAncestorRow(NavigationItemDTO item)
        {
            return new NavigationItemInRoleDTO
            {
                Id = Guid.Empty,
                NavigationItemId = item.Id,
                NavigationItemDescription = item.Description,
                navigationItemCode = item.Code,
                NavigationItemIcon = item.Icon,
                NavigationItemControllerName = item.ControllerName,
                NavigationItemActionName = item.ActionName,
                NavigationItemParentId = item.ParentId,
                NavigationItemAreaName = item.AreaName,
                NavigationItemAreaCode = item.AreaCode,
                NavigationItemIsArea = item.IsArea,
                RoleName = "(ancestor)",
                CreatedBy = "system",
                CreatedDate = item.CreatedDate,
            };
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