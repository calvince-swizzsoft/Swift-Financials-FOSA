using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDeductibleAgg
{
    public static class LoanProductDeductibleSpecifications
    {
        public static Specification<LoanProductDeductible> DefaultSpec()
        {
            Specification<LoanProductDeductible> specification = new TrueSpecification<LoanProductDeductible>();

            return specification;
        }

        public static ISpecification<LoanProductDeductible> LoanProductDeductibleWithLoanProductId(Guid loanProductId)
        {
            Specification<LoanProductDeductible> specification = new TrueSpecification<LoanProductDeductible>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductDeductible>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }

        public static Specification<LoanProductDeductible> LoanProductDeductibleFullText(string text)
        {
            Specification<LoanProductDeductible> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<LoanProductDeductible>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
