using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg
{
    public class Budget : Domain.Seedwork.Entity
    {
        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; set; }

        public Guid? BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public string Description { get; set; }

        public decimal TotalValue { get; set; }

        

        HashSet<BudgetEntry> _budgetEntries;
        public virtual ICollection<BudgetEntry> BudgetEntries
        {
            get
            {
                if (_budgetEntries == null)
                {
                    _budgetEntries = new HashSet<BudgetEntry>();
                }
                return _budgetEntries;
            }
            private set
            {
                _budgetEntries = new HashSet<BudgetEntry>(value);
            }
        }
    }
}
