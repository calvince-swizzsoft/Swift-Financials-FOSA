using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg
{
    public class LevySplit : Domain.Seedwork.Entity
    {
        public Guid LevyId { get; set; }

        public virtual Levy Levy { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string Description { get; set; }

        public double Percentage { get; set; }
    }
}
