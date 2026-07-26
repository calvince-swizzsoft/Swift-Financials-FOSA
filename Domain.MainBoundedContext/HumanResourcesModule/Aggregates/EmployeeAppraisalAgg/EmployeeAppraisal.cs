using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalTargetAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAppraisalAgg
{
    public class EmployeeAppraisal : Entity
    {
        public Guid EmployeeAppraisalPeriodId { get; set; }

        public virtual EmployeeAppraisalPeriod EmployeeAppraisalPeriod { get; private set; }

        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid? BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid EmployeeAppraisalTargetId { get; set; }

        public virtual EmployeeAppraisalTarget EmployeeAppraisalTarget { get; private set; }

        public string AppraiseeAnswer { get; set; }

        public string AppraiserAnswer { get; set; }

        public string AppraisedBy { get; set; }

        public DateTime? AppraisedDate { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }
    }
}
