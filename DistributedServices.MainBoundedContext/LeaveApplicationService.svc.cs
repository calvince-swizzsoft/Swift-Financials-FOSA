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
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly ILeaveApplicationAppService _leaveApplicationAppService;

        public LeaveApplicationService(
            ILeaveApplicationAppService leaveApplicationAppService)
        {
            Guard.ArgumentNotNull(leaveApplicationAppService, nameof(leaveApplicationAppService));

            _leaveApplicationAppService = leaveApplicationAppService;
        }

        #region LeaveApplication

        public LeaveApplicationDTO AddLeaveApplication(LeaveApplicationDTO leaveApplicationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.AddNewLeaveApplication(leaveApplicationDTO, serviceHeader);
        }

        public bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.UpdateLeaveApplication(leaveApplicationDTO, serviceHeader);
        }

        public bool AuthorizeLeaveApplication(LeaveApplicationDTO leaveApplicationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.AuthorizeLeaveApplication(leaveApplicationDTO, serviceHeader);
        }

        public bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.RecallLeaveApplication(leaveApplicationDTO, serviceHeader);
        }

        public List<LeaveApplicationDTO> FindLeaveApplications()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplications(serviceHeader);
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplications(pageIndex, pageSize, serviceHeader);
        }

        public decimal FindEmployeeLeaveBalances(Guid employeeId, Guid leaveTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindEmployeeLeaveBalances(employeeId, leaveTypeId, serviceHeader);
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplications(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplications(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public LeaveApplicationDTO FindLeaveApplication(Guid leaveApplicationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplication(leaveApplicationId, serviceHeader);
        }

        public List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeId(Guid employeeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplicationsByEmployeeId(employeeId, serviceHeader);
        }

        public List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _leaveApplicationAppService.FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(employeeId, leaveTypeId, serviceHeader);
        }

        #endregion
    }
}
