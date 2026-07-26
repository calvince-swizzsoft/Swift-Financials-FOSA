using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class DebitBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid DebitBatchId { get; set; }

        public Guid CustomerAccountId { get; set; }
        
        public double Multiplier { get; set; }

        public decimal BasisValue { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
