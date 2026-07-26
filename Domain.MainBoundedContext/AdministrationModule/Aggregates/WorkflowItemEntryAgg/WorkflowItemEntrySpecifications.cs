using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemEntryAgg
{
    public static class WorkflowItemEntrySpecifications
    {
        public static Specification<WorkflowItemEntry> DefaultSpec()
        {
            Specification<WorkflowItemEntry> specification = new TrueSpecification<WorkflowItemEntry>();

            return specification;
        }

        public static Specification<WorkflowItemEntry> WorkflowItemEntryByWorkflowItem(Guid workFlowItemId)
        {
            Specification<WorkflowItemEntry> specification = new DirectSpecification<WorkflowItemEntry>(x => x.WorkflowItemId == workFlowItemId);

            return specification;
        }

        public static Specification<WorkflowItemEntry> WorkflowItemEntryByWorkflow(Guid workFlowId)
        {
            Specification<WorkflowItemEntry> specification = new DirectSpecification<WorkflowItemEntry>(x => x.WorkflowItem.WorkflowId == workFlowId);

            return specification;
        }
    }
}
