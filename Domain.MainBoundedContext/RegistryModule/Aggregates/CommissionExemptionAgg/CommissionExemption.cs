using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg
{
    public class CommissionExemption : Entity
    {
        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

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
