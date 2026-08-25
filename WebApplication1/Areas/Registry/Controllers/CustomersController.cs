using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NOTE (2026-08-20): this controller (api/registry/customers, plural)
    // used to depend on WebApplication1/Services/CustomerService.cs, a
    // raw-ADO.NET class — every action here now goes through
    // ICustomerAppService instead, the same app service the real
    // create/edit path (api/registry/customer, singular, CustomerController.cs)
    // already uses. Along the way, every action that only ever wrapped
    // CustomerService with no live frontend caller was removed rather than
    // reimplemented against the app service — GetByType, GetByName,
    // GetByIdentificationNumber, GetByStation, the bundled
    // customer+accounts+next-of-kin+SMS Create flow (real creation goes
    // through CustomerController.cs / CustomerAppService.AddNewCustomerAsync,
    // which already handles company-mandatory product auto-attachment),
    // CheckDuplicateCustomer, GetByBankName/GetByBranchName/distinct-name
    // lookups, Search, and Delete (ICustomerAppService has no
    // delete/remove — customers are never hard-deleted in this domain).
    // What's left — GetAll, Get(id), Update, and the identity-card search —
    // are the only actions with a confirmed live frontend caller.
    [Authorize]
    [RoutePrefix("api/registry/customers")]
    public class CustomersController : ApiController
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly INextOfKinAppService _nextOfKinAppService;

        public CustomersController(
            ICustomerAppService customerAppService,
            INextOfKinAppService nextOfKinAppService)
        {
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _nextOfKinAppService = nextOfKinAppService ?? throw new ArgumentNullException(nameof(nextOfKinAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        [HttpGet]
        [Route("search/identity-card")]
        public async Task<IHttpActionResult> SearchByIdentityCard(string identityCardNumber, bool exactMatch = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityCardNumber))
                    return ApiResponse(false, "Identity card number is required");

                var serviceHeader = Utils.CreateServiceHeader();

                var customers = await _customerAppService.FindCustomersByIdentityCardNumberAsync(identityCardNumber, exactMatch, serviceHeader)
                    ?? new List<CustomerDTO>();

                return ApiResponse(
                    true,
                    customers.Any()
                        ? "Customers found"
                        : "No customer found with the given identity card number",
                    customers
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customers = await _customerAppService.FindCustomersAsync(serviceHeader) ?? new List<CustomerDTO>();

                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customer = _customerAppService.FindCustomer(id, serviceHeader);
                if (customer == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer not found" });

                // Get next of kins for this customer
                var nextOfKins = _nextOfKinAppService.FindNextOfKins(id, serviceHeader) ?? new List<NextOfKinDTO>();
                var nextOfKinTotal = nextOfKins.Sum(n => n.NominatedPercentage);
                var percentageSummary = new
                {
                    TotalNextOfKins = nextOfKins.Count,
                    TotalPercentage = nextOfKinTotal,
                    RemainingPercentage = Math.Max(0, 100 - nextOfKinTotal),
                };

                return ApiResponse(true, "Customer retrieved successfully", new
                {
                    customer = customer,
                    nextOfKins = nextOfKins,
                    percentageSummary = percentageSummary
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] CustomerDTO customer)
        {
            try
            {
                if (customer == null)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Invalid customer data" });

                if (id != customer.Id)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "ID mismatch" });

                // Validate required fields based on customer type
                if (customer.Type == 1) // Individual
                {
                    if (string.IsNullOrEmpty(customer.IndividualFirstName))
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "First name is required for individual customers" });

                    if (string.IsNullOrEmpty(customer.IndividualIdentityCardNumber))
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Identity card number is required for individual customers" });
                }
                else if (customer.Type == 3 || customer.Type == 2 || customer.Type == 4) // Corporate/Partnership/MicroCredit
                {
                    if (string.IsNullOrEmpty(customer.NonIndividualDescription))
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Description is required for corporate customers" });

                    if (string.IsNullOrEmpty(customer.NonIndividualRegistrationNumber))
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Registration number is required for corporate customers" });
                }

                // Validate mobile number for SMS
                if (string.IsNullOrEmpty(customer.AddressMobileLine))
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Mobile number is required" });

                var serviceHeader = Utils.CreateServiceHeader();

                // Check if customer exists
                var existingCustomer = _customerAppService.FindCustomer(id, serviceHeader);
                if (existingCustomer == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer not found" });

                // Set ModifiedBy if not provided
                if (string.IsNullOrEmpty(customer.ModifiedBy))
                    customer.ModifiedBy = serviceHeader.ApplicationUserName;

                // Update the customer — validation (duplicate identity/registration
                // numbers, required-field checks, etc.) lives inside
                // UpdateCustomerAsync itself, same as the real edit path.
                var updated = await _customerAppService.UpdateCustomerAsync(customer, serviceHeader);
                if (!updated)
                    return Content(System.Net.HttpStatusCode.InternalServerError,
                                   new { success = false, message = "Failed to update customer" });

                // Get updated customer with next of kin info
                var updatedCustomer = _customerAppService.FindCustomer(id, serviceHeader);
                var nextOfKins = _nextOfKinAppService.FindNextOfKins(id, serviceHeader) ?? new List<NextOfKinDTO>();
                var nextOfKinTotal = nextOfKins.Sum(n => n.NominatedPercentage);
                var percentageSummary = new
                {
                    TotalNextOfKins = nextOfKins.Count,
                    TotalPercentage = nextOfKinTotal,
                    RemainingPercentage = Math.Max(0, 100 - nextOfKinTotal),
                };

                return Content(System.Net.HttpStatusCode.OK,
                               new
                               {
                                   success = true,
                                   message = "Customer updated successfully",
                                   data = new
                                   {
                                       customer = updatedCustomer,
                                       nextOfKins = nextOfKins,
                                       percentageSummary = percentageSummary
                                   }
                               });
            }
            catch (InvalidOperationException) // Duplicate validation error
            {
                return Content(System.Net.HttpStatusCode.Conflict,
                               new { success = false, message = "The request could not be completed." });
            }
            catch (KeyNotFoundException) // Customer not found
            {
                return Content(System.Net.HttpStatusCode.NotFound,
                               new { success = false, message = "The request could not be completed." });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
