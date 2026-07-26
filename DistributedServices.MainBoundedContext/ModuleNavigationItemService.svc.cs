using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ModuleNavigationItemService : IModuleNavigationItemService
    {
        private readonly IAuthorizationAppService _authorizationAppService;

        public ModuleNavigationItemService(
            IAuthorizationAppService authorizationAppService)
        {
            Guard.ArgumentNotNull(authorizationAppService, nameof(authorizationAppService));

            _authorizationAppService = authorizationAppService;
        }

        #region Module Navigation Item

        public bool AddModuleNavigationItems(List<ModuleNavigationItemDTO> moduleNavigationItems)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.AddNewModuleNavigationItems(moduleNavigationItems, serviceHeader);
        }

        public string[] GetRolesForModuleNavigationItemCode(int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetRolesForModuleNavigationItemCode(moduleNavigationItemCode, serviceHeader);
        }

        public bool AddModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.AddModuleNavigationItemToRoles(moduleNavigationItem, roleNames, serviceHeader);
        }

        public bool RemoveModuleNavigationItemFromRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.RemoveModuleNavigationItemFromRoles(moduleNavigationItem, roleNames, serviceHeader);
        }

        public bool IsModuleNavigationItemInRole(ModuleNavigationItemDTO moduleNavigationItem, string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.IsModuleNavigationItemInRole(moduleNavigationItem, roleName, serviceHeader);
        }

        public bool MapModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.MapModuleNavigationItemToRoles(moduleNavigationItem, roleNames, serviceHeader);
        }

        public string[] GetRolesForSystemPermissionType(int systemPermissionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetRolesForSystemPermissionType(systemPermissionType, serviceHeader);
        }

        public List<SystemPermissionTypeInRoleDTO> GetRolesListForSystemPermissionType(int systemPermissionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetRolesListForSystemPermissionType(systemPermissionType, serviceHeader);
        }

        public List<ModuleNavigationItemInRoleDTO> GetModuleNavigationItemsInRole(string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetModuleNavigationItemsInRole(roleName, serviceHeader);
        }

        public List<SystemPermissionTypeInRoleDTO> GetSystemPermissionTypesInRole(string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetSystemPermissionTypesInRole(roleName, serviceHeader);
        }

        public List<SystemPermissionTypeInBranchDTO> GetSystemPermissionTypesInBranch(Guid branchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetSystemPermissionTypesInBranch(branchId, serviceHeader);
        }

        public bool AddSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.AddSystemPermissionTypeToRoles(systemPermissionType, rolesInSystemPermissionType, serviceHeader);
        }

        public bool RemoveSystemPermissionTypeFromRoles(int systemPermissionType, string[] roleNames)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.RemoveSystemPermissionTypeFromRoles(systemPermissionType, roleNames, serviceHeader);
        }

        public bool IsSystemPermissionTypeInRole(int systemPermissionType, string roleName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.IsSystemPermissionTypeInRole(systemPermissionType, roleName, serviceHeader);
        }

        public bool MapSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.MapSystemPermissionTypeToRoles(systemPermissionType, rolesInSystemPermissionType, serviceHeader);
        }

        public List<BranchDTO> GetBranchesForSystemPermissionType(int systemPermissionType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetBranchesForSystemPermissionType(systemPermissionType, serviceHeader);
        }

        public bool AddSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.AddSystemPermissionTypeToBranches(systemPermissionType, branches, serviceHeader);
        }

        public bool RemoveSystemPermissionTypeFromBranches(int systemPermissionType, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.RemoveSystemPermissionTypeFromBranches(systemPermissionType, branches, serviceHeader);
        }

        public bool IsSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.IsSystemPermissionTypeInBranch(systemPermissionType, branchId, serviceHeader);
        }

        public bool MapSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.MapSystemPermissionTypeToBranches(systemPermissionType, branches, serviceHeader);
        }

        public List<BranchDTO> GetBranchesForEmployee(Guid employeeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.GetBranchesForEmployee(employeeId, serviceHeader);
        }

        public bool AddEmployeeToBranches(Guid employeeId, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.AddEmployeeToBranches(employeeId, branches, serviceHeader);
        }

        public bool RemoveEmployeeFromBranches(Guid employeeId, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.RemoveEmployeeFromBranches(employeeId, branches, serviceHeader);
        }

        public bool IsEmployeeInBranch(Guid employeeId, Guid branchId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.IsEmployeeInBranch(employeeId, branchId, serviceHeader);
        }

        public bool MapEmployeeToBranches(Guid employeeId, BranchDTO[] branches)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _authorizationAppService.MapEmployeeToBranches(employeeId, branches, serviceHeader);
        }

        #endregion
    }
}
