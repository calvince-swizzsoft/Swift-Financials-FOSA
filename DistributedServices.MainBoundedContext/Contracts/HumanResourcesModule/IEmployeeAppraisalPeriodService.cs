using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeAppraisalPeriodService
    {
        #region Employee Appraisal Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalPeriodDTO AddEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriodsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalPeriodDTO FindEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalPeriodDTO FindCurrentEmployeeAppraisalPeriod();

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeAppraisalPeriodRecommendation(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeAppraisalPeriodRecommendationDTO FindEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId);

        #endregion
    }
}
