using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.WhatsAppBanking.Controllers
{
    // Onboarding (new customer + mandatory accounts) and channel linking for WhatsApp Banking -
    // WORKFLOW.md §6 (onboarding) and §5.4-equivalent (linking). Both actions require a
    // phoneVerifiedToken from IdentityController.VerifyOtp - phone control must be proven
    // before either a Customer row or an AlternateChannel link gets created.
    [Authorize]
    [RoutePrefix("api/whatsappbanking")]
    public class RegistrationController : ApiController
    {
        private readonly WhatsAppBankingTokenStore _tokenStore;
        private readonly ICustomerAppService _customerAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly ICompanyAppService _companyAppService;
        private readonly IAlternateChannelAppService _alternateChannelAppService;

        public RegistrationController(
            WhatsAppBankingTokenStore tokenStore,
            ICustomerAppService customerAppService,
            ICustomerAccountAppService customerAccountAppService,
            IBranchAppService branchAppService,
            ICompanyAppService companyAppService,
            IAlternateChannelAppService alternateChannelAppService)
        {
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
            _companyAppService = companyAppService ?? throw new ArgumentNullException(nameof(companyAppService));
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
        }

        // Internally: POST-equivalent of api/registry/customer (Individual, branchId =
        // DefaultSettings.Instance.DigitalChannelBranchId - must be configured by ops first, see
        // docs/api/whatsapp-banking-api-spec.md), then the same bulk-create sequence
        // CustomerAccountsController.CreateAccountsForCustomer uses (mandatory products resolved
        // from the branch's company - no product picker, self-onboarding shouldn't need one).
        // Does NOT link the channel yet - proceed to POST link.
        [HttpPost]
        [Route("customer")]
        public async Task<IHttpActionResult> Register(RegisterCustomerRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneVerifiedToken) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneVerifiedToken, firstName and lastName are required", data = (object)null });

                // Not consumed here - Link (below) needs the same token right after Register in
                // the intended flow.
                var phoneNumber = _tokenStore.GetPhoneForVerifiedToken(request.PhoneVerifiedToken, consume: false);

                if (string.IsNullOrEmpty(phoneNumber))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid or expired phoneVerifiedToken", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                // Refuse to create a duplicate Customer if this number already resolves to one -
                // the bot should have routed here only when otp/verify said isExistingCustomer:
                // false, but this is enforced server-side, not just assumed of a well-behaved caller.
                var existingCustomerId = await WhatsAppBankingCustomerLookup.FindExistingCustomerIdAsync(phoneNumber, _alternateChannelAppService, _customerAppService, serviceHeader);

                if (existingCustomerId.HasValue)
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "This number belongs to an existing customer - use POST link with one of their existing accounts instead of registering.", data = (object)null });

                var branchId = DefaultSettings.Instance.DigitalChannelBranchId;

                if (branchId == Guid.Empty)
                    return InternalServerError(new InvalidOperationException("Digital Channel Branch is not configured (DefaultSettings.Instance.DigitalChannelBranchId) - back office must create a Branch for self-onboarded customers and set this before registration can work."));

                var branch = _branchAppService.FindBranch(branchId, serviceHeader);

                if (branch == null)
                    return InternalServerError(new InvalidOperationException("The configured Digital Channel Branch was not found - DefaultSettings.Instance.DigitalChannelBranchId points at a Branch that no longer exists."));

                var customerDTO = new CustomerDTO
                {
                    BranchId = branchId,
                    Type = (byte)CustomerType.Individual,
                    IndividualFirstName = request.FirstName,
                    IndividualLastName = request.LastName,
                    IndividualIdentityCardType = (byte)request.IdentityCardType,
                    IndividualIdentityCardNumber = request.IdentityCardNumber,
                    IndividualGender = (byte)request.Gender,
                    IndividualBirthDate = request.BirthDate,
                    AddressMobileLine = phoneNumber
                };

                var createdCustomer = await _customerAppService.AddNewCustomerAsync(customerDTO, new List<DebitTypeDTO>(), new List<InvestmentProductDTO>(), new List<SavingsProductDTO>(), (int)SystemTransactionCode.CustomerRegistration, serviceHeader);

                if (createdCustomer == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Could not create customer", data = (object)null });

                createdCustomer.BranchId = branchId;

                var attachedProducts = _companyAppService.FindCachedAttachedProducts(branch.CompanyId, serviceHeader);

                _customerAccountAppService.AddNewCustomerAccounts(createdCustomer, attachedProducts?.SavingsProductCollection, attachedProducts?.InvestmentProductCollection, attachedProducts?.LoanProductCollection, serviceHeader);

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(createdCustomer.Id, serviceHeader) ?? new List<CustomerAccountDTO>();

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Customer created",
                    data = new
                    {
                        id = createdCustomer.Id,
                        firstName = createdCustomer.IndividualFirstName,
                        lastName = createdCustomer.IndividualLastName,
                        accounts = accounts.Select(a => new { id = a.Id, accountNumber = a.FullAccountNumber })
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Internally: AlternateChannelDTO, Type = WhatsAppBanking, CardNumber = the verified
        // phone number, MobilePIN = pin (hashed inside AddNewAlternateChannel), DailyLimit =
        // DefaultSettings.Instance.AlternateChannelsDefaultDailyLimit (back-office-configured
        // default, not client-supplied), RecordStatus starts New. Authorization: the target
        // accountId must belong to the SAME customer this phone number resolves to - checked
        // explicitly, not assumed just because the caller supplied a phoneVerifiedToken (that
        // only proves phone control, not which account it should be allowed to touch).
        [HttpPost]
        [Route("link")]
        public async Task<IHttpActionResult> Link(LinkRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneVerifiedToken) || request.AccountId == Guid.Empty || string.IsNullOrWhiteSpace(request.Pin))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "phoneVerifiedToken, accountId and pin are required", data = (object)null });

                var phoneNumber = _tokenStore.GetPhoneForVerifiedToken(request.PhoneVerifiedToken, consume: true);

                if (string.IsNullOrEmpty(phoneNumber))
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid or expired phoneVerifiedToken", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _alternateChannelAppService.FindAlternateChannelsByCardNumberAndCardType(phoneNumber, (int)AlternateChannelType.WhatsAppBanking, serviceHeader);

                if (existing != null && existing.Any())
                    return Content(HttpStatusCode.Conflict, new { success = false, message = "This number is already linked for WhatsApp Banking", data = (object)null });

                var customerId = await WhatsAppBankingCustomerLookup.FindExistingCustomerIdAsync(phoneNumber, _alternateChannelAppService, _customerAppService, serviceHeader);

                if (!customerId.HasValue)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "No customer found for this phone number - register first", data = (object)null });

                var account = _customerAccountAppService.FindCustomerAccountDTO(request.AccountId, serviceHeader);

                if (account == null)
                    return NotFound();

                if (account.CustomerId != customerId.Value)
                    return Content(HttpStatusCode.Forbidden, new { success = false, message = "This account does not belong to the verified customer", data = (object)null });

                var alternateChannelDTO = new AlternateChannelDTO
                {
                    CustomerAccountId = account.Id,
                    Type = (int)AlternateChannelType.WhatsAppBanking,
                    CardNumber = phoneNumber,
                    MobilePIN = request.Pin,
                    ValidFrom = DateTime.Now,
                    Expires = DateTime.Now.AddYears(10),
                    DailyLimit = DefaultSettings.Instance.AlternateChannelsDefaultDailyLimit,
                    Remarks = "Linked via WhatsApp Banking self-service"
                };

                alternateChannelDTO.ValidateAll();

                if (alternateChannelDTO.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", alternateChannelDTO.ErrorMessages), data = (object)null });

                var created = _alternateChannelAppService.AddNewAlternateChannel(alternateChannelDTO, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Could not create the link", data = (object)null });

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, new { success = false, message = created.ErrorMessageResult, data = (object)null });

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Submitted for approval - we'll confirm once your WhatsApp Banking link is active.",
                    data = new { id = created.Id, recordStatus = created.RecordStatus }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class RegisterCustomerRequest
    {
        public string PhoneVerifiedToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdentityCardType { get; set; }
        public string IdentityCardNumber { get; set; }
        public int Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class LinkRequest
    {
        public string PhoneVerifiedToken { get; set; }
        public Guid AccountId { get; set; }
        public string Pin { get; set; }
    }
}
