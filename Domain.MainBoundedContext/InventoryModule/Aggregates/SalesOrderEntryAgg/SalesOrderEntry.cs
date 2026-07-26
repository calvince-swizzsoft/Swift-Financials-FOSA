using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SalesOrderEntryAgg
{
    public class SalesOrderEntry : Domain.Seedwork.Entity
    {
        public Guid SalesOrderId { get; set; }

        public virtual SalesOrder SalesOrder { get; private set; }

        public Guid InventoryId { get; set; }

        public virtual Inventory Inventory { get; private set; }

        public decimal OrderQuantity { get; set; }

        public decimal AmountPerUnit { get; set; }
    }
}
