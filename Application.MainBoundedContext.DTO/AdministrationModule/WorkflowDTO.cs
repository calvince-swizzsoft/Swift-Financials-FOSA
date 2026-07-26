using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Record")]
        public Guid RecordId { get; set; }

        [Display(Name = "Reference #")]
        public int ReferenceNumber { get; set; }

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

        [Display(Name = "Matched Status")]
        public int MatchedStatus { get; set; }

        [Display(Name = "Matched Status")]
        public string MatchedStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WorkflowMatchedStatus), MatchedStatus) ? EnumHelper.GetDescription((WorkflowMatchedStatus)MatchedStatus) : string.Empty;
            }
        }

        [Display(Name = "System Permission Type")]
        public int SystemPermissionType { get; set; }

        [Display(Name = "System Permission Type")]
        public string SystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), SystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)SystemPermissionType) : string.Empty;
            }
        }

        [Display(Name = "Required Approvals")]
        public int RequiredApprovals { get; set; }

        [Display(Name = "Current Approvals")]
        public int CurrentApprovals { get; set; }

    }
}
