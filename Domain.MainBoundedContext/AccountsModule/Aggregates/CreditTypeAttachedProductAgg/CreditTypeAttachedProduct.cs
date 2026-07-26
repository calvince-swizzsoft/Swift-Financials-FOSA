using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAttachedProductAgg
{
    public class CreditTypeAttachedProduct : Domain.Seedwork.Entity
    {
        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        public byte ProductCode { get; set; }

        public Guid TargetProductId { get; set; }
    }
}
