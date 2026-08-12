using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchDynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class InterAccountTransferBatchAppService : IInterAccountTransferBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<InterAccountTransferBatch> _interAccountTransferBatchRepository;
        private readonly IRepository<InterAccountTransferBatchEntry> _interAccountTransferBatchEntryRepository;
        private readonly IRepository<InterAccountTransferBatchDynamicCharge> _interAccountTransferBatchDynamicChargeRepository;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly IAppCache _appCache;

        public InterAccountTransferBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<InterAccountTransferBatch> interAccountTransferBatchRepository,
           IRepository<InterAccountTransferBatchEntry> interAccountTransferBatchEntryRepository,
           IRepository<InterAccountTransferBatchDynamicCharge> interAccountTransferBatchDynamicChargeRepository,
           ICustomerAccountAppService customerAccountAppService,
           IJournalAppService journalAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (interAccountTransferBatchRepository == null)
                throw new ArgumentNullException(nameof(interAccountTransferBatchRepository));

            if (interAccountTransferBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(interAccountTransferBatchEntryRepository));

            if (interAccountTransferBatchDynamicChargeRepository == null)
                throw new ArgumentNullException(nameof(interAccountTransferBatchDynamicChargeRepository));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _interAccountTransferBatchRepository = interAccountTransferBatchRepository;
            _interAccountTransferBatchEntryRepository = interAccountTransferBatchEntryRepository;
            _interAccountTransferBatchDynamicChargeRepository = interAccountTransferBatchDynamicChargeRepository;
            _customerAccountAppService = customerAccountAppService;
            _journalAppService = journalAppService;
            _appCache = appCache;
        }

        public InterAccountTransferBatchDTO AddNewInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var interAccountTransferBatch = InterAccountTransferBatchFactory.CreateInterAccountTransferBatch(interAccountTransferBatchDTO.BranchId, interAccountTransferBatchDTO.CustomerAccountId, interAccountTransferBatchDTO.Reference);

                    interAccountTransferBatch.BatchNumber = _interAccountTransferBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}InterAccountTransferBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    interAccountTransferBatch.Status = (int)BatchStatus.Pending;
                    interAccountTransferBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _interAccountTransferBatchRepository.Add(interAccountTransferBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return interAccountTransferBatch.ProjectedAs<InterAccountTransferBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchDTO == null || interAccountTransferBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Reference = interAccountTransferBatchDTO.Reference;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool UpdateDynamicCharges(Guid interAccountTransferBatchId, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != null && dynamicCharges != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = InterAccountTransferBatchDynamicChargeSpecifications.InterAccountTransferBatchDynamicChargeWithInterAccountTransferBatchId(interAccountTransferBatchId);

                        ISpecification<InterAccountTransferBatchDynamicCharge> spec = filter;

                        var interAccountTransferBatchDynamicCharges = _interAccountTransferBatchDynamicChargeRepository.AllMatching(spec, serviceHeader);

                        if (interAccountTransferBatchDynamicCharges != null)
                        {
                            interAccountTransferBatchDynamicCharges.ToList().ForEach(x => _interAccountTransferBatchDynamicChargeRepository.Remove(x, serviceHeader));
                        }

                        if (dynamicCharges.Any())
                        {
                            foreach (var item in dynamicCharges)
                            {
                                var interAccountTransferBatchDynamicCharge = InterAccountTransferBatchDynamicChargeFactory.CreateInterAccountTransferBatchDynamicCharge(persisted.Id, item.Id);

                                _interAccountTransferBatchDynamicChargeRepository.Add(interAccountTransferBatchDynamicCharge, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public InterAccountTransferBatchEntryDTO AddNewInterAccountTransferBatchEntry(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var interAccountTransferBatchEntry = InterAccountTransferBatchEntryFactory.CreateInterAccountTransferBatchEntry(interAccountTransferBatchEntryDTO.InterAccountTransferBatchId, interAccountTransferBatchEntryDTO.ApportionTo, interAccountTransferBatchEntryDTO.CustomerAccountId, interAccountTransferBatchEntryDTO.ChartOfAccountId, interAccountTransferBatchEntryDTO.Principal, interAccountTransferBatchEntryDTO.Interest, interAccountTransferBatchEntryDTO.PrimaryDescription, interAccountTransferBatchEntryDTO.SecondaryDescription, interAccountTransferBatchEntryDTO.Reference);

                    interAccountTransferBatchEntry.Status = (int)BatchEntryStatus.Pending;

                    switch ((ApportionTo)interAccountTransferBatchEntry.ApportionTo)
                    {
                        case ApportionTo.CustomerAccount:
                            interAccountTransferBatchEntry.ChartOfAccountId = null;
                            break;
                        case ApportionTo.GeneralLedgerAccount:
                            interAccountTransferBatchEntry.CustomerAccountId = null;
                            break;
                        default:
                            break;
                    }

                    interAccountTransferBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _interAccountTransferBatchEntryRepository.Add(interAccountTransferBatchEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return interAccountTransferBatchEntry.ProjectedAs<InterAccountTransferBatchEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveInterAccountTransferBatchEntries(List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in interAccountTransferBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _interAccountTransferBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _interAccountTransferBatchEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Audited;
                        persisted.AuditRemarks = interAccountTransferBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = interAccountTransferBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var interAccountTransferBatchEntries = FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(persisted.Id, serviceHeader);

                        if (interAccountTransferBatchEntries != null && interAccountTransferBatchEntries.Any())
                        {
                            var sourceCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(persisted.CustomerAccountId, serviceHeader);

                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { sourceCustomerAccount }, serviceHeader);

                            var dynamicCharges = FindDynamicCharges(persisted.Id, serviceHeader);

                            foreach (var item in interAccountTransferBatchEntries)
                            {
                                CustomerAccountDTO destinationCustomerAccount = null;

                                if (item.CustomerAccountId.HasValue)
                                    destinationCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(item.CustomerAccountId.Value, serviceHeader);

                                if (item.Status == (int)BatchEntryStatus.Pending)
                                {
                                    var primaryDescription = item.PrimaryDescription;
                                    var secondaryDescription = item.SecondaryDescription;
                                    var reference = string.Format("{0}->{1}", item.Reference, interAccountTransferBatchDTO.PaddedBatchNumber);

                                    var apportionments =
                                        new List<ApportionmentWrapper>
                                        {
                                            new ApportionmentWrapper
                                            {
                                                ApportionTo = item.ApportionTo,
                                                Principal = item.Principal,
                                                Interest = item.Interest,
                                                PrimaryDescription = primaryDescription,
                                                SecondaryDescription = secondaryDescription,
                                                Reference = reference,
                                                DebitCustomerAccount = sourceCustomerAccount,
                                                CreditCustomerAccount = destinationCustomerAccount,
                                                CreditChartOfAccountId = item.ChartOfAccountId ?? Guid.Empty,
                                            }
                                        };

                                    var journalDTO = _journalAppService.AddNewJournal(persisted.BranchId, null, (item.Principal + item.Interest), primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterAcccountTransfer, null, sourceCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, sourceCustomerAccount, sourceCustomerAccount, apportionments, null, dynamicCharges, serviceHeader);

                                    if (journalDTO != null)
                                    {
                                        var batchEntry = _interAccountTransferBatchEntryRepository.Get(item.Id, serviceHeader);

                                        if (batchEntry != null)
                                        {
                                            batchEntry.Status = (int)BatchEntryStatus.Posted;
                                        }
                                    }
                                }
                            }

                            persisted.Status = (int)BatchStatus.Posted;

                            persisted.AuthorizationRemarks = interAccountTransferBatchDTO.AuthorizationRemarks;

                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }

                        break;
                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;

                        persisted.AuthorizationRemarks = interAccountTransferBatchDTO.AuthorizationRemarks;

                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        var rejectedInterAccountTransferBatchEntries = FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(persisted.Id, serviceHeader);

                        if (rejectedInterAccountTransferBatchEntries != null && rejectedInterAccountTransferBatchEntries.Any())
                        {
                            foreach (var item in rejectedInterAccountTransferBatchEntries)
                            {
                                var batchEntry = _interAccountTransferBatchEntryRepository.Get(item.Id, serviceHeader);

                                if (batchEntry != null)
                                {
                                    batchEntry.Status = (int)BatchEntryStatus.Rejected;
                                }
                            }
                        }

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateInterAccountTransferBatchEntryCollection(Guid interAccountTransferBatchId, List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != null && interAccountTransferBatchEntryCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var interAccountTransferBatchEntry = _interAccountTransferBatchEntryRepository.Get(item.Id, serviceHeader);

                                if (interAccountTransferBatchEntry != null)
                                {
                                    _interAccountTransferBatchEntryRepository.Remove(interAccountTransferBatchEntry, serviceHeader);
                                }
                            }
                        }

                        if (interAccountTransferBatchEntryCollection.Any())
                        {
                            foreach (var item in interAccountTransferBatchEntryCollection)
                            {
                                var interAccountTransferBatchEntry = InterAccountTransferBatchEntryFactory.CreateInterAccountTransferBatchEntry(persisted.Id, item.ApportionTo, item.CustomerAccountId, item.ChartOfAccountId, item.Principal, item.Interest, item.PrimaryDescription, item.SecondaryDescription, item.Reference);

                                interAccountTransferBatchEntry.Status = (int)BatchEntryStatus.Pending;

                                switch ((ApportionTo)interAccountTransferBatchEntry.ApportionTo)
                                {
                                    case ApportionTo.CustomerAccount:
                                        interAccountTransferBatchEntry.ChartOfAccountId = null;
                                        break;
                                    case ApportionTo.GeneralLedgerAccount:
                                        interAccountTransferBatchEntry.CustomerAccountId = null;
                                        break;
                                    default:
                                        break;
                                }

                                interAccountTransferBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                _interAccountTransferBatchEntryRepository.Add(interAccountTransferBatchEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var interAccountTransferBatches = _interAccountTransferBatchRepository.GetAll(serviceHeader);

                if (interAccountTransferBatches != null && interAccountTransferBatches.Any())
                {
                    return interAccountTransferBatches.ProjectedAsCollection<InterAccountTransferBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InterAccountTransferBatchSpecifications.DefaultSpec();

                ISpecification<InterAccountTransferBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var interAccountTransferBatchPagedCollection = _interAccountTransferBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (interAccountTransferBatchPagedCollection != null)
                {
                    var pageCollection = interAccountTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<InterAccountTransferBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.InterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id, null), serviceHeader);

                            var postedItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.PostedInterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = interAccountTransferBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<InterAccountTransferBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InterAccountTransferBatchSpecifications.InterAccountTransferBatchWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<InterAccountTransferBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var interAccountTransferBatchPagedCollection = _interAccountTransferBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (interAccountTransferBatchPagedCollection != null)
                {
                    var pageCollection = interAccountTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<InterAccountTransferBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.InterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id, null), serviceHeader);

                            var postedItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.PostedInterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = interAccountTransferBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<InterAccountTransferBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InterAccountTransferBatchDTO> FindInterAccountTransferBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InterAccountTransferBatchSpecifications.InterAccountTransferBatchesWithStatus(status, startDate, endDate, text);

                ISpecification<InterAccountTransferBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var interAccountTransferBatchPagedCollection = _interAccountTransferBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (interAccountTransferBatchPagedCollection != null)
                {
                    var pageCollection = interAccountTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<InterAccountTransferBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.InterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id, null), serviceHeader);

                            var postedItems = _interAccountTransferBatchEntryRepository.AllMatchingCount(InterAccountTransferBatchEntrySpecifications.PostedInterAccountTransferBatchEntryWithInterAccountTransferBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = interAccountTransferBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<InterAccountTransferBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public InterAccountTransferBatchDTO FindInterAccountTransferBatch(Guid interAccountTransferBatchId, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var interAccountTransferBatch = _interAccountTransferBatchRepository.Get(interAccountTransferBatchId, serviceHeader);

                    if (interAccountTransferBatch != null)
                    {
                        return interAccountTransferBatch.ProjectedAs<InterAccountTransferBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != null && interAccountTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InterAccountTransferBatchEntrySpecifications.InterAccountTransferBatchEntryWithInterAccountTransferBatchId(interAccountTransferBatchId, null);

                    ISpecification<InterAccountTransferBatchEntry> spec = filter;

                    var interAccountTransferBatchEntries = _interAccountTransferBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (interAccountTransferBatchEntries != null && interAccountTransferBatchEntries.Any())
                    {
                        return interAccountTransferBatchEntries.ProjectedAsCollection<InterAccountTransferBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<InterAccountTransferBatchEntryDTO> FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != null && interAccountTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InterAccountTransferBatchEntrySpecifications.InterAccountTransferBatchEntryWithInterAccountTransferBatchId(interAccountTransferBatchId, text);

                    ISpecification<InterAccountTransferBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var interAccountTransferBatchPagedCollection = _interAccountTransferBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (interAccountTransferBatchPagedCollection != null)
                    {
                        var pageCollection = interAccountTransferBatchPagedCollection.PageCollection.ProjectedAsCollection<InterAccountTransferBatchEntryDTO>();

                        var itemsCount = interAccountTransferBatchPagedCollection.ItemsCount;

                        return new PageCollectionInfo<InterAccountTransferBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DynamicChargeDTO> FindDynamicCharges(Guid interAccountTransferBatchId, ServiceHeader serviceHeader)
        {
            if (interAccountTransferBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InterAccountTransferBatchDynamicChargeSpecifications.InterAccountTransferBatchDynamicChargeWithInterAccountTransferBatchId(interAccountTransferBatchId);

                    ISpecification<InterAccountTransferBatchDynamicCharge> spec = filter;

                    var interAccountTransferBatchDynamicCharges = _interAccountTransferBatchDynamicChargeRepository.AllMatching(spec, serviceHeader);

                    if (interAccountTransferBatchDynamicCharges != null)
                    {
                        var projection = interAccountTransferBatchDynamicCharges.ProjectedAsCollection<InterAccountTransferBatchDynamicChargeDTO>();

                        return (from p in projection select p.DynamicCharge).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DynamicChargeDTO> FindCachedDynamicCharges(Guid interAccountTransferBatchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<DynamicChargeDTO>>(string.Format("DynamicChargesByInterAccountTransferBatchId_{0}_{1}", serviceHeader.ApplicationDomainName, interAccountTransferBatchId.ToString("D")), () =>
            {
                return FindDynamicCharges(interAccountTransferBatchId, serviceHeader);
            });
        }
    }
}
