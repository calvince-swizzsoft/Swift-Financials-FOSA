using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg
{
    public static class ExpensePayableEntrySpecifications
    {
        public static Specification<ExpensePayableEntry> DefaultSpec()
        {
            Specification<ExpensePayableEntry> specification = new TrueSpecification<ExpensePayableEntry>();

            return specification;
        }

        public static Specification<ExpensePayableEntry> ExpensePayableEntryWithExpensePayableId(Guid expensePayableId)
        {
            Specification<ExpensePayableEntry> specification = DefaultSpec();

            if (expensePayableId != null && expensePayableId != Guid.Empty)
            {
                var expensePayableIdSpec = new DirectSpecification<ExpensePayableEntry>(c => c.ExpensePayableId == expensePayableId);

                specification &= expensePayableIdSpec;
            }

            return specification;
        }

        public static Specification<ExpensePayableEntry> PostedExpensePayableEntryWithExpensePayableId(Guid expensePayableId)
        {
            Specification<ExpensePayableEntry> specification = new DirectSpecification<ExpensePayableEntry>(x => x.ExpensePayableId == expensePayableId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }
    }
}
