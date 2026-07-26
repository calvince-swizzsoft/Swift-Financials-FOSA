using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg
{
    public class InvestmentProduct : Entity
    {
        public Guid? ParentId { get; set; }

        public virtual InvestmentProduct Parent { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid? PoolChartOfAccountId { get; set; }

        public virtual ChartOfAccount PoolChartOfAccount { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal MaximumBalance { get; set; }

        public decimal PoolAmount { get; set; }

        public short Priority { get; set; }

        public short MaturityPeriod { get; set; }

        public double AnnualPercentageYield { get; set; }

        public bool IsPooled { get; set; }

        public bool IsRefundable { get; set; }

        public bool IsSuperSaver { get; set; }

        public bool TransferBalanceToParentOnMembershipTermination { get; set; }

        public bool TrackArrears { get; set; }

        public bool ThrottleScheduledArrearsRecovery { get; set; }

        public bool IsLocked { get; private set; }

        public bool IsMandatory { get; private set; }

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
