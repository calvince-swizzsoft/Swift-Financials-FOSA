using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IBranchAppService
    {
        BranchDTO AddNewBranch(BranchDTO branchDTO, ServiceHeader serviceHeader);

        bool UpdateBranch(BranchDTO branchDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateBranchAsync(BranchDTO branchDTO, ServiceHeader serviceHeader);

        List<BranchDTO> FindBranches( ServiceHeader serviceHeader);

        PageCollectionInfo<BranchDTO> FindBranches(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BranchDTO> FindBranches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BranchDTO FindBranch(Guid branchId, ServiceHeader serviceHeader);

        BranchDTO FindBranch(int branchCode, ServiceHeader serviceHeader);

        BranchDTO FindCachedBranch(Guid branchId, ServiceHeader serviceHeader);
    }
}
