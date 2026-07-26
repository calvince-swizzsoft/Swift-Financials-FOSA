using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ISalaryPeriodAppService
    {
        SalaryProcessingDTO AddNewSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader);

        bool ProcessSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, List<EmployeeDTO> employees, ServiceHeader serviceHeader);

        bool CloseSalaryPeriod(SalaryProcessingDTO salaryPeriodDTO, ServiceHeader serviceHeader);

        bool PostPaySlip(Guid paySlipId, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<SalaryProcessingDTO> FindSalaryPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryProcessingDTO> FindSalaryPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SalaryProcessingDTO FindSalaryPeriod(Guid salaryPeriodId, ServiceHeader serviceHeader);

        SalaryProcessingDTO FindCachedSalaryPeriod(Guid salaryPeriodId, ServiceHeader serviceHeader);
    }
}
