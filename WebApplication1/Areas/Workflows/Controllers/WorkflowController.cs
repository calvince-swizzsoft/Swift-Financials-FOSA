using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.ApiErrors;

namespace WebApplication1.Areas.Workflows.Controllers
{
    [Authorize]
    [RoutePrefix("api/administration/workflows")]
    public class WorkflowController : ApiController
    {
        private readonly IWorkflowAppService _workflowAppService;
        private readonly IWorkflowProcessorAppService _workflowProcessorAppService;

        public WorkflowController(IWorkflowAppService workflowAppService, IWorkflowProcessorAppService workflowProcessorAppService)
        {
            _workflowAppService = workflowAppService;
            _workflowProcessorAppService = workflowProcessorAppService ?? throw new ArgumentNullException(nameof(workflowProcessorAppService));
        }

        #region Workflow

        [HttpGet, Route("")]
        public IHttpActionResult Index()
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflows = _workflowAppService.FindWorkflows(serviceHeader);

                return Ok(workflows);
            }

            catch (InvalidOperationException)
            {
                return Error(HttpStatusCode.Conflict, ErrorCodes.WorkflowInvalidState,
                    "The workflow request conflicts with its current state.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-record")]
        public IHttpActionResult GetByRecord(Guid recordId, int systemPermissionType)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflow = _workflowAppService.FindWorkflow(recordId, systemPermissionType, serviceHeader);

                return Ok(workflow);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{workflowId:guid}")]
        public IHttpActionResult Get(Guid workflowId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflow = _workflowAppService.FindWorkflow(workflowId, serviceHeader);

                return Ok(workflow);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("in-progress")]
        public IHttpActionResult IsInProgress(Guid recordId, int systemPermissionType)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var inProgress = _workflowAppService.IsWorkflowInProgress(recordId, systemPermissionType, serviceHeader);

                return Ok(inProgress);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("queueable")]
        public IHttpActionResult GetQueueable(int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflows = _workflowAppService.FindQueableWorkflows(pageIndex, pageSize, serviceHeader);

                return Ok(workflows);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(CreateWorkflowRequest request)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var result = _workflowAppService.AddNewWorkflow(request.Workflow, request.RolesInSystemPermissionType, serviceHeader);

                return Ok(result);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("matched")]
        public IHttpActionResult MarkMatched(Guid recordId, int systemPermissionType)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var result = _workflowAppService.MarkWorkflowMatched(recordId, systemPermissionType, serviceHeader);

                return Ok(result);
            }

            catch (Exception)
            {
                throw;
            }
        }

        // Admin recovery tool: a Workflow reaches Approved/Rejected and gets a message sent to the async
        // dispatcher queue (WorkflowAppService.SendToQueue), but if that message never arrives or never gets
        // processed (dispatcher not running, queue misconfigured, etc.) the workflow sits at
        // MatchedStatus=NotMatched forever with no user-facing symptom other than "nothing happened" on the
        // underlying record. This re-runs the exact same processing the dispatcher would have done
        // (IWorkflowProcessorAppService.ProcessWorkflowQueueAsync), synchronously, on demand, for one workflow -
        // it does not touch the queue/dispatcher at all, it's a direct bypass for when that path is stuck.
        [HttpPost, Route("{workflowId:guid}/match")]
        public async Task<IHttpActionResult> ManuallyMatch(Guid workflowId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflow = _workflowAppService.FindWorkflow(workflowId, serviceHeader);

                if (workflow == null)
                    return Error(HttpStatusCode.NotFound, ErrorCodes.ResourceNotFound,
                        "The workflow was not found.");

                if (workflow.MatchedStatus == (int)WorkflowMatchedStatus.Matched)
                    return Ok(new { success = true, message = "Workflow was already matched - no action taken.", data = workflow });

                if (workflow.Status != (int)WorkflowApprovalOption.Approved && workflow.Status != (int)WorkflowApprovalOption.Rejected)
                    return Error(HttpStatusCode.Conflict, ErrorCodes.WorkflowNotFinal,
                        "The workflow must reach an Approved or Rejected status before it can be matched.");

                var result = await _workflowProcessorAppService.ProcessWorkflowQueueAsync(workflow.RecordId, workflow.SystemPermissionType, workflow.Status, serviceHeader);

                return Ok(new { success = result, message = result ? "Workflow manually matched and processed." : "Manual match ran but reported no changes - check the underlying record/permission type wiring.", data = workflow });
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region WorkflowItem

        [HttpGet, Route("{workflowId:guid}/items")]
        public IHttpActionResult GetItemsForWorkflow(Guid workflowId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var items = _workflowAppService.FindWorkflowItems(workflowId, serviceHeader);

                return Ok(items);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("items/{workflowItemId:guid}")]
        public IHttpActionResult GetItem(Guid workflowItemId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var item = _workflowAppService.FindWorkflowItem(workflowItemId, serviceHeader);

                return Ok(item);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("items/{workflowItemId:guid}/record")]
        public IHttpActionResult GetRelatedRecord(Guid workflowItemId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            var item = _workflowAppService.FindWorkflowItem(workflowItemId, serviceHeader);
            if (item == null)
                return Error(HttpStatusCode.NotFound, ErrorCodes.ResourceNotFound,
                    "The workflow item was not found.");

            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            if (!callerRoles.Any(role => string.Equals(role, item.RoleName, StringComparison.OrdinalIgnoreCase)))
                return Error(HttpStatusCode.Forbidden, ErrorCodes.AccessDenied,
                    "You do not hold the role assigned to this approval item.");

            try
            {
                var record = _workflowProcessorAppService.FindRelatedRecord(
                    item.WorkflowRecordId,
                    item.WorkflowSystemPermissionType,
                    serviceHeader);

                if (record == null)
                    return Error(HttpStatusCode.NotFound, ErrorCodes.ResourceNotFound,
                        "The record related to this workflow item was not found.");

                return Ok(new
                {
                    recordType = item.WorkflowSystemPermissionTypeDescription,
                    data = record
                });
            }
            catch (NotSupportedException exception)
            {
                return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed, exception.Message);
            }
        }

        // Checker inbox: pending (and other status) items awaiting action for a given permission type.
        [HttpGet, Route("items")]
        public IHttpActionResult GetItems(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate, int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var items = _workflowAppService.FindWorkflowItems(systemPermissionType, status, text, startDate, endDate, pageIndex, pageSize, serviceHeader);

                return Ok(items);
            }

            catch (Exception)
            {
                throw;
            }
        }

        // Unified checker inbox across every permission type the caller's role(s) can act on. No
        // systemPermissionType parameter by design - it's resolved server-side purely from the caller's
        // roles (see WorkflowItemSpecifications.WorkflowItemForCallerRoles), never client-supplied.
        [HttpGet, Route("items/mine")]
        public IHttpActionResult GetMyItems(int status, string text, DateTime startDate, DateTime endDate, int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var items = _workflowAppService.FindWorkflowItems(status, text, startDate, endDate, pageIndex, pageSize, serviceHeader);

                return Ok(items);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("items/approve")]
        public async Task<IHttpActionResult> ApproveItem(ApproveWorkflowItemRequest request)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                if (request?.WorkflowItem == null)
                    return Error(HttpStatusCode.BadRequest, ErrorCodes.ValidationFailed,
                        "Workflow item data is required.");

                var persisted = _workflowAppService.FindWorkflowItem(request.WorkflowItem.Id, serviceHeader);
                if (persisted == null)
                    return Error(HttpStatusCode.NotFound, ErrorCodes.ResourceNotFound,
                        "The workflow item was not found.");

                var isLoanStage = persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.BackOfficeLoanAppraisal
                    || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.BackOfficeLoanApproval
                    || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.BackOfficeLoanAudit
                    || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.FrontOfficeLoanAppraisal
                    || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.FrontOfficeLoanApproval
                    || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.FrontOfficeLoanAudit;

                // Earlier priority stages remain ordinary sign-offs. The
                // final approval must travel with the detailed loan form so
                // the business transition and workflow completion cannot
                // be separated.
                if (isLoanStage
                    && request.WorkflowItem.Status == (int)WorkflowApprovalOption.Approved
                    && persisted.IsLastItemInOverallApprovalChain)
                    return Error(HttpStatusCode.Conflict,
                        ErrorCodes.WorkflowItemRequiresDetailedScreen,
                        "The final loan-stage workflow item must be completed from its detailed loan screen.");

                var result = _workflowAppService.ApproveWorkflowItem(request.WorkflowItem, request.UsedBiometrics, serviceHeader);

                if (result)
                {
                    var workflow = _workflowAppService.FindWorkflow(persisted.WorkflowId, serviceHeader);
                    var isCashRequestWorkflow = persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.CashDepositRequestAuthorization
                        || persisted.WorkflowSystemPermissionType == (int)SystemPermissionType.CashWithdrawalRequestAuthorization;

                    if (isCashRequestWorkflow
                        && workflow != null
                        && workflow.MatchedStatus != (int)WorkflowMatchedStatus.Matched
                        && (workflow.Status == (int)WorkflowApprovalOption.Approved
                            || workflow.Status == (int)WorkflowApprovalOption.Rejected))
                    {
                        var processed = await _workflowProcessorAppService.ProcessWorkflowQueueAsync(
                            workflow.RecordId, workflow.SystemPermissionType, workflow.Status, serviceHeader);
                        if (!processed)
                            return Error(HttpStatusCode.Conflict, ErrorCodes.WorkflowInvalidState,
                                "The approval was recorded, but the cash request could not be moved to its final authorization status. Verify request maturity and workflow configuration.");
                    }
                }

                return Ok(result);
            }

            catch (InvalidOperationException)
            {
                return Error(HttpStatusCode.Conflict, ErrorCodes.WorkflowInvalidState,
                    "The workflow item cannot be changed from its current state.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region WorkflowItemEntry

        [HttpGet, Route("{workflowId:guid}/entries")]
        public IHttpActionResult GetEntries(Guid workflowId)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var entries = _workflowAppService.FindWorkflowItemEntriesByWorkflow(workflowId, serviceHeader);

                return Ok(entries);
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region WorkflowSetting

        [HttpGet, Route("settings/{systemPermissionType:int}")]
        public IHttpActionResult GetSetting(int systemPermissionType)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var setting = _workflowAppService.FindWorkflowSetting(systemPermissionType, serviceHeader);

                return Ok(setting);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("settings")]
        public IHttpActionResult UpsertSetting(WorkflowSettingDTO workflowSettingDto)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var result = _workflowAppService.MapWorkflowSettingToSystemPermissionType(workflowSettingDto, serviceHeader);

                return Ok(result);
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public class CreateWorkflowRequest
        {
            public WorkflowDTO Workflow { get; set; }

            public List<SystemPermissionTypeInRoleDTO> RolesInSystemPermissionType { get; set; }
        }

        public class ApproveWorkflowItemRequest
        {
            public WorkflowItemDTO WorkflowItem { get; set; }

            public bool UsedBiometrics { get; set; }
        }

        private IHttpActionResult Error(HttpStatusCode statusCode, string code, string message)
        {
            return ResponseMessage(ApiErrorResponses.Create(Request, statusCode, code, message));
        }
    }
}
