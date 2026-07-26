using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeDocumentAppService
    {
        EmployeeDocumentDTO AddNewEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        bool UpdateEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader);

        List<EmployeeDocumentDTO> FindEmployeeDocuments(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocuments(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocuments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeDocumentDTO FindEmployeeDocument(Guid employeeDocumentId, ServiceHeader serviceHeader);
    }
}
