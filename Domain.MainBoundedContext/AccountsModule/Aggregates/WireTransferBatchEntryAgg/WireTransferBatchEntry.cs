using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg
{
    public class WireTransferBatchEntry : Entity
    {
        public Guid WireTransferBatchId { get; set; }

        public virtual WireTransferBatch WireTransferBatch { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public decimal Amount { get; set; }

        public string Payee { get; set; }

        public string AccountNumber { get; set; }

        public string Reference { get; set; }

        public string ThirdPartyResponse { get; set; }

        public byte Status { get; set; }
    }
}
