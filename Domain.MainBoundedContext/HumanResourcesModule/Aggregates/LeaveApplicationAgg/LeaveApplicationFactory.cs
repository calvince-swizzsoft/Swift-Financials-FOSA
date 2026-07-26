using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg
{
    public static class LeaveApplicationFactory
    {
        public static LeaveApplication CreateLeaveApplication(Guid employeeId, Guid? leaveTypeId, Duration duration, string reason, decimal balance, string documentNumber, string fileName, string title, string fileDescription, string mimeType)
        {
            var leaveApplication = new LeaveApplication();

            leaveApplication.GenerateNewIdentity();

            leaveApplication.EmployeeId = employeeId;

            leaveApplication.LeaveTypeId = (leaveTypeId != null && leaveTypeId != Guid.Empty) ? leaveTypeId : null;

            leaveApplication.Duration = duration;

            leaveApplication.Reason = reason;

            leaveApplication.Balance = balance;

            leaveApplication.DocumentNumber = documentNumber;

            leaveApplication.FileName = fileName;

            leaveApplication.FileTitle = title;

            leaveApplication.FileDescription = fileDescription;

            leaveApplication.FileMIMEType = mimeType;

            leaveApplication.CreatedDate = DateTime.Now;

            return leaveApplication;
        }
    }
}
