using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg
{
    public static class ReportTemplateSpecifications
    {
        public static Specification<ReportTemplate> DefaultSpec()
        {
            Specification<ReportTemplate> specification = new TrueSpecification<ReportTemplate>();

            return specification;
        }

        public static ISpecification<ReportTemplate> ParentReportTemplates()
        {
            Specification<ReportTemplate> specification = new TrueSpecification<ReportTemplate>();

            specification &= new DirectSpecification<ReportTemplate>(c => c.ParentId == null);

            return specification;
        }
    }
}
