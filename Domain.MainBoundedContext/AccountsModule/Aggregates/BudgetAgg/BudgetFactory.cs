using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg
{
    public static class BudgetFactory
    {
        public static Budget CreateBudget(Guid postingPeriodId, Guid branchId, string description, decimal totalValue)
        {
            var budget = new Budget();

            budget.GenerateNewIdentity();

            budget.PostingPeriodId = postingPeriodId;

            budget.BranchId = branchId;

            budget.Description = description;

            budget.TotalValue = totalValue;

            budget.CreatedDate = DateTime.Now;

            return budget;
        }
    }
}
