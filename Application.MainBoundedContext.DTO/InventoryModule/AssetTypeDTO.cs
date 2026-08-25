using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class AssetTypeDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        // Infrastructure.Crosscutting.Framework.Utils.Enumerations.DepreciationMethod value —
        // SLN=1, SYD=2, DB=4, DDB=8, VDB=16. Was a free-text string on the original stub DTO;
        // there's a real backing enum, so this now stores that enum's value like every other
        // enum-backed field in the codebase.
        [Display(Name = "Depreciation Method")]
        public int DepreciationMethod { get; set; }

        [Display(Name = "Useful Life (Years)")]
        public int UsefulLife { get; set; }

        [Display(Name = "Is Tangible")]
        public bool IsTangible { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
