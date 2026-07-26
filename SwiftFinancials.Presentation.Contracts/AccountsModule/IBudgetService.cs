using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IBudgetService")]
    public interface IBudgetService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBudget(BudgetDTO budgetDTO, AsyncCallback callback, Object state);
        BudgetDTO EndAddBudget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBudget(BudgetDTO budgetDTO, AsyncCallback callback, Object state);
        bool EndUpdateBudget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBudgetEntry(BudgetEntryDTO budgetEntryDTO, AsyncCallback callback, Object state);
        BudgetEntryDTO EndAddBudgetEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveBudgetEntries(List<BudgetEntryDTO> budgetEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveBudgetEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudgets(AsyncCallback callback, Object state);
        List<BudgetDTO> EndFindBudgets(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudget(Guid budgetId, AsyncCallback callback, Object state);
        BudgetDTO EndFindBudget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudgetsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BudgetDTO> EndFindBudgetsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudgetsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<BudgetDTO> EndFindBudgetsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudgetEntriesByBudgetId(Guid budgetId, bool includeBalances, AsyncCallback callback, Object state);
        List<BudgetEntryDTO> EndFindBudgetEntriesByBudgetId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindBudgetEntriesByBudgetIdInPage(Guid budgetId, int pageIndex, int pageSize, bool includeBalances, AsyncCallback callback, Object state);
        PageCollectionInfo<BudgetEntryDTO> EndFindBudgetEntriesByBudgetIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFetchBudgetBalanceByBranchId(Guid branchId, int type, Guid typeIdentifier, AsyncCallback callback, Object state);
        decimal EndFetchBudgetBalanceByBranchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateBudgetEntriesByBudgetId(Guid budgetId, List<BudgetEntryDTO> budgetEntries, AsyncCallback callback, Object state);
        bool EndUpdateBudgetEntriesByBudgetId(IAsyncResult result);
    }
}
