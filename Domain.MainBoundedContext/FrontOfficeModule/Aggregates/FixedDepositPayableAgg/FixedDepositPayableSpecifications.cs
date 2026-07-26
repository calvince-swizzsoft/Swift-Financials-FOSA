using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositPayableAgg
{
    public static class FixedDepositPayableSpecifications
    {
        public static Specification<FixedDepositPayable> DefaultSpec()
        {
            Specification<FixedDepositPayable> specification = new TrueSpecification<FixedDepositPayable>();

            return specification;
        }

        public static Specification<FixedDepositPayable> FixedDepositPayableWithFixedDepositId(Guid fixedDepositId)
        {
            Specification<FixedDepositPayable> specification = new DirectSpecification<FixedDepositPayable>(c => c.FixedDepositId == fixedDepositId);

            return specification;
        }
    }
}
