using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EmployeeExitService : IEmployeeExitService
    {
        private readonly IEmployeeExitAppService _employeeExitAppService;

        public EmployeeExitService(IEmployeeExitAppService employeeExitAppService)
        {
            Guard.ArgumentNotNull(employeeExitAppService, nameof(employeeExitAppService));

            _employeeExitAppService = employeeExitAppService;
        }

        public EmployeeExitDTO AddNewEmployeeExit(EmployeeExitDTO employeeExitDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeExitAppService.AddNewEmployeeExit(employeeExitDTO, serviceHeader);
        }

        public bool VerifyEmployeeExit(EmployeeExitDTO employeeExitDTO, int auditOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeExitAppService.VerifyEmployeeExit(employeeExitDTO, auditOption, serviceHeader);
        }

        public bool ApproveEmployeeExit(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeExitAppService.ApproveEmployeeExit(employeeExitDTO, authorizationOption, moduleNavigationItemCode, serviceHeader);
        }

        public PageCollectionInfo<EmployeeExitDTO> FindEmployeeExitsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeExitAppService.FindEmployeeExits(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateEmployeeExit(EmployeeExitDTO employeeExitDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeExitAppService.UpdateEmployeeExit(employeeExitDTO, serviceHeader);
        }
    }
}
