using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AdministrativeDivisionAgg
{
    public static class AdministrativeDivisionSpecifications
    {
        public static Specification<AdministrativeDivision> DefaultSpec()
        {
            Specification<AdministrativeDivision> specification = new TrueSpecification<AdministrativeDivision>();

            return specification;
        }

        public static Specification<AdministrativeDivision> AdministrativeDivisionFullText(string text)
        {
            Specification<AdministrativeDivision> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<AdministrativeDivision>(c => c.Description.Contains(text));
                var remarksSpec = new DirectSpecification<AdministrativeDivision>(c => c.Remarks.Contains(text));

                specification &= (descriptionSpec | remarksSpec);
            }

            return specification;
        }

        public static ISpecification<AdministrativeDivision> ParentAdministrativeDivisions()
        {
            Specification<AdministrativeDivision> specification = new TrueSpecification<AdministrativeDivision>();

            specification &= new DirectSpecification<AdministrativeDivision>(c => c.ParentId == null);

            return specification;
        }
    }
}
