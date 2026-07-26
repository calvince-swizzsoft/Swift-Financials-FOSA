using DistributedServices.Seedwork.ErrorHandlers;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IRoleManagerService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        string[] GetUsersInRole(string role);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        string[] GetAllRoles();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CreateRole(string role);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveUserFromRoles(string userName, string[] roles);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddUserToRoles(string userName, string[] roles);
    }
}
