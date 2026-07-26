using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanGuarantorBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid? LoaneeCustomerId { get; set; }

        public Guid? LoanProductId { get; set; }

        public Guid? LoanCaseId { get; set; }

        public int Status { get; set; }

        public decimal TotalShares { get; set; }

        public decimal CommittedShares { get; set; }

        public decimal AmountGuaranteed { get; set; }

        public decimal AmountPledged { get; set; }

        public int AppraisalFactor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
