using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class DebitBatchDTO : BindingModelBase<DebitBatchDTO>
    {
        public DebitBatchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "DebitType")]
        [ValidGuid]
        public Guid DebitTypeId { get; set; }

        [DataMember]
        [Display(Name = "DebitType")]
        public string DebitTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "DebitTypeProductCode")]
        public int DebitTypeCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Allow Debit Batch To Overdraw Account?")]
        public bool BranchCompanyAllowDebitBatchToOverdrawAccount { get; set; }

        [DataMember]
        [Display(Name = "BatchNumber")]
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
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public string PriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), Priority) ? EnumHelper.GetDescription((QueuePriority)Priority) : string.Empty;
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
                return Enum.IsDefined(typeof(BatchStatus), Status) ? EnumHelper.GetDescription((BatchStatus)Status) : string.Empty;
            }
        }


        [DataMember]
        [Display(Name = "Debit Batch Auth Option")]
        public byte BatchAuthOption { get; set; }

        [DataMember]
        [Display(Name = "Debit Batch Auth Option")]
        public string BatchAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), (int)BatchAuthOption) ? EnumHelper.GetDescription((BatchAuthOption)BatchAuthOption) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Verified/RejectedBy")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/RejectionRemarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/RejectedDate")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Authorized/RejectedBy")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/RejectionRemarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/RejectedDate")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public ObservableCollection<DebitBatchEntryDTO> DebitBatchEntries { get; set; }
        [DataMember]
        
        public DebitBatchEntryDTO DebitBatchEntry { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string CustomerAccountTypeProductCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ProductCode), CustomerAccountTypeProductCode) ? EnumHelper.GetDescription((ProductCode)CustomerAccountTypeProductCode) : string.Empty;
            }
        }
        [DataMember]
        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), RecordStatus) ? EnumHelper.GetDescription((RecordStatus)RecordStatus) : string.Empty;
            }
        }

    }
}
