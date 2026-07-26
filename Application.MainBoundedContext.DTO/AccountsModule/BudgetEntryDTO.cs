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
    public class BudgetEntryDTO : BindingModelBase<BudgetEntryDTO>
    {
        public BudgetEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Budget")]
        public Guid BudgetId { get; set; }

        [DataMember]
        [Display(Name = "Budget Name")]
        public string BudgetDescription { get; set; }

        [DataMember]
        [Display(Name = "Budget Branch")]
        public Guid? BudgetBranchId { get; set; }

        [DataMember]
        [Display(Name = "Budget Posting Period")]
        public Guid BudgetPostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Budget Start Date")]
        public DateTime BudgetPostingPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "Budget End Date")]
        public DateTime BudgetPostingPeriodDurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BudgetEntryType), Type) ? EnumHelper.GetDescription((BudgetEntryType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid? ChartOfAccountId { get; set; }

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
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        [ValidGuid]
        public Guid? LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount must be greater than zero!")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Monthly Budget")]
        public decimal MonthlyBudget
        {
            get
            {
                return Amount / 12;
            }
        }

        [DataMember]
        [Display(Name = "Actual To-Date")]
        public decimal ActualToDate { get; set; }

        [DataMember]
        [Display(Name = "Budget To-Date")]
        public decimal BudgetToDate
        {
            get
            {
                return (Amount / 12) * DateTime.Today.Month;
            }
        }

        [DataMember]
        [Display(Name = "Budget Balance")]
        public decimal BudgetBalance { get; set; }

        [DataMember]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public ObservableCollection<BudgetDTO> BudgetEntry { get; set; }
    }
}
