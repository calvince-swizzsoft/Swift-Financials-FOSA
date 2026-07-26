using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInRoleAgg
{
    public static class SystemPermissionTypeInRoleSpecifications
    {
        public static Specification<SystemPermissionTypeInRole> SystemPermissionType(int systemPermissionType)
        {
            Specification<SystemPermissionTypeInRole> specification =
                new DirectSpecification<SystemPermissionTypeInRole>(m => m.SystemPermissionType == systemPermissionType);

            return specification;
        }

        public static Specification<SystemPermissionTypeInRole> SystemPermissionTypeAndRoleName(int systemPermissionType, string roleName)
        {
            Specification<SystemPermissionTypeInRole> specification =
                new DirectSpecification<SystemPermissionTypeInRole>(m => m.SystemPermissionType == systemPermissionType && m.RoleName.ToUpper() == roleName.ToUpper());

            return specification;
        }

        public static Specification<SystemPermissionTypeInRole> SystemPermissionTypeInRole(string roleName)
        {
            Specification<SystemPermissionTypeInRole> specification = new DirectSpecification<SystemPermissionTypeInRole>(m => m.RoleName == roleName);

            return specification;
        }
    }
}
