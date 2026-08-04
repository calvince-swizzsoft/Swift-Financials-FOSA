using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [RoutePrefix("api/accounts/customer-accounts")]
    public class CustomerAccountManagementController : ApiController
    {
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public CustomerAccountManagementController(ICustomerAccountAppService customerAccountAppService)
        {
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        private IHttpActionResult Manage(Guid customerAccountId, CustomerAccountManagementAction action, ManageCustomerAccountRequest request, string successMessage)
        {
            try
            {
                if (request == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid request data");

                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var result = _customerAccountAppService.ManageCustomerAccount(customerAccountId, (int)action, request.Remarks, request.RemarkType, serviceHeader);

                return ApiResponse(result, result ? successMessage : $"Failed to {successMessage.ToLowerInvariant()}");
            }
            catch (InvalidOperationException ex)
            {
                // A business-rule guard (e.g. activating an account with no prior freeze history) — not a server fault.
                return ErrorResponse(HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("{customerAccountId:guid}/activate")]
        public IHttpActionResult Activate(Guid customerAccountId, [FromBody] ManageCustomerAccountRequest request)
        {
            return Manage(customerAccountId, CustomerAccountManagementAction.Activation, request, "Customer account activated successfully");
        }

        [HttpPost, Route("{customerAccountId:guid}/freeze")]
        public IHttpActionResult Freeze(Guid customerAccountId, [FromBody] ManageCustomerAccountRequest request)
        {
            return Manage(customerAccountId, CustomerAccountManagementAction.Deactivation, request, "Customer account frozen successfully");
        }

        [HttpPost, Route("{customerAccountId:guid}/close")]
        public IHttpActionResult Close(Guid customerAccountId, [FromBody] ManageCustomerAccountRequest request)
        {
            return Manage(customerAccountId, CustomerAccountManagementAction.Closure, request, "Customer account closed successfully");
        }

        [HttpPost, Route("{customerAccountId:guid}/remark")]
        public IHttpActionResult Remark(Guid customerAccountId, [FromBody] ManageCustomerAccountRequest request)
        {
            return Manage(customerAccountId, CustomerAccountManagementAction.Remark, request, "Remark added successfully");
        }

        [HttpPost, Route("{customerAccountId:guid}/signing-instructions")]
        public IHttpActionResult SigningInstructions(Guid customerAccountId, [FromBody] ManageCustomerAccountRequest request)
        {
            return Manage(customerAccountId, CustomerAccountManagementAction.SigningInstructions, request, "Signing instructions recorded successfully");
        }

        [HttpGet, Route("{customerAccountId:guid}/history")]
        public IHttpActionResult GetHistory(Guid customerAccountId, [FromUri] int? managementAction = null)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var history = managementAction.HasValue
                    ? _customerAccountAppService.FindCustomerAccountHistory(customerAccountId, managementAction.Value, serviceHeader)
                    : _customerAccountAppService.FindCustomerAccountHistory(customerAccountId, serviceHeader);

                return ApiResponse(true, "Customer account history retrieved successfully", history);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public class ManageCustomerAccountRequest
        {
            public string Remarks { get; set; }

            public int RemarkType { get; set; }
        }
    }
}
