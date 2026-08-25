namespace Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg
{
    public class PackageType : Domain.Seedwork.Entity
    {
        public string Name { get; set; }

        public string Remarks { get; set; }
    }
}
