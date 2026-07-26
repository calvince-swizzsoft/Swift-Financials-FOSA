using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using KBCsv;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class WireTransferBatchAppService : IWireTransferBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<WireTransferBatch> _wireTransferBatchRepository;
        private readonly IRepository<WireTransferBatchEntry> _wireTransferBatchEntryRepository;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public WireTransferBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<WireTransferBatch> wireTransferBatchRepository,
           IRepository<WireTransferBatchEntry> wireTransferBatchEntryRepository,
           ISavingsProductAppService savingsProductAppService,
           IPostingPeriodAppService postingPeriodAppService,
           ICommissionAppService commissionAppService,
           IJournalAppService journalAppService,
           ISqlCommandAppService sqlCommandAppService,
           IBrokerService brokerService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (wireTransferBatchRepository == null)
                throw new ArgumentNullException(nameof(wireTransferBatchRepository));

            if (wireTransferBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(wireTransferBatchEntryRepository));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _wireTransferBatchRepository = wireTransferBatchRepository;
            _wireTransferBatchEntryRepository = wireTransferBatchEntryRepository;
            _savingsProductAppService = savingsProductAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _commissionAppService = commissionAppService;
            _journalAppService = journalAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _brokerService = brokerService;
            _appCache = appCache;
        }

        public WireTransferBatchDTO AddNewWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var wireTransferBatch = WireTransferBatchFactory.CreateWireTransferBatch(wireTransferBatchDTO.WireTransferTypeId, wireTransferBatchDTO.BranchId, wireTransferBatchDTO.TotalValue, wireTransferBatchDTO.Type, wireTransferBatchDTO.Reference, wireTransferBatchDTO.Priority);

                    wireTransferBatch.BatchNumber = _wireTransferBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}WireTransferBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    wireTransferBatch.Status = (int)BatchStatus.Pending;
                    wireTransferBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _wireTransferBatchRepository.Add(wireTransferBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return wireTransferBatch.ProjectedAs<WireTransferBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchDTO == null || wireTransferBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchRepository.Get(wireTransferBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.TotalValue = wireTransferBatchDTO.TotalValue;
                    persisted.Reference = wireTransferBatchDTO.Reference;
                    persisted.Priority = (byte)wireTransferBatchDTO.Priority;

                    dbContextScope.SaveChanges(serviceHeader);

                    var persistedEntriesTotal = _sqlCommandAppService.FindWireTransferBatchEntriesTotal(persisted.Id, serviceHeader);

                    return persisted.TotalValue >= persistedEntriesTotal;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool AuditWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchRepository.Get(wireTransferBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var entriesTotal = _sqlCommandAppService.FindWireTransferBatchEntriesTotal(persisted.Id, serviceHeader);

                        if (persisted.TotalValue >= entriesTotal)
                        {
                            persisted.Status = (int)BatchStatus.Audited;
                            persisted.AuditRemarks = wireTransferBatchDTO.AuditRemarks;
                            persisted.AuditedBy = serviceHeader.ApplicationUserName;
                            persisted.AuditedDate = DateTime.Now;
                        }

                        break;
                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = wireTransferBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeWireTransferBatch(WireTransferBatchDTO wireTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (wireTransferBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchRepository.Get(wireTransferBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return result;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var entriesTotal = _sqlCommandAppService.FindWireTransferBatchEntriesTotal(persisted.Id, serviceHeader);

                        if (persisted.TotalValue >= entriesTotal)
                        {
                            persisted.Status = (int)BatchStatus.Posted;
                            persisted.AuthorizationRemarks = wireTransferBatchDTO.AuthorizationRemarks;
                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }

                        break;
                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = wireTransferBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                if (result && batchAuthOption == (int)BatchAuthOption.Post)
                {
                    var query = _wireTransferBatchRepository.DatabaseSqlQuery<Guid>(string.Format(
                          @"SELECT Id
                            FROM  {0}WireTransferBatchEntries
                            WHERE(WireTransferBatchId = @WireTransferBatchId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                            new SqlParameter("WireTransferBatchId", wireTransferBatchDTO.Id));

                    if (query != null)
                    {
                        var data = from l in query
                                   select new WireTransferBatchEntryDTO
                                   {
                                       Id = l,
                                       WireTransferBatchPriority = wireTransferBatchDTO.Priority
                                   };

                        _brokerService.ProcessWireTransferBatchEntries(DMLCommand.None, serviceHeader, data.ToArray());
                    }
                }
            }

            return result;
        }

        public WireTransferBatchEntryDTO AddNewWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var wireTransferBatchEntry = WireTransferBatchEntryFactory.CreateWireTransferBatchEntry(wireTransferBatchEntryDTO.WireTransferBatchId, wireTransferBatchEntryDTO.CustomerAccountId, wireTransferBatchEntryDTO.Amount, wireTransferBatchEntryDTO.Payee, wireTransferBatchEntryDTO.AccountNumber, wireTransferBatchEntryDTO.Reference);

                    wireTransferBatchEntry.Status = (int)BatchEntryStatus.Pending;
                    wireTransferBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _wireTransferBatchEntryRepository.Add(wireTransferBatchEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return wireTransferBatchEntry.ProjectedAs<WireTransferBatchEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveWireTransferBatchEntries(List<WireTransferBatchEntryDTO> wireTransferBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in wireTransferBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _wireTransferBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _wireTransferBatchEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateWireTransferBatchEntry(WireTransferBatchEntryDTO wireTransferBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchEntryDTO == null || wireTransferBatchEntryDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchEntryRepository.Get(wireTransferBatchEntryDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status < wireTransferBatchEntryDTO.Status/*status flags can only go up?*/)
                {
                    persisted.Status = (byte)wireTransferBatchEntryDTO.Status;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool PostWireTransferBatchEntry(Guid wireTransferBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (MarkWireTransferBatchEntryPosted(wireTransferBatchEntryId, serviceHeader))
            {
                var wireTransferBatchEntryDTO = FindWireTransferBatchEntry(wireTransferBatchEntryId, serviceHeader);
                if (wireTransferBatchEntryDTO == null || wireTransferBatchEntryDTO.Status != (int)BatchEntryStatus.Posted)
                    return result;

                var wireTransferBatchDTO = FindCachedWireTransferBatch(wireTransferBatchEntryDTO.WireTransferBatchId, serviceHeader);
                if (wireTransferBatchDTO == null)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                serviceHeader.ApplicationUserName = wireTransferBatchDTO.AuthorizedBy ?? serviceHeader.ApplicationUserName;

                var primaryDescription = wireTransferBatchDTO.Reference;

                var secondaryDescription = string.Format("{0}~{1}~{2}", wireTransferBatchDTO.TypeDescription, wireTransferBatchEntryDTO.Payee, wireTransferBatchEntryDTO.AccountNumber);

                var reference = string.Format("{0}~{1}", wireTransferBatchDTO.PaddedBatchNumber, wireTransferBatchEntryDTO.Reference);

                var transactionOwnershipBranchId = Guid.Empty;

                var wireTransferCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(wireTransferBatchEntryDTO.CustomerAccountId, serviceHeader);

                switch ((TransactionOwnership)wireTransferBatchDTO.WireTransferTypeTransactionOwnership)
                {
                    case TransactionOwnership.InitiatingBranch:
                        transactionOwnershipBranchId = wireTransferBatchDTO.BranchId;
                        break;
                    case TransactionOwnership.BeneficiaryBranch:
                        transactionOwnershipBranchId = wireTransferCustomerAccount.BranchId;
                        break;
                    default:
                        break;
                }

                var wireTransferTariffs = _commissionAppService.ComputeTariffsByWireTransferType(wireTransferBatchDTO.WireTransferTypeId, wireTransferBatchEntryDTO.Amount, wireTransferCustomerAccount, serviceHeader);

                var totalDebitAmount = wireTransferBatchEntryDTO.Amount + wireTransferTariffs.Sum(x => x.Amount);

                var availableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(wireTransferCustomerAccount, DateTime.Now, serviceHeader);

                if (totalDebitAmount <= availableBalance)
                {
                    var targetSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(wireTransferCustomerAccount.CustomerAccountTypeTargetProductId, wireTransferCustomerAccount.BranchId, serviceHeader);

                    var primaryJournal = _journalAppService.AddNewJournal(transactionOwnershipBranchId, null, wireTransferBatchEntryDTO.Amount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.WireTransferBatch, null, wireTransferBatchDTO.WireTransferTypeChartOfAccountId, targetSavingsProduct.ChartOfAccountId, wireTransferCustomerAccount, wireTransferCustomerAccount, wireTransferTariffs, serviceHeader, true);

                    result = primaryJournal != null;
                }
                else result = MarkWireTransferBatchEntryRejected(wireTransferBatchEntryId, serviceHeader);
            }

            return result;
        }

        public List<WireTransferBatchDTO> FindWireTransferBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var wireTransferBatches = _wireTransferBatchRepository.GetAll(serviceHeader);

                if (wireTransferBatches != null && wireTransferBatches.Any())
                {
                    return wireTransferBatches.ProjectedAsCollection<WireTransferBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<WireTransferBatchDTO> FindWireTransferBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WireTransferBatchSpecifications.WireTransferBatchesWithStatus(status, startDate, endDate, text);

                ISpecification<WireTransferBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var wireTransferBatchPagedCollection = _wireTransferBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (wireTransferBatchPagedCollection != null)
                {
                    var pageCollection = wireTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<WireTransferBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _wireTransferBatchEntryRepository.AllMatchingCount(WireTransferBatchEntrySpecifications.WireTransferBatchEntryWithWireTransferBatchId(item.Id, null), serviceHeader);

                            var postedItems = _wireTransferBatchEntryRepository.AllMatchingCount(WireTransferBatchEntrySpecifications.PostedWireTransferBatchEntryWithWireTransferBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = wireTransferBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<WireTransferBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public WireTransferBatchDTO FindWireTransferBatch(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var wireTransferBatch = _wireTransferBatchRepository.Get(wireTransferBatchId, serviceHeader);

                    if (wireTransferBatch != null)
                    {
                        return wireTransferBatch.ProjectedAs<WireTransferBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public WireTransferBatchDTO FindCachedWireTransferBatch(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<WireTransferBatchDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, wireTransferBatchId.ToString("D")), () =>
            {
                return FindWireTransferBatch(wireTransferBatchId, serviceHeader);
            });
        }

        public WireTransferBatchEntryDTO FindWireTransferBatchEntry(Guid wireTransferBatchEntryId, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var wireTransferBatchEntry = _wireTransferBatchEntryRepository.Get(wireTransferBatchEntryId, serviceHeader);

                    if (wireTransferBatchEntry != null)
                    {
                        return wireTransferBatchEntry.ProjectedAs<WireTransferBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchId != null && wireTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WireTransferBatchEntrySpecifications.WireTransferBatchEntryWithWireTransferBatchId(wireTransferBatchId, null);

                    ISpecification<WireTransferBatchEntry> spec = filter;

                    var wireTransferBatchEntries = _wireTransferBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (wireTransferBatchEntries != null && wireTransferBatchEntries.Any())
                    {
                        return wireTransferBatchEntries.ProjectedAsCollection<WireTransferBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<WireTransferBatchEntryDTO> FindWireTransferBatchEntriesByWireTransferBatchId(Guid wireTransferBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (wireTransferBatchId != null && wireTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WireTransferBatchEntrySpecifications.WireTransferBatchEntryWithWireTransferBatchId(wireTransferBatchId, text);

                    ISpecification<WireTransferBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var wireTransferBatchEntryPagedCollection = _wireTransferBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (wireTransferBatchEntryPagedCollection != null)
                    {
                        var persisted = _wireTransferBatchRepository.Get(wireTransferBatchId, serviceHeader);

                        var persistedEntriesTotal = (FindWireTransferBatchEntriesByWireTransferBatchId(wireTransferBatchId, serviceHeader) ?? new List<WireTransferBatchEntryDTO>()).Sum(x => x.Amount);

                        var pageCollection = wireTransferBatchEntryPagedCollection.PageCollection.ProjectedAsCollection<WireTransferBatchEntryDTO>();

                        var itemsCount = wireTransferBatchEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<WireTransferBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<WireTransferBatchEntryDTO> FindQueableWireTransferBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WireTransferBatchEntrySpecifications.QueableWireTransferBatchEntries();

                ISpecification<WireTransferBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var wireTransferBatchPagedCollection = _wireTransferBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (wireTransferBatchPagedCollection != null)
                {
                    var pageCollection = wireTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<WireTransferBatchEntryDTO>();

                    var itemsCount = wireTransferBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<WireTransferBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<BatchImportEntryWrapper> ParseWireTransferBatchImport(Guid wireTransferBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _wireTransferBatchRepository.Get(wireTransferBatchId, serviceHeader);

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

                                if (dataRecord.Count == 6)
                                {
                                    var payoutEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //PayrollNumber
                                        Column2 = dataRecord[1], //Payee
                                        Column3 = dataRecord[2], //Amount
                                        Column4 = dataRecord[3], //AccountNumber
                                        Column5 = dataRecord[4], //Reference
                                        Column6 = dataRecord[5], //Savings Product Code
                                    };

                                    importEntries.Add(payoutEntry);
                                }
                            }

                            if (importEntries.Any())
                            {
                                BatchImportParseInfo parseInfo = ParseWireTransfer(importEntries, serviceHeader);

                                if (parseInfo != null)
                                {
                                    UpdateWireTransferBatchEntries(wireTransferBatchId, parseInfo.MatchedCollection7, serviceHeader);

                                    return parseInfo.MismatchedCollection;
                                }
                                else return null;
                            }
                            else return null;
                        }
                    }
                    else return null;
                }
                else return null;
            }
        }

        private BatchImportParseInfo ParseWireTransfer(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection7 = new List<WireTransferBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var savingsProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader);

            var count = 0;

            importEntries.ForEach(item =>
            {
                var amount = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    var productCode = default(int);

                    if (int.TryParse(item.Column6, out productCode))
                    {
                        var matchedSavingsProducts = savingsProducts.Where(x => x.Code == productCode);

                        if (matchedSavingsProducts != null && matchedSavingsProducts.Any() && matchedSavingsProducts.Count() == 1)
                        {
                            var targetSavingsProduct = matchedSavingsProducts.First();

                            var customerSavingsAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(targetSavingsProduct.Id, item.Column1, serviceHeader);

                            if (customerSavingsAccounts.Any())
                            {
                                if (customerSavingsAccounts.Count == 1)
                                {
                                    var targetCustomerAccount = customerSavingsAccounts[0];

                                    WireTransferBatchEntryDTO wireTransferBatchEntry = new WireTransferBatchEntryDTO();

                                    wireTransferBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                    wireTransferBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                    wireTransferBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                    wireTransferBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                    wireTransferBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                    wireTransferBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                    wireTransferBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                    wireTransferBatchEntry.ProductDescription = targetSavingsProduct.Description;
                                    wireTransferBatchEntry.Amount = amount;
                                    wireTransferBatchEntry.Payee = item.Column2;
                                    wireTransferBatchEntry.AccountNumber = item.Column4;
                                    wireTransferBatchEntry.Reference = item.Column5;

                                    result.MatchedCollection7.Add(wireTransferBatchEntry);
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by payroll number {2}", count, customerSavingsAccounts.Count(), item.Column1);

                                    result.MismatchedCollection.Add(item);
                                }
                            }
                            else
                            {
                                item.Remarks = string.Format("Record #{0} ~ no match for savings product customer account by payroll number {1}", count, item.Column1);

                                result.MismatchedCollection.Add(item);
                            }
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ no match for savings product code {1}", count, item.Column6);

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to parse product code {1}", count, item.Column6);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount {1}", count, item.Column3);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private bool UpdateWireTransferBatchEntries(Guid wireTransferBatchId, List<WireTransferBatchEntryDTO> wireTransferBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (wireTransferBatchId != null && wireTransferBatchEntries != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _wireTransferBatchRepository.Get(wireTransferBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteWireTransferBatchEntries(persisted.Id, serviceHeader);

                        if (wireTransferBatchEntries.Any())
                        {
                            List<WireTransferBatchEntry> batchEntries = new List<WireTransferBatchEntry>();

                            foreach (var item in wireTransferBatchEntries)
                            {
                                var wireTransferBatchEntry = WireTransferBatchEntryFactory.CreateWireTransferBatchEntry(persisted.Id, item.CustomerAccountId, item.Amount, item.Payee, item.AccountNumber, item.Reference);

                                wireTransferBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                wireTransferBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(wireTransferBatchEntry);
                            }

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<WireTransferBatchEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    WireTransferBatchEntryBulkCopyDTO bcpc =
                                        new WireTransferBatchEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            WireTransferBatchId = c.WireTransferBatchId,
                                            CustomerAccountId = c.CustomerAccountId,
                                            Amount = c.Amount,
                                            Payee = c.Payee,
                                            AccountNumber = c.AccountNumber,
                                            Reference = c.Reference,
                                            Status = c.Status,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _wireTransferBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool MarkWireTransferBatchEntryPosted(Guid wireTransferBatchEntryId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (wireTransferBatchEntryId == null || wireTransferBatchEntryId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchEntryRepository.Get(wireTransferBatchEntryId, serviceHeader);

                if (persisted != null)
                {
                    switch ((BatchEntryStatus)persisted.Status)
                    {
                        case BatchEntryStatus.Pending:
                            persisted.Status = (int)BatchEntryStatus.Posted;
                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private bool MarkWireTransferBatchEntryRejected(Guid wireTransferBatchEntryId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (wireTransferBatchEntryId == null || wireTransferBatchEntryId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _wireTransferBatchEntryRepository.Get(wireTransferBatchEntryId, serviceHeader);

                if (persisted != null && persisted.Status == (int)BatchEntryStatus.Posted)
                {
                    persisted.Status = (int)BatchEntryStatus.Rejected;

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }
    }
}
