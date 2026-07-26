using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg
{
    public static class ReportTemplateFactory
    {
        public static ReportTemplate CreateReportTemplate(Guid? parentId, string description, int category, string spreadsheetCellReference)
        {
            var reportTemplate = new ReportTemplate();

            reportTemplate.GenerateNewIdentity();

            reportTemplate.ParentId = parentId;

            reportTemplate.Description = description;

            reportTemplate.Category = (short)category;

            reportTemplate.SpreadsheetCellReference = spreadsheetCellReference;

            reportTemplate.CreatedDate = DateTime.Now;

            return reportTemplate;
        }
    }
}
