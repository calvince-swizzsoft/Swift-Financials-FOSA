using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg
{
    public static class ReportSpecifications
    {
        public static Specification<Report> DefaultSpec()
        {
            Specification<Report> specification = new TrueSpecification<Report>();

            return specification;
        }

        public static ISpecification<Report> ParentReports()
        {
            Specification<Report> specification = new TrueSpecification<Report>();

            specification &= new DirectSpecification<Report>(c => c.ParentId == null);

            return specification;
        }
    }
}
