using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg
{
    public class EmployeeDisciplinaryCaseFactory
    {
        public static EmployeeDisciplinaryCase CreateEmployeeDisciplinaryCase(Guid employeeId, DateTime incidentDate, int type,  string fileName, string title, string description, string mimeType, string remarks)
        {
            var employeeDisciplinaryCase = new EmployeeDisciplinaryCase();

            employeeDisciplinaryCase.GenerateNewIdentity();

            employeeDisciplinaryCase.EmployeeId = employeeId;            

            employeeDisciplinaryCase.IncidentDate = incidentDate;

            employeeDisciplinaryCase.Type = (byte)type;

            employeeDisciplinaryCase.FileName = fileName;

            employeeDisciplinaryCase.FileTitle = title;

            employeeDisciplinaryCase.FileDescription = description;

            employeeDisciplinaryCase.FileMIMEType = mimeType;

            employeeDisciplinaryCase.Remarks = remarks;

            employeeDisciplinaryCase.CreatedDate = DateTime.Now;

            return employeeDisciplinaryCase;
        }
    }
}
