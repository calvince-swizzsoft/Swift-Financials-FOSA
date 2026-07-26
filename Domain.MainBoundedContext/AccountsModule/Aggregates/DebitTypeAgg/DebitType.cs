using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg
{
    public class DebitType : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        public virtual CustomerAccountType CustomerAccountType { get; set; }

        public bool IsLocked { get; private set; }

        public bool IsMandatory { get; private set; }



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
        public void SetAsMandatory()
        {
            if (!IsMandatory)
                this.IsMandatory = true;
        }

        public void ResetAsMandatory()
        {
            if (IsMandatory)
                this.IsMandatory = false;
        }
    }
}
