using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg
{
    public static class AlternateChannelReconciliationEntryFactory
    {
        public static AlternateChannelReconciliationEntry CreateAlternateChannelReconciliationEntry(Guid alternateChannelReconciliationPeriodId, string primaryAccountNumber, string systemTraceAuditNumber, string retrievalReferenceNumber, decimal amount, string reference, string remarks)
        {
            var alternateChannelReconciliationEntry = new AlternateChannelReconciliationEntry();

            alternateChannelReconciliationEntry.GenerateNewIdentity();

            alternateChannelReconciliationEntry.AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId;

            alternateChannelReconciliationEntry.PrimaryAccountNumber = primaryAccountNumber;

            alternateChannelReconciliationEntry.SystemTraceAuditNumber = systemTraceAuditNumber;

            alternateChannelReconciliationEntry.RetrievalReferenceNumber = retrievalReferenceNumber;

            alternateChannelReconciliationEntry.Amount = amount;

            alternateChannelReconciliationEntry.Reference = reference;

            alternateChannelReconciliationEntry.Remarks = remarks;

            alternateChannelReconciliationEntry.CreatedDate = DateTime.Now;

            return alternateChannelReconciliationEntry;
        }
    }
}
