using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

using WebApplication1.Helpers;
using Utils = WebApplication1.Helpers.Utils;

namespace WebApplication1.Areas.Accounts.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/accounts/customer-accounts")]
    public class CustomerAccountsController : ApiController
    {
        private readonly ICustomerAccountAppService _customerAccountService;
        private readonly ICustomerAppService _customerAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly ICompanyAppService _companyAppService;

        public CustomerAccountsController(
            ICustomerAccountAppService customerAccountAppService,
            ICustomerAppService customerAppService,
            IBranchAppService branchAppService,
            ICompanyAppService companyAppService)
        {
            _customerAccountService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
            _companyAppService = companyAppService ?? throw new ArgumentNullException(nameof(companyAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
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
                    ? _customerAccountService.FindCustomerAccounts(pageIndex, pageSize, serviceHeader)
                    : _customerAccountService.FindCustomerAccounts(text, customerFilter, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Customer accounts retrieved successfully", page);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var accounts = _customerAccountService.FindCustomerAccounts(serviceHeader);
                return ApiResponse(true, "Customer accounts retrieved successfully", accounts);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                // FindCustomerAccounts (not FindCustomerAccountDTO) - a direct projection with no balance
                // fetch, which calls raw stored procs that assume an established transaction history.
                var account = _customerAccountService.FindCustomerAccounts(id, serviceHeader);
                if (account == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer account not found" });

                return ApiResponse(true, "Customer account retrieved successfully", account);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
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

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _customerAccountService.AddNewCustomerAccount(account, serviceHeader);

                if (created == null)
                    return Content(System.Net.HttpStatusCode.InternalServerError,
                                   new { success = false, message = "Failed to create customer account" });

                // AddNewCustomerAccount doesn't throw for a duplicate customer/product combo - it returns the
                // DTO with ErrorMessageResult set instead.
                if (!string.IsNullOrEmpty(created.ErrorMessageResult))
                    return Content(System.Net.HttpStatusCode.Conflict,
                                   new { success = false, message = created.ErrorMessageResult });

                return Content(System.Net.HttpStatusCode.Created,
                               new { success = true, message = "Customer account created successfully", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.BadRequest,
                               new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
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

                var serviceHeader = Utils.CreateServiceHeader();

                var customer = _customerAppService.FindCustomer(customerId, serviceHeader);
                if (customer == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer not found" });

                var branch = _branchAppService.FindBranch(branchId, serviceHeader);
                if (branch == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Branch not found" });

                customer.BranchId = branchId;

                var attachedProducts = _companyAppService.FindCachedAttachedProducts(branch.CompanyId, serviceHeader);

                var created = _customerAccountService.AddNewCustomerAccounts(
                    customer,
                    attachedProducts?.SavingsProductCollection,
                    attachedProducts?.InvestmentProductCollection,
                    attachedProducts?.LoanProductCollection,
                    serviceHeader);

                // AddNewCustomerAccounts only returns bool - re-fetch to give the caller the customer's
                // current account list rather than nothing.
                var accounts = _customerAccountService.FindCustomerAccountsByCustomerId(customerId, serviceHeader)
                    ?? new System.Collections.Generic.List<CustomerAccountDTO>();

                if (!created)
                    return ApiResponse(true, "No new accounts created (customer already has accounts for all attached products, or no products are attached)", accounts);

                return Content(System.Net.HttpStatusCode.Created,
                               new { success = true, message = "Customer account(s) created successfully", data = accounts });
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }

        [HttpGet, Route("{id:guid}/accounts")]
        public IHttpActionResult GetCustomerAccounts(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var accounts = _customerAccountService.FindCustomerAccountsByCustomerId(id, serviceHeader);

                return ApiResponse(true, "Customer accounts retrieved successfully", accounts);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }

        [HttpGet, Route("customer/{customerId:guid}")]
        public IHttpActionResult GetByCustomerId(Guid customerId, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var page = _customerAccountService.FindCustomerAccountsByCustomerId(customerId, pageIndex, pageSize, serviceHeader);
                return ApiResponse(true, "Customer accounts retrieved successfully", page);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }

        [HttpGet, Route("account-number/{accountNumber}")]
        public IHttpActionResult GetByAccountNumber(string accountNumber)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var account = _customerAccountService.FindCustomerAccountDTO(accountNumber, serviceHeader);
                if (account == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer account not found" });

                return ApiResponse(true, "Customer account retrieved successfully", account);
            }
            catch (Exception)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "The request could not be completed." });
            }
        }
    }
}
