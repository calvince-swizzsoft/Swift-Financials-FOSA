using System;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class PaySlipEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid PaySlipId { get; set; }

        public Guid CustomerAccountId { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public string Description { get; set; }

        public int SalaryHeadType { get; set; }

        public int SalaryHeadCategory { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public int RoundingType { get; set; }

        public byte SalaryCardEntryCharge_Type { get; set; }

        public double SalaryCardEntryCharge_Percentage { get; set; }

        public decimal SalaryCardEntryCharge_FixedAmount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
