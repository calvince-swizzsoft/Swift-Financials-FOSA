using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [RoutePrefix("api/accounts/customer-accounts")]
    public class CustomerAccountSignatoryController : ApiController
    {
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public CustomerAccountSignatoryController(ICustomerAccountAppService customerAccountAppService)
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

        [HttpGet, Route("{customerAccountId:guid}/signatories")]
        public IHttpActionResult Get(Guid customerAccountId, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var signatories = _customerAccountAppService.FindCustomerAccountSignatoriesByCustomerAccountId(customerAccountId, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Signatories retrieved successfully", signatories);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{customerAccountId:guid}/signatories/all")]
        public IHttpActionResult GetAll(Guid customerAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var signatories = _customerAccountAppService.FindCustomerAccountSignatoriesByCustomerAccountId(customerAccountId, serviceHeader);

                return ApiResponse(true, "Signatories retrieved successfully", signatories);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("{customerAccountId:guid}/signatories")]
        public IHttpActionResult Create(Guid customerAccountId, [FromBody] CustomerAccountSignatoryDTO signatory)
        {
            try
            {
                if (signatory == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid signatory data");

                signatory.CustomerAccountId = customerAccountId;

                signatory.ValidateAll();

                if (signatory.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join("; ", signatory.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var created = _customerAccountAppService.AddNewCustomerAccountSignatory(signatory, serviceHeader);

                if (created == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to add signatory");

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Signatory added successfully",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Bulk-remove by id — the app service has no single-remove or update, so re-add to change one.
        [HttpDelete, Route("signatories")]
        public IHttpActionResult Remove([FromBody] List<Guid> signatoryIds)
        {
            try
            {
                if (signatoryIds == null || !signatoryIds.Any())
                    return ErrorResponse(HttpStatusCode.BadRequest, "No signatory ids supplied");

                var serviceHeader = Utils.CreateServiceHeader();

                var signatories = signatoryIds.Select(id => new CustomerAccountSignatoryDTO { Id = id }).ToList();

                var result = _customerAccountAppService.RemoveCustomerAccountSignatories(signatories, serviceHeader);

                return ApiResponse(result, result ? "Signatories removed successfully" : "No signatories were removed");
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
