using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg
{
    public static class CompanyFactory
    {
        public static Company CreateCompany(string description, string vision, string mission, string motto, string registrationNumber, string personalIdentificationNumber, string applicationDisplayName,
            int transactionReceiptTopIndentation, int transactionReceiptLeftIndentation, string transactionReceiptFooter, int fingerprintBiometricThreshold, bool applicationMembershipTextAlertsEnabled, bool enforceCustomerAccountMakerChecker,
            bool bypassJournalVoucherAudit, bool bypassCreditBatchAudit, bool bypassDebitBatchAudit, bool bypassRefundBatchAudit, bool bypassWireTransferBatchAudit, bool bypassLoanDisbursementBatchAudit, bool bypassJournalReversalBatchAudit,
            bool bypassInterAccountTransferBatchAudit, bool bypassExpensePayableAudit, bool bypassGeneralLedgerAudit, bool excludeChargesInTransactionReceipt, int membershipTerminationNoticePeriod,
            bool excludeChequeMaturityDateInTransactionReceipt, bool trackGuarantorCommittedInvestments, bool transferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement, bool receiveLoanRequestBeforeLoanRegistration,
            Address address, TimeDuration timeDuration, string payoutRecoveryPriority, bool isWithholdingTaxAgent, bool enforceBudgetControl, bool localizeOnlineNotifications, bool excludeCustomerAccountBalanceInTransactionReceipt, bool enforceFixedDepositBands,
            bool enforceBiometricsForCashWithdrawal, bool enforceTwoFactorAuthentication, bool recoverArrearsOnCashDeposit, bool recoverArrearsOnExternalChequeClearance, bool recoverArrearsOnFixedDepositPayment, bool allowDebitBatchToOverdrawAccount, bool enforceTellerLimits, bool enforceSystemLock, bool enforceTellerCashTransferAcknowledgement,
            bool enforceSingleUserSession, bool customerMembershipTextAlertsEnabled, bool enforceInvestmentProductExemptions, bool enforceMobileToBankReconciliationVerification)
        {
            var company = new Company()
            {
                Description = description,
                Vision = vision,
                Mission = mission,
                Motto = motto,
                RegistrationNumber = registrationNumber,
                PersonalIdentificationNumber = personalIdentificationNumber,
                ApplicationDisplayName = applicationDisplayName,
                TransactionReceiptTopIndentation = (byte)transactionReceiptTopIndentation,
                TransactionReceiptLeftIndentation = (byte)transactionReceiptLeftIndentation,
                TransactionReceiptFooter = transactionReceiptFooter,
                FingerprintBiometricThreshold = fingerprintBiometricThreshold,
                ApplicationMembershipTextAlertsEnabled = applicationMembershipTextAlertsEnabled,
                EnforceCustomerAccountMakerChecker = enforceCustomerAccountMakerChecker,
                BypassJournalVoucherAudit = bypassJournalVoucherAudit,
                BypassCreditBatchAudit = bypassCreditBatchAudit,
                BypassDebitBatchAudit = bypassDebitBatchAudit,
                BypassRefundBatchAudit = bypassRefundBatchAudit,
                BypassWireTransferBatchAudit = bypassWireTransferBatchAudit,
                BypassLoanDisbursementBatchAudit = bypassLoanDisbursementBatchAudit,
                BypassJournalReversalBatchAudit = bypassJournalReversalBatchAudit,
                BypassInterAccountTransferBatchAudit = bypassInterAccountTransferBatchAudit,
                BypassExpensePayableAudit = bypassExpensePayableAudit,
                BypassGeneralLedgerAudit = bypassGeneralLedgerAudit,
                ExcludeChargesInTransactionReceipt = excludeChargesInTransactionReceipt,
                MembershipTerminationNoticePeriod = (short)membershipTerminationNoticePeriod,
                ExcludeChequeMaturityDateInTransactionReceipt = excludeChequeMaturityDateInTransactionReceipt,
                TrackGuarantorCommittedInvestments = trackGuarantorCommittedInvestments,
                TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement = transferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement,
                ReceiveLoanRequestBeforeLoanRegistration = receiveLoanRequestBeforeLoanRegistration,
                Address = address,
                TimeDuration = timeDuration,
                RecoveryPriority = payoutRecoveryPriority,
                IsWithholdingTaxAgent = isWithholdingTaxAgent,
                EnforceBudgetControl = enforceBudgetControl,
                LocalizeOnlineNotifications = localizeOnlineNotifications,
                ExcludeCustomerAccountBalanceInTransactionReceipt = excludeCustomerAccountBalanceInTransactionReceipt,
                EnforceFixedDepositBands = enforceFixedDepositBands,
                EnforceBiometricsForCashWithdrawal = enforceBiometricsForCashWithdrawal,
                EnforceTwoFactorAuthentication = enforceTwoFactorAuthentication,
                RecoverArrearsOnCashDeposit = recoverArrearsOnCashDeposit,
                RecoverArrearsOnExternalChequeClearance = recoverArrearsOnExternalChequeClearance,
                RecoverArrearsOnFixedDepositPayment = recoverArrearsOnFixedDepositPayment,
                AllowDebitBatchToOverdrawAccount = allowDebitBatchToOverdrawAccount,
                EnforceSystemLock = enforceSystemLock,
                EnforceTellerLimits = enforceTellerLimits,
                EnforceTellerCashTransferAcknowledgement = enforceTellerCashTransferAcknowledgement,
                EnforceSingleUserSession = enforceSingleUserSession,
                CustomerMembershipTextAlertsEnabled = customerMembershipTextAlertsEnabled,
                EnforceInvestmentProductExemptions = enforceInvestmentProductExemptions,
                EnforceMobileToBankReconciliationVerification = enforceMobileToBankReconciliationVerification
            };

            company.GenerateNewIdentity();

            company.UnLock();

            company.CreatedDate = DateTime.Now;

            return company;
        }
    }
}
