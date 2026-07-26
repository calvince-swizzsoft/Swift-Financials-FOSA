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
    public class EmployeeAppraisalService : IEmployeeAppraisalService
    {
        private readonly IEmployeeAppraisalAppService _employeeAppraisalAppService;

        public EmployeeAppraisalService(IEmployeeAppraisalAppService employeeAppraisalAppService)
        {
            Guard.ArgumentNotNull(employeeAppraisalAppService, nameof(employeeAppraisalAppService));

            _employeeAppraisalAppService = employeeAppraisalAppService;
        }

        public bool AddNewEmployeeAppraisal(List<EmployeeAppraisalDTO> employeeAppraisalDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalAppService.AddNewEmployeeAppraisal(employeeAppraisalDTOs, serviceHeader);
        }

        public bool UpdateEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalAppService.UpdateEmployeeAppraisal(employeeAppraisalDTO, serviceHeader);
        }

        public bool AppraiseEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalAppService.AppraiseEmployeeAppraisal(employeeAppraisalDTO, serviceHeader);
        }

        public List<EmployeeAppraisalDTO> FindEmployeeAppraisalsByPeriod(Guid employeeId, Guid employeeAppraisalPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalAppService.FindEmployeeAppraisals(employeeId, employeeAppraisalPeriodId, serviceHeader);
        }

        public PageCollectionInfo<EmployeeAppraisalDTO> FindEmployeeAppraisalsByPeriodInPage(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalAppService.FindEmployeeAppraisals(employeeId, employeeAppraisalPeriodId, pageIndex, pageSize, serviceHeader);
        }
    }
}
