using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMembershipManagerService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        int CreateUser(EmployeeDTO employeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateUser(EmployeeDTO employeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDTO GetUserInfo(string userName);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDTO> GetUserInfoCollection();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDTO> GetUserInfoCollectionInPage(int pageIndex, int pageSize, string filter);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDTO> GetUserInfoCollectionInRole(string roleName);
    }
}
