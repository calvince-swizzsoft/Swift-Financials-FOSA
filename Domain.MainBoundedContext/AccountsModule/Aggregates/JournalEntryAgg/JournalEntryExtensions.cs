using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg
{
    public static class JournalEntryExtensions
    {
        public static void GenerateIntegrityHash(this JournalEntry journalEntry)
        {
            if (journalEntry != null)
            {
                using (SHA1 sha1 = SHA1.Create())
                {
                    var nfi = new NumberFormatInfo();
                    nfi.CurrencySymbol = string.Empty;

                    var theString = string.Join("|",
                         journalEntry.Id.ToString("D"),
                         journalEntry.JournalId.ToString("D"),
                         journalEntry.ChartOfAccountId.ToString("D"),
                         journalEntry.ContraChartOfAccountId.HasValue ? journalEntry.ContraChartOfAccountId.Value.ToString("D") : string.Empty,
                         journalEntry.CustomerAccountId.HasValue ? journalEntry.CustomerAccountId.Value.ToString("D") : string.Empty,
                         string.Format(nfi, "{0:C}", journalEntry.Amount),
                         journalEntry.CreatedBy,
                         journalEntry.CreatedDate.ToString("dd/MM/yyyy HH:mm"));

                    journalEntry.IntegrityHash = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(theString))).Replace("-", String.Empty);
                }
            }
        }
    }
}
