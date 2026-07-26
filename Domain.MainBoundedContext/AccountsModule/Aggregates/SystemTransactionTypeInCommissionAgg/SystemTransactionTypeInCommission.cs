using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg
{
    public class SystemTransactionTypeInCommission : Entity
    {
        public int SystemTransactionType { get; set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public virtual Charge Complement { get; set; }

        
    }
}
