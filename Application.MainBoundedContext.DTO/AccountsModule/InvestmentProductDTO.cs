using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class InvestmentProductDTO : BindingModelBase<InvestmentProductDTO>
    {
        public InvestmentProductDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent Product")]
        public string ParentChartOfAccountNameDescription { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public string PaddedCode
        {
            get
            {
                return string.Format("{0}", Code).PadLeft(3, '0');
            }
        }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Minimum Balance")]
        public decimal MinimumBalance { get; set; }

        [DataMember]
        [Display(Name = "Maximum Balance")]
        public decimal MaximumBalance { get; set; }

        [DataMember]
        [Display(Name = "Pool Amount")]
        public decimal PoolAmount { get; set; }

        [DataMember]
        [Display(Name = "Recovery Priority")]
        public int Priority { get; set; }

        [DataMember]
        [Display(Name = "Recovery Priority")]
        public string PriorityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecoveryPriority), Priority) ? EnumHelper.GetDescription((RecoveryPriority)Priority) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Maturity Period (Days)")]
        public int MaturityPeriod { get; set; }

        [DataMember]
        [Display(Name = "Annual Percentage Yield")]
        public double AnnualPercentageYield { get; set; }

        [DataMember]
        [Display(Name = "Is refundable?")]
        public bool IsRefundable { get; set; }

        [DataMember]
        [Display(Name = "Is pooled?")]
        public bool IsPooled { get; set; }

        [DataMember]
        [Display(Name = "Is super-saver?")]
        public bool IsSuperSaver { get; set; }

        [DataMember]
        [Display(Name = "Transfer balance to parent product on membership termination?")]
        public bool TransferBalanceToParentOnMembershipTermination { get; set; }

        [DataMember]
        [Display(Name = "Track arrears?")]
        public bool TrackArrears { get; set; }

        [DataMember]
        [Display(Name = "Throttle scheduled arrears recovery?")]
        public bool ThrottleScheduledArrearsRecovery { get; set; }

        [DataMember]
        [Display(Name = "Is locked?")]
        public bool IsLocked { get; set; }


        [DataMember]
        [Display(Name = "Is Mandatory?")]
        public bool IsMandatory { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

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
        [Display(Name = "G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account")]

        public Guid? PoolChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account Type")]
        public int PoolChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account Code")]
        public int PoolChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account Name")]
        public string PoolChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account Name")]
        public string PoolChartOfAccountName
        {
            get
            {
                if (PoolChartOfAccountId != null)
                {
                    return string.Format("{0}-{1} {2}", PoolChartOfAccountAccountType.FirstDigit(), PoolChartOfAccountAccountCode, PoolChartOfAccountAccountName);
                }
                else return string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Pool G/L Account Cost Center")]
        public Guid? PoolChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Pool G/L Account Cost Center")]
        public string PoolChartOfAccountCostCenterDescription { get; set; }


        public ChartOfAccountDTO chartOfAccount { get; set; }

        [DataMember]
        [Display(Name = "Id")]
        public Guid ProductId { get; set; }

        [DataMember]
        [Display(Name = "Product")]
        public string ProductName { get; set; }
    }
}
