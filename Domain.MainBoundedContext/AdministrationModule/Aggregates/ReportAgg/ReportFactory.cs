using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ReportAgg
{
    public static class ReportFactory
    {
        public static Report CreateReport(Guid? parentId, string reportName, string reportPath, string reportQuery, int category)
        {
            var report = new Report();

            report.GenerateNewIdentity();

            report.ParentId = parentId;

            report.ReportName = reportName;

            report.ReportPath = reportPath;

            report.ReportQuery = reportQuery;

            report.Category = category;

            report.CreatedDate = DateTime.Now;

            return report;
        }
    }
}
