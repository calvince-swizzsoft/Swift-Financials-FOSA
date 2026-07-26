using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ISalaryGroupAppService
    {
        SalaryGroupDTO AddNewSalaryGroup(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader);

        bool UpdateSalaryGroup(SalaryGroupDTO salaryGroupDTO, ServiceHeader serviceHeader);

        List<SalaryGroupDTO> FindSalaryGroups(ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryGroupDTO> FindSalaryGroups(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryGroupDTO> FindSalaryGroups(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SalaryGroupDTO FindSalaryGroup(Guid salaryGroupId, ServiceHeader serviceHeader);

        SalaryGroupEntryDTO FindSalaryGroupEntry(Guid salaryGroupEntryId, ServiceHeader serviceHeader);

        List<SalaryGroupEntryDTO> FindSalaryGroupEntriesBySalaryGroupId(Guid salaryGroupId, ServiceHeader serviceHeader);

        bool UpdateSalaryGroupEntries(Guid salaryGroupId, List<SalaryGroupEntryDTO> salaryGroupEntries, ServiceHeader serviceHeader);
    }
}
