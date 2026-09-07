using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg
{
    public static class AssetTypeFactory
    {
        public static AssetType CreateAssetType(string name, int depreciationMethod, int usefulLife, bool isTangible, System.Guid depreciationExpenseAccountId = default(System.Guid), System.Guid accumulatedDepreciationAccountId = default(System.Guid))
        {
            var assetType = new AssetType();

            assetType.GenerateNewIdentity();

            assetType.Name = name;

            assetType.DepreciationMethod = depreciationMethod;

            assetType.UsefulLife = usefulLife;

           assetType.IsTangible = isTangible;
            assetType.DepreciationExpenseAccountId = depreciationExpenseAccountId;
            assetType.AccumulatedDepreciationAccountId = accumulatedDepreciationAccountId;

            assetType.CreatedDate = DateTime.Now;

            return assetType;
        }
    }
}
