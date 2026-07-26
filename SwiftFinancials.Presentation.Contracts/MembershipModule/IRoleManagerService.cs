using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MembershipModule
{
    [ServiceContract(Name = "IRoleManagerService")]
    public interface IRoleManagerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetUsersInRole(string role, AsyncCallback callback, Object state);
        string[] EndGetUsersInRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetAllRoles(AsyncCallback callback, Object state);
        string[] EndGetAllRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCreateRole(string role, AsyncCallback callback, Object state);
        bool EndCreateRole(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveUserFromRoles(string userName, string[] roles, AsyncCallback callback, Object state);
        bool EndRemoveUserFromRoles(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddUserToRoles(string userName, string[] roles, AsyncCallback callback, Object state);
        bool EndAddUserToRoles(IAsyncResult result);
    }
}
