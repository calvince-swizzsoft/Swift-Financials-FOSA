using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg
{
    public class AlternateChannelTypeCommission : Entity
    {
        public int AlternateChannelType { get; set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public int KnownChargeType { get; set; }

        public byte ChargeBenefactor { get; set; }
    }
}
