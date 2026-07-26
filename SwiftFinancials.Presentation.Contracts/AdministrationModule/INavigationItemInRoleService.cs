using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "INavigationItemInRoleService")]
    public interface INavigationItemInRoleService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetNavigationItemsInRole(string roleName, AsyncCallback callback, Object state);
        List<NavigationItemInRoleDTO> EndGetNavigationItemsInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetRolesForNavigationItemCode(int navigationItemCode, AsyncCallback callback, Object state);
        List<NavigationItemInRoleDTO> EndGetRolesForNavigationItemCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsNavigationItemInRole(int navigationItemCode, string roleName, AsyncCallback callback, Object state);
        bool EndIsNavigationItemInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNavigationItemToRole(NavigationItemInRoleDTO navigationItemInRoleDTO, AsyncCallback callback, Object state);
        bool EndAddNavigationItemToRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveNavigationItemRole(Guid navigationItemId, string roleName, AsyncCallback callback, Object state);
        bool EndRemoveNavigationItemRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginValidateModuleAccess(string controllerName, string roleName, AsyncCallback callback, Object state);
        bool EndValidateModuleAccess(IAsyncResult result);
    }
}
