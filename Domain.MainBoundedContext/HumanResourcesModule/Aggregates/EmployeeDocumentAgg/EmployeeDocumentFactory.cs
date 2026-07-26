using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg
{
    public static class EmployeeDocumentFactory
    {
        public static EmployeeDocument CreateEmployeeDocument(Guid employeeId, string fileName, string fileTitle, string fileDescription, string fileMIMEType, byte[] fileBuffer)
        {
            var employeeDocument = new EmployeeDocument();

            employeeDocument.GenerateNewIdentity();

            employeeDocument.EmployeeId = employeeId;

            employeeDocument.FileName = fileName;

            employeeDocument.FileTitle = fileTitle;

            employeeDocument.FileDescription = fileDescription;

            employeeDocument.FileMIMEType = fileMIMEType;

            employeeDocument.FileBuffer = fileBuffer;

            employeeDocument.CreatedDate = DateTime.Now;

            return employeeDocument;
        }
    }
}
