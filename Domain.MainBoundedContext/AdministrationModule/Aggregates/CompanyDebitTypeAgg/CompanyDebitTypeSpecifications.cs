using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyDebitTypeAgg
{
    public static class CompanyDebitTypeSpecifications
    {
        public static Specification<CompanyDebitType> DefaultSpec()
        {
            Specification<CompanyDebitType> specification = new TrueSpecification<CompanyDebitType>();

            return specification;
        }

        public static ISpecification<CompanyDebitType> CompanyDebitTypeWithCompanyId(Guid companyId)
        {
            Specification<CompanyDebitType> specification = new TrueSpecification<CompanyDebitType>();

            if (companyId != null && companyId != Guid.Empty)
            {
                specification &= new DirectSpecification<CompanyDebitType>(x => x.CompanyId == companyId);
            }

            return specification;
        }
    }
}
