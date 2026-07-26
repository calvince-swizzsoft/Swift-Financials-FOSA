using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid JournalId { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public Guid ContraChartOfAccountId { get; set; }

        public Guid? CustomerAccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ValueDate { get; set; }

        public string IntegrityHash { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
