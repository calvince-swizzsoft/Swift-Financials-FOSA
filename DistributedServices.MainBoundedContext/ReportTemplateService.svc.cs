using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly IReportTemplateAppService _reportTemplateAppService;

        public ReportTemplateService(
           IReportTemplateAppService reportTemplateAppService)
        {
            Guard.ArgumentNotNull(reportTemplateAppService, nameof(reportTemplateAppService));

            _reportTemplateAppService = reportTemplateAppService;
        }

        #region Report Template

        public ReportTemplateDTO AddReportTemplate(ReportTemplateDTO reportTemplateDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.AddNewReportTemplate(reportTemplateDTO, serviceHeader);
        }

        public bool UpdateReportTemplate(ReportTemplateDTO reportTemplateDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.UpdateReportTemplate(reportTemplateDTO, serviceHeader);
        }

        public ReportTemplateDTO FindReportTemplate(Guid reportTemplateId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.FindReportTemplate(reportTemplateId, serviceHeader);
        }

        public List<ReportTemplateDTO> FindReportTemplates(bool updateDepth, bool traverseTree)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.FindReportTemplates(serviceHeader, updateDepth, traverseTree);
        }

        public List<ReportTemplateDTO> FindReportTemplateBalances(Guid rootTemplateId, DateTime cutOffDate, int transactionDateFilter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.FindReportTemplateBalances(rootTemplateId, cutOffDate, transactionDateFilter, serviceHeader);
        }

        public ReportTemplateDTO PopulateReportTemplate(ReportTemplateDTO reportTemplateDTO, List<ReportTemplateDTO> templateBalances)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _reportTemplateAppService.PopulateReportTemplate(reportTemplateDTO, templateBalances, serviceBrokerSettingsElement.FileUploadDirectory, serviceHeader);
        }

        public List<ReportTemplateEntryDTO> FindReportTemplateEntriesByReportTemplateId(Guid reportTemplateId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.FindReportTemplateEntries(reportTemplateId, serviceHeader);
        }

        public bool UpdateReportTemplateEntries(Guid reportTemplateId, List<ReportTemplateEntryDTO> reportTemplateEntries)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportTemplateAppService.UpdateReportTemplateEntries(reportTemplateId, reportTemplateEntries, serviceHeader);
        }

        #endregion
    }
}
