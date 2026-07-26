using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg
{
    public static class JournalExtensions
    {
        public static void GenerateNewCustomIdentity(this Journal journal)
        {
            if (journal == null)
                throw new ArgumentNullException(nameof(journal));

            journal.IntegrityHash = journal.GenerateIntegrityHash();

            switch ((SystemTransactionCode)journal.TransactionCode)
            {
                // block possible duplicates for this tx codes (IFF parent tx)
                case SystemTransactionCode.CashWithdrawal:
                case SystemTransactionCode.CashDeposit:
                case SystemTransactionCode.ChequeDeposit:
                case SystemTransactionCode.CashWithdrawalPaymentVoucher:
                case SystemTransactionCode.BankToTreasury:
                case SystemTransactionCode.TreasuryToBank:
                case SystemTransactionCode.TreasuryToTeller:
                case SystemTransactionCode.TreasuryToTreasury:
                case SystemTransactionCode.GeneralCashReceipt:
                case SystemTransactionCode.GeneralChequeReceipt:
                case SystemTransactionCode.GeneralCashPayment:
                case SystemTransactionCode.ExternalChequeClearance:
                case SystemTransactionCode.ExternalChequeBanking:
                case SystemTransactionCode.ExternalChequeTransfer:
                case SystemTransactionCode.TellerEndOfDay:
                case SystemTransactionCode.TellerCashTransfer:
                case SystemTransactionCode.CreditBatchCashPickup:
                case SystemTransactionCode.CreditBatchSundryPayment:
                case SystemTransactionCode.FixedDeposit:
                case SystemTransactionCode.BackOfficeCashReceipt:

                    if (!journal.ParentId.HasValue)
                    {
                        var newIdentity = GuidUtility.Create(GuidUtility.IsoOidNamespace, journal.IntegrityHash);

                        journal.ChangeCurrentIdentity(newIdentity, journal.SequentialId, journal.CreatedBy, journal.CreatedDate);
                    }
                    break;
                default:
                    break;
            }

        }

        public static string GenerateIntegrityHash(this Journal journal)
        {
            if (journal == null)
                throw new ArgumentNullException(nameof(journal));

            using (SHA1 sha1 = SHA1.Create())
            {
                var nfi = new NumberFormatInfo();
                nfi.CurrencySymbol = string.Empty;

                var theString = string.Join("|",
                                 journal.PostingPeriodId.ToString("D"),
                                 journal.BranchId.ToString("D"),
                                 string.Format(nfi, "{0:C}", journal.TotalValue),
                                 journal.PrimaryDescription ?? string.Empty,
                                 journal.SecondaryDescription ?? string.Empty,
                                 journal.Reference ?? string.Empty,
                                 journal.ApplicationUserName ?? string.Empty,
                                 journal.EnvironmentUserName ?? string.Empty,
                                 journal.EnvironmentMachineName ?? string.Empty,
                                 journal.EnvironmentDomainName ?? string.Empty,
                                 journal.EnvironmentMACAddress ?? string.Empty,
                                 journal.EnvironmentMotherboardSerialNumber ?? string.Empty,
                                 journal.EnvironmentProcessorId ?? string.Empty,
                                 journal.EnvironmentOSVersion ?? string.Empty,
                                 journal.EnvironmentIPAddress ?? string.Empty,
                                 journal.ModuleNavigationItemCode,
                                 journal.TransactionCode,
                                 journal.CreatedBy,
                                 journal.CreatedDate.ToString("dd/MM/yyyy HH:mm"));

                return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(theString))).Replace("-", String.Empty);
            }
        }
    }
}
