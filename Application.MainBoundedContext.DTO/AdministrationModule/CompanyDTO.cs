using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class CompanyDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Description { get; set; }

        [Display(Name = "Vision")]
        public string Vision { get; set; }

        [Display(Name = "Mission")]
        public string Mission { get; set; }

        [Display(Name = "Motto")]
        public string Motto { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Personal Identification Number")]
        public string PersonalIdentificationNumber { get; set; }

        [Display(Name = "Rpt Display Name")]
        public string ApplicationDisplayName { get; set; }

        [Display(Name = "Recovery Priority")]
        public string RecoveryPriority { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string AddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string AddressMobileLine { get; set; }

        [Display(Name = "TxRcpt Indentation(T)")]
        public byte TransactionReceiptTopIndentation { get; set; }

        [Display(Name = "TxRcpt Indentation(L)")]
        public byte TransactionReceiptLeftIndentation { get; set; }

        [Display(Name = "TxRcpt Footer")]
        public string TransactionReceiptFooter { get; set; }

        [Display(Name = "Fingerprint Biometric Threshold")]
        public int FingerprintBiometricThreshold { get; set; }

        [Display(Name = "Termn Notice Period")]
        public short MembershipTerminationNoticePeriod { get; set; }

        [Display(Name = "System Initialization Time")]
        public TimeSpan TimeDurationStartTime { get; set; }

        [Display(Name = "System Lock Time")]
        public TimeSpan TimeDurationEndTime { get; set; }

        [Display(Name = "Application membership text alerts enabled?")]
        public bool ApplicationMembershipTextAlertsEnabled { get; set; }

        [Display(Name = "Enforce customer account maker/checker?")]
        public bool EnforceCustomerAccountMakerChecker { get; set; }

        [Display(Name = "Bypass journal voucher verification?")]
        public bool BypassJournalVoucherAudit { get; set; }

        [Display(Name = "Bypass credit batch verification?")]
        public bool BypassCreditBatchAudit { get; set; }

        [Display(Name = "Bypass debit batch verification?")]
        public bool BypassDebitBatchAudit { get; set; }

        [Display(Name = "Bypass refund batch verification?")]
        public bool BypassRefundBatchAudit { get; set; }

        [Display(Name = "Bypass EFT batch verification?")]
        public bool BypassWireTransferBatchAudit { get; set; }

        [Display(Name = "Bypass disbursement batch verification?")]
        public bool BypassLoanDisbursementBatchAudit { get; set; }

        [Display(Name = "Bypass reversal batch verification?")]
        public bool BypassJournalReversalBatchAudit { get; set; }

        [Display(Name = "Bypass inter-account transfer batch verification?")]
        public bool BypassInterAccountTransferBatchAudit { get; set; }

        [Display(Name = "Bypass expense payable verification?")]
        public bool BypassExpensePayableAudit { get; set; }

        [Display(Name = "Bypass general ledger verification?")]
        public bool BypassGeneralLedgerAudit { get; set; }

        [Display(Name = "Exclude charges in TxRcpt?")]
        public bool ExcludeChargesInTransactionReceipt { get; set; }

        [Display(Name = "Exclude cheque maturity date in TxRcpt?")]
        public bool ExcludeChequeMaturityDateInTransactionReceipt { get; set; }

        [Display(Name = "Track guarantor committed investments?")]
        public bool TrackGuarantorCommittedInvestments { get; set; }

        [Display(Name = "Transfer net refundable amount to savings A/C on death claim settlement?")]
        public bool TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement { get; set; }

        [Display(Name = "Receive loan request before loan registration?")]
        public bool ReceiveLoanRequestBeforeLoanRegistration { get; set; }

        [Display(Name = "Localize Online Notifications?")]
        public bool LocalizeOnlineNotifications { get; set; }

        [Display(Name = "Is Withholding Tax Agent?")]
        public bool IsWithholdingTaxAgent { get; set; }

        [Display(Name = "Enforce Budget Control?")]
        public bool EnforceBudgetControl { get; set; }

        [Display(Name = "Is File Tracking Enforced?")]
        public bool IsFileTrackingEnforced { get; set; }

        [Display(Name = "Exclude Customer Account Balance In TxRcpt?")]
        public bool ExcludeCustomerAccountBalanceInTransactionReceipt { get; set; }

        [Display(Name = "Enforce Fixed Deposit Bands?")]
        public bool EnforceFixedDepositBands { get; set; }

        [Display(Name = "Enforce Biometrics For Cash Withdrawal?")]
        public bool EnforceBiometricsForCashWithdrawal { get; set; }

        [Display(Name = "Enforce Two Factor Authentication?")]
        public bool EnforceTwoFactorAuthentication { get; set; }

        [Display(Name = "Recover Arrears On Cash Deposit?")]
        public bool RecoverArrearsOnCashDeposit { get; set; }

        [Display(Name = "Recover Arrears On External Cheque Clearance?")]
        public bool RecoverArrearsOnExternalChequeClearance { get; set; }

        [Display(Name = "Recover Arrears On Fixed Deposit Payment?")]
        public bool RecoverArrearsOnFixedDepositPayment { get; set; }

        [Display(Name = "Allow Debit Batch To Overdraw Account?")]
        public bool AllowDebitBatchToOverdrawAccount { get; set; }

        [Display(Name = "Enforce System Initialization/Lock Time?")]
        public bool EnforceSystemLock { get; set; }

        [Display(Name = "Enforce Teller Limits?")]
        public bool EnforceTellerLimits { get; set; }

        [Display(Name = "Enforce Teller Cash Transfer Acknowledgement?")]
        public bool EnforceTellerCashTransferAcknowledgement { get; set; }

        [Display(Name = "Enforce Single User Session?")]
        public bool EnforceSingleUserSession { get; set; }

        [Display(Name = "Customer Membership Text Alerts Enabled?")]
        public bool CustomerMembershipTextAlertsEnabled { get; set; }

        [Display(Name = "Enforce Investment Product Exemptions?")]
        public bool EnforceInvestmentProductExemptions { get; set; }

        [Display(Name = "Enforce Mobile To Bank Reconciliation Verification?")]
        public bool EnforceMobileToBankReconciliationVerification { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

    }
}
