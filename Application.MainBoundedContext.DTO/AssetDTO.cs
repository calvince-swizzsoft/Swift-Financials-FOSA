using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO
{
    public class AssetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public string AssetType { get; set; }
        public string Supplier { get; set; }
        public string Department { get; set; }
        public string PicturePath { get; set; }
        public string GLAccount { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string TagNumber { get; set; }
        public string Location { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ResidualValue { get; set; }
    }

}
