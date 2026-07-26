using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchEntryAgg
{
    public static class DebitBatchEntryFactory
    {
        public static DebitBatchEntry CreateDebitBatchEntry(Guid debitBatchId, Guid customerAccountId, double multiplier, decimal basisValue, string reference)
        {
            var debitBatchEntry = new DebitBatchEntry();

            debitBatchEntry.GenerateNewIdentity();

            debitBatchEntry.DebitBatchId = debitBatchId;

            debitBatchEntry.CustomerAccountId = customerAccountId;

            debitBatchEntry.Multiplier = multiplier;

            debitBatchEntry.BasisValue = basisValue;

            debitBatchEntry.Reference = reference;

            debitBatchEntry.CreatedDate = DateTime.Now;

            return debitBatchEntry;
        }
    }
}
