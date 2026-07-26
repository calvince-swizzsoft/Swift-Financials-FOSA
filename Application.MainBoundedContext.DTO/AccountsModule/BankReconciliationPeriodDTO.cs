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
    public class BankReconciliationPeriodDTO : BindingModelBase<BankReconciliationPeriodDTO>
    {
        public BankReconciliationPeriodDTO()
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
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Bank Linkage")]
        [ValidGuid]
        public Guid BankLinkageId { get; set; }

        [DataMember]
        [Display(Name = "Bank")]
        public string BankLinkageBankName { get; set; }

        [DataMember]
        [Display(Name = "Bank Branch")]
        public string BankLinkageBankBranchName { get; set; }

        [DataMember]
        [Display(Name = "Bank Account Number")]
        public string BankLinkageBankAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Bank Account Number")]
        [Required]
        public string BankAccountNumber { get; set; }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        [CustomValidation(typeof(BankReconciliationPeriodDTO), "CheckDurationEndDate")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Bank Account Balance")]
        public decimal BankAccountBalance { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Balance")]
        public decimal GeneralLedgerAccountBalance { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BankReconciliationPeriodStatus), Status) ? EnumHelper.GetDescription((BankReconciliationPeriodStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejection Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public static ValidationResult CheckDurationEndDate(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as BankReconciliationPeriodDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be BankReconciliationPeriodDTO");

            if (bindingModel.DurationEndDate == null || bindingModel.DurationStartDate == null)
                return new ValidationResult("The duration dates must be specified.");
            else if (bindingModel.DurationEndDate <= bindingModel.DurationStartDate)
                return new ValidationResult("The end date must be greater than the start date.");

            return ValidationResult.Success;
        }
        [DataMember]
        [Display(Name = "Cheque Number")]
        public string ChequeNumber { get; set; }

        [DataMember]
        [Display(Name = "Cheque Drawee")]
        public string ChequeDrawee { get; set; }


        [DataMember]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        public BankReconciliationEntryDTO bankReconciliationEntryDTOs;
    }
}
