using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IPostingPeriodAppService
    {
        PostingPeriodDTO AddNewPostingPeriod(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader);

        bool UpdatePostingPeriod(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader);

        bool ClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        List<PostingPeriodDTO> FindPostingPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<PostingPeriodDTO> FindPostingPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<PostingPeriodDTO> FindPostingPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PostingPeriodDTO FindPostingPeriod(Guid postingPeriodId, ServiceHeader serviceHeader);

        PostingPeriodDTO FindCurrentPostingPeriod(ServiceHeader serviceHeader);

        PostingPeriodDTO FindCachedCurrentPostingPeriod(ServiceHeader serviceHeader);
    }
}
