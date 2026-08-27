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
                using (var command = new SqlCommand("dbo.sp_FinancialStatementBranch", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120;
                    command.Parameters.Add("@Enddate", SqlDbType.DateTime).Value = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    command.Parameters.Add("@Branch", SqlDbType.VarChar, 36).Value = branchId.Value.ToString();
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
            catch (SqlException ex) when (ex.Number == 2812)
            {
                return Content(HttpStatusCode.NotImplemented, new { success = false, message = "dbo.sp_FinancialStatementBranch is not installed in this database. Apply the faithful legacy reporting procedures first." });
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
                using (var command = new SqlCommand("dbo.sp_FinancialSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate.Value.Date;
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
            catch (SqlException ex) when (ex.Number == 2812)
            {
                return Content(HttpStatusCode.NotImplemented, new { success = false, message = "dbo.sp_FinancialSummary is not installed in this database. Apply the faithful legacy reporting procedure first." });
            }
            catch (Exception) { throw; }
        }

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
