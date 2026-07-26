using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg
{
    public class Inventory : Seedwork.Entity
    {
        public string Code { get; set; }
        public byte Type { get; set; }

        public Guid CompanyId { get; set; }

        public virtual Company Company { get; private set; }

        public Guid CategoryId { get; set; }
        public virtual Category Catergory { get; private set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public bool IsLocked { get; private set; }

        public decimal UnitPrice { get; set; }

        public decimal QuantityInstore { get; set; }

        public string UnitOfMeasure { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "varbinary(MAX)")] 
        public byte[] Image { get; set; }
        public DateTime ExpiryDate { get; set; }
        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
