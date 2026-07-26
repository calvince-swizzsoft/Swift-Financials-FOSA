using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class SystemPermissionTypeInRoleDTOEqualityComparer : EqualityComparer<SystemPermissionTypeInRoleDTO>
    {
        public override bool Equals(SystemPermissionTypeInRoleDTO x, SystemPermissionTypeInRoleDTO y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            return (x.Id == y.Id || x.SystemPermissionType == y.SystemPermissionType || x.RoleName.ToLower() == y.RoleName.ToLower() || x.RequiredApprovers == y.RequiredApprovers || x.ApprovalPriority == y.ApprovalPriority);
        }

        public override int GetHashCode(SystemPermissionTypeInRoleDTO obj)
        {
            int hashCode = 29;
            hashCode = (hashCode * 31) + obj.Id.GetHashCode();
            hashCode = (hashCode * 31) + obj.SystemPermissionType.GetHashCode();
            hashCode = (hashCode * 31) + obj.RoleName.ToLower().GetHashCode();
            hashCode = (hashCode * 31) + obj.RequiredApprovers.GetHashCode();
            hashCode = (hashCode * 31) + obj.ApprovalPriority.GetHashCode();
            return hashCode;
        }
    }
}
