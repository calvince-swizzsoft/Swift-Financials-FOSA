using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class JournalVoucherAppService : IJournalVoucherAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<JournalVoucher> _journalVoucherRepository;
        private readonly IRepository<JournalVoucherEntry> _journalVoucherEntryRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public JournalVoucherAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<JournalVoucher> journalVoucherRepository,
           IRepository<JournalVoucherEntry> journalVoucherEntryRepository,
           IPostingPeriodAppService postingPeriodAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (journalVoucherRepository == null)
                throw new ArgumentNullException(nameof(journalVoucherRepository));

            if (journalVoucherEntryRepository == null)
                throw new ArgumentNullException(nameof(journalVoucherEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _journalVoucherRepository = journalVoucherRepository;
            _journalVoucherEntryRepository = journalVoucherEntryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public JournalVoucherDTO AddNewJournalVoucher(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader)
        {
            if (journalVoucherDTO != null)
            {
                var postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(journalVoucherDTO.PostingPeriodId, serviceHeader);

                if (postingPeriodDTO != null)
                {
                    if (journalVoucherDTO.ValueDate != null && (journalVoucherDTO.ValueDate < postingPeriodDTO.DurationStartDate || journalVoucherDTO.ValueDate > postingPeriodDTO.DurationEndDate || journalVoucherDTO.ValueDate > DateTime.Today))
                    {
                        journalVoucherDTO.ErrorMessageResult=string.Format("ValueDate", "Sorry, but value date is out of range!");

                        return journalVoucherDTO;
                    }

                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        var journalVoucher = JournalVoucherFactory.CreateJournalVoucher(journalVoucherDTO.BranchId, journalVoucherDTO.PostingPeriodId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.CustomerAccountId, journalVoucherDTO.Type, journalVoucherDTO.TotalValue, journalVoucherDTO.PrimaryDescription, journalVoucherDTO.SecondaryDescription, journalVoucherDTO.Reference, journalVoucherDTO.ValueDate);

                        journalVoucher.VoucherNumber = _journalVoucherRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(VoucherNumber),0) + 1 AS Expr1 FROM {0}JournalVouchers", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                        journalVoucher.Status = (int)JournalVoucherStatus.Pending;
                        journalVoucher.CreatedBy = serviceHeader.ApplicationUserName;

                        switch ((JournalVoucherType)journalVoucher.Type)
                        {
                            case JournalVoucherType.DebitGLAccount:
                            case JournalVoucherType.CreditGLAccount:
                                journalVoucher.CustomerAccountId = null;
                                break;
                            case JournalVoucherType.DebitCustomerAccount:
                                break;
                            case JournalVoucherType.CreditCustomerAccount:
                                break;
                            default:
                                break;
                        }

                        _journalVoucherRepository.Add(journalVoucher, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return journalVoucher.ProjectedAs<JournalVoucherDTO>();
                    }
                }
                else return null;
            }
            else return null;
        }

        public bool UpdateJournalVoucher(JournalVoucherDTO journalVoucherDTO, ServiceHeader serviceHeader)
        {
            if (journalVoucherDTO == null || journalVoucherDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _journalVoucherRepository.Get(journalVoucherDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(journalVoucherDTO.PostingPeriodId, serviceHeader);

                    if (journalVoucherDTO.ValueDate != null && (journalVoucherDTO.ValueDate < postingPeriodDTO.DurationStartDate || journalVoucherDTO.ValueDate > postingPeriodDTO.DurationEndDate || journalVoucherDTO.ValueDate > DateTime.Today))
                        throw new ArgumentOutOfRangeException("ValueDate", "Sorry, but value date is out of range!");

                    var current = JournalVoucherFactory.CreateJournalVoucher(journalVoucherDTO.BranchId, journalVoucherDTO.PostingPeriodId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.CustomerAccountId, journalVoucherDTO.Type, journalVoucherDTO.TotalValue, journalVoucherDTO.PrimaryDescription, journalVoucherDTO.SecondaryDescription, journalVoucherDTO.Reference, journalVoucherDTO.ValueDate);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.VoucherNumber = persisted.VoucherNumber;
                    current.CreatedBy = persisted.CreatedBy;
                    current.CreatedDate = persisted.CreatedDate;

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return persisted.TotalValue == persisted.JournalVoucherEntries.Sum(x => x.Amount);
                    }
                    else return false;
                }
                else return false;
            }
        }


        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalVoucherSpecifications.JournalVoucherFullText(text);

                ISpecification<JournalVoucher> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalVoucherPagedCollection = _journalVoucherRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalVoucherPagedCollection != null)
                {
                    var pageCollection = journalVoucherPagedCollection.PageCollection.ProjectedAsCollection<JournalVoucherDTO>();

                    //if (pageCollection != null && pageCollection.Any())
                    //{
                    //    foreach (var item in pageCollection)
                    //    {
                    //        var totalItems = _journalVoucherRepository.AllMatchingCount(JournalVoucherSpecifications.ExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                    //        var postedItems = _journalVoucherRepository.AllMatchingCount(JournalVoucherSpecifications.PostedExpensePayableEntryWithExpensePayableId(item.Id), serviceHeader);

                    //        item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                    //    }
                    //}

                    var itemsCount = journalVoucherPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalVoucherDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }


        public JournalVoucherEntryDTO AddNewJournalVoucherEntry(JournalVoucherEntryDTO journalVoucherEntryDTO, ServiceHeader serviceHeader)
        {
            if (journalVoucherEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var journalVoucherEntry = JournalVoucherEntryFactory.CreateJournalVoucherEntry(journalVoucherEntryDTO.JournalVoucherId, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherEntryDTO.CustomerAccountId, journalVoucherEntryDTO.Amount);

                    journalVoucherEntry.Status = (int)JournalVoucherEntryStatus.Pending;
                    journalVoucherEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _journalVoucherEntryRepository.Add(journalVoucherEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return journalVoucherEntry.ProjectedAs<JournalVoucherEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveJournalVoucherEntries(List<JournalVoucherEntryDTO> journalVoucherEntryDTOs, ServiceHeader serviceHeader)
        {
            if (journalVoucherEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in journalVoucherEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _journalVoucherEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _journalVoucherEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, ServiceHeader serviceHeader)
        {
            if (journalVoucherDTO == null || !Enum.IsDefined(typeof(JournalVoucherAuthOption), journalVoucherAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _journalVoucherRepository.Get(journalVoucherDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)JournalVoucherStatus.Pending)
                    return false;

                switch ((JournalVoucherAuthOption)journalVoucherAuthOption)
                {
                    case JournalVoucherAuthOption.Post:

                        persisted.Status = (int)JournalVoucherStatus.Audited;
                        persisted.AuditRemarks = journalVoucherDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case JournalVoucherAuthOption.Reject:

                        persisted.Status = (int)JournalVoucherStatus.Rejected;
                        persisted.AuditRemarks = journalVoucherDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeJournalVoucher(JournalVoucherDTO journalVoucherDTO, int journalVoucherAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (journalVoucherDTO == null || !Enum.IsDefined(typeof(JournalVoucherAuthOption), journalVoucherAuthOption))
                return result;

            var journals = new List<Journal>();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _journalVoucherRepository.Get(journalVoucherDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)JournalVoucherStatus.Audited)
                    return result;

                switch ((JournalVoucherAuthOption)journalVoucherAuthOption)
                {
                    case JournalVoucherAuthOption.Post:

                        var journalVoucherEntries = FindJournalVoucherEntriesByJournalVoucherId(persisted.Id, serviceHeader);

                        if (journalVoucherEntries != null && persisted.TotalValue == journalVoucherEntries.Sum(x => x.Amount))
                        {
                            var primaryJournal = JournalFactory.CreateJournal(null, journalVoucherDTO.PostingPeriodId, journalVoucherDTO.BranchId, null, journalVoucherDTO.TotalValue, journalVoucherDTO.PrimaryDescription, journalVoucherDTO.SecondaryDescription, string.Format("{0}~JV#{1}", journalVoucherDTO.Reference, journalVoucherDTO.PaddedVoucherNumber), moduleNavigationItemCode, (int)SystemTransactionCode.JournalVoucher, journalVoucherDTO.ValueDate, serviceHeader);

                            switch ((JournalVoucherType)journalVoucherDTO.Type)
                            {
                                case JournalVoucherType.DebitGLAccount:
                                    _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.TotalValue * -1, serviceHeader);
                                    break;
                                case JournalVoucherType.CreditGLAccount:
                                    _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.TotalValue, serviceHeader);
                                    break;
                                case JournalVoucherType.DebitCustomerAccount:
                                    _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.CustomerAccountId.Value, journalVoucherDTO.TotalValue * -1, serviceHeader);
                                    break;
                                case JournalVoucherType.CreditCustomerAccount:
                                    _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherDTO.CustomerAccountId.Value, journalVoucherDTO.TotalValue, serviceHeader);
                                    break;
                                default:
                                    break;
                            }

                            foreach (var journalVoucherEntryDTO in journalVoucherEntries)
                            {
                                switch ((JournalVoucherType)journalVoucherDTO.Type)
                                {
                                    case JournalVoucherType.DebitGLAccount:
                                        if (journalVoucherEntryDTO.CustomerAccountId.HasValue)
                                            _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.CustomerAccountId.Value, journalVoucherEntryDTO.Amount, serviceHeader);
                                        else _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.Amount, serviceHeader);
                                        break;
                                    case JournalVoucherType.CreditGLAccount:
                                        if (journalVoucherEntryDTO.CustomerAccountId.HasValue)
                                            _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.CustomerAccountId.Value, journalVoucherEntryDTO.Amount * -1, serviceHeader);
                                        else _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.Amount * -1, serviceHeader);
                                        break;
                                    case JournalVoucherType.DebitCustomerAccount:
                                        if (journalVoucherEntryDTO.CustomerAccountId.HasValue)
                                            _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.CustomerAccountId.Value, journalVoucherEntryDTO.Amount, serviceHeader);
                                        else _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.Amount, serviceHeader);
                                        break;
                                    case JournalVoucherType.CreditCustomerAccount:
                                        if (journalVoucherEntryDTO.CustomerAccountId.HasValue)
                                            _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.CustomerAccountId.Value, journalVoucherEntryDTO.Amount * -1, serviceHeader);
                                        else _journalEntryPostingService.PerformSingleEntry(primaryJournal, journalVoucherEntryDTO.ChartOfAccountId, journalVoucherDTO.ChartOfAccountId, journalVoucherEntryDTO.Amount * -1, serviceHeader);
                                        break;
                                    default:
                                        break;
                                }

                                var persistedJournalVoucherEntry = _journalVoucherEntryRepository.Get(journalVoucherEntryDTO.Id, serviceHeader);

                                if (persistedJournalVoucherEntry != null)
                                {
                                    persistedJournalVoucherEntry.Status = (int)ExpensePayableEntryStatus.Posted;
                                }
                            }

                            journals.Add(primaryJournal);

                            persisted.Status = (int)JournalVoucherStatus.Posted;

                            persisted.AuthorizationRemarks = journalVoucherDTO.AuthorizationRemarks;

                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }

                        break;
                    case JournalVoucherAuthOption.Reject:

                        persisted.Status = (int)JournalVoucherStatus.Rejected;

                        persisted.AuthorizationRemarks = journalVoucherDTO.AuthorizationRemarks;

                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        var rejectedJournalVoucherEntries = FindJournalVoucherEntriesByJournalVoucherId(persisted.Id, serviceHeader);

                        if (rejectedJournalVoucherEntries != null && rejectedJournalVoucherEntries.Any())
                        {
                            foreach (var item in rejectedJournalVoucherEntries)
                            {
                                var persistedJournalVoucherEntry = _journalVoucherEntryRepository.Get(item.Id, serviceHeader);

                                if (persistedJournalVoucherEntry != null)
                                {
                                    persistedJournalVoucherEntry.Status = (int)JournalVoucherEntryStatus.Rejected;
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

        public bool UpdateJournalVoucherEntryCollection(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntryCollection, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != null && journalVoucherEntryCollection != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _journalVoucherRepository.Get(journalVoucherId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindJournalVoucherEntriesByJournalVoucherId(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var journalVoucherEntry = _journalVoucherEntryRepository.Get(item.Id, serviceHeader);

                                if (journalVoucherEntry != null)
                                {
                                    _journalVoucherEntryRepository.Remove(journalVoucherEntry, serviceHeader);
                                }
                            }
                        }

                        if (journalVoucherEntryCollection.Any())
                        {
                            foreach (var item in journalVoucherEntryCollection)
                            {
                                var journalVoucherEntry = JournalVoucherEntryFactory.CreateJournalVoucherEntry(persisted.Id, item.ChartOfAccountId, item.CustomerAccountId, item.Amount);

                                journalVoucherEntry.Status = (int)JournalVoucherEntryStatus.Pending;
                                journalVoucherEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                _journalVoucherEntryRepository.Add(journalVoucherEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<JournalVoucherDTO> FindJournalVouchers(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var journalVouchers = _journalVoucherRepository.GetAll(serviceHeader);

                if (journalVouchers != null && journalVouchers.Any())
                {
                    return journalVouchers.ProjectedAsCollection<JournalVoucherDTO>();
                }
                else return null;
            }
        }

        public List<JournalVoucherEntryDTO> FindJournalVoucherEntries(Guid journalVoucherId, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(journalVoucherId);

                    ISpecification<JournalVoucherEntry> spec = filter;

                    var journalVoucherEntries = _journalVoucherEntryRepository.AllMatching(spec, serviceHeader);

                    if (journalVoucherEntries != null)
                    {
                        return journalVoucherEntries.ProjectedAsCollection<JournalVoucherEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateJournalVoucherEntries(Guid journalVoucherId, List<JournalVoucherEntryDTO> journalVoucherEntries, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != null && journalVoucherEntries != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _journalVoucherRepository.Get(journalVoucherId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindJournalVoucherEntries(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var journalVoucherEntry = _journalVoucherEntryRepository.Get(item.Id, serviceHeader);

                                if (journalVoucherEntry != null)
                                {
                                    _journalVoucherEntryRepository.Remove(journalVoucherEntry, serviceHeader);
                                }
                            }
                        }

                        if (journalVoucherEntries.Any())
                        {
                            foreach (var item in journalVoucherEntries)
                            {
                                var journalVoucherEntry = JournalVoucherEntryFactory.CreateJournalVoucherEntry(persisted.Id, item.ChartOfAccountId, item.CustomerAccountId, item.Amount);

                                _journalVoucherEntryRepository.Add(journalVoucherEntry, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalVoucherSpecifications.DefaultSpec();

                ISpecification<JournalVoucher> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalVoucherPagedCollection = _journalVoucherRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalVoucherPagedCollection != null)
                {
                    var pageCollection = journalVoucherPagedCollection.PageCollection.ProjectedAsCollection<JournalVoucherDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            var postedItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.PostedJournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = journalVoucherPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalVoucherDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalVoucherSpecifications.JournalVoucherWithDateRangeAndFullText(startDate, endDate, text);

                ISpecification<JournalVoucher> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalVoucherPagedCollection = _journalVoucherRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalVoucherPagedCollection != null)
                {
                    var pageCollection = journalVoucherPagedCollection.PageCollection.ProjectedAsCollection<JournalVoucherDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            var postedItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.PostedJournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = journalVoucherPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalVoucherDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<JournalVoucherDTO> FindJournalVouchers(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalVoucherSpecifications.JournalVouchersWithStatus(status, startDate, endDate, text);

                ISpecification<JournalVoucher> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var journalVoucherPagedCollection = _journalVoucherRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalVoucherPagedCollection != null)
                {
                    var pageCollection = journalVoucherPagedCollection.PageCollection.ProjectedAsCollection<JournalVoucherDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            var postedItems = _journalVoucherEntryRepository.AllMatchingCount(JournalVoucherEntrySpecifications.PostedJournalVoucherEntryWithJournalVoucherId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = journalVoucherPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalVoucherDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public JournalVoucherDTO FindJournalVoucher(Guid journalVoucherId, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var journalVoucher = _journalVoucherRepository.Get(journalVoucherId, serviceHeader);

                    if (journalVoucher != null)
                    {
                        return journalVoucher.ProjectedAs<JournalVoucherDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != null && journalVoucherId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(journalVoucherId);

                    ISpecification<JournalVoucherEntry> spec = filter;

                    var journalVoucherEntries = _journalVoucherEntryRepository.AllMatching(spec, serviceHeader);

                    if (journalVoucherEntries != null && journalVoucherEntries.Any())
                    {
                        return journalVoucherEntries.ProjectedAsCollection<JournalVoucherEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalVoucherEntryDTO> FindJournalVoucherEntriesByJournalVoucherId(Guid journalVoucherId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (journalVoucherId != null && journalVoucherId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(journalVoucherId);

                    ISpecification<JournalVoucherEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var journalVoucherPagedCollection = _journalVoucherEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalVoucherPagedCollection != null)
                    {
                        var persisted = _journalVoucherRepository.Get(journalVoucherId, serviceHeader);

                        var persistedEntriesTotal = 0m;

                        var journalVoucherEntries = _journalVoucherEntryRepository.AllMatching(JournalVoucherEntrySpecifications.JournalVoucherEntryWithJournalVoucherId(persisted.Id), serviceHeader);

                        if (journalVoucherEntries != null && journalVoucherEntries.Any())
                            persistedEntriesTotal = journalVoucherEntries.Sum(x => x.Amount);

                        var pageCollection = journalVoucherPagedCollection.PageCollection.ProjectedAsCollection<JournalVoucherEntryDTO>();

                        var itemsCount = journalVoucherPagedCollection.ItemsCount;

                        return new PageCollectionInfo<JournalVoucherEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }

    }
}
