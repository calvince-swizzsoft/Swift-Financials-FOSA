using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderEntryAgg
{
    public static class PurchaseOrderEntrySpecifications
    {
        public static Specification<PurchaseOrderEntry> DefaultSpec()
        {
            Specification<PurchaseOrderEntry> specification = new TrueSpecification<PurchaseOrderEntry>();
            return specification;
        }

        public static Specification<PurchaseOrderEntry> PurchaseOrderEntryWithPurchaseOrderId(Guid purchaseOrderId, string text)
        {
            Specification<PurchaseOrderEntry> specification = DefaultSpec();

            if (purchaseOrderId != null && purchaseOrderId != Guid.Empty)
            {
                var purchaseOrderIdSpec = new DirectSpecification<PurchaseOrderEntry>(po => po.PurchaseOrderId == purchaseOrderId);

                specification &= purchaseOrderIdSpec;
            }

            return specification;
        }
        

        public static Specification<PurchaseOrderEntry> PurchaseOrderEntryWithInventoryId(Guid inventoryId)
        {
            Specification<PurchaseOrderEntry> specification = DefaultSpec();
            if (inventoryId != Guid.Empty)
            {
                var InventorySpec = new DirectSpecification<PurchaseOrderEntry>(po => po.InventoryId == inventoryId);
                specification &= InventorySpec;
            }
            return specification;
        }



    }
}
