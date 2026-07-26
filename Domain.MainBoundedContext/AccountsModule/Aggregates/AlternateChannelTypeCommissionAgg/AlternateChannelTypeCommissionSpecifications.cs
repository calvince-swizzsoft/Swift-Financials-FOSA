using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg
{
    public static class AlternateChannelTypeCommissionSpecifications
    {
        public static Specification<AlternateChannelTypeCommission> DefaultSpec()
        {
            Specification<AlternateChannelTypeCommission> specification = new TrueSpecification<AlternateChannelTypeCommission>();

            return specification;
        }

        public static ISpecification<AlternateChannelTypeCommission> AlternateChannelTypeCommission(int alternateChannelType, int alternateChannelKnownChargeType)
        {
            Specification<AlternateChannelTypeCommission> specification = new TrueSpecification<AlternateChannelTypeCommission>();

            specification &= new DirectSpecification<AlternateChannelTypeCommission>(x => x.AlternateChannelType == alternateChannelType && x.KnownChargeType == alternateChannelKnownChargeType);

            return specification;
        }
    }
}
