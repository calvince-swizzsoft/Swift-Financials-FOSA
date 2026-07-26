using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg
{
    public class WorkflowItem : Entity
    {
        public Guid WorkflowId { get; set; }

        public virtual Workflow Workflow { get; private set; }

        public byte Status { get; set; }

        public int RequiredApprovals { get; set; }

        public int CurrentApprovals { get; set; }

        public string RoleName { get; set; }

        public int ApprovalPriority { get; set; }

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
