using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg
{
    public static class DesignationSpecifications
    {
        public static Specification<Designation> DefaultSpec()
        {
            Specification<Designation> specification = new TrueSpecification<Designation>();

            return specification;
        }

        public static Specification<Designation> DesignationFullText(string text)
        {
            Specification<Designation> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Designation>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<Designation> ParentDesignations()
        {
            Specification<Designation> specification = new TrueSpecification<Designation>();

            specification &= new DirectSpecification<Designation>(c => c.ParentId == null);

            return specification;
        }
    }
}
