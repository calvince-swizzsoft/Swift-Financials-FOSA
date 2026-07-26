using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ILevyAppService
    {
        LevyDTO AddNewLevy(LevyDTO levyDTO, ServiceHeader serviceHeader);

        bool UpdateLevy(LevyDTO levyDTO, ServiceHeader serviceHeader);

        List<LevyDTO> FindLevies(ServiceHeader serviceHeader);

        PageCollectionInfo<LevyDTO> FindLevies(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LevyDTO> FindLevies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LevyDTO FindLevy(Guid levyId, ServiceHeader serviceHeader);

        List<LevySplitDTO> FindLevySplits(Guid levyId, ServiceHeader serviceHeader);

        bool UpdateLevySplits(Guid levyId, List<LevySplitDTO> levySplits, ServiceHeader serviceHeader);
    }
}
