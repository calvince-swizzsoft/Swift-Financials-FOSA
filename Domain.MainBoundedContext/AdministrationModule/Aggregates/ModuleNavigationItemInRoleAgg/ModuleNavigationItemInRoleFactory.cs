using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemInRoleAgg
{
    public static class ModuleNavigationItemInRoleFactory
    {
        public static ModuleNavigationItemInRole CreateModuleNavigationItemInRole(Guid moduleNavigationItemId, string roleName)
        {
            var moduleNavigationItemInRole = new ModuleNavigationItemInRole();

            moduleNavigationItemInRole.GenerateNewIdentity();

            moduleNavigationItemInRole.ModuleNavigationItemId = moduleNavigationItemId;

            moduleNavigationItemInRole.RoleName = roleName;

            moduleNavigationItemInRole.CreatedDate = DateTime.Now;

            return moduleNavigationItemInRole;
        }
    }
}
