using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg
{
    public static class WireTransferTypeCommissionSpecifications
    {
        public static Specification<WireTransferTypeCommission> DefaultSpec()
        {
            Specification<WireTransferTypeCommission> specification = new TrueSpecification<WireTransferTypeCommission>();

            return specification;
        }

        public static ISpecification<WireTransferTypeCommission> WireTransferTypeCommissionWithWireTransferTypeId(Guid wireTransferTypeId)
        {
            Specification<WireTransferTypeCommission> specification = new TrueSpecification<WireTransferTypeCommission>();

            if (wireTransferTypeId != null && wireTransferTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<WireTransferTypeCommission>(x => x.WireTransferTypeId == wireTransferTypeId);
            }

            return specification;
        }
    }
}
