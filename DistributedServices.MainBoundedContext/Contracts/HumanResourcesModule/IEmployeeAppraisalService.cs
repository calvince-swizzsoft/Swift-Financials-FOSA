using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeAppraisalService
    {
        #region Employee Appraisal

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddNewEmployeeAppraisal(List<EmployeeAppraisalDTO> employeeAppraisalDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AppraiseEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalDTO> FindEmployeeAppraisalsByPeriodInPage(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeAppraisalDTO> FindEmployeeAppraisalsByPeriod(Guid employeeId, Guid employeeAppraisalPeriodId);

        #endregion
    }
}
