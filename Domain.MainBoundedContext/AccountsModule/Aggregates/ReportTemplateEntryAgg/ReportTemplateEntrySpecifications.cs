using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg
{
    public static class ReportTemplateEntrySpecifications
    {
        public static Specification<ReportTemplateEntry> DefaultSpec()
        {
            Specification<ReportTemplateEntry> specification = new TrueSpecification<ReportTemplateEntry>();

            return specification;
        }

        public static Specification<ReportTemplateEntry> ReportTemplateEntryWithReportTemplateId(Guid reportTemplateId)
        {
            Specification<ReportTemplateEntry> specification = DefaultSpec();

            if (reportTemplateId != null && reportTemplateId != Guid.Empty)
            {
                var reportTemplateIdSpec = new DirectSpecification<ReportTemplateEntry>(c => c.ReportTemplateId == reportTemplateId);

                specification &= reportTemplateIdSpec;
            }

            return specification;
        }
    }
}
