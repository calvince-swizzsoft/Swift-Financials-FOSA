using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg
{
    public static class WireTransferBatchEntryFactory
    {
        public static WireTransferBatchEntry CreateWireTransferBatchEntry(Guid wireTransferBatchId, Guid customerAccountId, decimal amount, string payee, string accountNumber, string reference)
        {
            var wireTransferBatchEntry = new WireTransferBatchEntry();

            wireTransferBatchEntry.GenerateNewIdentity();

            wireTransferBatchEntry.WireTransferBatchId = wireTransferBatchId;

            wireTransferBatchEntry.CustomerAccountId = customerAccountId;

            wireTransferBatchEntry.Amount = amount;

            wireTransferBatchEntry.Payee = payee;

            wireTransferBatchEntry.AccountNumber = accountNumber;

            wireTransferBatchEntry.Reference = reference;

            wireTransferBatchEntry.CreatedDate = DateTime.Now;

            return wireTransferBatchEntry;
        }
    }
}
