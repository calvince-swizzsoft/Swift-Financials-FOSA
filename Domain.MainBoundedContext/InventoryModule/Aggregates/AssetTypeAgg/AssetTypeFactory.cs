using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg
{
    public static class AssetTypeFactory
    {
        public static AssetType CreateAssetType(string name, int depreciationMethod, int usefulLife, bool isTangible)
        {
            var assetType = new AssetType();

            assetType.GenerateNewIdentity();

            assetType.Name = name;

            assetType.DepreciationMethod = depreciationMethod;

            assetType.UsefulLife = usefulLife;

            assetType.IsTangible = isTangible;

            assetType.CreatedDate = DateTime.Now;

            return assetType;
        }
    }
}
