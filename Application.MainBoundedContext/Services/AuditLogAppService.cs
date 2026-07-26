using Application.MainBoundedContext.DTO;
using Application.Seedwork;
using Domain.MainBoundedContext.Aggregates.AuditLogAgg;
using Domain.MainBoundedContext.Aggregates.AuditTrailAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.Services
{
    public class AuditLogAppService : IAuditLogAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly IRepository<AuditTrail> _auditTrailRepository;

        public AuditLogAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<AuditLog> auditLogRepository,
           IRepository<AuditTrail> auditTrailRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (auditLogRepository == null)
                throw new ArgumentNullException(nameof(auditLogRepository));

            if (auditTrailRepository == null)
                throw new ArgumentNullException(nameof(auditTrailRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _auditLogRepository = auditLogRepository;
            _auditTrailRepository = auditTrailRepository;
        }

        public AuditLogDTO AddNewAuditLog(AuditLogDTO auditLogDTO, ServiceHeader serviceHeader)
        {
            if (auditLogDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var auditLog = AuditLogFactory.CreateAuditLog(auditLogDTO.EventType, auditLogDTO.TableName, auditLogDTO.RecordID, auditLogDTO.AdditionalNarration, serviceHeader);

                    _auditLogRepository.Add(auditLog, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return auditLog.ProjectedAs<AuditLogDTO>();
                }
            }
            else return null;
        }

        public bool AddNewAuditLogs(List<AuditLogDTO> auditLogDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (auditLogDTOs != null && auditLogDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var auditLogDTO in auditLogDTOs)
                    {
                        var auditLog = AuditLogFactory.CreateAuditLog(auditLogDTO.EventType, auditLogDTO.TableName, auditLogDTO.RecordID, auditLogDTO.AdditionalNarration, auditLogDTO.ApplicationUserName, auditLogDTO.EnvironmentUserName, auditLogDTO.EnvironmentMachineName, auditLogDTO.EnvironmentDomainName, auditLogDTO.EnvironmentOSVersion,
                            auditLogDTO.EnvironmentMACAddress, auditLogDTO.EnvironmentMotherboardSerialNumber, auditLogDTO.EnvironmentProcessorId, auditLogDTO.EnvironmentIPAddress, auditLogDTO.CreatedBy, auditLogDTO.CreatedDate);

                        _auditLogRepository.Add(auditLog, serviceHeader);
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }

        public async Task<bool> AddNewAuditLogsAsync(List<AuditLogDTO> auditLogDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var result = default(bool);

                if (auditLogDTOs != null && auditLogDTOs.Any())
                {
                    foreach (var auditLogDTO in auditLogDTOs)
                    {
                        var auditLog = AuditLogFactory.CreateAuditLog(auditLogDTO.EventType, auditLogDTO.TableName, auditLogDTO.RecordID, auditLogDTO.AdditionalNarration, auditLogDTO.ApplicationUserName, auditLogDTO.EnvironmentUserName, auditLogDTO.EnvironmentMachineName, auditLogDTO.EnvironmentDomainName, auditLogDTO.EnvironmentOSVersion,
                               auditLogDTO.EnvironmentMACAddress, auditLogDTO.EnvironmentMotherboardSerialNumber, auditLogDTO.EnvironmentProcessorId, auditLogDTO.EnvironmentIPAddress, auditLogDTO.CreatedBy, auditLogDTO.CreatedDate);

                        _auditLogRepository.Add(auditLog, serviceHeader);
                    }

                    result = await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                }

                return result;
            }
        }

        public AuditLogDTO FindAuditLog(Guid auditLogId, ServiceHeader serviceHeader)
        {
            if (auditLogId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var auditLog = _auditLogRepository.Get(auditLogId, serviceHeader);

                    if (auditLog != null)
                    {
                        return auditLog.ProjectedAs<AuditLogDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<AuditLogDTO> FindAuditLogs(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var auditLogs = _auditLogRepository.GetAll(serviceHeader);

                if (auditLogs != null && auditLogs.Any())
                {
                    return auditLogs.ProjectedAsCollection<AuditLogDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<AuditLogDTO> FindAuditLogs(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AuditLogSpecifications.DefaultSpec();

                ISpecification<AuditLog> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var auditLogPagedCollection = _auditLogRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (auditLogPagedCollection != null)
                {
                    var pageCollection = auditLogPagedCollection.PageCollection.ProjectedAsCollection<AuditLogDTO>();

                    var itemsCount = auditLogPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AuditLogDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AuditLogDTO> FindAuditLogsByDateRangeAndFilter(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader)
        {
            if (startDate != null && endDate != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = AuditLogSpecifications.AuditLogWithDateRangeAndFullText(startDate, endDate, text);

                    ISpecification<AuditLog> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var auditLogPagedCollection = _auditLogRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (auditLogPagedCollection != null)
                    {
                        var pageCollection = auditLogPagedCollection.PageCollection.ProjectedAsCollection<AuditLogDTO>();

                        var itemsCount = auditLogPagedCollection.ItemsCount;

                        return new PageCollectionInfo<AuditLogDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public async Task<PageCollectionInfo<AuditLogDTO>> FindAuditLogsByDateRangeAndFilterAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                PageCollectionInfo<AuditLogDTO> source = null; /*page items*/

                var filter = AuditLogSpecifications.AuditLogWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<AuditLog> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var auditLogPagedCollection = await _auditLogRepository.AllMatchingPagedAsync(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (auditLogPagedCollection != null)
                {
                    var pageCollection = auditLogPagedCollection.PageCollection.ProjectedAsCollection<AuditLogDTO>();

                    var itemsCount = auditLogPagedCollection.ItemsCount;

                    source = new PageCollectionInfo<AuditLogDTO>
                    {
                        PageCollection = pageCollection,
                        ItemsCount = itemsCount
                    };
                }
                return source;
            }
        }

        public List<AuditLogDTO> FindAuditLogs(string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AuditLogSpecifications.AuditLogFullText(text);

                ISpecification<AuditLog> spec = filter;

                var auditLogs = _auditLogRepository.AllMatching(spec, serviceHeader);

                if (auditLogs != null && auditLogs.Any())
                {
                    return auditLogs.ProjectedAsCollection<AuditLogDTO>();
                }
                else return null;
            }
        }

        #region Audit Trail

        public async Task<bool> AddNewAuditTrailsAsync(List<AuditTrailDTO> auditTrailDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var auditTrailDTO in auditTrailDTOs)
                {
                    var auditTrail = AuditTrailFactory.CreateAuditTrail(auditTrailDTO.EventType, auditTrailDTO.Activity, auditTrailDTO.CustomerId, auditTrailDTO.ApplicationUserName, auditTrailDTO.ApplicationUserDesignation, auditTrailDTO.EnvironmentUserName, auditTrailDTO.EnvironmentMachineName, auditTrailDTO.EnvironmentDomainName, auditTrailDTO.EnvironmentOSVersion,
                            auditTrailDTO.EnvironmentMACAddress, auditTrailDTO.EnvironmentMotherboardSerialNumber, auditTrailDTO.EnvironmentProcessorId, auditTrailDTO.EnvironmentIPAddress, serviceHeader);

                    _auditTrailRepository.Add(auditTrail, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public AuditTrailDTO AddNewAuditTrail(AuditTrailDTO auditTrailDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var auditTrail = AuditTrailFactory.CreateAuditTrail(auditTrailDTO.EventType, auditTrailDTO.Activity, auditTrailDTO.CustomerId, auditTrailDTO.ApplicationUserName, auditTrailDTO.ApplicationUserDesignation, auditTrailDTO.EnvironmentUserName, auditTrailDTO.EnvironmentMachineName, auditTrailDTO.EnvironmentDomainName, auditTrailDTO.EnvironmentOSVersion,
                        auditTrailDTO.EnvironmentMACAddress, auditTrailDTO.EnvironmentMotherboardSerialNumber, auditTrailDTO.EnvironmentProcessorId, auditTrailDTO.EnvironmentIPAddress, serviceHeader);

                _auditTrailRepository.Add(auditTrail, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) > 0 ? auditTrail.ProjectedAs<AuditTrailDTO>() : null;
            }
        }

        public async Task<PageCollectionInfo<AuditTrailDTO>> FindAuditTrailsByDateRangeAndFilterAsync(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AuditTrailSpecifications.AuditTrailWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<AuditTrail> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _auditTrailRepository.AllMatchingPagedAsync<AuditTrailDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        #endregion
    }
}
