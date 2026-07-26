using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg
{
    public class GeneralLedgerEntry : Entity
    {
        public Guid GeneralLedgerId { get; set; }

        public virtual GeneralLedger GeneralLedger { get; private set; }

        public Guid? BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid ContraChartOfAccountId { get; set; }

        public virtual ChartOfAccount ContraChartOfAccount { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? ContraCustomerAccountId { get; set; }

        public virtual CustomerAccount ContraCustomerAccount { get; private set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ValueDate { get; set; }

        public byte Status { get; set; }
    }
}
