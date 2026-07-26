using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg
{
    public class InvestmentProductExemption : Entity
    {
        public Guid InvestmentProductId { get; set; }

        public virtual InvestmentProduct InvestmentProduct { get; private set; }

        public byte CustomerClassification { get; set; }

        public decimal MaximumBalance { get; set; }

        public double AppraisalMultiplier { get; set; }
    }
}
