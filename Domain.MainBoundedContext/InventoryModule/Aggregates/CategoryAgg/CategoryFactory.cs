using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg
{
    public static class CategoryFactory
    {
        public static Category CreateCategory(string description)
        {
            var category = new Category();

            category.GenerateNewIdentity();

            category.Description = description;

            category.CreatedDate = DateTime.Now;

            return category;
        }
    }
}
