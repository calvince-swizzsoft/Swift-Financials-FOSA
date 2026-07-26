using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg
{
    public class Branch : Domain.Seedwork.Entity
    {
        public Guid? CompanyId { get; set; }

        public virtual Company Company { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public virtual Address Address { get; set; }

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
