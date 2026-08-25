using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class PackageTypeDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
