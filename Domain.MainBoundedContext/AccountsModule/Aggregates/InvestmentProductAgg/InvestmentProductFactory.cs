using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg
{
    public static class InvestmentProductFactory
    {
        public static InvestmentProduct CreateInvestmentProduct(Guid? parentInvestmentProductId, Guid chartOfAccountId, Guid? poolChartOfAccountId, string description, decimal minimumBalance, decimal maximumBalance, decimal poolAmount, bool isRefundable, int priority, int maturityPeriod, bool isPooled, bool isSuperSaver, double annualPercentageYield, bool transferBalanceToParentOnMembershipTermination, bool trackArrears, bool throttleScheduledArrearsRecovery)
        {
            var investmentProduct = new InvestmentProduct();

            investmentProduct.GenerateNewIdentity();

            investmentProduct.ParentId = (parentInvestmentProductId != null && parentInvestmentProductId != Guid.Empty) ? parentInvestmentProductId : null;

            investmentProduct.Description = description;

            investmentProduct.MinimumBalance = minimumBalance;

            investmentProduct.MaximumBalance = maximumBalance;

            investmentProduct.PoolAmount = poolAmount;

            investmentProduct.Priority = (short)priority;

            investmentProduct.MaturityPeriod = (short)maturityPeriod;

            investmentProduct.IsPooled = isPooled;

            investmentProduct.IsSuperSaver = isSuperSaver;

            investmentProduct.IsRefundable = (isPooled) ? false : isRefundable;

            investmentProduct.CreatedDate = DateTime.Now;

            investmentProduct.ChartOfAccountId = chartOfAccountId;

            investmentProduct.PoolChartOfAccountId = (isPooled) ? poolChartOfAccountId : null;

            investmentProduct.AnnualPercentageYield = annualPercentageYield;

            investmentProduct.TransferBalanceToParentOnMembershipTermination = (isRefundable) ? false : transferBalanceToParentOnMembershipTermination;

            investmentProduct.TrackArrears = trackArrears;

            investmentProduct.ThrottleScheduledArrearsRecovery = throttleScheduledArrearsRecovery;

            return investmentProduct;
        }
    }
}
