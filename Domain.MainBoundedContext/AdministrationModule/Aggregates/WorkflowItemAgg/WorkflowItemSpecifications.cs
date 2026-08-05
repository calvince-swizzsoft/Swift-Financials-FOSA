using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// callerRoleNames must be the caller's roles resolved server-side (e.g. from a validated auth token),
        /// never a role name supplied directly by the client - only items belonging to one of these roles are returned.
        /// </summary>
        public static Specification<WorkflowItem> WorkflowItemBySystemPermissionAndStatus(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate, List<string> callerRoleNames)
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

            var roleNamesUpper = (callerRoleNames ?? new List<string>()).Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.ToUpper()).ToList();

            var roleSpecification = roleNamesUpper.Any()
                ? new DirectSpecification<WorkflowItem>(x => roleNamesUpper.Contains(x.RoleName.ToUpper()))
                : new DirectSpecification<WorkflowItem>(x => false);

            specification &= roleSpecification;

            return specification;
        }

        /// <summary>
        /// callerRoleNames must be the caller's roles resolved server-side (e.g. from a validated auth token),
        /// never a role name supplied directly by the client. Unlike WorkflowItemBySystemPermissionAndStatus,
        /// this deliberately takes no systemPermissionType - a WorkflowItem only ever exists under a RoleName
        /// that was mapped to its Workflow's SystemPermissionType in the first place (see
        /// WorkflowAppService.AddNewWorkflow), so filtering by RoleName alone already scopes the result to
        /// every permission type the caller's role(s) can act on. Use this for a unified "my approvals" inbox
        /// spanning every permission type; use the systemPermissionType-specific method above for a
        /// single-type/tabbed view.
        /// </summary>
        public static Specification<WorkflowItem> WorkflowItemForCallerRoles(int status, string text, DateTime startDate, DateTime endDate, List<string> callerRoleNames)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<WorkflowItem> specification = DefaultSpec();

            if (status == (int)WorkflowRecordStatus.Pending)
            {
                var lockedItemsSpecification = new DirectSpecification<WorkflowItem>(x => x.Status == status && x.CreatedDate >= startDate && x.CreatedDate <= endDate && !x.IsLocked);

                specification &= lockedItemsSpecification;
            }
            else
            {
                var unlockedItemsSpecification = new DirectSpecification<WorkflowItem>(x => x.Status == status && x.CreatedDate >= startDate && x.CreatedDate <= endDate);

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

            var roleNamesUpper = (callerRoleNames ?? new List<string>()).Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.ToUpper()).ToList();

            var roleSpecification = roleNamesUpper.Any()
                ? new DirectSpecification<WorkflowItem>(x => roleNamesUpper.Contains(x.RoleName.ToUpper()))
                : new DirectSpecification<WorkflowItem>(x => false);

            specification &= roleSpecification;

            return specification;
        }

    }
}
