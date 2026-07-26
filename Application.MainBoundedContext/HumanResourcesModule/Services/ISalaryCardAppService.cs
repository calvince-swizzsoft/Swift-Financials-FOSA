using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ISalaryCardAppService
    {
        SalaryCardDTO AddNewSalaryCard(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader);

        bool UpdateSalaryCard(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader);

        bool ResetSalaryCardEntries(SalaryCardDTO salaryCardDTO, ServiceHeader serviceHeader);

        bool UpdateSalaryCardEntry(SalaryCardEntryDTO salaryCardEntryDTO, ServiceHeader serviceHeader);

        bool ZeroizeOneOffEarnings(Guid salaryCardId, ServiceHeader serviceHeader);

        List<SalaryCardDTO> FindSalaryCards(ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryCardDTO> FindSalaryCards(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryCardDTO> FindSalaryCards(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SalaryCardDTO FindSalaryCard(Guid salaryCardId, ServiceHeader serviceHeader);

        SalaryCardDTO FindSalaryCardByEmployeeId(Guid employeeId, ServiceHeader serviceHeader);

        List<SalaryCardEntryDTO> FindSalaryCardEntriesBySalaryCardId(Guid salaryCardId, ServiceHeader serviceHeader);
    }
}
