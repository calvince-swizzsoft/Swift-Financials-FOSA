using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC ExpensePayableController — routes through
    // IExpensePayableAppService directly instead of the monolithic _channelService.
    //
    // Real sequence (per ExpensePayableAppService, not the reference controller's
    // ViewBag/TempData wiring): Create -> Pending; Verify (AuditExpensePayable) ->
    // Audited/Rejected/Deferred; Approve (AuthorizeExpensePayable) -> Posted (GL
    // journals created) /Rejected/Deferred.
    //
    // AuthorizeExpensePayable is already wired into the generic maker-checker engine
    // (WorkflowProcessorAppService, SystemPermissionType.ExpensePayablesAuthorization)
    // the same way cash deposit requests are — so there is no direct "Approve"
    // endpoint here. Verify enqueues a Workflow row when it succeeds; the actual
    // approval/posting happens through POST /api/administration/workflows/items/approve,
    // matching CashDepositController's pattern.
    [Authorize]
    [RoutePrefix("api/frontoffice/expensepayables")]
    public class ExpensePayableController : ApiController
    {
        private readonly IExpensePayableAppService _expensePayableAppService;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IWorkflowAppService _workflowAppService;

        public ExpensePayableController(
            IExpensePayableAppService expensePayableAppService,
            IAuthorizationAppService authorizationAppService,
            IWorkflowAppService workflowAppService)
        {
            _expensePayableAppService = expensePayableAppService ?? throw new ArgumentNullException(nameof(expensePayableAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
            _workflowAppService = workflowAppService ?? throw new ArgumentNullException(nameof(workflowAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int? status, string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var payables = status.HasValue
                    ? _expensePayableAppService.FindExpensePayables(status.Value, startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _expensePayableAppService.FindExpensePayables(text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = payables });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var payable = _expensePayableAppService.FindExpensePayable(id, serviceHeader);

                if (payable == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = payable });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult GetEntries(Guid id, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _expensePayableAppService.FindExpensePayableEntriesByExpensePayableId(id, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = entries });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Header — creates the voucher shell (Pending). Entry lines are added
        // separately (POST {id}/entries) before Verify.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(ExpensePayableDTO expensePayableDTO)
        {
            if (expensePayableDTO == null)
                return BadRequest("Request body is required");

            expensePayableDTO.ValidateAll();
            if (expensePayableDTO.HasErrors)
                return BadRequest(string.Join("; ", expensePayableDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _expensePayableAppService.AddNewExpensePayable(expensePayableDTO, serviceHeader);

                if (created == null)
                    return BadRequest("Failed to create the expense payable");

                if (!string.IsNullOrWhiteSpace(created.errormassage))
                    return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = created.errormassage, data = (object)null });

                return Ok(new { success = true, message = "Expense payable created successfully", data = created });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Add one GL line to a Pending expense payable's entry batch.
        [HttpPost]
        [Route("{id:guid}/entries")]
        public IHttpActionResult AddEntry(Guid id, ExpensePayableEntryDTO entry)
        {
            if (entry == null)
                return BadRequest("Request body is required");

            try
            {
                entry.ExpensePayableId = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _expensePayableAppService.AddNewExpensePayableEntry(entry, serviceHeader);

                if (created == null)
                    return BadRequest("Failed to add the expense payable entry");

                return Ok(new { success = true, message = "Entry added", data = created });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Batch-remove entries by id.
        [HttpPost]
        [Route("entries/remove")]
        public IHttpActionResult RemoveEntries([FromBody] List<ExpensePayableEntryDTO> entries)
        {
            if (entries == null || !entries.Any())
                return BadRequest("No entries selected for removal.");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _expensePayableAppService.RemoveExpensePayableEntries(entries, serviceHeader);

                if (!result)
                    return BadRequest("Failed to remove the selected entries");

                return Ok(new { success = true, message = "Entries removed", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify (checker step 1) — only accepts a Pending payable. On Post/Audit
        // success, enqueues the generic Workflow so the approval/posting step
        // (step 2) is handled by the existing maker-checker inbox, not this
        // controller directly.
        [HttpPost]
        [Route("{id:guid}/verify")]
        public IHttpActionResult Verify(Guid id, [FromBody] ExpensePayableActionRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _expensePayableAppService.FindExpensePayable(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.AuditRemarks = request?.Remarks;

                var option = request?.Option ?? (int)ExpensePayableAuthOption.Reject;

                var result = _expensePayableAppService.AuditExpensePayable(existing, option, serviceHeader);

                if (!result)
                    return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = "Expense payable is not Pending, or the action option is invalid", data = (object)null });

                if (option == (int)ExpensePayableAuthOption.Post)
                {
                    var rolesList = _authorizationAppService.GetRolesListForSystemPermissionType((int)SystemPermissionType.ExpensePayablesAuthorization, serviceHeader);

                    var workflowDto = new WorkflowDTO
                    {
                        RecordId = id,
                        BranchId = existing.BranchId,
                        Status = (int)WorkflowRecordStatus.Pending,
                        SystemPermissionType = (int)SystemPermissionType.ExpensePayablesAuthorization,
                        RequiredApprovals = rolesList.Sum(x => x.RequiredApprovers)
                    };

                    _workflowAppService.AddNewWorkflow(workflowDto, rolesList, serviceHeader);
                }

                var updated = _expensePayableAppService.FindExpensePayable(id, serviceHeader);

                return Ok(new { success = true, message = "Operation success", data = updated });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ExpensePayableActionRequest
    {
        // ExpensePayableAuthOption: 1 = Post, 2 = Reject, 4 = Defer.
        public int Option { get; set; }

        public string Remarks { get; set; }
    }
}
