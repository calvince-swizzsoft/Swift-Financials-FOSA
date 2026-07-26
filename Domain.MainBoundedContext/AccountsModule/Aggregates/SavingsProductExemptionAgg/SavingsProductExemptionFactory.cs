using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductExemptionAgg
{
    public static class SavingsProductExemptionFactory
    {
        public static SavingsProductExemption CreateSavingsProductExemption(Guid savingsProductId, Guid branchId, decimal maximumAllowedWithdrawal, decimal maximumAllowedDeposit, decimal minimumBalance, decimal operatingBalance, decimal withdrawalNoticeAmount, int withdrawalNoticePeriod, int withdrawalInterval, double annualPercentageYield)
        {
            var savingsProductExemption = new SavingsProductExemption();

            savingsProductExemption.GenerateNewIdentity();

            savingsProductExemption.SavingsProductId = savingsProductId;

            savingsProductExemption.BranchId = branchId;

            savingsProductExemption.MaximumAllowedWithdrawal = maximumAllowedWithdrawal;

            savingsProductExemption.MaximumAllowedDeposit = maximumAllowedDeposit;

            savingsProductExemption.MinimumBalance = minimumBalance;

            savingsProductExemption.OperatingBalance = operatingBalance;

            savingsProductExemption.WithdrawalNoticeAmount = withdrawalNoticeAmount;

            savingsProductExemption.WithdrawalNoticePeriod = (short)withdrawalNoticePeriod;

            savingsProductExemption.WithdrawalInterval = (short)withdrawalInterval;

            savingsProductExemption.AnnualPercentageYield = annualPercentageYield;

            savingsProductExemption.CreatedDate = DateTime.Now;

            return savingsProductExemption;
        }
    }
}
