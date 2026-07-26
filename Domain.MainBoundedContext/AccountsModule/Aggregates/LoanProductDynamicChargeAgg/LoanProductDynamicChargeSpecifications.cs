using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDynamicChargeAgg
{
    public static class LoanProductDynamicChargeSpecifications
    {
        public static Specification<LoanProductDynamicCharge> DefaultSpec()
        {
            Specification<LoanProductDynamicCharge> specification = new TrueSpecification<LoanProductDynamicCharge>();

            return specification;
        }

        public static ISpecification<LoanProductDynamicCharge> LoanProductDynamicChargeWithLoanProductId(Guid loanProductId)
        {
            Specification<LoanProductDynamicCharge> specification = new TrueSpecification<LoanProductDynamicCharge>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductDynamicCharge>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }

        public static ISpecification<LoanProductDynamicCharge> LoanProductDynamicChargeWithLoanProductIdAndRecoveryMode(Guid loanProductId, int recoveryMode)
        {
            Specification<LoanProductDynamicCharge> specification = new TrueSpecification<LoanProductDynamicCharge>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductDynamicCharge>(x => x.LoanProductId == loanProductId && x.DynamicCharge.RecoveryMode == recoveryMode);
            }

            return specification;
        }
    }
}
