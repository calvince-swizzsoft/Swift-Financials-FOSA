using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg
{
    public class ElectronicStatementOrder : Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public virtual Duration Duration { get; set; }

        public virtual Schedule Schedule { get; set; }

        public string Remarks { get; set; }

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
