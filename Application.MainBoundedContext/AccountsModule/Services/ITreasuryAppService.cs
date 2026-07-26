using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ITreasuryAppService
    {
        TreasuryDTO AddNewTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader);

        bool UpdateTreasury(TreasuryDTO treasuryDTO, ServiceHeader serviceHeader);

        List<TreasuryDTO> FindTreasuries(ServiceHeader serviceHeader);

        PageCollectionInfo<TreasuryDTO> FindTreasuries(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<TreasuryDTO> FindTreasuries(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        TreasuryDTO FindTreasury(Guid treasuryId, ServiceHeader serviceHeader);

        TreasuryDTO FindTreasuryByBranchId(Guid branchId, ServiceHeader serviceHeader);

        void FetchTreasuryBalances(List<TreasuryDTO> treasuries, ServiceHeader serviceHeader);
    }
}
