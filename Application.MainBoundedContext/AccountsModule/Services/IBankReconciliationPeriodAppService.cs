using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IBankReconciliationPeriodAppService
    {
        BankReconciliationPeriodDTO AddNewBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, ServiceHeader serviceHeader);

        bool CloseBankReconciliationPeriod(BankReconciliationPeriodDTO bankReconciliationPeriodDTO, int bankReconciliationPeriodAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        BankReconciliationEntryDTO AddNewBankReconciliationEntry(BankReconciliationEntryDTO bankReconciliationEntryDTO, ServiceHeader serviceHeader);

        bool RemoveBankReconciliationEntries(List<BankReconciliationEntryDTO> bankReconciliationEntryDTOs, ServiceHeader serviceHeader);

        List<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BankReconciliationPeriodDTO> FindBankReconciliationPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BankReconciliationPeriodDTO FindBankReconciliationPeriod(Guid bankReconciliationPeriodId, ServiceHeader serviceHeader);

        PageCollectionInfo<BankReconciliationEntryDTO> FindBankReconciliationEntriesByBankReconciliationPeriodId(Guid bankReconciliationPeriodId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
