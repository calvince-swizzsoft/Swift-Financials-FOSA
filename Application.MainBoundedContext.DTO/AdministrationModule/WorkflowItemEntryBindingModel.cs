using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowItemEntryBindingModel : BindingModelBase<WorkflowItemEntryBindingModel>
    {
        public WorkflowItemEntryBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Workflow Item")]
        public Guid WorkflowItemId { get; set; }

        [DataMember]
        [Display(Name = "Workflow")]
        public Guid WorkflowItemWorkflowId { get; set; }

        [DataMember]
        [Display(Name = "Role")]
        public string WorkflowItemRoleName { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Decision")]
        public string Decision { get; set; }

        [DataMember]
        [Display(Name = "Used Biometrics?")]
        public bool UsedBiometrics { get; set; }

        [DataMember]
        [Display(Name = "Approved By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Approval Date")]
        public DateTime CreatedDate { get; set; }
    }
}
