using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg
{
    public class DebitTypeCommission : Domain.Seedwork.Entity
    {
        public Guid DebitTypeId { get; set; }

        public virtual DebitType DebitType { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        
    }
}
