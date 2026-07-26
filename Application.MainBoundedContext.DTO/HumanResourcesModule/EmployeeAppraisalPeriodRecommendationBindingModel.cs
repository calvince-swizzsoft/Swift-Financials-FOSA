using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeAppraisalPeriodRecommendationBindingModel : BindingModelBase<EmployeeAppraisalPeriodRecommendationBindingModel>
    {
        public EmployeeAppraisalPeriodRecommendationBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Recommendation")]
        [Required]
        public string Recommendation { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        [Required]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Employee Appraisal Period")]
        [Required]
        public Guid EmployeeAppraisalPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
