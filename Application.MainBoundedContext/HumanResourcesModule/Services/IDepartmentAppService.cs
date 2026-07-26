using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IDepartmentAppService
    {
        DepartmentDTO AddNewDepartment(DepartmentDTO departmentDTO, ServiceHeader serviceHeader);

        bool UpdateDepartment(DepartmentDTO departmentDTO, ServiceHeader serviceHeader);

        List<DepartmentDTO> FindDepartments(ServiceHeader serviceHeader);

        PageCollectionInfo<DepartmentDTO> FindDepartments(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<DepartmentDTO> FindDepartments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        DepartmentDTO FindDepartment(Guid departmentId, ServiceHeader serviceHeader);

        DepartmentDTO FindRegistryDepartment(ServiceHeader serviceHeader);
    }
}
