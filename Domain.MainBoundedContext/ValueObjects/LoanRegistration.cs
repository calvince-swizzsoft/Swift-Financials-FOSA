using Domain.Seedwork;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class LoanRegistration : ValueObject<LoanRegistration>
    {
        public short TermInMonths { get; private set; }

        public decimal MinimumAmount { get; private set; }

        public decimal MaximumAmount { get; private set; }

        public decimal MinimumInterestAmount { get; private set; }

        public byte LoanProductSection { get; private set; }

        public byte LoanProductCategory { get; private set; }

        public short ConsecutiveIncome { get; private set; }

        public double InvestmentsMultiplier { get; private set; }

        public short MinimumGuarantors { get; private set; }

        public short MaximumGuarantees { get; private set; }

        public bool RejectIfMemberHasBalance { get; private set; }

        public bool SecurityRequired { get; private set; }

        public bool AllowSelfGuarantee { get; private set; }

        public short GracePeriod { get; private set; }

        public short MinimumMembershipPeriod { get; private set; }

        public short PaymentFrequencyPerYear { get; private set; }

        public byte PaymentDueDate { get; private set; }

        public short PayoutRecoveryMode { get; private set; }

        public double PayoutRecoveryPercentage { get; private set; }

        public byte AggregateCheckOffRecoveryMode { get; private set; }

        public bool ChargeClearanceFee { get; private set; }

        public bool Microcredit { get; private set; }

        public byte StandingOrderTrigger { get; private set; }

        public bool TrackArrears { get; private set; }

        public bool ChargeArrearsFee { get; private set; }

        public bool EnforceSystemAppraisalRecommendation { get; private set; }

        public bool BypassAudit { get; private set; }

        public double MaximumSelfGuaranteeEligiblePercentage { get; private set; }

        public byte GuarantorSecurityMode { get; private set; }

        public byte RoundingType { get; private set; }

        public bool DisburseMicroLoanLessDeductions { get; private set; }

        public bool ExcludeOutstandingLoansOnMaximumEntitlement { get; private set; }

        public bool ConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal { get; private set; }

        public bool ThrottleScheduledArrearsRecovery { get; private set; }

        public bool CreateStandingOrderOnLoanAudit { get; private set; }

        public LoanRegistration(int termInMonths, decimal minimumAmount, decimal maximumAmount, decimal minimumInterestAmount, int loanProductSection, int loanProductCategory, int consecutiveIncome, double investmentsMultiplier, int minimumGuarantors, int maximumGuarantees, bool rejectIfMemberHasBalance, bool securityRequired, bool allowSelfGuarantee, int gracePeriod, int minimumMembershipPeriod, int paymentFrequencyPerYear, int paymentDueDate, int payoutRecoveryMode, double payoutRecoveryPercentage, int aggregateCheckOffRecoveryMode, bool chargeClearanceFee, bool microcredit, int standingOrderTrigger, bool trackArrears, bool chargeArrearsFee, bool enforceSystemAppraisalRecommendation, bool bypassAudit, double maximumSelfGuaranteeEligiblePercentage, int guarantorSecurityMode, int roundingType, bool disburseMicroLoanLessDeductions, bool excludeOutstandingLoansOnMaximumEntitlement, bool considerInvestmentsBalanceForIncomeBasedLoanAppraisal, bool throttleScheduledArrearsRecovery, bool createStandingOrderOnLoanAudit)
        {
            this.TermInMonths = (short)termInMonths;
            this.MinimumAmount = minimumAmount;
            this.MaximumAmount = maximumAmount;
            this.MinimumInterestAmount = minimumInterestAmount;
            this.LoanProductSection = (byte)loanProductSection;
            this.LoanProductCategory = (byte)loanProductCategory;
            this.ConsecutiveIncome = (short)consecutiveIncome;
            this.InvestmentsMultiplier = investmentsMultiplier;
            this.MinimumGuarantors = (short)minimumGuarantors;
            this.MaximumGuarantees = (short)maximumGuarantees;
            this.RejectIfMemberHasBalance = rejectIfMemberHasBalance;
            this.SecurityRequired = securityRequired;
            this.AllowSelfGuarantee = allowSelfGuarantee;
            this.GracePeriod = (short)gracePeriod;
            this.MinimumMembershipPeriod = (short)minimumMembershipPeriod;
            this.PaymentFrequencyPerYear = (short)paymentFrequencyPerYear;
            this.PaymentDueDate = (byte)paymentDueDate;
            this.PayoutRecoveryMode = (short)payoutRecoveryMode;
            this.PayoutRecoveryPercentage = payoutRecoveryPercentage;
            this.AggregateCheckOffRecoveryMode = (byte)aggregateCheckOffRecoveryMode;
            this.ChargeClearanceFee = chargeClearanceFee;
            this.Microcredit = microcredit;
            this.StandingOrderTrigger = (byte)standingOrderTrigger;
            this.TrackArrears = trackArrears;
            this.ChargeArrearsFee = chargeArrearsFee;
            this.EnforceSystemAppraisalRecommendation = enforceSystemAppraisalRecommendation;
            this.BypassAudit = bypassAudit;
            this.MaximumSelfGuaranteeEligiblePercentage = maximumSelfGuaranteeEligiblePercentage;
            this.GuarantorSecurityMode = loanProductSection == (int)Infrastructure.Crosscutting.Framework.Utils.LoanProductSection.FOSA ? (byte)guarantorSecurityMode : (byte)Infrastructure.Crosscutting.Framework.Utils.GuarantorSecurityMode.Investments;
            this.RoundingType = (byte)roundingType;
            this.DisburseMicroLoanLessDeductions = disburseMicroLoanLessDeductions;
            this.ExcludeOutstandingLoansOnMaximumEntitlement = excludeOutstandingLoansOnMaximumEntitlement;
            this.ConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal = considerInvestmentsBalanceForIncomeBasedLoanAppraisal;
            this.ThrottleScheduledArrearsRecovery = throttleScheduledArrearsRecovery;
            this.CreateStandingOrderOnLoanAudit = createStandingOrderOnLoanAudit;
        }

        private LoanRegistration()
        {

        }
    }
}
