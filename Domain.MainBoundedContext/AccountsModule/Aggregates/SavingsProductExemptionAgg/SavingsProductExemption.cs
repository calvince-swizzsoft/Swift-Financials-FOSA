using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductExemptionAgg
{
    public class SavingsProductExemption : Entity
    {
        public Guid SavingsProductId { get; set; }

        public virtual SavingsProduct SavingsProduct { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public decimal MaximumAllowedWithdrawal { get; set; }

        public decimal MaximumAllowedDeposit { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal OperatingBalance { get; set; }

        public decimal WithdrawalNoticeAmount { get; set; }

        public short WithdrawalNoticePeriod { get; set; }

        public short WithdrawalInterval { get; set; }

        public double AnnualPercentageYield { get; set; }

        
    }
}
