using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg
{
    public static class WorkflowFactory
    {
        public static Workflow CreateWorkflow(Guid recordId, int referenceNumber, Guid branchId, int systemPermissionType, int requiredApprovals, string payload = null)
        {
            var workflow = new Workflow();

            workflow.GenerateNewIdentity();

            workflow.RecordId = recordId;

            workflow.ReferenceNumber = referenceNumber;

            workflow.BranchId = branchId;

            workflow.SystemPermissionType = systemPermissionType;

            workflow.RequiredApprovals = requiredApprovals;
            workflow.Payload = payload;

            workflow.CreatedDate = DateTime.Now;

            return workflow;
        }
    }
}
