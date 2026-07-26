using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg
{
    public class MicroCreditOfficer : Domain.Seedwork.Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

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
