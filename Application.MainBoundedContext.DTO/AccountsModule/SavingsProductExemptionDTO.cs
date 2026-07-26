using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SavingsProductExemptionDTO : BindingModelBase<SavingsProductExemptionDTO>
    {
        public SavingsProductExemptionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        [ValidGuid]
        public Guid SavingsProductId { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        public string SavingsProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int BranchCode { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Maximum Withdrawal")]
        public decimal MaximumAllowedWithdrawal { get; set; }

        [DataMember]
        [Display(Name = "Maximum Allowed Deposit")]
        public decimal MaximumAllowedDeposit { get; set; }

        [DataMember]
        [Display(Name = "Minimum Balance")]
        public decimal MinimumBalance { get; set; }

        [DataMember]
        [Display(Name = "Operating Balance")]
        public decimal OperatingBalance { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Notice Amount")]
        public decimal WithdrawalNoticeAmount { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Notice Period (Days)")]
        public int WithdrawalNoticePeriod { get; set; }

        [DataMember]
        [Display(Name = "Withdrawal Interval (Days)")]
        public int WithdrawalInterval { get; set; }

        [DataMember]
        [Display(Name = "Annual Percentage Yield")]
        public double AnnualPercentageYield { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
