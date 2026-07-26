using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg
{
    public static class JournalEntryFactory
    {
        public static JournalEntry CreateJournalEntry(Guid journalId, Guid chartOfAccountId, Guid? contraChartOfAccountId, Guid? customerAccountId, decimal amount, DateTime? valueDate, ServiceHeader serviceHeader)
        {
            var journalEntry = new JournalEntry();

            journalEntry.GenerateNewIdentity();

            journalEntry.JournalId = journalId;

            journalEntry.ChartOfAccountId = chartOfAccountId;

            journalEntry.ContraChartOfAccountId = (contraChartOfAccountId != null && contraChartOfAccountId != Guid.Empty) ? contraChartOfAccountId : null;

            journalEntry.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;

            journalEntry.Amount = amount;

            journalEntry.CreatedBy = serviceHeader.ApplicationUserName;

            journalEntry.CreatedDate = DateTime.Now;

            journalEntry.ValueDate = valueDate ?? journalEntry.CreatedDate;

            journalEntry.GenerateIntegrityHash();

            return journalEntry;
        }
    }
}
