using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg
{
    public static class InvestmentProductExemptionFactory
    {
        public static InvestmentProductExemption CreateInvestmentProductExemption(Guid investmentProductId, int customerClassification, decimal maximumBalance, double appraisalMultiplier)
        {
            var investmentProductExemption = new InvestmentProductExemption();

            investmentProductExemption.GenerateNewIdentity();

            investmentProductExemption.InvestmentProductId = investmentProductId;

            investmentProductExemption.CustomerClassification = (byte)customerClassification;

            investmentProductExemption.MaximumBalance = maximumBalance;

            investmentProductExemption.AppraisalMultiplier = appraisalMultiplier;
            
            investmentProductExemption.CreatedDate = DateTime.Now;

            return investmentProductExemption;
        }
    }
}
