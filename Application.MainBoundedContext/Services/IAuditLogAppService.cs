using Application.MainBoundedContext.DTO;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.Services
{
    public interface IAuditLogAppService
    {
        AuditLogDTO AddNewAuditLog(AuditLogDTO auditLogDTO, ServiceHeader serviceHeader);

        bool AddNewAuditLogs(List<AuditLogDTO> auditLogDTOs, ServiceHeader serviceHeader);

        Task<bool> AddNewAuditLogsAsync(List<AuditLogDTO> auditLogDTOs, ServiceHeader serviceHeader);

        AuditLogDTO FindAuditLog(Guid auditLogId, ServiceHeader serviceHeader);

        List<AuditLogDTO> FindAuditLogs(ServiceHeader serviceHeader);

        PageCollectionInfo<AuditLogDTO> FindAuditLogs(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<AuditLogDTO> FindAuditLogsByDateRangeAndFilter(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsByDateRangeAndFilterAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader);

        List<AuditLogDTO> FindAuditLogs(string text, ServiceHeader serviceHeader);

        #region AuditTrailDTO

        Task<bool> AddNewAuditTrailsAsync(List<AuditTrailDTO> auditTrailDTOs, ServiceHeader serviceHeader);

        AuditTrailDTO AddNewAuditTrail(AuditTrailDTO auditTrailDTO, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<AuditTrailDTO>> FindAuditTrailsByDateRangeAndFilterAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader);

        #endregion
    }
}
