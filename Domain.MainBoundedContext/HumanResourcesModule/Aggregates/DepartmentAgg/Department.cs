using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg
{
    public class Department : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        public bool IsRegistry { get; private set; }

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

        public void SetAsRegistry()
        {
            if (!IsRegistry)
                this.IsRegistry = true;
        }

        public void ResetAsRegistry()
        {
            if (IsRegistry)
                this.IsRegistry = false;
        }
    }
}
