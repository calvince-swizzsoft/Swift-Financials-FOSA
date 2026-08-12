using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC CoA_eStatementsController (Areas/Accounts) —
    // CRUD + listing over ElectronicStatementOrderDTO, a recurring "send this
    // customer account a statement on a schedule" subscription (Duration/Schedule
    // value objects), distinct from CustomerAccountStatementController, which
    // handles on-demand statement content (mini/full/PDF) and never touches this
    // resource at all.
    //
    // Split into two controllers, mirroring StandingOrderController/
    // StandingOrderExecutionController for the same underlying reason: the actual
    // "run the batch now" capability (ExecuteElectronicStatementOrders) lives on a
    // separate app service (IRecurringBatchAppService), not on
    // IElectronicStatementOrderAppService. Batch triggers (execute/fix-skipped)
    // are grouped in ElectronicStatementOrderExecutionController.
    [RoutePrefix("api/accounts/electronicstatementorders")]
    public class ElectronicStatementOrderController : ApiController
    {
        private readonly IElectronicStatementOrderAppService _electronicStatementOrderAppService;

        public ElectronicStatementOrderController(IElectronicStatementOrderAppService electronicStatementOrderAppService)
        {
            _electronicStatementOrderAppService = electronicStatementOrderAppService ?? throw new ArgumentNullException(nameof(electronicStatementOrderAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _electronicStatementOrderAppService.FindElectronicStatementOrders(pageIndex, pageSize, serviceHeader)
                    : _electronicStatementOrderAppService.FindElectronicStatementOrders(text, customerFilter, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "E-statement orders retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var electronicStatementOrder = _electronicStatementOrderAppService.FindElectronicStatementOrder(id, serviceHeader);
                if (electronicStatementOrder == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "E-statement order not found");

                return ApiResponse(true, "E-statement order retrieved successfully", electronicStatementOrder);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/history")]
        public IHttpActionResult GetHistory(Guid id, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var history = _electronicStatementOrderAppService.FindElectronicStatementOrderHistory(id, pageIndex, pageSize, serviceHeader);
                if (history == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "E-statement order history not found");

                return ApiResponse(true, "E-statement order history retrieved successfully", history);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("by-customer-account/{customerAccountId:guid}")]
        public IHttpActionResult GetByCustomerAccount(Guid customerAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrdersByCustomerAccountId(customerAccountId, serviceHeader);

                return ApiResponse(true, "E-statement orders retrieved successfully", electronicStatementOrders ?? new List<ElectronicStatementOrderDTO>());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("by-customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomer(Guid customerId, [FromUri] int productCode = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrdersByCustomerId(customerId, productCode, serviceHeader);

                return ApiResponse(true, "E-statement orders retrieved successfully", electronicStatementOrders ?? new List<ElectronicStatementOrderDTO>());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("due")]
        public IHttpActionResult GetDue(
            [FromUri] DateTime? targetDate = null,
            [FromUri] int targetDateOption = 0,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var electronicStatementOrders = _electronicStatementOrderAppService.FindDueElectronicStatementOrders(targetDate ?? DateTime.Today, targetDateOption, text, customerFilter, serviceHeader);

                return ApiResponse(true, "Due e-statement orders retrieved successfully", electronicStatementOrders ?? new List<ElectronicStatementOrderDTO>());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Unlike GetSkipped's Standing Order sibling, FindSkippedElectronicStatementOrders
        // returns an unpaged List<T> — verified directly against
        // IElectronicStatementOrderAppService, not assumed from the naming symmetry.
        [HttpGet, Route("skipped")]
        public IHttpActionResult GetSkipped(
            [FromUri] DateTime? targetDate = null,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var electronicStatementOrders = _electronicStatementOrderAppService.FindSkippedElectronicStatementOrders(targetDate ?? DateTime.Today, text, customerFilter, serviceHeader);

                return ApiResponse(true, "Skipped e-statement orders retrieved successfully", electronicStatementOrders ?? new List<ElectronicStatementOrderDTO>());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] ElectronicStatementOrderDTO electronicStatementOrderDTO)
        {
            try
            {
                if (electronicStatementOrderDTO == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid e-statement order data");

                electronicStatementOrderDTO.ValidateAll();

                if (electronicStatementOrderDTO.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", electronicStatementOrderDTO.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                ElectronicStatementOrderDTO createdElectronicStatementOrder;

                try
                {
                    createdElectronicStatementOrder = _electronicStatementOrderAppService.AddNewElectronicStatementOrder(electronicStatementOrderDTO, serviceHeader);
                }
                catch (InvalidOperationException ex)
                {
                    // AddNewElectronicStatementOrder throws rather than setting an
                    // ErrorMessageResult (the DTO has no such field, unlike Commission/
                    // UnPayReason) when the account already has an order — surfaced
                    // here as a real 409, not a generic 500.
                    return ErrorResponse(HttpStatusCode.Conflict, ex.Message);
                }

                if (createdElectronicStatementOrder == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create e-statement order");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "E-statement order created successfully",
                    data = createdElectronicStatementOrder
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] ElectronicStatementOrderDTO electronicStatementOrderDTO)
        {
            try
            {
                if (electronicStatementOrderDTO == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid e-statement order data");

                if (id != electronicStatementOrderDTO.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                electronicStatementOrderDTO.ValidateAll();

                if (electronicStatementOrderDTO.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", electronicStatementOrderDTO.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var existingElectronicStatementOrder = _electronicStatementOrderAppService.FindElectronicStatementOrder(id, serviceHeader);
                if (existingElectronicStatementOrder == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "E-statement order not found");

                bool updated;

                try
                {
                    updated = _electronicStatementOrderAppService.UpdateElectronicStatementOrder(electronicStatementOrderDTO, serviceHeader);
                }
                catch (InvalidOperationException ex)
                {
                    // e.g. "The start date must not be less than today!" — a real
                    // rule enforced inside the app service, not covered by ValidateAll().
                    return ErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }

                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update e-statement order");

                var updatedElectronicStatementOrder = _electronicStatementOrderAppService.FindElectronicStatementOrder(id, serviceHeader);
                return ApiResponse(true, "E-statement order updated successfully", updatedElectronicStatementOrder);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
