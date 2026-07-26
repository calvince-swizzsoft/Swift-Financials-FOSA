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
    public class EmployeeAppraisalTargetService : IEmployeeAppraisalTargetService
    {
        private readonly IEmployeeAppraisalTargetAppService _employeeAppraisalTargetAppService;

        public EmployeeAppraisalTargetService(
            IEmployeeAppraisalTargetAppService employeeAppraisalTargetAppService)
        {
            Guard.ArgumentNotNull(employeeAppraisalTargetAppService, nameof(employeeAppraisalTargetAppService));

            _employeeAppraisalTargetAppService = employeeAppraisalTargetAppService;
        }

        #region EmployeeAppraisalTarget

        public EmployeeAppraisalTargetDTO AddEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.AddNewEmployeeAppraisalTarget(employeeAppraisalTargetDTO, serviceHeader);
        }

        public bool UpdateEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.UpdateEmployeeAppraisalTarget(employeeAppraisalTargetDTO, serviceHeader);
        }

        public List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(bool updateDepth, bool traverseTree)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindEmployeeAppraisalTargets(serviceHeader, updateDepth, traverseTree);
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindEmployeeAppraisalTargets(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindChildEmployeeAppraisalTargets(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargetsByFilterInPage(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindEmployeeAppraisalTargets(filter, pageIndex, pageSize, serviceHeader);
        }

        public EmployeeAppraisalTargetDTO FindEmployeeAppraisalTarget(Guid employeeAppraisalTargetId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindEmployeeAppraisalTarget(employeeAppraisalTargetId, serviceHeader);
        }

        public List<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _employeeAppraisalTargetAppService.FindChildEmployeeAppraisalTargets(serviceHeader);
        }

        #endregion
    }
}
