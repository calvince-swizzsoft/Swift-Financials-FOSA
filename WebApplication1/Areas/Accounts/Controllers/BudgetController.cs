using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;
using WebApplication1.ApiErrors;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize, RoutePrefix("api/accounts/budgets")]
    public class BudgetController : ApiController
    {
        private readonly IBudgetAppService _appService;
        public BudgetController(IBudgetAppService appService) { _appService = appService ?? throw new ArgumentNullException(nameof(appService)); }

        private IHttpActionResult Error(HttpStatusCode status, string code, string message, IDictionary<string, string[]> fields = null)
        { return ResponseMessage(ApiErrorResponses.Create(Request, status, code, message, fields)); }

        private IHttpActionResult Execute(Func<IHttpActionResult> action)
        {
            if (!ModelState.IsValid)
            {
                var fields = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key,
                    x => x.Value.Errors.Select(e => BudgetFieldError(x.Key, e)).Distinct().ToArray());
                System.Diagnostics.Trace.TraceWarning("Budget validation rejected. Reference={0}; Fields={1}", CorrelationIdHandler.GetCorrelationId(Request), string.Join(", ", fields.Keys));
                return Error(HttpStatusCode.BadRequest, "VALIDATION_FAILED", string.Join(" ", fields.Values.SelectMany(x => x).Distinct()), fields);
            }
            try { return action(); }
            catch (BudgetValidationException ex)
            { return Error((HttpStatusCode)ex.StatusCode, ex.StatusCode == 409 ? "BUDGET_ALREADY_EXISTS" : ex.StatusCode == 404 ? "NOT_FOUND" : "VALIDATION_FAILED", ex.Message, new Dictionary<string, string[]> { { ex.Field, new[] { ex.Message } } }); }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Budget operation failed: {0}", ex);
                var sql = ex;
                while (sql != null && !(sql is SqlException)) sql = sql.InnerException;
                var dbError = sql as SqlException;
                if (dbError != null && dbError.Number == 2812)
                    return Error(HttpStatusCode.ServiceUnavailable, "BUDGET_BALANCES_UNAVAILABLE", "Actual budget balances are unavailable because database setup is incomplete. Ask your administrator to install the budget balance procedure. You can still create or edit allocations in Budget Appropriation.");
                if (dbError != null && (dbError.Number == 1205 || dbError.Number == 2601 || dbError.Number == 2627))
                    return Error(HttpStatusCode.Conflict, "BUDGET_CONFLICT", "Another budget operation conflicted with this request. Refresh the budget list and try again.");
                return Error(HttpStatusCode.InternalServerError, "BUDGET_OPERATION_FAILED", "The budget operation could not be completed. Your form has been kept. Refresh the budget list to check its saved state before retrying; contact support if this continues.");
            }
        }

        internal static string BudgetFieldError(string path, System.Web.Http.ModelBinding.ModelError error)
        {
            var field = (path ?? "").Split('.').Last();
            var line = System.Text.RegularExpressions.Regex.Match(path ?? "", @"Entries\[(\d+)\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            int index;
            var prefix = line.Success && int.TryParse(line.Groups[1].Value, out index) && index < int.MaxValue ? "Line " + (index + 1) + ": " : "";
            string message;
            switch (field)
            {
                case "ChartOfAccountId": message = "Select a valid G/L account; leave it empty for a loan-product allocation."; break;
                case "LoanProductId": message = "Select a valid loan product; leave it empty for a G/L allocation."; break;
                case "BranchId": message = "Select a valid branch."; break;
                case "PostingPeriodId": message = "Select a valid posting period."; break;
                case "Amount": message = "Enter a positive allocation amount with at most two decimal places."; break;
                case "TotalValue": message = "Enter a positive budget total with at most two decimal places."; break;
                case "Description": message = "Enter a budget name."; break;
                case "Type": message = "Choose Income / Expense or Loan Product."; break;
                case "Id": message = "The budget selection is invalid. Refresh the list and select the budget again, or choose New budget."; break;
                case "asAt": message = "Select a valid as-at date within the posting period."; break;
                case "pageIndex": message = "Enter a whole-number page index starting at zero."; break;
                case "pageSize": message = "Enter a whole-number page size from 1 to 100."; break;
                case "Reference": message = "Enter a reference of no more than 256 characters."; break;
                default: message = "The budget request could not be read. Refresh the page and try again."; break;
            }
            // Do not echo JSON conversion exceptions, which can contain submitted values.
            return prefix + message;
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            // Web API's string binder marks ?text= as a missing required value.
            // Empty search is valid; preserve all other binding/validation errors.
            if (string.IsNullOrWhiteSpace(text)) ModelState.Remove("text.String");
            return Execute(() =>
            {
                if (pageIndex < 0 || pageSize < 1 || pageSize > 100) return Error(HttpStatusCode.BadRequest, "VALIDATION_FAILED", "Page index must be nonnegative and page size must be between 1 and 100.");
                var header = Utils.CreateServiceHeader();
                var data = string.IsNullOrWhiteSpace(text) ? _appService.FindBudgets(pageIndex, pageSize, header) : _appService.FindBudgets(text.Trim(), pageIndex, pageSize, header);
                return Ok(new { success = true, message = "", data });
            });
        }

        [HttpGet, Route("{id:guid}/actuals")]
        public IHttpActionResult Actuals(Guid id, DateTime asAt) => Execute(() => Ok(new {
            success = true, message = "", data = _appService.FindBudgetActuals(id, asAt, Utils.CreateServiceHeader()) }));

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll() => Execute(() => Ok(new { success = true, message = "", data = _appService.FindBudgets(Utils.CreateServiceHeader()) ?? new List<BudgetDTO>() }));

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetOne(Guid id) => Execute(() =>
        {
            var data = _appService.FindBudget(id, Utils.CreateServiceHeader());
            return data == null ? Error(HttpStatusCode.NotFound, "NOT_FOUND", "This budget no longer exists. Refresh the budget list.") : (IHttpActionResult)Ok(new { success = true, message = "", data });
        });

        [HttpPost, Route("")]
        public IHttpActionResult Create(SaveBudgetRequest request) => Execute(() =>
        {
            if (request?.Budget == null) return Error(HttpStatusCode.BadRequest, "VALIDATION_FAILED", "A budget header and allocation lines are required.");
            request.Budget.Id = Guid.Empty;
            var data = _appService.SaveBudget(request.Budget, request.Entries, Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "Budget and allocations created successfully.", data });
        });

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, SaveBudgetRequest request) => Execute(() =>
        {
            if (request?.Budget == null || id == Guid.Empty) return Error(HttpStatusCode.BadRequest, "VALIDATION_FAILED", "Select a budget and complete its header and allocations.");
            request.Budget.Id = id;
            var data = _appService.SaveBudget(request.Budget, request.Entries, Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "Budget and allocations updated successfully.", data });
        });

        [HttpGet, Route("{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, bool includeBalances = true) => Execute(() =>
        {
            var header = Utils.CreateServiceHeader();
            if (_appService.FindBudget(id, header) == null) return Error(HttpStatusCode.NotFound, "NOT_FOUND", "This budget no longer exists. Refresh the budget list.");
            var data = _appService.FindBudgetEntries(id, header) ?? new List<BudgetEntryDTO>();
            if (includeBalances) _appService.FetchBudgetEntryBalances(data, header);
            return Ok(new { success = true, message = "", data });
        });
    }
    public class SaveBudgetRequest
    {
        public BudgetDTO Budget { get; set; }
        public List<BudgetEntryDTO> Entries { get; set; }
    }
}
