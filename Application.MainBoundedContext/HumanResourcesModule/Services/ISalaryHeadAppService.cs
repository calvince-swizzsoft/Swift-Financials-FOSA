using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ISalaryHeadAppService
    {
        SalaryHeadDTO AddNewSalaryHead(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader);

        bool UpdateSalaryHead(SalaryHeadDTO salaryHeadDTO, ServiceHeader serviceHeader);

        List<SalaryHeadDTO> FindSalaryHeads(ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryHeadDTO> FindSalaryHeads(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<SalaryHeadDTO> FindSalaryHeads(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        SalaryHeadDTO FindSalaryHead(Guid salaryHeadId, ServiceHeader serviceHeader);
    }
}
