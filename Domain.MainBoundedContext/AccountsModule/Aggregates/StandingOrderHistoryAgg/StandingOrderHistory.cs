using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg
{
    public class StandingOrderHistory : Entity
    {
        public Guid StandingOrderId { get; set; }

        public virtual StandingOrder StandingOrder { get; private set; }

        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid BenefactorCustomerAccountId { get; set; }

        public virtual CustomerAccount BenefactorCustomerAccount { get; private set; }

        public Guid BeneficiaryCustomerAccountId { get; set; }

        public virtual CustomerAccount BeneficiaryCustomerAccount { get; private set; }

        public virtual Duration Duration { get; set; }

        public virtual Schedule Schedule { get; set; }

        public virtual Charge Charge { get; set; }

        public byte Month { get; set; }

        public byte Trigger { get; set; }

        public decimal ExpectedPrincipal { get; set; }

        public decimal ExpectedInterest { get; set; }

        public decimal ActualPrincipal { get; set; }

        public decimal ActualInterest { get; set; }

        public string Remarks { get; set; }
    }
}
