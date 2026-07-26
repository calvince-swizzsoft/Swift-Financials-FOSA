using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg
{
    public class PostingPeriod : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        public virtual Duration Duration { get; set; }

        public bool IsLocked { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsClosed { get; private set; }

        public string ClosedBy { get; set; }

        public DateTime? ClosedDate { get; set; }

        

        

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

        public void Activate()
        {
            if (!IsActive)
                this.IsActive = true;
        }

        public void DeActivate()
        {
            if (IsActive)
                this.IsActive = false;
        }

        public void Close()
        {
            if (!IsClosed)
                this.IsClosed = true;
        }
    }
}
