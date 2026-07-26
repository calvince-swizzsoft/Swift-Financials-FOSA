using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CustomerAccountBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid BranchId { get; set; }

        public byte CustomerAccountType_ProductCode { get; set; }

        public Guid CustomerAccountType_TargetProductId { get; set; }

        public short CustomerAccountType_TargetProductCode { get; set; }

        public short ScoredLoanDisbursementProductCode { get; set; }

        public decimal ScoredLoanLimit { get; set; }

        public string ScoredLoanLimitRemarks { get; set; }

        public DateTime? ScoredLoanLimitDate { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
