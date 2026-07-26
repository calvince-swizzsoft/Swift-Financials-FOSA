using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SavingsProductBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public decimal MaximumAllowedWithdrawal { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal OperatingBalance { get; set; }

        public decimal WithdrawalNoticeAmount { get; set; }

        public short WithdrawalNoticePeriod { get; set; }

        public short WithdrawalInterval { get; set; }

        public double AnnualPercentageYield { get; set; }

        public short Priority { get; set; }

        public bool IsLocked { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
