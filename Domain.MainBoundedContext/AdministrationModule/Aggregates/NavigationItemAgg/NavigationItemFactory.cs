using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg
{
    public static class NavigationItemFactory
    {
        public static NavigationItem CreateNavigationItem(Guid? parentId, string description, string icon, int code, string controllerName, string actionName, int parentCode, string areaName)
        {
            var navigationItem = new NavigationItem();

            navigationItem.GenerateNewIdentity();

            navigationItem.ParentId = (parentId != null && parentId != Guid.Empty) ? parentId : null;

            navigationItem.Description = description;

            navigationItem.Icon = icon;

            navigationItem.Code = code;

            navigationItem.ControllerName = controllerName;

            navigationItem.ActionName = actionName;

            navigationItem.AreaCode = parentCode;

            navigationItem.AreaName = areaName;

            navigationItem.CreatedDate = DateTime.Now;

            return navigationItem;
        }
    }
}