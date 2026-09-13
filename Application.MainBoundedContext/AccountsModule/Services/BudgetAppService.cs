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

        public BudgetActualsDTO FindBudgetActuals(Guid budgetId, DateTime asAt, ServiceHeader header)
        {
            using (_dbContextScopeFactory.CreateReadOnlyWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var budget = FindBudget(budgetId, header);
                if (budget == null) throw new BudgetValidationException("Select an existing budget.", "BudgetId", 404);
                if (asAt.Year < 1753 || asAt.Date == DateTime.MaxValue.Date || asAt.Date < budget.PostingPeriodDurationStartDate.Date || asAt.Date > budget.PostingPeriodDurationEndDate.Date)
                    throw new BudgetValidationException("Choose an as-at date within the budget's posting period.", "AsAt");
                var entries = FindBudgetEntries(budgetId, header) ?? new List<BudgetEntryDTO>();
                var parameters = new object[] {
                    new System.Data.SqlClient.SqlParameter("@Branch", budget.BranchId.Value),
                    new System.Data.SqlClient.SqlParameter("@Period", budget.PostingPeriodId),
                    new System.Data.SqlClient.SqlParameter("@Start", budget.PostingPeriodDurationStartDate.Date),
                    new System.Data.SqlClient.SqlParameter("@End", asAt.Date.AddDays(1)) };
                var actuals = _budgetRepository.DatabaseSqlQuery<BudgetActualLineDTO>(@"
SELECT a.Id TargetId, CASE WHEN a.AccountType=4000 THEN 'Income' ELSE 'Expenses' END Section,
CAST(a.AccountCode AS varchar(20)) Code, a.AccountName Description, CAST(0 AS decimal(18,2)) Budget,
SUM(CASE WHEN a.AccountType=4000 THEN -e.Amount ELSE e.Amount END) Actual
FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN dbo.swiftFin_ChartOfAccounts a ON a.Id=e.ChartOfAccountId
WHERE j.BranchId=@Branch AND j.PostingPeriodId=@Period AND a.AccountType IN (4000,5000)
AND COALESCE(j.ValueDate,j.CreatedDate)>=@Start AND COALESCE(j.ValueDate,j.CreatedDate)<@End
GROUP BY a.Id,a.AccountType,a.AccountCode,a.AccountName HAVING SUM(e.Amount)<>0", header, parameters).ToList();
                actuals.AddRange(_budgetRepository.DatabaseSqlQuery<BudgetActualLineDTO>(@"
SELECT p.Id TargetId, 'Loan disbursements' Section, CAST('' AS varchar(20)) Code,
p.Description Description, CAST(0 AS decimal(18,2)) Budget, SUM(l.DisbursedAmount) Actual
FROM dbo.swiftFin_LoanCases l JOIN dbo.swiftFin_LoanProducts p ON p.Id=l.LoanProductId
WHERE l.BranchId=@Branch AND l.DisbursedDate>=@Start AND l.DisbursedDate<@End
GROUP BY p.Id,p.Description HAVING SUM(l.DisbursedAmount)<>0", header,
                    new System.Data.SqlClient.SqlParameter("@Branch", budget.BranchId.Value),
                    new System.Data.SqlClient.SqlParameter("@Start", budget.PostingPeriodDurationStartDate.Date),
                    new System.Data.SqlClient.SqlParameter("@End", asAt.Date.AddDays(1))));
                return BuildBudgetActuals(budget, asAt.Date, entries, actuals);
            }
        }

        internal static BudgetActualsDTO BuildBudgetActuals(BudgetDTO budget, DateTime asAt, List<BudgetEntryDTO> entries, List<BudgetActualLineDTO> actuals)
        {
            var lines = new Dictionary<string, BudgetActualLineDTO>();
            foreach (var entry in entries)
            {
                var section = entry.Type == 1 ? "Loan disbursements" : entry.ChartOfAccountAccountType == 4000 ? "Income" : entry.ChartOfAccountAccountType == 5000 ? "Expenses" : null;
                var target = entry.Type == 1 ? entry.LoanProductId : entry.ChartOfAccountId;
                if (section == null || !target.HasValue || target == Guid.Empty)
                    throw new BudgetValidationException("This budget contains an invalid allocation. Correct its account or loan product in Budget Appropriation.", "Entries");
                var key = section + target.Value;
                if (!lines.ContainsKey(key)) lines.Add(key, new BudgetActualLineDTO {
                    TargetId = target.Value, Section = section,
                    Code = entry.Type == 1 ? "" : entry.ChartOfAccountAccountCode.ToString(),
                    Description = entry.Type == 1 ? entry.LoanProductDescription : entry.ChartOfAccountAccountName });
                lines[key].Budget += entry.Amount;
            }
            foreach (var actual in actuals)
            {
                var key = actual.Section + actual.TargetId;
                if (!lines.ContainsKey(key)) lines.Add(key, actual);
                else lines[key].Actual += actual.Actual;
            }
            var sections = new[] { "Income", "Expenses", "Loan disbursements" };
            return new BudgetActualsDTO {
                Budget = budget, AsAt = asAt,
                Lines = lines.Values.OrderBy(l => Array.IndexOf(sections, l.Section)).ThenBy(l => l.Code).ThenBy(l => l.Description).ToList(),
                Totals = sections.Select(section => new BudgetActualTotalDTO {
                    Section = section, Budget = lines.Values.Where(l => l.Section == section).Sum(l => l.Budget),
                    Actual = lines.Values.Where(l => l.Section == section).Sum(l => l.Actual) }).ToList()
            };
        }

        public BudgetDTO SaveBudget(BudgetDTO model, List<BudgetEntryDTO> entries, ServiceHeader header)
        {
            ValidateAppropriation(model, entries);
            using (var scope = _dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var persisted = model.Id == Guid.Empty ? null : _budgetRepository.Get(model.Id, header);
                if (model.Id != Guid.Empty && persisted == null)
                    throw new BudgetValidationException("The selected budget no longer exists. Refresh the budget list.", "Budget.Id", 404);
                var duplicates = _budgetRepository.AllMatching(BudgetSpecifications.BudgetWithPostingPeriodIdAndBranchId(model.PostingPeriodId, model.BranchId.Value), header);
                if (duplicates != null && duplicates.Any(b => b.Id != model.Id))
                    throw new BudgetValidationException("A budget already exists for this branch and posting period. Select it under Existing Budget to edit its allocations.", "Budget.PostingPeriodId", 409);
                RequireBudgetTarget("SELECT COUNT(*) FROM dbo.swiftFin_Branches WHERE Id=@Id", model.BranchId.Value, "Budget.BranchId", "Select an existing branch.", header);
                RequireBudgetTarget("SELECT COUNT(*) FROM dbo.swiftFin_PostingPeriods WHERE Id=@Id", model.PostingPeriodId, "Budget.PostingPeriodId", "Select an existing posting period.", header);
                for (var i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    if (e.Type == (int)BudgetEntryType.IncomeOrExpense)
                        RequireBudgetTarget("SELECT COUNT(*) FROM dbo.swiftFin_ChartOfAccounts a WHERE a.Id=@Id AND a.AccountType IN (4000,5000) AND NOT EXISTS (SELECT 1 FROM dbo.swiftFin_ChartOfAccounts c WHERE c.ParentId=a.Id)", e.ChartOfAccountId.Value, "Entries[" + i + "].ChartOfAccountId", "Line " + (i + 1) + ": select an income or expense posting account, not a parent account.", header);
                    else
                        RequireBudgetTarget("SELECT COUNT(*) FROM dbo.swiftFin_LoanProducts WHERE Id=@Id", e.LoanProductId.Value, "Entries[" + i + "].LoanProductId", "Line " + (i + 1) + ": select an existing loan product.", header);
                }
                var current = BudgetFactory.CreateBudget(model.PostingPeriodId, model.BranchId.Value, model.Description.Trim(), model.TotalValue);
                if (persisted == null)
                {
                    current.CreatedBy = header.ApplicationUserName;
                    _budgetRepository.Add(current, header);
                }
                else
                {
                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    _budgetRepository.Merge(persisted, current, header);
                    var previous = _budgetEntryRepository.AllMatching(BudgetEntrySpecifications.BudgetEntryWithBudgetId(persisted.Id), header);
                    if (previous != null) foreach (var e in previous) _budgetEntryRepository.Remove(e, header);
                }
                foreach (var e in entries)
                {
                    var line = BudgetEntryFactory.CreateBudgetEntry(current.Id, e.Type, e.ChartOfAccountId, e.LoanProductId, e.Amount, e.Reference);
                    line.CreatedBy = header.ApplicationUserName;
                    _budgetEntryRepository.Add(line, header);
                }
                scope.SaveChanges(header);
                return current.ProjectedAs<BudgetDTO>();
            }
        }

        private void RequireBudgetTarget(string sql, Guid id, string field, string message, ServiceHeader header)
        {
            if (_budgetRepository.DatabaseSqlQuery<int>(sql, header, new System.Data.SqlClient.SqlParameter("@Id", id)).Single() != 1)
                throw new BudgetValidationException(message, field);
        }

        internal static void ValidateAppropriation(BudgetDTO model, List<BudgetEntryDTO> entries)
        {
            if (model == null) throw new BudgetValidationException("Complete the budget header.", "Budget");
            if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Trim().Length > 256)
                throw new BudgetValidationException("Enter a budget name of 1–256 characters.", "Budget.Description");
            if (!model.BranchId.HasValue || model.BranchId == Guid.Empty) throw new BudgetValidationException("Select a branch.", "Budget.BranchId");
            if (model.PostingPeriodId == Guid.Empty) throw new BudgetValidationException("Select a posting period.", "Budget.PostingPeriodId");
            if (!ValidBudgetAmount(model.TotalValue)) throw new BudgetValidationException("Enter a positive total with no more than two decimal places (maximum 9,999,999,999,999.99).", "Budget.TotalValue");
            if (entries == null || entries.Count == 0) throw new BudgetValidationException("Add at least one allocation before saving.", "Entries");
            for (var i = 0; i < entries.Count; i++)
            {
                var e = entries[i]; var key = "Entries[" + i + "]"; var label = "Line " + (i + 1) + ": ";
                if (e == null) throw new BudgetValidationException(label + "allocation is missing.", key);
                if (e.Type != 0 && e.Type != 1) throw new BudgetValidationException(label + "choose Income / Expense or Loan Product.", key + ".Type");
                if (!ValidBudgetAmount(e.Amount)) throw new BudgetValidationException(label + "enter a positive amount with no more than two decimal places (maximum 9,999,999,999,999.99).", key + ".Amount");
                if ((e.Reference ?? "").Length > 256) throw new BudgetValidationException(label + "reference must not exceed 256 characters.", key + ".Reference");
                if (e.Type == 0 && (!e.ChartOfAccountId.HasValue || e.ChartOfAccountId == Guid.Empty || (e.LoanProductId.HasValue && e.LoanProductId != Guid.Empty)))
                    throw new BudgetValidationException(label + "select a G/L account only.", key + ".ChartOfAccountId");
                if (e.Type == 1 && (!e.LoanProductId.HasValue || e.LoanProductId == Guid.Empty || (e.ChartOfAccountId.HasValue && e.ChartOfAccountId != Guid.Empty)))
                    throw new BudgetValidationException(label + "select a loan product only.", key + ".LoanProductId");
            }
            var allocated = entries.Sum(e => e.Amount);
            if (allocated != model.TotalValue)
                throw new BudgetValidationException(string.Format("Allocations total {0:N2}; budget total is {1:N2}. {2} {3:N2} to balance the budget.", allocated, model.TotalValue, allocated < model.TotalValue ? "Allocate another" : "Reduce allocations by", Math.Abs(model.TotalValue - allocated)), "Entries");
        }

        private static bool ValidBudgetAmount(decimal value) { return value > 0 && value <= 9999999999999.99m && decimal.Round(value, 2) == value; }


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
            if (budgetId == Guid.Empty || budgetEntries == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _budgetRepository.GetAsync(budgetId, serviceHeader);

                if (persisted != null)
                {
                    if (budgetEntries.Sum(x => x.Amount) != persisted.TotalValue)
                        return false;

                    var currentEntries = _budgetEntryRepository.AllMatching(
                        BudgetEntrySpecifications.BudgetEntryWithBudgetId(budgetId), serviceHeader);
                    if (currentEntries != null)
                    {
                        foreach (var currentEntry in currentEntries)
                            _budgetEntryRepository.Remove(currentEntry, serviceHeader);
                    }

                    foreach (var item in budgetEntries)
                    {
                        var budgetEntry = BudgetEntryFactory.CreateBudgetEntry(
                            persisted.Id, item.Type, item.ChartOfAccountId,
                            item.LoanProductId, item.Amount, item.Reference);
                        budgetEntry.CreatedBy = serviceHeader.ApplicationUserName;
                        _budgetEntryRepository.Add(budgetEntry, serviceHeader);
                    }

                    return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                }
                return false;
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

                            // Ledger debits are positive; income credits are negative.
                            // Preserve reversal signs so refunds restore the expense allowance.
                            var actualUsage = budgetEntry.ChartOfAccountAccountType == (int)ChartOfAccountType.Income
                                ? -budgetEntry.ActualToDate
                                : budgetEntry.ActualToDate;
                            budgetEntry.BudgetBalance = budgetEntry.Amount - actualUsage;
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
