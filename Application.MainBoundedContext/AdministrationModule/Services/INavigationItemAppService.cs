using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface INavigationItemAppService
    {
        Task<bool> AddNavigationItemsAsync(List<NavigationItemDTO> navigationItems, ServiceHeader serviceHeader);

        Task<bool> BulkInsertNavigationItemAsync(List<Guid> navigationItemIds, string roleName, ServiceHeader serviceHeader);

        Task<List<NavigationItemDTO>> FindNavigationItemByControllerNameAndActionNameAsync(string controllerName, string actionName, ServiceHeader serviceHeader);

        Task<NavigationItemDTO> FindNavigationItemAsync(Guid navigationItemId, ServiceHeader serviceHeader);

        Task<NavigationItemDTO> FindNavigationItemAsync(int navigationItemCode, ServiceHeader serviceHeader);

        Task<List<NavigationItemDTO>> FindNavigationItemsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<NavigationItemDTO>> FindNavigationItemsFilterPageCollectionInfoAsync(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending, ServiceHeader serviceHeader);

        Task<List<NavigationItemDTO>> FindModuleNavigationActionByControllerNameAsync(string controllerName, ServiceHeader serviceHeader);
    }
}