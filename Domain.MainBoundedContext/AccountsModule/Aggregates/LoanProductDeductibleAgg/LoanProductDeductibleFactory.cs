using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDeductibleAgg
{
    public static class LoanProductDeductibleFactory
    {
        public static LoanProductDeductible CreateLoanProductDeductible(Guid loanProductId, string description, CustomerAccountType customerAccountType, Charge charge, bool netOffInvestmentBalance, bool computeChargeOnTopUp)
        {
            var loanProductDeductible = new LoanProductDeductible();

            loanProductDeductible.GenerateNewIdentity();

            loanProductDeductible.LoanProductId = loanProductId;

            loanProductDeductible.Description = description;

            loanProductDeductible.CustomerAccountType = customerAccountType;

            loanProductDeductible.Charge = charge;

            loanProductDeductible.NetOffInvestmentBalance = netOffInvestmentBalance;

            loanProductDeductible.ComputeChargeOnTopUp = computeChargeOnTopUp;

            loanProductDeductible.CreatedDate = DateTime.Now;

            return loanProductDeductible;
        }
    }
}
