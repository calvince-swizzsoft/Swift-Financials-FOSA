using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg
{
    public static class ReportTemplateEntryFactory
    {
        public static ReportTemplateEntry CreateReportTemplateEntry(Guid reportTemplateId, Guid chartOfAccountId)
        {
            var reportTemplateEntry = new ReportTemplateEntry();

            reportTemplateEntry.GenerateNewIdentity();

            reportTemplateEntry.ReportTemplateId = reportTemplateId;

            reportTemplateEntry.ChartOfAccountId = chartOfAccountId;

            reportTemplateEntry.CreatedDate = DateTime.Now;

            return reportTemplateEntry;
        }
    }
}
