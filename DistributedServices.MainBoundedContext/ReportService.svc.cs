using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
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
    public class ReportService : IReportService
    {
        private readonly IReportAppService _reportAppService;

        public ReportService(
            IReportAppService reportAppService)
        {
            Guard.ArgumentNotNull(reportAppService, nameof(reportAppService));

            _reportAppService = reportAppService;
        }

        #region Report

        public ReportDTO AddReport(ReportDTO reportDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportAppService.AddNewReport(reportDTO, serviceHeader);
        }

        public bool UpdateReport(ReportDTO reportDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportAppService.UpdateReport(reportDTO, serviceHeader);
        }

        public List<ReportDTO> FindReports(bool updateDepth, bool traverseTree)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportAppService.FindReports(updateDepth, traverseTree, serviceHeader);
        }

        public ReportDTO FindReport(Guid reportId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _reportAppService.FindReport(reportId, serviceHeader);
        }

        #endregion
    }
}
