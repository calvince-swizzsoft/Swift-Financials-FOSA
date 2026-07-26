using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg
{
    public class Holiday : Domain.Seedwork.Entity
    {
        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public string Description { get; set; }

        public virtual Duration Duration { get; set; }

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
