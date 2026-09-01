using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/financial-statements")]
    public class FinancialStatementsController : ApiController
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;

        [HttpGet, Route("trial-balance")]
        public Task<IHttpActionResult> TrialBalance([FromUri] DateTime? endDate = null)
        {
            return GetSummary(endDate, 1, "Trial Balance");
        }

        [HttpGet, Route("income-expenditure")]
        public Task<IHttpActionResult> IncomeAndExpenditure([FromUri] DateTime? endDate = null)
        {
            return GetSummary(endDate, 2, "Income and Expenditure");
        }

        [HttpGet, Route("balance-sheet")]
        public Task<IHttpActionResult> BalanceSheet([FromUri] DateTime? endDate = null)
        {
            return GetSummary(endDate, 3, "Balance Sheet");
        }

        // Compatibility endpoint retaining the original sp_FinancialSummary type contract.
        [HttpGet, Route("summary")]
        public Task<IHttpActionResult> Summary([FromUri] DateTime? endDate = null, [FromUri] int type = 1)
        {
            if (type < 1 || type > 3)
                return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.BadRequest,
                    new { success = false, message = "type must be 1 (Trial Balance), 2 (Income and Expenditure), or 3 (Balance Sheet)." }));

            var names = new[] { "", "Trial Balance", "Income and Expenditure", "Balance Sheet" };
            return GetSummary(endDate, type, names[type]);
        }

        [HttpGet, Route("branch")]
        public async Task<IHttpActionResult> BranchStatement([FromUri] DateTime? endDate = null, [FromUri] Guid? branchId = null)
        {
            if (!endDate.HasValue || !branchId.HasValue || branchId.Value == Guid.Empty)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "endDate and branchId are required." });

            try
            {
                var rows = new List<BranchFinancialStatementRow>();
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(BranchStatementSql, connection))
                {
                    command.CommandTimeout = 120;
                    command.Parameters.Add("@ExclusiveEndDate", SqlDbType.DateTime).Value = endDate.Value.Date.AddDays(1);
                    command.Parameters.Add("@Branch", SqlDbType.UniqueIdentifier).Value = branchId.Value;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new BranchFinancialStatementRow
                            {
                                AccountType = ReadInt(reader, "AccountType"),
                                AccountTypeCode = ReadString(reader, "AccountTypeCode"),
                                ShortCode = ReadString(reader, "ShortCode"),
                                Code = ReadString(reader, "Code"),
                                Balance = ReadDecimal(reader, "Balance")
                            });
                        }
                    }
                }
                return Ok(new { success = true, message = "Branch financial statement retrieved successfully.", data = new { endDate = endDate.Value.Date, branchId, rows } });
            }
            catch (Exception) { throw; }
        }

        private async Task<IHttpActionResult> GetSummary(DateTime? endDate, int type, string statementName)
        {
            if (!endDate.HasValue)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "endDate is required." });

            try
            {
                var rows = new List<FinancialSummaryRow>();
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(FinancialSummarySql, connection))
                {
                    command.CommandTimeout = 120;
                    command.Parameters.Add("@ExclusiveEndDate", SqlDbType.DateTime).Value = endDate.Value.Date.AddDays(1);
                    command.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rows.Add(new FinancialSummaryRow
                            {
                                AccountCode = ReadString(reader, "AccountCode"),
                                AccountName = ReadString(reader, "AccountName"),
                                ParentCode = ReadString(reader, "ParentCode"),
                                ParentName = ReadString(reader, "ParentName"),
                                Debit = ReadDecimal(reader, "Debit"),
                                Credit = ReadDecimal(reader, "Credit"),
                                CostCenter = ReadString(reader, "CostCenter"),
                                Type = ReadString(reader, "Type"),
                                TypeName = ReadString(reader, "TypeName")
                            });
                        }
                    }
                }

                decimal totalDebit = 0, totalCredit = 0;
                foreach (var row in rows) { totalDebit += row.Debit; totalCredit += row.Credit; }
                return Ok(new { success = true, message = statementName + " retrieved successfully.", data = new { statementType = type, statementName, endDate = endDate.Value.Date, totalDebit, totalCredit, difference = totalDebit - totalCredit, rows } });
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Content(HttpStatusCode.Conflict, new { success = false, message = "Configure the Profit & Loss Appropriation system G/L account before generating the Balance Sheet." });
            }
            catch (Exception) { throw; }
        }

        private const string FinancialSummarySql = @"
IF @Type = 3 AND NOT EXISTS
(
    SELECT 1
    FROM dbo.swiftfin_SystemGeneralLedgerAccountMappings
    WHERE SystemGeneralLedgerAccountCode = '48831'
      AND ChartOfAccountId IS NOT NULL
)
    THROW 50001, 'Profit & Loss Appropriation system G/L account is not configured.', 1;

;WITH AccountBalances AS
(
    SELECT coa.Id,
           coa.AccountCode,
           coa.AccountName,
           coa.ParentId,
           coa.CostCenterId,
           coa.AccountType,
           COALESCE(SUM(je.Amount), 0) AS Balance
    FROM dbo.swiftfin_ChartOfAccounts coa
    LEFT JOIN dbo.swiftfin_JournalEntries je
      ON je.ChartOfAccountId = coa.Id
     AND COALESCE(je.ValueDate, je.CreatedDate) < @ExclusiveEndDate
    GROUP BY coa.Id, coa.AccountCode, coa.AccountName, coa.ParentId, coa.CostCenterId, coa.AccountType
),
StatementBalances AS
(
    SELECT ab.Id, ab.AccountCode, ab.AccountName, ab.ParentId, ab.CostCenterId, ab.AccountType, ab.Balance
    FROM AccountBalances ab
    WHERE @Type = 1
       OR (@Type = 2 AND ab.AccountType IN (4000, 5000))
       OR (@Type = 3 AND ab.AccountType NOT IN (4000, 5000))

    UNION ALL

    SELECT appropriation.Id,
           appropriation.AccountCode,
           appropriation.AccountName,
           appropriation.ParentId,
           appropriation.CostCenterId,
           appropriation.AccountType,
           COALESCE(SUM(je.Amount), 0) AS Balance
    FROM dbo.swiftfin_SystemGeneralLedgerAccountMappings mapping
    INNER JOIN dbo.swiftfin_ChartOfAccounts appropriation ON appropriation.Id = mapping.ChartOfAccountId
    LEFT JOIN dbo.swiftfin_JournalEntries je
      ON je.ChartOfAccountId IN
         (SELECT Id FROM dbo.swiftfin_ChartOfAccounts WHERE AccountType IN (4000, 5000))
     AND COALESCE(je.ValueDate, je.CreatedDate) < @ExclusiveEndDate
    WHERE @Type = 3
      AND mapping.SystemGeneralLedgerAccountCode = '48831'
    GROUP BY appropriation.Id, appropriation.AccountCode, appropriation.AccountName,
             appropriation.ParentId, appropriation.CostCenterId, appropriation.AccountType
)
SELECT sb.AccountCode,
       sb.AccountName,
       parent.AccountCode AS ParentCode,
       parent.AccountName AS ParentName,
       CASE WHEN SUM(sb.Balance) > 0 THEN SUM(sb.Balance) ELSE 0 END AS Debit,
       CASE WHEN SUM(sb.Balance) < 0 THEN -SUM(sb.Balance) ELSE 0 END AS Credit,
       COALESCE(costCenter.Description, 'Back Office') AS CostCenter,
       LEFT(LTRIM(RTRIM(CONVERT(varchar(32), sb.AccountCode))), 1) AS Type,
       CASE sb.AccountType
           WHEN 1000 THEN 'Assets'
           WHEN 2000 THEN 'Liabilities'
           WHEN 3000 THEN 'Equity'
           WHEN 4000 THEN 'Incomes'
           WHEN 5000 THEN 'Expenses'
       END AS TypeName
FROM StatementBalances sb
LEFT JOIN dbo.swiftfin_ChartOfAccounts parent ON parent.Id = sb.ParentId
LEFT JOIN dbo.swiftfin_CostCenters costCenter ON costCenter.Id = sb.CostCenterId
GROUP BY sb.Id, sb.AccountCode, sb.AccountName, parent.AccountCode, parent.AccountName,
         costCenter.Description, sb.AccountType
HAVING SUM(sb.Balance) <> 0
ORDER BY sb.AccountCode;";

        private const string BranchStatementSql = @"
SELECT coa.AccountType,
       CASE coa.AccountType
           WHEN 1000 THEN 'Assets'
           WHEN 2000 THEN 'Liabilities'
           WHEN 3000 THEN 'Equity'
           WHEN 4000 THEN 'Income'
           WHEN 5000 THEN 'Expenses'
       END AS AccountTypeCode,
       LEFT(CONVERT(varchar(32), coa.AccountCode), 3) AS ShortCode,
       SPACE(COALESCE(coa.Depth, 0) * 4) + LTRIM(RTRIM(CONVERT(varchar(32), coa.AccountCode))) + ' ' + LTRIM(RTRIM(coa.AccountName)) AS Code,
       COALESCE(SUM(je.Amount), 0) AS Balance
FROM dbo.swiftfin_ChartOfAccounts coa
LEFT JOIN dbo.swiftfin_JournalEntries je
  ON je.ChartOfAccountId = coa.Id
 AND COALESCE(je.ValueDate, je.CreatedDate) < @ExclusiveEndDate
 AND EXISTS
     (SELECT 1 FROM dbo.swiftfin_Journals journal WHERE journal.Id = je.JournalId AND journal.BranchId = @Branch)
GROUP BY coa.AccountType, coa.AccountCode, coa.AccountName, coa.Depth
ORDER BY coa.AccountType, coa.AccountCode;";

        private static string ReadString(SqlDataReader reader, string name) { var value = reader[name]; return value == DBNull.Value ? null : Convert.ToString(value); }
        private static decimal ReadDecimal(SqlDataReader reader, string name) { var value = reader[name]; return value == DBNull.Value ? 0m : Convert.ToDecimal(value); }
        private static int ReadInt(SqlDataReader reader, string name) { var value = reader[name]; return value == DBNull.Value ? 0 : Convert.ToInt32(value); }

        private sealed class FinancialSummaryRow
        {
            public string AccountCode { get; set; }
            public string AccountName { get; set; }
            public string ParentCode { get; set; }
            public string ParentName { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public string CostCenter { get; set; }
            public string Type { get; set; }
            public string TypeName { get; set; }
        }

        private sealed class BranchFinancialStatementRow
        {
            public int AccountType { get; set; }
            public string AccountTypeCode { get; set; }
            public string ShortCode { get; set; }
            public string Code { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
