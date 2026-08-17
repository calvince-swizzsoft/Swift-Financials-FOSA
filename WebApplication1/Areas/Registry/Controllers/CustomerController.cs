using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/registry/customer")]
    public class CustomerController : ApiController
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public CustomerController(ICustomerAppService customerAppService, IAuthorizationAppService authorizationAppService)
        {
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        private bool CanEditCustomer(ServiceHeader serviceHeader)
        {
            var configuredRoles = _authorizationAppService.GetRolesAndApprovalPriorityByPermissionType((int)SystemPermissionType.CustomerEditing, serviceHeader);
            return configuredRoles != null && configuredRoles.Any(x =>
                serviceHeader.ApplicationUserRoles.Any(role => string.Equals(role, x.RoleName, StringComparison.OrdinalIgnoreCase)));
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
        public async Task<IHttpActionResult> Get(
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? await _customerAppService.FindCustomersAsync(pageIndex, pageSize, serviceHeader)
                    : await _customerAppService.FindCustomersAsync(text, customerFilter, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Customers retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                if (customer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer not found");

                var nextOfKins = await _customerAppService.FindNextOfKinCollectionAsync(id, serviceHeader);

                return ApiResponse(true, "Customer retrieved successfully", new
                {
                    customer,
                    nextOfKins
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("count")]
        public async Task<IHttpActionResult> GetCount()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var count = await _customerAppService.GetCustomersCountAsync(serviceHeader);
                return ApiResponse(true, "Customer count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("edit-access")]
        public IHttpActionResult GetEditAccess()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            return ApiResponse(true, "Customer edit access resolved", new { canEdit = CanEditCustomer(serviceHeader) });
        }

        [HttpGet, Route("by-type/{type:int}")]
        public async Task<IHttpActionResult> GetByType(
            int type,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = await _customerAppService.FindCustomersByTypeAsync(type, text, customerFilter, pageIndex, pageSize, serviceHeader);
                return ApiResponse(true, "Customers retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("by-record-status/{recordStatus:int}")]
        public async Task<IHttpActionResult> GetByRecordStatus(
            int recordStatus,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var page = await _customerAppService.FindCustomersByRecordStatusAsync(recordStatus, text, customerFilter, pageIndex, pageSize, serviceHeader);
                return ApiResponse(true, "Customers retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("by-station/{stationId:guid}")]
        public async Task<IHttpActionResult> GetByStation(
            Guid stationId,
            [FromUri] string text = "",
            [FromUri] int customerFilter = 0,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var page = await _customerAppService.FindCustomersAsync(stationId, text, customerFilter, pageIndex, pageSize, serviceHeader);
                return ApiResponse(true, "Customers retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("search/identity-card")]
        public async Task<IHttpActionResult> SearchByIdentityCard(
            [FromUri] string identityCardNumber,
            [FromUri] bool exactMatch = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityCardNumber))
                    return ErrorResponse(HttpStatusCode.BadRequest, "Identity card number is required");

                var serviceHeader = Utils.CreateServiceHeader();
                var customers = await _customerAppService.FindCustomersByIdentityCardNumberAsync(identityCardNumber, exactMatch, serviceHeader);

                return ApiResponse(
                    true,
                    customers.Count > 0 ? "Customers found" : "No customer found with the given identity card number",
                    customers);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("search/id-number/{identityCardNumber}")]
        public async Task<IHttpActionResult> SearchByIdNumber(string identityCardNumber)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var customers = await _customerAppService.FindCustomersByIDNumberAsync(identityCardNumber, serviceHeader);

                return ApiResponse(
                    true,
                    customers.Count > 0 ? "Customers found" : "No customer found with the given ID number",
                    customers);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("search/serial-number/{serialNumber:int}")]
        public async Task<IHttpActionResult> SearchBySerialNumber(int serialNumber)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var customers = await _customerAppService.FindCustomerBySerialNumberAsync(serialNumber, serviceHeader);

                return ApiResponse(
                    true,
                    customers.Count > 0 ? "Customers found" : "No customer found with the given serial number",
                    customers);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("search/payroll-numbers")]
        public async Task<IHttpActionResult> SearchByPayrollNumbers(
            [FromUri] string payrollNumbers,
            [FromUri] bool matchExact = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(payrollNumbers))
                    return ErrorResponse(HttpStatusCode.BadRequest, "Payroll numbers are required");

                var serviceHeader = Utils.CreateServiceHeader();
                var customers = await _customerAppService.FindCustomersByPayrollNumbersAsync(payrollNumbers, matchExact, serviceHeader);

                return ApiResponse(
                    true,
                    customers.Count > 0 ? "Customers found" : "No customer found for the given payroll numbers",
                    customers);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/next-of-kin")]
        public async Task<IHttpActionResult> GetNextOfKin(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var nextOfKins = await _customerAppService.FindNextOfKinCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Next of kin retrieved successfully", nextOfKins);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/account-alerts")]
        public async Task<IHttpActionResult> GetAccountAlerts(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var alerts = await _customerAppService.FindAccountAlertCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Account alerts retrieved successfully", alerts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/partnership-members")]
        public async Task<IHttpActionResult> GetPartnershipMembers(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var members = await _customerAppService.FindPartnershipMemberCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Partnership members retrieved successfully", members);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/corporation-members")]
        public async Task<IHttpActionResult> GetCorporationMembers(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var members = await _customerAppService.FindCorporationMemberCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Corporation members retrieved successfully", members);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/referees")]
        public async Task<IHttpActionResult> GetReferees(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var referees = await _customerAppService.FindRefereeCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Referees retrieved successfully", referees);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/credit-types")]
        public IHttpActionResult GetCreditTypes(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var creditTypes = _customerAppService.FindCreditTypes(id, serviceHeader);
                return ApiResponse(true, "Credit types retrieved successfully", creditTypes);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            try
            {
                if (request?.Customer == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid customer data");

                var serviceHeader = Utils.CreateServiceHeader();

                var createdCustomer = await _customerAppService.AddNewCustomerAsync(
                    request.Customer,
                    request.AdditionalDebitTypes ?? new List<DebitTypeDTO>(),
                    request.AdditionalInvestmentProducts ?? new List<InvestmentProductDTO>(),
                    request.AdditionalSavingsProducts ?? new List<SavingsProductDTO>(),
                    request.ModuleNavigationItemCode,
                    serviceHeader);

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Customer created successfully",
                    data = createdCustomer
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, [FromBody] CustomerDTO customer)
        {
            try
            {
                if (customer == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid customer data");

                if (id != customer.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

                var serviceHeader = Utils.CreateServiceHeader();

                var existingCustomer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                if (existingCustomer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer not found");

                if (!CanEditCustomer(serviceHeader))
                    return ErrorResponse(HttpStatusCode.Forbidden, "Customer Editing permission is required.");

                // RecordStatus is deliberately ignored by the application service. With maker-checker enabled
                // this stages the snapshot; otherwise it updates the live record immediately.
                var updated = await _customerAppService.SubmitCustomerEditAsync(customer, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update customer");

                var updatedCustomer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                return ApiResponse(true, "Customer edit submitted successfully", updatedCustomer);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}/next-of-kin")]
        public async Task<IHttpActionResult> UpdateNextOfKin(Guid id, [FromBody] List<NextOfKinDTO> nextOfKins)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                if (customer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer not found");

                var updated = await _customerAppService.UpdateNextOfKinCollectionAsync(customer, nextOfKins ?? new List<NextOfKinDTO>(), serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update next of kin");

                var refreshed = await _customerAppService.FindNextOfKinCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Next of kin updated successfully", refreshed);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}/account-alerts")]
        public async Task<IHttpActionResult> UpdateAccountAlerts(Guid id, [FromBody] List<AccountAlertDTO> accountAlerts)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _customerAppService.UpdateAccountAlertCollectionAsync(id, accountAlerts ?? new List<AccountAlertDTO>(), serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update account alerts");

                var refreshed = await _customerAppService.FindAccountAlertCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Account alerts updated successfully", refreshed);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}/station")]
        public async Task<IHttpActionResult> UpdateStation(Guid id, [FromBody] CustomerDTO customer)
        {
            try
            {
                if (customer == null || id != customer.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid customer data");

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _customerAppService.UpdateCustomerStationAsync(customer, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update customer station");

                var updatedCustomer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                return ApiResponse(true, "Customer station updated successfully", updatedCustomer);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

    public class CreateCustomerRequest
    {
        public CustomerDTO Customer { get; set; }

        // Company-mandatory debit types/savings/investment products are resolved and attached
        // server-side automatically. These are extras the caller wants attached on top of those.
        public List<DebitTypeDTO> AdditionalDebitTypes { get; set; }
        public List<InvestmentProductDTO> AdditionalInvestmentProducts { get; set; }
        public List<SavingsProductDTO> AdditionalSavingsProducts { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }
}
