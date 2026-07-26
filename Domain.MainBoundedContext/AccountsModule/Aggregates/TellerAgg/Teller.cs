using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg
{
    public class Teller : Domain.Seedwork.Entity
    {
        public byte Type { get; set; }

        public Guid? EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public Guid? ShortageChartOfAccountId { get; set; }

        public virtual ChartOfAccount ShortageChartOfAccount { get; private set; }

        public Guid? ExcessChartOfAccountId { get; set; }

        public virtual ChartOfAccount ExcessChartOfAccount { get; private set; }

        public Guid? FloatCustomerAccountId { get; set; }

        public virtual CustomerAccount FloatCustomerAccount { get; private set; }

        public Guid? CommissionCustomerAccountId { get; set; }

        public virtual CustomerAccount CommissionCustomerAccount { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public virtual Range Range { get; set; }

        public int MiniStatementItemsCap { get; set; }

        public string Reference { get; set; }

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
