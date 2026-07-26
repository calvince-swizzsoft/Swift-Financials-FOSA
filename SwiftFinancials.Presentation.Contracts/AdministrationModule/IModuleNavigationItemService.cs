using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "IModuleNavigationItemService")]
    public interface IModuleNavigationItemService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddModuleNavigationItems(List<ModuleNavigationItemDTO> moduleNavigationItems, AsyncCallback callback, Object state);
        bool EndAddModuleNavigationItems(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetRolesForModuleNavigationItemCode(int moduleNavigationItemCode, AsyncCallback callback, Object state);
        string[] EndGetRolesForModuleNavigationItemCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, AsyncCallback callback, Object state);
        bool EndAddModuleNavigationItemToRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveModuleNavigationItemFromRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, AsyncCallback callback, Object state);
        bool EndRemoveModuleNavigationItemFromRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapModuleNavigationItemToRoles(ModuleNavigationItemDTO moduleNavigationItem, string[] roleNames, AsyncCallback callback, Object state);
        bool EndMapModuleNavigationItemToRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetModuleNavigationItemsInRole(string roleName, AsyncCallback callback, Object state);
        List<ModuleNavigationItemInRoleDTO> EndGetModuleNavigationItemsInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetSystemPermissionTypesInRole(string roleName, AsyncCallback callback, Object state);
        List<SystemPermissionTypeInRoleDTO> EndGetSystemPermissionTypesInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetSystemPermissionTypesInBranch(Guid branchId, AsyncCallback callback, Object state);
        List<SystemPermissionTypeInBranchDTO> EndGetSystemPermissionTypesInBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsModuleNavigationItemInRole(ModuleNavigationItemDTO moduleNavigationItem, string roleName, AsyncCallback callback, Object state);
        bool EndIsModuleNavigationItemInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetRolesForSystemPermissionType(int systemPermissionType, AsyncCallback callback, Object state);
        string[] EndGetRolesForSystemPermissionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetRolesListForSystemPermissionType(int systemPermissionType, AsyncCallback callback, Object state);
        List<SystemPermissionTypeInRoleDTO> EndGetRolesListForSystemPermissionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, AsyncCallback callback, Object state);
        bool EndAddSystemPermissionTypeToRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveSystemPermissionTypeFromRoles(int systemPermissionType, string[] roleNames, AsyncCallback callback, Object state);
        bool EndRemoveSystemPermissionTypeFromRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsSystemPermissionTypeInRole(int systemPermissionType, string roleName, AsyncCallback callback, Object state);
        bool EndIsSystemPermissionTypeInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapSystemPermissionTypeToRoles(int systemPermissionType, List<SystemPermissionTypeInRoleDTO> rolesInSystemPermissionType, AsyncCallback callback, Object state);
        bool EndMapSystemPermissionTypeToRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetBranchesForSystemPermissionType(int systemPermissionType, AsyncCallback callback, Object state);
        List<BranchDTO> EndGetBranchesForSystemPermissionType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndAddSystemPermissionTypeToBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveSystemPermissionTypeFromBranches(int systemPermissionType, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndRemoveSystemPermissionTypeFromBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId, AsyncCallback callback, Object state);
        bool EndIsSystemPermissionTypeInBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapSystemPermissionTypeToBranches(int systemPermissionType, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndMapSystemPermissionTypeToBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetBranchesForEmployee(Guid employeeId, AsyncCallback callback, Object state);
        List<BranchDTO> EndGetBranchesForEmployee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployeeToBranches(Guid employeeId, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndAddEmployeeToBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveEmployeeFromBranches(Guid employeeId, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndRemoveEmployeeFromBranches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsEmployeeInBranch(Guid employeeId, Guid branchId, AsyncCallback callback, Object state);
        bool EndIsEmployeeInBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMapEmployeeToBranches(Guid employeeId, BranchDTO[] branches, AsyncCallback callback, Object state);
        bool EndMapEmployeeToBranches(IAsyncResult result);
    }
}
