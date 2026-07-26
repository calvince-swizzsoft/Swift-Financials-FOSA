using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg
{
    public static class LoanProductAuxilliaryAppraisalFactorSpecifications
    {
        public static Specification<LoanProductAuxilliaryAppraisalFactor> DefaultSpec()
        {
            Specification<LoanProductAuxilliaryAppraisalFactor> specification = new TrueSpecification<LoanProductAuxilliaryAppraisalFactor>();

            return specification;
        }

        public static ISpecification<LoanProductAuxilliaryAppraisalFactor> LoanProductAuxilliaryAppraisalFactorWithLoanProductId(Guid loanProductId)
        {
            Specification<LoanProductAuxilliaryAppraisalFactor> specification = new TrueSpecification<LoanProductAuxilliaryAppraisalFactor>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductAuxilliaryAppraisalFactor>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }
    }
}
