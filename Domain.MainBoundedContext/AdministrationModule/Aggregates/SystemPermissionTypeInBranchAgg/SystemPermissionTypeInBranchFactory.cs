using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg
{
    public static class SystemPermissionTypeInBranchFactory
    {
        public static SystemPermissionTypeInBranch CreateSystemPermissionTypeInBranch(int systemPermissionType, Guid branchId)
        {
            var systemPermissionTypeInRole = new SystemPermissionTypeInBranch();

            systemPermissionTypeInRole.GenerateNewIdentity();

            systemPermissionTypeInRole.SystemPermissionType = systemPermissionType;

            systemPermissionTypeInRole.BranchId = branchId;

            systemPermissionTypeInRole.CreatedDate = DateTime.Now;

            return systemPermissionTypeInRole;
        }
    }
}
