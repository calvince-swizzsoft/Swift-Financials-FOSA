using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryArchiveAgg
{
    public static class JournalEntryArchiveFactory
    {
        public static JournalEntryArchive CreateJournalEntryArchive(Guid journalArchiveId, Guid chartOfAccountId, Guid contraChartOfAccountId, Guid? customerAccountId, decimal amount, DateTime? valueDate, ServiceHeader serviceHeader)
        {
            var journalEntryArchive = new JournalEntryArchive();

            journalEntryArchive.GenerateNewIdentity();

            journalEntryArchive.JournalArchiveId = journalArchiveId;

            journalEntryArchive.ChartOfAccountId = chartOfAccountId;

            journalEntryArchive.ContraChartOfAccountId = contraChartOfAccountId;

            journalEntryArchive.CustomerAccountId = customerAccountId;

            journalEntryArchive.Amount = amount;

            journalEntryArchive.CreatedBy = serviceHeader.ApplicationUserName;

            journalEntryArchive.CreatedDate = DateTime.Now;

            journalEntryArchive.ValueDate = valueDate ?? journalEntryArchive.CreatedDate;

            return journalEntryArchive;
        }
    }
}
