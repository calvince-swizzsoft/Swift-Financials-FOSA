using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg
{
    public static class PackageTypeFactory
    {
        public static PackageType CreatePackageType(string name, string remarks)
        {
            var packageType = new PackageType();

            packageType.GenerateNewIdentity();

            packageType.Name = name;

            packageType.Remarks = remarks;

            packageType.CreatedDate = DateTime.Now;

            return packageType;
        }
    }
}
