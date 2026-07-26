using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg
{
    public class RecurringBatch : Entity
    {
        public Guid? BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public int BatchNumber { get; set; }

        public byte Type { get; set; }

        public byte Month { get; set; }

        public string Reference { get; set; }

        public byte Priority { get; set; }

        public bool EnforceMonthValueDate { get; set; }

        public byte Status { get; set; }
    }
}
