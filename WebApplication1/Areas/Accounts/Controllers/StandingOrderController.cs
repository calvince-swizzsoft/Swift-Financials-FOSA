using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/standingorders")]
    public class StandingOrderController : ApiController
    {
        private readonly IStandingOrderAppService _standingOrderAppService;

        public StandingOrderController(IStandingOrderAppService standingOrderAppService)
        {
            _standingOrderAppService = standingOrderAppService ?? throw new ArgumentNullException(nameof(standingOrderAppService));
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
            [FromUri] int customerAccountFilter = 0,
            [FromUri] int customerFilter = 0,
            [FromUri] int? trigger = null)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = trigger.HasValue
                    ? _standingOrderAppService.FindStandingOrders(trigger.Value, text, customerAccountFilter, customerFilter, pageIndex, pageSize, serviceHeader)
                    : string.IsNullOrWhiteSpace(text)
                        ? _standingOrderAppService.FindStandingOrders(pageIndex, pageSize, serviceHeader)
                        : _standingOrderAppService.FindStandingOrders(text, customerAccountFilter, customerFilter, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Standing orders retrieved successfully", page);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrder = _standingOrderAppService.FindStandingOrder(id, serviceHeader);
                if (standingOrder == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Standing order not found");

                return ApiResponse(true, "Standing order retrieved successfully", standingOrder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}/history")]
        public IHttpActionResult GetHistory(Guid id, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var history = _standingOrderAppService.FindStandingOrderHistory(id, pageIndex, pageSize, serviceHeader);
                if (history == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Standing order history not found");

                return ApiResponse(true, "Standing order history retrieved successfully", history);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-benefactor-account/{benefactorCustomerAccountId:guid}")]
        public IHttpActionResult GetByBenefactorAccount(Guid benefactorCustomerAccountId, [FromUri] int? trigger = null)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrders = trigger.HasValue
                    ? _standingOrderAppService.FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccountId, trigger.Value, serviceHeader)
                    : _standingOrderAppService.FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccountId, serviceHeader);

                return ApiResponse(true, "Standing orders retrieved successfully", standingOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-beneficiary-account/{beneficiaryCustomerAccountId:guid}")]
        public IHttpActionResult GetByBeneficiaryAccount(Guid beneficiaryCustomerAccountId, [FromUri] int? trigger = null)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrders = trigger.HasValue
                    ? _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, trigger.Value, serviceHeader)
                    : _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, serviceHeader);

                return ApiResponse(true, "Standing orders retrieved successfully", standingOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("by-benefactor-customer/{benefactorCustomerId:guid}")]
        public IHttpActionResult GetByBenefactorCustomer(Guid benefactorCustomerId, [FromUri] int productCode = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrders = _standingOrderAppService.FindStandingOrdersByBenefactorCustomerId(benefactorCustomerId, productCode, serviceHeader);

                return ApiResponse(true, "Standing orders retrieved successfully", standingOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("due")]
        public IHttpActionResult GetDue(
            [FromUri] DateTime? targetDate = null,
            [FromUri] int targetDateOption = 0,
            [FromUri] string text = "",
            [FromUri] int customerAccountFilter = 0,
            [FromUri] int customerFilter = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrders = _standingOrderAppService.FindDueStandingOrders(targetDate ?? DateTime.Today, targetDateOption, text, customerAccountFilter, customerFilter, serviceHeader);

                return ApiResponse(true, "Due standing orders retrieved successfully", standingOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("skipped")]
        public IHttpActionResult GetSkipped(
            [FromUri] DateTime? targetDate = null,
            [FromUri] string text = "",
            [FromUri] int customerAccountFilter = 0,
            [FromUri] int customerFilter = 0,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var standingOrders = _standingOrderAppService.FindSkippedStandingOrders(targetDate ?? DateTime.Today, text, customerAccountFilter, customerFilter, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Skipped standing orders retrieved successfully", standingOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] StandingOrderDTO standingOrderDTO)
        {
            try
            {
                if (standingOrderDTO == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid standing order data");

                standingOrderDTO.ValidateAll();

                if (standingOrderDTO.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", standingOrderDTO.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var createdStandingOrder = _standingOrderAppService.AddNewStandingOrder(standingOrderDTO, serviceHeader);

                if (createdStandingOrder == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create standing order");

                if (!string.IsNullOrEmpty(createdStandingOrder.ErrorMessageResult))
                    return ErrorResponse(HttpStatusCode.Conflict, createdStandingOrder.ErrorMessageResult);

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Standing order created successfully",
                    data = createdStandingOrder
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] StandingOrderDTO standingOrderDTO)
        {
            try
            {
                if (standingOrderDTO == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid standing order data");

                if (id != standingOrderDTO.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                standingOrderDTO.ValidateAll();

                if (standingOrderDTO.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", standingOrderDTO.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var existingStandingOrder = _standingOrderAppService.FindStandingOrder(id, serviceHeader);
                if (existingStandingOrder == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Standing order not found");

                var updated = _standingOrderAppService.UpdateStandingOrder(standingOrderDTO, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update standing order");

                var updatedStandingOrder = _standingOrderAppService.FindStandingOrder(id, serviceHeader);
                return ApiResponse(true, "Standing order updated successfully", updatedStandingOrder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("auto-create")]
        public IHttpActionResult AutoCreate([FromBody] AutoCreateStandingOrdersRequest request)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var result = _standingOrderAppService.AutoCreateStandindOrders(request.BenefactorProductId, request.BenefactorProductCode, request.BeneficiaryProductId, serviceHeader);

                return ApiResponse(result, result ? "Standing orders auto-created successfully" : "No standing orders were auto-created");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public class AutoCreateStandingOrdersRequest
        {
            public Guid BenefactorProductId { get; set; }

            public int BenefactorProductCode { get; set; }

            public Guid BeneficiaryProductId { get; set; }
        }
    }
}
