using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg
{
    public class SalaryCardEntry : Domain.Seedwork.Entity
    {
        public Guid SalaryCardId { get; set; }

        public virtual SalaryCard SalaryCard { get; private set; }

        public Guid SalaryGroupEntryId { get; set; }

        public virtual SalaryGroupEntry SalaryGroupEntry { get; private set; }

        public virtual Charge Charge { get; set; }
    }
}
