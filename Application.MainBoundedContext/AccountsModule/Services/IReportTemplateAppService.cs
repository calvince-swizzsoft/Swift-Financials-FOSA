using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IReportTemplateAppService
    {
        ReportTemplateDTO AddNewReportTemplate(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader);

        bool UpdateReportTemplate(ReportTemplateDTO reportTemplateDTO, ServiceHeader serviceHeader);

        List<ReportTemplateDTO> FindReportTemplates(ServiceHeader serviceHeader);

        List<ReportTemplateDTO> FindReportTemplates(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true);

        List<ReportTemplateDTO> FindReportTemplateBalances(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter, ServiceHeader serviceHeader);

        ReportTemplateDTO PopulateReportTemplate(ReportTemplateDTO reportTemplateDTO, List<ReportTemplateDTO> templateBalances, string fileUploadDirectory, ServiceHeader serviceHeader);

        ReportTemplateDTO FindReportTemplate(Guid reportTemplateId, ServiceHeader serviceHeader);

        List<ReportTemplateEntryDTO> FindReportTemplateEntries(Guid reportTemplateId, ServiceHeader serviceHeader);

        bool UpdateReportTemplateEntries(Guid reportTemplateId, List<ReportTemplateEntryDTO> reportTemplateEntries, ServiceHeader serviceHeader);
    }
}
