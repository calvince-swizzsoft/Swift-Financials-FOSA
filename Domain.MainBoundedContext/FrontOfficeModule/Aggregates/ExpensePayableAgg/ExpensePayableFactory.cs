using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg
{
    public static class ExpensePayableFactory
    {
        public static ExpensePayable CreateExpensePayable(Guid branchId, Guid chartOfAccountId, int type, decimal totalValue, DateTime valueDate, string remarks)
        {
            var expensePayable = new ExpensePayable();

            expensePayable.GenerateNewIdentity();

            expensePayable.BranchId = branchId;

            expensePayable.ChartOfAccountId = chartOfAccountId;

            expensePayable.Type = type;

            expensePayable.TotalValue = totalValue;

            expensePayable.ValueDate = valueDate;

            expensePayable.Remarks = remarks;

            expensePayable.CreatedDate = DateTime.Now;

            return expensePayable;
        }
    }
}
