using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    // Fulfillment endpoints for an already-linked, PIN-authenticated WhatsApp Banking session -
    // WORKFLOW.md §7 and docs/api/whatsapp-banking-api-spec.md §6-7. Every action requires a
    // valid X-WhatsApp-Session header (issued by IdentityController.AuthenticateWithPin/ResetPin).
    [Authorize]
    [RoutePrefix("api/whatsappbanking")]
    public class TransactionsController : ApiController
    {
        private readonly WhatsAppBankingTokenStore _tokenStore;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IAlternateChannelAppService _alternateChannelAppService;
        private readonly IBankToMobileRequestAppService _bankToMobileRequestAppService;

        public TransactionsController(
            WhatsAppBankingTokenStore tokenStore,
            ICustomerAccountAppService customerAccountAppService,
            IAlternateChannelAppService alternateChannelAppService,
            IBankToMobileRequestAppService bankToMobileRequestAppService)
        {
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
            _bankToMobileRequestAppService = bankToMobileRequestAppService ?? throw new ArgumentNullException(nameof(bankToMobileRequestAppService));
        }

        private WhatsAppBankingSession RequireSession()
        {
            if (!Request.Headers.TryGetValues("X-WhatsApp-Session", out var values))
                return null;

            return _tokenStore.GetSession(values.FirstOrDefault());
        }

        private IHttpActionResult SessionExpired()
        {
            return Content(HttpStatusCode.Unauthorized, new { success = false, message = "Session expired or invalid", data = (object)null });
        }

        [HttpGet]
        [Route("accounts")]
        public async Task<IHttpActionResult> GetAccounts()
        {
            try
            {
                var session = RequireSession();
                if (session == null) return SessionExpired();

                var serviceHeader = Utils.CreateServiceHeader();

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(session.CustomerId, serviceHeader) ?? new List<CustomerAccountDTO>();

                _customerAccountAppService.FetchCustomerAccountsProductDescription(accounts, serviceHeader, true);

                return Ok(new
                {
                    success = true,
                    message = "",
                    data = accounts.Select(a => new { id = a.Id, accountNumber = a.FullAccountNumber, productDescription = a.CustomerAccountTypeTargetProductDescription })
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Looks up whether AlternateChannelKnownChargeType.BalanceInquiryCharges is configured for
        // WhatsAppBanking and reports it via feeApplicable - still lookup-only, not posted. Unlike
        // when this comment was first written, a real "compute + post" primitive now exists and is
        // used by RequestPayout (withdrawal) and MobileToBankRequestAppService (deposit) below -
        // ICommissionAppService.ComputeTariffsByAlternateChannelType. Balance inquiry doesn't debit
        // anything to attach a fee journal to, so it wasn't wired the same way here; if
        // BalanceInquiryCharges needs to actually be charged, it'd need its own debit-something
        // decision (e.g. a standalone fee-only journal against the account), not just this call.
        [HttpGet]
        [Route("accounts/{accountId:guid}/balance")]
        public async Task<IHttpActionResult> GetBalance(Guid accountId)
        {
            try
            {
                var session = RequireSession();
                if (session == null) return SessionExpired();

                var serviceHeader = Utils.CreateServiceHeader();

                var account = _customerAccountAppService.FindCustomerAccountDTO(accountId, serviceHeader);

                if (account == null || account.CustomerId != session.CustomerId)
                    return NotFound();

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { account }, serviceHeader);

                var fee = _alternateChannelAppService.FindCachedCommissions((int)AlternateChannelType.WhatsAppBanking, (int)AlternateChannelKnownChargeType.BalanceInquiryCharges, serviceHeader)?.FirstOrDefault();

                return Ok(new
                {
                    success = true,
                    message = "",
                    data = new
                    {
                        accountId = account.Id,
                        accountNumber = account.FullAccountNumber,
                        availableBalance = account.AvailableBalance,
                        feeApplicable = fee != null
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Not a money-movement call - WhatsApp Banking doesn't pull funds itself. Tells the
        // customer how to pay; the provider's own confirmation is expected to reach the C2B
        // webhook (Areas/WhatsAppBanking/Controllers/DepositWebhookController), which matches by
        // MSISDN against the AlternateChannel link created via POST link and posts the deposit
        // journal for real. See docs/api/whatsapp-banking-api-spec.md §7.1 for the full picture,
        // including what still needs to be true of the provider integration for this to be real
        // end-to-end (the webhook existing is necessary but not sufficient - it also needs to be
        // registered with the actual mobile money provider).
        [HttpGet]
        [Route("deposits/instructions")]
        public async Task<IHttpActionResult> GetDepositInstructions()
        {
            try
            {
                var session = RequireSession();
                if (session == null) return SessionExpired();

                var businessShortCode = DefaultSettings.Instance.MobileMoneyPaybillBusinessShortCode;

                if (string.IsNullOrWhiteSpace(businessShortCode))
                    return InternalServerError(new InvalidOperationException("Mobile money Paybill/business shortcode is not configured (DefaultSettings.Instance.MobileMoneyPaybillBusinessShortCode)."));

                return Ok(new
                {
                    success = true,
                    message = "",
                    data = new
                    {
                        method = "MobileMoneyPaybill",
                        businessShortCode,
                        accountReference = session.PhoneNumber,
                        note = "Use your linked WhatsApp Banking number as the account/reference."
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Real money movement: debits the account and posts a real journal via
        // IBankToMobileRequestAppService.RequestPayout (Debit customer's product G/L, Credit
        // SystemGeneralLedgerAccountCode.MobileWalletB2CSettlement) - see that method's own
        // comments for why this does NOT reuse AddNewBankToMobileRequest (it does not debit or
        // post anything despite the name). RequestPayout also now computes and posts any
        // configured WithdrawalCharges commission for WhatsAppBanking, included in the
        // available-balance check - a BadRequest below can mean the amount plus fee together
        // exceed the balance, not just the amount alone. The success message below is
        // deliberately honest about what happens next: SwiftFinancials.BankToMobileHostInterface
        // (the process that would actually pay the customer out over mobile money) is still an
        // empty, unimplemented stub - the debit is real, the payout is not automated yet.
        [HttpPost]
        [Route("withdrawals")]
        public async Task<IHttpActionResult> RequestWithdrawal(WithdrawalRequest request)
        {
            try
            {
                var session = RequireSession();
                if (session == null) return SessionExpired();

                if (request == null || request.AccountId == Guid.Empty || request.Amount <= 0m)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "accountId and a positive amount are required", data = (object)null });

                var serviceHeader = Utils.CreateServiceHeader();

                var account = _customerAccountAppService.FindCustomerAccountDTO(request.AccountId, serviceHeader);

                if (account == null || account.CustomerId != session.CustomerId)
                    return NotFound();

                var link = _alternateChannelAppService.FindAlternateChannel(session.AlternateChannelId, serviceHeader);

                if (link != null && request.Amount > link.DailyLimit)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Format("Amount exceeds your daily limit of {0}", link.DailyLimit), data = (object)null });

                var payout = _bankToMobileRequestAppService.RequestPayout(account.Id, request.Amount, session.PhoneNumber, (int)AlternateChannelType.WhatsAppBanking, serviceHeader);

                if (payout == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Withdrawal could not be processed - check that the account has sufficient available balance.", data = (object)null });

                return Ok(new
                {
                    success = true,
                    message = "Your account has been debited. Payout to your mobile money number is not yet automated - back office will process it manually.",
                    data = new { bankToMobileRequestId = payout.Id, status = payout.StatusDescription }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class WithdrawalRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
