using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class JournalReversalBatchAppService : IJournalReversalBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<JournalReversalBatch> _journalReversalBatchRepository;
        private readonly IRepository<JournalReversalBatchEntry> _journalReversalBatchEntryRepository;
        private readonly IJournalAppService _journalAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IBrokerService _brokerService;

        public JournalReversalBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<JournalReversalBatch> journalReversalBatchRepository,
           IRepository<JournalReversalBatchEntry> journalReversalBatchEntryRepository,
           IJournalAppService journalAppService,
           ISqlCommandAppService sqlCommandAppService,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (journalReversalBatchRepository == null)
                throw new ArgumentNullException(nameof(journalReversalBatchRepository));

            if (journalReversalBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(journalReversalBatchEntryRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _journalReversalBatchRepository = journalReversalBatchRepository;
            _journalReversalBatchEntryRepository = journalReversalBatchEntryRepository;
            _journalAppService = journalAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _brokerService = brokerService;
        }

        public JournalReversalBatchDTO AddNewJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var journalReversalBatch = JournalReversalBatchFactory.CreateJournalReversalBatch(journalReversalBatchDTO.BranchId, journalReversalBatchDTO.Remarks, journalReversalBatchDTO.Priority);

                    journalReversalBatch.BatchNumber = _journalReversalBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}JournalReversalBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    journalReversalBatch.Status = (int)BatchStatus.Pending;
                    journalReversalBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _journalReversalBatchRepository.Add(journalReversalBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return journalReversalBatch.ProjectedAs<JournalReversalBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateJournalReversalBatch(JournalReversalBatchDTO dto, ServiceHeader header)
        {
            if (dto == null || dto.Id == Guid.Empty) return false;
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var batch = _journalReversalBatchRepository.Get(dto.Id, header);
                if (batch == null || batch.Status != (int)BatchStatus.Pending) return false;
                batch.Remarks = dto.Remarks;
                batch.Priority = (byte)dto.Priority;
                return scope.SaveChanges(header) >= 0;
            }
        }

        public JournalReversalBatchEntryDTO AddNewJournalReversalBatchEntry(JournalReversalBatchEntryDTO dto, ServiceHeader header)
        {
            if (dto == null || dto.JournalId == Guid.Empty) return null;
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var batch = _journalReversalBatchRepository.Get(dto.JournalReversalBatchId, header);
                if (batch == null || batch.Status != (int)BatchStatus.Pending) return null;
                var journal = _journalAppService.FindJournal(dto.JournalId, header);
                if (journal == null || journal.IsLocked) return null;
                var existing = FindJournalReversalBatchEntriesByJournalReversalBatchId(batch.Id, header);
                if (existing != null && existing.Any(item => item.JournalId == dto.JournalId)) return null;
                var entry = JournalReversalBatchEntryFactory.CreateJournalReversalBatchEntry(batch.Id, dto.JournalId, dto.Remarks);
                entry.Status = (int)BatchEntryStatus.Pending;
                entry.CreatedBy = header.ApplicationUserName;
                _journalReversalBatchEntryRepository.Add(entry, header);
                scope.SaveChanges(header);
                return entry.ProjectedAs<JournalReversalBatchEntryDTO>();
            }
        }

        public bool RemoveJournalReversalBatchEntries(List<JournalReversalBatchEntryDTO> dtos, ServiceHeader header)
        {
            if (dtos == null || dtos.Any(item => item == null || item.Id == Guid.Empty)) return false;
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var entries = dtos.Select(item => item.Id).Distinct().OrderBy(id => id)
                    .Select(id => _journalReversalBatchEntryRepository.Get(id, header)).ToList();
                if (entries.Any(item => item == null || item.Status != (int)BatchEntryStatus.Pending)) return false;
                foreach (var id in entries.Select(item => item.JournalReversalBatchId).Distinct().OrderBy(id => id))
                {
                    var batch = _journalReversalBatchRepository.Get(id, header);
                    if (batch == null || batch.Status != (int)BatchStatus.Pending) return false;
                }
                entries.ForEach(item => _journalReversalBatchEntryRepository.Remove(item, header));
                return scope.SaveChanges(header) >= 0;
            }
        }

        public bool AuditJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _journalReversalBatchRepository.Get(journalReversalBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Audited;
                        persisted.AuditRemarks = journalReversalBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = journalReversalBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeJournalReversalBatch(JournalReversalBatchDTO journalReversalBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (journalReversalBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _journalReversalBatchRepository.Get(journalReversalBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Posted;
                        persisted.AuthorizationRemarks = journalReversalBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;
                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = journalReversalBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) > 0;

                if (result && batchAuthOption == (int)BatchAuthOption.Post)
                {
                    var query = _journalReversalBatchEntryRepository.DatabaseSqlQuery<Guid>(string.Format(
                          @"SELECT Id
                            FROM  {0}JournalReversalBatchEntries
                            WHERE(JournalReversalBatchId = @JournalReversalBatchId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                            new SqlParameter("JournalReversalBatchId", journalReversalBatchDTO.Id));

                    if (query != null)
                    {
                        var data = from l in query
                                   select new JournalReversalBatchEntryDTO
                                   {
                                       Id = l,
                                       JournalReversalBatchPriority = journalReversalBatchDTO.Priority
                                   };

                        _brokerService.ProcessJournalReversalBatchEntries(DMLCommand.None, serviceHeader, data.ToArray());
                    }
                }
            }

            return result;
        }

        public List<JournalReversalBatchDTO> FindJournalReversalBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var journalReversalBatchs = _journalReversalBatchRepository.GetAll(serviceHeader);

                if (journalReversalBatchs != null && journalReversalBatchs.Any())
                {
                    return journalReversalBatchs.ProjectedAsCollection<JournalReversalBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<JournalReversalBatchDTO> FindJournalReversalBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalReversalBatchSpecifications.JournalReversalBatchWithStatus(status, startDate, endDate, text);

                ISpecification<JournalReversalBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalReversalBatchPagedCollection = _journalReversalBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalReversalBatchPagedCollection != null)
                {
                    var pageCollection = journalReversalBatchPagedCollection.PageCollection.ProjectedAsCollection<JournalReversalBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _journalReversalBatchEntryRepository.AllMatchingCount(JournalReversalBatchEntrySpecifications.JournalReversalBatchEntryWithJournalReversalBatchId(item.Id, null), serviceHeader);

                            var postedItems = _journalReversalBatchEntryRepository.AllMatchingCount(JournalReversalBatchEntrySpecifications.PostedJournalReversalBatchEntryWithJournalReversalBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = journalReversalBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalReversalBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public JournalReversalBatchDTO FindJournalReversalBatch(Guid journalReversalBatchId, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var journalReversalBatch = _journalReversalBatchRepository.Get(journalReversalBatchId, serviceHeader);

                    if (journalReversalBatch != null)
                    {
                        return journalReversalBatch.ProjectedAs<JournalReversalBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public JournalReversalBatchEntryDTO FindJournalReversalBatchEntry(Guid journalReversalBatchEntryId, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var journalReversalBatchEntry = _journalReversalBatchEntryRepository.Get(journalReversalBatchEntryId, serviceHeader);

                    if (journalReversalBatchEntry != null)
                    {
                        return journalReversalBatchEntry.ProjectedAs<JournalReversalBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchId != null && journalReversalBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalReversalBatchEntrySpecifications.JournalReversalBatchEntryWithJournalReversalBatchId(journalReversalBatchId, null);

                    ISpecification<JournalReversalBatchEntry> spec = filter;

                    var journalReversalBatchEntries = _journalReversalBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (journalReversalBatchEntries != null && journalReversalBatchEntries.Any())
                    {
                        return journalReversalBatchEntries.ProjectedAsCollection<JournalReversalBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalReversalBatchEntryDTO> FindJournalReversalBatchEntriesByJournalReversalBatchId(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchId != null && journalReversalBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalReversalBatchEntrySpecifications.JournalReversalBatchEntryWithJournalReversalBatchId(journalReversalBatchId, text);

                    ISpecification<JournalReversalBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var journalReversalBatchEntryPagedCollection = _journalReversalBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalReversalBatchEntryPagedCollection != null)
                    {
                        var pageCollection = journalReversalBatchEntryPagedCollection.PageCollection.ProjectedAsCollection<JournalReversalBatchEntryDTO>();

                        var itemsCount = journalReversalBatchEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<JournalReversalBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalEntryDTO> FindJournalEntriesByJournalReversalBatchId(Guid journalReversalBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchId != null && journalReversalBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var batchEntryFilter = JournalReversalBatchEntrySpecifications.JournalReversalBatchEntryWithJournalReversalBatchId(journalReversalBatchId, null);

                    ISpecification<JournalReversalBatchEntry> batchEntrySpec = batchEntryFilter;

                    var journalReversalBatchEntryCollection = _journalReversalBatchEntryRepository.AllMatching(batchEntrySpec, serviceHeader);

                    if (journalReversalBatchEntryCollection != null)
                    {
                        var batchEntryProjection = journalReversalBatchEntryCollection.ProjectedAsCollection<JournalReversalBatchEntryDTO>();

                        var journalProjection = (from p in batchEntryProjection select p.JournalId);

                        var journalIdList = new List<Guid>();

                        foreach (var item in journalProjection)
                            journalIdList.Add(item);

                        if (journalIdList.Any())
                        {
                            return _journalAppService.FindJournalEntries(pageIndex, pageSize, serviceHeader, journalIdList.ToArray());
                        }
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateJournalReversalBatchEntries(Guid batchId, List<JournalReversalBatchEntryDTO> dtos, ServiceHeader header)
        {
            if (dtos == null || dtos.Any(item => item == null || item.JournalId == Guid.Empty)) return false;
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var batch = _journalReversalBatchRepository.Get(batchId, header);
                if (batch == null || batch.Status != (int)BatchStatus.Pending) return false;
                var existing = FindJournalReversalBatchEntriesByJournalReversalBatchId(batchId, header) ?? new List<JournalReversalBatchEntryDTO>();
                var additions = dtos.GroupBy(item => item.JournalId).Select(group => group.First())
                    .Where(item => !existing.Any(current => current.JournalId == item.JournalId)).ToList();
                foreach (var dto in additions)
                {
                    var journal = _journalAppService.FindJournal(dto.JournalId, header);
                    if (journal == null || journal.IsLocked) return false;
                }
                foreach (var dto in additions)
                {
                    var entry = JournalReversalBatchEntryFactory.CreateJournalReversalBatchEntry(batchId, dto.JournalId, dto.Remarks);
                    entry.Status = (int)BatchEntryStatus.Pending;
                    entry.CreatedBy = header.ApplicationUserName;
                    _journalReversalBatchEntryRepository.Add(entry, header);
                }
                return scope.SaveChanges(header) >= 0;
            }
        }

        public PageCollectionInfo<JournalReversalBatchEntryDTO> FindQueableJournalReversalBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalReversalBatchEntrySpecifications.QueableJournalReversalBatchEntries();

                ISpecification<JournalReversalBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalReversalBatchPagedCollection = _journalReversalBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalReversalBatchPagedCollection != null)
                {
                    var pageCollection = journalReversalBatchPagedCollection.PageCollection.ProjectedAsCollection<JournalReversalBatchEntryDTO>();

                    var itemsCount = journalReversalBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalReversalBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool PostJournalReversalBatchEntry(Guid journalReversalBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (journalReversalBatchEntryId == Guid.Empty) return false;
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var entry = _journalReversalBatchEntryRepository.Get(journalReversalBatchEntryId, serviceHeader);
                if (entry == null) return false;
                var batch = _journalReversalBatchRepository.Get(entry.JournalReversalBatchId, serviceHeader);
                if (batch == null || batch.Status != (int)BatchStatus.Posted) return false;
                // Duplicate delivery after a successful commit is an acknowledged
                // no-op. A failed attempt leaves Pending and is safe to retry.
                if (entry.Status == (int)BatchEntryStatus.Posted) return true;
                if (entry.Status != (int)BatchEntryStatus.Pending || !entry.JournalId.HasValue) return false;
                var journal = _journalAppService.FindJournal(entry.JournalId.Value, serviceHeader);
                if (journal == null || journal.IsLocked) return false;
                var previousUser = serviceHeader.ApplicationUserName;
                try
                {
                    serviceHeader.ApplicationUserName = batch.AuthorizedBy ?? previousUser;
                    var description = string.Format("B#{0}~{1}", batch.BatchNumber.ToString().PadLeft(7, '0'), batch.Remarks);
                    if (!_journalAppService.ReverseJournals(new List<JournalDTO> { journal }, description, moduleNavigationItemCode, serviceHeader))
                        return false;
                    entry.Status = (int)BatchEntryStatus.Posted;
                    return scope.SaveChanges(serviceHeader) >= 0;
                }
                finally { serviceHeader.ApplicationUserName = previousUser; }
            }
        }
    }
}
