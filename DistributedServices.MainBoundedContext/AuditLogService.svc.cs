using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogAppService _auditLogAppService;

        public AuditLogService(IAuditLogAppService auditLogAppService)
        {
            Guard.ArgumentNotNull(auditLogAppService, nameof(auditLogAppService));

            _auditLogAppService = auditLogAppService;
        }

        #region Audit Log

        public AuditLogDTO AddAuditLog(AuditLogDTO auditLogDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.AddNewAuditLog(auditLogDTO, serviceHeader);
        }

        public async Task<bool> AddAuditLogsAsync(List<AuditLogDTO> auditLogDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _auditLogAppService.AddNewAuditLogsAsync(auditLogDTOs, serviceHeader);
        }

        public AuditLogDTO FindAuditLog(Guid auditLogId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.FindAuditLog(auditLogId, serviceHeader);
        }

        public List<AuditLogDTO> FindAuditLogs()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.FindAuditLogs(serviceHeader);
        }

        public async Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _auditLogAppService.FindAuditLogsByDateRangeAndFilterAsync(pageIndex, pageSize, startDate, endDate, filter, serviceHeader);
        }

        public List<AuditLogDTO> FindAuditLogsByFilter(string filter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.FindAuditLogs(filter, serviceHeader);
        }

        public PageCollectionInfo<AuditLogDTO> FindAuditLogsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.FindAuditLogs(pageIndex, pageSize, serviceHeader);
        }

        #endregion

        #region Audit Trail

        public async Task<bool> AddAuditTrailsAsync(List<AuditTrailDTO> auditTrailDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _auditLogAppService.AddNewAuditTrailsAsync(auditTrailDTOs, serviceHeader);
        }

        public AuditTrailDTO AddAuditTrail(AuditTrailDTO auditTrailDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _auditLogAppService.AddNewAuditTrail(auditTrailDTO, serviceHeader);
        }

        public async Task<PageCollectionInfo<AuditTrailDTO>> FindAuditTrailsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _auditLogAppService.FindAuditTrailsByDateRangeAndFilterAsync(pageIndex, pageSize, startDate, endDate, filter, serviceHeader);
        }

        #endregion
    }
}
