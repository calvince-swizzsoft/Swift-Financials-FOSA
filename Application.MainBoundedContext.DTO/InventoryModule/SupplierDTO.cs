using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class SupplierDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Land-Line")]
        public string LandLine { get; set; }

        [Display(Name = "Mobile-Line")]
        public string MobileLine { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "G/L Account")]
        public Guid ChartOfAccountId { get; set; }

        // Populated by SupplierAppService from IChartOfAccountAppService.FindChartOfAccount —
        // not an AutoMapper flattening of the ChartOfAccount navigation property, so it's
        // reliable regardless of whether that navigation was eager-loaded.
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
