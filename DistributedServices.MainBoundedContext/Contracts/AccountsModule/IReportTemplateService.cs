using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IReportTemplateService
    {
        #region Report Template

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReportTemplateDTO AddReportTemplate(ReportTemplateDTO reportTemplateDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateReportTemplate(ReportTemplateDTO reportTemplateDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReportTemplateDTO FindReportTemplate(Guid reportTemplateId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReportTemplateDTO> FindReportTemplates(bool updateDepth, bool traverseTree);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReportTemplateDTO> FindReportTemplateBalances(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ReportTemplateEntryDTO> FindReportTemplateEntriesByReportTemplateId(Guid reportTemplateId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateReportTemplateEntries(Guid reportTemplateId, List<ReportTemplateEntryDTO> reportTemplateEntries);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ReportTemplateDTO PopulateReportTemplate(ReportTemplateDTO reportTemplateDTO, List<ReportTemplateDTO> templateBalances);

        #endregion
    }
}
