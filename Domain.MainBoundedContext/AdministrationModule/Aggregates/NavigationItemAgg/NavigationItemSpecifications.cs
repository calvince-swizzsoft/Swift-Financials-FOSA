using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.NavigationItemAgg
{
    public static class NavigationItemSpecifications
    {
        public static Specification<NavigationItem> DefaultSpecification()
        {
            Specification<NavigationItem> specification = new TrueSpecification<NavigationItem>();

            return specification;
        }

        public static Specification<NavigationItem> NavigationItemByControllerAndActionName(string controllerName, string actionName)
        {
            Specification<NavigationItem> specification = new TrueSpecification<NavigationItem>();

            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrWhiteSpace(actionName)) return specification;
            Specification<NavigationItem> controllerNameSpec =
                new DirectSpecification<NavigationItem>(c => c.ControllerName.Equals(controllerName) && c.ActionName.Equals(actionName));

            specification &= controllerNameSpec;

            return specification;
        }

        public static Specification<NavigationItem> NavigationItemByControllerName(string controllerName)
        {
            Specification<NavigationItem> specification = new TrueSpecification<NavigationItem>();

            if (string.IsNullOrEmpty(controllerName)) return specification;
            Specification<NavigationItem> controllerNameSpec =
                new DirectSpecification<NavigationItem>(c => c.ControllerName.Equals(controllerName));

            specification &= controllerNameSpec;

            return specification;
        }

        public static Specification<NavigationItem> NavigationItemByDescription(string description)
        {
            Specification<NavigationItem> specification = new TrueSpecification<NavigationItem>();

            if (string.IsNullOrEmpty(description)) return specification;

            Specification<NavigationItem> descSpec = new DirectSpecification<NavigationItem>(c => c.Description.Equals(description));

            specification &= descSpec;

            return specification;
        }

        public static Specification<NavigationItem> NavigationItemByCode(int code)
        {
            Specification<NavigationItem> specification = new TrueSpecification<NavigationItem>();

            Specification<NavigationItem> codeSpec =
                new DirectSpecification<NavigationItem>(c => c.Code.Equals(code));

            specification &= codeSpec;

            return specification;
        }
    }
}