using Application.MainBoundedContext.DTO;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts
{
    [ServiceContract(Name = "IAuditLogService")]
    public interface IAuditLogService
    {
        #region AuditLogDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAuditLog(AuditLogDTO auditLogDTO, AsyncCallback callback, Object state);
        AuditLogDTO EndAddAuditLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAuditLogs(List<AuditLogDTO> auditLogDTOs, AsyncCallback callback, Object state);
        bool EndAddAuditLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditLog(Guid auditLogId, AsyncCallback callback, Object state);
        AuditLogDTO EndFindAuditLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditLogs(AsyncCallback callback, Object state);
        List<AuditLogDTO> EndFindAuditLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditLogsByDateRangeAndFilterInPage(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter, AsyncCallback callback, Object state);
        PageCollectionInfo<AuditLogDTO> EndFindAuditLogsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditLogsByFilter(string filter, AsyncCallback callback, Object state);
        List<AuditLogDTO> EndFindAuditLogsByFilter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditLogsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AuditLogDTO> EndFindAuditLogsInPage(IAsyncResult result);

        #endregion

        #region AuditTrailDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAuditTrails(List<AuditTrailDTO> auditTrailDTOs, AsyncCallback callback, Object state);
        bool EndAddAuditTrails(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAuditTrail(AuditTrailDTO auditTrailDTO, AsyncCallback callback, Object state);
        AuditTrailDTO EndAddAuditTrail(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAuditTrailsByDateRangeAndFilterInPage(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string filter, AsyncCallback callback, Object state);
        PageCollectionInfo<AuditTrailDTO> EndFindAuditTrailsByDateRangeAndFilterInPage(IAsyncResult result);

        #endregion
    }
}
