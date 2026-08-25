using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg
{
    public static class PackageTypeSpecifications
    {
        public static Specification<PackageType> DefaultSpec()
        {
            Specification<PackageType> specification = new TrueSpecification<PackageType>();

            return specification;
        }

        public static Specification<PackageType> PackageTypeFullText(string text)
        {
            Specification<PackageType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nameSpec = new DirectSpecification<PackageType>(p => p.Name.Contains(text));

                specification &= (nameSpec);
            }

            return specification;
        }
    }
}
