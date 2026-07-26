using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg
{
    public class StandingOrder : Entity
    {
        public Guid BenefactorCustomerAccountId { get; set; }

        public virtual CustomerAccount BenefactorCustomerAccount { get; private set; }

        public Guid BeneficiaryCustomerAccountId { get; set; }

        public virtual CustomerAccount BeneficiaryCustomerAccount { get; private set; }

        public virtual Duration Duration { get; set; }

        public virtual Schedule Schedule { get; set; }

        public virtual Charge Charge { get; set; }

        public byte Trigger { get; set; }

        public decimal LoanAmount { get; set; }

        public decimal PaymentPerPeriod { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal CapitalizedInterest { get; set; }

        public string Remarks { get; set; }

        public bool Chargeable { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
