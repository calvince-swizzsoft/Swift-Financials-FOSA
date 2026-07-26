using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeDirectDebitAgg
{
    public class CreditTypeDirectDebit : Entity
    {
        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        public Guid DirectDebitId { get; set; }

        public virtual DirectDebit DirectDebit { get; private set; }
    }
}
