using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeAppraisalAppService
    {
        bool AddNewEmployeeAppraisal(List<EmployeeAppraisalDTO> employeeAppraisalDTOs, ServiceHeader serviceHeader);

        bool UpdateEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader);

        bool AppraiseEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, ServiceHeader serviceHeader);

        List<EmployeeAppraisalDTO> FindEmployeeAppraisals(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeAppraisalDTO> FindEmployeeAppraisals(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
