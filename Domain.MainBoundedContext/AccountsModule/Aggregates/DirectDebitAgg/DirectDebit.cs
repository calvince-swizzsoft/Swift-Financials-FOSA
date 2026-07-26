using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg
{
    public class DirectDebit : Entity
    {
        public string Description { get; set; }

        public virtual CustomerAccountType CustomerAccountType { get; set; }

        public virtual Charge Charge { get; set; }

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
