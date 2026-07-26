using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ChartOfAccountAppService : IChartOfAccountAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly IRepository<SystemGeneralLedgerAccountMapping> _systemGeneralLedgerAccountMappingRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IAppCache _appCache;

        public ChartOfAccountAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<ChartOfAccount> chartOfAccountRepository,
            IRepository<SystemGeneralLedgerAccountMapping> systemGeneralLedgerAccountMappingRepository,
            ISqlCommandAppService sqlCommandAppService,
            IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (chartOfAccountRepository == null)
                throw new ArgumentNullException(nameof(chartOfAccountRepository));

            if (systemGeneralLedgerAccountMappingRepository == null)
                throw new ArgumentNullException(nameof(systemGeneralLedgerAccountMappingRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _chartOfAccountRepository = chartOfAccountRepository;
            _systemGeneralLedgerAccountMappingRepository = systemGeneralLedgerAccountMappingRepository;
            _sqlCommandAppService = sqlCommandAppService;
            _appCache = appCache;
        }

        public async Task<PageCollectionInfo<ChartOfAccountDTO>> FindChartOfAccountsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<ChartOfAccount> filter = ChartOfAccountSpecifications.ChartOfAccountFullText(text);

                ISpecification<ChartOfAccount> spec = filter;

                List<string> sortFields = new List<string> { "SequentialId" };

                return await _chartOfAccountRepository.AllMatchingPagedAsync<ChartOfAccountDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public PageCollectionInfo<ChartOfAccountDTO> FindChartOfAccounts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.ChartOfAccountFullText(text);

                ISpecification<ChartOfAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chartOfAccountCollection = _chartOfAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chartOfAccountCollection != null)
                {
                    var pageCollection = chartOfAccountCollection.PageCollection.ProjectedAsCollection<ChartOfAccountDTO>();

                    var itemsCount = chartOfAccountCollection.ItemsCount;

                    return new PageCollectionInfo<ChartOfAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }



        public PageCollectionInfo<ChartOfAccountDTO> FindChartOfAccountsWithCategoryInPage(int accountCategory, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                //var filter = ChartOfAccountSpecifications.ChartOfAccountFullText(text);

                var filter = ChartOfAccountSpecifications.ChartOfAccountWithAccountCategory(accountCategory);

                ISpecification<ChartOfAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chartOfAccountCollection = _chartOfAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chartOfAccountCollection != null)
                {
                    var pageCollection = chartOfAccountCollection.PageCollection.ProjectedAsCollection<ChartOfAccountDTO>();

                    var itemsCount = chartOfAccountCollection.ItemsCount;

                    return new PageCollectionInfo<ChartOfAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ChartOfAccountDTO AddNewChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader)
        {
            if (chartOfAccountDTO != null)
            {
                
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    //get the specification
                    ISpecification<ChartOfAccount> spec = ChartOfAccountSpecifications.ChartOfAccountWithAccountCode(chartOfAccountDTO.AccountCode);

                    //Query this criteria
                    var matchedChartOfAccounts = _chartOfAccountRepository.AllMatching(spec, serviceHeader);

                    if (matchedChartOfAccounts != null && matchedChartOfAccounts.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        chartOfAccountDTO.ErrorMessageResult = string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode);
                        return chartOfAccountDTO;
                    }


                    else
                    {
                        // Get persisted item
                        var persistedParent = _chartOfAccountRepository.Get(chartOfAccountDTO.ParentId ?? Guid.Empty, serviceHeader);

                        // Create chart of account from factory and set persistent id
                        var newChartOfAccount =
                              (persistedParent != null)
                              ? ChartOfAccountFactory.CreateChartOfAccount(persistedParent, (ChartOfAccountCategory)chartOfAccountDTO.AccountCategory, chartOfAccountDTO.AccountCode, chartOfAccountDTO.AccountName, chartOfAccountDTO.IsControlAccount, chartOfAccountDTO.IsReconciliationAccount, chartOfAccountDTO.PostAutomaticallyOnly, chartOfAccountDTO.CostCenterId)
                              : ChartOfAccountFactory.CreateChartOfAccount((ChartOfAccountType)chartOfAccountDTO.AccountType, (ChartOfAccountCategory)chartOfAccountDTO.AccountCategory, chartOfAccountDTO.AccountCode, chartOfAccountDTO.AccountName, chartOfAccountDTO.IsControlAccount, chartOfAccountDTO.IsReconciliationAccount, chartOfAccountDTO.PostAutomaticallyOnly, chartOfAccountDTO.CostCenterId);

                        if (chartOfAccountDTO.IsLocked)
                            newChartOfAccount.Lock();
                        else newChartOfAccount.UnLock();

                        // Save entity
                        _chartOfAccountRepository.Add(newChartOfAccount, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return newChartOfAccount.ProjectedAs<ChartOfAccountDTO>();
                    }
                }
            }
            else // if chart of account is null , cannot create a new chart of account
                return null;
        }

        public bool UpdateChartOfAccount(ChartOfAccountDTO chartOfAccountDTO, ServiceHeader serviceHeader)
        {
            if (chartOfAccountDTO == null || chartOfAccountDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Get persisted item
                var persisted = _chartOfAccountRepository.Get(chartOfAccountDTO.Id, serviceHeader);

                if (persisted != null) // If chart of account exists
                {
                    //get the specification
                    ISpecification<ChartOfAccount> spec = ChartOfAccountSpecifications.ChartOfAccountWithAccountCode(chartOfAccountDTO.AccountCode);

                    //Query this criteria
                    var matchedChartOfAccounts = _chartOfAccountRepository.AllMatching(spec, serviceHeader);

                    if (matchedChartOfAccounts != null && matchedChartOfAccounts.Any(x => x.Id != chartOfAccountDTO.Id))
                        throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                    else
                    {
                        // Get persisted parent item
                        var persistedParent = _chartOfAccountRepository.Get(chartOfAccountDTO.ParentId ?? Guid.Empty, serviceHeader);

                        //create the current instance with changes from customerDTO
                        var current =
                            (persistedParent != null)
                            ? ChartOfAccountFactory.CreateChartOfAccount(persistedParent, (ChartOfAccountCategory)chartOfAccountDTO.AccountCategory, chartOfAccountDTO.AccountCode, chartOfAccountDTO.AccountName, chartOfAccountDTO.IsControlAccount, chartOfAccountDTO.IsReconciliationAccount, chartOfAccountDTO.PostAutomaticallyOnly, chartOfAccountDTO.CostCenterId)
                            : ChartOfAccountFactory.CreateChartOfAccount((ChartOfAccountType)chartOfAccountDTO.AccountType, (ChartOfAccountCategory)chartOfAccountDTO.AccountCategory, chartOfAccountDTO.AccountCode, chartOfAccountDTO.AccountName, chartOfAccountDTO.IsControlAccount, chartOfAccountDTO.IsReconciliationAccount, chartOfAccountDTO.PostAutomaticallyOnly, chartOfAccountDTO.CostCenterId);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                        if (chartOfAccountDTO.IsLocked)
                            current.Lock();
                        else current.UnLock();

                        // Merge changes
                        _chartOfAccountRepository.Merge(persisted, current, serviceHeader);

                        // Commit unit of work
                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
        }

        public ChartOfAccountDTO FindChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            if (chartOfAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    //recover existing ChartOfAccount and map
                    var persisted = _chartOfAccountRepository.Get(chartOfAccountId, serviceHeader);

                    if (persisted != null) //adapt
                    {
                        return persisted.ProjectedAs<ChartOfAccountDTO>();
                    }
                    else
                        return null;
                }
            }
            else
                return null;
        }

        public ChartOfAccountSummaryDTO FindChartOfAccountSummary(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _chartOfAccountRepository.Get<ChartOfAccountSummaryDTO>(chartOfAccountId, serviceHeader);
            }
        }

        public ChartOfAccountSummaryDTO FindCachedChartOfAccountSummary(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ChartOfAccountSummaryDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, chartOfAccountId.ToString("D")), () =>
            {
                return FindChartOfAccountSummary(chartOfAccountId, serviceHeader);
            });
        }

        public List<ChartOfAccountSummaryDTO> FindChartOfAccounts(Guid[] chartOfAccountIds, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.ChartOfAccountWithId(chartOfAccountIds);

                ISpecification<ChartOfAccount> spec = filter;

                return _chartOfAccountRepository.AllMatching<ChartOfAccountSummaryDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<ChartOfAccountSummaryDTO>> FindChartOfAccountsAsync(Guid[] chartOfAccountIds, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.ChartOfAccountWithId(chartOfAccountIds);

                ISpecification<ChartOfAccount> spec = filter;

                return await _chartOfAccountRepository.AllMatchingAsync<ChartOfAccountSummaryDTO>(spec, serviceHeader);
            }
        }

        public List<ChartOfAccountDTO> FindChartOfAccounts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.DefaultSpec();

                ISpecification<ChartOfAccount> spec = filter;

                var chartOfAccounts = _chartOfAccountRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (chartOfAccounts != null && chartOfAccounts.Any())
                {
                    return chartOfAccounts.ProjectedAsCollection<ChartOfAccountDTO>();
                }
                else return null;
            }
        }

        public List<ChartOfAccountDTO> FindChartOfAccounts(int chartOfAccountCode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.ChartOfAccountWithAccountCode(chartOfAccountCode);

                ISpecification<ChartOfAccount> spec = filter;

                var chartOfAccounts = _chartOfAccountRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (chartOfAccounts != null && chartOfAccounts.Any())
                {
                    return chartOfAccounts.ProjectedAsCollection<ChartOfAccountDTO>();
                }
                else return null;
            }
        }



        public List<GeneralLedgerAccount> FindGeneralLedgerAccountsWithCategoryAndText(string text, int? accountCategory, ServiceHeader serviceHeader, bool updateDepth)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChartOfAccountSpecifications.ChartOfAccountFullTextAndCategory(text, accountCategory);
                ISpecification<ChartOfAccount> spec = filter;

                var chartOfAccounts = _chartOfAccountRepository.AllMatching(spec, serviceHeader, c => c.Children);

                if (chartOfAccounts != null && chartOfAccounts.Any())
                {
                    var glAccountsList = new List<GeneralLedgerAccount>();

                    // First, get only root accounts (ParentId is null)
                    var rootAccounts = chartOfAccounts.Where(c => c.ParentId == null).OrderBy(c => c.AccountCode).ToList();

                    // Recursive function to traverse and add accounts
                    void Traverse(ChartOfAccount node, int depth)
                    {
                        // Create tabs based on depth
                        string tabs = string.Empty;
                        for (int i = 0; i < depth; i++)
                        {
                            tabs += "\t";
                        }

                        var glAccount = new GeneralLedgerAccount();
                        glAccount.Id = node.Id;
                        glAccount.ParentId = node.ParentId;
                        glAccount.Category = node.AccountCategory;
                        glAccount.CategoryDescription = EnumHelper.GetDescription((ChartOfAccountCategory)node.AccountCategory);
                        glAccount.Type = (int)node.AccountType;
                        glAccount.TypeDescription = EnumHelper.GetDescription((ChartOfAccountType)node.AccountType);
                        glAccount.Code = node.AccountCode;
                        glAccount.Description = node.AccountName;
                        glAccount.Name = string.Format("{0}-{1} {2}", node.AccountType, node.AccountCode, node.AccountName);
                        glAccount.IndentedName = string.Format("{0}{1}-{2} {3}", tabs, node.AccountType, node.AccountCode, node.AccountName);
                        glAccount.Depth = depth;
                        glAccount.IsControlAccount = node.IsControlAccount;
                        glAccount.IsReconciliationAccount = node.IsReconciliationAccount;
                        glAccount.PostAutomaticallyOnly = node.PostAutomaticallyOnly;
                        glAccount.IsLocked = node.IsLocked;
                        glAccount.CreatedDate = node.CreatedDate;

                        if (node.CostCenterId != null)
                        {
                            glAccount.CostCenterId = node.CostCenterId;
                        }

                        glAccountsList.Add(glAccount);

                        // Process children (order them by AccountCode or Name as needed)
                        if (node.Children != null && node.Children.Any())
                        {
                            foreach (var child in node.Children.OrderBy(c => c.AccountCode))
                            {
                                Traverse(child, depth + 1);
                            }
                        }
                    }

                    // Process each root account
                    foreach (var root in rootAccounts)
                    {
                        Traverse(root, 0);
                    }

                    #region update depth
                    // TODO: Your existing update depth logic
                    #endregion

                    return glAccountsList;
                }
                else return null;
            }
        }






        public List<GeneralLedgerAccount> FindGeneralLedgerAccounts(ServiceHeader serviceHeader, bool updateDepth)
        {
            var chartOfAccounts = FindChartOfAccounts(serviceHeader);

            if (chartOfAccounts != null && chartOfAccounts.Any())
            {
                var glAccountsList = new List<GeneralLedgerAccount>();

                Action<ChartOfAccountDTO> traverse = null;

                /* recursive lambda */
                traverse = (node) =>
                {
                    string tabs = string.Empty;
                    int depth = 0;

                    var tempNode = node;
                    while (tempNode.Parent != null)
                    {
                        tempNode = tempNode.Parent;
                        tabs += "\t";
                        depth++;
                    }

                    var glAccount = new GeneralLedgerAccount();
                    glAccount.Id = node.Id;
                    glAccount.ParentId = node.ParentId;
                    glAccount.Category = node.AccountCategory;
                    glAccount.CategoryDescription = EnumHelper.GetDescription((ChartOfAccountCategory)node.AccountCategory);
                    glAccount.Type = (int)node.AccountType;
                    glAccount.TypeDescription = EnumHelper.GetDescription((ChartOfAccountType)node.AccountType);
                    glAccount.Code = node.AccountCode;
                    glAccount.Description = node.AccountName;
                    glAccount.Name = string.Format("{0}-{1} {2}", node.AccountType, node.AccountCode, node.AccountName);
                    glAccount.IndentedName = string.Format("{0}{1}-{2} {3}", tabs, node.AccountType, node.AccountCode, node.AccountName);
                    glAccount.Depth = depth;
                    glAccount.IsControlAccount = node.IsControlAccount;
                    glAccount.IsReconciliationAccount = node.IsReconciliationAccount;
                    glAccount.PostAutomaticallyOnly = node.PostAutomaticallyOnly;
                    glAccount.IsLocked = node.IsLocked;
                    glAccount.CreatedDate = node.CreatedDate;

                    if (node.CostCenterId != null)
                    {
                        glAccount.CostCenterId = node.CostCenterId;
                        glAccount.CostCenterDescription = node.CostCenterDescription;
                    }

                    glAccountsList.Add(glAccount);

                    if (node.Children != null)
                    {
                        foreach (var item in node.Children)
                        {
                            traverse(item);
                        }
                    }
                };

                foreach (var c in chartOfAccounts)
                {
                    traverse(c);
                }

                #region update depth
                // TODO:
                //if (updateDepth && glAccountsList.Any())
                //{
                //    glAccountsList.ForEach(item =>
                //    {
                //        if (item != null)
                //        {
                //            var chartOfAccount = _chartOfAccountRepository.Get(item.Id);

                //            if (chartOfAccount != null)
                //                chartOfAccount.Depth = item.Depth;
                //        }
                //    });

                //   dbContextScope.SaveChanges(serviceHeader);
                //}

                #endregion

                return glAccountsList;
            }
            else return null;
        }

        public GeneralLedgerAccount FindGeneralLedgerAccount(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            var chartOfAccountSummaryDTO = FindCachedChartOfAccountSummary(chartOfAccountId, serviceHeader);

            if (chartOfAccountSummaryDTO != null)
            {
                var glAccount = new GeneralLedgerAccount
                {
                    Id = chartOfAccountSummaryDTO.Id,
                    ParentId = chartOfAccountSummaryDTO.ParentId,
                    Category = chartOfAccountSummaryDTO.AccountCategory,
                    CategoryDescription = EnumHelper.GetDescription((ChartOfAccountCategory)chartOfAccountSummaryDTO.AccountCategory),
                    Type = chartOfAccountSummaryDTO.AccountType,
                    TypeDescription = EnumHelper.GetDescription((ChartOfAccountType)chartOfAccountSummaryDTO.AccountType),
                    Code = chartOfAccountSummaryDTO.AccountCode,
                    Description = chartOfAccountSummaryDTO.AccountName,
                    Name = string.Format("{0}-{1} {2}", ((int)chartOfAccountSummaryDTO.AccountType).FirstDigit(), chartOfAccountSummaryDTO.AccountCode, chartOfAccountSummaryDTO.AccountName),
                    IndentedName = string.Format("{0}{1}-{2} {3}", "\t", ((int)chartOfAccountSummaryDTO.AccountType).FirstDigit(), chartOfAccountSummaryDTO.AccountCode, chartOfAccountSummaryDTO.AccountName),
                    Depth = chartOfAccountSummaryDTO.Depth,
                    IsControlAccount = chartOfAccountSummaryDTO.IsControlAccount,
                    IsReconciliationAccount = chartOfAccountSummaryDTO.IsReconciliationAccount,
                    PostAutomaticallyOnly = chartOfAccountSummaryDTO.PostAutomaticallyOnly,
                    IsLocked = chartOfAccountSummaryDTO.IsLocked,
                    CreatedDate = chartOfAccountSummaryDTO.CreatedDate,
                    CostCenterId = chartOfAccountSummaryDTO.CostCenterId,
                };

                return glAccount;
            }
            else return null;
        }

        public Guid GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, ServiceHeader serviceHeader)
        {
            var mappings = FindSystemGeneralLedgerAccountMappings(serviceHeader) ?? new List<SystemGeneralLedgerAccountMappingDTO>();

            var target = mappings.FirstOrDefault(x => x.SystemGeneralLedgerAccountCode == systemGeneralLedgerAccountCode);

            if (target != null)
            {
                return target.ChartOfAccountId;
            }
            else return Guid.Empty;
        }

        public Guid GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<Guid>(string.Format("ChartOfAccountMappingForSystemGeneralLedgerAccountCode_{0}_{1}", serviceHeader.ApplicationDomainName, systemGeneralLedgerAccountCode), () =>
            {
                return GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(systemGeneralLedgerAccountCode, serviceHeader);
            });
        }

        public Guid GetCostCenterMappingForChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            if (chartOfAccountId != Guid.Empty)
            {
                var glAccounts = FindGeneralLedgerAccounts(serviceHeader, false) ?? new List<GeneralLedgerAccount>();

                var target = glAccounts.FirstOrDefault(x => x.Id == chartOfAccountId);

                if (target != null)
                {
                    return target.CostCenterId != null ? target.CostCenterId.Value : Guid.Empty;
                }
                else return Guid.Empty;
            }
            else return Guid.Empty;
        }

        public Guid GetCachedCostCenterMappingForChartOfAccount(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<Guid>(string.Format("CostCenterMappingForChartOfAccount_{0}_{1}", serviceHeader.ApplicationDomainName, chartOfAccountId.ToString("D")), () =>
            {
                return GetCostCenterMappingForChartOfAccount(chartOfAccountId, serviceHeader);
            });
        }

        public bool MapSystemGeneralLedgerAccountCodeToChartOfAccount(int systemGeneralLedgerAccountCode, Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            if (chartOfAccountId == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var filter = SystemGeneralLedgerAccountMappingSpecifications.SystemGeneralLedgerAccountCode(systemGeneralLedgerAccountCode);

                // get the specification
                ISpecification<SystemGeneralLedgerAccountMapping> spec = filter;

                // Query this criteria
                var matches = _systemGeneralLedgerAccountMappingRepository.AllMatching(spec, serviceHeader);

                if (matches != null && matches.Any()) // existing mapping
                {
                    foreach (var item in matches)
                    {
                        var persisted = _systemGeneralLedgerAccountMappingRepository.Get(item.Id, serviceHeader);

                        persisted.ChartOfAccountId = chartOfAccountId;
                    }
                }
                else // new mapping
                {
                    var systemGeneralLedgerAccountMapping = SystemGeneralLedgerAccountMappingFactory.CreateSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountCode, chartOfAccountId);

                    _systemGeneralLedgerAccountMappingRepository.Add(systemGeneralLedgerAccountMapping, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemGeneralLedgerAccountMappingSpecifications.DefaultSpec();

                ISpecification<SystemGeneralLedgerAccountMapping> spec = filter;

                var systemGeneralLedgerAccountMappings = _systemGeneralLedgerAccountMappingRepository.AllMatching(spec, serviceHeader);

                if (systemGeneralLedgerAccountMappings != null && systemGeneralLedgerAccountMappings.Any())
                {
                    return systemGeneralLedgerAccountMappings.ProjectedAsCollection<SystemGeneralLedgerAccountMappingDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemGeneralLedgerAccountMappingSpecifications.DefaultSpec();

                ISpecification<SystemGeneralLedgerAccountMapping> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var systemGeneralLedgerAccountMappingPagedCollection = _systemGeneralLedgerAccountMappingRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (systemGeneralLedgerAccountMappingPagedCollection != null)
                {
                    var pageCollection = systemGeneralLedgerAccountMappingPagedCollection.PageCollection.ProjectedAsCollection<SystemGeneralLedgerAccountMappingDTO>();

                    var itemsCount = systemGeneralLedgerAccountMappingPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public void FetchGeneralLedgerAccountBalances(List<GeneralLedgerAccount> glAccounts, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate, bool includeSubTotals = false)
        {
            glAccounts.ForEach(glAccount =>
            {
                glAccount.Balance = _sqlCommandAppService.FindGlAccountBalance(glAccount.Id, cutOffDate, (int)transactionDateFilter, serviceHeader);
            });

            if (includeSubTotals) /*fetch sub-totals?*/
            {
                glAccounts.ForEach(glAccount =>
                {
                    var childItems = Level(glAccounts, glAccount.Id, 0);

                    if (childItems != null && childItems.Any())
                    {
                        glAccount.Balance = childItems.Aggregate(0m, (result, element) => result + element.Key.Balance);
                    }
                });
            }
        }

        public void FetchGeneralLedgerAccountBalances(Guid branchId, List<GeneralLedgerAccount> glAccounts, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate, bool includeSubTotals = false)
        {
            glAccounts.ForEach(glAccount =>
            {
                glAccount.Balance = _sqlCommandAppService.FindGlAccountBalance(branchId, glAccount.Id, cutOffDate, (int)transactionDateFilter, serviceHeader);
            });

            if (includeSubTotals) /*fetch sub-totals?*/
            {
                glAccounts.ForEach(glAccount =>
                {
                    var childItems = Level(glAccounts, glAccount.Id, 0);

                    if (childItems != null && childItems.Any())
                    {
                        glAccount.Balance = childItems.Aggregate(0m, (result, element) => result + element.Key.Balance);
                    }
                });
            }
        }

        public void FetchGeneralLedgerAccountBalances(GeneralLedgerAccount glAccount, DateTime cutOffDate, ServiceHeader serviceHeader, TransactionDateFilter transactionDateFilter = TransactionDateFilter.CreatedDate)
        {
            if (glAccount != null)
                glAccount.Balance = _sqlCommandAppService.FindGlAccountBalance(glAccount.Id, cutOffDate, (int)transactionDateFilter, serviceHeader);
        }

        private IEnumerable<KeyValuePair<GeneralLedgerAccount, int>> Level(List<GeneralLedgerAccount> list, Guid? parentId, int lvl)
        {
            return list
                .Where(x => x.ParentId == parentId)
                .SelectMany(x =>
                    new[] { new KeyValuePair<GeneralLedgerAccount, int>(x, lvl) }.Concat(Level(list, x.Id, lvl + 1))
                );
        }
    }
}
