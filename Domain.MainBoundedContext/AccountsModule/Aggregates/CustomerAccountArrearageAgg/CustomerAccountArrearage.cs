using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg
{
    public class CustomerAccountArrearage : Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public byte Category { get; set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }
    }
}
