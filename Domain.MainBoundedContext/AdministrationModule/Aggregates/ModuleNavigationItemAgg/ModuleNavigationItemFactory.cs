using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg
{
    public static class ModuleNavigationItemFactory
    {
        public static ModuleNavigationItem CreateModuleNavigationItem(Guid? parentId, string description, string icon, int code, string controllerName, string actionName, int parentCode, string areaName)
        {
            var moduleNavigationItem = new ModuleNavigationItem();

            moduleNavigationItem.GenerateNewIdentity();

            moduleNavigationItem.ParentId = (parentId != null && parentId != Guid.Empty) ? parentId : null;

            moduleNavigationItem.Description = description;

            moduleNavigationItem.Icon = icon;

            moduleNavigationItem.Code = code;

            moduleNavigationItem.ControllerName = controllerName;

            moduleNavigationItem.ActionName = actionName;

            moduleNavigationItem.AreaCode = parentCode;

            moduleNavigationItem.AreaName = areaName;

            moduleNavigationItem.CreatedDate = DateTime.Now;

            return moduleNavigationItem;
        }
    }
}
