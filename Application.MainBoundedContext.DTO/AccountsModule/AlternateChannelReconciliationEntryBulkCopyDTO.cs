using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelReconciliationEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid AlternateChannelReconciliationPeriodId { get; set; }

        public string PrimaryAccountNumber { get; set; }

        public string SystemTraceAuditNumber { get; set; }

        public string RetrievalReferenceNumber { get; set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
