using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg
{
    public static class CommissionFactory
    {
        public static Commission CreateCommission(string description, decimal maximumCharge, int roundingType)
        {
            var commission = new Commission();

            commission.GenerateNewIdentity();

            commission.Description = description;

            commission.MaximumCharge = maximumCharge;

            commission.RoundingType = (byte)roundingType;

            commission.CreatedDate = DateTime.Now;

            return commission;
        }
    }
}
