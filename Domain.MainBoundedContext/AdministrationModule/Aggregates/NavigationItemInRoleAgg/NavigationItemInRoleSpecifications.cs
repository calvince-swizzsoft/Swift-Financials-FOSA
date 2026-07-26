using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemInRoleAgg
{
    public static class NavigationItemInRoleSpecifications
    {
        public static Specification<NavigationItemInRole> DefaultSpecification()
        {
            Specification<NavigationItemInRole> specification = new TrueSpecification<NavigationItemInRole>();

            return specification;
        }

        public static Specification<NavigationItemInRole> ModuleNavigationRoleName(string roleName)
        {
            Specification<NavigationItemInRole> specification = new DirectSpecification<NavigationItemInRole>(m => m.RoleName == roleName);

            return specification;
        }

        public static Specification<NavigationItemInRole> NavigationItemAndRoleName(Guid navigationItemId, string roleName)
        {
            Specification<NavigationItemInRole> specification =
                new DirectSpecification<NavigationItemInRole>(m => m.NavigationItemId == navigationItemId && m.RoleName.ToUpper() == roleName.ToUpper());

            return specification;
        }

        public static Specification<NavigationItemInRole> NavigationItemCode(int navigationItemCode)
        {
            Specification<NavigationItemInRole> specification = new DirectSpecification<NavigationItemInRole>(m => m.NavigationItem.Code == navigationItemCode);

            return specification;
        }

        public static Specification<NavigationItemInRole> NavigationItemCodeAndRole(int navigationItemCode, string roleName)
        {
            Specification<NavigationItemInRole> specification = new DirectSpecification<NavigationItemInRole>(m => m.NavigationItem.Code == navigationItemCode && m.RoleName.ToUpper() == roleName.ToUpper());

            return specification;
        }
        public static Specification<NavigationItemInRole> ControllerNameAndRole(string controllerName, string roleName)
        {
            Specification<NavigationItemInRole> specification = new DirectSpecification<NavigationItemInRole>(m => m.RoleName.ToUpper() == roleName.ToUpper() && m.NavigationItem.ControllerName == controllerName);

            return specification;
        }
    }
}