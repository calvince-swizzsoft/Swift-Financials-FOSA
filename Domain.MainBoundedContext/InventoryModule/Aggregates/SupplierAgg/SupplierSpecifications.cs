using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg
{
    public static class SupplierSpecifications
    {
        public static Specification<Supplier> DefaultSpec()
        {
            Specification<Supplier> specification = new TrueSpecification<Supplier>();

            return specification;
        }

        public static Specification<Supplier> SupplierFullText(string text)
        {
            Specification<Supplier> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nameSpec = new DirectSpecification<Supplier>(s => s.Name.Contains(text));

                specification &= (nameSpec);
            }

            return specification;
        }
    }
}
