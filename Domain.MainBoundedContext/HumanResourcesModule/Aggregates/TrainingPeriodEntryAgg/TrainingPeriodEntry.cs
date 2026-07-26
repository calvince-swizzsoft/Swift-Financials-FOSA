using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg
{
    public class TrainingPeriodEntry : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid TrainingPeriodId { get; set; }

        public virtual TrainingPeriod TrainingPeriod { get; private set; }
    }
}
