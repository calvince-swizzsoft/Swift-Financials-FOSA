using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO
{
    public class InventoryCatalogDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string BaseUOM { get; set; }
        public string PackageType { get; set; }
        public string GLAccount { get; set; }
        public string MainSupplier { get; set; }

        public int ReorderPoint { get; set; }
        public int MaximumOrder { get; set; }
        public int UnitsPerPack { get; set; }
        public int PaletteTI { get; set; }
        public int PaletteHI { get; set; }

        public decimal UnitNetWeight { get; set; }
        public decimal UnitGrossWeight { get; set; }
        public int LeadDays { get; set; }
        public int EconomicOrder { get; set; }

        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public int MonthlyDemand { get; set; }

        public string Remarks { get; set; }
    }
}
