using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg
{
    public static class CompanyAttachedProductSpecifications
    {
        public static Specification<CompanyAttachedProduct> DefaultSpec()
        {
            Specification<CompanyAttachedProduct> specification = new TrueSpecification<CompanyAttachedProduct>();

            return specification;
        }

        public static ISpecification<CompanyAttachedProduct> CompanyAttachedProductWithCompanyId(Guid companyId)
        {
            Specification<CompanyAttachedProduct> specification = new TrueSpecification<CompanyAttachedProduct>();

            if (companyId != null && companyId != Guid.Empty)
            {
                specification &= new DirectSpecification<CompanyAttachedProduct>(x => x.CompanyId == companyId);
            }

            return specification;
        }

        public static ISpecification<CompanyAttachedProduct> CompanyAttachedProductWithTargetProductId(Guid targetProductId)
        {
            Specification<CompanyAttachedProduct> specification = new TrueSpecification<CompanyAttachedProduct>();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<CompanyAttachedProduct>(x => x.TargetProductId == targetProductId);
            }

            return specification;
        }
    }
}
