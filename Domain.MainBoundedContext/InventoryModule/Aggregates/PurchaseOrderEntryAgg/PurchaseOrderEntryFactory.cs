using System;


namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderEntryAgg
{
    public static class PurchaseOrderEntryFactory
    {
        public static PurchaseOrderEntry CreatePurchaseOrderEntry(Guid purchaseOrderId, Guid inventoryId, decimal quantity,decimal amountPerUnit)
        {
            var purchaseOrderEntry = new PurchaseOrderEntry();

            purchaseOrderEntry.GenerateNewIdentity();

            purchaseOrderEntry.PurchaseOrderId = purchaseOrderId;

            purchaseOrderEntry.InventoryId = inventoryId;

            purchaseOrderEntry.AmountPerUnit = amountPerUnit;

            purchaseOrderEntry.Quantity = quantity;

            purchaseOrderEntry.CreatedDate = DateTime.Now;

            return purchaseOrderEntry;
        }
    }

}
