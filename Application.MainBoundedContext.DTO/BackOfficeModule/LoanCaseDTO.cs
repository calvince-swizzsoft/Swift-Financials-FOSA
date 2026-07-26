using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanCaseDTO : BindingModelBase<LoanCaseDTO>
    {
        public LoanCaseDTO()
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
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string BranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid BranchCompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        public string BranchCompanyAddressCity { get; set; }

        [DataMember]
        public string BranchCompanyAddressStreet { get; set; }

        [DataMember]
        public string BranchCompanyAddressEmail { get; set; }

        [DataMember]
        public string BranchCompanyAddressLandLine { get; set; }

        [DataMember]
        public string BranchCompanyAddressMobileLine { get; set; }

        [DataMember]
        public bool BranchCompanyEnforceBudgetControl { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")] // Personal Identification Number
        public string CustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [DataMember]
        [Display(Name = "Loanee")]
        public string CustomerLoaneeFullName { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Number")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Date of Birth")]
        public DateTime? CustomerIndividualBirthDate { get; set; }

        [DataMember]
        [Display(Name = "Age")]
        public int CustomerAge
        {
            get
            {
                var result = -1;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        if (CustomerIndividualBirthDate.HasValue && CustomerIndividualBirthDate.Value <= DateTime.Today)
                            result = UberUtil.GetAge(CustomerIndividualBirthDate.Value);
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        if (CustomerNonIndividualDateEstablished.HasValue && CustomerNonIndividualDateEstablished.Value <= DateTime.Today)
                            result = UberUtil.GetAge(CustomerNonIndividualDateEstablished.Value);
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        public int CustomerStationZoneDivisionEmployerRetirementAge { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string CustomerStationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string GuarantorCustomerStationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string CustomerStation { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string GuarantorCustomerStation { get; set; }

        [DataMember]
        public bool CustomerStationZoneDivisionEmployerEnforceRetirementAge { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        [ValidGuid]
        public Guid LoanProductId { get; set; }

        [DataMember]
        [Display(Name = "Loan Product")]
        public string LoanProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        [ValidGuid]
        public Guid? LoanPurposeId { get; set; }

        [DataMember]
        [Display(Name = "Loan Purpose")]
        public string LoanPurposeDescription { get; set; }

        [DataMember]
        [Display(Name = "Savings Product")]
        [ValidGuid]
        public Guid? SavingsProductId { get; set; }

        [DataMember]
        [Display(Name = "Savings Product Code")]
        public int SavingsProductCode { get; set; }

        [Display(Name = "Savings Product")]
        public string SavingsProductDescription { get; set; }

        [Display(Name = "Savings Product G/L Account")]
        public Guid SavingsProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Registration Remarks")]
        public Guid RegistrationRemarkId { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public int CaseNumber { get; set; }

        [DataMember]
        [Display(Name = "Loan Number")]
        public string PaddedCaseNumber
        {
            get
            {
                return string.Format("{0}", CaseNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Amount Applied")]
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Amount applied must be greater than zero!")]
        public decimal AmountApplied { get; set; }

        [DataMember]
        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        [DataMember]
        [Display(Name = "Appraised By")]
        public string AppraisedBy { get; set; }

        [DataMember]
        [Display(Name = "Appraised Date")]
        public DateTime? AppraisedDate { get; set; }

        [DataMember]
        [Display(Name = "System Appraisal Remarks")]
        public string SystemAppraisalRemarks { get; set; }

        [DataMember]
        [Display(Name = "System Appraised Amount")]
        public decimal SystemAppraisedAmount { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Remarks")]
        public string AppraisalRemarks { get; set; }

        [DataMember]
        [Display(Name = "Appraised Amount")]
        public decimal AppraisedAmount { get; set; }

        [DataMember]
        [Display(Name = "Appraised Amount Remarks")]
        public string AppraisedAmountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Appraised Net Income")]
        public decimal AppraisedNetIncome { get; set; }

        [DataMember]
        [Display(Name = "Appraised Ability")]
        public decimal AppraisedAbility { get; set; }

        [DataMember]
        [Display(Name = "Approved Amount")]
        public decimal ApprovedAmount { get; set; }

        [DataMember]
        [Display(Name = "Approved Amount Remarks")]
        public string ApprovedAmountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Approved Standing Order Principal")]
        public decimal ApprovedPrincipalPayment { get; set; }

        [DataMember]
        [Display(Name = "Approved Standing Order Interest")]
        public decimal ApprovedInterestPayment { get; set; }

        [DataMember]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [DataMember]
        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [DataMember]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verified Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Verification Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Top-Up Amount")]
        public decimal AuditTopUpAmount { get; set; }

        [DataMember]
        [Display(Name = "Cancelled By")]
        public string CancelledBy { get; set; }

        [DataMember]
        [Display(Name = "Cancellation Date")]
        public DateTime? CancelledDate { get; set; }

        [DataMember]
        [Display(Name = "Is Batched?")]
        public bool IsBatched { get; set; }

        [DataMember]
        [Display(Name = "Is Batched?")]
        public string IsBatchedDescription
        {
            get
            {
                return IsBatched ? "Yes" : "No";
            }
        }

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
        [Display(Name = "Batched By")]
        public string BatchedBy { get; set; }

        [DataMember]
        [Display(Name = "Disbursement Remarks")]
        public string DisbursementRemarks { get; set; }

        [DataMember]
        [Display(Name = "Disbursed By")]
        public string DisbursedBy { get; set; }

        [DataMember]
        [Display(Name = "Disbursed Date")]
        public DateTime? DisbursedDate { get; set; }

        [DataMember]
        [Display(Name = "Disbursed Amount")]
        public decimal DisbursedAmount { get; set; }

        [DataMember]
        [Display(Name = "Monthly Payback Amount")]
        public decimal MonthlyPaybackAmount { get; set; }

        [DataMember]
        [Display(Name = "Total Payback Amount")]
        public decimal TotalPaybackAmount { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanCaseStatus), Status) ? EnumHelper.GetDescription((LoanCaseStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Verification Action")]
        public int LoanAuditOption { get; set; }

        [DataMember]
        [Display(Name = "Verification Action")]
        public string LoanAuditOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanAuditOption), LoanAuditOption) ? EnumHelper.GetDescription((LoanAuditOption)LoanAuditOption) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Loan Appraisal Option")]
        public int LoanAppraisalOption { get; set; }

        [DataMember]
        [Display(Name = "Loan Appraisal Option")]
        public string LoanAppraisalOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanAppraisalOption), LoanAppraisalOption) ? EnumHelper.GetDescription((LoanAppraisalOption)LoanAppraisalOption) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Loan Cancellation Option")]
        public int LoanCancellationOption { get; set; }

        [DataMember]
        [Display(Name = "Loan Cancellation Option")]
        public string LoanCancellationOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanCancellationOption), LoanCancellationOption) ? EnumHelper.GetDescription((LoanCancellationOption)LoanCancellationOption) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Investments Balance")]
        public decimal LoanProductInvestmentsBalance { get; set; }

        [DataMember]
        [Display(Name = "Total Shares")]
        public decimal LoanProductTotalSharesInvestmentsBalance { get; set; }

        [DataMember]
        [Display(Name = "Committed Shares")]
        public decimal LoanProductCommittedSharesInvestmentsBalance { get; set; }

        [DataMember]
        [Display(Name = "Loan Balance")]
        public decimal LoanProductLoanBalance { get; set; }

        [DataMember]
        [Display(Name = "BOSA Loans Balance")]
        public decimal TotalLoansBalance { get; set; }

        [DataMember]
        [Display(Name = "Attached Loans Balance")]
        public decimal TotalAttachedLoansBalance { get; set; }

        [DataMember]
        [Display(Name = "Latest Income")]
        public decimal LoanProductLatestIncome { get; set; }

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
        [Display(Name = "Loan Approval Option")]
        public int LoanApprovalOption { get; set; }

        [DataMember]
        [Display(Name = "Loan Approval Option")]
        public string LoanApprovalOptionDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanApprovalOption), LoanApprovalOption) ? EnumHelper.GetDescription((LoanApprovalOption)LoanApprovalOption) : string.Empty;
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
        [RegularExpression(@"^\s*(?=.*[1-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Term in months must be greater than zero!")]
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
        public int LoanRegistrationMaximumGuarantees { get; set; }

        [DataMember]
        [Display(Name = "Reject if owing?")]
        public bool LoanRegistrationRejectIfMemberHasBalance { get; set; }

        [DataMember]
        [Display(Name = "Security is required?")]
        public bool LoanRegistrationSecurityRequired { get; set; }

        [DataMember]
        [Display(Name = "Allow self-guarantee?")]
        public bool LoanRegistrationAllowSelfGuarantee { get; set; }

        [DataMember]
        [Display(Name = "Grace Period")]
        public int LoanRegistrationGracePeriod { get; set; }

        [DataMember]
        [Display(Name = "Minimum Membership Period")]
        public int LoanRegistrationMinimumMembershipPeriod { get; set; }

        [DataMember]
        [Display(Name = "Payment Frequency Per Year")]
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
        [Display(Name = "Maximum Amount Percentage")]
        public double MaximumAmountPercentage { get; set; }

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
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Total Number of Guarantors")]
        public int TotalNumberOfGuarantors { get; set; }

        [Display(Name = "Amount Guaranteed")]
        [CustomValidation(typeof(LoanCaseDTO), "ValidateLoanSecurity", ErrorMessage = "Security is required for the selected loan product and following conditions must be met:-\n\n-If guarantor security mode is income, the total number of guarantors must not be less than the minimum required\n-If guarantor security mode is investments, the total amount guaranteed must not be less than the amount applied")]
        public decimal TotalAmountGuaranteed { get; set; }

        [Display(Name = "Amount Pledged")]
        public decimal TotalAmountPledged { get; set; }

        [Display(Name = "Total Collateral")]
        public decimal TotalCollateralAmount { get; set; }

        [Display(Name = "Total Number of Consecutive Incomes")]
        [CustomValidation(typeof(LoanCaseDTO), "ValidateConsecutiveIncome", ErrorMessage = "The number of consecutive incomes falls short of the minimum required!")]
        public int TotalNumberOfIncomes { get; set; }

        [DataMember]
        [Display(Name = "Amount Applied Range Validation")]
        [CustomValidation(typeof(LoanCaseDTO), "ValidateAmountApplied", ErrorMessage = "Amount applied is out of range for the selected loan product!")]
        public string RangeValidation { get; set; }

        [DataMember]
        [Display(Name = "Retirement Age Validation")]
        [CustomValidation(typeof(LoanCaseDTO), "ValidateRetirementAge", ErrorMessage = "Retirement age restriction will be violated!")]
        public string RetirementAgeValidation { get; set; }

        [DataMember]
        [Display(Name = "Loan Cycle Range (Lower Limit)")]
        public decimal LoanCycleRangeLowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Loan Cycle Range (Upper Limit)")]
        public decimal LoanCycleRangeUpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Branch Budget Balance Validation")]
        [CustomValidation(typeof(LoanCaseDTO), "ValidateBudgetBalance", ErrorMessage = "Amount applied will exceed the branch budget balance for the selected loan product!")]
        public decimal BranchBudgetBalance { get; set; }

        public static ValidationResult ValidateBudgetBalance(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanCaseDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanCaseDTO");

            if (bindingModel.BranchCompanyEnforceBudgetControl && bindingModel.AmountApplied > bindingModel.BranchBudgetBalance)
            {
                return new ValidationResult("Amount applied will exceed the branch budget balance!");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateAmountApplied(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanCaseDTO;

            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanCaseDTO");

            if (bindingModel.LoanRegistrationMicrocredit)
            {
                if (bindingModel.AmountApplied > bindingModel.LoanCycleRangeUpperLimit)
                    return new ValidationResult("Amount applied is out of range!");
            }
            else if ((bindingModel.AmountApplied < bindingModel.LoanRegistrationMinimumAmount || bindingModel.AmountApplied > bindingModel.LoanRegistrationMaximumAmount))
            {
                return new ValidationResult("Amount applied is out of range!");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateLoanSecurity(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanCaseDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanCaseDTO");

            if (!bindingModel.LoanRegistrationMicrocredit && bindingModel.LoanRegistrationSecurityRequired)
            {
                if (bindingModel.LoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Income && (bindingModel.TotalNumberOfGuarantors < bindingModel.LoanRegistrationMinimumGuarantors))
                    return new ValidationResult("The total number of guarantors is less than the minimum required!");
                else if (bindingModel.LoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Investments && ((bindingModel.TotalAmountGuaranteed + bindingModel.TotalCollateralAmount) < bindingModel.AmountApplied))
                    return new ValidationResult("The total amount guaranteed is less than amount applied!");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateConsecutiveIncome(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanCaseDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanCaseDTO");

            if (!bindingModel.LoanRegistrationMicrocredit && bindingModel.LoanRegistrationLoanProductSection == (int)LoanProductSection.FOSA && (bindingModel.TotalNumberOfIncomes < bindingModel.LoanRegistrationConsecutiveIncome))
                return new ValidationResult("TotalNumberOfIncomes Specification Not Satisfied!");

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateRetirementAge(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LoanCaseDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LoanCaseDTO");

            if (bindingModel.CustomerStationZoneDivisionEmployerEnforceRetirementAge && bindingModel.CustomerAge <= 0)
                return new ValidationResult("RetirementAge Specification Not Satisfied!");
            else
            {
                var ageAtLoanTermCompletion = bindingModel.CustomerAge;

                switch ((CustomerType)bindingModel.CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        if (bindingModel.CustomerIndividualBirthDate.HasValue)
                            ageAtLoanTermCompletion = UberUtil.GetAge(bindingModel.CustomerIndividualBirthDate.Value.AddMonths(bindingModel.LoanRegistrationTermInMonths * -1));
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        if (bindingModel.CustomerNonIndividualDateEstablished.HasValue)
                            ageAtLoanTermCompletion = UberUtil.GetAge(bindingModel.CustomerNonIndividualDateEstablished.Value.AddMonths(bindingModel.LoanRegistrationTermInMonths * -1));
                        break;
                    default:
                        break;
                }

                if (bindingModel.CustomerStationZoneDivisionEmployerEnforceRetirementAge && ageAtLoanTermCompletion <= 0)
                    return new ValidationResult("RetirementAge Specification Not Satisfied!");
                else if (bindingModel.CustomerStationZoneDivisionEmployerEnforceRetirementAge && (ageAtLoanTermCompletion > bindingModel.CustomerStationZoneDivisionEmployerRetirementAge))
                    return new ValidationResult("RetirementAge Specification Not Satisfied!");

                return ValidationResult.Success;
            }
        }


        [DataMember]
        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; }

        public IList<LoanGuarantorDTO> LoanGuarantors { get; set; }

        public List<EmployerDTO> Employers { get; set; }

        public EmployerDTO Employer { get; set; }


        public List<StationDTO> Stations { get; set; }

        public StationDTO Station { get; set; }



        public ObservableCollection<CustomerAccountDTO> CustomerAccountsModel { get; set; }

        [DataMember]
        public LoanGuarantorDTO LoanGuarantor { get; set; }

        [DataMember]
        public CustomerDTO Customer { get; set; }




        // Additional Guarantor DTOs
        [DataMember]
        [Display(Name = "Customer")]
        public Guid GuarantorId { get; set; }

        [DataMember]
        [Display(Name = "First Name")]
        public string GuarantorIndividualFirstName { get; set; }
        
        [DataMember]
        [Display(Name = "Type")]
        public string GuarantorTypeDescription { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string GuarantorRemarks { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string GuarantorIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Substitute Guarantor")]
        public string GuarantorName { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public Guid GuarantorStationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string GuarantorStationDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid GuarantorEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string GuarantorEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Identification Number")]
        public string GuarantorIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string GuarantorReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string GuarantorReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string GuarantorReference3 { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Factor")]
        public int AppraisalFactor { get; set; }

        [DataMember]
        [Display(Name = "Total Shares")]
        public decimal GuarantorTotalshares { get; set; }

        [DataMember]
        [Display(Name = "Committed Shares")]
        public decimal GuarantorCommittedShares { get; set; }

        [DataMember]
        [Display(Name = "Amount Guaranteed")]
        public decimal GuarantorAmountGuaranteed { get; set; }

        [DataMember]
        [Display(Name = "Interest Calculation Mode")]
        public string InterestCalculationModeDescription { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public int LoanProductSectionSection { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string LoanProductSectionDescription { get; set; }


        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullAccountNumber { get; set; }


        [DataMember]
        public CustomerAccountDTO CustomerAccountDTOModel { get; set; }


        // Additional Customer Accounts DTOs
        [DataMember]
        [Display(Name = "Customer Account")]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string CustomerAccountFullNumber { get; set; }


        // Loan Case Filter
        [DataMember]
        [Display(Name = "Loan Case Filter")]
        public int filterText { get; set; }


        [DataMember]
        [Display(Name = "Loan Case Filter")]
        public string filterTextDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanCaseFilter), filterText) ? EnumHelper.GetDescription((LoanCaseFilter)filterText) : string.Empty;
            }
        }

        [DataMember]
        public string ErrorMessageResult { get; set; }


        // Additional Fields for Loan Qualification Section in Loan Appraisal
        [DataMember]
        [Display(Name = "Maximum Loan")]
        public decimal LoanRegistrationMaximumLoan { get; set; }

        [DataMember]
        [Display(Name = "Outstanding Loans Balance")]
        public decimal LoanRegistrationOutstandingLoansBalance { get; set; }

        [DataMember]
        [Display(Name = "Maxmimum Entitled")]
        public decimal LoanRegistrationMaximumEntitled { get; set; }

        [DataMember]
        [Display(Name = "Net Income")]
        public decimal LoanRegistrationNetIncome { get; set; }

        [DataMember]
        [Display(Name = "Total Allowance")]
        public decimal LoanRegistrationTotalAllowance { get; set; }

        [DataMember]
        [Display(Name = "Total Deduction")]
        public decimal LoanRegistrationTotalDeduction { get; set; }

        [DataMember]
        [Display(Name = "Total Income")]
        public decimal LoanRegistrationTotalIncome { get; set; }


        [DataMember]
        [Display(Name = "Ability to Pay")]
        public decimal LoanRegistrationAbilityToPay { get; set; }

        [DataMember]
        [Display(Name = "Ability to Pay Over Loan Term")]
        public decimal LoanRegistrationAbilityToPayOverLoanTerm { get; set; }

        [DataMember]
        [Display(Name = "Loan + Interest")]
        public decimal LoanRegistrationLoanPlusInterest { get; set; }

        [DataMember]
        [Display(Name = "Loan Part")]
        public decimal LoanRegistrationLoanPart { get; set; }

        [DataMember]
        [Display(Name = "Interest Part")]
        public decimal LoanRegistrationInterestPart { get; set; }

        [DataMember]
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Principal Balance")]
        public decimal PrincipalBalance { get; set; }

        [DataMember]
        [Display(Name = "Interest Balance")]
        public decimal InterestBalance { get; set; }

        [DataMember]
        [Display(Name = "Payment Per Period")]
        public double PaymentPerPeriod { get; set; }

        [DataMember]
        [Display(Name = "Number Of Periods")]
        public int NumberOfPeriods { get; set; }



        // Additional DTOs
        [DataMember]
        public List<LoanAppraisalFactorDTO> incomeAdjustmentDTO { get; set; }


        // Reports DTOs
        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }



        // 
        [DataMember]
        [Display(Name = "Security Qualification")]
        public double LoanQualificationSecurityQualification { get; set; }

        [DataMember]
        [Display(Name = "System Recommendation")]
        public double LoanQualificationSystemRecommendation { get; set; }

        [DataMember]
        [Display(Name = "Income Qualification")]
        public double LoanQualificationIncomeQualification { get; set; }

        [DataMember]
        [Display(Name = "Investments Qualification")]
        public double LoanQualificationInvestmentsQualification { get; set; }

        [DataMember]
        [Display(Name = "Attached Loans Balance")]
        public double LoanQualificationAttachedLoansBalance { get; set; }

        [DataMember]
        [Display(Name = "Total Loans + Interest")]
        public double LoanQualificationTotalLoansPlusInterest { get; set; }

        [DataMember]
        [Display(Name = "Loan Amount")]
        public double LoanQualificationLoanAmount { get; set; }



        [DataMember]
        [Display(Name = "Standing Order Principal")]
        public double StandingOrderPrincipal { get; set; }

        [DataMember]
        [Display(Name = "Standing Order Interest")]
        public double StandingOrderInterest { get; set; }


        [DataMember]
        public ObservableCollection<LoanGuarantorDTO> LoanGuarantorDTO { get; set; }





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

        [Display(Name = "Customer Filter")]
        public int CustomerFilter { get; set; }

        [Display(Name = "Customer Filter")]
        public string CustomerFilterDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerFilter)CustomerFilter);
            }
        }
        public Guid DocumentID { get; set; }  // Unique identifier for the document

        public byte[] PassportPhoto { get; set; }
        public byte[] SignaturePhoto { get; set; }
        public byte[] IDCardFrontPhoto { get; set; }
        public byte[] IDCardBackPhoto { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string LoanStatus { get; set; }


        //Additional DTOs
        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber { get; set; }

        [DataMember]
        public LoanProductDTO LoanProductsDTO { get; set; }

        [DataMember]
        [Display(Name = "Section")]
        public string loanProductSection { get; set; }
        
        [DataMember]
        [Display(Name = "Payment Frequency Per Year")]
        public string loanProductPaymentFrequencyPerYear { get; set; }

        [DataMember]
        [Display(Name = "Product Category")]
        public string LoanProductCategory { get; set; }

        public IList<LoanGuarantorDTO> Guarantors { get; set; }

        [DataMember]
        public ObservableCollection<LoanGuarantorDTO> Guarantor { get; set; }
        public int LoanProductCode { get; set; }
        public Guid BankAccountId { get; set; }
    }
}
