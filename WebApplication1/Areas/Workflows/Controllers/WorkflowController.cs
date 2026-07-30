using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebApplication1.Areas.Workflows.Controllers
{
    [RoutePrefix("api/administration/workflows")]
    public class WorkflowController : ApiController
    {
        private readonly IWorkflowAppService _workflowAppService;

        public WorkflowController(IWorkflowAppService workflowAppService)
        {
            _workflowAppService = workflowAppService;
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("queueable")]
        public IHttpActionResult GetQueueable(int pageIndex = 1, int pageSize = 20)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var workflows = _workflowAppService.FindQueableWorkflows(pageIndex, pageSize, serviceHeader);

                return Ok(workflows);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Checker inbox: pending (and other status) items awaiting action for a given permission type.
        [HttpGet, Route("items")]
        public IHttpActionResult GetItems(int systemPermissionType, int status, string text, DateTime startDate, DateTime endDate, int pageIndex = 1, int pageSize = 20)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var items = _workflowAppService.FindWorkflowItems(systemPermissionType, status, text, startDate, endDate, pageIndex, pageSize, serviceHeader);

                return Ok(items);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("items/approve")]
        public IHttpActionResult ApproveItem(ApproveWorkflowItemRequest request)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var result = _workflowAppService.ApproveWorkflowItem(request.WorkflowItem, request.UsedBiometrics, serviceHeader);

                return Ok(result);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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

            catch (Exception ex)
            {
                return InternalServerError(ex);
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
    }
}
