using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertCommissionAgg
{
    public class TextAlertCommission : Entity
    {
        public int SystemTransactionCode { get; set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public byte ChargeBenefactor { get; set; }
    }
}
