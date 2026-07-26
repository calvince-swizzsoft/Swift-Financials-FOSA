using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CreditBatchId { get; set; }

        public Guid? CustomerAccountId { get; set; }

        public Guid? ChartOfAccountId { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal Balance { get; set; }

        public string Beneficiary { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
