using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg
{
    public static class InventorySpecifications
    {
        public static Specification<Inventory> DefaultSpec()
        {
            Specification<Inventory> specification = new TrueSpecification<Inventory>();

            return specification;
        }

        public static Specification<Inventory> InventoryFullText(string text)
        {
            Specification<Inventory> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Inventory>(c => c.Description.Contains(text));
                var remarksSpec = new DirectSpecification<Inventory>(c => c.Remarks.Contains(text));

                specification &= (descriptionSpec | remarksSpec);
            }

            return specification;
        }
    }
}
