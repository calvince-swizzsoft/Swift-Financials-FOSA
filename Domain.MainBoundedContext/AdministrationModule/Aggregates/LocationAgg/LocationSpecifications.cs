using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.LocationAgg
{
    public static class LocationSpecifications
    {
        public static Specification<Location> DefaultSpec()
        {
            Specification<Location> specification = new TrueSpecification<Location>();

            return specification;
        }

        public static Specification<Location> LocationFullText(string text)
        {
            Specification<Location> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Location>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    } 
}
