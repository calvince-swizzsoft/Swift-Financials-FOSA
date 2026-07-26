using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class WorkflowBindingModel : BindingModelBase<WorkflowBindingModel>
    {
        public WorkflowBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Record")]
        [ValidGuid]
        public Guid RecordId { get; set; }

        [DataMember]
        [Display(Name = "Reference #")]
        public int ReferenceNumber { get; set; }

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
        [Display(Name = "Matched Status")]
        public int MatchedStatus { get; set; }

        [DataMember]
        [Display(Name = "Matched Status")]
        public string MatchedStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WorkflowMatchedStatus), MatchedStatus) ? EnumHelper.GetDescription((WorkflowMatchedStatus)MatchedStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "System Permission Type")]
        public int SystemPermissionType { get; set; }

        [DataMember]
        [Display(Name = "System Permission Type")]
        public string SystemPermissionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemPermissionType), SystemPermissionType) ? EnumHelper.GetDescription((SystemPermissionType)SystemPermissionType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Required Approvals")]
        public int RequiredApprovals { get; set; }

        [DataMember]
        [Display(Name = "Current Approvals")]
        public int CurrentApprovals { get; set; }

    }
}
