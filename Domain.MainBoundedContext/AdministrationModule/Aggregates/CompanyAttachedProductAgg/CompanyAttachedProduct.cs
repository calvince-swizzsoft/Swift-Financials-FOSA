using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg
{
    public class CompanyAttachedProduct : Entity
    {
        public Guid CompanyId { get; set; }

        public virtual Company Company { get; private set; }

        public byte ProductCode { get; set; }

        public Guid TargetProductId { get; set; }
    }
}
