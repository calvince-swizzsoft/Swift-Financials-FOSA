using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
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
    public class GeneralLedgerAppService : IGeneralLedgerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<GeneralLedger> _generalLedgerRepository;
        private readonly IRepository<GeneralLedgerEntry> _generalLedgerEntryRepository;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public GeneralLedgerAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<GeneralLedger> generalLedgerRepository,
           IRepository<GeneralLedgerEntry> generalLedgerEntryRepository,
           IJournalEntryPostingService journalEntryPostingService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService,
           ILoanProductAppService loanProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IChartOfAccountAppService chartOfAccountAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (generalLedgerRepository == null)
                throw new ArgumentNullException(nameof(generalLedgerRepository));

            if (generalLedgerEntryRepository == null)
                throw new ArgumentNullException(nameof(generalLedgerEntryRepository));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _generalLedgerRepository = generalLedgerRepository;
            _generalLedgerEntryRepository = generalLedgerEntryRepository;
            _journalEntryPostingService = journalEntryPostingService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _loanProductAppService = loanProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
        }

        public GeneralLedgerDTO AddNewGeneralLedger(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader)
        {
            if (generalLedgerDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var generalLedger = GeneralLedgerFactory.CreateGeneralLedger(generalLedgerDTO.BranchId, generalLedgerDTO.PostingPeriodId, generalLedgerDTO.TotalValue, generalLedgerDTO.Remarks);

                    generalLedger.LedgerNumber = _generalLedgerRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(LedgerNumber),0) + 1 AS Expr1 FROM {0}GeneralLedgers", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    generalLedger.Status = (int)GeneralLedgerStatus.Pending;
                    generalLedger.CreatedBy = serviceHeader.ApplicationUserName;

                    _generalLedgerRepository.Add(generalLedger, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return generalLedger.ProjectedAs<GeneralLedgerDTO>();
                }
            }
            else return null;
        }

        public bool UpdateGeneralLedger(GeneralLedgerDTO generalLedgerDTO, ServiceHeader serviceHeader)
        {
            if (generalLedgerDTO == null || generalLedgerDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _generalLedgerRepository.Get(generalLedgerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = GeneralLedgerFactory.CreateGeneralLedger(generalLedgerDTO.BranchId, generalLedgerDTO.PostingPeriodId, generalLedgerDTO.TotalValue, generalLedgerDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.LedgerNumber = persisted.LedgerNumber;
                    current.Status = persisted.Status;
                    current.CreatedBy = persisted.CreatedBy;


                    _generalLedgerRepository.Merge(persisted, current, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return persisted.TotalValue == persisted.GeneralLedgerEntries.Sum(x => x.Amount);
                    }
                    else return false;
                }
                else return false;
            }
        }

        public GeneralLedgerEntryDTO AddNewGeneralLedgerEntry(GeneralLedgerEntryDTO generalLedgerEntryDTO, ServiceHeader serviceHeader)
        {
            if (generalLedgerEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var generalLedgerEntry = GeneralLedgerEntryFactory.CreateGeneralLedgerEntry(generalLedgerEntryDTO.GeneralLedgerId, generalLedgerEntryDTO.BranchId, generalLedgerEntryDTO.ChartOfAccountId, generalLedgerEntryDTO.ContraChartOfAccountId, generalLedgerEntryDTO.CustomerAccountId, generalLedgerEntryDTO.ContraCustomerAccountId, generalLedgerEntryDTO.PrimaryDescription, generalLedgerEntryDTO.SecondaryDescription, generalLedgerEntryDTO.Reference, generalLedgerEntryDTO.Amount, generalLedgerEntryDTO.ValueDate);

                    generalLedgerEntry.Status = (int)GeneralLedgerEntryStatus.Pending;

                    generalLedgerEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _generalLedgerEntryRepository.Add(generalLedgerEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return generalLedgerEntry.ProjectedAs<GeneralLedgerEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveGeneralLedgerEntries(List<GeneralLedgerEntryDTO> generalLedgerEntryDTOs, ServiceHeader serviceHeader)
        {
            if (generalLedgerEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in generalLedgerEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _generalLedgerEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _generalLedgerEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, ServiceHeader serviceHeader)
        {
            if (generalLedgerDTO == null || !Enum.IsDefined(typeof(GeneralLedgerAuthOption), generalLedgerAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _generalLedgerRepository.Get(generalLedgerDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)GeneralLedgerStatus.Pending)
                    return false;

                switch ((GeneralLedgerAuthOption)generalLedgerAuthOption)
                {
                    case GeneralLedgerAuthOption.Post:

                        persisted.Status = (int)GeneralLedgerStatus.Audited;
                        persisted.AuditRemarks = generalLedgerDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case GeneralLedgerAuthOption.Reject:

                        persisted.Status = (int)GeneralLedgerStatus.Rejected;
                        persisted.AuditRemarks = generalLedgerDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (generalLedgerDTO == null || !Enum.IsDefined(typeof(GeneralLedgerAuthOption), generalLedgerAuthOption))
                return result;

            var journals = new List<Journal>();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _generalLedgerRepository.Get(generalLedgerDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)GeneralLedgerStatus.Audited)
                    return result;

                switch ((GeneralLedgerAuthOption)generalLedgerAuthOption)
                {
                    case GeneralLedgerAuthOption.Post:

                        var generalLedgerEntries = FindGeneralLedgerEntriesByGeneralLedgerId(persisted.Id, serviceHeader);

                        if (generalLedgerEntries != null && persisted.TotalValue == generalLedgerEntries.Sum(x => x.Amount))
                        {
                            var counter = 0;

                            foreach (var generalLedgerEntry in generalLedgerEntries)
                            {
                                counter += 1;

                                var reference = string.Format("{0}:{1}->{2} ({3}/{4})", persisted.Remarks, generalLedgerEntry.Reference, generalLedgerDTO.PaddedLedgerNumber, counter, generalLedgerEntries.Count);

                                var generalLedgerEntryJournal = JournalFactory.CreateJournal(null, persisted.PostingPeriodId, generalLedgerEntry.BranchId, null, generalLedgerEntry.Amount, generalLedgerEntry.PrimaryDescription, generalLedgerEntry.SecondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.GeneralLedger, generalLedgerEntry.ValueDate, serviceHeader);

                                if (generalLedgerEntry.CustomerAccountId.HasValue && generalLedgerEntry.CustomerAccountId != Guid.Empty && generalLedgerEntry.ContraCustomerAccountId.HasValue && generalLedgerEntry.ContraCustomerAccountId != Guid.Empty)
                                {
                                    _journalEntryPostingService.PerformDoubleEntry(generalLedgerEntryJournal, generalLedgerEntry.ChartOfAccountId, generalLedgerEntry.ContraChartOfAccountId, _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.CustomerAccountId.Value, serviceHeader), _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.ContraCustomerAccountId.Value, serviceHeader), serviceHeader);
                                }
                                else if (generalLedgerEntry.CustomerAccountId.HasValue && generalLedgerEntry.CustomerAccountId != Guid.Empty && (!generalLedgerEntry.ContraCustomerAccountId.HasValue || generalLedgerEntry.ContraCustomerAccountId == Guid.Empty))
                                {
                                    _journalEntryPostingService.PerformDoubleEntry(generalLedgerEntryJournal, generalLedgerEntry.ChartOfAccountId, generalLedgerEntry.ContraChartOfAccountId, _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.CustomerAccountId.Value, serviceHeader), _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.CustomerAccountId.Value, serviceHeader), serviceHeader);
                                }
                                else if (generalLedgerEntry.ContraCustomerAccountId.HasValue && generalLedgerEntry.ContraCustomerAccountId != Guid.Empty && (!generalLedgerEntry.CustomerAccountId.HasValue || generalLedgerEntry.CustomerAccountId == Guid.Empty))
                                {
                                    _journalEntryPostingService.PerformDoubleEntry(generalLedgerEntryJournal, generalLedgerEntry.ChartOfAccountId, generalLedgerEntry.ContraChartOfAccountId, _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.ContraCustomerAccountId.Value, serviceHeader), _sqlCommandAppService.FindCustomerAccountById(generalLedgerEntry.ContraCustomerAccountId.Value, serviceHeader), serviceHeader);
                                }
                                else _journalEntryPostingService.PerformDoubleEntry(generalLedgerEntryJournal, generalLedgerEntry.ChartOfAccountId, generalLedgerEntry.ContraChartOfAccountId, serviceHeader);

                                journals.Add(generalLedgerEntryJournal);

                                var batchEntry = _generalLedgerEntryRepository.Get(generalLedgerEntry.Id, serviceHeader);

                                if (batchEntry != null)
                                {
                                    batchEntry.Status = (int)ExpensePayableEntryStatus.Posted;
                                }
                            }

                            persisted.Status = (int)GeneralLedgerStatus.Posted;

                            persisted.AuthorizationRemarks = generalLedgerDTO.AuthorizationRemarks;

                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }
                        else throw new InvalidOperationException("Sorry, but requisite minimum requirements have not been satisfied viz. (batch total)");

                        break;
                    case GeneralLedgerAuthOption.Reject:

                        persisted.Status = (int)GeneralLedgerStatus.Rejected;

                        persisted.AuthorizationRemarks = generalLedgerDTO.AuthorizationRemarks;

                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        var rejectedGeneralLedgerEntries = FindGeneralLedgerEntriesByGeneralLedgerId(persisted.Id, serviceHeader);

                        if (rejectedGeneralLedgerEntries != null && rejectedGeneralLedgerEntries.Any())
                        {
                            foreach (var item in rejectedGeneralLedgerEntries)
                            {
                                var batchEntry = _generalLedgerEntryRepository.Get(item.Id, serviceHeader);

                                if (batchEntry != null)
                                {
                                    batchEntry.Status = (int)GeneralLedgerEntryStatus.Rejected;
                                }
                            }
                        }

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result && journals.Any())
            {
                result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
            }

            return result;
        }

        public bool UpdateGeneralLedgerEntryCollection(Guid generalLedgerId, List<GeneralLedgerEntryDTO> generalLedgerEntryCollection, ServiceHeader serviceHeader)
        {
            if (generalLedgerId != null && generalLedgerEntryCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _generalLedgerRepository.Get(generalLedgerId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindGeneralLedgerEntriesByGeneralLedgerId(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var generalLedgerEntry = _generalLedgerEntryRepository.Get(item.Id, serviceHeader);

                                if (generalLedgerEntry != null)
                                {
                                    _generalLedgerEntryRepository.Remove(generalLedgerEntry, serviceHeader);
                                }
                            }
                        }

                        if (generalLedgerEntryCollection.Any())
                        {
                            foreach (var item in generalLedgerEntryCollection)
                            {
                                var generalLedgerEntry = GeneralLedgerEntryFactory.CreateGeneralLedgerEntry(persisted.Id, item.BranchId, item.ChartOfAccountId, item.ContraChartOfAccountId, item.CustomerAccountId, item.ContraCustomerAccountId, item.PrimaryDescription, item.SecondaryDescription, item.Reference, item.Amount, item.ValueDate);

                                generalLedgerEntry.Status = (int)GeneralLedgerEntryStatus.Pending;

                                generalLedgerEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                _generalLedgerEntryRepository.Add(generalLedgerEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<GeneralLedgerDTO> FindGeneralLedgers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var generalLedgers = _generalLedgerRepository.GetAll(serviceHeader);

                if (generalLedgers != null && generalLedgers.Any())
                {
                    return generalLedgers.ProjectedAsCollection<GeneralLedgerDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = GeneralLedgerSpecifications.DefaultSpec();

                ISpecification<GeneralLedger> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var generalLedgerPagedCollection = _generalLedgerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (generalLedgerPagedCollection != null)
                {
                    var pageCollection = generalLedgerPagedCollection.PageCollection.ProjectedAsCollection<GeneralLedgerDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            var postedItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.PostedGeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = generalLedgerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<GeneralLedgerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = GeneralLedgerSpecifications.GeneralLedgerWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<GeneralLedger> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var generalLedgerPagedCollection = _generalLedgerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (generalLedgerPagedCollection != null)
                {
                    var pageCollection = generalLedgerPagedCollection.PageCollection.ProjectedAsCollection<GeneralLedgerDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            var postedItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.PostedGeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = generalLedgerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<GeneralLedgerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgers(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = GeneralLedgerSpecifications.GeneralLedgersWithStatus(status, startDate, endDate, text);

                ISpecification<GeneralLedger> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var generalLedgerPagedCollection = _generalLedgerRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (generalLedgerPagedCollection != null)
                {
                    var pageCollection = generalLedgerPagedCollection.PageCollection.ProjectedAsCollection<GeneralLedgerDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            var postedItems = _generalLedgerEntryRepository.AllMatchingCount(GeneralLedgerEntrySpecifications.PostedGeneralLedgerEntryWithGeneralLedgerId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = generalLedgerPagedCollection.ItemsCount;

                    return new PageCollectionInfo<GeneralLedgerDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public GeneralLedgerDTO FindGeneralLedger(Guid generalLedgerId, ServiceHeader serviceHeader)
        {
            if (generalLedgerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var generalLedger = _generalLedgerRepository.Get(generalLedgerId, serviceHeader);

                    if (generalLedger != null)
                    {
                        return generalLedger.ProjectedAs<GeneralLedgerDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId, ServiceHeader serviceHeader)
        {
            if (generalLedgerId != null && generalLedgerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(generalLedgerId);

                    ISpecification<GeneralLedgerEntry> spec = filter;

                    var generalLedgerEntries = _generalLedgerEntryRepository.AllMatching(spec, serviceHeader);

                    if (generalLedgerEntries != null && generalLedgerEntries.Any())
                    {
                        return generalLedgerEntries.ProjectedAsCollection<GeneralLedgerEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (generalLedgerId != null && generalLedgerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(generalLedgerId);

                    ISpecification<GeneralLedgerEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var generalLedgerPagedCollection = _generalLedgerEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (generalLedgerPagedCollection != null)
                    {
                        var persisted = _generalLedgerRepository.Get(generalLedgerId, serviceHeader);

                        var persistedEntriesTotal = 0m;

                        var generalLedgerEntries = _generalLedgerEntryRepository.AllMatching(GeneralLedgerEntrySpecifications.GeneralLedgerEntryWithGeneralLedgerId(persisted.Id), serviceHeader);

                        if (generalLedgerEntries != null && generalLedgerEntries.Any())
                            persistedEntriesTotal = generalLedgerEntries.Sum(x => x.Amount);

                        var pageCollection = generalLedgerPagedCollection.PageCollection.ProjectedAsCollection<GeneralLedgerEntryDTO>();

                        var itemsCount = generalLedgerPagedCollection.ItemsCount;

                        return new PageCollectionInfo<GeneralLedgerEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public BatchImportParseInfo ParseGeneralLedgerImportEntries(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            BatchImportParseInfo parseInfo = null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                if (!string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (File.Exists(path))
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

                                if (dataRecord.Count == 17)
                                {
                                    var importEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //DebitType
                                        Column2 = dataRecord[1], //DebitChartOfAccountCode
                                        Column3 = dataRecord[2], //DebitCustomerPayrollNumber
                                        Column4 = dataRecord[3], //DebitProductCode
                                        Column5 = dataRecord[4], //DebitTargetProductCode
                                        Column6 = dataRecord[5], //DebitCustomerAccountComponent
                                        Column7 = dataRecord[6], //CreditType
                                        Column8 = dataRecord[7], //CreditChartOfAccountCode
                                        Column9 = dataRecord[8], //CreditCustomerPayrollNumber
                                        Column10 = dataRecord[9], //CreditProductCode
                                        Column11 = dataRecord[10], //CreditTargetProductCode
                                        Column12 = dataRecord[11], //CreditCustomerAccountComponent
                                        Column13 = dataRecord[12], //Amount
                                        Column14 = dataRecord[13], //ValueDate
                                        Column15 = dataRecord[14], //PrimaryDescription
                                        Column16 = dataRecord[15], //SecondaryDescription
                                        Column17 = dataRecord[16], //YourReference
                                    };

                                    importEntries.Add(importEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            var result = new BatchImportParseInfo
                            {
                                MatchedCollection9 = new List<GeneralLedgerEntryDTO> { },
                                MismatchedCollection = new List<BatchImportEntryWrapper> { }
                            };

                            var count = 0;

                            importEntries.ForEach(item =>
                            {
                                var debitType = default(int);

                                var creditType = default(int);

                                var amount = default(int);

                                var valuedate = new DateTime();

                                if (int.TryParse(item.Column1, NumberStyles.Any, CultureInfo.InvariantCulture, out debitType) && int.TryParse(item.Column7, NumberStyles.Any, CultureInfo.InvariantCulture, out creditType)
                                && int.TryParse(item.Column13, NumberStyles.Any, CultureInfo.InvariantCulture, out amount) && DateTime.TryParse(item.Column14, out valuedate))
                                {
                                    var productCode = default(int);

                                    var code = default(int);

                                    var chartOfAccountCode = default(int);

                                    #region General Ledger Entry Mapping - Debit

                                    switch ((GeneralLedgerEntryType)debitType)
                                    {
                                        case GeneralLedgerEntryType.GLAccount:

                                            if ((int.TryParse(item.Column2, NumberStyles.Any, CultureInfo.InvariantCulture, out chartOfAccountCode)))
                                            {
                                                var chartOfAccountDTO = _chartOfAccountAppService.FindChartOfAccounts(chartOfAccountCode, serviceHeader).SingleOrDefault();

                                                if (chartOfAccountDTO != null)
                                                {
                                                    generalLedgerEntryDTO.ContraChartOfAccountId = chartOfAccountDTO.Id;
                                                    generalLedgerEntryDTO.ContraChartOfAccountAccountType = chartOfAccountDTO.AccountType;
                                                    generalLedgerEntryDTO.ContraChartOfAccountAccountCode = chartOfAccountDTO.AccountCode;
                                                    generalLedgerEntryDTO.ContraChartOfAccountAccountName = chartOfAccountDTO.AccountName;
                                                }

                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, no match general ledger account with code {1}", count, chartOfAccountCode);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ unable to parse code {1}", count, item.Column2);

                                                result.MismatchedCollection.Add(item);
                                            }

                                            break;

                                        case GeneralLedgerEntryType.Customer:

                                            if (int.TryParse(item.Column4, NumberStyles.Any, CultureInfo.InvariantCulture, out productCode) && int.TryParse(item.Column5, NumberStyles.Any, CultureInfo.InvariantCulture, out code))
                                            {
                                                var customerDTO = _sqlCommandAppService.FindCustomersByPayrollNumber(item.Column3, serviceHeader).SingleOrDefault();

                                                if (customerDTO != null)
                                                {
                                                    switch ((ProductCode)productCode)
                                                    {
                                                        case ProductCode.Investment:

                                                            var investmentProductDTO = _investmentProductAppService.FindInvestmentProducts(code, serviceHeader).SingleOrDefault();

                                                            if (investmentProductDTO != null)
                                                            {
                                                                var debitCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, investmentProductDTO.Id, serviceHeader);

                                                                if (debitCustomerAccountDTOs != null)
                                                                {
                                                                    var debitCustomerAccount = debitCustomerAccountDTOs[0];

                                                                    generalLedgerEntryDTO.ContraCustomerAccountId = debitCustomerAccount.Id;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountBranchCode = debitCustomerAccount.BranchCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerSerialNumber = debitCustomerAccount.CustomerSerialNumber;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeProductCode = debitCustomerAccount.CustomerAccountTypeProductCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeTargetProductCode = debitCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerType = debitCustomerAccount.CustomerType;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualSalutation = debitCustomerAccount.CustomerIndividualSalutation;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualFirstName = debitCustomerAccount.CustomerIndividualFirstName;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualLastName = debitCustomerAccount.CustomerIndividualLastName;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerNonIndividualDescription = debitCustomerAccount.CustomerNonIndividualDescription;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference1 = debitCustomerAccount.CustomerReference1;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference2 = debitCustomerAccount.CustomerReference2;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference3 = debitCustomerAccount.CustomerReference3;
                                                                    generalLedgerEntryDTO.ChartOfAccountId = investmentProductDTO.ChartOfAccountId;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = investmentProductDTO.ChartOfAccountAccountType;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = investmentProductDTO.ChartOfAccountAccountCode;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = investmentProductDTO.ChartOfAccountAccountName;
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, the debit customer account type with target product code {1} could not be found.", count, code);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                item.Remarks = string.Format("Record #{0} ~ unable to parse, the investment product with code {1} could not be found.", count, code);

                                                                result.MismatchedCollection.Add(item);
                                                            }

                                                            break;

                                                        case ProductCode.Savings:

                                                            var savingsProductDTO = _savingsProductAppService.FindSavingsProducts(code, serviceHeader).SingleOrDefault();

                                                            if (savingsProductDTO != null)
                                                            {
                                                                var debitCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, savingsProductDTO.Id, serviceHeader);

                                                                if (debitCustomerAccountDTOs != null)
                                                                {
                                                                    var debitCustomerAccount = debitCustomerAccountDTOs[0];

                                                                    generalLedgerEntryDTO.ContraCustomerAccountId = debitCustomerAccount.Id;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountBranchCode = debitCustomerAccount.BranchCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerSerialNumber = debitCustomerAccount.CustomerSerialNumber;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeProductCode = debitCustomerAccount.CustomerAccountTypeProductCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeTargetProductCode = debitCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerType = debitCustomerAccount.CustomerType;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualSalutation = debitCustomerAccount.CustomerIndividualSalutation;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualFirstName = debitCustomerAccount.CustomerIndividualFirstName;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualLastName = debitCustomerAccount.CustomerIndividualLastName;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerNonIndividualDescription = debitCustomerAccount.CustomerNonIndividualDescription;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference1 = debitCustomerAccount.CustomerReference1;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference2 = debitCustomerAccount.CustomerReference2;
                                                                    generalLedgerEntryDTO.ContraCustomerAccountCustomerReference3 = debitCustomerAccount.CustomerReference3;
                                                                    generalLedgerEntryDTO.ChartOfAccountId = savingsProductDTO.ChartOfAccountId;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = savingsProductDTO.ChartOfAccountAccountType;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = savingsProductDTO.ChartOfAccountAccountCode;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = savingsProductDTO.ChartOfAccountAccountName;
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, the debit customer account type with target product code {1} could not be found.", count, code);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                item.Remarks = string.Format("Record #{0} ~ unable to parse, the savings product with code {1} could not be found.", count, code);

                                                                result.MismatchedCollection.Add(item);
                                                            }

                                                            break;

                                                        case ProductCode.Loan:

                                                            var debitCustomerAccountComponent = default(int);

                                                            if ((int.TryParse(item.Column12, NumberStyles.Any, CultureInfo.InvariantCulture, out debitCustomerAccountComponent)))
                                                            {
                                                                if (debitCustomerAccountComponent.In(1, 2, 3, 4))
                                                                {
                                                                    var loanProductDTO = _loanProductAppService.FindLoanProducts(code, serviceHeader).SingleOrDefault();

                                                                    if (loanProductDTO != null)
                                                                    {
                                                                        var debitCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, loanProductDTO.Id, serviceHeader);

                                                                        if (debitCustomerAccountDTOs != null)
                                                                        {
                                                                            var debitCustomerAccount = debitCustomerAccountDTOs[0];

                                                                            generalLedgerEntryDTO.ContraCustomerAccountId = debitCustomerAccount.Id;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountBranchCode = debitCustomerAccount.BranchCode;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerSerialNumber = debitCustomerAccount.CustomerSerialNumber;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeProductCode = debitCustomerAccount.CustomerAccountTypeProductCode;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerAccountTypeTargetProductCode = debitCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerType = debitCustomerAccount.CustomerType;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualSalutation = debitCustomerAccount.CustomerIndividualSalutation;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualFirstName = debitCustomerAccount.CustomerIndividualFirstName;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerIndividualLastName = debitCustomerAccount.CustomerIndividualLastName;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerNonIndividualDescription = debitCustomerAccount.CustomerNonIndividualDescription;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerReference1 = debitCustomerAccount.CustomerReference1;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerReference2 = debitCustomerAccount.CustomerReference2;
                                                                            generalLedgerEntryDTO.ContraCustomerAccountCustomerReference3 = debitCustomerAccount.CustomerReference3;

                                                                            switch ((CustomerAccountComponent)debitCustomerAccountComponent)
                                                                            {
                                                                                case CustomerAccountComponent.Principal:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.ChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.ChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.ChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.ChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestReceivable:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestReceivableChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestReceivableChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestReceivableChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestReceivableChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestReceived:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestReceivedChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestReceivedChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestReceivedChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestReceivedChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestCharged:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestChargedChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestChargedChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestChargedChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestChargedChartOfAccountAccountName;
                                                                                    break;
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            item.Remarks = string.Format("Record #{0} ~ unable to parse, the debit customer account type with target product code {1} could not be found.", count, code);

                                                                            result.MismatchedCollection.Add(item);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        item.Remarks = string.Format("Record #{0} ~ unable to parse, the loan product with code {1} could not be found.", count, code);

                                                                        result.MismatchedCollection.Add(item);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, debit customer account Component {1} could not be found.", count, debitCustomerAccountComponent);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }

                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, there is no customer matches by payroll number {1}", count, item.Column3);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }

                                            break;

                                        default:

                                            item.Remarks = string.Format("Record #{0} ~ no debit match for general ledger entry type.", count);

                                            result.MismatchedCollection.Add(item);

                                            break;
                                    }

                                    #endregion

                                    #region General Ledger Entry Mapping - Credit

                                    switch ((GeneralLedgerEntryType)creditType)
                                    {
                                        case GeneralLedgerEntryType.GLAccount:

                                            if ((int.TryParse(item.Column8, NumberStyles.Any, CultureInfo.InvariantCulture, out chartOfAccountCode)))
                                            {
                                                var chartOfAccountDTO = _chartOfAccountAppService.FindChartOfAccounts(chartOfAccountCode, serviceHeader).SingleOrDefault();

                                                if (chartOfAccountDTO != null)
                                                {
                                                    generalLedgerEntryDTO.ChartOfAccountId = chartOfAccountDTO.Id;
                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = chartOfAccountDTO.AccountType;
                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = chartOfAccountDTO.AccountCode;
                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = chartOfAccountDTO.AccountName;
                                                }

                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, no match general ledger account with code {1}", count, chartOfAccountCode);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ unable to parse code {1}", count, item.Column2);

                                                result.MismatchedCollection.Add(item);
                                            }

                                            break;

                                        case GeneralLedgerEntryType.Customer:

                                            if (int.TryParse(item.Column10, NumberStyles.Any, CultureInfo.InvariantCulture, out productCode) && int.TryParse(item.Column11, NumberStyles.Any, CultureInfo.InvariantCulture, out code))
                                            {
                                                var customerDTO = _sqlCommandAppService.FindCustomersByPayrollNumber(item.Column9, serviceHeader).SingleOrDefault();

                                                if (customerDTO != null)
                                                {
                                                    switch ((ProductCode)productCode)
                                                    {
                                                        case ProductCode.Investment:

                                                            var investmentProductDTO = _investmentProductAppService.FindInvestmentProducts(code, serviceHeader).SingleOrDefault();

                                                            if (investmentProductDTO != null)
                                                            {
                                                                var creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, investmentProductDTO.Id, serviceHeader);

                                                                if (creditCustomerAccountDTOs != null)
                                                                {
                                                                    var creditCustomerAccount = creditCustomerAccountDTOs[0];

                                                                    generalLedgerEntryDTO.CustomerAccountId = creditCustomerAccount.Id;
                                                                    generalLedgerEntryDTO.CustomerAccountBranchCode = creditCustomerAccount.BranchCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerSerialNumber = creditCustomerAccount.CustomerSerialNumber;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeProductCode = creditCustomerAccount.CustomerAccountTypeProductCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeTargetProductCode = creditCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerType = creditCustomerAccount.CustomerType;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualSalutation = creditCustomerAccount.CustomerIndividualSalutation;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualFirstName = creditCustomerAccount.CustomerIndividualFirstName;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualLastName = creditCustomerAccount.CustomerIndividualLastName;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerNonIndividualDescription = creditCustomerAccount.CustomerNonIndividualDescription;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference1 = creditCustomerAccount.CustomerReference1;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference2 = creditCustomerAccount.CustomerReference2;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference3 = creditCustomerAccount.CustomerReference3;
                                                                    generalLedgerEntryDTO.ChartOfAccountId = investmentProductDTO.ChartOfAccountId;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = investmentProductDTO.ChartOfAccountAccountType;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = investmentProductDTO.ChartOfAccountAccountCode;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = investmentProductDTO.ChartOfAccountAccountName;
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, the credit customer account type with target product code {1} could not be found.", count, code);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                item.Remarks = string.Format("Record #{0} ~ unable to parse, the investment product with code {1} could not be found.", count, code);

                                                                result.MismatchedCollection.Add(item);
                                                            }

                                                            break;

                                                        case ProductCode.Savings:

                                                            var savingsProductDTO = _savingsProductAppService.FindSavingsProducts(code, serviceHeader).SingleOrDefault();

                                                            if (savingsProductDTO != null)
                                                            {
                                                                var creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, savingsProductDTO.Id, serviceHeader);

                                                                if (creditCustomerAccountDTOs != null)
                                                                {
                                                                    var creditCustomerAccount = creditCustomerAccountDTOs[0];

                                                                    generalLedgerEntryDTO.CustomerAccountId = creditCustomerAccount.Id;
                                                                    generalLedgerEntryDTO.CustomerAccountBranchCode = creditCustomerAccount.BranchCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerSerialNumber = creditCustomerAccount.CustomerSerialNumber;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeProductCode = creditCustomerAccount.CustomerAccountTypeProductCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeTargetProductCode = creditCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerType = creditCustomerAccount.CustomerType;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualSalutation = creditCustomerAccount.CustomerIndividualSalutation;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualFirstName = creditCustomerAccount.CustomerIndividualFirstName;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerIndividualLastName = creditCustomerAccount.CustomerIndividualLastName;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerNonIndividualDescription = creditCustomerAccount.CustomerNonIndividualDescription;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference1 = creditCustomerAccount.CustomerReference1;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference2 = creditCustomerAccount.CustomerReference2;
                                                                    generalLedgerEntryDTO.CustomerAccountCustomerReference3 = creditCustomerAccount.CustomerReference3;
                                                                    generalLedgerEntryDTO.ChartOfAccountId = savingsProductDTO.ChartOfAccountId;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = savingsProductDTO.ChartOfAccountAccountType;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = savingsProductDTO.ChartOfAccountAccountCode;
                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = savingsProductDTO.ChartOfAccountAccountName;
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, the credit customer account type with target product code {1} could not be found.", count, code);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                item.Remarks = string.Format("Record #{0} ~ unable to parse, the savings product with code {1} could not be found.", count, code);

                                                                result.MismatchedCollection.Add(item);
                                                            }

                                                            break;

                                                        case ProductCode.Loan:

                                                            var creditCustomerAccountComponent = default(int);

                                                            if ((int.TryParse(item.Column12, NumberStyles.Any, CultureInfo.InvariantCulture, out creditCustomerAccountComponent)))
                                                            {
                                                                if (creditCustomerAccountComponent == 1 || creditCustomerAccountComponent == 2)
                                                                {
                                                                    var loanProductDTO = _loanProductAppService.FindLoanProducts(code, serviceHeader).SingleOrDefault();

                                                                    if (loanProductDTO != null)
                                                                    {
                                                                        var creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerDTO.Id, loanProductDTO.Id, serviceHeader);

                                                                        if (creditCustomerAccountDTOs != null)
                                                                        {
                                                                            var creditCustomerAccount = creditCustomerAccountDTOs[0];

                                                                            generalLedgerEntryDTO.CustomerAccountId = creditCustomerAccount.Id;
                                                                            generalLedgerEntryDTO.CustomerAccountBranchCode = creditCustomerAccount.BranchCode;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerSerialNumber = creditCustomerAccount.CustomerSerialNumber;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeProductCode = creditCustomerAccount.CustomerAccountTypeProductCode;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerAccountTypeTargetProductCode = creditCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerType = creditCustomerAccount.CustomerType;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerIndividualSalutation = creditCustomerAccount.CustomerIndividualSalutation;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerIndividualFirstName = creditCustomerAccount.CustomerIndividualFirstName;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerIndividualLastName = creditCustomerAccount.CustomerIndividualLastName;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerNonIndividualDescription = creditCustomerAccount.CustomerNonIndividualDescription;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerReference1 = creditCustomerAccount.CustomerReference1;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerReference2 = creditCustomerAccount.CustomerReference2;
                                                                            generalLedgerEntryDTO.CustomerAccountCustomerReference3 = creditCustomerAccount.CustomerReference3;

                                                                            switch ((CustomerAccountComponent)creditCustomerAccountComponent)
                                                                            {
                                                                                case CustomerAccountComponent.Principal:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.ChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.ChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.ChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.ChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestReceivable:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestReceivableChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestReceivableChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestReceivableChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestReceivableChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestReceived:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestReceivedChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestReceivedChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestReceivedChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestReceivedChartOfAccountAccountName;
                                                                                    break;
                                                                                case CustomerAccountComponent.InterestCharged:
                                                                                    generalLedgerEntryDTO.ChartOfAccountId = loanProductDTO.InterestChargedChartOfAccountId;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountType = loanProductDTO.InterestChargedChartOfAccountAccountType;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountCode = loanProductDTO.InterestChargedChartOfAccountAccountCode;
                                                                                    generalLedgerEntryDTO.ChartOfAccountAccountName = loanProductDTO.InterestChargedChartOfAccountAccountName;
                                                                                    break;
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            item.Remarks = string.Format("Record #{0} ~ unable to parse, the credit customer account type with target product code {1} could not be found.", count, code);

                                                                            result.MismatchedCollection.Add(item);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        item.Remarks = string.Format("Record #{0} ~ unable to parse, the loan product with code {1} could not be found.", count, code);

                                                                        result.MismatchedCollection.Add(item);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, credit customer account Component {1} could not be found.", count, creditCustomerAccountComponent);

                                                                    result.MismatchedCollection.Add(item);
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ unable to parse, there is no customer matches by payroll number {1}", count, item.Column3);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }

                                            break;

                                        default:

                                            item.Remarks = string.Format("Record #{0} ~ no match for credit type.", count);

                                            result.MismatchedCollection.Add(item);

                                            break;
                                    }

                                    #endregion

                                    generalLedgerEntryDTO.Amount = amount;
                                    generalLedgerEntryDTO.ValueDate = valuedate;
                                    generalLedgerEntryDTO.PrimaryDescription = item.Column15;
                                    generalLedgerEntryDTO.SecondaryDescription = item.Column16;
                                    generalLedgerEntryDTO.Reference = item.Column17;

                                    generalLedgerEntryDTO.ValidateAll();

                                    if (generalLedgerEntryDTO.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, generalLedgerEntryDTO.ErrorMessages));

                                    result.MatchedCollection9.Add(generalLedgerEntryDTO);
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ either of the credittype/debittype/amount/valuedate values are not valid.", count);

                                    result.MismatchedCollection.Add(item);
                                }
                                // tally
                                count += 1;
                            });

                            parseInfo = result;
                        }
                    }
                    return parseInfo;
                }
                return parseInfo;
            }
        }
    }
}
