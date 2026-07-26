using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface INavigationItemInRoleAppService
    {
        Task<bool> AddNavigationItemToRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader);

        Task<bool> AddNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO, ServiceHeader serviceHeader);

        Task<bool> MapNavigationItemToRoleAsync(NavigationItemInRoleDTO navigationItemInRoleDTO, ServiceHeader serviceHeader);

        Task<bool> MapNavigationItemToRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader);

        Task<bool> IsNavigationItemInRoleAsync(int navigationItemCode, string roleName, ServiceHeader serviceHeader);

        Task<List<NavigationItemInRoleDTO>> GetNavigationItemsInRoleAsync(string roleName, ServiceHeader serviceHeader);

        Task<bool> RemoveNavigationItemFromRolesAsync(NavigationItemDTO navigationItemDTO, string[] roleNames, ServiceHeader serviceHeader);

        Task<bool> RemoveNavigationItemRoleAsync(Guid navigationItemId, string roleName, ServiceHeader serviceHeader);

        Task<string[]> GetRolesForNavigationItemCodeAsync(int navigationItemCode, ServiceHeader serviceHeader);

        Task<bool> ValidateModuleAccessAsync(string controllerName, string roleName, ServiceHeader serviceHeader);
    }
}