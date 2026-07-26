using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeGraduatedScaleAgg
{
    public static class FixedDepositTypeGraduatedScaleSpecifications
    {
        public static Specification<FixedDepositTypeGraduatedScale> DefaultSpec()
        {
            Specification<FixedDepositTypeGraduatedScale> specification = new TrueSpecification<FixedDepositTypeGraduatedScale>();

            return specification;
        }

        public static ISpecification<FixedDepositTypeGraduatedScale> GraduatedScaleWithFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            Specification<FixedDepositTypeGraduatedScale> specification = new TrueSpecification<FixedDepositTypeGraduatedScale>();

            if (fixedDepositTypeId != null && fixedDepositTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<FixedDepositTypeGraduatedScale>(x => x.FixedDepositTypeId == fixedDepositTypeId);
            }

            return specification;
        }
    }
}
