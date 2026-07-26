using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg
{
    public class EmployeeAppraisalPeriod : Entity
    {
        public string Description { get; set; }

        public virtual Duration Duration { get; set; }

        public bool IsLocked { get; private set; }

        public bool IsActive { get; private set; }

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
    }
}
