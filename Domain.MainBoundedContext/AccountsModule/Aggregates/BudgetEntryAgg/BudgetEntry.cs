using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg
{
    public class BudgetEntry : Domain.Seedwork.Entity
    {
        public Guid BudgetId { get; set; }

        public virtual Budget Budget { get; private set; }

        public byte Type { get; set; }
        
        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid? LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }
    }
}
