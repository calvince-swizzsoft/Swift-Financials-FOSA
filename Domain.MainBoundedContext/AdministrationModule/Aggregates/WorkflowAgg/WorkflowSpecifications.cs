using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg
{
    public static class WorkflowSpecifications
    {
        public static Specification<Workflow> DefaultSpec()
        {
            Specification<Workflow> specification = new TrueSpecification<Workflow>();

            return specification;
        }

        public static Specification<Workflow> WorkflowByRecordAndType(Guid recordId, int systemPermissionType)
        {
            Specification<Workflow> specification = new DirectSpecification<Workflow>(x => x.RecordId == recordId && x.SystemPermissionType == systemPermissionType);

            return specification;
        }

        public static Specification<Workflow> QueableWorkflows()
        {
            var approved = (int)WorkflowRecordStatus.Approved;

            var rejected = (int)WorkflowRecordStatus.Rejected;

            Specification<Workflow> specification = new DirectSpecification<Workflow>(c => (c.Status == approved || c.Status == rejected) && c.MatchedStatus == (int)WorkflowMatchedStatus.NotMatched);

            return specification;
        }
    }
}
