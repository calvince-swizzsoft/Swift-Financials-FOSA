using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeAppraisalTargetService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalTargetDTO AddEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(bool updateDepth, bool traverseTree);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargetsByFilterInPage(string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalTargetDTO FindEmployeeAppraisalTarget(Guid employeeAppraisalTargetId);
    }
}
