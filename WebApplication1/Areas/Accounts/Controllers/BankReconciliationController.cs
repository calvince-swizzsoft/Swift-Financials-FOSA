using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
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
            var data = _appService.FindBankReconciliationPeriod(id, Utils.CreateServiceHeader());
            return data == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data });
        }

        [HttpPost, Route("periods")]
        public IHttpActionResult CreatePeriod(BankReconciliationPeriodDTO model)
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
        }

        [HttpPut, Route("periods/{id:guid}")]
        public IHttpActionResult UpdatePeriod(Guid id, BankReconciliationPeriodDTO model)
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
        }

        [HttpGet, Route("periods/{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var header = Utils.CreateServiceHeader();
            if (_appService.FindBankReconciliationPeriod(id, header) == null) return NotFound();
            var data = _appService.FindBankReconciliationEntriesByBankReconciliationPeriodId(id, text, pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data });
        }

        [HttpPost, Route("periods/{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, BankReconciliationEntryDTO model)
        {
            if (model == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Reconciliation entry payload is required." });
            var header = Utils.CreateServiceHeader();
            var period = _appService.FindBankReconciliationPeriod(id, header);
            if (period == null) return NotFound();
            if (period.Status != (int)BankReconciliationPeriodStatus.Open)
                return Content(HttpStatusCode.Conflict, new { success = false, message = "Entries can only be added to an open reconciliation period." });
            model.BankReconciliationPeriodId = id;
            model.ValidateAll();
            if (model.HasErrors)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", model.ErrorMessages) });
            var data = _appService.AddNewBankReconciliationEntry(model, header);
            return Ok(new { success = true, message = "Reconciliation entry added successfully.", data });
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
        }
    }

    public class CloseBankReconciliationRequest
    {
        public int AuthOption { get; set; }
        public string AuthorizationRemarks { get; set; }
    }
}
