using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg
{
    public static class CategorySpecifications
    {
        public static Specification<Category> DefaultSpec()
        {
            Specification<Category> specification = new TrueSpecification<Category>();

            return specification;
        }

        public static Specification<Category> CategoryFullText(string text)
        {
            Specification<Category> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Category>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<Category> CategoryWithLevel(int level)
        {
            Specification<Category> specification = DefaultSpec();

            return specification;
        }
    }
}
