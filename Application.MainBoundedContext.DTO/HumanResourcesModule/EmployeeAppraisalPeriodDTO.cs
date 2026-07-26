using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeAppraisalPeriodDTO : BindingModelBase<EmployeeAppraisalPeriodDTO>
    {
        public EmployeeAppraisalPeriodDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        [CustomValidation(typeof(EmployeeAppraisalPeriodDTO), "CheckDurationEndDate")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public static ValidationResult CheckDurationEndDate(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as EmployeeAppraisalPeriodDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be EmployeeAppraisalPeriodDTO");

            if (bindingModel.DurationEndDate == null || bindingModel.DurationStartDate == null)
                return new ValidationResult("The duration dates must be specified.");
            else if (bindingModel.DurationEndDate <= bindingModel.DurationStartDate)
                return new ValidationResult("The end date must be greater than the start date.");

            return ValidationResult.Success;
        }
    }
}
