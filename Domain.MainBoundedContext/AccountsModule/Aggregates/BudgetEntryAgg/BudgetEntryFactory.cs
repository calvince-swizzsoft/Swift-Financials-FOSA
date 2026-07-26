using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg
{
    public static class BudgetEntryFactory
    {
        public static BudgetEntry CreateBudgetEntry(Guid budgetId, int type, Guid? chartOfAccountId, Guid? loanProductId, decimal amount, string reference)
        {
            var budgetEntry = new BudgetEntry();

            budgetEntry.GenerateNewIdentity();

            budgetEntry.BudgetId = budgetId;

            budgetEntry.Type = (byte)type;

            budgetEntry.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;

            budgetEntry.LoanProductId = (loanProductId != null && loanProductId != Guid.Empty) ? loanProductId : null;

            budgetEntry.Amount = amount;

            budgetEntry.Reference = reference;

            budgetEntry.CreatedDate = DateTime.Now;

            return budgetEntry;
        }
    }
}
