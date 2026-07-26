using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg
{
    public class CustomerAccountCarryForward : Entity
    {
        public Guid BenefactorCustomerAccountId { get; set; }

        public virtual CustomerAccount BenefactorCustomerAccount { get; private set; }

        public Guid BeneficiaryCustomerAccountId { get; set; }

        public virtual CustomerAccount BeneficiaryCustomerAccount { get; private set; }

        public Guid BeneficiaryChartOfAccountId { get; set; }

        public virtual ChartOfAccount BeneficiaryChartOfAccount { get; private set; }

        public decimal Amount { get; set; }

        public string Reference { get; set; }
    }
}
