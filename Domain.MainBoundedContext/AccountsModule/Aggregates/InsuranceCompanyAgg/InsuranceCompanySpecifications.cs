using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InsuranceCompanyAgg
{
    public static class InsuranceCompanySpecifications
    {
        public static Specification<InsuranceCompany> DefaultSpec()
        {
            Specification<InsuranceCompany> specification = new TrueSpecification<InsuranceCompany>();

            return specification;
        }

        public static Specification<InsuranceCompany> InsuranceFullText(string text)
        {
            Specification<InsuranceCompany> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<InsuranceCompany>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
