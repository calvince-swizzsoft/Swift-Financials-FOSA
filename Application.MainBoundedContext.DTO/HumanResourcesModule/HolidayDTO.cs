using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class HolidayDTO : BindingModelBase<HolidayDTO>
    {
        public HolidayDTO()
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
        [Display(Name = "Posting Period Start Date")]
        public DateTime PostingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "Posting Period End Date")]
        public DateTime PostingPeriodDurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        [Required]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        [Required]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [CustomValidation(typeof(HolidayDTO), "CheckDates", ErrorMessage = "Invalid duration!")]
        public int ValidateDates { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public static ValidationResult CheckDates(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as HolidayDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be HolidayDTO");

            if (bindingModel.DurationStartDate < bindingModel.PostingPeriodDurationStartDate || bindingModel.DurationStartDate > bindingModel.PostingPeriodDurationEndDate)
                return new ValidationResult(string.Format("Invalid duration!", value));
            else if (bindingModel.DurationEndDate < bindingModel.PostingPeriodDurationStartDate || bindingModel.DurationEndDate > bindingModel.PostingPeriodDurationEndDate)
                return new ValidationResult(string.Format("Invalid duration!", value));
            else if (bindingModel.DurationStartDate > bindingModel.DurationEndDate)
                return new ValidationResult(string.Format("Invalid duration!", value));

            return ValidationResult.Success;
        }

        
    }
}

