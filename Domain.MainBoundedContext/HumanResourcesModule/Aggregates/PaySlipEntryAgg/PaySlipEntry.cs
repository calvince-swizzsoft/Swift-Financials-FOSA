using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg
{
    public class PaySlipEntry : Domain.Seedwork.Entity
    {
        public Guid PaySlipId { get; set; }

        public virtual PaySlip PaySlip { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string Description { get; set; }

        public int SalaryHeadType { get; set; }

        public int SalaryHeadCategory { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public int RoundingType { get; set; }

        public virtual Charge SalaryCardEntryCharge { get; set; }
    }
}
