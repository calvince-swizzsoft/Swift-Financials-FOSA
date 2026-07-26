using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg
{
    public class ElectronicStatementOrderHistory : Entity
    {
        public Guid ElectronicStatementOrderId { get; set; }

        public virtual ElectronicStatementOrder ElectronicStatementOrder { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public virtual Duration Duration { get; set; }

        public virtual Schedule Schedule { get; set; }

        public string Sender { get; set; }

        public string Remarks { get; set; }
    }
}
