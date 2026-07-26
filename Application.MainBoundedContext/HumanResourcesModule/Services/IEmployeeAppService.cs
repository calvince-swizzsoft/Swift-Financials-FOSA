using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeAppService
    {
        EmployeeDTO AddNewEmployee(EmployeeDTO employeeDTO, ServiceHeader serviceHeader);

        bool UpdateEmployee(EmployeeDTO employeeDTO, ServiceHeader serviceHeader);

        bool CustomerIsEmployee(Guid customerId, ServiceHeader serviceHeader);

        List<EmployeeDTO> FindEmployees(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDTO> FindEmployees(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDTO> FindEmployees(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeDTO> FindEmployees(Guid departmentId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeDTO FindEmployee(Guid employeeId, ServiceHeader serviceHeader);

        List<EmployeeDTO> FindEmployees(SalaryProcessingDTO salaryPeriodDTO, List<SalaryGroupDTO> salaryGroups, List<BranchDTO> branches, List<DepartmentDTO> departments, ServiceHeader serviceHeader);

        EmployeeDTO FindEmployee(int serialNumber, ServiceHeader serviceHeader);
    }
}
