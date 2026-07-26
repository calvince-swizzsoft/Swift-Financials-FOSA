using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyDebitTypeAgg
{
    public class CompanyDebitType : Entity
    {
        public Guid CompanyId { get; set; }

        public virtual Company Company { get; private set; }

        public Guid DebitTypeId { get; set; }

        public virtual DebitType DebitType { get; private set; }
    }
}
