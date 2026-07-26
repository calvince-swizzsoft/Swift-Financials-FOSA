using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class ExpensePayableAppService : IExpensePayableAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ExpensePayable> _expensePayableRepository;
        private readonly IRepository<ExpensePayableEntry> _expensePayableEntryRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public ExpensePayableAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ExpensePayable> expensePayableRepository,
           IRepository<ExpensePayableEntry> expensePayableEntryRepository,
           IPostingPeriodAppService postingPeriodAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (expensePayableRepository == null)
                throw new ArgumentNullException(nameof(expensePayableRepository));

            if (expensePayableEntryRepository == null)
                throw new ArgumentNullException(nameof(expensePayableEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _expensePayableRepository = expensePayableRepository;
            _expensePayableEntryRepository = expensePayableEntryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public ExpensePayableDTO AddNewExpensePayable(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader)
        {
            if (expensePayableDTO != null)
            {
                var postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(expensePayableDTO.PostingPeriodId, serviceHeader);

                if (postingPeriodDTO != null)
                {
                    if (expensePayableDTO.ValueDate != null && (expensePayableDTO.ValueDate < postingPeriodDTO.DurationStartDate || expensePayableDTO.ValueDate > postingPeriodDTO.DurationEndDate || expensePayableDTO.ValueDate > DateTime.Today))

                    {
                        expensePayableDTO.errormassage=string.Format("ValueDate", "Sorry, but value date is out of range!");
                        return expensePayableDTO;
                    }
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        var expensePayable = ExpensePayableFactory.CreateExpensePayable(expensePayableDTO.BranchId, expensePayableDTO.ChartOfAccountId, expensePayableDTO.Type, expensePayableDTO.TotalValue, expensePayableDTO.ValueDate, expensePayableDTO.Remarks);

                        expensePayable.VoucherNumber = _expensePayableRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(VoucherNumber),0) + 1 AS Expr1 FROM {0}ExpensePayables", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                        expensePayable.Status = (int)ExpensePayableStatus.Pending;
                        expensePayable.CreatedBy = serviceHeader.ApplicationUserName;

                        _expensePayableRepository.Add(expensePayable, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return expensePayable.ProjectedAs<ExpensePayableDTO>();
                    }
                }
                else return null;
            }
            else return null;
        }

        public bool UpdateExpensePayable(ExpensePayableDTO expensePayableDTO, ServiceHeader serviceHeader)
        {
            if (expensePayableDTO == null || expensePayableDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _expensePayableRepository.Get(expensePayableDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.TotalValue = expensePayableDTO.TotalValue;
                    persisted.Remarks = expensePayableDTO.Remarks;
                    persisted.Status = expensePayableDTO.Status;

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return persisted.TotalValue == persisted.ExpensePayableEntries.Sum(x => x.Value);
                    }
                    else return false;
                }
                else
                { 
                    expensePayableDTO.errormassage = string.Format("Sorry, but the persisted entity could not be identified!");

                    return true;
                }
            }
        }

        public ExpensePayableEntryDTO AddNewExpensePayableEntry(ExpensePayableEntryDTO expensePayableEntryDTO, ServiceHeader serviceHeader)
        {
            if (expensePayableEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var expensePayableEntry = ExpensePayableEntryFactory.CreateExpensePayableEntry(expensePayableEntryDTO.ExpensePayableId, expensePayableEntryDTO.BranchId, expensePayableEntryDTO.ChartOfAccountId, expensePayableEntryDTO.Value, expensePayableEntryDTO.PrimaryDescription, expensePayableEntryDTO.SecondaryDescription, expensePayableEntryDTO.Reference);

                    expensePayableEntry.Status = (int)ExpensePayableEntryStatus.Pending;

                    expensePayableEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _expensePayableEntryRepository.Add(expensePayableEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return expensePayableEntry.ProjectedAs<ExpensePayableEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveExpensePayableEntries(List<ExpensePayableEntryDTO> expensePayableEntryDTOs, ServiceHeader serviceHeader)
        {
            if (expensePayableEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in expensePayableEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _expensePayableEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _expensePayableEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, ServiceHeader serviceHeader)
        {
            if (expensePayableDTO == null || !Enum.IsDefined(typeof(ExpensePayableAuthOption), expensePayableAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _expensePayableRepository.Get(expensePayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)ExpensePayableStatus.Pending)
                    return false;

                switch ((ExpensePayableAuthOption)expensePayableAuthOption)
                {
                    case ExpensePayableAuthOption.Post:

                        persisted.Status = (int)ExpensePayableStatus.Audited;
                        persisted.AuditRemarks = expensePayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case ExpensePayableAuthOption.Reject:

                        persisted.Status = (int)ExpensePayableStatus.Rejected;
                        persisted.AuditRemarks = expensePayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case ExpensePayableAuthOption.Defer:
                        persisted.Status = (int)ExpensePayableStatus.Deferred;
                        persisted.AuditRemarks = expensePayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;
                        break;

                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeExpensePayable(ExpensePayableDTO expensePayableDTO, int expensePayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (expensePayableDTO == null || !Enum.IsDefined(typeof(ExpensePayableAuthOption), expensePayableAuthOption))
                return result;

            var journals = new List<Journal>();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _expensePayableRepository.Get(expensePayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)ExpensePayableStatus.Audited)
                    return result;

                switch ((ExpensePayableAuthOption)expensePayableAuthOption)
                {
                    case ExpensePayableAuthOption.Post:

                        var expensePayableEntries = FindExpensePayableEntriesByExpensePayableId(persisted.Id, serviceHeader);

                        var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                        if (postingPeriod != null && expensePayableEntries != null && persisted.TotalValue == expensePayableEntries.Sum(x => x.Value))
                        {
                            int counter = 0;

                            foreach (var item in expensePayableEntries)
                            {
                                counter += 1;

                                var reference = string.Format("{0}:{1}->{2} ({3}/{4})", expensePayableDTO.PaddedVoucherNumber, persisted.Remarks, item.Reference, counter, expensePayableEntries.Count);

                                switch ((ExpensePayableType)persisted.Type)
                                {
                                    case ExpensePayableType.DebitGLAccount:

                                        var creditGLAccountJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, item.BranchId, null, item.Value, item.PrimaryDescription, item.SecondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExpensePayables, expensePayableDTO.ValueDate, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(creditGLAccountJournal, item.ChartOfAccountId, persisted.ChartOfAccountId, serviceHeader);
                                        journals.Add(creditGLAccountJournal);

                                        break;
                                    case ExpensePayableType.CreditGLAccount:

                                        var debitGLAccountJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, item.BranchId, null, item.Value, item.PrimaryDescription, item.SecondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExpensePayables, expensePayableDTO.ValueDate, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(debitGLAccountJournal, persisted.ChartOfAccountId, item.ChartOfAccountId, serviceHeader);
                                        journals.Add(debitGLAccountJournal);

                                        break;
                                    default:
                                        break;
                                }

                                var batchEntry = _expensePayableEntryRepository.Get(item.Id, serviceHeader);

                                if (batchEntry != null)
                                {
                                    batchEntry.Status = (int)ExpensePayableEntryStatus.Posted;
                                }
                            }

                            persisted.Status = (int)ExpensePayableStatus.Posted;

                            persisted.AuthorizationRemarks = expensePayableDTO.AuthorizationRemarks;

                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }
                        else
                            expensePayableDTO.errormassage = string.Format(("Sorry, but requisite minimum requirements have not been satisfied viz. (batch total/posting period/journal voucher control account)"));

                        break;
                    case ExpensePayableAuthOption.Reject:

                        persisted.Status = (int)ExpensePayableStatus.Rejected;
                        persisted.AuthorizationRemarks = expensePayableDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        var rejectedExpensePayableEntries = FindExpensePayableEntriesByExpensePayableId(persisted.Id, serviceHeader);

                        if (rejectedExpensePayableEntries != null && rejectedExpensePayableEntries.Any())
                        {
                            foreach (var item in rejectedExpensePayableEntries)
                            {
                                var batchEntry = _expensePayableEntryRepository.Get(item.Id, serviceHeader);

                                if (batchEntry != null)
                                {
                                    batchEntry.Status = (int)ExpensePayableEntryStatus.Rejected;
                                }
                            }
                        }

                        break;

                    case ExpensePayableAuthOption.Defer:
                        persisted.Status = (int)ExpensePayableStatus.Deferred;
                        persisted.AuthorizationRemarks = expensePayableDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;
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

        public bool UpdateExpensePayableEntryCollection(Guid expensePayableId, List<ExpensePayableEntryDTO> expensePayableEntryCollection, ServiceHeader serviceHeader)
        {
            if (expensePayableId != null && expensePayableEntryCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _expensePayableRepository.Get(expensePayableId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindExpensePayableEntriesByExpensePayableId(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var expensePayableEntry = _expensePayableEntryRepository.Get(item.Id, serviceHeader);

                                if (expensePayableEntry != null)
                                {
                                    _expensePayableEntryRepository.Remove(expensePayableEntry, serviceHeader);
                                }
                            }
                        }

                        if (expensePayableEntryCollection.Any())
                        {
                            foreach (var item in expensePayableEntryCollection)
                            {
                                var expensePayableEntry = ExpensePayableEntryFactory.CreateExpensePayableEntry(persisted.Id, item.BranchId, item.ChartOfAccountId, item.Value, item.PrimaryDescription, item.SecondaryDescription, item.Reference);

                                expensePayableEntry.Status = (int)ExpensePayableEntryStatus.Pending;

                                expensePayableEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                _expensePayableEntryRepository.Add(expensePayableEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<ExpensePayableDTO> FindExpensePayables(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var expensePayables = _expensePayableRepository.GetAll(serviceHeader);

                if (expensePayables != null && expensePayables.Any())
                {
                    return expensePayables.ProjectedAsCollection<ExpensePayableDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExpensePayableSpecifications.DefaultSpec();

                ISpecification<ExpensePayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var expensePayablePagedCollection = _expensePayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (expensePayablePagedCollection != null)
                {
                    var pageCollection = expensePayablePagedCollection.PageCollection.ProjectedAsCollection<ExpensePayableDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            var postedItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.PostedExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = expensePayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExpensePayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExpensePayableSpecifications.ExpensePayableWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<ExpensePayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var expensePayablePagedCollection = _expensePayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (expensePayablePagedCollection != null)
                {
                    var pageCollection = expensePayablePagedCollection.PageCollection.ProjectedAsCollection<ExpensePayableDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            var postedItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.PostedExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = expensePayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExpensePayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayables(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExpensePayableSpecifications.ExpensePayablesWithStatus(status, startDate, endDate, text);

                ISpecification<ExpensePayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var expensePayablePagedCollection = _expensePayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (expensePayablePagedCollection != null)
                {
                    var pageCollection = expensePayablePagedCollection.PageCollection.ProjectedAsCollection<ExpensePayableDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            var postedItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.PostedExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = expensePayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExpensePayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExpensePayableDTO> FindExpensePayables( string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExpensePayableSpecifications.ExpensePayablesWithText(text);

                ISpecification<ExpensePayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var expensePayablePagedCollection = _expensePayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (expensePayablePagedCollection != null)
                {
                    var pageCollection = expensePayablePagedCollection.PageCollection.ProjectedAsCollection<ExpensePayableDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            var postedItems = _expensePayableEntryRepository.AllMatchingCount(ExpensePayableEntrySpecifications.PostedExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = expensePayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExpensePayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ExpensePayableDTO FindExpensePayable(Guid expensePayableId, ServiceHeader serviceHeader)
        {
            if (expensePayableId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var expensePayable = _expensePayableRepository.Get(expensePayableId, serviceHeader);

                    if (expensePayable != null)
                    {
                        return expensePayable.ProjectedAs<ExpensePayableDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId, ServiceHeader serviceHeader)
        {
            if (expensePayableId != null && expensePayableId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(expensePayableId);

                    ISpecification<ExpensePayableEntry> spec = filter;

                    var expensePayableEntries = _expensePayableEntryRepository.AllMatching(spec, serviceHeader);

                    if (expensePayableEntries != null && expensePayableEntries.Any())
                    {
                        return expensePayableEntries.ProjectedAsCollection<ExpensePayableEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<ExpensePayableEntryDTO> FindExpensePayableEntriesByExpensePayableId(Guid expensePayableId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (expensePayableId != null && expensePayableId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(expensePayableId);

                    ISpecification<ExpensePayableEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var expensePayablePagedCollection = _expensePayableEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (expensePayablePagedCollection != null)
                    {
                        var persisted = _expensePayableRepository.Get(expensePayableId, serviceHeader);

                        var persistedEntriesTotal = 0m;

                        var expensePayableEntries = _expensePayableEntryRepository.AllMatching(ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(persisted.Id), serviceHeader);

                        if (expensePayableEntries != null && expensePayableEntries.Any())
                            persistedEntriesTotal = expensePayableEntries.Sum(x => x.Value);

                        var pageCollection = expensePayablePagedCollection.PageCollection.ProjectedAsCollection<ExpensePayableEntryDTO>();

                        var itemsCount = expensePayablePagedCollection.ItemsCount;

                        return new PageCollectionInfo<ExpensePayableEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }



        public bool UpdateExpensePayableEntries(Guid expensePayableId, List<ExpensePayableEntryDTO> ExpensePayableEntries, ServiceHeader serviceHeader)
        {
            if (expensePayableId != null && ExpensePayableEntries != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _expensePayableRepository.Get(expensePayableId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindExpensePayableEntries(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var ExpensePayableEntry = _expensePayableEntryRepository.Get(item.Id, serviceHeader);

                                if (ExpensePayableEntry != null)
                                {
                                    _expensePayableEntryRepository.Remove(ExpensePayableEntry, serviceHeader);
                                }
                            }
                        }

                        if (ExpensePayableEntries.Any())
                        {
                            foreach (var item in ExpensePayableEntries)
                            {
                                var ExpensePayableEntry = ExpensePayableEntryFactory.CreateExpensePayableEntry(persisted.Id, item.BranchId , item.ChartOfAccountId, item.Value,item.PrimaryDescription,item.SecondaryDescription,item.Reference);
                                
                                _expensePayableEntryRepository.Add(ExpensePayableEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<ExpensePayableEntryDTO> FindExpensePayableEntries(Guid expensePayableId, ServiceHeader serviceHeader)
        {
            if (expensePayableId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExpensePayableEntrySpecifications.ExpensePayableEntryWithExpensePayableId(expensePayableId);

                    ISpecification<ExpensePayableEntry> spec = filter;

                    var expensePayableEntries = _expensePayableEntryRepository.AllMatching(spec, serviceHeader);

                    if (expensePayableEntries != null)
                    {
                        return expensePayableEntries.ProjectedAsCollection<ExpensePayableEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }




    }
}
