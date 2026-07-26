using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class PostingPeriodDTO : BindingModelBase<PostingPeriodDTO>
    {
        public PostingPeriodDTO()
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
        [CustomValidation(typeof(PostingPeriodDTO), "CheckDurationEndDate")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [DataMember]
        [Display(Name = "Is Closed?")]
        public bool IsClosed { get; set; }

        [DataMember]
        [Display(Name = "Closed By")]
        public string ClosedBy { get; set; }

        [DataMember]
        [Display(Name = "Closed Date")]
        public DateTime? ClosedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public object ErrorMessageResult { get; set; }

        public static ValidationResult CheckDurationEndDate(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as PostingPeriodDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be PostingPeriodDTO");

            if (bindingModel.DurationEndDate == null || bindingModel.DurationStartDate == null)
                return new ValidationResult("The duration dates must be specified.");
            else if (bindingModel.DurationEndDate <= bindingModel.DurationStartDate)
                return new ValidationResult("The end date must be greater than the start date.");

            return ValidationResult.Success;
        }
    }
}
