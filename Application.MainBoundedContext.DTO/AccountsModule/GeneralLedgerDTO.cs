using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class GeneralLedgerDTO : BindingModelBase<GeneralLedgerDTO>
    {
        public GeneralLedgerDTO()
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
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Ledger Number")]
        public int LedgerNumber { get; set; }

        [DataMember]
        [Display(Name = "Ledger Number")]
        public string PaddedLedgerNumber
        {
            get
            {
                return string.Format("{0}", LedgerNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Total Value")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total value must be greater than zero!")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GeneralLedgerStatus), Status) ? EnumHelper.GetDescription((GeneralLedgerStatus)Status) : string.Empty;
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
        [Display(Name = "Can Suppress Maker/Checker Validation?")]
        public bool CanSuppressMakerCheckerValidation { get; set; }
        public List<GeneralLedgerDTO> generalLedgerDTOs;
        public GeneralLedgerDTO LedgerDTO { get; set; }

        [DataMember]
        public ObservableCollection<GeneralLedgerEntryDTO> GeneralLedgerEntries { get; set; }



        // Additional DTOs
        [DataMember]
        [Display(Name = "Refund Batch Auth Option")]
        public byte GeneralLedgerAuthOption { get; set; }

        [DataMember]
        [Display(Name = "Refund Batch Auth Option")]
        public string RefundAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), (int)GeneralLedgerAuthOption) ? EnumHelper.GetDescription((BatchAuthOption)GeneralLedgerAuthOption) : string.Empty;
            }
        }

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

        public object ErrorMessageResult { get; set; }
    }
}
