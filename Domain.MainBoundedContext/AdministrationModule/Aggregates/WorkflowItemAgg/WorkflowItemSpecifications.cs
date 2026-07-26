using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg
{
    public static class WorkflowItemSpecifications
    {
        public static Specification<WorkflowItem> DefaultSpec()
        {
            Specification<WorkflowItem> specification = new TrueSpecification<WorkflowItem>();

            return specification;
        }

        public static Specification<WorkflowItem> WorkflowItem(Guid workFlowId, string roleName)
        {
            Specification<WorkflowItem> specification = new DirectSpecification<WorkflowItem>(x => x.WorkflowId == workFlowId);

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleNameSpec = new DirectSpecification<WorkflowItem>(c => c.RoleName.Contains(roleName));

                specification &= (roleNameSpec);
            }

            return specification;
        }

        public static Specification<WorkflowItem> WorkflowItems(Guid workFlowId)
        {
            Specification<WorkflowItem> specification = new DirectSpecification<WorkflowItem>(x => x.WorkflowId == workFlowId);

            return specification;
        }

        public static Specification<WorkflowItem> WorkflowItemBySystemPermissionAndStatus(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<WorkflowItem> specification = DefaultSpec();

            if (status == (int)WorkflowRecordStatus.Pending)
            {
                var lockedItemsSpecification = new DirectSpecification<WorkflowItem>(x => x.Workflow.SystemPermissionType == systemPermissionType && x.Status == status && x.CreatedDate >= startDate && x.CreatedDate <= endDate && !x.IsLocked);

                specification &= lockedItemsSpecification;
            }
            else
            {
                var unlockedItemsSpecification = new DirectSpecification<WorkflowItem>(x => x.Workflow.SystemPermissionType == systemPermissionType && x.Status == status && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                specification &= unlockedItemsSpecification;
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                int number = default(int);

                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var referenceNumberSpec = new DirectSpecification<WorkflowItem>(x => x.Workflow.ReferenceNumber == number);

                    specification &= referenceNumberSpec;
                }
            }

            return specification;
        }

    }
}
