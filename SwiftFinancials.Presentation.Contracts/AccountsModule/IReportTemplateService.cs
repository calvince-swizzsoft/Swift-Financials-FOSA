using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IReportTemplateService")]
    public interface IReportTemplateService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddReportTemplate(ReportTemplateDTO reportTemplateDTO, AsyncCallback callback, Object state);
        ReportTemplateDTO EndAddReportTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateReportTemplate(ReportTemplateDTO reportTemplateDTO, AsyncCallback callback, Object state);
        bool EndUpdateReportTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportTemplate(Guid reportTemplateId, AsyncCallback callback, Object state);
        ReportTemplateDTO EndFindReportTemplate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportTemplates(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<ReportTemplateDTO> EndFindReportTemplates(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportTemplateBalances(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter, AsyncCallback callback, Object state);
        List<ReportTemplateDTO> EndFindReportTemplateBalances(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindReportTemplateEntriesByReportTemplateId(Guid reportTemplateId, AsyncCallback callback, Object state);
        List<ReportTemplateEntryDTO> EndFindReportTemplateEntriesByReportTemplateId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateReportTemplateEntries(Guid reportTemplateId, List<ReportTemplateEntryDTO> reportTemplateEntries, AsyncCallback callback, Object state);
        bool EndUpdateReportTemplateEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPopulateReportTemplate(ReportTemplateDTO reportTemplateDTO, List<ReportTemplateDTO> templateBalances, AsyncCallback callback, Object state);
        ReportTemplateDTO EndPopulateReportTemplate(IAsyncResult result);
    }
}
