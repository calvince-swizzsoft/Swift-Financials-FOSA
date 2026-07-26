using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeLevyAgg
{
    public static class FixedDepositTypeLevySpecifications
    {
        public static Specification<FixedDepositTypeLevy> DefaultSpec()
        {
            Specification<FixedDepositTypeLevy> specification = new TrueSpecification<FixedDepositTypeLevy>();

            return specification;
        }

        public static ISpecification<FixedDepositTypeLevy> FixedDepositTypeLevyWithFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            Specification<FixedDepositTypeLevy> specification = new TrueSpecification<FixedDepositTypeLevy>();

            if (fixedDepositTypeId != null && fixedDepositTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<FixedDepositTypeLevy>(x => x.FixedDepositTypeId == fixedDepositTypeId);
            }

            return specification;
        }
    }
}
