using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg
{
    public static class AlternateChannelTypeCommissionFactory
    {
        public static AlternateChannelTypeCommission CreateAlternateChannelTypeCommission(int alternateChannelType, Guid commissionId, int alternateChannelTypeKnownChargeType, int chargeBenefactor)
        {
            var alternateChannelTypeCommission = new AlternateChannelTypeCommission();

            alternateChannelTypeCommission.GenerateNewIdentity();

            alternateChannelTypeCommission.AlternateChannelType = alternateChannelType;

            alternateChannelTypeCommission.CommissionId = commissionId;

            alternateChannelTypeCommission.KnownChargeType = alternateChannelTypeKnownChargeType;

            alternateChannelTypeCommission.ChargeBenefactor = (byte)chargeBenefactor;

            alternateChannelTypeCommission.CreatedDate = DateTime.Now;

            return alternateChannelTypeCommission;
        }
    }
}
