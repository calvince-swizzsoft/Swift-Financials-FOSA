using Domain.Seedwork;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg
{
    public class PurchaseOrder : Entity
    {
        public string InventoryDescription { get; set; }
        public decimal OrderQuantity { get; set; }

        public string SupplierName { get; set; } 
        public string SupplierContact { get; set; }

        public string PaymentTerms { get; set; }

        public decimal Amount { get; set; }

        public string Remarks { get; set; }

        public short RecordStatus { get; set; }
    }
}
