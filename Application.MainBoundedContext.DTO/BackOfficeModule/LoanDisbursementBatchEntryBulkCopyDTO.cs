using System;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanDisbursementBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid LoanDisbursementBatchId { get; set; }

        public Guid LoanCaseId { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
