using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IBankAppService
    {
        BankDTO AddNewBank(BankDTO bankDTO, ServiceHeader serviceHeader);

        bool UpdateBank(BankDTO bankDTO, ServiceHeader serviceHeader);

        List<BankDTO> FindBanks(ServiceHeader serviceHeader);

        PageCollectionInfo<BankDTO> FindBanks(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BankDTO> FindBanks(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BankDTO FindBank(Guid bankId, ServiceHeader serviceHeader);

        List<BankBranchDTO> FindBankBranches(Guid bankId, ServiceHeader serviceHeader);

        bool UpdateBankBranches(Guid bankId, List<BankBranchDTO> bankBranches, ServiceHeader serviceHeader);

        bool BulkImport(List<string> bankCodes, List<string> bankNames, List<string> branchCodes, List<string> branchNames, List<int> branchIndexes, ServiceHeader serviceHeader);
    }
}
