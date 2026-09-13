using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using WebApplication1.ApiErrors;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/bank-reconciliations")]
    public class BankReconciliationController : ApiController
    {
        private const int ClosingModuleCode = 0x000059D8 + 63;
        private readonly IBankReconciliationPeriodAppService _appService;

        public BankReconciliationController(IBankReconciliationPeriodAppService appService)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        private IHttpActionResult Execute(Func<IHttpActionResult> action)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Enter a valid value for " + x.Key.Split('.').Last() + ". Check the date, amount or account selection." : e.ErrorMessage).ToArray());
                return ResponseMessage(ApiErrorResponses.Create(Request, HttpStatusCode.BadRequest, "reconciliation.invalid_request", string.Join(" ", errors.Values.SelectMany(x => x).Distinct()), errors));
            }
            try { return action(); }
            catch (BankReconciliationValidationException ex) { return Content(HttpStatusCode.Conflict, new { success = false, message = ex.Message }); }
        }

        [HttpGet, Route("balance")]
        public IHttpActionResult Balance(Guid bankLinkageId, DateTime endDate)
        {
            return Execute(() => Ok(new { success = true, data = _appService.FindBankReconciliationBalance(bankLinkageId, endDate, Utils.CreateServiceHeader()) }));
        }

        [HttpGet, Route("periods")]
        public IHttpActionResult GetPeriods(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var header = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? _appService.FindBankReconciliationPeriods(pageIndex, pageSize, header)
                : _appService.FindBankReconciliationPeriods(text, pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet, Route("periods/all")]
        public IHttpActionResult GetAllPeriods()
        {
            var data = _appService.FindBankReconciliationPeriods(Utils.CreateServiceHeader())
                ?? new List<BankReconciliationPeriodDTO>();
            return Ok(new { success = true, message = "", data });
        }

        [HttpGet, Route("periods/{id:guid}")]
        public IHttpActionResult GetPeriod(Guid id)
        {
            return Execute(() =>
            {
                var data = _appService.FindBankReconciliationPeriod(id, Utils.CreateServiceHeader());
                return data == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data });
            });
        }

        [HttpPost, Route("periods")]
        public IHttpActionResult CreatePeriod(BankReconciliationPeriodDTO model)
        {
            return Execute(() =>
            {
                if (model == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Bank reconciliation period payload is required." });
                model.ValidateAll();
                if (model.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", model.ErrorMessages) });
                var data = _appService.AddNewBankReconciliationPeriod(model, Utils.CreateServiceHeader());
                return data == null
                    ? Content(HttpStatusCode.BadRequest, new { success = false, message = "The bank reconciliation period could not be created." })
                    : (IHttpActionResult)Ok(new { success = true, message = "Bank reconciliation period created successfully.", data });
            });
        }

        [HttpPut, Route("periods/{id:guid}")]
        public IHttpActionResult UpdatePeriod(Guid id, BankReconciliationPeriodDTO model)
        {
            return Execute(() =>
            {
                if (model == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Bank reconciliation period payload is required." });
                var header = Utils.CreateServiceHeader();
                var current = _appService.FindBankReconciliationPeriod(id, header);
                if (current == null) return NotFound();
                if (current.Status != (int)BankReconciliationPeriodStatus.Open)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "Only an open reconciliation period can be updated." });
                model.Id = id;
                model.ValidateAll();
                if (model.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", model.ErrorMessages) });
                if (!_appService.UpdateBankReconciliationPeriod(model, header))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The bank reconciliation period could not be updated." });
                return Ok(new { success = true, message = "Bank reconciliation period updated successfully.", data = _appService.FindBankReconciliationPeriod(id, header) });
            });
        }

        [HttpGet, Route("periods/{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var header = Utils.CreateServiceHeader();
            if (_appService.FindBankReconciliationPeriod(id, header) == null) return NotFound();
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100) return Content(HttpStatusCode.BadRequest, new { success = false, message = "Use a nonnegative page index and page size 1–100." });
            var data = _appService.FindBankReconciliationEntriesByBankReconciliationPeriodId(id, text, pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data });
        }

        [HttpPost, Route("periods/{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, AddBankReconciliationEntryRequest request)
        {
            return Execute(() =>
            {
                if (request == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Reconciliation entry payload is required." });
                var header = Utils.CreateServiceHeader();
                var period = _appService.FindBankReconciliationPeriod(id, header);
                if (period == null) return NotFound();
                if (period.Status != (int)BankReconciliationPeriodStatus.Open)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "Entries can only be added to an open reconciliation period." });
                var model = request.ToEntry(id);
                model.ValidateAll();
                if (model.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", model.ErrorMessages) });
                var data = _appService.AddNewBankReconciliationEntry(model, header);
                if (data == null) return Content(HttpStatusCode.Conflict, new { success = false, message = "The period is no longer open. Refresh and try again." });
                return Ok(new { success = true, message = "Reconciliation entry added successfully.", data });
            });
        }

        [HttpDelete, Route("periods/{periodId:guid}/entries/{entryId:guid}")]
        public IHttpActionResult RemoveEntry(Guid periodId, Guid entryId)
        {
            var header = Utils.CreateServiceHeader();
            var period = _appService.FindBankReconciliationPeriod(periodId, header);
            if (period == null) return NotFound();
            if (period.Status != (int)BankReconciliationPeriodStatus.Open)
                return Content(HttpStatusCode.Conflict, new { success = false, message = "Entries can only be removed from an open reconciliation period." });
            var removed = _appService.RemoveBankReconciliationEntries(
                new List<BankReconciliationEntryDTO> { new BankReconciliationEntryDTO { Id = entryId, BankReconciliationPeriodId = periodId } }, header);
            if (!removed)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "The reconciliation entry could not be removed." });
            return Ok(new { success = true, message = "Reconciliation entry removed successfully." });
        }

        [HttpPost, Route("periods/{id:guid}/close")]
        public IHttpActionResult Close(Guid id, CloseBankReconciliationRequest request)
        {
            return Execute(() =>
            {
                if (request == null || (request.AuthOption != 1 && request.AuthOption != 2))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "AuthOption must be 1 (Post) or 2 (Reject)." });
                var header = Utils.CreateServiceHeader();
                var period = _appService.FindBankReconciliationPeriod(id, header);
                if (period == null) return NotFound();
                if (period.Status != (int)BankReconciliationPeriodStatus.Open)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "Only an open reconciliation period can be closed or rejected." });
                period.AuthorizationRemarks = request.AuthorizationRemarks;
                var closed = _appService.CloseBankReconciliationPeriod(period, request.AuthOption, ClosingModuleCode, header);
                if (!closed)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The bank reconciliation could not be closed." });
                return Ok(new { success = true, message = request.AuthOption == 1 ? "Bank reconciliation posted and closed successfully." : "Bank reconciliation rejected successfully.", data = _appService.FindBankReconciliationPeriod(id, header) });
            });
        }
    }

    // The URL owns the period ID. Bind only user-editable fields so DTO validation
    // cannot reject a missing period ID before the action has assigned it.
    public class AddBankReconciliationEntryRequest
    {
        [Range(0, 3)] public int AdjustmentType { get; set; }
        public Guid? ChartOfAccountId { get; set; }
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Value { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeDrawee { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string Remarks { get; set; }
        public BankReconciliationEntryDTO ToEntry(Guid periodId)
        {
            return new BankReconciliationEntryDTO { BankReconciliationPeriodId = periodId,
                AdjustmentType = AdjustmentType, ChartOfAccountId = ChartOfAccountId, Value = Value,
                ChequeNumber = ChequeNumber, ChequeDrawee = ChequeDrawee, ChequeDate = ChequeDate, Remarks = Remarks };
        }
    }

    public class CloseBankReconciliationRequest
    {
        public int AuthOption { get; set; }
        public string AuthorizationRemarks { get; set; }
    }
}
