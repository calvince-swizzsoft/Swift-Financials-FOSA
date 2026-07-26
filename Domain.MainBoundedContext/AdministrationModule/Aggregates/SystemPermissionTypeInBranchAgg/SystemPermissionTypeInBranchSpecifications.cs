using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.SystemPermissionTypeInBranchAgg
{
    public static class SystemPermissionTypeInBranchSpecifications
    {
        public static Specification<SystemPermissionTypeInBranch> SystemPermissionType(int systemPermissionType)
        {
            Specification<SystemPermissionTypeInBranch> specification =
                new DirectSpecification<SystemPermissionTypeInBranch>(m => m.SystemPermissionType == systemPermissionType);

            return specification;
        }

        public static Specification<SystemPermissionTypeInBranch> SystemPermissionTypeAndBranchId(int systemPermissionType, Guid branchId)
        {
            Specification<SystemPermissionTypeInBranch> specification =
                new DirectSpecification<SystemPermissionTypeInBranch>(m => m.SystemPermissionType == systemPermissionType && m.BranchId == branchId);

            return specification;
        }

        public static Specification<SystemPermissionTypeInBranch> SystemPermissionTypeInBranch(Guid branchId)
        {
            Specification<SystemPermissionTypeInBranch> specification = new DirectSpecification<SystemPermissionTypeInBranch>(m => m.BranchId == branchId);

            return specification;
        }
    }
}
