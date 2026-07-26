using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg
{
    public class SavingsProductCommission : Entity
    {
        public Guid SavingsProductId { get; set; }

        public virtual SavingsProduct SavingsProduct { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public int KnownChargeType { get; set; }
        
        public byte ChargeBenefactor { get; set; }
    }
}
