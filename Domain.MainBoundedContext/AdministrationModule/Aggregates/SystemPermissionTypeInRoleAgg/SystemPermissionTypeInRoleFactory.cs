using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInRoleAgg
{
    public static class SystemPermissionTypeInRoleFactory
    {
        public static SystemPermissionTypeInRole CreateSystemPermissionTypeInRole(int systemPermissionType, string roleName, int requiredApprovers, int approvalPriority)
        {
            var systemPermissionTypeInRole = new SystemPermissionTypeInRole();

            systemPermissionTypeInRole.GenerateNewIdentity();

            systemPermissionTypeInRole.SystemPermissionType = systemPermissionType;

            systemPermissionTypeInRole.RoleName = roleName;

            systemPermissionTypeInRole.RequiredApprovers = requiredApprovers;

            systemPermissionTypeInRole.ApprovalPriority = approvalPriority;

            systemPermissionTypeInRole.CreatedDate = DateTime.Now;

            return systemPermissionTypeInRole;
        }
    }
}
