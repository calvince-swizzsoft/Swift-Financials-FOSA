using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg
{
    public class ChequeType : Entity
    {
        public string Description { get; set; }

        public int MaturityPeriod { get; set; }

        public int ChargeRecoveryMode { get; set; }

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
