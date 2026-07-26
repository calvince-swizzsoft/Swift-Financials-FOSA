using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositPayableAgg
{
    public class FixedDepositPayable : Domain.Seedwork.Entity
    {
        public Guid FixedDepositId { get; set; }

        public virtual FixedDeposit FixedDeposit { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }
    }
}
