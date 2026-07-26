using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeAppraisalTargetAppService
    {
        EmployeeAppraisalTargetDTO AddNewEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader);

        bool UpdateEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, ServiceHeader serviceHeader);

        List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(ServiceHeader serviceHeader);

        List<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true);

        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<EmployeeAppraisalTargetDTO> FindChildEmployeeAppraisalTargets(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeAppraisalTargetDTO> FindEmployeeAppraisalTargets(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeAppraisalTargetDTO FindEmployeeAppraisalTarget(Guid employeeAppraisalTargetId, ServiceHeader serviceHeader);
    }
}
