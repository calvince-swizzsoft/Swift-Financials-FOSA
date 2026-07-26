using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FixedDepositBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid BranchId { get; set; }

        public Guid CustomerAccountId { get; set; }

        public decimal Value { get; set; }

        public int Term { get; set; }

        public double Rate { get; set; }

        public string Remarks { get; set; }

        public int Status { get; set; }

        public DateTime MaturityDate { get; set; }

        public decimal ExpectedInterest { get; set; }

        public decimal TotalExpected { get; set; }

        public string PaidBy { get; set; }

        public DateTime? PaidDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
