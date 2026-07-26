using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Services;
using WebApplication1.Helpers;
using WebApplication1.Services;


namespace WebApplication1.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
     
        private readonly CustomerService _service = new CustomerService();
        private readonly NextOfKinService _nextOfKinService = new NextOfKinService();

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        [HttpGet]
        [Route("search/identity-card")]
        public IHttpActionResult SearchByIdentityCard(
string identityCardNumber,
bool exactMatch = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identityCardNumber))
                    return ApiResponse(false, "Identity card number is required");

                var customers = _service
                    .SearchByIndividualIdentityCardNumber(identityCardNumber, exactMatch)
                    .ToList();

                return ApiResponse(
                    true,
                    customers.Any()
                        ? "Customers found"
                        : "No customer found with the given identity card number",
                    customers
                );
            }
            catch (Exception ex)
            {
                return ApiResponse(false, ex.Message);
            }
        }




        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var customers = _service.GetAll();
                return ApiResponse(true, "Customers retrieved successfully", customers);
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
                var customer = _service.GetById(id);
                if (customer == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer not found" });

                // Get next of kins for this customer
                var nextOfKins = _nextOfKinService.GetByCustomerId(id);
                var percentageSummary = _nextOfKinService.GetPercentageSummary(id);

                return ApiResponse(true, "Customer retrieved successfully", new
                {
                    customer = customer,
                    nextOfKins = nextOfKins,
                    percentageSummary = percentageSummary
                });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-type/{type:int}")]
        public IHttpActionResult GetByType(int type)
        {
            try
            {
                var customers = _service.GetByType(type);
                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-name/{name}")]
        public IHttpActionResult GetByName(string name)
        {
            try
            {
                var customers = _service.GetByName(name);
                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-identification/{identificationNumber}")]
        public IHttpActionResult GetByIdentificationNumber(string identificationNumber)
        {
            try
            {
                var customers = _service.GetByIdentificationNumber(identificationNumber);
                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-station/{stationId:guid}")]
        public IHttpActionResult GetByStation(Guid stationId)
        {
            try
            {
                var customers = _service.GetByStationId(stationId);
                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CustomerWithNextOfKinsDTO request)
        {
            try
            {
                if (request?.Customer == null)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Invalid customer data" });

                var customer = request.Customer;

                // Validate required fields
                if (customer.BranchId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Branch ID is required" });

                // Validate phone number
                if (string.IsNullOrEmpty(customer.AddressMobileLine))
                {
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Mobile number is required for SMS notification" });
                }

                // ENHANCED DUPLICATE VALIDATION
                string duplicateCheckMessage = "";
                CustomerDTO existingCustomer = null;

                if (customer.Type == 1) // Individual
                {
                    if (string.IsNullOrEmpty(customer.IndividualIdentityCardNumber))
                    {
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Identity card number is required for individual customers" });
                    }

                    existingCustomer = _service.GetByIdentityCardNumber(customer.IndividualIdentityCardNumber);
                    if (existingCustomer != null)
                    {
                        duplicateCheckMessage = $"Individual customer with ID Card Number '{customer.IndividualIdentityCardNumber}' already exists";
                    }
                }
                else if (customer.Type == 3) // Corporation
                {
                    if (string.IsNullOrEmpty(customer.NonIndividualRegistrationNumber))
                    {
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Registration number is required for corporate customers" });
                    }

                    existingCustomer = _service.GetByRegistrationNumber(customer.NonIndividualRegistrationNumber);
                    if (existingCustomer != null)
                    {
                        duplicateCheckMessage = $"Corporate customer with Registration Number '{customer.NonIndividualRegistrationNumber}' already exists";
                    }
                }
                else if (customer.Type == 2 || customer.Type == 4) // Partnership or MicroCredit
                {
                    if (string.IsNullOrEmpty(customer.NonIndividualRegistrationNumber))
                    {
                        return Content(System.Net.HttpStatusCode.BadRequest,
                                       new { success = false, message = "Registration number is required for this customer type" });
                    }

                    existingCustomer = _service.GetByRegistrationNumber(customer.NonIndividualRegistrationNumber);
                    if (existingCustomer != null)
                    {
                        duplicateCheckMessage = $"Customer with Registration Number '{customer.NonIndividualRegistrationNumber}' already exists";
                    }
                }

                if (existingCustomer != null)
                {
                    return Content(System.Net.HttpStatusCode.Conflict,
                                   new
                                   {
                                       success = false,
                                       message = duplicateCheckMessage,
                                       existingCustomer = new
                                       {
                                           existingCustomer.Id,
                                           Type = existingCustomer.Type,
                                           Name = existingCustomer.Type == 1
                                               ? $"{existingCustomer.IndividualFirstName} {existingCustomer.IndividualLastName}"
                                               : existingCustomer.NonIndividualDescription,
                                           IdentityNumber = existingCustomer.Type == 1
                                               ? existingCustomer.IndividualIdentityCardNumber
                                               : existingCustomer.NonIndividualRegistrationNumber,
                                           existingCustomer.Reference2,
                                           existingCustomer.CreatedDate,
                                           BankName = existingCustomer.BankName,
                                           BranchName = existingCustomer.BranchName
                                       }
                                   });
                }

                var createdCustomer = _service.Create(customer);

                // Create accounts automatically for the customer
                if (createdCustomer != null)
                {
                    try
                    {
                        var customerAccountService = new CustomerAccountService();
                        var createdAccounts = customerAccountService.CreateAccountsForCustomer(
                            createdCustomer.Id,
                            customer.BranchId
                        );

                        // Create next of kins if provided
                        var createdNextOfKins = new System.Collections.Generic.List<NextOfKinDTO>();
                        var nextOfKinErrors = new System.Collections.Generic.List<string>();

                        if (request.NextOfKins != null && request.NextOfKins.Any())
                        {
                            double totalPercentage = 0;

                            // Validate percentages first
                            foreach (var nextOfKin in request.NextOfKins)
                            {
                                nextOfKin.CustomerId = createdCustomer.Id;
                                totalPercentage += nextOfKin.NominatedPercentage;
                            }

                            if (totalPercentage > 100)
                            {
                                return Content(System.Net.HttpStatusCode.BadRequest,
                                               new
                                               {
                                                   success = false,
                                                   message = $"Total next of kin percentage ({totalPercentage:0.##}%) exceeds 100%",
                                                   totalPercentage
                                               });
                            }

                            // Create next of kins
                            foreach (var nextOfKin in request.NextOfKins)
                            {
                                try
                                {
                                    var createdNextOfKin = _nextOfKinService.Create(nextOfKin);
                                    createdNextOfKins.Add(createdNextOfKin);
                                }
                                catch (Exception nextOfKinEx)
                                {
                                    nextOfKinErrors.Add($"Error creating next of kin {nextOfKin.FirstName} {nextOfKin.LastName}: {nextOfKinEx.Message}");
                                }
                            }
                        }

                        var percentageSummary = _nextOfKinService.GetPercentageSummary(createdCustomer.Id);

                        // Send welcome SMS to the customer
                        bool smsSent = false;
                        string smsResponse = "";

                        try
                        {
                            string fullName = $"{createdCustomer.IndividualFirstName ?? ""} {createdCustomer.IndividualLastName ?? ""}".Trim();

                            if (string.IsNullOrEmpty(fullName))
                            {
                                fullName = createdCustomer.FullName ?? "Valued Customer";
                            }

                            if (string.IsNullOrEmpty(fullName))
                            {
                                fullName = $"{customer.IndividualFirstName ?? ""} {customer.IndividualLastName ?? ""}".Trim();
                            }

                            System.Diagnostics.Debug.WriteLine($"SMS - Phone: {customer.AddressMobileLine}, Name: '{fullName}', MemberNo: '{createdCustomer.Reference2 ?? "N/A"}'");

                            smsSent = await SmsHelper.SendWelcomeSmsAsync(
                                customer.AddressMobileLine,
                                fullName,
                                createdCustomer.Reference2 ?? customer.Reference2 ?? "N/A"
                            );

                            if (smsSent)
                            {
                                System.Diagnostics.Debug.WriteLine($"Successfully sent welcome SMS to {customer.AddressMobileLine}");
                                smsResponse = "SMS sent successfully";
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to send welcome SMS to {customer.AddressMobileLine}");
                                smsResponse = "SMS failed to send";
                            }
                        }
                        catch (Exception smsEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"SMS exception for {customer.AddressMobileLine}: {smsEx.Message}");
                            smsResponse = $"SMS error: {smsEx.Message}";
                        }

                        return Content(System.Net.HttpStatusCode.Created,
                                       new
                                       {
                                           success = true,
                                           message = "Customer created successfully with accounts and next of kins",
                                           sms = new
                                           {
                                               sent = smsSent,
                                               phone = customer.AddressMobileLine,
                                               response = smsResponse
                                           },
                                           data = new
                                           {
                                               customer = createdCustomer,
                                               accounts = createdAccounts,
                                               accountCount = createdAccounts.Count(),
                                               nextOfKins = createdNextOfKins,
                                               nextOfKinCount = createdNextOfKins.Count,
                                               nextOfKinErrors = nextOfKinErrors,
                                               percentageSummary = percentageSummary
                                           }
                                       });
                    }
                    catch (Exception accountEx)
                    {
                        // Customer was created but accounts failed
                        System.Diagnostics.Debug.WriteLine($"Customer created but accounts failed: {accountEx.Message}");

                        return Content(System.Net.HttpStatusCode.Created,
                                       new
                                       {
                                           success = true,
                                           message = "Customer created successfully, but account creation failed",
                                           data = createdCustomer,
                                           warning = accountEx.Message
                                       });
                    }
                }

                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = "Failed to create customer" });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("check-duplicate/{idType}/{idNumber}")]
        public IHttpActionResult CheckDuplicateCustomer(string idType, string idNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(idNumber))
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "ID number is required" });

                CustomerDTO existingCustomer = null;
                string message = "";

                if (idType.ToLower() == "individual")
                {
                    existingCustomer = _service.GetByIdentityCardNumber(idNumber);
                    message = existingCustomer != null
                        ? $"Individual customer with ID Card Number '{idNumber}' already exists"
                        : $"No individual customer found with ID Card Number '{idNumber}'";
                }
                else if (idType.ToLower() == "corporate" || idType.ToLower() == "registration")
                {
                    existingCustomer = _service.GetByRegistrationNumber(idNumber);
                    message = existingCustomer != null
                        ? $"Corporate customer with Registration Number '{idNumber}' already exists"
                        : $"No corporate customer found with Registration Number '{idNumber}'";
                }
                else
                {
                    // Check both
                    existingCustomer = _service.GetByIdentityCardNumber(idNumber) ??
                                       _service.GetByRegistrationNumber(idNumber);
                    message = existingCustomer != null
                        ? $"Customer with ID/Registration Number '{idNumber}' already exists"
                        : $"No customer found with ID/Registration Number '{idNumber}'";
                }

                var exists = existingCustomer != null;

                return ApiResponse(true, message, new
                {
                    exists,
                    idType,
                    idNumber,
                    customer = exists ? new
                    {
                        existingCustomer.Id,
                        Type = existingCustomer.Type,
                        Name = existingCustomer.Type == 1
                            ? $"{existingCustomer.IndividualFirstName} {existingCustomer.IndividualLastName}"
                            : existingCustomer.NonIndividualDescription,
                        IdentityNumber = existingCustomer.Type == 1
                            ? existingCustomer.IndividualIdentityCardNumber
                            : existingCustomer.NonIndividualRegistrationNumber,
                        existingCustomer.Reference2,
                        existingCustomer.CreatedDate,
                        BankName = existingCustomer.BankName,
                        BranchName = existingCustomer.BranchName
                    } : null
                });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-bank/{bankName}")]
        public IHttpActionResult GetByBankName(string bankName)
        {
            try
            {
                var customers = _service.GetByBankName(bankName);
                return ApiResponse(true, "Customers retrieved successfully by bank name", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("by-branch/{branchName}")]
        public IHttpActionResult GetByBranchName(string branchName)
        {
            try
            {
                var customers = _service.GetByBranchName(branchName);
                return ApiResponse(true, "Customers retrieved successfully by branch name", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("banks/distinct")]
        public IHttpActionResult GetDistinctBankNames()
        {
            try
            {
                var banks = _service.GetDistinctBankNames();
                return ApiResponse(true, "Distinct bank names retrieved successfully", banks);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("branches/distinct/{bankName}")]
        public IHttpActionResult GetDistinctBranchNames(string bankName)
        {
            try
            {
                if (string.IsNullOrEmpty(bankName))
                    return Content(System.Net.HttpStatusCode.BadRequest,
                                   new { success = false, message = "Bank name is required" });

                var branches = _service.GetDistinctBranchNames(bankName);
                return ApiResponse(true, "Distinct branch names retrieved successfully", branches);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] CustomerDTO customer)
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

                // Check if customer exists
                var existingCustomer = _service.GetById(id);
                if (existingCustomer == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Customer not found" });

                // Set ModifiedBy if not provided
                if (string.IsNullOrEmpty(customer.ModifiedBy))
                {
                    // You might want to get this from the authenticated user
                    customer.ModifiedBy = "System";
                }

                // Update the customer
                _service.Update(customer);

                // Get updated customer with next of kin info
                var updatedCustomer = _service.GetById(id);
                var nextOfKins = _nextOfKinService.GetByCustomerId(id);
                var percentageSummary = _nextOfKinService.GetPercentageSummary(id);

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
            catch (InvalidOperationException ex) // Duplicate validation error
            {
                return Content(System.Net.HttpStatusCode.Conflict,
                               new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex) // Customer not found
            {
                return Content(System.Net.HttpStatusCode.NotFound,
                               new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpDelete, Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                // Delete next of kins first
                _nextOfKinService.DeleteByCustomerId(id);

                // Then delete customer
                _service.Delete(id);

                return ApiResponse(true, "Customer and associated next of kins deleted successfully");
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("search")]
        public IHttpActionResult Search([FromUri] string query)
        {
            try
            {
                var customers = _service.Search(query);
                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }

        [HttpGet, Route("debug-branch-company/{branchId:guid}")]
        public IHttpActionResult DebugBranchCompany(Guid branchId)
        {
            try
            {
                var branchService = new BranchService();
                var branch = branchService.GetById(branchId);

                if (branch == null)
                    return Content(System.Net.HttpStatusCode.NotFound,
                                   new { success = false, message = "Branch not found" });

                var companyAttachedProductService = new CompanyAttachedProductService();
                var attachedProducts = companyAttachedProductService.GetByCompanyId(branch.CompanyId);

                return ApiResponse(true, "Branch and company details retrieved", new
                {
                    branch = new
                    {
                        branch.Id,
                        branch.Code,
                        branch.Description,
                        branch.CompanyId,
                        branch.CompanyDescription
                    },
                    attachedProductsCount = attachedProducts.Count(),
                    attachedProducts = attachedProducts.Select(ap => new
                    {
                        ap.Id,
                        ap.CompanyId,
                        ap.TargetProductId,
                        ap.ProductCode
                    })
                });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError,
                               new { success = false, message = ex.Message });
            }
        }
    }

    public class CustomerWithNextOfKinsDTO
    {
        public CustomerDTO Customer { get; set; }
        public System.Collections.Generic.List<NextOfKinDTO> NextOfKins { get; set; }
    }
}