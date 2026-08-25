using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class UnitOfMeasurementDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        // "Contains" per Unit Of Measure.md — how many of BaseUnit this unit is made
        // up of (e.g. "Dozen" contains 12 of base unit "Piece").
        [Display(Name = "Contains")]
        public decimal? Contains { get; set; }

        [Display(Name = "Of Base Units")]
        public Guid? BaseUnitId { get; set; }

        // Populated by UnitOfMeasurementAppService, not AutoMapper-flattened — same
        // treatment as SupplierDTO.ChartOfAccountAccountName.
        [Display(Name = "Base Unit Name")]
        public string BaseUnitName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
