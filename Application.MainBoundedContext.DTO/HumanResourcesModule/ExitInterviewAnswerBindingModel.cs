using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class ExitInterviewAnswerBindingModel : BindingModelBase<ExitInterviewAnswerBindingModel>
    {
        public ExitInterviewAnswerBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Question")]
        [ValidGuid]
        public Guid ExitInterviewQuestionId { get; set; }

        [DataMember]
        [Display(Name = "Question")]
        public string ExitInterviewQuestionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        [ValidGuid]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Answer")]
        public string Answer { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; private set; }
    }
}
