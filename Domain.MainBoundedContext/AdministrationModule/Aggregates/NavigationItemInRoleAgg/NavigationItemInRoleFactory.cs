using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemInRoleAgg
{
    public static class NavigationItemInRoleFactory
    {
        public static NavigationItemInRole CreateNavigationItemInRole(Guid navigationItemId, string roleName)
        {
            var NavigationItemInRole = new NavigationItemInRole();

            NavigationItemInRole.GenerateNewIdentity();

            NavigationItemInRole.NavigationItemId = navigationItemId;

            NavigationItemInRole.RoleName = roleName;

            NavigationItemInRole.CreatedDate = DateTime.Now;

            return NavigationItemInRole;
        }
    }
}