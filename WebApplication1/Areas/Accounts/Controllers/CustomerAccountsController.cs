using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using iTextSharp.xmp.impl;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

using WebApplication.Services;
using WebApplication1.Helpers;
using Utils = WebApplication1.Helpers.Utils;

namespace WebApplication1.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/customer-accounts")]
    public class CustomerAccountsController : ApiController
    {
        private readonly CustomerAccountService _service = new CustomerAccountService();

        private readonly ICustomerAccountAppService _customerAccountService; 

        public CustomerAccountsController(ICustomerAccountAppService customerAccountAppService)
        {
            _customerAccountService = customerAccountAppService;
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var accounts = _service.GetAll();
                return ApiResponse(true, "Customer accounts retrieved successfully", accounts);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var account = _service.GetById(id);
                if (account == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer account not found" });

                return ApiResponse(true, "Customer account retrieved successfully", account);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        // --------------------------------------------------------
        // Create a single customer account for a specific product.
        // The DTO's CustomerId, BranchId, and
        // CustomerAccountTypeTargetProductId must all be set.
        // --------------------------------------------------------
        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] CustomerAccountDTO account)
        {
            try
            {
                if (account == null)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Invalid customer account data" });

                if (account.CustomerId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "CustomerId is required" });

                if (account.BranchId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "BranchId is required" });

                if (account.CustomerAccountTypeTargetProductId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "CustomerAccountTypeTargetProductId is required" });

                var created = _service.Create(account);
                return Content(System.Net.HttpStatusCode.Created,
                               new { success = true, message = "Customer account created successfully", data = created });
            }
            catch (ArgumentException ex)
            {
                // Bad input (missing customer, missing required field, etc.)
                return Content(System.Net.HttpStatusCode.BadRequest,
                               new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Duplicate account for this customer/product combo
                return Content(System.Net.HttpStatusCode.Conflict,
                               new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        // --------------------------------------------------------
        // Bulk-create one account per product attached to the
        // customer's branch/company. Skips products the customer
        // already has an account for.
        // --------------------------------------------------------
        [HttpPost, Route("customer/{customerId:guid}/branch/{branchId:guid}")]
        public IHttpActionResult CreateAccountsForCustomer(Guid customerId, Guid branchId)
        {
            try
            {
                if (customerId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "CustomerId is required" });

                if (branchId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "BranchId is required" });

                var createdAccounts = _service.CreateAccountsForCustomer(customerId, branchId);
                var createdList = createdAccounts.ToList();

                if (!createdList.Any())
                    return ApiResponse(true, "No new accounts created (customer already has accounts for all attached products, or no products are attached)", createdList);

                return Content(System.Net.HttpStatusCode.Created,
                               new { success = true, message = $"{createdList.Count} customer account(s) created successfully", data = createdList });
            }
            catch (ArgumentException ex)
            {
                return Content(System.Net.HttpStatusCode.BadRequest,
                               new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("{id:guid}/accounts")]
        public IHttpActionResult GetCustomerAccounts(Guid id)
        {
            try
            {
                //var customerAccountService = new CustomerAccountService();
                var serviceHeader = Utils.CreateServiceHeader();
                var accounts = _customerAccountService.FindCustomerAccountsByCustomerId(id, serviceHeader);
                //var accounts = customerAccountService.GetByCustomerId(id);

                return ApiResponse(true, "Customer accounts retrieved successfully", accounts);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        // Optional: Additional endpoints for specific queries
        [HttpGet, Route("customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomerId(Guid customerId)
        {
            try
            {
                var accounts = _service.GetAll();
                // Filter by customerId - you might want to implement a specific service method for this
                return ApiResponse(true, "Customer accounts retrieved successfully", accounts);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("account-number/{accountNumber}")]
        public IHttpActionResult GetByAccountNumber(string accountNumber)
        {
            try
            {
                // You'll need to implement logic to parse account number and query accordingly
                return ApiResponse(true, "Endpoint not fully implemented", null);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }
    }
}