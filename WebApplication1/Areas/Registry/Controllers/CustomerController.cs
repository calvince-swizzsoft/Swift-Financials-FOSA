using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using System.Diagnostics;
using System.Web.Hosting;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/registry/customer")]
    public class CustomerController : ApiController
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IDebitTypeAppService _debitTypeAppService;
        private readonly IMediaAppService _mediaAppService;

        public CustomerController(
            ICustomerAppService customerAppService,
            IAuthorizationAppService authorizationAppService,
            IDebitTypeAppService debitTypeAppService,
            IMediaAppService mediaAppService)
        {
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
            _debitTypeAppService = debitTypeAppService ?? throw new ArgumentNullException(nameof(debitTypeAppService));
            _mediaAppService = mediaAppService ?? throw new ArgumentNullException(nameof(mediaAppService));
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        // For the account-alerts picker below — no REST exposure of
        // SystemTransactionCode existed anywhere before this.
        [HttpGet, Route("transaction-codes")]
        public IHttpActionResult GetTransactionCodes()
        {
            try
            {
                var codes = Enum.GetValues(typeof(SystemTransactionCode))
                    .Cast<SystemTransactionCode>()
                    .Select(code => new { Value = (int)code, Description = EnumHelper.GetDescription(code) })
                    .OrderBy(x => x.Description)
                    .ToList();

                return ApiResponse(true, "Transaction codes retrieved successfully", codes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet, Route("registration/debit-types")]
        public IHttpActionResult GetRegistrationDebitTypes()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            return ApiResponse(true, "Debit types retrieved successfully", _debitTypeAppService.FindDebitTypes(serviceHeader));
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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

                if (createdCustomer == null || createdCustomer.Id == Guid.Empty)
                    return ErrorResponse(HttpStatusCode.BadRequest, createdCustomer?.ErrorMessageResult ?? "Customer creation failed");

                if (!string.IsNullOrWhiteSpace(createdCustomer.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, new
                    {
                        success = false,
                        message = createdCustomer.ErrorMessageResult,
                        data = createdCustomer
                    });

                if (request.PartnershipMembers != null && request.PartnershipMembers.Any())
                    await _customerAppService.UpdatePartnershipMemberCollectionAsync(createdCustomer.Id, request.PartnershipMembers, serviceHeader);

                if (request.CorporationMembers != null && request.CorporationMembers.Any())
                    await _customerAppService.UpdateCorporationMemberCollectionAsync(createdCustomer.Id, request.CorporationMembers, serviceHeader);

                if (request.Referees != null && request.Referees.Any())
                    await _customerAppService.UpdateRefereeCollectionAsync(createdCustomer.Id, request.Referees, serviceHeader);

                string imageWarning = null;
                try
                {
                    SaveRegistrationImages(request.Customer, createdCustomer, serviceHeader);
                }
                catch (Exception imageException)
                {
                    // The customer is already committed before media is sent
                    // to SQL FILESTREAM. Do not report the whole registration
                    // as failed (which encourages a duplicate retry); return a
                    // successful registration with an actionable warning.
                    var correlationId = WebApplication1.ApiErrors.CorrelationIdHandler.GetCorrelationId(Request);
                    Trace.TraceError(
                        "Customer registration image save failed. CorrelationId={0} CustomerId={1} Exception={2}",
                        correlationId, createdCustomer.Id, imageException);
                    imageWarning = "Customer created, but one or more registration images could not be saved. " +
                        "Ask an administrator to check SQL FILESTREAM access. Do not register the customer again.";
                }

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = imageWarning ?? "Customer created successfully",
                    warning = imageWarning,
                    data = createdCustomer
                });
            }
            catch (InvalidOperationException exception)
            {
                return ErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveRegistrationImages(CustomerDTO source, CustomerDTO created, ServiceHeader serviceHeader)
        {
            var blobConnection = ConfigurationManager.ConnectionStrings["BLOBStore"]?.ConnectionString;
            var stagingDirectory = HostingEnvironment.MapPath("~/App_Data");
            if (string.IsNullOrWhiteSpace(blobConnection) || string.IsNullOrWhiteSpace(stagingDirectory))
                return;

            SaveImage(created.PassportImageId, source.PassportBuffer, "Customer passport photo", stagingDirectory, blobConnection, serviceHeader);
            SaveImage(created.SignatureImageId, source.SignatureBuffer, "Customer signature", stagingDirectory, blobConnection, serviceHeader);
            SaveImage(created.IdentityCardFrontSideImageId, source.IdentityCardFrontSideBuffer, "Identity card front", stagingDirectory, blobConnection, serviceHeader);
            SaveImage(created.IdentityCardBackSideImageId, source.IdentityCardBackSideBuffer, "Identity card back", stagingDirectory, blobConnection, serviceHeader);
        }

        private void SaveImage(Guid? imageId, byte[] content, string remarks, string stagingDirectory,
            string blobConnection, ServiceHeader serviceHeader)
        {
            if (!imageId.HasValue || imageId.Value == Guid.Empty || content == null || content.Length == 0)
                return;

            var saved = _mediaAppService.PostImage(new MediaDTO
            {
                SKU = imageId.Value,
                Content = content,
                ContentType = "image/jpeg",
                FileType = "CustomerRegistration",
                FileRemarks = remarks
            }, stagingDirectory, blobConnection, serviceHeader);

            if (!saved)
                throw new InvalidOperationException(string.Format("The {0} could not be saved.", remarks.ToLowerInvariant()));
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut, Route("{id:guid}/account-alerts")]
        public async Task<IHttpActionResult> UpdateAccountAlerts(Guid id, [FromBody] List<AccountAlertDTO> accountAlerts)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var customer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                if (customer == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer not found. The alert preferences were not changed.");

                var requestedAlerts = accountAlerts ?? new List<AccountAlertDTO>();
                for (var index = 0; index < requestedAlerts.Count; index++)
                {
                    var item = requestedAlerts[index];
                    var row = index + 1;
                    if (item == null)
                        return ErrorResponse(HttpStatusCode.BadRequest, string.Format("Account alert {0} is empty.", row));
                    if (!Enum.IsDefined(typeof(SystemTransactionCode), (int)item.Type))
                        return ErrorResponse(HttpStatusCode.BadRequest, string.Format("Account alert {0} has an invalid transaction type. Remove it and select an available transaction type.", row));
                    if (item.Threshold < 0)
                        return ErrorResponse(HttpStatusCode.BadRequest, string.Format("Account alert {0} has a negative threshold. Enter zero or a positive amount.", row));
                    if (!Enum.IsDefined(typeof(QueuePriority), (int)item.Priority))
                        return ErrorResponse(HttpStatusCode.BadRequest, string.Format("Account alert {0} has an invalid priority. Select Lowest through Highest.", row));
                    if (!item.ReceiveTextAlert && !item.ReceiveEmailAlert)
                        return ErrorResponse(HttpStatusCode.BadRequest, string.Format("Account alert {0} needs at least one delivery channel. Select SMS, email, or both.", row));
                }

                if (requestedAlerts.GroupBy(item => item.Type).Any(group => group.Count() > 1))
                    return ErrorResponse(HttpStatusCode.BadRequest, "Only one account alert preference is allowed per transaction type.");

                var existing = await _customerAppService.FindAccountAlertCollectionAsync(id, serviceHeader) ?? new List<AccountAlertDTO>();
                var updated = await _customerAppService.UpdateAccountAlertCollectionAsync(id, requestedAlerts, serviceHeader);
                // Replacing an already-empty collection with another empty collection is a valid no-op.
                if (!updated && !existing.Any() && !requestedAlerts.Any())
                    return ApiResponse(true, "Account alerts were already empty", requestedAlerts);

                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "The account alert changes were valid but the database did not save them. No success has been reported; retry once, then give the administrator the request reference shown by the UI.");

                var refreshed = await _customerAppService.FindAccountAlertCollectionAsync(id, serviceHeader);
                return ApiResponse(true, "Account alerts updated successfully", refreshed);
            }
            catch (Exception exception)
            {
                var correlationId = WebApplication1.ApiErrors.CorrelationIdHandler.GetCorrelationId(Request);
                Trace.TraceError("Account alert update failed. CorrelationId={0} CustomerId={1} Exception={2}", correlationId, id, exception);
                return Content(HttpStatusCode.InternalServerError, new
                {
                    success = false,
                    message = "The server could not save the account alert preferences. The failure was logged for investigation; use the reference below instead of retrying repeatedly.",
                    correlationId
                });
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
            catch (Exception)
            {
                throw;
            }
        }

        // ResetCustomerStationAsync takes a list (it's also used for bulk
        // resets elsewhere), but the Station Linkage screen only ever
        // removes one customer at a time — wrapped as a single-item list.
        [HttpDelete, Route("{id:guid}/station")]
        public async Task<IHttpActionResult> ResetStation(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _customerAppService.ResetCustomerStationAsync(new List<CustomerDTO> { new CustomerDTO { Id = id } }, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove customer from station");

                var updatedCustomer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                return ApiResponse(true, "Customer removed from station successfully", updatedCustomer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // UpdateCustomerBranch doesn't touch a Customer.BranchId field —
        // there isn't one. It reassigns every account the customer already
        // has to the given branch (via ICustomerAccountAppService), so it
        // returns false both when the branch doesn't exist AND when the
        // customer has no accounts yet; there's no way to tell which from
        // here without a second service call, so the message below covers
        // the more likely case. No reset/unlink counterpart exists in
        // ICustomerAppService — Areas/Registry/Branch linkage.md only
        // documents linking, not removing.
        [HttpPut, Route("{id:guid}/branch")]
        public async Task<IHttpActionResult> UpdateBranch(Guid id, [FromBody] CustomerDTO customer)
        {
            try
            {
                if (customer == null || id != customer.Id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid customer data");

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _customerAppService.UpdateCustomerBranch(customer, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Failed to link customer to branch — the customer must have at least one account before being linked to a branch.");

                var updatedCustomer = await _customerAppService.FindCustomerAsync(id, serviceHeader);
                return ApiResponse(true, "Customer linked to branch successfully", updatedCustomer);
            }
            catch (Exception)
            {
                throw;
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
        public List<PartnershipMemberDTO> PartnershipMembers { get; set; }
        public List<CorporationMemberDTO> CorporationMembers { get; set; }
        public List<RefereeDTO> Referees { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }
}
