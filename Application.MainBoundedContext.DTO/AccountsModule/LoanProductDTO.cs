using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductDTO : BindingModelBase<LoanProductDTO>
    {
        public LoanProductDTO()
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

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string ExtendedDescription
        {
            get
            {
                return string.Format("{0} ({1})", Description, LoanRegistrationLoanProductSectionDescription);
            }
        }

        [DataMember]
        [Display(Name = "Annual Percentage Rate")]
        public double LoanInterestAnnualPercentageRate { get; set; }

        [DataMember]
        [Display(Name = "Interest Charge Mode")]
        public int LoanInterestChargeMode { get; set; }

        [DataMember]
        [Display(Name = "Interest Charge Mode")]
        public string LoanInterestChargeModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(InterestChargeMode), LoanInterestChargeMode) ? EnumHelper.GetDescription((InterestChargeMode)LoanInterestChargeMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Interest Recovery Mode")]
        public int LoanInterestRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Interest Recovery Mode")]
        public string LoanInterestRecoveryModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(InterestRecoveryMode), LoanInterestRecoveryMode) ? EnumHelper.GetDescription((InterestRecoveryMode)LoanInterestRecoveryMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Interest Calculation Mode")]
        public int LoanInterestCalculationMode { get; set; }

        [DataMember]
        [Display(Name = "Interest Calculation Mode")]
        public string LoanInterestCalculationModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(InterestCalculationMode), LoanInterestCalculationMode) ? EnumHelper.GetDescription((InterestCalculationMode)LoanInterestCalculationMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Term (Months)")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Term must be greater than zero!")]
        public int LoanRegistrationTermInMonths { get; set; }

        [DataMember]
        [Display(Name = "Minimum Principal Amount")]
        public decimal LoanRegistrationMinimumAmount { get; set; }

        [DataMember]
        [Display(Name = "Maximum Principal Amount")]
        public decimal LoanRegistrationMaximumAmount { get; set; }

        [DataMember]
        [Display(Name = "Minimum Chargeable Interest Amount")]
        public decimal LoanRegistrationMinimumInterestAmount { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public int LoanRegistrationLoanProductSection { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string LoanRegistrationLoanProductSectionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductSection), LoanRegistrationLoanProductSection) ? EnumHelper.GetDescription((LoanProductSection)LoanRegistrationLoanProductSection) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Category")]
        public int LoanRegistrationLoanProductCategory { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string LoanRegistrationLoanProductCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductCategory), LoanRegistrationLoanProductCategory) ? EnumHelper.GetDescription((LoanProductCategory)LoanRegistrationLoanProductCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Consecutive Income")]
        public int LoanRegistrationConsecutiveIncome { get; set; }

        [DataMember]
        [Display(Name = "Investments Multiplier")]
        public double LoanRegistrationInvestmentsMultiplier { get; set; }

        [DataMember]
        [Display(Name = "Minimum Guarantors")]
        public int LoanRegistrationMinimumGuarantors { get; set; }

        [DataMember]
        [Display(Name = "Maximum Guarantees")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Maximum guarantees must be greater than zero!")]
        public int LoanRegistrationMaximumGuarantees { get; set; }

        [DataMember]
        [Display(Name = "Reject if owing?")]
        public bool LoanRegistrationRejectIfMemberHasBalance { get; set; }

        [DataMember]
        [Display(Name = "Enforce security rules?")]
        public bool LoanRegistrationSecurityRequired { get; set; }

        [DataMember]
        [Display(Name = "Allow self-guarantee?")]
        public bool LoanRegistrationAllowSelfGuarantee { get; set; }

        [DataMember]
        [Display(Name = "Grace Period (Days)")]
        public int LoanRegistrationGracePeriod { get; set; }

        [DataMember]
        [Display(Name = "Minimum Membership Period (Months)")]
        public int LoanRegistrationMinimumMembershipPeriod { get; set; }

        [DataMember]
        [Display(Name = "Payment Frequency Per Year")]
        [CustomValidation(typeof(LoanProductDTO), "ValidateTermVsPaymentFrequencyPerYear", ErrorMessage = "Term vs. Payment Frequency Per Year not valid!")]
        public int LoanRegistrationPaymentFrequencyPerYear { get; set; }

        [DataMember]
        [Display(Name = "Payment Frequency Per Year")]
        public string LoanRegistrationPaymentFrequencyPerYearDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PaymentFrequencyPerYear), LoanRegistrationPaymentFrequencyPerYear) ? EnumHelper.GetDescription((PaymentFrequencyPerYear)LoanRegistrationPaymentFrequencyPerYear) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payment Due Date")]
        public int LoanRegistrationPaymentDueDate { get; set; }

        [DataMember]
        [Display(Name = "Payment Due Date")]
        public string LoanRegistrationPaymentDueDateDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PaymentDueDate), LoanRegistrationPaymentDueDate) ? EnumHelper.GetDescription((PaymentDueDate)LoanRegistrationPaymentDueDate) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payout Recovery Mode")]
        public int LoanRegistrationPayoutRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Payout Recovery Mode")]
        public string LoanRegistrationPayoutRecoveryModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(PayoutRecoveryMode), LoanRegistrationPayoutRecoveryMode) ? EnumHelper.GetDescription((PayoutRecoveryMode)LoanRegistrationPayoutRecoveryMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payout Recovery Percentage")]
        public double LoanRegistrationPayoutRecoveryPercentage { get; set; }

        [DataMember]
        [Display(Name = "Aggregate Check-Off Recovery Mode")]
        public int LoanRegistrationAggregateCheckOffRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Aggregate Check-Off Recovery Mode")]
        public string LoanRegistrationAggregateCheckOffRecoveryModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AggregateCheckOffRecoveryMode), LoanRegistrationAggregateCheckOffRecoveryMode) ? EnumHelper.GetDescription((AggregateCheckOffRecoveryMode)LoanRegistrationAggregateCheckOffRecoveryMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Charge clearance fee?")]
        public bool LoanRegistrationChargeClearanceFee { get; set; }

        [DataMember]
        [Display(Name = "Microcredit?")]
        public bool LoanRegistrationMicrocredit { get; set; }

        [DataMember]
        [Display(Name = "Standing Order Trigger")]
        public int LoanRegistrationStandingOrderTrigger { get; set; }

        [DataMember]
        [Display(Name = "Standing Order Trigger")]
        public string LoanRegistrationStandingOrderTriggerDescription
        {
            get
            {
                return Enum.IsDefined(typeof(StandingOrderTrigger), LoanRegistrationStandingOrderTrigger) ? EnumHelper.GetDescription((StandingOrderTrigger)LoanRegistrationStandingOrderTrigger) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Track arrears?")]
        public bool LoanRegistrationTrackArrears { get; set; }

        [DataMember]
        [Display(Name = "Charge arrears fee?")]
        public bool LoanRegistrationChargeArrearsFee { get; set; }

        [DataMember]
        [Display(Name = "Enforce system appraisal recommendation?")]
        public bool LoanRegistrationEnforceSystemAppraisalRecommendation { get; set; }

        [DataMember]
        [Display(Name = "Bypass verification?")]
        public bool LoanRegistrationBypassAudit { get; set; }

        [DataMember]
        [Display(Name = "Maximum self-guarantee eligible percentage")]
        public double LoanRegistrationMaximumSelfGuaranteeEligiblePercentage { get; set; }

        [DataMember]
        [Display(Name = "Guarantor Security Mode")]
        public int LoanRegistrationGuarantorSecurityMode { get; set; }

        [DataMember]
        [Display(Name = "Guarantor Security Mode")]
        public string LoanRegistrationGuarantorSecurityModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(GuarantorSecurityMode), LoanRegistrationGuarantorSecurityMode) ? EnumHelper.GetDescription((GuarantorSecurityMode)LoanRegistrationGuarantorSecurityMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public int LoanRegistrationRoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string LoanRegistrationRoundingTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RoundingType), LoanRegistrationRoundingType) ? EnumHelper.GetDescription((RoundingType)LoanRegistrationRoundingType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Disburse micro loan less deductions?")]
        public bool LoanRegistrationDisburseMicroLoanLessDeductions { get; set; }

        [DataMember]
        [Display(Name = "Exclude outstanding loans on maximum entitlement?")]
        public bool LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement { get; set; }

        [DataMember]
        [Display(Name = "Consider investments balance for income-based loan appraisal?")]
        public bool LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal { get; set; }

        [DataMember]
        [Display(Name = "Throttle scheduled arrears recovery?")]
        public bool LoanRegistrationThrottleScheduledArrearsRecovery { get; set; }

        [DataMember]
        [Display(Name = "Create standing order on loan verification?")]
        public bool LoanRegistrationCreateStandingOrderOnLoanAudit { get; set; }

        [DataMember]
        [Display(Name = "Take-Home Type")]
        public int TakeHomeType { get; set; }

        [DataMember]
        [Display(Name = "Take-Home Type")]
        public string TakeHomeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), TakeHomeType) ? EnumHelper.GetDescription((ChargeType)TakeHomeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Take-Home Percentage")]
        public double TakeHomePercentage { get; set; }

        [DataMember]
        [Display(Name = "Take-Home Fixed Amount")]
        public decimal TakeHomeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Recovery Priority")]
        public int Priority { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Name")]
        public string ChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Principal G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account")]
        [ValidGuid]
        public Guid InterestReceivedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Type")]
        public int InterestReceivedChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Code")]
        public int InterestReceivedChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Name")]
        public string InterestReceivedChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Name")]
        public string InterestReceivedChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", InterestReceivedChartOfAccountAccountType.FirstDigit(), InterestReceivedChartOfAccountAccountCode, InterestReceivedChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Cost Center")]
        public Guid? InterestReceivedChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Interest Received G/L Account Cost Center")]
        public string InterestReceivedChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account")]
        [ValidGuid]
        public Guid InterestReceivableChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Type")]
        public int InterestReceivableChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Code")]
        public int InterestReceivableChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Name")]
        public string InterestReceivableChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Name")]
        public string InterestReceivableChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", InterestReceivableChartOfAccountAccountType.FirstDigit(), InterestReceivableChartOfAccountAccountCode, InterestReceivableChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Cost Center")]
        public Guid? InterestReceivableChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Interest Receivable G/L Account Cost Center")]
        public string InterestReceivableChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account")]
        public Guid InterestChargedChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Type")]
        public int InterestChargedChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Code")]
        public int InterestChargedChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Name")]
        public string InterestChargedChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Name")]
        public string InterestChargedChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", InterestChargedChartOfAccountAccountType.FirstDigit(), InterestChargedChartOfAccountAccountCode, InterestChargedChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Cost Center")]
        public Guid? InterestChargedChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Interest Charged G/L Account Cost Center")]
        public string InterestChargedChartOfAccountCostCenterDescription { get; set; }

        public static ValidationResult ValidateTermVsPaymentFrequencyPerYear(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanProductDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanProductDTO");

            bool valid = false;

            switch ((PaymentFrequencyPerYear)bindingModel.LoanRegistrationPaymentFrequencyPerYear)
            {
                case PaymentFrequencyPerYear.SemiAnnual:
                    // TermInMonths Bust Be A Whole Number Divisible By 6
                    valid = (bindingModel.LoanRegistrationTermInMonths % 6d) == 0;
                    break;
                case PaymentFrequencyPerYear.TriAnnual:
                    // TermInMonths Bust Be A Whole Number Divisible By 4
                    valid = (bindingModel.LoanRegistrationTermInMonths % 4d) == 0;
                    break;
                case PaymentFrequencyPerYear.Quarterly:
                    // TermInMonths Bust Be A Whole Number Divisible By 3
                    valid = (bindingModel.LoanRegistrationTermInMonths % 3d) == 0;
                    break;
                case PaymentFrequencyPerYear.BiMonthly:
                case PaymentFrequencyPerYear.SemiMonthly:
                case PaymentFrequencyPerYear.BiWeekly:
                    // TermInMonths * 2 Bust Be A Whole Number Divisible By 2
                    valid = ((bindingModel.LoanRegistrationTermInMonths * 2) % 2d) == 0;
                    break;
                case PaymentFrequencyPerYear.Annual:
                    // TermInMonths Bust Be A Whole Number Divisible By 12
                    valid = (bindingModel.LoanRegistrationTermInMonths % 12d) == 0;
                    break;
                case PaymentFrequencyPerYear.Weekly:
                case PaymentFrequencyPerYear.Daily:
                case PaymentFrequencyPerYear.Monthly:
                default:
                    // No Need For Validation
                    valid = true;
                    break;
            }

            if (!valid)
                return new ValidationResult("LoanRegistrationTermInMonths Not Valid!");

            return ValidationResult.Success;
        }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public LoanProductAuxilliaryAppraisalFactorDTO AuxilliaryAppraisalFactor { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<LoanProductAuxilliaryAppraisalFactorDTO> Tiers { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<LoanProductAuxiliaryConditionDTO> AuxiliaryConditions { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<LoanProductDeductibleDTO> Deductiles { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<LoanCycleDTO> Cycles { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<DynamicChargeDTO> charges { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ProductCollectionInfo productCollection { get; set; }

        [JsonIgnore] // BREAK CIRCULAR REFERENCE
        [DataMember]
        public ObservableCollection<CommissionDTO> commissions { get; set; }
    }
}
