using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg
{
    public class ConditionalLending: Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

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
