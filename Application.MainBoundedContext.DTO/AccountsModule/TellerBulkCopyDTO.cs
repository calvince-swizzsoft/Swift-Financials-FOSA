using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class TellerBulkCopyDTO
    {
        public Guid Id { get; set; }

        public byte Type { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? ChartOfAccountId { get; set; }

        public Guid? ShortageChartOfAccountId { get; set; }

        public Guid? ExcessChartOfAccountId { get; set; }

        public Guid? FloatCustomerAccountId { get; set; }

        public Guid? CommissionCustomerAccountId { get; set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public decimal Range_LowerLimit { get; set; }

        public decimal Range_UpperLimit { get; set; }

        public int MiniStatementItemsCap { get; set; }

        public string Reference { get; set; }

        public bool IsLocked { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
