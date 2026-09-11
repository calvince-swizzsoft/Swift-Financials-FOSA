using Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg;
using Domain.Seedwork.Specification;
using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class CashFlowAppService : ICashFlowAppService
    {
        private readonly IDbContextScopeFactory _scopes;
        private readonly IRepository<ChartOfAccount> _accounts;
        private readonly IRepository<CashFlowMapping> _mappings;

        public CashFlowAppService(IDbContextScopeFactory scopes, IRepository<ChartOfAccount> accounts,
            IRepository<CashFlowMapping> mappings)
        {
            _scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
            _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            _mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
        }

        public async Task<List<CashFlowMappingDTO>> GetMappings(ServiceHeader header)
        {
            using (_scopes.CreateReadOnly())
            {
                var mappings = await _mappings.AllMatchingAsync(CashFlowMappingSpecifications.All(), header, mapping => mapping.ChartOfAccount);
                return mappings.OrderBy(mapping => mapping.Section).ThenBy(mapping => mapping.ChartOfAccount.AccountCode)
                    .Select(mapping => new CashFlowMappingDTO
                    {
                        ChartOfAccountId = mapping.ChartOfAccountId,
                        AccountCode = mapping.ChartOfAccount.AccountCode.ToString(),
                        AccountName = mapping.ChartOfAccount.AccountName,
                        Section = mapping.Section,
                        Line = mapping.Line
                    }).ToList();
            }
        }

        public async Task SaveMapping(CashFlowMappingDTO mapping, ServiceHeader header)
        {
            if (mapping == null || mapping.ChartOfAccountId == Guid.Empty)
                throw new CashFlowValidationException("Select a G/L account.");
            using (var scope = _scopes.CreateWithTransaction(IsolationLevel.Serializable))
            {
                var account = await _accounts.GetAsync(mapping.ChartOfAccountId, header);
                var children = await _accounts.AllMatchingCountAsync(
                    new DirectSpecification<ChartOfAccount>(child => child.ParentId == mapping.ChartOfAccountId), header);
                if (account == null || children != 0 || (mapping.Section == "Cash" && account.AccountType != 1000))
                    throw new CashFlowValidationException("Select a posting account without child accounts. Cash accounts must be asset accounts.");
                var persisted = (await _mappings.AllMatchingAsync(CashFlowMappingSpecifications.WithAccount(mapping.ChartOfAccountId), header)).SingleOrDefault();
                try
                {
                    if (persisted == null)
                    {
                        persisted = CashFlowMappingFactory.CreateCashFlowMapping(mapping.ChartOfAccountId, mapping.Section, mapping.Line, header.ApplicationUserName);
                        _mappings.Add(persisted, header);
                    }
                    else persisted.UpdateClassification(mapping.Section, mapping.Line, header.ApplicationUserName);
                }
                catch (ArgumentException ex) { throw new CashFlowValidationException(ex.Message); }
                await scope.SaveChangesAsync(header);
            }
        }

        public async Task RemoveMapping(Guid accountId, ServiceHeader header)
        {
            using (var scope = _scopes.Create())
            {
                var mapping = (await _mappings.AllMatchingAsync(CashFlowMappingSpecifications.WithAccount(accountId), header)).SingleOrDefault();
                if (mapping == null) return;
                _mappings.Remove(mapping, header);
                await scope.SaveChangesAsync(header);
            }
        }

        private async Task<List<T>> Query<T>(DateTime start, DateTime end, Guid? branch, string tail, ServiceHeader header, params object[] extra)
        {
            if (start.Date > end.Date || start.Year < 1753 || end.Date == DateTime.MaxValue.Date || branch == Guid.Empty)
                throw new CashFlowValidationException("Choose a valid date range and branch.");
            using (_scopes.CreateReadOnlyWithTransaction(IsolationLevel.Serializable))
            {
                var count = await _mappings.AllMatchingCountAsync(CashFlowMappingSpecifications.CashAccounts(), header);
                if (count == 0) throw new CashFlowValidationException("Configure at least one cash or cash-equivalent account before generating Cash Flow.");
                if (branch.HasValue)
                {
                    var branches = await _accounts.DatabaseSqlQueryAsync<int>("SELECT COUNT(*) FROM dbo.swiftFin_Branches WHERE Id=@Id", header, new SqlParameter("@Id", branch.Value));
                    if (branches.Single() != 1) throw new CashFlowValidationException("The selected branch does not exist.");
                }
                var args = new List<object> { new SqlParameter("@Start", start.Date), new SqlParameter("@End", end.Date.AddDays(1)), new SqlParameter("@Branch", SqlDbType.UniqueIdentifier) { Value = (object)branch ?? DBNull.Value } };
                args.AddRange(extra);
                var rows = await _accounts.DatabaseSqlQueryAsync<T>(CashFlowSql.Prepare + tail + @"
DROP TABLE #details; DROP TABLE #flows; DROP TABLE #journals; DROP TABLE #legs; DROP TABLE #cash; DROP TABLE #balances; DROP TABLE #mapping;", header, args.ToArray());
                return rows;
            }
        }

        public async Task<CashFlowReportDTO> Generate(DateTime startDate, DateTime endDate, Guid? branchId, ServiceHeader header)
        {
            var rows = await Query<CashFlowRowDTO>(startDate,endDate,branchId,@"
SELECT Section,Line,SUM(Receipts) Receipts,SUM(Payments) Payments,COUNT(*) DetailCount FROM #details GROUP BY Section,Line
UNION ALL SELECT 'Balance','Opening',OpeningCash,CAST(0 AS DECIMAL(18,2)),0 FROM #balances
UNION ALL SELECT 'Balance','Closing',ClosingCash,CAST(0 AS DECIMAL(18,2)),0 FROM #balances;", header);
            return BuildReport(startDate, endDate, branchId, rows);
        }

        public static CashFlowReportDTO BuildReport(DateTime start, DateTime end, Guid? branch, List<CashFlowRowDTO> rows)
        {
            var opening = rows.Single(r => r.Section == "Balance" && r.Line == "Opening").Net;
            var closing = rows.Single(r => r.Section == "Balance" && r.Line == "Closing").Net;
            var flows = rows.Where(r => r.Section != "Balance").OrderBy(r => Array.IndexOf(new[] { "Operating", "Investing", "Financing", "Exchange", "Review", "Internal" }, r.Section)).ThenBy(r => r.Line).ToList();
            var net = flows.Where(r => new[] { "Operating", "Investing", "Financing" }.Contains(r.Section)).Sum(r => r.Net);
            var exchange = flows.Where(r => r.Section == "Exchange").Sum(r => r.Net);
            var review = flows.Where(r => r.Section == "Review").Sum(r => r.Net);
            var difference = closing - opening - net - exchange - review;
            return new CashFlowReportDTO { StartDate=start.Date,EndDate=end.Date,BranchId=branch,OpeningCash=opening,ClosingCash=closing,
                NetCashFlow=net,ExchangeEffects=exchange,UnclassifiedNet=review,ReconciliationDifference=difference,
                IsComplete=!flows.Any(r => r.Section == "Review") && difference == 0,Rows=flows };
        }

        public Task<List<CashFlowDetailDTO>> GetDetails(DateTime startDate, DateTime endDate, Guid? branchId, string section, string line, int pageIndex, int pageSize, ServiceHeader header)
        {
            if (pageIndex<0 || pageIndex>1000000 || pageSize<1 || pageSize>100 || string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(line) || line.Length>240)
                throw new CashFlowValidationException("Choose a report line and a valid page (1–100 entries).");
            return Query<CashFlowDetailDTO>(startDate,endDate,branchId,@"
SELECT d.JournalId,d.ValueDate,j.Reference,j.PrimaryDescription Narration,d.Section,d.Line,d.Receipts,d.Payments,0 DetailCount,COUNT(*) OVER() TotalCount
FROM #details d JOIN dbo.swiftFin_Journals j ON j.Id=d.JournalId
WHERE d.Section=@Section AND d.Line=@Line ORDER BY d.ValueDate,d.JournalId
OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;",header,
                new SqlParameter("@Section",section),new SqlParameter("@Line",line),new SqlParameter("@Skip",pageIndex*pageSize),new SqlParameter("@Take",pageSize));
        }
    }
}
