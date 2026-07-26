using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class PostingPeriodAppService : IPostingPeriodAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PostingPeriod> _postingPeriodRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IBranchAppService _branchAppService;
        private readonly IAppCache _appCache;

        public PostingPeriodAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<PostingPeriod> postingPeriodRepository,
            IChartOfAccountAppService chartOfAccountAppService,
            ISqlCommandAppService sqlCommandAppService,
            IJournalEntryPostingService journalEntryPostingService,
            IBranchAppService branchAppService,
            IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (postingPeriodRepository == null)
                throw new ArgumentNullException(nameof(postingPeriodRepository));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _postingPeriodRepository = postingPeriodRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _branchAppService = branchAppService;
            _appCache = appCache;
        }

        public PostingPeriodDTO AddNewPostingPeriod(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader)
        {
            if (postingPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var duration = new Duration(postingPeriodDTO.DurationStartDate, postingPeriodDTO.DurationEndDate);

                    var postingPeriod = PostingPeriodFactory.CreatePostingPeriod(postingPeriodDTO.Description, duration);

                    postingPeriod.CreatedBy = serviceHeader.ApplicationUserName;

                    _postingPeriodRepository.Add(postingPeriod, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return postingPeriod.ProjectedAs<PostingPeriodDTO>();
                }
            }
            else return null;
        }

        public bool UpdatePostingPeriod(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader)
        {
            if (postingPeriodDTO == null || postingPeriodDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _postingPeriodRepository.Get(postingPeriodDTO.Id, serviceHeader);

                if (persisted != null && !persisted.IsClosed)
                {
                    var duration = new Duration(postingPeriodDTO.DurationStartDate, postingPeriodDTO.DurationEndDate);

                    var current = PostingPeriodFactory.CreatePostingPeriod(postingPeriodDTO.Description, duration);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;
                    
                    _postingPeriodRepository.Merge(persisted, current, serviceHeader);

                    // Lock?
                    if (postingPeriodDTO.IsLocked && !persisted.IsLocked)
                        LockPostingPeriod(persisted.Id, serviceHeader);

                    // Activate?
                    if (postingPeriodDTO.IsActive && !persisted.IsActive)
                        ActivatePostingPeriod(persisted.Id, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool ClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var appropriationChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.Appropriation, serviceHeader);

            var branches = _branchAppService.FindBranches(serviceHeader);

            if (branches != null && branches.Any() && postingPeriodDTO != null && appropriationChartOfAccountId != Guid.Empty)
            {
                if (ClosePostingPeriod(postingPeriodDTO, serviceHeader))
                {
                    var glAccounts = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader);

                    if (glAccounts != null && glAccounts.Any())
                    {
                        var journals = new List<Journal>();

                        var secondaryDescription = "Fiscal Period Closing";

                        var reference = postingPeriodDTO.Description;

                        glAccounts.ForEach(glAccount =>
                        {
                            switch ((ChartOfAccountType)glAccount.Type)
                            {
                                case ChartOfAccountType.Asset:
                                    break;
                                case ChartOfAccountType.Liability:
                                    break;
                                case ChartOfAccountType.Equity:
                                    break;
                                case ChartOfAccountType.Income:

                                    branches.ForEach(branchDTO =>
                                    {
                                        _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(branchDTO.Id, new List<GeneralLedgerAccount> { glAccount }, postingPeriodDTO.DurationEndDate, serviceHeader, TransactionDateFilter.ValueDate);

                                        var incomeAmount = Math.Abs(glAccount.Balance);

                                        var incomeJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, branchDTO.Id, null, incomeAmount, glAccount.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FiscalPeriodClosing, postingPeriodDTO.DurationEndDate, serviceHeader);

                                        _journalEntryPostingService.PerformDoubleEntry(incomeJournal, appropriationChartOfAccountId, glAccount.Id, serviceHeader);

                                        journals.Add(incomeJournal);
                                    });

                                    break;
                                case ChartOfAccountType.Expense:

                                    branches.ForEach(branchDTO =>
                                    {
                                        _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(branchDTO.Id, new List<GeneralLedgerAccount> { glAccount }, postingPeriodDTO.DurationEndDate, serviceHeader, TransactionDateFilter.ValueDate);

                                        var expenseAmount = Math.Abs(glAccount.Balance);

                                        var expenseJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, branchDTO.Id, null, expenseAmount, glAccount.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FiscalPeriodClosing, postingPeriodDTO.DurationEndDate, serviceHeader);

                                        _journalEntryPostingService.PerformDoubleEntry(expenseJournal, glAccount.Id, appropriationChartOfAccountId, serviceHeader);

                                        journals.Add(expenseJournal);
                                    });

                                    break;
                                default:
                                    break;
                            }
                        });

                        #region Bulk-Insert journals && journal entries

                        if (journals.Any())
                        {
                            result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                        }

                        #endregion
                    }
                }
            }

            return result;
        }

        public List<PostingPeriodDTO> FindPostingPeriods(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var postingPeriods = _postingPeriodRepository.GetAll(serviceHeader);

                if (postingPeriods != null && postingPeriods.Any())
                {
                    return postingPeriods.ProjectedAsCollection<PostingPeriodDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<PostingPeriodDTO> FindPostingPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PostingPeriodSpecifications.DefaultSpec();

                ISpecification<PostingPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var postingPeriodPagedCollection = _postingPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (postingPeriodPagedCollection != null)
                {
                    var pageCollection = postingPeriodPagedCollection.PageCollection.ProjectedAsCollection<PostingPeriodDTO>();

                    var itemsCount = postingPeriodPagedCollection.ItemsCount;

                    return new PageCollectionInfo<PostingPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<PostingPeriodDTO> FindPostingPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PostingPeriodSpecifications.PostingPeriodFullText(text);

                ISpecification<PostingPeriod> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var postingPeriodCollection = _postingPeriodRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (postingPeriodCollection != null)
                {
                    var pageCollection = postingPeriodCollection.PageCollection.ProjectedAsCollection<PostingPeriodDTO>();

                    var itemsCount = postingPeriodCollection.ItemsCount;

                    return new PageCollectionInfo<PostingPeriodDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PostingPeriodDTO FindPostingPeriod(Guid postingPeriodId, ServiceHeader serviceHeader)
        {
            if (postingPeriodId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var postingPeriod = _postingPeriodRepository.Get(postingPeriodId, serviceHeader);

                    if (postingPeriod != null)
                    {
                        return postingPeriod.ProjectedAs<PostingPeriodDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PostingPeriodDTO FindCurrentPostingPeriod(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = PostingPeriodSpecifications.CurrentPostingPeriod();

                ISpecification<PostingPeriod> spec = filter;

                var postingPeriods = _postingPeriodRepository.AllMatching(spec, serviceHeader);

                if (postingPeriods != null && postingPeriods.Any() && postingPeriods.Count() == 1)
                {
                    var postingPeriod = postingPeriods.SingleOrDefault();

                    if (postingPeriod != null)
                    {
                        return postingPeriod.ProjectedAs<PostingPeriodDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        public PostingPeriodDTO FindCachedCurrentPostingPeriod(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<PostingPeriodDTO>(string.Format("CurrentPostingPeriod_{0}", serviceHeader.ApplicationDomainName), () =>
            {
                return FindCurrentPostingPeriod(serviceHeader);
            });
        }

        private bool ActivatePostingPeriod(Guid postingPeriodId, ServiceHeader serviceHeader)
        {
            if (postingPeriodId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _postingPeriodRepository.Get(postingPeriodId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Activate();

                    var otherPeriods = _postingPeriodRepository.GetAll(serviceHeader);

                    foreach (var item in otherPeriods)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var postingPeriod = _postingPeriodRepository.Get(item.Id, serviceHeader);

                            postingPeriod.DeActivate();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        private bool LockPostingPeriod(Guid postingPeriodId, ServiceHeader serviceHeader)
        {
            if (postingPeriodId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _postingPeriodRepository.Get(postingPeriodId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Lock();

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        private bool ClosePostingPeriod(PostingPeriodDTO postingPeriodDTO, ServiceHeader serviceHeader)
        {
            if (postingPeriodDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _postingPeriodRepository.Get(postingPeriodDTO.Id, serviceHeader);

                    if (persisted != null && !persisted.IsClosed && !persisted.IsLocked)
                    {
                        persisted.Close();
                        persisted.DeActivate();
                        persisted.Lock();

                        persisted.ClosedBy = serviceHeader.ApplicationUserName;
                        persisted.ClosedDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

    }
}
