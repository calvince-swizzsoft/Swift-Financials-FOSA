using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using KBCsv;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class AlternateChannelReconciliationPeriodAppService : IAlternateChannelReconciliationPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AlternateChannelReconciliationPeriod> _alternateChannelReconciliationPeriodRepository;
        private readonly IRepository<AlternateChannelReconciliationEntry> _alternateChannelReconciliationEntryRepository;
        private readonly IRepository<AlternateChannelLog> _alternateChannelLogRepository;
        private readonly IRepository<Journal> _journalRepository;
        private readonly IRepository<JournalEntry> _journalEntryRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        private readonly NumberFormatInfo _nfi;

        public AlternateChannelReconciliationPeriodAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<AlternateChannelReconciliationPeriod> alternateChannelReconciliationPeriodRepository,
           IRepository<AlternateChannelReconciliationEntry> alternateChannelReconciliationEntryRepository,
           IRepository<AlternateChannelLog> alternateChannelLogRepository,
           IRepository<Journal> journalRepository,
           IRepository<JournalEntry> journalEntryRepository,
           IChartOfAccountAppService chartOfAccountAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (alternateChannelReconciliationPeriodRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelReconciliationPeriodRepository));

            if (alternateChannelReconciliationEntryRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelReconciliationEntryRepository));

            if (alternateChannelLogRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelLogRepository));

            if (journalRepository == null)
                throw new ArgumentNullException(nameof(journalRepository));

            if (journalEntryRepository == null)
                throw new ArgumentNullException(nameof(journalEntryRepository));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _alternateChannelReconciliationPeriodRepository = alternateChannelReconciliationPeriodRepository;
            _alternateChannelReconciliationEntryRepository = alternateChannelReconciliationEntryRepository;
            _alternateChannelLogRepository = alternateChannelLogRepository;
            _journalRepository = journalRepository;
            _journalEntryRepository = journalEntryRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;

            _nfi = new NumberFormatInfo();
            _nfi.CurrencySymbol = string.Empty;
        }

        public AlternateChannelReconciliationPeriodDTO AddNewAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(alternateChannelReconciliationPeriodDTO.DurationStartDate, alternateChannelReconciliationPeriodDTO.DurationEndDate);

                    var alternateChannelReconciliationPeriod = AlternateChannelReconciliationPeriodFactory.CreateAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO.AlternateChannelType, duration, alternateChannelReconciliationPeriodDTO.SetDifferenceMode, alternateChannelReconciliationPeriodDTO.Remarks);

                    alternateChannelReconciliationPeriod.Status = (int)AlternateChannelReconciliationPeriodStatus.Open;
                    alternateChannelReconciliationPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _alternateChannelReconciliationPeriodRepository.Add(alternateChannelReconciliationPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return alternateChannelReconciliationPeriod.ProjectedAs<AlternateChannelReconciliationPeriodDTO>();
                }
            }
            else return null;
        }

        public bool UpdateAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationPeriodDTO == null || alternateChannelReconciliationPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelReconciliationPeriodRepository.Get(alternateChannelReconciliationPeriodDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var duration = new Duration(alternateChannelReconciliationPeriodDTO.DurationStartDate, alternateChannelReconciliationPeriodDTO.DurationEndDate);

                    var current = AlternateChannelReconciliationPeriodFactory.CreateAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO.AlternateChannelType, duration, alternateChannelReconciliationPeriodDTO.SetDifferenceMode, alternateChannelReconciliationPeriodDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Status = persisted.Status;
                    current.CreatedBy = persisted.CreatedBy;

                    _alternateChannelReconciliationPeriodRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool CloseAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _alternateChannelReconciliationPeriodRepository.Get(alternateChannelReconciliationPeriodDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)AlternateChannelReconciliationPeriodStatus.Open)
                    {
                        switch ((AlternateChannelReconciliationPeriodAuthOption)alternateChannelReconciliationPeriodAuthOption)
                        {
                            case AlternateChannelReconciliationPeriodAuthOption.Post:

                                persisted.Status = (int)AlternateChannelReconciliationPeriodStatus.Closed;
                                persisted.AuthorizationRemarks = alternateChannelReconciliationPeriodDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            case AlternateChannelReconciliationPeriodAuthOption.Reject:

                                persisted.Status = (int)AlternateChannelReconciliationPeriodStatus.Suspended;
                                persisted.AuthorizationRemarks = alternateChannelReconciliationPeriodDTO.AuthorizationRemarks;
                                persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                                persisted.AuthorizedDate = DateTime.Now;

                                break;
                            default:
                                break;
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public AlternateChannelReconciliationEntryDTO AddNewAlternateChannelReconciliationEntry(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var alternateChannelReconciliationEntry = AlternateChannelReconciliationEntryFactory.CreateAlternateChannelReconciliationEntry(alternateChannelReconciliationEntryDTO.AlternateChannelReconciliationPeriodId, alternateChannelReconciliationEntryDTO.PrimaryAccountNumber, alternateChannelReconciliationEntryDTO.SystemTraceAuditNumber, alternateChannelReconciliationEntryDTO.RetrievalReferenceNumber, alternateChannelReconciliationEntryDTO.Amount, alternateChannelReconciliationEntryDTO.Reference, alternateChannelReconciliationEntryDTO.Remarks);

                    alternateChannelReconciliationEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _alternateChannelReconciliationEntryRepository.Add(alternateChannelReconciliationEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return alternateChannelReconciliationEntry.ProjectedAs<AlternateChannelReconciliationEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveAlternateChannelReconciliationEntries(List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in alternateChannelReconciliationEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _alternateChannelReconciliationEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _alternateChannelReconciliationEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var alternateChannelReconciliationPeriods = _alternateChannelReconciliationPeriodRepository.GetAll(serviceHeader);

                if (alternateChannelReconciliationPeriods != null && alternateChannelReconciliationPeriods.Any())
                {
                    return alternateChannelReconciliationPeriods.ProjectedAsCollection<AlternateChannelReconciliationPeriodDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelReconciliationPeriodSpecifications.DefaultSpec();

                ISpecification<AlternateChannelReconciliationPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelReconciliationPeriodPagedCollection = _alternateChannelReconciliationPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelReconciliationPeriodPagedCollection != null)
                {
                    var pageCollection = alternateChannelReconciliationPeriodPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelReconciliationPeriodDTO>();

                    var itemsCount = alternateChannelReconciliationPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelReconciliationPeriodSpecifications.AlternateChannelReconciliationPeriodFullText(text);

                ISpecification<AlternateChannelReconciliationPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelReconciliationPeriodCollection = _alternateChannelReconciliationPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelReconciliationPeriodCollection != null)
                {
                    var pageCollection = alternateChannelReconciliationPeriodCollection.PageCollection.ProjectedAsCollection<AlternateChannelReconciliationPeriodDTO>();

                    var itemsCount = alternateChannelReconciliationPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelReconciliationPeriodSpecifications.AlternateChannelReconciliationPeriodFullText(status, startDate, endDate, text);

                ISpecification<AlternateChannelReconciliationPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var alternateChannelReconciliationPeriodPagedCollection = _alternateChannelReconciliationPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (alternateChannelReconciliationPeriodPagedCollection != null)
                {
                    var pageCollection = alternateChannelReconciliationPeriodPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelReconciliationPeriodDTO>();

                    var itemsCount = alternateChannelReconciliationPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public AlternateChannelReconciliationPeriodDTO FindAlternateChannelReconciliationPeriod(Guid alternateChannelReconciliationPeriodId, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var alternateChannelReconciliationPeriod = _alternateChannelReconciliationPeriodRepository.Get(alternateChannelReconciliationPeriodId, serviceHeader);

                    if (alternateChannelReconciliationPeriod != null)
                    {
                        return alternateChannelReconciliationPeriod.ProjectedAs<AlternateChannelReconciliationPeriodDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<AlternateChannelReconciliationEntryDTO> FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodId(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (alternateChannelReconciliationPeriodId != null && alternateChannelReconciliationPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = AlternateChannelReconciliationEntrySpecifications.AlternateChannelReconciliationEntryFullText(alternateChannelReconciliationPeriodId, status, text);

                    ISpecification<AlternateChannelReconciliationEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var alternateChannelReconciliationEntryPagedCollection = _alternateChannelReconciliationEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (alternateChannelReconciliationEntryPagedCollection != null)
                    {
                        var pageCollection = alternateChannelReconciliationEntryPagedCollection.PageCollection.ProjectedAsCollection<AlternateChannelReconciliationEntryDTO>();

                        var itemsCount = alternateChannelReconciliationEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<AlternateChannelReconciliationEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<BatchImportEntryWrapper> ParseAlternateChannelReconciliationImport(Guid alternateChannelReconciliationPeriodId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            BatchImportParseInfo parseInfo = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _alternateChannelReconciliationPeriodRepository.Get(alternateChannelReconciliationPeriodId, serviceHeader);

                if (persisted != null && persisted.Status == (int)BatchStatus.Pending && !string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (System.IO.File.Exists(path))
                    {
                        var importEntries = new List<BatchImportEntryWrapper> { };

                        using (var streamReader = new StreamReader(path))
                        using (var reader = new CsvReader(streamReader))
                        {
                            // the CSV file has a header record, so we read that first
                            reader.ReadHeaderRecord();

                            while (reader.HasMoreRecords)
                            {
                                var dataRecord = reader.ReadDataRecord();

                                if (dataRecord.Count >= 4)
                                {
                                    var alternateChannelEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //RetrievalReferenceNumber/SystemTraceAuditNumber/CallbackPayload
                                        Column2 = dataRecord[1], //PrimaryAccountNumber
                                        Column3 = dataRecord[2], //Amount
                                        Column4 = dataRecord[3], //YourReference
                                    };

                                    importEntries.Add(alternateChannelEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            parseInfo = ParseAlternateChannels(importEntries, persisted.AlternateChannelType, persisted.SetDifferenceMode, persisted.Duration.StartDate, persisted.Duration.EndDate, serviceHeader);

                            if (parseInfo != null)
                            {
                                var alternateChannelReconciliationEntries = new List<AlternateChannelReconciliationEntryDTO>();

                                #region collate matched

                                foreach (var item in parseInfo.MatchedCollection6)
                                {
                                    var journalFilter = JournalSpecifications.JournalWithAlternateChannelLogId(item.Id);

                                    ISpecification<Journal> journalSpec = journalFilter;

                                    var journals = _journalRepository.AllMatching(journalSpec, serviceHeader);

                                    switch ((AlternateChannelType)persisted.AlternateChannelType)
                                    {
                                        case AlternateChannelType.SaccoLink:

                                            var saccoLinkSettlementChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.SaccoLinkSettlement, serviceHeader);

                                            var saccoLinkPointOfSaleSettlementChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.SaccoLinkPOSSettlement, serviceHeader);

                                            foreach (var journal in journals)
                                            {
                                                foreach (var journalEntry in journal.JournalEntries)
                                                {
                                                    if (journalEntry.ChartOfAccountId.In(saccoLinkSettlementChartOfAccountId, saccoLinkPointOfSaleSettlementChartOfAccountId))
                                                    {
                                                        alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                                        {
                                                            AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                            Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                            PrimaryAccountNumber = item.ISO8583PrimaryAccountNumber,
                                                            SystemTraceAuditNumber = item.ISO8583SystemTraceAuditNumber,
                                                            RetrievalReferenceNumber = item.ISO8583RetrievalReferenceNumber,
                                                            Amount = item.ISO8583Amount,
                                                            Reference = item.ISO8583MessageTypeIdentification,
                                                            Remarks = journal.PrimaryDescription,
                                                        });
                                                    }
                                                }
                                            }

                                            break;
                                        case AlternateChannelType.MCoopCash:

                                            var mCoopCashSettlementChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.MCoopCashSettlement, serviceHeader);

                                            foreach (var journal in journals)
                                            {
                                                foreach (var journalEntry in journal.JournalEntries)
                                                {
                                                    if (journalEntry.ChartOfAccountId.In(mCoopCashSettlementChartOfAccountId))
                                                    {
                                                        alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                                        {
                                                            AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                            Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                            PrimaryAccountNumber = item.ISO8583PrimaryAccountNumber,
                                                            SystemTraceAuditNumber = item.ISO8583SystemTraceAuditNumber,
                                                            RetrievalReferenceNumber = item.ISO8583RetrievalReferenceNumber,
                                                            Amount = item.ISO8583Amount,
                                                            Reference = item.ISO8583MessageTypeIdentification,
                                                            Remarks = journal.PrimaryDescription,
                                                        });
                                                    }
                                                }
                                            }

                                            break;
                                        case AlternateChannelType.SpotCash:

                                            var spotCashSettlementChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.SpotCashSettlement, serviceHeader);

                                            foreach (var journal in journals)
                                            {
                                                foreach (var journalEntry in journal.JournalEntries)
                                                {
                                                    if (journalEntry.ChartOfAccountId.In(spotCashSettlementChartOfAccountId))
                                                    {
                                                        alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                                        {
                                                            AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                            Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                            PrimaryAccountNumber = item.ISO8583PrimaryAccountNumber,
                                                            SystemTraceAuditNumber = item.ISO8583SystemTraceAuditNumber,
                                                            RetrievalReferenceNumber = item.ISO8583RetrievalReferenceNumber,
                                                            Amount = item.ISO8583Amount,
                                                            Reference = item.ISO8583MessageTypeIdentification,
                                                            Remarks = journal.PrimaryDescription,
                                                        });
                                                    }
                                                }
                                            }

                                            break;
                                        case AlternateChannelType.AgencyBanking:
                                        case AlternateChannelType.AbcBank:
                                            alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                            {
                                                AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                PrimaryAccountNumber = item.ISO8583PrimaryAccountNumber,
                                                SystemTraceAuditNumber = item.ISO8583SystemTraceAuditNumber,
                                                RetrievalReferenceNumber = item.ISO8583RetrievalReferenceNumber,
                                                Amount = item.ISO8583Amount,
                                                Reference = item.ISO8583MessageTypeIdentification,
                                            });
                                            break;
                                        case AlternateChannelType.Sparrow:
                                            alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                            {
                                                AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                PrimaryAccountNumber = item.SPARROWCardNumber,
                                                SystemTraceAuditNumber = item.SPARROWSRCIMD,
                                                RetrievalReferenceNumber = item.SPARROWDeviceId,
                                                Amount = item.SPARROWAmount,
                                                Reference = item.SPARROWMessageType,
                                            });
                                            break;
                                        case AlternateChannelType.Citius:
                                            alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                            {
                                                AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                PrimaryAccountNumber = item.WALLETPrimaryAccountNumber,
                                                SystemTraceAuditNumber = item.WALLETSystemTraceAuditNumber,
                                                RetrievalReferenceNumber = item.WALLETRetrievalReferenceNumber,
                                                Amount = item.WALLETAmount,
                                                Reference = item.WALLETCallbackPayload ?? item.WALLETMessageTypeIdentification,
                                            });
                                            break;
                                        case AlternateChannelType.PesaPepe:

                                            var pesaPepeSettlementChartOfAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.MobileWalletB2CSettlement, serviceHeader);

                                            foreach (var journal in journals)
                                            {
                                                foreach (var journalEntry in journal.JournalEntries)
                                                {
                                                    if (journalEntry.ChartOfAccountId.In(pesaPepeSettlementChartOfAccountId))
                                                    {
                                                        alternateChannelReconciliationEntries.Add(new AlternateChannelReconciliationEntryDTO
                                                        {
                                                            AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                                            Status = (int)AlternateChannelReconciliationEntryStatus.Reconciled,
                                                            PrimaryAccountNumber = item.WALLETPrimaryAccountNumber,
                                                            SystemTraceAuditNumber = item.WALLETSystemTraceAuditNumber,
                                                            RetrievalReferenceNumber = item.WALLETRetrievalReferenceNumber,
                                                            Amount = journalEntry.Amount,
                                                            Reference = item.WALLETCallbackPayload ?? item.WALLETMessageTypeIdentification,
                                                            Remarks = journal.PrimaryDescription,
                                                        });
                                                    }
                                                }
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }

                                #endregion

                                #region collate unmatched

                                foreach (var item in parseInfo.MismatchedCollection)
                                {
                                    var entry = new AlternateChannelReconciliationEntryDTO
                                    {
                                        AlternateChannelReconciliationPeriodId = alternateChannelReconciliationPeriodId,
                                        Status = (int)AlternateChannelReconciliationEntryStatus.Unreconciled,
                                        PrimaryAccountNumber = item.Column2,
                                        Amount = !string.IsNullOrWhiteSpace(item.Column3) ? decimal.Parse(item.Column3, NumberStyles.Any) : 0m,
                                        Reference = item.Column4,
                                        Remarks = item.Remarks,
                                    };

                                    switch ((SetDifferenceMode)persisted.SetDifferenceMode)
                                    {
                                        case SetDifferenceMode.RRNExistsInFileButNotSystem:
                                        case SetDifferenceMode.RRNExistsInSystemButNotFile:
                                            entry.RetrievalReferenceNumber = item.Column1;
                                            break;
                                        case SetDifferenceMode.STANExistsInFileButNotSystem:
                                        case SetDifferenceMode.STANExistsInSystemButNotFile:
                                            entry.SystemTraceAuditNumber = item.Column1;
                                            break;
                                        default:
                                            break;
                                    }

                                    alternateChannelReconciliationEntries.Add(entry);
                                }

                                #endregion

                                #region update period entries

                                if (alternateChannelReconciliationEntries.Any())
                                {
                                    UpdateAlternateChannelReconciliationEntries(alternateChannelReconciliationPeriodId, alternateChannelReconciliationEntries, serviceHeader);
                                }

                                #endregion
                            }
                        }
                    }
                }
            }

            if (parseInfo != null)
            {
                #region Mark channel log as reconciled

                if (parseInfo.MatchedCollection6.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in parseInfo.MatchedCollection6)
                        {
                            var alternateChannelLog = _alternateChannelLogRepository.Get(item.Id, serviceHeader);

                            alternateChannelLog.MarkReconciled(serviceHeader.ApplicationUserName);
                        }

                        dbContextScope.SaveChanges(serviceHeader);
                    }
                }

                #endregion

                return parseInfo.MismatchedCollection;
            }
            else return null;
        }

        private BatchImportParseInfo ParseAlternateChannels(List<BatchImportEntryWrapper> importEntries, int alternateChannelType, int setDifferenceMode, DateTime startDate, DateTime endDate, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection6 = new List<AlternateChannelLogDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var projection = new List<AlternateChannelLogDTO>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelLogSpecifications.AlternateChannelLog(alternateChannelType, startDate, endDate);

                ISpecification<AlternateChannelLog> spec = filter;

                var alternateChannelLogDTOs = _alternateChannelLogRepository.AllMatching<AlternateChannelLogDTO>(spec, serviceHeader);

                if (alternateChannelLogDTOs != null && alternateChannelLogDTOs.Any())
                {
                    foreach (var item in alternateChannelLogDTOs)
                    {
                        var journalFilter = JournalSpecifications.JournalWithAlternateChannelLogId(startDate, endDate, item.Id);

                        ISpecification<Journal> journalSpec = journalFilter;

                        var matchedJournals = _journalRepository.AllMatchingCount(journalSpec, serviceHeader);

                        if (matchedJournals == 0) continue;

                        switch ((AlternateChannelType)alternateChannelType)
                        {
                            case AlternateChannelType.SaccoLink:

                                switch ((SetDifferenceMode)setDifferenceMode)
                                {
                                    case SetDifferenceMode.RRNExistsInSystemButNotFile: // we are looking for coop transactions
                                        if ((item.ISO8583Message.Contains("6010", StringComparison.OrdinalIgnoreCase) || item.ISO8583Message.Contains("6011", StringComparison.OrdinalIgnoreCase) || item.ISO8583Message.Contains("5999", StringComparison.OrdinalIgnoreCase)) && (item.ISO8583Message.Contains("RTPSID9666", StringComparison.OrdinalIgnoreCase) || item.ISO8583Message.Contains("RTPSID9900", StringComparison.OrdinalIgnoreCase)))
                                            projection.Add(item);
                                        break;
                                    case SetDifferenceMode.STANExistsInSystemButNotFile: // we are looking for non_coop transactions
                                        if (item.ISO8583Message.Contains("6011", StringComparison.OrdinalIgnoreCase) && !item.ISO8583Message.Contains("RTPSID9666", StringComparison.OrdinalIgnoreCase))
                                            projection.Add(item);
                                        break;
                                    case SetDifferenceMode.RRNExistsInFileButNotSystem:
                                    case SetDifferenceMode.STANExistsInFileButNotSystem:
                                        projection.Add(item);
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case AlternateChannelType.Sparrow:
                            case AlternateChannelType.MCoopCash:
                            case AlternateChannelType.SpotCash:
                            case AlternateChannelType.Citius:
                            case AlternateChannelType.AgencyBanking:
                            case AlternateChannelType.PesaPepe:
                            case AlternateChannelType.AbcBank:
                                projection.Add(item);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (projection.Any())
            {
                var Imported_FileList = from l in importEntries select l.Column1;

                switch ((SetDifferenceMode)setDifferenceMode)
                {
                    case SetDifferenceMode.RRNExistsInFileButNotSystem:
                    case SetDifferenceMode.STANExistsInFileButNotSystem:
                    case SetDifferenceMode.CallbackPayloadExistsInFileButNotSystem:

                        #region *___ExistsInFileButNotSystem

                        var ExistsInFileButNotSystem_SystemList = ProjectBySetDifferenceMode(setDifferenceMode, projection);

                        var ExistsInFileButNotSystem_SetIntersection = Imported_FileList.Intersect(ExistsInFileButNotSystem_SystemList);

                        var ExistsInFileButNotSystem_SetDifference = Imported_FileList.Except(ExistsInFileButNotSystem_SystemList);

                        foreach (var referenceNumber in ExistsInFileButNotSystem_SetIntersection)
                        {
                            switch ((AlternateChannelType)alternateChannelType)
                            {
                                case AlternateChannelType.SaccoLink:
                                case AlternateChannelType.MCoopCash:
                                case AlternateChannelType.SpotCash:
                                case AlternateChannelType.AgencyBanking:
                                case AlternateChannelType.AbcBank:

                                    var ExistsInFileButNotSystem_TargetISO8583AlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInFileButNotSystem
                                        ? projection.Where(x => x.ISO8583RetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.ISO8583SystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInFileButNotSystem_TargetISO8583AlternateChannelLogs != null && ExistsInFileButNotSystem_TargetISO8583AlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInFileButNotSystem_TargetISO8583AlternateChannelLog in ExistsInFileButNotSystem_TargetISO8583AlternateChannelLogs)
                                        {
                                            if (!ExistsInFileButNotSystem_TargetISO8583AlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInFileButNotSystem_TargetISO8583AlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Sparrow:

                                    var ExistsInFileButNotSystem_TargetSparrowAlternateChannelLogs = projection.Where(x => x.SPARROWSRCIMD.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInFileButNotSystem_TargetSparrowAlternateChannelLogs != null && ExistsInFileButNotSystem_TargetSparrowAlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInFileButNotSystem_TargetSparrowAlternateChannelLog in ExistsInFileButNotSystem_TargetSparrowAlternateChannelLogs)
                                        {
                                            if (!ExistsInFileButNotSystem_TargetSparrowAlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInFileButNotSystem_TargetSparrowAlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Citius:

                                    var ExistsInFileButNotSystem_CitiusTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInFileButNotSystem
                                        ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInFileButNotSystem_CitiusTargetWalletAlternateChannelLogs != null && ExistsInFileButNotSystem_CitiusTargetWalletAlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInFileButNotSystem_TargetWalletAlternateChannelLog in ExistsInFileButNotSystem_CitiusTargetWalletAlternateChannelLogs)
                                        {
                                            if (!ExistsInFileButNotSystem_TargetWalletAlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInFileButNotSystem_TargetWalletAlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.PesaPepe:

                                    if (setDifferenceMode == (int)SetDifferenceMode.CallbackPayloadExistsInFileButNotSystem)
                                    {
                                        var ExistsInFileButNotSystem_PesaPepeCallbackTargetWalletAlternateChannelLogs = projection.Where(x => !string.IsNullOrWhiteSpace(x.WALLETCallbackPayload) && x.WALLETCallbackPayload.StartsWith(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (ExistsInFileButNotSystem_PesaPepeCallbackTargetWalletAlternateChannelLogs != null && ExistsInFileButNotSystem_PesaPepeCallbackTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var ExistsInFileButNotSystem_TargetWalletAlternateChannelLog in ExistsInFileButNotSystem_PesaPepeCallbackTargetWalletAlternateChannelLogs)
                                            {
                                                if (!ExistsInFileButNotSystem_TargetWalletAlternateChannelLog.IsReconciled)
                                                    result.MatchedCollection6.Add(ExistsInFileButNotSystem_TargetWalletAlternateChannelLog);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var ExistsInFileButNotSystem_PesaPepeTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInFileButNotSystem
                                            ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                            : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (ExistsInFileButNotSystem_PesaPepeTargetWalletAlternateChannelLogs != null && ExistsInFileButNotSystem_PesaPepeTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var ExistsInFileButNotSystem_TargetWalletAlternateChannelLog in ExistsInFileButNotSystem_PesaPepeTargetWalletAlternateChannelLogs)
                                            {
                                                if (!ExistsInFileButNotSystem_TargetWalletAlternateChannelLog.IsReconciled)
                                                    result.MatchedCollection6.Add(ExistsInFileButNotSystem_TargetWalletAlternateChannelLog);
                                            }
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        foreach (var referenceNumber in ExistsInFileButNotSystem_SetDifference)
                        {
                            var targetImportEntries = importEntries.Where(x => x.Column1 == referenceNumber);

                            if (targetImportEntries != null && targetImportEntries.Any())
                            {
                                foreach (var targetImportEntry in targetImportEntries)
                                {
                                    targetImportEntry.Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode));

                                    result.MismatchedCollection.Add(targetImportEntry);
                                }
                            }
                        }

                        #endregion

                        break;
                    case SetDifferenceMode.RRNExistsInSystemButNotFile:
                    case SetDifferenceMode.STANExistsInSystemButNotFile:
                    case SetDifferenceMode.CallbackPayloadExistsInSystemButNotFile:

                        #region *___ExistsInSystemButNotFile

                        var ExistsInSystemButNotFile_SystemList = ProjectBySetDifferenceMode(setDifferenceMode, projection);

                        var ExistsInSystemButNotFile_SetIntersection = ExistsInSystemButNotFile_SystemList.Intersect(Imported_FileList);

                        var ExistsInSystemButNotFile_SetDifference = ExistsInSystemButNotFile_SystemList.Except(Imported_FileList);

                        foreach (var referenceNumber in ExistsInSystemButNotFile_SetIntersection)
                        {
                            switch ((AlternateChannelType)alternateChannelType)
                            {
                                case AlternateChannelType.SaccoLink:
                                case AlternateChannelType.MCoopCash:
                                case AlternateChannelType.SpotCash:
                                case AlternateChannelType.AgencyBanking:
                                case AlternateChannelType.AbcBank:

                                    var ExistsInSystemButNotFile_TargetISO8583AlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                        ? projection.Where(x => x.ISO8583RetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.ISO8583SystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInSystemButNotFile_TargetISO8583AlternateChannelLogs != null && ExistsInSystemButNotFile_TargetISO8583AlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInSystemButNotFile_TargetISO8583AlternateChannelLog in ExistsInSystemButNotFile_TargetISO8583AlternateChannelLogs)
                                        {
                                            if (!ExistsInSystemButNotFile_TargetISO8583AlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInSystemButNotFile_TargetISO8583AlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Sparrow:

                                    var ExistsInSystemButNotFile_TargetSparrowAlternateChannelLogs = projection.Where(x => x.SPARROWSRCIMD.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInSystemButNotFile_TargetSparrowAlternateChannelLogs != null && ExistsInSystemButNotFile_TargetSparrowAlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInSystemButNotFile_TargetSparrowAlternateChannelLog in ExistsInSystemButNotFile_TargetSparrowAlternateChannelLogs)
                                        {
                                            if (!ExistsInSystemButNotFile_TargetSparrowAlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInSystemButNotFile_TargetSparrowAlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Citius:

                                    var ExistsInSystemButNotFile_CitiusTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                        ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (ExistsInSystemButNotFile_CitiusTargetWalletAlternateChannelLogs != null && ExistsInSystemButNotFile_CitiusTargetWalletAlternateChannelLogs.Any())
                                    {
                                        foreach (var ExistsInSystemButNotFile_TargetWalletAlternateChannelLog in ExistsInSystemButNotFile_CitiusTargetWalletAlternateChannelLogs)
                                        {
                                            if (!ExistsInSystemButNotFile_TargetWalletAlternateChannelLog.IsReconciled)
                                                result.MatchedCollection6.Add(ExistsInSystemButNotFile_TargetWalletAlternateChannelLog);
                                        }
                                    }

                                    break;
                                case AlternateChannelType.PesaPepe:

                                    if (setDifferenceMode == (int)SetDifferenceMode.CallbackPayloadExistsInSystemButNotFile)
                                    {
                                        var ExistsInSystemButNotFile_PesaPepeCallbackTargetWalletAlternateChannelLogs = projection.Where(x => !string.IsNullOrWhiteSpace(x.WALLETCallbackPayload) && x.WALLETCallbackPayload.StartsWith(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (ExistsInSystemButNotFile_PesaPepeCallbackTargetWalletAlternateChannelLogs != null && ExistsInSystemButNotFile_PesaPepeCallbackTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var ExistsInSystemButNotFile_TargetWalletAlternateChannelLog in ExistsInSystemButNotFile_PesaPepeCallbackTargetWalletAlternateChannelLogs)
                                            {
                                                if (!ExistsInSystemButNotFile_TargetWalletAlternateChannelLog.IsReconciled)
                                                    result.MatchedCollection6.Add(ExistsInSystemButNotFile_TargetWalletAlternateChannelLog);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var ExistsInSystemButNotFile_PesaPepeTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                            ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                            : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (ExistsInSystemButNotFile_PesaPepeTargetWalletAlternateChannelLogs != null && ExistsInSystemButNotFile_PesaPepeTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var ExistsInSystemButNotFile_TargetWalletAlternateChannelLog in ExistsInSystemButNotFile_PesaPepeTargetWalletAlternateChannelLogs)
                                            {
                                                if (!ExistsInSystemButNotFile_TargetWalletAlternateChannelLog.IsReconciled)
                                                    result.MatchedCollection6.Add(ExistsInSystemButNotFile_TargetWalletAlternateChannelLog);
                                            }
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        foreach (var referenceNumber in ExistsInSystemButNotFile_SetDifference)
                        {
                            switch ((AlternateChannelType)alternateChannelType)
                            {
                                case AlternateChannelType.SaccoLink:
                                case AlternateChannelType.MCoopCash:
                                case AlternateChannelType.SpotCash:
                                case AlternateChannelType.AgencyBanking:
                                case AlternateChannelType.AbcBank:

                                    var targetISO8583AlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                        ? projection.Where(x => x.ISO8583RetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.ISO8583SystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (targetISO8583AlternateChannelLogs != null && targetISO8583AlternateChannelLogs.Any())
                                    {
                                        foreach (var targetISO8583AlternateChannelLog in targetISO8583AlternateChannelLogs)
                                        {
                                            result.MismatchedCollection.Add(
                                                new BatchImportEntryWrapper
                                                {
                                                    Column1 = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile ? targetISO8583AlternateChannelLog.ISO8583RetrievalReferenceNumber : targetISO8583AlternateChannelLog.ISO8583SystemTraceAuditNumber,
                                                    Column2 = targetISO8583AlternateChannelLog.ISO8583PrimaryAccountNumber,
                                                    Column3 = string.Format(_nfi, "{0:C}", targetISO8583AlternateChannelLog.ISO8583Amount),
                                                    Column4 = targetISO8583AlternateChannelLog.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss tt"),
                                                    Column5 = targetISO8583AlternateChannelLog.ISO8583MessageTypeIdentification,
                                                    Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode))
                                                });
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Sparrow:

                                    var targetSparrowAlternateChannelLogs = projection.Where(x => x.SPARROWSRCIMD.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (targetSparrowAlternateChannelLogs != null && targetSparrowAlternateChannelLogs.Any())
                                    {
                                        foreach (var targetSparrowAlternateChannelLog in targetSparrowAlternateChannelLogs)
                                        {
                                            result.MismatchedCollection.Add(
                                                new BatchImportEntryWrapper
                                                {
                                                    Column1 = targetSparrowAlternateChannelLog.SPARROWSRCIMD,
                                                    Column2 = targetSparrowAlternateChannelLog.SPARROWCardNumber,
                                                    Column3 = string.Format(_nfi, "{0:C}", targetSparrowAlternateChannelLog.SPARROWAmount),
                                                    Column4 = targetSparrowAlternateChannelLog.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss tt"),
                                                    Column5 = targetSparrowAlternateChannelLog.SPARROWMessageType,
                                                    Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode))
                                                });
                                        }
                                    }

                                    break;
                                case AlternateChannelType.Citius:

                                    var citiusTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                        ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                        : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                    if (citiusTargetWalletAlternateChannelLogs != null && citiusTargetWalletAlternateChannelLogs.Any())
                                    {
                                        foreach (var targetWalletAlternateChannelLog in citiusTargetWalletAlternateChannelLogs)
                                        {
                                            result.MismatchedCollection.Add(
                                                new BatchImportEntryWrapper
                                                {
                                                    Column1 = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile ? targetWalletAlternateChannelLog.WALLETRetrievalReferenceNumber : targetWalletAlternateChannelLog.WALLETSystemTraceAuditNumber,
                                                    Column2 = targetWalletAlternateChannelLog.WALLETPrimaryAccountNumber,
                                                    Column3 = string.Format(_nfi, "{0:C}", targetWalletAlternateChannelLog.WALLETAmount),
                                                    Column4 = targetWalletAlternateChannelLog.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss tt"),
                                                    Column5 = targetWalletAlternateChannelLog.WALLETMessageTypeIdentification,
                                                    Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode))
                                                });
                                        }
                                    }

                                    break;
                                case AlternateChannelType.PesaPepe:

                                    if (setDifferenceMode == (int)SetDifferenceMode.CallbackPayloadExistsInSystemButNotFile)
                                    {
                                        var pesapepeCallbackTargetWalletAlternateChannelLogs = projection.Where(x => !string.IsNullOrWhiteSpace(x.WALLETCallbackPayload) && x.WALLETCallbackPayload.StartsWith(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (pesapepeCallbackTargetWalletAlternateChannelLogs != null && pesapepeCallbackTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var targetWalletAlternateChannelLog in pesapepeCallbackTargetWalletAlternateChannelLogs)
                                            {
                                                result.MismatchedCollection.Add(
                                                    new BatchImportEntryWrapper
                                                    {
                                                        Column1 = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile ? targetWalletAlternateChannelLog.WALLETRetrievalReferenceNumber : targetWalletAlternateChannelLog.WALLETSystemTraceAuditNumber,
                                                        Column2 = targetWalletAlternateChannelLog.WALLETPrimaryAccountNumber,
                                                        Column3 = string.Format(_nfi, "{0:C}", targetWalletAlternateChannelLog.WALLETAmount),
                                                        Column4 = targetWalletAlternateChannelLog.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss tt"),
                                                        Column5 = targetWalletAlternateChannelLog.WALLETMessageTypeIdentification,
                                                        Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode))
                                                    });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var pesapepeTargetWalletAlternateChannelLogs = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile
                                            ? projection.Where(x => x.WALLETRetrievalReferenceNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase))
                                            : projection.Where(x => x.WALLETSystemTraceAuditNumber.Equals(referenceNumber, StringComparison.OrdinalIgnoreCase));

                                        if (pesapepeTargetWalletAlternateChannelLogs != null && pesapepeTargetWalletAlternateChannelLogs.Any())
                                        {
                                            foreach (var targetWalletAlternateChannelLog in pesapepeTargetWalletAlternateChannelLogs)
                                            {
                                                result.MismatchedCollection.Add(
                                                    new BatchImportEntryWrapper
                                                    {
                                                        Column1 = setDifferenceMode == (int)SetDifferenceMode.RRNExistsInSystemButNotFile ? targetWalletAlternateChannelLog.WALLETRetrievalReferenceNumber : targetWalletAlternateChannelLog.WALLETSystemTraceAuditNumber,
                                                        Column2 = targetWalletAlternateChannelLog.WALLETPrimaryAccountNumber,
                                                        Column3 = string.Format(_nfi, "{0:C}", targetWalletAlternateChannelLog.WALLETAmount),
                                                        Column4 = targetWalletAlternateChannelLog.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss tt"),
                                                        Column5 = targetWalletAlternateChannelLog.WALLETMessageTypeIdentification,
                                                        Remarks = string.Format("{0}", EnumHelper.GetDescription((SetDifferenceMode)setDifferenceMode))
                                                    });
                                            }
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        #endregion

                        break;
                    default:
                        break;
                }
            }
            else
            {
                importEntries.ForEach(x => x.Remarks = string.Format("No matching {0} channel logs and/or transactions for the given period", EnumHelper.GetDescription((AlternateChannelType)alternateChannelType)));

                result.MismatchedCollection.AddRange(importEntries);
            };

            return result;
        }

        private bool UpdateAlternateChannelReconciliationEntries(Guid alternateChannelReconciliationPeriodId, List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (alternateChannelReconciliationPeriodId != null && alternateChannelReconciliationEntries != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _alternateChannelReconciliationPeriodRepository.Get(alternateChannelReconciliationPeriodId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteAlternateChannelReconciliationEntries(persisted.Id, serviceHeader);

                        if (alternateChannelReconciliationEntries.Any())
                        {
                            List<AlternateChannelReconciliationEntry> batchEntries = new List<AlternateChannelReconciliationEntry>();

                            foreach (var item in alternateChannelReconciliationEntries)
                            {
                                var alternateChannelReconciliationEntry = AlternateChannelReconciliationEntryFactory.CreateAlternateChannelReconciliationEntry(persisted.Id, item.PrimaryAccountNumber, item.SystemTraceAuditNumber, item.RetrievalReferenceNumber, item.Amount, item.Reference, item.Remarks);

                                alternateChannelReconciliationEntry.Status = (byte)item.Status;
                                alternateChannelReconciliationEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(alternateChannelReconciliationEntry);
                            }

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<AlternateChannelReconciliationEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    AlternateChannelReconciliationEntryBulkCopyDTO bcpc =
                                        new AlternateChannelReconciliationEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            AlternateChannelReconciliationPeriodId = c.AlternateChannelReconciliationPeriodId,
                                            PrimaryAccountNumber = c.PrimaryAccountNumber,
                                            SystemTraceAuditNumber = c.SystemTraceAuditNumber,
                                            RetrievalReferenceNumber = c.RetrievalReferenceNumber,
                                            Amount = c.Amount,
                                            Reference = c.Reference,
                                            Status = c.Status,
                                            Remarks = c.Remarks,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _alternateChannelReconciliationEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static IEnumerable<string> ProjectBySetDifferenceMode(int setDifferenceMode, List<AlternateChannelLogDTO> projection)
        {
            var result = new List<string>();

            switch ((SetDifferenceMode)setDifferenceMode)
            {
                case SetDifferenceMode.RRNExistsInFileButNotSystem:
                case SetDifferenceMode.RRNExistsInSystemButNotFile:

                    result = (from l in projection select l.ISO8583RetrievalReferenceNumber ?? l.WALLETRetrievalReferenceNumber ?? l.SPARROWSRCIMD).ToList();

                    break;
                case SetDifferenceMode.STANExistsInFileButNotSystem:
                case SetDifferenceMode.STANExistsInSystemButNotFile:

                    result = (from l in projection select l.ISO8583SystemTraceAuditNumber ?? l.WALLETSystemTraceAuditNumber ?? l.SPARROWSRCIMD).ToList();

                    break;
                case SetDifferenceMode.CallbackPayloadExistsInFileButNotSystem:
                case SetDifferenceMode.CallbackPayloadExistsInSystemButNotFile:

                    var stringList = from l in projection select l.WALLETCallbackPayload/*MDI2WIYZF8 - 254722XXXXXX - FOO BAR*/;

                    if (stringList != null && stringList.Any())
                    {
                        foreach (var item in stringList)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var parts = item.Split(new char[] { '-' });

                                if (parts.Length != 0)
                                    result.Add(parts[0].Trim());
                            }
                        }
                    }

                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
