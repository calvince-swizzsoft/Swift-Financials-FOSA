using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg
{
    public static class AssetTypeSpecifications
    {
        public static Specification<AssetType> DefaultSpec()
        {
            Specification<AssetType> specification = new TrueSpecification<AssetType>();

            return specification;
        }

        public static Specification<AssetType> AssetTypeFullText(string text)
        {
            Specification<AssetType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nameSpec = new DirectSpecification<AssetType>(a => a.Name.Contains(text));

                specification &= (nameSpec);
            }

            return specification;
        }
    }
}
