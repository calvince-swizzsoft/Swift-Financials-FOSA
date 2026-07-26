using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg
{
    public static class JournalReversalBatchEntryFactory
    {
        public static JournalReversalBatchEntry CreateJournalReversalBatchEntry(Guid journalReversalBatchId, Guid journalId, string remarks)
        {
            var journalReversalBatchEntry = new JournalReversalBatchEntry();

            journalReversalBatchEntry.GenerateNewIdentity();

            journalReversalBatchEntry.JournalReversalBatchId = journalReversalBatchId;

            journalReversalBatchEntry.JournalId = journalId;

            journalReversalBatchEntry.Remarks = remarks;

            journalReversalBatchEntry.CreatedDate = DateTime.Now;

            return journalReversalBatchEntry;
        }
    }
}
