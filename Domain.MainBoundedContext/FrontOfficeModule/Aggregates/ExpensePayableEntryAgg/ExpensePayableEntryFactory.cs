using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg
{
    public static class ExpensePayableEntryFactory
    {
        public static ExpensePayableEntry CreateExpensePayableEntry(Guid expensePayableId, Guid branchId, Guid chartOfAccountId, decimal value, string primaryDescription, string secondaryDescription, string reference)
        {
            var expensePayableEntry = new ExpensePayableEntry();

            expensePayableEntry.GenerateNewIdentity();

            expensePayableEntry.ExpensePayableId = expensePayableId;

            expensePayableEntry.BranchId = branchId;

            expensePayableEntry.ChartOfAccountId = chartOfAccountId;

            expensePayableEntry.Value = value;

            expensePayableEntry.PrimaryDescription = primaryDescription;

            expensePayableEntry.SecondaryDescription = secondaryDescription;

            expensePayableEntry.Reference = reference;

            expensePayableEntry.CreatedDate = DateTime.Now;

            return expensePayableEntry;
        }
    }
}
