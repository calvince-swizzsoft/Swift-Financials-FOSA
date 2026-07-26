using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class BranchDTO : BindingModelBase<BranchDTO>
    {
        public BranchDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        [ValidGuid]
        public Guid CompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string CompanyDescription { get; set; }

        [DataMember]
        public string CompanyAddressCity { get; set; }

        [DataMember]
        public string CompanyAddressStreet { get; set; }

        [DataMember]
        public string CompanyAddressEmail { get; set; }

        [DataMember]
        public string CompanyAddressLandLine { get; set; }

        [DataMember]
        public string CompanyAddressMobileLine { get; set; }

        [DataMember]
        public int CompanyTransactionReceiptTopIndentation { get; set; }

        [DataMember]
        public int CompanyTransactionReceiptLeftIndentation { get; set; }

        [DataMember]
        public string CompanyTransactionReceiptFooter { get; set; }

        [DataMember]
        public string CompanyRecoveryPriority { get; set; }

        [DataMember]
        public bool CompanyApplicationMembershipTextAlertsEnabled { get; set; }

        [DataMember]
        public bool CompanyCustomerMembershipTextAlertsEnabled { get; set; }

        [DataMember]
        public bool CompanyAuditFrontOfficeLoans { get; set; }

        [DataMember]
        public bool CompanyAuditBackOfficeLoans { get; set; }

        [DataMember]
        [Display(Name = "Enforce customer account maker/checker?")]
        public bool CompanyEnforceCustomerAccountMakerChecker { get; set; }

        [DataMember]
        [Display(Name = "Bypass journal voucher verification?")]
        public bool CompanyBypassJournalVoucherAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass credit batch verification?")]
        public bool CompanyBypassCreditBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass debit batch verification?")]
        public bool CompanyBypassDebitBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass refund batch verification?")]
        public bool CompanyBypassRefundBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass wire transfer batch verification?")]
        public bool CompanyBypassWireTransferBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass disbursement batch verification?")]
        public bool CompanyBypassLoanDisbursementBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass reversal batch verification?")]
        public bool CompanyBypassJournalReversalBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass inter-account transfer batch verification?")]
        public bool CompanyBypassInterAccountTransferBatchAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass expense payable verification?")]
        public bool CompanyBypassExpensePayableAudit { get; set; }

        [DataMember]
        [Display(Name = "Bypass general ledger verification?")]
        public bool CompanyBypassGeneralLedgerAudit { get; set; }

        [DataMember]
        [Display(Name = "Exclude charges in TxRcpt?")]
        public bool CompanyExcludeChargesInTransactionReceipt { get; set; }

        [DataMember]
        public bool CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement { get; set; }

        [DataMember]
        public bool CompanyReceiveLoanRequestBeforeLoanRegistration { get; set; }

        [DataMember]
        public bool CompanyIsFileTrackingEnforced { get; set; }

        [DataMember]
        public int CompanyMembershipTerminationNoticePeriod { get; set; }

        [DataMember]
        public bool CompanyExcludeChequeMaturityDateInTransactionReceipt { get; set; }

        [DataMember]
        public bool CompanyTrackGuarantorCommittedInvestments { get; set; }

        [DataMember]
        public bool CompanyLocalizeOnlineNotifications { get; set; }

        [DataMember]
        public bool CompanyEnforceBudgetControl { get; set; }

        [DataMember]
        public bool CompanyExcludeCustomerAccountBalanceInTransactionReceipt { get; set; }

        [DataMember]
        public bool CompanyEnforceFixedDepositBands { get; set; }

        [DataMember]
        public bool CompanyEnforceBiometricsForCashWithdrawal { get; set; }

        [DataMember]
        public int CompanyFingerprintBiometricThreshold { get; set; }

        [DataMember]
        public bool CompanyRecoverArrearsOnCashDeposit { get; set; }

        [DataMember]
        public bool CompanyRecoverArrearsOnExternalChequeClearance { get; set; }

        [DataMember]
        public bool CompanyRecoverArrearsOnFixedDepositPayment { get; set; }

        [DataMember]
        public bool CompanyEnforceBiometricsForLogin { get; set; }

        [DataMember]
        public bool CompanyEnforceTellerLimits { get; set; }

        [DataMember]
        public bool CompanyEnforceSystemLock { get; set; }

        [DataMember]
        public bool CompanyEnforceTellerCashTransferAcknowledgement { get; set; }

        [DataMember]
        public bool CompanyEnforceInvestmentProductExemptions { get; set; }

        [DataMember]
        public bool CompanyEnforceMobileToBankReconciliationVerification { get; set; }

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
        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        public string AddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        public string AddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public bool IsChecked { get; set; }
        public CompanyDTO companyDTO;


        [DataMember]
        public CustomerDTO CustomerDTO { get; set; }

    }
}
