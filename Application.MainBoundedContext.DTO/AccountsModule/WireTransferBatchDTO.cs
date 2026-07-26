using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WireTransferBatchDTO : BindingModelBase<WireTransferBatchDTO>
    {
        public WireTransferBatchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type")]
        [ValidGuid]
        public Guid WireTransferTypeId { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type")]
        public string WireTransferTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type G/L Account")]
        public Guid WireTransferTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type Transaction Ownership")]
        public int WireTransferTypeTransactionOwnership { get; set; }

        [DataMember]
        [Display(Name = "Wire Transfer Type Transaction Ownership")]
        public string WireTransferTypeTransactionOwnershipDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TransactionOwnership), WireTransferTypeTransactionOwnership) ? EnumHelper.GetDescription((TransactionOwnership)WireTransferTypeTransactionOwnership) : string.Empty;
            }
        }

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
        [Display(Name = "Batch Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WireTransferBatchType), Type) ? EnumHelper.GetDescription((WireTransferBatchType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Auth Option")]
        public byte BatchAuthOption { get; set; }

        [DataMember]
        [Display(Name = "Auth Option")]
        public string BatchAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), (int)BatchAuthOption) ? EnumHelper.GetDescription((BatchAuthOption)BatchAuthOption) : string.Empty;
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
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public ObservableCollection<WireTransferBatchEntryDTO> WireTransferEntries { get; set; }
        [DataMember]

        public WireTransferBatchEntryDTO WireTransferEntry { get; set; }

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
