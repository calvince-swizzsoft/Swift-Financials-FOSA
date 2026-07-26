using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeAgg
{
    public static class WireTransferTypeFactory
    {
        public static WireTransferType CreateWireTransferType(Guid chartOfAccountId, string description, int transactionOwnership)
        {
            var wireTransferType = new WireTransferType();

            wireTransferType.GenerateNewIdentity();

            wireTransferType.ChartOfAccountId = chartOfAccountId;

            wireTransferType.Description = description;

            wireTransferType.TransactionOwnership = (byte)transactionOwnership;

            wireTransferType.CreatedDate = DateTime.Now;

            return wireTransferType;
        }
    }
}
