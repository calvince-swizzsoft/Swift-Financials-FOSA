using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg
{
    public static class FiscalCountExtensions
    {
        public static string GenerateSystemTraceAuditNumber(this FiscalCount fiscalCount)
        {
            var hashCode = string.Empty;

            if (fiscalCount != null)
            {
                switch ((SystemTransactionCode)fiscalCount.TransactionCode)
                {
                    // block possible duplicates for cash mgt fiscal entries transactions
                    case SystemTransactionCode.BankToTreasury:
                    case SystemTransactionCode.TreasuryToBank:
                    case SystemTransactionCode.TreasuryToTeller:
                    case SystemTransactionCode.TellerEndOfDay:
                    case SystemTransactionCode.TellerCashTransfer:
                        
                        using (SHA1 sha1 = SHA1.Create())
                        {
                            var nfi = new NumberFormatInfo();
                            nfi.CurrencySymbol = string.Empty;

                            var theString = string.Join("|",
                             fiscalCount.PostingPeriodId.ToString("D"),
                             fiscalCount.BranchId.ToString("D"),
                             fiscalCount.ChartOfAccountId.ToString("D"),
                             fiscalCount.PrimaryDescription ?? string.Empty,
                             fiscalCount.SecondaryDescription ?? string.Empty,
                             fiscalCount.Reference ?? string.Empty,
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.OneThousandValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.FiveHundredValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.TwoHundredValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.OneHundredValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.FiftyValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.FourtyValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.TwentyValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.TenValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.FiveValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.OneValue),
                             string.Format(nfi, "{0:C}", fiscalCount.Denomination.FiftyCentValue),
                             fiscalCount.TransactionCode,
                             fiscalCount.CreatedBy ?? string.Empty,
                             fiscalCount.CreatedDate.ToString("dd/MM/yyyy HH:mm"));

                            hashCode = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(theString))).Replace("-", String.Empty);
                        }

                        break;
                    default:
                        hashCode = string.Format("{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), StrongRandom.Next(1, 1999999998));
                        break;
                }
            }

            return hashCode;
        }
    }
}
