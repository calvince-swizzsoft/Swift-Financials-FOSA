using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg
{
    public static class CompanySpecifications
    {
        public static Specification<Company> DefaultSpec()
        {
            Specification<Company> specification = new TrueSpecification<Company>();

            return specification;
        }
        
        public static Specification<Company> CompanyFullText(string text)
        {
            Specification<Company> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Company>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
