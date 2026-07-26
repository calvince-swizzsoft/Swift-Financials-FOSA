using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class BudgetAppService : IBudgetAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Budget> _budgetRepository;
        private readonly IRepository<BudgetEntry> _budgetEntryRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public BudgetAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Budget> budgetRepository,
           IRepository<BudgetEntry> budgetEntryRepository,
           IPostingPeriodAppService postingPeriodAppService,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (budgetRepository == null)
                throw new ArgumentNullException(nameof(budgetRepository));

            if (budgetEntryRepository == null)
                throw new ArgumentNullException(nameof(budgetEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _budgetRepository = budgetRepository;
            _budgetEntryRepository = budgetEntryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public BudgetDTO AddNewBudget(BudgetDTO budgetDTO, ServiceHeader serviceHeader)
        {
            if (budgetDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    //get the specification
                    var filter = BudgetSpecifications.BudgetWithPostingPeriodIdAndBranchId(budgetDTO.PostingPeriodId, budgetDTO.BranchId.Value);

                    ISpecification<Budget> spec = filter;

                    //Query this criteria
                    var budgets = _budgetRepository.AllMatching(spec, serviceHeader);

                    if (budgets != null && budgets.Any())
                    {
                        budgetDTO.ErrorMessageResult = ("Sorry, but a budget for the selected posting period already exists!"+budgetDTO.PostingPeriodDescription);
                        return budgetDTO;
                    }
                    else
                    {
                        var budget = BudgetFactory.CreateBudget(budgetDTO.PostingPeriodId, budgetDTO.BranchId.Value, budgetDTO.Description, budgetDTO.TotalValue);

                        _budgetRepository.Add(budget, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return budget.ProjectedAs<BudgetDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateBudget(BudgetDTO budgetDTO, ServiceHeader serviceHeader)
        {
            if (budgetDTO == null || budgetDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _budgetRepository.Get(budgetDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    //get the specification
                    var filter = BudgetSpecifications.BudgetWithPostingPeriodIdAndBranchId(budgetDTO.PostingPeriodId, budgetDTO.BranchId.Value);

                    ISpecification<Budget> spec = filter;

                    //Query this criteria
                    var budgets = _budgetRepository.AllMatching(spec, serviceHeader);

                    if (budgets != null && budgets.Except(new List<Budget> { persisted }).Any())
                    {
                        budgetDTO.ErrorMessageResult = ("Sorry, but a budget for the selected posting period already exists!" + budgetDTO.PostingPeriodDescription);
                        return false;
                    }
                    else
                    {
                        var current = BudgetFactory.CreateBudget(budgetDTO.PostingPeriodId, budgetDTO.BranchId.Value, budgetDTO.Description, budgetDTO.TotalValue);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);


                        _budgetRepository.Merge(persisted, current, serviceHeader);

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
                else return false;
            }
        }

        public BudgetEntryDTO AddNewBudgetEntry(BudgetEntryDTO budgetEntryDTO, ServiceHeader serviceHeader)
        {
            if (budgetEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var budgetEntry = BudgetEntryFactory.CreateBudgetEntry(budgetEntryDTO.BudgetId, budgetEntryDTO.Type, budgetEntryDTO.ChartOfAccountId, budgetEntryDTO.LoanProductId, budgetEntryDTO.Amount, budgetEntryDTO.Reference);

                    budgetEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _budgetEntryRepository.Add(budgetEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return budgetEntry.ProjectedAs<BudgetEntryDTO>();
                }
            }
            else return null;
        }

        public async Task<bool> UpdateBudgetEntriesAsync(Guid budgetId, List<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _budgetRepository.GetAsync(budgetId, serviceHeader);

                if (persisted != null)
                {
                    {
                        foreach (var item in budgetEntries)
                        {
                            if (item.Id == Guid.Empty)
                            {
                                var budgetEntry = BudgetEntryFactory.CreateBudgetEntry(persisted.Id, item.Type, item.ChartOfAccountId, item.LoanProductId, item.Amount, item.Reference);

                                _budgetEntryRepository.Add(budgetEntry, serviceHeader);
                            }
                            else
                            {

                                if (!(budgetEntries != null && budgetEntries.Any()))
                                {
                                    var budgetEntry = BudgetEntryFactory.CreateBudgetEntry(persisted.Id, item.Type, item.ChartOfAccountId, item.LoanProductId, item.Amount, item.Reference);

                                    _budgetEntryRepository.Add(budgetEntry, serviceHeader);
                                }
                            }
                        }
                    }

                }
                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public bool RemoveBudgetEntries(List<BudgetEntryDTO> budgetEntryDTOs, ServiceHeader serviceHeader)
        {
            if (budgetEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in budgetEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _budgetEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _budgetEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<BudgetDTO> FindBudgets(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var budgets = _budgetRepository.GetAll(serviceHeader);

                if (budgets != null && budgets.Any())
                {
                    return budgets.ProjectedAsCollection<BudgetDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BudgetDTO> FindBudgets(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BudgetSpecifications.DefaultSpec();

                ISpecification<Budget> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var budgetPagedCollection = _budgetRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (budgetPagedCollection != null)
                {
                    var pageCollection = budgetPagedCollection.PageCollection.ProjectedAsCollection<BudgetDTO>();

                    var itemsCount = budgetPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BudgetDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BudgetDTO> FindBudgets(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? BudgetSpecifications.DefaultSpec() : BudgetSpecifications.BudgetFullText(text);

                ISpecification<Budget> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var budgetPagedCollection = _budgetRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (budgetPagedCollection != null)
                {
                    var pageCollection = budgetPagedCollection.PageCollection.ProjectedAsCollection<BudgetDTO>();

                    var itemsCount = budgetPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BudgetDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public BudgetDTO FindBudget(Guid budgetId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var budget = _budgetRepository.Get(budgetId, serviceHeader);

                if (budget != null)
                {
                    return budget.ProjectedAs<BudgetDTO>();
                }
                else return null;
            }
        }

        public BudgetDTO FindBudget(Guid postingPeriodId, Guid branchId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BudgetSpecifications.BudgetWithPostingPeriodIdAndBranchId(postingPeriodId, branchId);

                ISpecification<Budget> spec = filter;

                var budgets = _budgetRepository.AllMatching(spec, serviceHeader);

                if (budgets != null && budgets.Any() && budgets.Count() == 1)
                {
                    var budget = budgets.SingleOrDefault();

                    if (budget != null)
                    {
                        return budget.ProjectedAs<BudgetDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        public List<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BudgetEntrySpecifications.BudgetEntryWithBudgetId(budgetId);

                ISpecification<BudgetEntry> spec = filter;

                var budgetEntries = _budgetEntryRepository.AllMatching(spec, serviceHeader);

                if (budgetEntries != null)
                {
                    return budgetEntries.ProjectedAsCollection<BudgetEntryDTO>();
                }
                else return null;
            }
        }

        public List<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, int type, Guid typeIdentifier, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BudgetEntrySpecifications.BudgetEntryByBudgetIdAndTypeIdentifier(budgetId, type, typeIdentifier);

                ISpecification<BudgetEntry> spec = filter;

                var budgetEntries = _budgetEntryRepository.AllMatching(spec, serviceHeader);

                if (budgetEntries != null)
                {
                    return budgetEntries.ProjectedAsCollection<BudgetEntryDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<BudgetEntryDTO> FindBudgetEntries(Guid budgetId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BudgetEntrySpecifications.BudgetEntryWithBudgetId(budgetId);

                ISpecification<BudgetEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var budgetPagedCollection = _budgetEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (budgetPagedCollection != null)
                {
                    var persisted = _budgetRepository.Get(budgetId, serviceHeader);

                    var persistedEntriesTotal = persisted.BudgetEntries.Sum(x => x.Amount);

                    var pageCollection = budgetPagedCollection.PageCollection.ProjectedAsCollection<BudgetEntryDTO>();

                    var itemsCount = budgetPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BudgetEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                }
                else return null;
            }
        }

        public void FetchBudgetEntryBalances(List<BudgetEntryDTO> budgetEntries, ServiceHeader serviceHeader)
        {
            budgetEntries.ForEach(budgetEntry =>
            {
                switch ((BudgetEntryType)budgetEntry.Type)
                {
                    case BudgetEntryType.IncomeOrExpense:

                        if (budgetEntry.ChartOfAccountId != null && budgetEntry.ChartOfAccountId != Guid.Empty)
                        {
                            budgetEntry.ActualToDate = _sqlCommandAppService.FindGlAccountBalance(budgetEntry.BudgetBranchId.Value, budgetEntry.ChartOfAccountId.Value, budgetEntry.BudgetPostingPeriodId, DateTime.Today, (int)TransactionDateFilter.CreatedDate, serviceHeader);

                            budgetEntry.BudgetBalance = budgetEntry.Amount - (budgetEntry.ChartOfAccountAccountType == (int)ChartOfAccountType.Expense ? budgetEntry.ActualToDate * -1 : budgetEntry.ActualToDate);
                        }

                        break;
                    case BudgetEntryType.LoanProduct:

                        if (budgetEntry.LoanProductId != null && budgetEntry.LoanProductId != Guid.Empty)
                        {
                            budgetEntry.ActualToDate = _sqlCommandAppService.FindDisbursedLoanCasesValue(budgetEntry.LoanProductId.Value, budgetEntry.BudgetBranchId.Value, budgetEntry.BudgetPostingPeriodDurationStartDate, DateTime.Today, serviceHeader);

                            budgetEntry.BudgetBalance = budgetEntry.Amount - budgetEntry.ActualToDate;
                        }

                        break;
                    default:
                        break;
                }
            });
        }

        public decimal FetchBudgetBalance(Guid branchId, int type, Guid typeIdentifier, ServiceHeader serviceHeader)
        {
            var result = default(decimal);

            var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null)
            {
                var budgetDTO = FindBudget(postingPeriodDTO.Id, branchId, serviceHeader);

                if (budgetDTO != null)
                {
                    var budgetEntries = FindBudgetEntries(budgetDTO.Id, type, typeIdentifier, serviceHeader);

                    if (budgetEntries != null && budgetEntries.Any())
                    {
                        FetchBudgetEntryBalances(budgetEntries, serviceHeader);

                        result = budgetEntries.Sum(x => x.BudgetBalance);
                    }
                }
            }

            return result;
        }
    }
}
