using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg
{
    public static class AlternateChannelReconciliationEntrySpecifications
    {
        public static Specification<AlternateChannelReconciliationEntry> DefaultSpec()
        {
            Specification<AlternateChannelReconciliationEntry> specification = new TrueSpecification<AlternateChannelReconciliationEntry>();

            return specification;
        }

        public static Specification<AlternateChannelReconciliationEntry> AlternateChannelReconciliationEntryFullText(Guid alternateChannelReconciliationPeriodId, int status, string text)
        {
            Specification<AlternateChannelReconciliationEntry> specification = new DirectSpecification<AlternateChannelReconciliationEntry>(x => x.AlternateChannelReconciliationPeriodId == alternateChannelReconciliationPeriodId && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var primaryAccountNumberSpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.PrimaryAccountNumber.Contains(text));

                var systemTraceAuditNumberSpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.SystemTraceAuditNumber.Contains(text));

                var retrievalReferenceNumberSpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.RetrievalReferenceNumber.Contains(text));

                var referenceSpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.Reference.Contains(text));

                var remarksSpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.Remarks.Contains(text));

                var createdBySpec = new DirectSpecification<AlternateChannelReconciliationEntry>(c => c.CreatedBy.Contains(text));

                specification &= (primaryAccountNumberSpec | systemTraceAuditNumberSpec | retrievalReferenceNumberSpec | referenceSpec | remarksSpec | createdBySpec);
            }

            return specification;
        }
    }
}
