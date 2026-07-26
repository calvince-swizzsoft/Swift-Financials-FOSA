using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanCaseBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid BranchId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid LoanProductId { get; set; }

        public Guid LoanPurposeId { get; set; }

        public int CaseNumber { get; set; }

        public string Remarks { get; set; }

        public decimal AmountApplied { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string AppraisedBy { get; set; }

        public DateTime? AppraisedDate { get; set; }

        public string SystemAppraisalRemarks { get; set; }

        public decimal SystemAppraisedAmount { get; set; }

        public string AppraisalRemarks { get; set; }

        public decimal AppraisedAmount { get; set; }

        public string AppraisedAmountRemarks { get; set; }

        public decimal AppraisedNetIncome { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovalRemarks { get; set; }

        public string AuditedBy { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuditRemarks { get; set; }

        public decimal AuditTopUpAmount { get; set; }

        public int DisburseBatchNumber { get; set; }

        public string DisbursedBy { get; set; }

        public DateTime? DisbursedDate { get; set; }

        public decimal DisbursedAmount { get; set; }

        public decimal MonthlyPaybackAmount { get; set; }

        public decimal TotalPaybackAmount { get; set; }

        public int Status { get; set; }

        public decimal LoanProductInvestmentsBalance { get; set; }

        public decimal LoanProductLoanBalance { get; set; }

        public decimal TotalLoansBalance { get; set; }

        public decimal LoanProductLatestIncome { get; set; }

        public double LoanInterest_AnnualPercentageRate { get; set; }

        public int LoanInterest_ChargeMode { get; set; }

        public int LoanInterest_RecoveryMode { get; set; }

        public int LoanInterest_CalculationMode { get; set; }

        public int LoanRegistration_TermInMonths { get; set; }

        public decimal LoanRegistration_MinimumAmount { get; set; }

        public decimal LoanRegistration_MaximumAmount { get; set; }

        public decimal LoanRegistration_MinimumInterestAmount { get; set; }

        public int LoanRegistration_LoanProductSection { get; set; }

        public short LoanRegistration_ConsecutiveIncome { get; set; }

        public double LoanRegistration_InvestmentsMultiplier { get; set; }

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

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
