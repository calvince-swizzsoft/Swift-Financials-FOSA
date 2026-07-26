using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowItemDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Workflow")]
        public Guid WorkflowId { get; set; }

        [Display(Name = "Branch")]
        public Guid WorkflowBranchId { get; set; }

        [Display(Name = "Branch")]
        public string WorkflowBranchDescription { get; set; }

        [Display(Name = "System Permission Type")]
        public int WorkflowSystemPermissionType { get; set; }

        [Display(Name = "System Permission Type")]
        public string WorkflowSystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), WorkflowSystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)WorkflowSystemPermissionType) : string.Empty;
            }
        }

        [Display(Name = "Reference #")]
        public int WorkflowReferenceNumber { get; set; }

        [Display(Name = "Reference #")]
        public string PaddedReferenceNumber
        {
            get
            {
                return string.Format("{0}", WorkflowReferenceNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Record")]
        public Guid WorkflowRecordId { get; set; }

        [Display(Name = "Required Approvals - Workflow")]
        public int WorkflowRequiredApprovals { get; set; }

        [Display(Name = "Current Approvals - Workflow")]
        public int WorkflowCurrentApprovals { get; set; }

        [Display(Name = "Approval Stage In Workflow")]
        public string OverallApprovalStage
        {
            get
            {
                return string.Format("{0} of {1}", WorkflowCurrentApprovals, WorkflowRequiredApprovals);
            }
        }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WorkflowRecordStatus), Status) ? EnumHelper.GetDescription((WorkflowRecordStatus)Status) : string.Empty;
            }
        }

        [Display(Name = "Required Approvals")]
        public int RequiredApprovals { get; set; }

        [Display(Name = "Current Approvals")]
        public int CurrentApprovals { get; set; }

        [Display(Name = "Approval Stage In Role")]
        public string Stage
        {
            get
            {
                return string.Format("{0} of {1}", CurrentApprovals, RequiredApprovals);
            }
        }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Approval Priority")]
        public int ApprovalPriority { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// To be used before persisting to the DB.
        /// Determines if the item being persisted is the last in the overall approval chain.
        /// </summary>
        [Display(Name = "Is Last Item In Overall Approval Chain?")]
        public bool IsLastItemInOverallApprovalChain
        {
            get
            {
                if ((WorkflowRequiredApprovals - WorkflowCurrentApprovals) == 1 && (RequiredApprovals - CurrentApprovals) == 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
