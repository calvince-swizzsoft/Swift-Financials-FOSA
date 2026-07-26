using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanCycleAgg
{
    public class LoanCycle : Domain.Seedwork.Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public virtual Range Range { get; set; }

        
    }
}
