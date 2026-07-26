using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAuditLogService
    {
        #region Audit Log

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AuditLogDTO AddAuditLog(AuditLogDTO auditLogDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AddAuditLogsAsync(List<AuditLogDTO> auditLogDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AuditLogDTO FindAuditLog(Guid auditLogId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AuditLogDTO> FindAuditLogs();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AuditLogDTO> FindAuditLogsByFilter(string filter);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AuditLogDTO> FindAuditLogsInPage(int pageIndex, int pageSize);

        #endregion

        #region Audit Trail

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> AddAuditTrailsAsync(List<AuditTrailDTO> auditTrailDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AuditTrailDTO AddAuditTrail(AuditTrailDTO auditTrailDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<AuditTrailDTO>> FindAuditTrailsByDateRangeAndFilterInPageAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter);

        #endregion
    }
}
