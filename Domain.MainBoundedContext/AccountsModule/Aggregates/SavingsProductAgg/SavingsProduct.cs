using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg
{
    public class SavingsProduct : Entity
    {
        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public decimal MaximumAllowedWithdrawal { get; set; }

        public decimal MaximumAllowedDeposit { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal OperatingBalance { get; set; }

        public decimal WithdrawalNoticeAmount { get; set; }

        public short WithdrawalNoticePeriod { get; set; }

        public short WithdrawalInterval { get; set; }

        public double AnnualPercentageYield { get; set; }

        public short Priority { get; set; }

        public bool IsLocked { get; private set; }

        public bool IsDefault { get; private set; }

        public bool IsMandatory { get; private set; }

        public bool AutomateLedgerFeeCalculation { get; set; }

        public bool ThrottleOverTheCounterWithdrawals { get; set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }

        public void SetAsDefault()
        {
            if (!IsDefault)
                this.IsDefault = true;
        }

        public void ResetAsDefault()
        {
            if (IsDefault)
                this.IsDefault = false;
        }

        public void SetAsMandatory()
        {
            if (!IsMandatory)
                this.IsMandatory = true;
        }

        public void ResetAsMandatory()
        {
            if (IsMandatory)
                this.IsMandatory = false;
        }
    }
}
