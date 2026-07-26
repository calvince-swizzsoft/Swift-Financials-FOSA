using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;


namespace Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg
{
    public class Category : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

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
