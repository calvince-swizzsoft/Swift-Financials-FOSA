namespace Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg
{
    public class AssetType : Domain.Seedwork.Entity
    {
        public string Name { get; set; }

        // Backed by Infrastructure.Crosscutting.Framework.Utils.Enumerations.DepreciationMethod.
        // The enum is [Flags]-shaped like several others in this codebase, but per Asset
        // Types.md an asset type has exactly one depreciation method selected from a
        // dropdown — this is never a bitwise combination, same treatment as the frontend's
        // other transcribed [Flags] enums (see frontOfficeEnums.js's header note).
        public int DepreciationMethod { get; set; }

        public int UsefulLife { get; set; }

        public bool IsTangible { get; set; }
    }
}
