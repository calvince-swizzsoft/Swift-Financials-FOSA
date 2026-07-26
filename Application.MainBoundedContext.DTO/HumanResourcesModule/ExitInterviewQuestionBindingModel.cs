using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class ExitInterviewQuestionBindingModel : BindingModelBase<ExitInterviewQuestionBindingModel>
    {
        public ExitInterviewQuestionBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Question")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }
    }
}
