using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg
{
    public class WithdrawalSettlement : Domain.Seedwork.Entity
    {
        public Guid WithdrawalNotificationId { get; set; }

        public virtual WithdrawalNotification WithdrawalNotification { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal CarryForwards { get; set; }

        public string Reference { get; set; }
    }
}
