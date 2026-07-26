using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class ExitInterviewAnswerDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Question")]
        public Guid ExitInterviewQuestionId { get; set; }

        [Display(Name = "Question")]
        public string ExitInterviewQuestionDescription { get; set; }

        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Answer")]
        public string Answer { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; private set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
