using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg
{
    public class SalesOrder : Entity
    {
        public decimal OrderQuantity { get; set; }

        public decimal Amount { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerContact { get; set; }

        public string PaymentTerms { get; set; }

        public string Remarks { get; set; }

        public string InventoryDescription { get; set; }

        public short RecordStatus { get; set; }
    }
}

