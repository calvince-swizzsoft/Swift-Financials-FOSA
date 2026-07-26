using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanPurposeAgg
{
    public static class LoanPurposeSpecifications
    {
        public static Specification<LoanPurpose> DefaultSpec()
        {
            Specification<LoanPurpose> specification = new TrueSpecification<LoanPurpose>();

            return specification;
        }

        public static Specification<LoanPurpose> LoanPurposeFullText(string text)
        {
            Specification<LoanPurpose> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<LoanPurpose>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<LoanPurpose> LoanPurposeDescription(string Description)
        {
            Specification<LoanPurpose> specification = new TrueSpecification<LoanPurpose>();

            specification &= new DirectSpecification<LoanPurpose>(c => c.Description == Description);

            return specification;
        }
    }
}
