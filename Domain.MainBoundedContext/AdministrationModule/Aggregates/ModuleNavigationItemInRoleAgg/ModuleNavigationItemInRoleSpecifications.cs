using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemInRoleAgg
{
    public static class ModuleNavigationItemInRoleSpecifications
    {
        public static Specification<ModuleNavigationItemInRole> ModuleNavigationItemCode(int itemCode)
        {
            Specification<ModuleNavigationItemInRole> specification = new DirectSpecification<ModuleNavigationItemInRole>(m => m.ModuleNavigationItem.Code == itemCode);

            return specification;
        }

        public static Specification<ModuleNavigationItemInRole> ModuleNavigationRoleName(string roleName)
        {
            Specification<ModuleNavigationItemInRole> specification = new DirectSpecification<ModuleNavigationItemInRole>(m => m.RoleName == roleName);

            return specification;
        }

        public static Specification<ModuleNavigationItemInRole> ModuleNavigationItemAndRoleName(Guid moduleNavigationItemId, string roleName)
        {
            Specification<ModuleNavigationItemInRole> specification =
                new DirectSpecification<ModuleNavigationItemInRole>(m => m.ModuleNavigationItemId == moduleNavigationItemId && m.RoleName.ToUpper() == roleName.ToUpper());

            return specification;
        }
    }
}
