using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg
{
    public class LoanProduct : Entity
    {
        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid InterestReceivedChartOfAccountId { get; set; }

        public virtual ChartOfAccount InterestReceivedChartOfAccount { get; private set; }

        public Guid InterestReceivableChartOfAccountId { get; set; }

        public virtual ChartOfAccount InterestReceivableChartOfAccount { get; private set; }

        public Guid? InterestChargedChartOfAccountId { get; set; }

        public virtual ChartOfAccount InterestChargedChartOfAccount { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public virtual LoanInterest LoanInterest { get; set; }

        public virtual LoanRegistration LoanRegistration { get; set; }

        public virtual Charge TakeHome { get; set; }

        public short Priority { get; set; }

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
