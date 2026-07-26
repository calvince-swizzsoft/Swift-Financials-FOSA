using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LoanProductBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public Guid InterestReceivedChartOfAccountId { get; set; }

        public Guid InterestReceivableChartOfAccountId { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public double LoanInterest_AnnualPercentageRate { get; set; }

        public int LoanInterest_ChargeMode { get; set; }

        public int LoanInterest_RecoveryMode { get; set; }

        public int LoanInterest_CalculationMode { get; set; }

        public int LoanRegistration_TermInMonths { get; set; }

        public decimal LoanRegistration_MinimumAmount { get; set; }

        public decimal LoanRegistration_MaximumAmount { get; set; }

        public decimal LoanRegistration_MinimumInterestAmount { get; set; }

        public int LoanRegistration_LoanProductSection { get; set; }

        public int LoanRegistration_AppraisalFactor { get; set; }

        public int LoanRegistration_MinimumGuarantors { get; set; }

        public int LoanRegistration_MaximumGuarantees { get; set; }

        public bool LoanRegistration_RejectIfMemberHasBalance { get; set; }

        public bool LoanRegistration_SecurityRequired { get; set; }

        public bool LoanRegistration_AllowSelfGuarantee { get; set; }

        public int LoanRegistration_GracePeriod { get; set; }

        public int LoanRegistration_MinimumMembershipPeriod { get; set; }

        public int LoanRegistration_PaymentFrequencyPerYear { get; set; }

        public int LoanRegistration_PaymentDueDate { get; set; }

        public int LoanRegistration_PayoutRecoveryMode { get; set; }

        public double LoanRegistration_PayoutRecoveryPercentage { get; set; }

        public int LoanRegistration_AggregateCheckOffRecoveryMode { get; set; }

        public bool LoanRegistration_ChargeClearanceFee { get; set; }

        public bool LoanRegistration_Microcredit { get; set; }

        public int LoanRegistration_StandingOrderTrigger { get; set; }

        public bool LoanRegistration_TrackArrears { get; set; }

        public bool LoanRegistration_ChargeArrearsFee { get; set; }

        public bool LoanRegistration_EnforceSystemAppraisalRecommendation { get; set; }

        public bool LoanRegistration_BypassAudit { get; set; }

        public double LoanRegistration_MaximumSelfGuaranteeEligiblePercentage { get; set; }

        public int LoanRegistration_GuarantorSecurityMode { get; set; }

        public int LoanRegistration_RoundingType { get; set; }

        public int TakeHome_Type { get; set; }

        public double TakeHome_Percentage { get; set; }

        public decimal TakeHome_FixedAmount { get; set; }

        public int Priority { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
