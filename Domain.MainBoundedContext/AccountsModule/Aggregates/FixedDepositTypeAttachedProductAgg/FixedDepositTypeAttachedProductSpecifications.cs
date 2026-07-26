using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAttachedProductAgg
{
    public static class FixedDepositTypeAttachedProductSpecifications
    {
        public static Specification<FixedDepositTypeAttachedProduct> DefaultSpec()
        {
            Specification<FixedDepositTypeAttachedProduct> specification = new TrueSpecification<FixedDepositTypeAttachedProduct>();

            return specification;
        }

        public static ISpecification<FixedDepositTypeAttachedProduct> FixedDepositTypeAttachedProductWithFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            Specification<FixedDepositTypeAttachedProduct> specification = new TrueSpecification<FixedDepositTypeAttachedProduct>();

            if (fixedDepositTypeId != null && fixedDepositTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<FixedDepositTypeAttachedProduct>(x => x.FixedDepositTypeId == fixedDepositTypeId);
            }

            return specification;
        }

        public static ISpecification<FixedDepositTypeAttachedProduct> FixedDepositTypeAttachedProductWithTargetProductId(Guid targetProductId)
        {
            Specification<FixedDepositTypeAttachedProduct> specification = new TrueSpecification<FixedDepositTypeAttachedProduct>();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<FixedDepositTypeAttachedProduct>(x => x.TargetProductId == targetProductId);
            }

            return specification;
        }
    }
}
