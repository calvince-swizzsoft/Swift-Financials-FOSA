using System.ComponentModel.DataAnnotations;
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

        public CustomerAccountDTO FindTransferAccount(Guid accountId, ServiceHeader serviceHeader)
        {
            var account = _customerAccountAppService.FindCustomerAccounts(accountId, serviceHeader);
            if (account == null) return null;
            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { account }, serviceHeader);
            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { account }, serviceHeader, true, false);
            return account;
        }

        // Validate the entire allocation, including repeated rows for the same loan.
        private bool ValidTransferEntries(Guid sourceId, List<InterAccountTransferBatchEntryDTO> entries, ServiceHeader header)
        {
            var source = FindTransferAccount(sourceId, header);
            if (source == null) throw new ValidationException("The source account could not be found. Reopen the batch and select a valid source account.");
            if (entries == null) throw new ValidationException("No transfer entries were supplied.");
            foreach (var entry in entries)
            {
                if (entry == null || entry.Principal < 0m || entry.Interest < 0m || entry.Principal + entry.Interest <= 0m)
                    throw new ValidationException("Each transfer entry must have non-negative principal and interest, with a total greater than zero.");
                if (entry.ApportionTo == (int)ApportionTo.GeneralLedgerAccount)
                {
                    if (!entry.ChartOfAccountId.HasValue || entry.ChartOfAccountId.Value == Guid.Empty) throw new ValidationException("Select a destination G/L account.");
                }
                else if (entry.ApportionTo != (int)ApportionTo.CustomerAccount || !entry.CustomerAccountId.HasValue || entry.CustomerAccountId.Value == Guid.Empty)
                    throw new ValidationException("Select a valid apportionment type and destination customer account.");
            }
            foreach (var group in entries.Where(e => e.ApportionTo == (int)ApportionTo.CustomerAccount).GroupBy(e => e.CustomerAccountId.Value))
            {
                var target = FindTransferAccount(group.Key, header);
                if (target == null) throw new ValidationException("The destination customer account could not be found. Select it again.");
                if (target.Id == source.Id) throw new ValidationException("The source and destination accounts must be different.");
                if (target.CustomerId != source.CustomerId) throw new ValidationException("The destination account must belong to the same customer as the source account.");
                if (target.CustomerAccountTypeProductCode == (int)ProductCode.Loan)
                {
                    var principal = group.Sum(e => e.Principal);
                    var interest = group.Sum(e => e.Interest);
                    if (principal > Math.Abs(target.PrincipalBalance))
                        throw new ValidationException(string.Format("Loan {0}: total principal allocated in this batch is {1:N2}, but outstanding principal is {2:N2}. Reduce the principal amount or remove an existing allocation to this loan.", target.FullAccountNumber, principal, Math.Abs(target.PrincipalBalance)));
                    if (interest > Math.Abs(target.InterestBalance))
                        throw new ValidationException(string.Format("Loan {0}: total interest allocated in this batch is {1:N2}, but outstanding interest is {2:N2}. Reduce the interest amount or remove an existing allocation to this loan.", target.FullAccountNumber, interest, Math.Abs(target.InterestBalance)));
                }
            }
            return true;
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
                    var batch = _interAccountTransferBatchRepository.Get(interAccountTransferBatchEntryDTO.InterAccountTransferBatchId, serviceHeader);
                    if (batch == null) throw new ValidationException("This transfer batch no longer exists. Refresh the batch list.");
                    if (batch.Status != (int)BatchStatus.Pending) throw new ValidationException("Entries can only be added to a pending transfer batch. This batch has already moved to another stage; refresh the batch list.");
                    var allocation = FindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(batch.Id, serviceHeader) ?? new List<InterAccountTransferBatchEntryDTO>();
                    allocation.Add(interAccountTransferBatchEntryDTO);
                    if (!ValidTransferEntries(batch.CustomerAccountId, allocation, serviceHeader)) return null;

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
                            if (!ValidTransferEntries(persisted.CustomerAccountId, interAccountTransferBatchEntries.Where(e => e.Status == (int)BatchEntryStatus.Pending).ToList(), serviceHeader)) return false;

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
                        if (persisted.Status != (int)BatchStatus.Pending || !ValidTransferEntries(persisted.CustomerAccountId, interAccountTransferBatchEntryCollection, serviceHeader)) return false;

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
