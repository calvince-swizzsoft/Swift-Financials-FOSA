using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg
{
    public class AlternateChannelReconciliationEntry : Entity
    {
        public Guid AlternateChannelReconciliationPeriodId { get; set; }

        public virtual AlternateChannelReconciliationPeriod AlternateChannelReconciliationPeriod { get; private set; }

        public string PrimaryAccountNumber { get; set; }

        public string SystemTraceAuditNumber { get; set; }

        public string RetrievalReferenceNumber { get; set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }
    }
}
