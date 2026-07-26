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
    public class EmployeeAppraisalPeriodService : IEmployeeAppraisalPeriodService
    {
        private readonly IEmployeeAppraisalPeriodAppService  _employeeAppraisalPeriodAppService;

        public EmployeeAppraisalPeriodService(
            IEmployeeAppraisalPeriodAppService employeeAppraisalPeriodAppService)
        {
            Guard.ArgumentNotNull(employeeAppraisalPeriodAppService, nameof(employeeAppraisalPeriodAppService));

             _employeeAppraisalPeriodAppService = employeeAppraisalPeriodAppService;
        }

        #region Employee Appraisal Period

        public EmployeeAppraisalPeriodDTO AddEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.AddNewEmployeeAppraisalPeriod(employeeAppraisalPeriodDTO, serviceHeader);
        }

        public bool UpdateEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.UpdateEmployeeAppraisalPeriod(employeeAppraisalPeriodDTO, serviceHeader);
        }

        public List<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.FindEmployeeAppraisalPeriods(serviceHeader);
        }

        public PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.FindEmployeeAppraisalPeriods(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriodsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.FindEmployeeAppraisalPeriods(text, pageIndex, pageSize, serviceHeader);
        }

        public EmployeeAppraisalPeriodDTO FindEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.FindEmployeeAppraisalPeriod(employeeAppraisalPeriodId, serviceHeader);
        }

        public EmployeeAppraisalPeriodDTO FindCurrentEmployeeAppraisalPeriod()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return  _employeeAppraisalPeriodAppService.FindCurrentEmployeeAppraisalPeriod(serviceHeader);
        }

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO
        public bool UpdateEmployeeAppraisalPeriodRecommendation(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalPeriodAppService.UpdateEmployeeAppraisalPeriodRecommendation(employeeAppraisalPeriodRecommendationDTO, serviceHeader);
        }

        public EmployeeAppraisalPeriodRecommendationDTO FindEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalPeriodAppService.FindEmployeeAppraisalPeriodRecommendation(employeeId, employeeAppraisalPeriodId, serviceHeader);
        }

        #endregion
    }
}
