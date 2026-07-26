using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg
{
    public static class BudgetEntrySpecifications
    {
        public static Specification<BudgetEntry> DefaultSpec()
        {
            Specification<BudgetEntry> specification = new TrueSpecification<BudgetEntry>();

            return specification;
        }

        public static ISpecification<BudgetEntry> BudgetEntryWithBudgetId(Guid budgetId)
        {
            Specification<BudgetEntry> specification = new TrueSpecification<BudgetEntry>();

            if (budgetId != null && budgetId != Guid.Empty)
            {
                specification &= new DirectSpecification<BudgetEntry>(x => x.BudgetId == budgetId);
            }

            return specification;
        }

        public static ISpecification<BudgetEntry> BudgetEntryByBudgetIdAndTypeIdentifier(Guid budgetId, int type, Guid typeIdentifier)
        {
            Specification<BudgetEntry> specification = new DirectSpecification<BudgetEntry>(x => x.BudgetId == budgetId);

            switch ((BudgetEntryType)type)
            {
                case BudgetEntryType.IncomeOrExpense:
                    specification &= new DirectSpecification<BudgetEntry>(x => x.BudgetId == budgetId && x.ChartOfAccountId == typeIdentifier);
                    break;
                case BudgetEntryType.LoanProduct:
                    specification &= new DirectSpecification<BudgetEntry>(x => x.BudgetId == budgetId && x.LoanProductId == typeIdentifier);
                    break;
                default:
                    break;
            }

            return specification;
        }
    }
}
