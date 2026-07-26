using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowItemBindingModel : BindingModelBase<WorkflowItemBindingModel>
    {
        public WorkflowItemBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Workflow")]
        public Guid WorkflowId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid WorkflowBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string WorkflowBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Record")]
        public Guid WorkflowRecordId { get; set; }

        [Display(Name = "Required Approvals - Workflow")]
        public int WorkflowRequiredApprovals { get; set; }

        [Display(Name = "Current Approvals - Workflow")]
        public int WorkflowCurrentApprovals { get; set; }

        [DataMember]
        [Display(Name = "System Permission Type")]
        public int WorkflowSystemPermissionType { get; set; }

        [DataMember]
        [Display(Name = "System Permission Type")]
        public string WorkflowSystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), WorkflowSystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)WorkflowSystemPermissionType) : string.Empty;
            }
        }
        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WorkflowRecordStatus), Status) ? EnumHelper.GetDescription((WorkflowRecordStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Required Approvals")]
        public int RequiredApprovals { get; set; }

        [DataMember]
        [Display(Name = "Current Approvals")]
        public int CurrentApprovals { get; set; }

        [DataMember]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [DataMember]
        [Display(Name = "Approval Priority")]
        public int ApprovalPriority { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// To be used before persisting to the DB.
        /// Determines if the item being persisted is the last in the overall approval chain.
        /// </summary>
        [DataMember]
        [Display(Name = "Is Last Item In Overall Approval Chain?")]
        public bool IsLastItemInOverallApprovalChain { get; set; }
    }
}
