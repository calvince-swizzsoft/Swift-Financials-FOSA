using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ModuleNavigationItemAgg
{
    public static class ModuleNavigationItemSpecifications
    {
        public static Specification<ModuleNavigationItem> ModuleNavigationItemCode(int itemCode)
        {
            Specification<ModuleNavigationItem> specification = new DirectSpecification<ModuleNavigationItem>(m => m.Code == itemCode);

            return specification;
        }
    }
}
