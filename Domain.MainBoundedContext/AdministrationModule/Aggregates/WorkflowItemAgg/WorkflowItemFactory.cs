using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg
{
    public static class WorkflowItemFactory
    {
        public static WorkflowItem CreateWorkflowItem(Guid workflowId, int requiredApprovals, int currentApprovals, string roleName, int approvalPriority, int status)
        {
            var workflowItem = new WorkflowItem();

            workflowItem.GenerateNewIdentity();

            workflowItem.WorkflowId = workflowId;

            workflowItem.RequiredApprovals = requiredApprovals;

            workflowItem.CurrentApprovals = currentApprovals;

            workflowItem.RoleName = roleName;

            workflowItem.ApprovalPriority = approvalPriority;

            workflowItem.Status = (byte)status;

            workflowItem.CreatedDate = DateTime.Now;

            return workflowItem;
        }
    }
}
