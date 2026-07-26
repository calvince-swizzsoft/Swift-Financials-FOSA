using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg
{
    public class Delegate : Domain.Seedwork.Entity
    {
        public Guid ZoneId { get; set; }

        public virtual Zone Zone { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

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
