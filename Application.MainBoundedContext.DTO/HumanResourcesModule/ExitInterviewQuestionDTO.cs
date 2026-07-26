using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class ExitInterviewQuestionDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Question")]
        public string Description { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }
    }
}
