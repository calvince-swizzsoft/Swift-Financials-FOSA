using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IBudgetService
    {
        #region Budget

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BudgetDTO AddBudget(BudgetDTO budgetDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateBudget(BudgetDTO budgetDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BudgetEntryDTO AddBudgetEntry(BudgetEntryDTO budgetEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveBudgetEntries(List<BudgetEntryDTO> budgetEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateBudgetEntriesByBudgetIdAsync(Guid budgetId, List<BudgetEntryDTO> budgetEntries);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BudgetDTO> FindBudgets();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        BudgetDTO FindBudget(Guid budgetId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BudgetDTO> FindBudgetsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BudgetDTO> FindBudgetsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<BudgetEntryDTO> FindBudgetEntriesByBudgetId(Guid budgetId, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<BudgetEntryDTO> FindBudgetEntriesByBudgetIdInPage(Guid budgetId, int pageIndex, int pageSize, bool includeBalances);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        decimal FetchBudgetBalanceByBranchId(Guid branchId, int type, Guid typeIdentifier);

        #endregion
    }
}
