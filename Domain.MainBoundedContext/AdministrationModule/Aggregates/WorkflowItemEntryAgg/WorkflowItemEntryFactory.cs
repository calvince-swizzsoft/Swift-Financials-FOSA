using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemEntryAgg
{
    public static class WorkflowItemEntryFactory
    {
        public static WorkflowItemEntry CreateWorkflowItemEntry(Guid workflowItemId, string remarks, string decision, bool usedBiometrics)
        {
            var workflowItemEntry = new WorkflowItemEntry();

            workflowItemEntry.GenerateNewIdentity();

            workflowItemEntry.WorkflowItemId = workflowItemId;

            workflowItemEntry.Remarks = remarks;

            workflowItemEntry.Decision = decision;

            workflowItemEntry.UsedBiometrics = usedBiometrics;

            workflowItemEntry.CreatedDate = DateTime.Now;

            return workflowItemEntry;
        }
    }
}
