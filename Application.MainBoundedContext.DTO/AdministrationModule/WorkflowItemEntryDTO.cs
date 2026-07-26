using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowItemEntryDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Workflow Item")]
        public Guid WorkflowItemId { get; set; }

        [Display(Name = "Role")]
        public string WorkflowItemRoleName { get; set; }

        [Display(Name = "Workflow")]
        public Guid WorkflowItemWorkflowId { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Decision")]
        public string Decision { get; set; }

        [Display(Name = "Used Biometrics?")]
        public bool UsedBiometrics { get; set; }

        [Display(Name = "Approved By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Approval Date")]
        public DateTime CreatedDate { get; set; }
    }
}
