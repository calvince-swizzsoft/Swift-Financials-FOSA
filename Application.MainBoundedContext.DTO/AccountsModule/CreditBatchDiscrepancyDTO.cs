using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditBatchDiscrepancyDTO : BindingModelBase<CreditBatchDiscrepancyDTO>
    {
        public CreditBatchDiscrepancyDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Credit Batch")]
        [ValidGuid]
        public Guid CreditBatchId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type")]
        public Guid CreditBatchCreditTypeId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account")]
        public Guid CreditBatchCreditTypeChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Code")]
        public int CreditBatchCreditTypeChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Credit Type G/L Account Name")]
        public string CreditBatchCreditTypeChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid CreditBatchBranchId { get; set; }

        [Display(Name = "Branch")]
        public string CreditBatchBranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid? CreditBatchPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string CreditBatchPostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Posting Period Start Date")]
        public DateTime CreditBatchPostingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "Posting Period End Date")]
        public DateTime CreditBatchPostingPeriodDurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int CreditBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedCreditBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", CreditBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Batch Type")]
        public int CreditBatchType { get; set; }

        [DataMember]
        [Display(Name = "Batch Type")]
        public string CreditBatchTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CreditBatchType), CreditBatchType) ? EnumHelper.GetDescription((CreditBatchType)CreditBatchType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public int CreditBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Batch Priority")]
        public string CreditBatchPriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), CreditBatchPriority) ? EnumHelper.GetDescription((QueuePriority)CreditBatchPriority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Status")]
        public int CreditBatchStatus { get; set; }

        [DataMember]
        [Display(Name = "Batch Status")]
        public string CreditBatchStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchStatus), CreditBatchStatus) ? EnumHelper.GetDescription((BatchStatus)CreditBatchStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Month")]
        public int CreditBatchMonth { get; set; }

        [DataMember]
        [Display(Name = "Batch Month")]
        public string CreditBatchMonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), CreditBatchMonth) ? EnumHelper.GetDescription((Month)CreditBatchMonth) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Batch Value Date")]
        public DateTime? CreditBatchValueDate { get; set; }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool CreditBatchEnforceMonthValueDate { get; set; }

        [DataMember]
        [Display(Name = "Batch Reference")]
        public string CreditBatchReference { get; set; }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal CreditBatchTotalValue { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Column 1")]
        public string Column1 { get; set; }

        [DataMember]
        [Display(Name = "Column 2")]
        public string Column2 { get; set; }

        [DataMember]
        [Display(Name = "Column 3")]
        public string Column3 { get; set; }

        [DataMember]
        [Display(Name = "Column 4")]
        public string Column4 { get; set; }

        [DataMember]
        [Display(Name = "Column 5")]
        public string Column5 { get; set; }

        [DataMember]
        [Display(Name = "Column 6")]
        public string Column6 { get; set; }

        [DataMember]
        [Display(Name = "Column 7")]
        public string Column7 { get; set; }

        [DataMember]
        [Display(Name = "Column 8")]
        public string Column8 { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
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
                return Enum.IsDefined(typeof(BatchEntryStatus), Status) ? EnumHelper.GetDescription((BatchEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Posted By")]
        public string PostedBy { get; set; }

        [DataMember]
        [Display(Name = "Posted Date")]
        public DateTime? PostedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
