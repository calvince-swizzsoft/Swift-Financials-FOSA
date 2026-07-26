using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg
{
    public static class ConditionalLendingSpecifications
    {
        public static Specification<ConditionalLending> DefaultSpec()
        {
            Specification<ConditionalLending> specification = new TrueSpecification<ConditionalLending>();

            return specification;
        }

        public static Specification<ConditionalLending> ConditionalLendingFullText(string text)
        {
            Specification<ConditionalLending> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<ConditionalLending>(c => c.Description.Contains(text));
                var loanProductDescriptionSpec = new DirectSpecification<ConditionalLending>(c => c.LoanProduct.Description.Contains(text));

                specification &= (descriptionSpec | loanProductDescriptionSpec);
            }

            return specification;
        }

        public static ISpecification<ConditionalLending> ConditionalLendingWithLoanProductId(Guid loanProductId)
        {
            Specification<ConditionalLending> specification = new TrueSpecification<ConditionalLending>();

            if (loanProductId != null && loanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<ConditionalLending>(x => x.LoanProductId == loanProductId);
            }

            return specification;
        }
    }
}
