using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ILeaveApplicationAppService
    {
        LeaveApplicationDTO AddNewLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader);

        bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader);

        bool AuthorizeLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader);

        EmployeeLeaveStatisticsDTO GetEmployeeLeaveStatistics(Guid employeeId, Guid leaveTypeId, DateTime asAt, int pageIndex, ServiceHeader serviceHeader);

        LeavePreviewDTO PreviewLeave(Guid employeeId, Guid leaveTypeId, DateTime start, DateTime end, Guid? excludedId, ServiceHeader serviceHeader);
        bool WithdrawLeaveApplication(Guid id, ServiceHeader serviceHeader);
        void MarkLeaveNotificationQueued(Guid id, ServiceHeader serviceHeader);
        bool RetryLeaveNotification(Guid id, ServiceHeader serviceHeader);

        bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, ServiceHeader serviceHeader);

        List<LeaveApplicationDTO> FindLeaveApplications(ServiceHeader serviceHeader);

        List<LeaveApplicationDTO> FindActiveLeaveApplications(Guid employeeId, ServiceHeader serviceHeader);

        List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeId(Guid employeeId, ServiceHeader serviceHeader);

        List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader);

        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        decimal FindEmployeeLeaveBalances(Guid employeeId, Guid leaveTypeId, ServiceHeader serviceHeader);

        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplications(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LeaveApplicationDTO FindLeaveApplication(Guid leaveApplicationId, ServiceHeader serviceHeader);
    }
}
