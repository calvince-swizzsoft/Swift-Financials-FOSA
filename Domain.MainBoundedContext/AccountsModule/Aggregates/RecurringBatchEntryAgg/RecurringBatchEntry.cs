using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg
{
    public class RecurringBatchEntry : Entity
    {
        public Guid RecurringBatchId { get; set; }

        public virtual RecurringBatch RecurringBatch { get; private set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? SecondaryCustomerAccountId { get; set; }

        public virtual CustomerAccount SecondaryCustomerAccount { get; private set; }

        public Guid? StandingOrderId { get; set; }

        public virtual StandingOrder StandingOrder { get; private set; }

        public Guid? ElectronicStatementOrderId { get; set; }

        public virtual ElectronicStatementOrder ElectronicStatementOrder { get; private set; }

        public virtual Duration ElectronicStatement { get; set; }

        public string ElectronicStatementSender { get; set; }

        public string Reference { get; set; }

        public string Remarks { get; set; }

        public byte InterestCapitalizationMonths { get; set; }

        public bool EnforceCeiling { get; set; }

        public byte Status { get; set; }
    }
}
