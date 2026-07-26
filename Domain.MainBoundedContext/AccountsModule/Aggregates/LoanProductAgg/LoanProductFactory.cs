using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg
{
    public static class LoanProductFactory
    {
        public static LoanProduct CreateLoanProduct(Guid chartOfAccountId, Guid interestReceivedChartOfAccountId, Guid interestReceivableChartOfAccountId, Guid? interestChargedChartOfAccountId, string description, LoanInterest loanInterest, LoanRegistration loanRegistration, Charge takeHome, int priority)
        {
            var loanProduct = new LoanProduct();

            loanProduct.GenerateNewIdentity();

            loanProduct.Description = description;

            loanProduct.LoanInterest = loanInterest;

            loanProduct.LoanRegistration = loanRegistration;

            loanProduct.TakeHome = takeHome;

            loanProduct.Priority = (short)priority;

            loanProduct.CreatedDate = DateTime.Now;

            loanProduct.ChartOfAccountId = chartOfAccountId;

            loanProduct.InterestReceivedChartOfAccountId = interestReceivedChartOfAccountId;

            loanProduct.InterestReceivableChartOfAccountId = interestReceivableChartOfAccountId;

            loanProduct.InterestChargedChartOfAccountId = (interestChargedChartOfAccountId != null && interestChargedChartOfAccountId != Guid.Empty) ? interestChargedChartOfAccountId : null; 

            return loanProduct;
        }
    }
}
