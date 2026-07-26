using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg
{
    public static class JournalReversalBatchFactory
    {
        public static JournalReversalBatch CreateJournalReversalBatch(Guid branchId, string remarks, int priority)
        {
            var journalReversalBatch = new JournalReversalBatch();

            journalReversalBatch.GenerateNewIdentity();

            journalReversalBatch.BranchId = branchId;

            journalReversalBatch.Remarks = remarks;

            journalReversalBatch.CreatedDate = DateTime.Now;

            journalReversalBatch.Priority = (byte)priority;

            return journalReversalBatch;
        }
    }
}
