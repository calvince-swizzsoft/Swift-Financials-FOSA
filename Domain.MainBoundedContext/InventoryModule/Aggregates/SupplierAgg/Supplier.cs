using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg
{
    public class Supplier : Domain.Seedwork.Entity
    {
        public string Name { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string LandLine { get; set; }

        public string MobileLine { get; set; }

        public string Email { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public bool IsLocked { get; private set; }

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
