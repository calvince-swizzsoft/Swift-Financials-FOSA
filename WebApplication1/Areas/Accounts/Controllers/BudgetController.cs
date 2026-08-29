using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/budgets")]
    public class BudgetController : ApiController
    {
        private readonly IBudgetAppService _appService;
        public BudgetController(IBudgetAppService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var header = Utils.CreateServiceHeader();
            var data = string.IsNullOrWhiteSpace(text)
                ? _appService.FindBudgets(pageIndex, pageSize, header)
                : _appService.FindBudgets(text, pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data });
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            var data = _appService.FindBudgets(Utils.CreateServiceHeader()) ?? new List<BudgetDTO>();
            return Ok(new { success = true, message = "", data });
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetOne(Guid id)
        {
            var data = _appService.FindBudget(id, Utils.CreateServiceHeader());
            return data == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data });
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(SaveBudgetRequest request)
        {
            var error = ValidateRequest(request);
            if (error != null) return Content(HttpStatusCode.BadRequest, new { success = false, message = error });
            var header = Utils.CreateServiceHeader();
            var created = _appService.AddNewBudget(request.Budget, header);
            if (created == null || !string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                return Content(HttpStatusCode.Conflict, new { success = false, message = created?.ErrorMessageResult ?? "The budget could not be created." });
            foreach (var entry in request.Entries) entry.BudgetId = created.Id;
            if (!await _appService.UpdateBudgetEntriesAsync(created.Id, request.Entries, header))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Budget entries must total the budget value." });
            return Ok(new { success = true, message = "Budget created successfully.", data = created });
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, SaveBudgetRequest request)
        {
            var error = ValidateRequest(request);
            if (error != null) return Content(HttpStatusCode.BadRequest, new { success = false, message = error });
            var header = Utils.CreateServiceHeader();
            if (_appService.FindBudget(id, header) == null) return NotFound();
            request.Budget.Id = id;
            if (!_appService.UpdateBudget(request.Budget, header))
                return Content(HttpStatusCode.Conflict, new { success = false, message = request.Budget.ErrorMessageResult ?? "The budget could not be updated." });
            foreach (var entry in request.Entries) entry.BudgetId = id;
            if (!await _appService.UpdateBudgetEntriesAsync(id, request.Entries, header))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Budget entries must total the budget value." });
            return Ok(new { success = true, message = "Budget updated successfully.", data = _appService.FindBudget(id, header) });
        }

        [HttpGet, Route("{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, bool includeBalances = true)
        {
            var header = Utils.CreateServiceHeader();
            if (_appService.FindBudget(id, header) == null) return NotFound();
            var data = _appService.FindBudgetEntries(id, header) ?? new List<BudgetEntryDTO>();
            if (includeBalances) _appService.FetchBudgetEntryBalances(data, header);
            return Ok(new { success = true, message = "", data });
        }

        private static string ValidateRequest(SaveBudgetRequest request)
        {
            if (request?.Budget == null || request.Entries == null || !request.Entries.Any())
                return "A budget and at least one appropriation entry are required.";
            request.Budget.ValidateAll();
            if (request.Budget.HasErrors) return string.Join("; ", request.Budget.ErrorMessages);
            foreach (var entry in request.Entries)
            {
                entry.ValidateAll();
                if (entry.HasErrors) return string.Join("; ", entry.ErrorMessages);
            }
            return request.Entries.Sum(x => x.Amount) == request.Budget.TotalValue
                ? null : "Budget entries must total the budget value.";
        }
    }

    public class SaveBudgetRequest
    {
        public BudgetDTO Budget { get; set; }
        public List<BudgetEntryDTO> Entries { get; set; }
    }
}
