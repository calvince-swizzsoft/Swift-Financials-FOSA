using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeAppraisalPeriodRecommendationDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Recommendation")]
        public string Recommendation { get; set; }

        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Employee Appraisal Period")]
        public Guid EmployeeAppraisalPeriodId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
