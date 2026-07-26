using System;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class PaySlipBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid SalaryPeriodId { get; set; }

        public Guid SalaryCardId { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
