using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg
{
    public class Company : Entity
    {
        public string Description { get; set; }

        public string Vision { get; set; }

        public string Mission { get; set; }

        public string Motto { get; set; }

        public string RegistrationNumber { get; set; }

        public string PersonalIdentificationNumber { get; set; }

        public string ApplicationDisplayName { get; set; }

        public string RecoveryPriority { get; set; }

        public virtual Address Address { get; set; }

        public virtual Image Image { get; set; }

        public virtual TimeDuration TimeDuration { get; set; }

        public byte TransactionReceiptTopIndentation { get; set; }

        public byte TransactionReceiptLeftIndentation { get; set; }

        public string TransactionReceiptFooter { get; set; }

        public int FingerprintBiometricThreshold { get; set; }

        public short MembershipTerminationNoticePeriod { get; set; }

        public bool ApplicationMembershipTextAlertsEnabled { get; set; }

        public bool EnforceCustomerAccountMakerChecker { get; set; }

        public bool BypassJournalVoucherAudit { get; set; }

        public bool BypassCreditBatchAudit { get; set; }

        public bool BypassDebitBatchAudit { get; set; }

        public bool BypassRefundBatchAudit { get; set; }

        public bool BypassWireTransferBatchAudit { get; set; }

        public bool BypassLoanDisbursementBatchAudit { get; set; }

        public bool BypassJournalReversalBatchAudit { get; set; }

        public bool BypassInterAccountTransferBatchAudit { get; set; }

        public bool BypassExpensePayableAudit { get; set; }

        public bool BypassGeneralLedgerAudit { get; set; }

        public bool ExcludeChargesInTransactionReceipt { get; set; }

        public bool ExcludeChequeMaturityDateInTransactionReceipt { get; set; }

        public bool TrackGuarantorCommittedInvestments { get; set; }

        public bool TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement { get; set; }

        public bool ReceiveLoanRequestBeforeLoanRegistration { get; set; }

        public bool IsWithholdingTaxAgent { get; set; }

        public bool LocalizeOnlineNotifications { get; set; }

        public bool IsFileTrackingEnforced { get; private set; }

        public bool EnforceBudgetControl { get; set; }

        public bool IsLocked { get; private set; }

        public bool ExcludeCustomerAccountBalanceInTransactionReceipt { get; set; }

        public bool EnforceFixedDepositBands { get; set; }

        public bool EnforceBiometricsForCashWithdrawal { get; set; }

        public bool EnforceTwoFactorAuthentication { get; set; }

        public bool RecoverArrearsOnCashDeposit { get; set; }

        public bool RecoverArrearsOnExternalChequeClearance { get; set; }

        public bool RecoverArrearsOnFixedDepositPayment { get; set; }

        public bool AllowDebitBatchToOverdrawAccount { get; set; }

        public bool EnforceSystemLock { get; set; }

        public bool EnforceTellerLimits { get; set; }

        public bool EnforceTellerCashTransferAcknowledgement { get; set; }

        public bool EnforceSingleUserSession { get; set; }

        public bool CustomerMembershipTextAlertsEnabled { get; set; }

        public bool EnforceInvestmentProductExemptions { get; set; }

        public bool EnforceMobileToBankReconciliationVerification { get; set; }

        public void Lock()
        {
            if (!IsLocked)
                IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }

        public void EnforceFileTracking()
        {
            if (!IsFileTrackingEnforced)
                this.IsFileTrackingEnforced = true;
        }

        public void ExemptFileTracking()
        {
            if (IsFileTrackingEnforced)
                this.IsFileTrackingEnforced = false;
        }
    }
}
