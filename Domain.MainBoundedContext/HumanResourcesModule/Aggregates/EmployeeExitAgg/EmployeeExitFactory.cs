using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg
{
    public static class EmployeeExitFactory
    {
        public static EmployeeExit CreateEmployeeExit(Guid employeeId, Guid branchId, int type, int status, string reason, string fileName, string title, string description, string mimeType)
        {
            var employeeExit = new EmployeeExit();

            employeeExit.GenerateNewIdentity();

            employeeExit.EmployeeId = employeeId;

            employeeExit.BranchId = branchId;

            employeeExit.Type = (byte)type;

            employeeExit.Status = (byte)status;

            employeeExit.Reason = reason;

            employeeExit.FileName = fileName;

            employeeExit.FileTitle = title;

            employeeExit.FileDescription = description;

            employeeExit.FileMIMEType = mimeType;

            employeeExit.CreatedDate = DateTime.Now;

            return employeeExit;
        }
    }
}
