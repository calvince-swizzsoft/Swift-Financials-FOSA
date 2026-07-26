using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg
{
    public static class PurchaseOrderFactory
    {
        public static PurchaseOrder CreatePurchaseOrder(string inventoryDescription, decimal orderQuantity, decimal amount, string supplierName, string supplierContact, string paymentTerms, string remarks, int recordStatus)
        {
            var purchaseOrder = new PurchaseOrder();

            purchaseOrder.GenerateNewIdentity();

            purchaseOrder.InventoryDescription = inventoryDescription;

            purchaseOrder.OrderQuantity = orderQuantity;

            purchaseOrder.SupplierName = supplierName;

            purchaseOrder.SupplierContact = supplierContact;

            purchaseOrder.Amount = amount;

            purchaseOrder.PaymentTerms = paymentTerms;

            purchaseOrder.Remarks = remarks;

            purchaseOrder.RecordStatus = (short)recordStatus;

            purchaseOrder.CreatedDate = DateTime.Now;

            return purchaseOrder; 
        }

    }
}
