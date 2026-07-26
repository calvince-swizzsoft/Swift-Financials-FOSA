using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeDisciplinaryCaseAppService
    {
        EmployeeDisciplinaryCaseDTO AddNewEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader);

        bool UpdateEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, ServiceHeader serviceHeader);

        List<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeDisciplinaryCaseDTO FindEmployeeDisciplinaryCase(Guid employeeDisciplinaryCaseId, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByEmployeeId(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
