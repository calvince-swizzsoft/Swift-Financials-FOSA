using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchDiscrepancyAgg
{
    public static class OverDeductionBatchDiscrepancySpecifications
    {
        public static Specification<OverDeductionBatchDiscrepancy> DefaultSpec()
        {
            Specification<OverDeductionBatchDiscrepancy> specification = new TrueSpecification<OverDeductionBatchDiscrepancy>();

            return specification;
        }
    }
}
