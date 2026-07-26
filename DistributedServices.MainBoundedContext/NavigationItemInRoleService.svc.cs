using Application.MainBoundedContext.AdministrationModule.Services;
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
    public class NavigationItemInRoleService : INavigationItemInRoleService
    {
        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;

        public NavigationItemInRoleService(INavigationItemInRoleAppService navigationItemInRoleAppService)
        {
            Guard.ArgumentNotNull(navigationItemInRoleAppService, nameof(navigationItemInRoleAppService));

            _navigationItemInRoleAppService = navigationItemInRoleAppService;
        }

        public async Task<bool> AddNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.AddNavigationItemToRoleAsync(navigationItemInRoleDTO, serviceHeader);
        }

        public async Task<List<NavigationItemInRoleDTO>> GetNavigationItemsInRoleAsync(string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.GetNavigationItemsInRoleAsync(roleName, serviceHeader);
        }

        public async Task<string[]> GetRolesForNavigationItemCodeAsync(int navigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.GetRolesForNavigationItemCodeAsync(navigationItemCode, serviceHeader);
        }

        public async Task<bool> IsNavigationItemInRoleAsync(int navigationItemCode, string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.IsNavigationItemInRoleAsync(navigationItemCode, roleName, serviceHeader);
        }

        public async Task<bool> RemoveNavigationItemRoleAsync(Guid navigationItemId, string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.RemoveNavigationItemRoleAsync(navigationItemId, roleName, serviceHeader);
        }

        public async Task<bool> ValidateModuleAccessAsync(string controllerName, string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _navigationItemInRoleAppService.ValidateModuleAccessAsync(controllerName, roleName, serviceHeader);
        }
    }
}