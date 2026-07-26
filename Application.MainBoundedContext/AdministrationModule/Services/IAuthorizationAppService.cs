using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IAuthorizationAppService
    {
        bool AddNewModuleNavigationItems(List<ModuleNavigationItemDTO> moduleNavigationItems, ServiceHeader serviceHeader);

        string[] GetRolesForModuleNavigationItemCode(int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool AddModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader);

        bool RemoveModuleNavigationItemFromRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader);

        bool IsModuleNavigationItemInRole(ModuleNavigationItemDTO moduleNavigationItem, string roleName, ServiceHeader serviceHeader);

        bool MapModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, ServiceHeader serviceHeader);

        List<ModuleNavigationItemInRoleDTO> GetModuleNavigationItemsInRole(string roleName, ServiceHeader serviceHeader);

        string[] GetRolesForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader);

        List<SystemPermissionTypeInRoleDTO> GetRolesListForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader);

        bool AddSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader);

        bool RemoveSystemPermissionTypeFromRoles(int systemPermissionType, string[] roleNames, ServiceHeader serviceHeader);

        bool IsSystemPermissionTypeInRole(int systemPermissionType, string roleName, ServiceHeader serviceHeader);

        bool MapSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, ServiceHeader serviceHeader);

        List<BranchDTO> GetBranchesForSystemPermissionType(int systemPermissionType, ServiceHeader serviceHeader);

        bool AddSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader);

        bool RemoveSystemPermissionTypeFromBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader);

        bool IsSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId, ServiceHeader serviceHeader);

        bool MapSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, ServiceHeader serviceHeader);

        List<SystemPermissionTypeInRoleDTO> GetSystemPermissionTypesInRole(string roleName, ServiceHeader serviceHeader);

        List<SystemPermissionTypeInRoleDTO> GetRolesAndApprovalPriorityByPermissionType(int systemPermissionType, ServiceHeader serviceHeader);

        List<SystemPermissionTypeInBranchDTO> GetSystemPermissionTypesInBranch(Guid branchId, ServiceHeader serviceHeader);

        bool MapEmployeeToBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader);

        List<BranchDTO> GetBranchesForEmployee(Guid employeeId, ServiceHeader serviceHeader);

        bool AddEmployeeToBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader);

        bool RemoveEmployeeFromBranches(Guid employeeId, BranchDTO[] branches, ServiceHeader serviceHeader);

        bool IsEmployeeInBranch(Guid employeeId, Guid branchId, ServiceHeader serviceHeader);
    }
}
