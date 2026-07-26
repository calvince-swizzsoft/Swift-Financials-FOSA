using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Services
{
    public class BranchService
    {
        private readonly string _connectionString;

        public BranchService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;
        }

        public IEnumerable<BranchDTO> GetAll()
        {
            var list = new List<BranchDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT b.*, c.Description as CompanyDescription,
                                        c.Address_City as CompanyAddressCity,
                                        c.Address_Street as CompanyAddressStreet,
                                        c.Address_Email as CompanyAddressEmail,
                                        c.Address_LandLine as CompanyAddressLandLine,
                                        c.Address_MobileLine as CompanyAddressMobileLine,
                                        c.TransactionReceiptTopIndentation as CompanyTransactionReceiptTopIndentation,
                                        c.TransactionReceiptLeftIndentation as CompanyTransactionReceiptLeftIndentation,
                                        c.TransactionReceiptFooter as CompanyTransactionReceiptFooter,
                                        c.RecoveryPriority as CompanyRecoveryPriority,
                                        c.ApplicationMembershipTextAlertsEnabled as CompanyApplicationMembershipTextAlertsEnabled,
                                        c.CustomerMembershipTextAlertsEnabled as CompanyCustomerMembershipTextAlertsEnabled,
                                        c.EnforceCustomerAccountMakerChecker as CompanyEnforceCustomerAccountMakerChecker,
                                        c.BypassJournalVoucherAudit as CompanyBypassJournalVoucherAudit,
                                        c.BypassCreditBatchAudit as CompanyBypassCreditBatchAudit,
                                        c.BypassDebitBatchAudit as CompanyBypassDebitBatchAudit,
                                        c.BypassRefundBatchAudit as CompanyBypassRefundBatchAudit,
                                        c.BypassWireTransferBatchAudit as CompanyBypassWireTransferBatchAudit,
                                        c.BypassLoanDisbursementBatchAudit as CompanyBypassLoanDisbursementBatchAudit,
                                        c.BypassJournalReversalBatchAudit as CompanyBypassJournalReversalBatchAudit,
                                        c.BypassInterAccountTransferBatchAudit as CompanyBypassInterAccountTransferBatchAudit,
                                        c.BypassExpensePayableAudit as CompanyBypassExpensePayableAudit,
                                        c.BypassGeneralLedgerAudit as CompanyBypassGeneralLedgerAudit,
                                        c.ExcludeChargesInTransactionReceipt as CompanyExcludeChargesInTransactionReceipt,
                                        c.TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement as CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement,
                                        c.ReceiveLoanRequestBeforeLoanRegistration as CompanyReceiveLoanRequestBeforeLoanRegistration,
                                        c.IsFileTrackingEnforced as CompanyIsFileTrackingEnforced,
                                        c.MembershipTerminationNoticePeriod as CompanyMembershipTerminationNoticePeriod,
                                        c.ExcludeChequeMaturityDateInTransactionReceipt as CompanyExcludeChequeMaturityDateInTransactionReceipt,
                                        c.TrackGuarantorCommittedInvestments as CompanyTrackGuarantorCommittedInvestments,
                                        c.LocalizeOnlineNotifications as CompanyLocalizeOnlineNotifications,
                                        c.EnforceBudgetControl as CompanyEnforceBudgetControl,
                                        c.ExcludeCustomerAccountBalanceInTransactionReceipt as CompanyExcludeCustomerAccountBalanceInTransactionReceipt,
                                        c.EnforceFixedDepositBands as CompanyEnforceFixedDepositBands,
                                        c.EnforceBiometricsForCashWithdrawal as CompanyEnforceBiometricsForCashWithdrawal,
                                        c.FingerprintBiometricThreshold as CompanyFingerprintBiometricThreshold,
                                        c.RecoverArrearsOnCashDeposit as CompanyRecoverArrearsOnCashDeposit,
                                        c.RecoverArrearsOnExternalChequeClearance as CompanyRecoverArrearsOnExternalChequeClearance,
                                        c.RecoverArrearsOnFixedDepositPayment as CompanyRecoverArrearsOnFixedDepositPayment,
                                        c.EnforceSystemLock as CompanyEnforceSystemLock,
                                        c.EnforceTellerLimits as CompanyEnforceTellerLimits,
                                        c.EnforceTellerCashTransferAcknowledgement as CompanyEnforceTellerCashTransferAcknowledgement,
                                        c.EnforceInvestmentProductExemptions as CompanyEnforceInvestmentProductExemptions,
                                        c.EnforceMobileToBankReconciliationVerification as CompanyEnforceMobileToBankReconciliationVerification
                                FROM [swiftFin_Branches] b
                                INNER JOIN [swiftFin_Companies] c ON b.CompanyId = c.Id
                                ORDER BY b.Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public BranchDTO GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT b.*, c.Description as CompanyDescription,
                                        c.Address_City as CompanyAddressCity,
                                        c.Address_Street as CompanyAddressStreet,
                                        c.Address_Email as CompanyAddressEmail,
                                        c.Address_LandLine as CompanyAddressLandLine,
                                        c.Address_MobileLine as CompanyAddressMobileLine,
                                        c.TransactionReceiptTopIndentation as CompanyTransactionReceiptTopIndentation,
                                        c.TransactionReceiptLeftIndentation as CompanyTransactionReceiptLeftIndentation,
                                        c.TransactionReceiptFooter as CompanyTransactionReceiptFooter,
                                        c.RecoveryPriority as CompanyRecoveryPriority,
                                        c.ApplicationMembershipTextAlertsEnabled as CompanyApplicationMembershipTextAlertsEnabled,
                                        c.CustomerMembershipTextAlertsEnabled as CompanyCustomerMembershipTextAlertsEnabled,
                                        c.EnforceCustomerAccountMakerChecker as CompanyEnforceCustomerAccountMakerChecker,
                                        c.BypassJournalVoucherAudit as CompanyBypassJournalVoucherAudit,
                                        c.BypassCreditBatchAudit as CompanyBypassCreditBatchAudit,
                                        c.BypassDebitBatchAudit as CompanyBypassDebitBatchAudit,
                                        c.BypassRefundBatchAudit as CompanyBypassRefundBatchAudit,
                                        c.BypassWireTransferBatchAudit as CompanyBypassWireTransferBatchAudit,
                                        c.BypassLoanDisbursementBatchAudit as CompanyBypassLoanDisbursementBatchAudit,
                                        c.BypassJournalReversalBatchAudit as CompanyBypassJournalReversalBatchAudit,
                                        c.BypassInterAccountTransferBatchAudit as CompanyBypassInterAccountTransferBatchAudit,
                                        c.BypassExpensePayableAudit as CompanyBypassExpensePayableAudit,
                                        c.BypassGeneralLedgerAudit as CompanyBypassGeneralLedgerAudit,
                                        c.ExcludeChargesInTransactionReceipt as CompanyExcludeChargesInTransactionReceipt,
                                        c.TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement as CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement,
                                        c.ReceiveLoanRequestBeforeLoanRegistration as CompanyReceiveLoanRequestBeforeLoanRegistration,
                                        c.IsFileTrackingEnforced as CompanyIsFileTrackingEnforced,
                                        c.MembershipTerminationNoticePeriod as CompanyMembershipTerminationNoticePeriod,
                                        c.ExcludeChequeMaturityDateInTransactionReceipt as CompanyExcludeChequeMaturityDateInTransactionReceipt,
                                        c.TrackGuarantorCommittedInvestments as CompanyTrackGuarantorCommittedInvestments,
                                        c.LocalizeOnlineNotifications as CompanyLocalizeOnlineNotifications,
                                        c.EnforceBudgetControl as CompanyEnforceBudgetControl,
                                        c.ExcludeCustomerAccountBalanceInTransactionReceipt as CompanyExcludeCustomerAccountBalanceInTransactionReceipt,
                                        c.EnforceFixedDepositBands as CompanyEnforceFixedDepositBands,
                                        c.EnforceBiometricsForCashWithdrawal as CompanyEnforceBiometricsForCashWithdrawal,
                                        c.FingerprintBiometricThreshold as CompanyFingerprintBiometricThreshold,
                                        c.RecoverArrearsOnCashDeposit as CompanyRecoverArrearsOnCashDeposit,
                                        c.RecoverArrearsOnExternalChequeClearance as CompanyRecoverArrearsOnExternalChequeClearance,
                                        c.RecoverArrearsOnFixedDepositPayment as CompanyRecoverArrearsOnFixedDepositPayment,
                                        c.EnforceSystemLock as CompanyEnforceSystemLock,
                                        c.EnforceTellerLimits as CompanyEnforceTellerLimits,
                                        c.EnforceTellerCashTransferAcknowledgement as CompanyEnforceTellerCashTransferAcknowledgement,
                                        c.EnforceInvestmentProductExemptions as CompanyEnforceInvestmentProductExemptions,
                                        c.EnforceMobileToBankReconciliationVerification as CompanyEnforceMobileToBankReconciliationVerification
                                FROM [swiftFin_Branches] b
                                INNER JOIN [swiftFin_Companies] c ON b.CompanyId = c.Id
                                WHERE b.Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public IEnumerable<BranchDTO> GetByCompanyId(Guid companyId)
        {
            var list = new List<BranchDTO>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT b.*, c.Description as CompanyDescription
                                FROM [swiftFin_Branches] b
                                INNER JOIN [swiftFin_Companies] c ON b.CompanyId = c.Id
                                WHERE b.CompanyId = @CompanyId
                                ORDER BY b.Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(Map(reader));
                }
            }
            return list;
        }

        public BranchDTO GetByCode(int code)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT b.*, c.Description as CompanyDescription
                                FROM [swiftFin_Branches] b
                                INNER JOIN [swiftFin_Companies] c ON b.CompanyId = c.Id
                                WHERE b.Code = @Code";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Code", code);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public bool CompanyExists(Guid companyId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM [swiftFin_Companies] WHERE Id = @CompanyId AND IsLocked = 0";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public BranchDTO Create(BranchDTO branch)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // Generate new Guid if not provided
                if (branch.Id == Guid.Empty)
                    branch.Id = Guid.NewGuid();

                branch.CreatedDate = DateTime.Now;

                string query = @"INSERT INTO [swiftFin_Branches] 
                                ([Id], [CompanyId], [Code], [Description], 
                                 [Address_AddressLine1], [Address_AddressLine2], [Address_Street], 
                                 [Address_PostalCode], [Address_City], [Address_Email], 
                                 [Address_LandLine], [Address_MobileLine], [IsLocked], [CreatedDate])
                                VALUES 
                                (@Id, @CompanyId, @Code, @Description, 
                                 @Address_AddressLine1, @Address_AddressLine2, @Address_Street, 
                                 @Address_PostalCode, @Address_City, @Address_Email, 
                                 @Address_LandLine, @Address_MobileLine, @IsLocked, @CreatedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, branch);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Return the created branch with company details
            return GetById(branch.Id);
        }

        public void Update(BranchDTO branch)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [swiftFin_Branches] 
                                SET [CompanyId] = @CompanyId,
                                    [Code] = @Code,
                                    [Description] = @Description,
                                    [Address_AddressLine1] = @Address_AddressLine1,
                                    [Address_AddressLine2] = @Address_AddressLine2,
                                    [Address_Street] = @Address_Street,
                                    [Address_PostalCode] = @Address_PostalCode,
                                    [Address_City] = @Address_City,
                                    [Address_Email] = @Address_Email,
                                    [Address_LandLine] = @Address_LandLine,
                                    [Address_MobileLine] = @Address_MobileLine,
                                    [IsLocked] = @IsLocked
                                WHERE [Id] = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, branch);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [swiftFin_Branches] WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddParams(SqlCommand cmd, BranchDTO branch)
        {
            cmd.Parameters.AddWithValue("@Id", branch.Id);
            cmd.Parameters.AddWithValue("@CompanyId", branch.CompanyId);
            cmd.Parameters.AddWithValue("@Code", branch.Code);
            cmd.Parameters.AddWithValue("@Description", branch.Description ?? "");
            cmd.Parameters.AddWithValue("@Address_AddressLine1", branch.AddressAddressLine1 ?? "");
            cmd.Parameters.AddWithValue("@Address_AddressLine2", branch.AddressAddressLine2 ?? "");
            cmd.Parameters.AddWithValue("@Address_Street", branch.AddressStreet ?? "");
            cmd.Parameters.AddWithValue("@Address_PostalCode", branch.AddressPostalCode ?? "");
            cmd.Parameters.AddWithValue("@Address_City", branch.AddressCity ?? "");
            cmd.Parameters.AddWithValue("@Address_Email", branch.AddressEmail ?? "");
            cmd.Parameters.AddWithValue("@Address_LandLine", branch.AddressLandLine ?? "");
            cmd.Parameters.AddWithValue("@Address_MobileLine", branch.AddressMobileLine ?? "");
            cmd.Parameters.AddWithValue("@IsLocked", branch.IsLocked);
            cmd.Parameters.AddWithValue("@CreatedDate", branch.CreatedDate);
        }

        private BranchDTO Map(IDataReader reader)
        {
            return new BranchDTO
            {
                Id = (Guid)reader["Id"],
                CompanyId = (Guid)reader["CompanyId"],
                CompanyDescription = reader["CompanyDescription"]?.ToString(),
                CompanyAddressCity = reader["CompanyAddressCity"]?.ToString(),
                CompanyAddressStreet = reader["CompanyAddressStreet"]?.ToString(),
                CompanyAddressEmail = reader["CompanyAddressEmail"]?.ToString(),
                CompanyAddressLandLine = reader["CompanyAddressLandLine"]?.ToString(),
                CompanyAddressMobileLine = reader["CompanyAddressMobileLine"]?.ToString(),
                CompanyTransactionReceiptTopIndentation = reader["CompanyTransactionReceiptTopIndentation"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyTransactionReceiptTopIndentation"]),
                CompanyTransactionReceiptLeftIndentation = reader["CompanyTransactionReceiptLeftIndentation"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyTransactionReceiptLeftIndentation"]),
                CompanyTransactionReceiptFooter = reader["CompanyTransactionReceiptFooter"]?.ToString(),
                CompanyRecoveryPriority = reader["CompanyRecoveryPriority"]?.ToString(),
                CompanyApplicationMembershipTextAlertsEnabled = reader["CompanyApplicationMembershipTextAlertsEnabled"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyApplicationMembershipTextAlertsEnabled"]),
                CompanyCustomerMembershipTextAlertsEnabled = reader["CompanyCustomerMembershipTextAlertsEnabled"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyCustomerMembershipTextAlertsEnabled"]),
                CompanyEnforceCustomerAccountMakerChecker = reader["CompanyEnforceCustomerAccountMakerChecker"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceCustomerAccountMakerChecker"]),
                CompanyBypassJournalVoucherAudit = reader["CompanyBypassJournalVoucherAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassJournalVoucherAudit"]),
                CompanyBypassCreditBatchAudit = reader["CompanyBypassCreditBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassCreditBatchAudit"]),
                CompanyBypassDebitBatchAudit = reader["CompanyBypassDebitBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassDebitBatchAudit"]),
                CompanyBypassRefundBatchAudit = reader["CompanyBypassRefundBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassRefundBatchAudit"]),
                CompanyBypassWireTransferBatchAudit = reader["CompanyBypassWireTransferBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassWireTransferBatchAudit"]),
                CompanyBypassLoanDisbursementBatchAudit = reader["CompanyBypassLoanDisbursementBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassLoanDisbursementBatchAudit"]),
                CompanyBypassJournalReversalBatchAudit = reader["CompanyBypassJournalReversalBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassJournalReversalBatchAudit"]),
                CompanyBypassInterAccountTransferBatchAudit = reader["CompanyBypassInterAccountTransferBatchAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassInterAccountTransferBatchAudit"]),
                CompanyBypassExpensePayableAudit = reader["CompanyBypassExpensePayableAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassExpensePayableAudit"]),
                CompanyBypassGeneralLedgerAudit = reader["CompanyBypassGeneralLedgerAudit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyBypassGeneralLedgerAudit"]),
                CompanyExcludeChargesInTransactionReceipt = reader["CompanyExcludeChargesInTransactionReceipt"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyExcludeChargesInTransactionReceipt"]),
                CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement = reader["CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement"]),
                CompanyReceiveLoanRequestBeforeLoanRegistration = reader["CompanyReceiveLoanRequestBeforeLoanRegistration"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyReceiveLoanRequestBeforeLoanRegistration"]),
                CompanyIsFileTrackingEnforced = reader["CompanyIsFileTrackingEnforced"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyIsFileTrackingEnforced"]),
                CompanyMembershipTerminationNoticePeriod = reader["CompanyMembershipTerminationNoticePeriod"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyMembershipTerminationNoticePeriod"]),
                CompanyExcludeChequeMaturityDateInTransactionReceipt = reader["CompanyExcludeChequeMaturityDateInTransactionReceipt"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyExcludeChequeMaturityDateInTransactionReceipt"]),
                CompanyTrackGuarantorCommittedInvestments = reader["CompanyTrackGuarantorCommittedInvestments"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyTrackGuarantorCommittedInvestments"]),
                CompanyLocalizeOnlineNotifications = reader["CompanyLocalizeOnlineNotifications"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyLocalizeOnlineNotifications"]),
                CompanyEnforceBudgetControl = reader["CompanyEnforceBudgetControl"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceBudgetControl"]),
                CompanyExcludeCustomerAccountBalanceInTransactionReceipt = reader["CompanyExcludeCustomerAccountBalanceInTransactionReceipt"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyExcludeCustomerAccountBalanceInTransactionReceipt"]),
                CompanyEnforceFixedDepositBands = reader["CompanyEnforceFixedDepositBands"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceFixedDepositBands"]),
                CompanyEnforceBiometricsForCashWithdrawal = reader["CompanyEnforceBiometricsForCashWithdrawal"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceBiometricsForCashWithdrawal"]),
                CompanyFingerprintBiometricThreshold = reader["CompanyFingerprintBiometricThreshold"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyFingerprintBiometricThreshold"]),
                CompanyRecoverArrearsOnCashDeposit = reader["CompanyRecoverArrearsOnCashDeposit"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyRecoverArrearsOnCashDeposit"]),
                CompanyRecoverArrearsOnExternalChequeClearance = reader["CompanyRecoverArrearsOnExternalChequeClearance"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyRecoverArrearsOnExternalChequeClearance"]),
                CompanyRecoverArrearsOnFixedDepositPayment = reader["CompanyRecoverArrearsOnFixedDepositPayment"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyRecoverArrearsOnFixedDepositPayment"]),
                CompanyEnforceSystemLock = reader["CompanyEnforceSystemLock"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceSystemLock"]),
                CompanyEnforceTellerLimits = reader["CompanyEnforceTellerLimits"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceTellerLimits"]),
                CompanyEnforceTellerCashTransferAcknowledgement = reader["CompanyEnforceTellerCashTransferAcknowledgement"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceTellerCashTransferAcknowledgement"]),
                CompanyEnforceInvestmentProductExemptions = reader["CompanyEnforceInvestmentProductExemptions"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceInvestmentProductExemptions"]),
                CompanyEnforceMobileToBankReconciliationVerification = reader["CompanyEnforceMobileToBankReconciliationVerification"] == DBNull.Value ? false : Convert.ToBoolean(reader["CompanyEnforceMobileToBankReconciliationVerification"]),
                Code = Convert.ToInt32(reader["Code"]),
                Description = reader["Description"]?.ToString(),
                AddressAddressLine1 = reader["Address_AddressLine1"]?.ToString(),
                AddressAddressLine2 = reader["Address_AddressLine2"]?.ToString(),
                AddressStreet = reader["Address_Street"]?.ToString(),
                AddressPostalCode = reader["Address_PostalCode"]?.ToString(),
                AddressCity = reader["Address_City"]?.ToString(),
                AddressEmail = reader["Address_Email"]?.ToString(),
                AddressLandLine = reader["Address_LandLine"]?.ToString(),
                AddressMobileLine = reader["Address_MobileLine"]?.ToString(),
                IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }
    }
}