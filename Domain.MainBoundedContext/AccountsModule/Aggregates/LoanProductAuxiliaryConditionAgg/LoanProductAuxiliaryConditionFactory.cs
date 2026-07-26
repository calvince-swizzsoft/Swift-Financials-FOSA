using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg
{
    public static class LoanProductAuxiliaryConditionFactory
    {
        public static LoanProductAuxiliaryCondition CreateLoanAppraisalProduct(Guid baseLoanProductId, Guid targetLoanProductId, int condition, double maximumEligiblePercentage)
        {
            var loanProductAuxiliaryCondition = new LoanProductAuxiliaryCondition();

            loanProductAuxiliaryCondition.GenerateNewIdentity();

            loanProductAuxiliaryCondition.BaseLoanProductId = baseLoanProductId;

            loanProductAuxiliaryCondition.TargetLoanProductId = targetLoanProductId;

            loanProductAuxiliaryCondition.Condition = (byte)condition;

            loanProductAuxiliaryCondition.MaximumEligiblePercentage = maximumEligiblePercentage;

            loanProductAuxiliaryCondition.CreatedDate = DateTime.Now;

            return loanProductAuxiliaryCondition;
        }
    }
}
