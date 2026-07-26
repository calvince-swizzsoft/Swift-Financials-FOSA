using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CommissionExemptionDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Commission")]
        public Guid CommissionId { get; set; }

        [Display(Name = "Commission")]
        public string CommissionDescription { get; set; }

        [Display(Name = "Name")]
        public string Description { get; set; }

        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
