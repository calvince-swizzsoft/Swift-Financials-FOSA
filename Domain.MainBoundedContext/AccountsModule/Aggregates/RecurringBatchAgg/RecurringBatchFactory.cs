using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg
{
    public static class RecurringBatchFactory
    {
        public static RecurringBatch CreateRecurringBatch(Guid? branchId, Guid? postingPeriodId, int type, int month, string reference, int priority)
        {
            var recurringBatch = new RecurringBatch();

            recurringBatch.GenerateNewIdentity();

            recurringBatch.BranchId = (branchId != null && branchId != Guid.Empty) ? branchId : null;

            recurringBatch.PostingPeriodId = (postingPeriodId != null && postingPeriodId != Guid.Empty) ? postingPeriodId : null;

            recurringBatch.Type = (byte)type;

            recurringBatch.Month = (byte)month;

            recurringBatch.Reference = reference;

            recurringBatch.Priority = (byte)priority;

            recurringBatch.CreatedDate = DateTime.Now;

            return recurringBatch;
        }
    }
}
