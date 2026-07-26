using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalReversalBatchDTO : BindingModelBase<JournalReversalBatchDTO>
    {
        public JournalReversalBatchDTO()
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

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int BatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedBatchNumber
        {
            get
            {
                return string.Format("{0}", BatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks2 { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), Status) ? EnumHelper.GetDescription((BatchStatus)Status) : string.Empty;
            }
        }
        
        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public int Priority { get; set; }
    }
}
