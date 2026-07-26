using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequePayableAgg
{
    public class ExternalChequePayable : Entity
    {
        public Guid ExternalChequeId { get; set; }

        public virtual ExternalCheque ExternalCheque { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }
    }
}
