using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Seedwork;
using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderEntryAgg
{
    public class PurchaseOrderEntry : Entity
    {
        public Guid PurchaseOrderId { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; private set; }  
        
        public Guid InventoryId { get; set; }

        public virtual Inventory Inventory { get; private set; }

        public decimal Quantity { get; set; }

        public decimal AmountPerUnit { get; set; }
    }
}
