using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchEntryAgg
{
    public class DebitBatchEntry : Domain.Seedwork.Entity
    {
        public Guid DebitBatchId { get; set; }

        public virtual DebitBatch DebitBatch { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public decimal BasisValue { get; set; }

        public double Multiplier { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }
    }
}
