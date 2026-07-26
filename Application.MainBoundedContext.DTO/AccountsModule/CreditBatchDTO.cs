using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditBatchDTO : BindingModelBase<CreditBatchDTO>
    {
        public CreditBatchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        [ValidGuid]
        public Guid CreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public string CreditTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account")]
        public Guid CreditTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Type")]
        public int CreditTypeChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Code")]
        public int CreditTypeChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Name")]
        public string CreditTypeChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Name")]
        public string CreditTypeChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", CreditTypeChartOfAccountAccountType.FirstDigit(), CreditTypeChartOfAccountAccountCode, CreditTypeChartOfAccountAccountName);
            }
        }
        
        [DataMember]
        [Display(Name = "Credit Type Transaction Ownership")]
        public int CreditTypeTransactionOwnership { get; set; }

        [DataMember]
        [Display(Name = "Credit Type Transaction Ownership")]
        public string CreditTypeTransactionOwnershipDescription
        {
            get
            {
                return Enum.IsDefined(typeof(TransactionOwnership), CreditTypeTransactionOwnership) ? EnumHelper.GetDescription((TransactionOwnership)CreditTypeTransactionOwnership) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid? PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime PostingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime PostingPeriodDurationEndDate { get; set; }

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
                return Enum.IsDefined(typeof(CreditBatchType), Type) ? EnumHelper.GetDescription((CreditBatchType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Reference")]
        [Required]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Total value must be greater than zero!")]
        public decimal TotalValue { get; set; }

        [DataMember]
        [Display(Name = "Recovery Concession")]
        public decimal RecoveryConcession { get; set; }

        [DataMember]
        [Display(Name = "Recover Indefinite Charges?")]
        public bool RecoverIndefiniteCharges { get; set; }

        [DataMember]
        [Display(Name = "Recover Arrearages?")]
        public bool RecoverArrearages { get; set; }

        [DataMember]
        [Display(Name = "Recover Carry Forwards?")]
        public bool RecoverCarryForwards { get; set; }
        
        [DataMember]
        [Display(Name = "Preserve Account Balance?")]
        public bool PreserveAccountBalance { get; set; }

        [DataMember]
        [Display(Name = "Fuzzy Matching?")]
        public bool FuzzyMatching { get; set; }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool EnforceMonthValueDate { get; set; }

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
        [Display(Name = "Month")]
        public int Month { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public string MonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), Month) ? EnumHelper.GetDescription((Month)Month) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Value Date")]
        public DateTime? ValueDate { get; set; }

        [DataMember]
        [Display(Name = "Concession Type")]
        public int ConcessionType { get; set; }

        [DataMember]
        [Display(Name = "Concession Type")]
        public string ConcessionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ConcessionType) ? EnumHelper.GetDescription((ChargeType)ConcessionType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Concession Percentage")]
        public double ConcessionPercentage { get; set; }

        [DataMember]
        [Display(Name = "Concession Fixed Amount")]
        public decimal ConcessionFixedAmount { get; set; }

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
        public CustomerAccountDTO CreditCustomerAccountDTO { get; set; }

        [DataMember]
        public ObservableCollection<CreditBatchEntryDTO> CreditBatchEntries { get; set; }
        [DataMember]
        public CreditBatchEntryDTO CreditBatchEntry { get; set; }






        // Additional DTOs
        [DataMember]
        [Display(Name = "Credit Batch Auth Option")]
        public byte CreditAuthOption { get; set; }

        [DataMember]
        [Display(Name = "Credit Batch Auth Option")]
        public string BatchAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchAuthOption), (int)CreditAuthOption) ? EnumHelper.GetDescription((BatchAuthOption)CreditAuthOption) : string.Empty;
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
    }
}
