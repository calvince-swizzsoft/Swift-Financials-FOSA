using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SalaryPeriodService : ISalaryPeriodService
    {
        private readonly ISalaryPeriodAppService _salaryPeriodAppService;

        public SalaryPeriodService(
            ISalaryPeriodAppService salaryPeriodAppService)
        {
            Guard.ArgumentNotNull(salaryPeriodAppService, nameof(salaryPeriodAppService));

            _salaryPeriodAppService = salaryPeriodAppService;
        }

        #region Salary Period

        public SalaryProcessingDTO AddSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.AddNewSalaryPeriod(salaryPeriodDTO, serviceHeader);
        }

        public bool UpdateSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.UpdateSalaryPeriod(salaryPeriodDTO, serviceHeader);
        }

        public bool ProcessSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, List<EmployeeDTO> employees)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.ProcessSalaryPeriod(salaryPeriodDTO, employees, serviceHeader);
        }

        public bool CloseSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.CloseSalaryPeriod(salaryPeriodDTO, serviceHeader);
        }

        public bool PostPaySlip(Guid paySlipId, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.PostPaySlip(paySlipId, moduleNavigationItemCode, serviceHeader);
        }

        public List<SalaryProcessingDTO> FindSalaryPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.FindSalaryPeriods(serviceHeader);
        }

        public PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.FindSalaryPeriods(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.FindSalaryPeriods(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public SalaryProcessingDTO FindSalaryPeriod(Guid salaryPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _salaryPeriodAppService.FindSalaryPeriod(salaryPeriodId, serviceHeader);
        }

        #endregion
    }
}
