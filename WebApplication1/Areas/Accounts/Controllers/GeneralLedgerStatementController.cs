using Application.MainBoundedContext.AccountsModule.Services;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/statements/gl-account")]
    public class GeneralLedgerStatementController : ApiController
    {
        private readonly IJournalEntryAppService _journalEntryAppService;

        public GeneralLedgerStatementController(IJournalEntryAppService journalEntryAppService)
        {
            _journalEntryAppService = journalEntryAppService ?? throw new ArgumentNullException(nameof(journalEntryAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        // Ledger statement for one G/L account (chart of accounts), by date range and free-text filter.
        [HttpGet, Route("{chartOfAccountId:guid}")]
        public IHttpActionResult GetStatement(
            Guid chartOfAccountId,
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int journalEntryFilter = 0,
            [FromUri] int transactionDateFilter = 2,
            [FromUri] bool tallyDebitsCredits = true)
        {
            try
            {
                var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
                var effectiveEndDate = endDate ?? DateTime.Today;

                if (effectiveStartDate > effectiveEndDate)
                    return ErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be after endDate");

                var serviceHeader = Utils.CreateServiceHeader();

                var statement = _journalEntryAppService.FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeInPage(
                    pageIndex, pageSize, chartOfAccountId, effectiveStartDate, effectiveEndDate, text, journalEntryFilter, transactionDateFilter, tallyDebitsCredits, serviceHeader);

                return ApiResponse(true, "G/L statement retrieved successfully", statement);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Same ledger statement, narrowed to a specific transaction code + reference instead of free text —
        // e.g. "show me every Cash Deposit posting tied to reference X".
        [HttpGet, Route("{chartOfAccountId:guid}/by-transaction-code")]
        public IHttpActionResult GetStatementByTransactionCode(
            Guid chartOfAccountId,
            [FromUri] int transactionCode,
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] string reference = "",
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] int transactionDateFilter = 2,
            [FromUri] bool tallyDebitsCredits = true)
        {
            try
            {
                var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
                var effectiveEndDate = endDate ?? DateTime.Today;

                if (effectiveStartDate > effectiveEndDate)
                    return ErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be after endDate");

                var serviceHeader = Utils.CreateServiceHeader();

                var statement = _journalEntryAppService.FindGeneralLedgerTransactionsByChartOfAccountIdAndDateRangeAndTransactionCodeAndReferenceInPage(
                    pageIndex, pageSize, chartOfAccountId, effectiveStartDate, effectiveEndDate, transactionCode, reference, transactionDateFilter, tallyDebitsCredits, serviceHeader);

                return ApiResponse(true, "G/L statement retrieved successfully", statement);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Unscoped browse across every G/L posting in a date range — the back-office audit / "all transactions" view.
        [HttpGet, Route("")]
        public IHttpActionResult GetAllTransactions(
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int journalEntryFilter = 0)
        {
            try
            {
                var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
                var effectiveEndDate = endDate ?? DateTime.Today;

                if (effectiveStartDate > effectiveEndDate)
                    return ErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be after endDate");

                var serviceHeader = Utils.CreateServiceHeader();

                var transactions = _journalEntryAppService.FindGeneralLedgerTransactionsInPage(
                    pageIndex, pageSize, effectiveStartDate, effectiveEndDate, text, journalEntryFilter, serviceHeader);

                return ApiResponse(true, "G/L transactions retrieved successfully", transactions);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
