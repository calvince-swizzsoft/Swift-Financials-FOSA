using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class NavigationItemService : INavigationItemService
    {
        private readonly INavigationItemAppService _navigationItemAppService;

        public NavigationItemService(INavigationItemAppService navigationItemAppService)
        {
            Guard.ArgumentNotNull(navigationItemAppService, nameof(navigationItemAppService));

            _navigationItemAppService = navigationItemAppService;
        }

        #region NavigationItemDTO

        public async Task<bool> AddNavigationItemsAsync(List<NavigationItemDTO> navigationItems)
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemAppService.AddNavigationItemsAsync(navigationItems, serviceHeader);
        }

        public Task<bool> BulkInsertNavigationItemAsync(List<Guid> navigationItemIds, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkInsertNavigationItemInRolesAsync(List<Guid> navigationItemInRole, string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NavigationItemDTO>> FindModuleNavigationActionByControllerNameAsync(string controllerName)
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemAppService.FindModuleNavigationActionByControllerNameAsync(controllerName, serviceHeader);
        }

        public async Task<NavigationItemDTO> FindNavigationItemByIdAsync(Guid navigationItemId)
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemAppService.FindNavigationItemAsync(navigationItemId, serviceHeader);
        }

        public async Task<NavigationItemDTO> FindNavigationItemByCodeAsync(int navigationItemCode)
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemAppService.FindNavigationItemAsync(navigationItemCode, serviceHeader);
        }

        public Task<List<NavigationItemDTO>> FindNavigationItemsAsync()
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _navigationItemAppService.FindNavigationItemsAsync(serviceHeader);
        }

        public Task<PageCollectionInfo<NavigationItemDTO>> FindNavigationItemsFilterPageCollectionInfoAsync(int pageIndex, int pageSize, List<string> sortedColumns, string text, bool sortAscending)
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _navigationItemAppService.FindNavigationItemsFilterPageCollectionInfoAsync(pageIndex, pageSize, sortedColumns, text, sortAscending, serviceHeader);
        }

        #endregion
    }
}