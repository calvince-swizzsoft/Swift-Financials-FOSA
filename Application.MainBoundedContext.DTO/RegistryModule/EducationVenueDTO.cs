using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class EducationVenueDTO : BindingModelBase<EducationVenueDTO>
    {
        public EducationVenueDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Venue")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Enforce Investments Balance?")]
        public bool EnforceInvestmentsBalance { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
