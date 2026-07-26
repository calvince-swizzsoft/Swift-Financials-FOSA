using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg
{
    public static class PurchaseOrderSpecifications
    {
        public static Specification<PurchaseOrder> DefaultSpec()
        {
            Specification<PurchaseOrder> specification = new TrueSpecification<PurchaseOrder>();

            return specification;
        }


        public static Specification<PurchaseOrder> PurchaseOrderFullText(string text)
        {
            Specification<PurchaseOrder> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<PurchaseOrder>(c => c.SupplierName.Contains(text));
                var tagNumberSpec = new DirectSpecification<PurchaseOrder>(c => c.Remarks.Contains(text));

                specification &= (descriptionSpec | tagNumberSpec);
            }

            return specification;
        }

        public static ISpecification<PurchaseOrder> PurchaseOrderWithCode(string code)
        {
            Specification<PurchaseOrder> specification = new DirectSpecification<PurchaseOrder>(x => x.OrderQuantity.Equals(code));

            return specification;
        }
    }
}
