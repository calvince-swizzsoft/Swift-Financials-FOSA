using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MembershipModule
{
    [ServiceContract(Name = "IUserManagerService")]
    public interface IUserManagerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthenticate(string userName, string password, AsyncCallback callback, Object state);
        bool EndAuthenticate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsInRole(string userName, string role, AsyncCallback callback, Object state);
        bool EndIsInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetRoles(string userName, AsyncCallback callback, Object state);
        string[] EndGetRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsUserAuthorizedToAccessNavigationItems(string userName, List<PermissionWrapperDTO> permissionWrappers, AsyncCallback callback, Object state);
        List<PermissionWrapperDTO> EndIsUserAuthorizedToAccessNavigationItems(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsUserAuthorizedToAccessNavigationItem(string userName, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndIsUserAuthorizedToAccessNavigationItem(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsUserAuthorizedToAccessCustomerFile(string userName, Guid customerId, AsyncCallback callback, Object state);
        bool EndIsUserAuthorizedToAccessCustomerFile(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginIsUserAuthorizedToAccessSystemPermissionType(string userName, int systemPermissionType, Guid? targetBranchId, AsyncCallback callback, Object state);
        bool EndIsUserAuthorizedToAccessSystemPermissionType(IAsyncResult result);
    }
}
