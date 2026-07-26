using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GraduatedScaleAgg
{
    public class GraduatedScale : Domain.Seedwork.Entity
    {
        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public virtual Range Range { get; set; }

        public virtual Charge Charge { get; set; }

        
    }
}
