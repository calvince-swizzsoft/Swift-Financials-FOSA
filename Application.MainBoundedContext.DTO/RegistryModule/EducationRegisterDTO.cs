using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class EducationRegisterDTO : BindingModelBase<EducationRegisterDTO>
    {
        public EducationRegisterDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Venue")]
        [ValidGuid]
        public Guid EducationVenueId { get; set; }

        [DataMember]
        [Display(Name = "Venue")]
        public string EducationVenueDescription { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        [CustomValidation(typeof(EducationRegisterDTO), "CheckDurationEndDate")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public static ValidationResult CheckDurationEndDate(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as EducationRegisterDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be EducationRegisterDTO");

            if (bindingModel.DurationEndDate == null || bindingModel.DurationStartDate == null)
                return new ValidationResult("The duration dates must be specified.");
            else if (bindingModel.DurationEndDate < bindingModel.DurationStartDate)
                return new ValidationResult("The end date must be greater than the start date.");

            return ValidationResult.Success;
        }
    }
}
