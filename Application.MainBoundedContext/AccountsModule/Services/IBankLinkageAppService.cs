using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IBankLinkageAppService
    {
        BankLinkageDTO AddNewBankLinkage(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader);

        bool UpdateBankLinkage(BankLinkageDTO bankLinkageDTO, ServiceHeader serviceHeader);

        List<BankLinkageDTO> FindBankLinkages(ServiceHeader serviceHeader);

        PageCollectionInfo<BankLinkageDTO> FindBankLinkages(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BankLinkageDTO> FindBankLinkages(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BankLinkageDTO FindBankLinkage(Guid bankLinkageId, ServiceHeader serviceHeader);

        BankLinkageDTO FindBankLinkageByBankAccountId(Guid bankAccountId, ServiceHeader serviceHeader);
    }
}
