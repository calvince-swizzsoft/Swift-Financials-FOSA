using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ISalaryPeriodService
    {
        #region Salary Period

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryProcessingDTO AddSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ProcessSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, List<EmployeeDTO> employees);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CloseSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostPaySlip(Guid paySlipId, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<SalaryProcessingDTO> FindSalaryPeriods();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriodsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        SalaryProcessingDTO FindSalaryPeriod(Guid salaryPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        #endregion
    }
}
