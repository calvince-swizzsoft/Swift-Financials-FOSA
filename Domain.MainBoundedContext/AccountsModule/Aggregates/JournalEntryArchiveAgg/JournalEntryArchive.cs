using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalArchiveAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryArchiveAgg
{
    public class JournalEntryArchive : Entity
    {
        public Guid JournalArchiveId { get; set; }

        public virtual JournalArchive JournalArchive { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid ContraChartOfAccountId { get; set; }

        public virtual ChartOfAccount ContraChartOfAccount { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public decimal Amount { get; set; }
        
        public DateTime? ValueDate { get; set; }

        public string IntegrityHash { get; set; }
    }
}
