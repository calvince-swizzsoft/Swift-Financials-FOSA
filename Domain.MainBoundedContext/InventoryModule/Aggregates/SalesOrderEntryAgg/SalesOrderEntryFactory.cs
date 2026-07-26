using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SalesOrderEntryAgg
{
    public class SalesOrderEntryFactory
    {
        public static SalesOrderEntry CreateSalesOrderEntry(Guid salesOrderId, Guid inventoryId, decimal quantity, decimal amountPerUnit)
        {
            var salesOrderEntry = new SalesOrderEntry();

            salesOrderEntry.GenerateNewIdentity();

            salesOrderEntry.SalesOrderId = salesOrderId;

            salesOrderEntry.InventoryId = inventoryId;

            salesOrderEntry.OrderQuantity = quantity;

            salesOrderEntry.AmountPerUnit = amountPerUnit;

            salesOrderEntry.CreatedDate = DateTime.Now;

            return salesOrderEntry;
        }
    }
}
