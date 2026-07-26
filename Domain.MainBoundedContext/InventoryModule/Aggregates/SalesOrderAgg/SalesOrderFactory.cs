using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg
{
    public static class SalesOrderFactory
    {
        public static SalesOrder CreateReceipt(decimal qrderQuantity, string customerName, string customerEmail, string customerContact, string paymentTerms, string remarks , string inventoryDescriptionint, int recordStatus)
        {
            var salesOrder = new SalesOrder();

            salesOrder.GenerateNewIdentity();

            salesOrder.OrderQuantity = qrderQuantity;
             
            salesOrder.CustomerName = customerName;

            salesOrder.CustomerEmail = customerEmail;

            salesOrder.CustomerContact = customerContact;

            salesOrder.PaymentTerms = paymentTerms;

            salesOrder.Remarks = remarks;

            salesOrder.InventoryDescription = inventoryDescriptionint;

            salesOrder.RecordStatus = (short)recordStatus;

            salesOrder.CreatedDate = DateTime.Now;

            return salesOrder;
        }
    }
}
