using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeExitService
    {
        #region EmployeeExitDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeExitDTO AddNewEmployeeExit(EmployeeExitDTO employeeExitDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeExit(EmployeeExitDTO employeeExitDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool VerifyEmployeeExit(EmployeeExitDTO employeeExitDTO, int auditOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ApproveEmployeeExit(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeExitDTO> FindEmployeeExitsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        #endregion
    }
}
