using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public interface IFiscalCountAppService
    {
        FiscalCountDTO AddNewFiscalCount(FiscalCountDTO fiscalCountDTO, ServiceHeader serviceHeader);

        bool AddNewFiscalCounts(List<FiscalCountDTO> fiscalCountDTOs, ServiceHeader serviceHeader);

        List<FiscalCountDTO> FindFiscalCounts(ServiceHeader serviceHeader);

        PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<FiscalCountDTO> FindFiscalCounts(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        FiscalCountDTO FindFiscalCount(Guid fiscalCountId, ServiceHeader serviceHeader);
        
        bool IsEndOfDayExecuted(EmployeeDTO employeeDTO, ServiceHeader serviceHeader);

        Task<bool> IsEndOfDayExecutedAsync(EmployeeDTO employeeDTO, ServiceHeader serviceHeader);
    }
}
