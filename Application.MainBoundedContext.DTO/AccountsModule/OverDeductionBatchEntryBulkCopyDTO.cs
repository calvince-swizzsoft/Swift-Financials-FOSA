using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class OverDeductionBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid OverDeductionBatchId { get; set; }

        public Guid? CreditCustomerAccountId { get; set; }

        public Guid? DebitCustomerAccountId { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
