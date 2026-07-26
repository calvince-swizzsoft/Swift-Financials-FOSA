using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeTypeAppService
    {
        EmployeeTypeDTO AddNewEmployeeType(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader);

        bool UpdateEmployeeType(EmployeeTypeDTO employeeTypeDTO, ServiceHeader serviceHeader);

        List<EmployeeTypeDTO> FindEmployeeTypes(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeTypeDTO FindEmployeeType(Guid employeeTypeId, ServiceHeader serviceHeader);
    }
}
