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
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveTypeAppService _leaveTypeAppService;

        public LeaveTypeService(ILeaveTypeAppService leaveTypeAppService)
        {
            Guard.ArgumentNotNull(leaveTypeAppService, nameof(leaveTypeAppService));

            _leaveTypeAppService = leaveTypeAppService;
        }

        public LeaveTypeDTO AddNewLeaveType(LeaveTypeDTO leaveTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveTypeAppService.AddNewLeaveType(leaveTypeDTO, serviceHeader);
        }

        public LeaveTypeDTO FindLeaveType(Guid leaveTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveTypeAppService.FindLeaveType(leaveTypeId, serviceHeader);
        }

        public List<LeaveTypeDTO> FindLeaveTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveTypeAppService.FindLeaveTypes(serviceHeader);
        }

        public PageCollectionInfo<LeaveTypeDTO> FindLeaveTypesFilterInPage(string filterText, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveTypeAppService.FindLeaveTypes(filterText, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateLeaveType(LeaveTypeDTO leaveTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveTypeAppService.UpdateLeaveType(leaveTypeDTO, serviceHeader);
        }
    }
}
