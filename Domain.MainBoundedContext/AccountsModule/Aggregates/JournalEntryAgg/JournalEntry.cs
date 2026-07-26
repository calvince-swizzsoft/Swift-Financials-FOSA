using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg
{
    public class JournalEntry : Domain.Seedwork.Entity
    {
        public Guid JournalId { get; set; }

        public virtual Journal Journal { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid? ContraChartOfAccountId { get; set; }

        public virtual ChartOfAccount ContraChartOfAccount { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public decimal Amount { get; set; }

        public DateTime? ValueDate { get; set; }

        public string IntegrityHash { get; set; }
    }
}
