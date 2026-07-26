using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IUserManagerService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool Authenticate(string userName, string password);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsInRole(string userName, string role);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        string[] GetRoles(string userName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PermissionWrapperDTO> IsUserAuthorizedToAccessNavigationItems(string userName, List<PermissionWrapperDTO> permissionWrappers);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsUserAuthorizedToAccessNavigationItem(string userName, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsUserAuthorizedToAccessCustomerFile(string userName, Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool IsUserAuthorizedToAccessSystemPermissionType(string userName, int systemPermissionType, Guid? targetBranchId);
    }
}
