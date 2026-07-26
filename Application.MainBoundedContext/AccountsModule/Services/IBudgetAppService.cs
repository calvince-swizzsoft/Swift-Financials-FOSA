using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IBudgetAppService
    {
        BudgetDTO AddNewBudget(BudgetDTO budgetDTO, ServiceHeader serviceHeader);

        bool UpdateBudget(BudgetDTO budgetDTO, ServiceHeader serviceHeader);

        BudgetEntryDTO AddNewBudgetEntry(BudgetEntryDTO budgetEntryDTO, ServiceHeader serviceHeader);

        bool RemoveBudgetEntries(List<BudgetEntryDTO> budgetEntryDTOs, ServiceHeader serviceHeader);

        List<BudgetDTO> FindBudgets(ServiceHeader serviceHeader);

        PageCollectionInfo<BudgetDTO> FindBudgets(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<BudgetDTO> FindBudgets(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        BudgetDTO FindBudget(Guid budgetId, ServiceHeader serviceHeader);

        BudgetDTO FindBudget(Guid postingPeriodId, Guid branchId, ServiceHeader serviceHeader);

        List<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, ServiceHeader serviceHeader);

        List<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, int type, Guid chartOfAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        void FetchBudgetEntryBalances(List<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader);

        decimal FetchBudgetBalance(Guid branchId, int type, Guid typeIdentifier, ServiceHeader serviceHeader);

        Task<bool> UpdateBudgetEntriesAsync(Guid budgetId, List<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader);
    }
}
