using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class CompanyBindingModel : BindingModelBase<CompanyBindingModel>
    {
        public CompanyBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Vision")]
        public string Vision { get; set; }

        [DataMember]
        [Display(Name = "Mission")]
        public string Mission { get; set; }

        [DataMember]
        [Display(Name = "Motto")]
        public string Motto { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        [Required]
        public string RegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "P.I.N Number")]
        [Required]
        public string PersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Rpt Display Name")]
        public string ApplicationDisplayName { get; set; }

        [DataMember]
        [Display(Name = "Recovery Priority")]
        public string RecoveryPriority { get; set; }

        [DataMember]
        [Display(Name = "Address Line 1")]
        [Required]
        public string AddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        [Required]
        public string AddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        [Required]
        public string AddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        [Required]
        public string AddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        [Required]
        public string AddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        [Required]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        public string AddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Logo")]
        public byte[] ImageBuffer { get; set; }

        [DataMember]
        [Display(Name = "TxRcpt Indentation(T)")]
        public int TransactionReceiptTopIndentation { get; set; }

        [DataMember]
        [Display(Name = "TxRcpt Indentation(L)")]
        public int TransactionReceiptLeftIndentation { get; set; }

        [DataMember]
        [Display(Name = "TxRcpt Footer")]
        public string TransactionReceiptFooter { get; set; }

        [DataMember]
        [Display(Name = "Fingerprint Biometric Threshold")]
        public int FingerprintBiometricThreshold { get; set; }

        [DataMember]
        [Display(Name = "Termn Notice Period")]
        public int MembershipTerminationNoticePeriod { get; set; }

        [DataMember]
        [Display(Name = "System Initialization Time")]
        public TimeSpan TimeDurationStartTime { get; set; }

        [DataMember]
        [Display(Name = "System Lock Time")]
        public TimeSpan TimeDurationEndTime { get; set; }

        [DataMember]
        [Display(Name = "Application membership text alerts enabled?")]
        public bool ApplicationMembershipTextAlertsEnabled { get; set; }

        [DataMember]
        [Display(Name = "Enforce customer account maker/checker?")]
        public bool EnforceCustomerAccountMakerChecker { get; set; }

        [DataMember]
        [Display(Name = "Bypass journal voucher verification?")]
        public bool BypassJournalVoucherAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass credit batch verification?")]
        public bool BypassCreditBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass debit batch verification?")]
        public bool BypassDebitBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass refund batch verification?")]
        public bool BypassRefundBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass EFT batch verification?")]
        public bool BypassWireTransferBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass disbursement batch verification?")]
        public bool BypassLoanDisbursementBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass reversal batch verification?")]
        public bool BypassJournalReversalBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass inter-account transfer batch verification?")]
        public bool BypassInterAccountTransferBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass expense payable verification?")]
        public bool BypassExpensePayableAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass general ledger verification?")]
        public bool BypassGeneralLedgerAudit { get; set; }

        [DataMember]
        [Display(Name = "Exclude charges in TxRcpt?")]
        public bool ExcludeChargesInTransactionReceipt { get; set; }

        [DataMember]
        [Display(Name = "Exclude cheque maturity date in TxRcpt?")]
        public bool ExcludeChequeMaturityDateInTransactionReceipt { get; set; }

        [DataMember]
        [Display(Name = "Track guarantor committed investments?")]
        public bool TrackGuarantorCommittedInvestments { get; set; }

        [DataMember]
        [Display(Name = "Transfer net refundable amount to savings A/C on death claim settlement?")]
        public bool TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement { get; set; }

        [DataMember]
        [Display(Name = "Receive loan request before loan registration?")]
        public bool ReceiveLoanRequestBeforeLoanRegistration { get; set; }

        [DataMember]
        [Display(Name = "Localize Online Notifications?")]
        public bool LocalizeOnlineNotifications { get; set; }

        [DataMember]
        [Display(Name = "Is Withholding Tax Agent?")]
        public bool IsWithholdingTaxAgent { get; set; }

        [DataMember]
        [Display(Name = "Enforce Budget Control?")]
        public bool EnforceBudgetControl { get; set; }

        [DataMember]
        [Display(Name = "Is File Tracking Enforced?")]
        public bool IsFileTrackingEnforced { get; set; }

        [DataMember]
        [Display(Name = "Exclude Customer Account Balance In TxRcpt?")]
        public bool ExcludeCustomerAccountBalanceInTransactionReceipt { get; set; }

        [DataMember]
        [Display(Name = "Enforce Fixed Deposit Bands?")]
        public bool EnforceFixedDepositBands { get; set; }

        [DataMember]
        [Display(Name = "Enforce Biometrics For Cash Withdrawal?")]
        public bool EnforceBiometricsForCashWithdrawal { get; set; }

        [DataMember]
        [Display(Name = "Enforce Two Factor Authentication?")]
        public bool EnforceTwoFactorAuthentication { get; set; }

        [DataMember]
        [Display(Name = "Recover Arrears On Cash Deposit?")]
        public bool RecoverArrearsOnCashDeposit { get; set; }

        [DataMember]
        [Display(Name = "Recover Arrears On External Cheque Clearance?")]
        public bool RecoverArrearsOnExternalChequeClearance { get; set; }

        [DataMember]
        [Display(Name = "Recover Arrears On Fixed Deposit Payment?")]
        public bool RecoverArrearsOnFixedDepositPayment { get; set; }

        [DataMember]
        [Display(Name = "Allow Debit Batch To Overdraw Account?")]
        public bool AllowDebitBatchToOverdrawAccount { get; set; }

        [DataMember]
        [Display(Name = "Enforce System Initialization/Lock Time?")]
        public bool EnforceSystemLock { get; set; }

        [DataMember]
        [Display(Name = "Enforce Teller Limits?")]
        public bool EnforceTellerLimits { get; set; }

        [DataMember]
        [Display(Name = "Enforce Teller Cash Transfer Acknowledgement?")]
        public bool EnforceTellerCashTransferAcknowledgement { get; set; }

        [DataMember]
        [Display(Name = "Enforce Single User Session?")]
        public bool EnforceSingleUserSession { get; set; }

        [DataMember]
        [Display(Name = "Customer Membership Text Alerts Enabled?")]
        public bool CustomerMembershipTextAlertsEnabled { get; set; }

        [DataMember]
        [Display(Name = "Enforce Investment Product Exemptions?")]
        public bool EnforceInvestmentProductExemptions { get; set; }

        [DataMember]
        [Display(Name = "Enforce Mobile To Bank Reconciliation Verification?")]
        public bool EnforceMobileToBankReconciliationVerification { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
