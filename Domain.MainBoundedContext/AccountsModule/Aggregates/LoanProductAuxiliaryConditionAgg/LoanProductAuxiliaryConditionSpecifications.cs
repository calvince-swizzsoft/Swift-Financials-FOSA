using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg
{
    public static class LoanProductAuxiliaryConditionSpecifications
    {
        public static Specification<LoanProductAuxiliaryCondition> DefaultSpec()
        {
            Specification<LoanProductAuxiliaryCondition> specification = new TrueSpecification<LoanProductAuxiliaryCondition>();

            return specification;
        }

        public static ISpecification<LoanProductAuxiliaryCondition> LoanProductAuxiliaryConditionWithBaseLoanProductId(Guid baseLoanProductId)
        {
            Specification<LoanProductAuxiliaryCondition> specification = new TrueSpecification<LoanProductAuxiliaryCondition>();

            if (baseLoanProductId != null && baseLoanProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<LoanProductAuxiliaryCondition>(x => x.BaseLoanProductId == baseLoanProductId);
            }

            return specification;
        }
    }
}
