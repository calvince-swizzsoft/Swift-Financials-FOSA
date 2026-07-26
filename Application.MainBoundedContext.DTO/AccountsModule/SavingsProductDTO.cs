using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SavingsProductDTO : BindingModelBase<SavingsProductDTO>
    {
        public SavingsProductDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

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


        string _description; 
        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description {

            get { return _description; }

            set { if (_description != value)
                {

                    _description = value;

                    OnPropertyChanged(() => Description);
                }
                    
                        }
        
        }

        [DataMember]
        [Display(Name = "Maximum Allowed Withdrawal")]
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
        [Display(Name = "Recovery Priority")]
        public int Priority { get; set; }

        // Added DTO
        [DataMember]
        [Display(Name = "Recovery Priority")]
        public string PriorityDescription { get; set; }


        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Is Default?")]
        public bool IsDefault { get; set; }

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
        [Display(Name = "AutomateLedgerFeeCalculation?")]
        public bool AutomateLedgerFeeCalculation { get; set; }

        [DataMember]
        [Display(Name = "ThrottleOver-The-CounterWithdrawals?")]
        public bool ThrottleOverTheCounterWithdrawals { get; set; }

        public CustomerAccountDTO Accounts { get; set; }



        [DataMember]
        [Display(Name = "Charge Born By")]
        public int ChargeBenefactor { get; set; }

        [DataMember]
        [Display(Name = "Charge Born By")]
        public string ChargeBenefactorDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeBenefactor), ChargeBenefactor) ? EnumHelper.GetDescription((ChargeBenefactor)ChargeBenefactor) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Charge Type")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

    }
}
