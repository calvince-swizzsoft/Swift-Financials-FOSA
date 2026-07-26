using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg
{
    public static class ConditionalLendingEntryFactory
    {
        public static ConditionalLendingEntry CreateConditionalLendingEntry(Guid conditionalLendingId, Guid customerId, string remarks)
        {
            var conditionalLendingEntry = new ConditionalLendingEntry();

            conditionalLendingEntry.GenerateNewIdentity();

            conditionalLendingEntry.ConditionalLendingId = conditionalLendingId;

            conditionalLendingEntry.CustomerId = customerId;

            conditionalLendingEntry.Remarks = remarks;

            conditionalLendingEntry.CreatedDate = DateTime.Now;

            return conditionalLendingEntry;
        }
    }
}
