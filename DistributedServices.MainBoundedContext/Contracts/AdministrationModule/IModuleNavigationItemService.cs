using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IModuleNavigationItemService
    {
        #region Module Navigation Item

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddModuleNavigationItems(List<ModuleNavigationItemDTO> moduleNavigationItems);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        string[] GetRolesForModuleNavigationItemCode(int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveModuleNavigationItemFromRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ModuleNavigationItemInRoleDTO> GetModuleNavigationItemsInRole(string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SystemPermissionTypeInRoleDTO> GetSystemPermissionTypesInRole(string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SystemPermissionTypeInBranchDTO> GetSystemPermissionTypesInBranch(Guid branchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsModuleNavigationItemInRole(ModuleNavigationItemDTO moduleNavigationItem, string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        string[] GetRolesForSystemPermissionType(int systemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SystemPermissionTypeInRoleDTO> GetRolesListForSystemPermissionType(int systemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveSystemPermissionTypeFromRoles(int systemPermissionType, string[] roleNames);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsSystemPermissionTypeInRole(int systemPermissionType, string roleName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BranchDTO> GetBranchesForSystemPermissionType(int systemPermissionType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveSystemPermissionTypeFromBranches(int systemPermissionType, BranchDTO[] branches);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BranchDTO> GetBranchesForEmployee(Guid employeeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddEmployeeToBranches(Guid employeeId, BranchDTO[] branches);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveEmployeeFromBranches(Guid employeeId, BranchDTO[] branches);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsEmployeeInBranch(Guid employeeId, Guid branchId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MapEmployeeToBranches(Guid employeeId, BranchDTO[] branches);

        #endregion
    }
}
