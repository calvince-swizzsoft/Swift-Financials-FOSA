using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg
{
    public static class SavingsProductFactory
    {
        public static SavingsProduct CreateSavingsProduct(Guid chartOfAccountId, string description, decimal maximumAllowedWithdrawal, decimal maximumAllowedDeposit, decimal minimumBalance, decimal operatingBalance, decimal withdrawalNoticeAmount, int withdrawalNoticePeriod, int withdrawalInterval, double annualPercentageYield, int priority, bool automateLedgerFeeCalculation, bool throttleOverTheCounterWithdrawals)
        {
            var savingsProduct = new SavingsProduct();

            savingsProduct.GenerateNewIdentity();

            savingsProduct.Description = description;

            savingsProduct.MaximumAllowedWithdrawal = maximumAllowedWithdrawal;

            savingsProduct.MaximumAllowedDeposit = maximumAllowedDeposit;

            savingsProduct.MinimumBalance = minimumBalance;

            savingsProduct.OperatingBalance = operatingBalance;

            savingsProduct.ChartOfAccountId = chartOfAccountId;

            savingsProduct.WithdrawalNoticeAmount = withdrawalNoticeAmount;

            savingsProduct.WithdrawalNoticePeriod = (short)withdrawalNoticePeriod;

            savingsProduct.WithdrawalInterval = (short)withdrawalInterval;

            savingsProduct.AnnualPercentageYield = annualPercentageYield;

            savingsProduct.Priority = (short)priority;

            savingsProduct.CreatedDate = DateTime.Now;

            savingsProduct.AutomateLedgerFeeCalculation = automateLedgerFeeCalculation;

            savingsProduct.ThrottleOverTheCounterWithdrawals = throttleOverTheCounterWithdrawals;

            return savingsProduct;
        }
    }
}
