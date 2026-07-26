using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class ExpensePayableDTO : BindingModelBase<ExpensePayableDTO>
    {
        public ExpensePayableDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }
      
        [DataMember]
        [Display(Name = "Branch Id")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        
        public Guid PostingPeriodId { get; set; }

      
        [Display(Name = "PostingPeriod Description")]
        public string PostingPeriodDescription { get; set; }


        [Display(Name = "Branch Name")]

        public string BranchDescription { get; set; }

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

        //new g/l

        [DataMember]
        [Display(Name = "G/L Accounts")]

        public string ChartOfAccounts { get; set; }

    


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
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Voucher Number")]
        public int VoucherNumber { get; set; }

        [DataMember]
        [Display(Name = "Voucher Number")]
        public string PaddedVoucherNumber
        {
            get
            {
                return string.Format("{0}", VoucherNumber).PadLeft(6, '0');
            }
        }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ExpensePayableType), Type) ? EnumHelper.GetDescription((ExpensePayableType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Type")]
        public int ExpensePayableAuthOption { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string ExpensePayableAuthOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ExpensePayableAuthOption), ExpensePayableAuthOption) ? EnumHelper.GetDescription((ExpensePayableAuthOption)ExpensePayableAuthOption) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [Required(ErrorMessage = "Value Date is required.")]
        [Display(Name = "Value Date")]
        public DateTime ValueDate { get; set; }


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
                return Enum.IsDefined(typeof(ExpensePayableStatus), Status) ? EnumHelper.GetDescription((ExpensePayableStatus)Status) : string.Empty;
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
        [Display(Name = "module")]
        public int ModuleNavigationItemCode { get; set; }

        public string ErrorMessageResult;
        public List<ExpensePayableEntryDTO> ExpensePayableEntries { get; set; }
        public ExpensePayableEntryDTO ExpensePayableEntry { get; set; }
        public string errormassage;


        [DataMember]
        [Display(Name = "GeneralTransactionType")]
        public int TransactionType { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TransactionTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GeneralTransactionType), TransactionType) ? EnumHelper.GetDescription((GeneralTransactionType)TransactionType) : string.Empty;
            }
        }

        public CustomerAccountDTO CustomerAccount { get; set; }

        public AccountClosureRequestDTO AccountClosureRequest { get; set; }

        public CreditBatchEntryDTO CreditBatchEntry { get; set; }

        [DataMember]
        [Display (Name = "Reference")]
        public string CashReceiptReference { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal CashReceiptAmount { get; set; }

        public ExternalChequeDTO ExternalChequeDTO { get; set; }

    }



    // new G/L validation


}
