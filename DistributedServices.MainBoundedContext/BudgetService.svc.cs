using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetAppService _budgetAppService;

        public BudgetService(
            IBudgetAppService budgetAppService)
        {
            Guard.ArgumentNotNull(budgetAppService, nameof(budgetAppService));

            _budgetAppService = budgetAppService;
        }

        #region Budget

        public BudgetDTO AddBudget(BudgetDTO budgetDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.AddNewBudget(budgetDTO, serviceHeader);
        }

        public bool UpdateBudget(BudgetDTO budgetDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.UpdateBudget(budgetDTO, serviceHeader);
        }

        public BudgetEntryDTO AddBudgetEntry(BudgetEntryDTO budgetEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.AddNewBudgetEntry(budgetEntryDTO, serviceHeader);
        }

        public bool RemoveBudgetEntries(List<BudgetEntryDTO> budgetEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.RemoveBudgetEntries(budgetEntryDTOs, serviceHeader);
        }

        public async Task<bool> UpdateBudgetEntriesByBudgetIdAsync(Guid budgetId, List<BudgetEntryDTO> budgetEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _budgetAppService.UpdateBudgetEntriesAsync(budgetId, budgetEntries, serviceHeader);
        }

        public List<BudgetDTO> FindBudgets()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.FindBudgets(serviceHeader);
        }

        public BudgetDTO FindBudget(Guid budgetId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.FindBudget(budgetId, serviceHeader);
        }

        public PageCollectionInfo<BudgetDTO> FindBudgetsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.FindBudgets(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BudgetDTO> FindBudgetsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.FindBudgets(text, pageIndex, pageSize, serviceHeader);
        }

        public List<BudgetEntryDTO> FindBudgetEntriesByBudgetId(Guid budgetId, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var budgetEntries = _budgetAppService.FindBudgetEntries(budgetId, serviceHeader);

            if (includeBalances)
                _budgetAppService.FetchBudgetEntryBalances(budgetEntries, serviceHeader);

            return budgetEntries;
        }

        public decimal FetchBudgetBalanceByBranchId(Guid branchId, int type, Guid typeIdentifier)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _budgetAppService.FetchBudgetBalance(branchId, type, typeIdentifier, serviceHeader);
        }

        public PageCollectionInfo<BudgetEntryDTO> FindBudgetEntriesByBudgetIdInPage(Guid budgetId, int pageIndex, int pageSize, bool includeBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var budgetEntries = _budgetAppService.FindBudgetEntries(budgetId, pageIndex, pageSize, serviceHeader);

            if (includeBalances && budgetEntries != null)
                _budgetAppService.FetchBudgetEntryBalances(budgetEntries.PageCollection, serviceHeader);

            return budgetEntries;
        }

        #endregion
    }
}
