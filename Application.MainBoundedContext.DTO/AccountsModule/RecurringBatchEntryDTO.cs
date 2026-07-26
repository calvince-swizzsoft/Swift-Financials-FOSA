using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class RecurringBatchEntryDTO : BindingModelBase<RecurringBatchEntryDTO>
    {
        public RecurringBatchEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Recurring Batch")]
        public Guid RecurringBatchId { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public int RecurringBatchBatchNumber { get; set; }

        [DataMember]
        [Display(Name = "Batch Number")]
        public string PaddedRecurringBatchBatchNumber
        {
            get
            {
                return string.Format("{0}", RecurringBatchBatchNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid? RecurringBatchBranchId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public Guid? RecurringBatchPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int RecurringBatchType { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public int RecurringBatchMonth { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public string RecurringBatchMonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), RecurringBatchMonth) ? EnumHelper.GetDescription((Month)RecurringBatchMonth) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool RecurringBatchEnforceMonthValueDate { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public int RecurringBatchPriority { get; set; }

        [DataMember]
        [Display(Name = "Priority")]
        public string RecurringBatchPriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(QueuePriority), RecurringBatchPriority) ? EnumHelper.GetDescription((QueuePriority)RecurringBatchPriority) : string.Empty;
            }
        }
        
        [DataMember]
        [Display(Name = "Batch Reference")]
        public string RecurringBatchReference { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid? CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        public CustomerAccountDTO CustomerAccount { get; set; }

        [DataMember]
        [Display(Name = "Secondary Customer Account")]
        public Guid? SecondaryCustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Secondary Customer Account")]
        public CustomerAccountDTO SecondaryCustomerAccount { get; set; }

        [DataMember]
        [Display(Name = "Standing Order")]
        public Guid? StandingOrderId { get; set; }

        [DataMember]
        [Display(Name = "Standing Order")]
        public StandingOrderDTO StandingOrder { get; set; }

        [DataMember]
        [Display(Name = "e-Statement Order")]
        public Guid? ElectronicStatementOrderId { get; set; }

        [DataMember]
        [Display(Name = "e-Statement Order")]
        public ElectronicStatementOrderDTO ElectronicStatementOrder { get; set; }

        [DataMember]
        [Display(Name = "e-Statement Start Date")]
        public DateTime ElectronicStatementStartDate { get; set; }

        [DataMember]
        [Display(Name = "e-Statement End Date")]
        public DateTime ElectronicStatementEndDate { get; set; }

        [DataMember]
        [Display(Name = "e-Statement Sender")]
        public string ElectronicStatementSender { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Interest Capitalization Months")]
        public int InterestCapitalizationMonths { get; set; }

        [DataMember]
        [Display(Name = "Enforce Ceiling?")]
        public bool EnforceCeiling { get; set; }

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
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
