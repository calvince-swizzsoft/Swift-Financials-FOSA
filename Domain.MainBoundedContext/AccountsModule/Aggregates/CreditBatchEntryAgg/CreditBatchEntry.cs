using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg
{
    public class CreditBatchEntry : Domain.Seedwork.Entity
    {
        public Guid CreditBatchId { get; set; }

        public virtual CreditBatch CreditBatch { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal Balance { get; set; }

        public string Beneficiary { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }
    }
}
