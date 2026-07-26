using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg
{
    public class JournalReversalBatchEntry : Entity
    {
        public Guid JournalReversalBatchId { get; set; }

        public virtual JournalReversalBatch JournalReversalBatch { get; private set; }

        public Guid? JournalId { get; set; }

        public virtual Journal Journal { get; private set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        

        
    }
}
