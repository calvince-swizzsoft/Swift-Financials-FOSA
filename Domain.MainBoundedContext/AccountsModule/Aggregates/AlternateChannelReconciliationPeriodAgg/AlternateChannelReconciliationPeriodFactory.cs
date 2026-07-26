using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg
{
    public static class AlternateChannelReconciliationPeriodFactory
    {
        public static AlternateChannelReconciliationPeriod CreateAlternateChannelReconciliationPeriod(int alternateChannelType, Duration duration, int setDifferenceMode, string remarks)
        {
            var alternateChannelReconciliationPeriod = new AlternateChannelReconciliationPeriod();

            alternateChannelReconciliationPeriod.GenerateNewIdentity();

            alternateChannelReconciliationPeriod.AlternateChannelType = (byte)alternateChannelType;

            alternateChannelReconciliationPeriod.Duration = duration;

            alternateChannelReconciliationPeriod.SetDifferenceMode = (byte)setDifferenceMode;

            alternateChannelReconciliationPeriod.Remarks = remarks;

            alternateChannelReconciliationPeriod.CreatedDate = DateTime.Now;

            return alternateChannelReconciliationPeriod;
        }
    }
}
