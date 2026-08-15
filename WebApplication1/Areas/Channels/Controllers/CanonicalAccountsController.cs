using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Channels.Controllers
{
    // Implements the BALANCE and MINI_STATEMENT endpoints of the SwizzChannels canonical
    // financial API contract (see canonical-financial-api-v0.2.md in the SwizzChannels repo),
    // so this system can be registered as an institution on that platform (WhatsApp banking
    // and, later, other channels route through it). Routes and envelope shape are dictated by
    // that contract, not this project's usual { success, message, data } convention:
    // { "success": true, "data": {...} } / { "success": false, "error": { code, message, reference } }.
    //
    // customerReference is the customer's serial number (CustomerDTO.SerialNumber);
    // accountReference is the account's FullAccountNumber. Both are resolved and cross-checked
    // (the account must belong to the resolved customer) before any data is returned.
    //
    // No institution-side call-in authentication yet: SwizzChannels' OAuth2 client-credentials
    // authenticator has nothing to call on this side yet (see SwiftFinancialz CLAUDE.md /
    // conversation notes) — that's deferred, tracked separately from this pass.
    [RoutePrefix("v1/accounts")]
    public class CanonicalAccountsController : ApiController
    {
        // No multi-currency concept exists anywhere in this domain (checked directly — no
        // Currency field on CustomerAccountDTO, no base-currency setting anywhere in the
        // solution). Hardcoded until a real field exists to read from.
        private const string Currency = "KES";

        private readonly ICustomerAppService _customerAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IJournalEntryAppService _journalEntryAppService;

        public CanonicalAccountsController(
            ICustomerAppService customerAppService,
            ICustomerAccountAppService customerAccountAppService,
            IJournalEntryAppService journalEntryAppService)
        {
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _journalEntryAppService = journalEntryAppService ?? throw new ArgumentNullException(nameof(journalEntryAppService));
        }

        private IHttpActionResult Envelope(object data)
        {
            return Ok(new { success = true, data });
        }

        private IHttpActionResult CanonicalError(HttpStatusCode statusCode, string code, string message)
        {
            return Content(statusCode, new { success = false, error = new { code, message } });
        }

        private bool TryResolveAccount(
            string customerReference,
            string accountReference,
            ServiceHeader serviceHeader,
            out CustomerAccountDTO account,
            out IHttpActionResult errorResult)
        {
            account = null;
            errorResult = null;

            if (!int.TryParse(customerReference, out var serialNumber))
            {
                errorResult = CanonicalError(HttpStatusCode.NotFound, "CUSTOMER_NOT_FOUND", "Unknown customerReference.");
                return false;
            }

            var customer = _customerAppService.FindCustomerBySerialNumber(serialNumber, serviceHeader)?.FirstOrDefault();
            if (customer == null)
            {
                errorResult = CanonicalError(HttpStatusCode.NotFound, "CUSTOMER_NOT_FOUND", "Unknown customerReference.");
                return false;
            }

            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(accountReference, serviceHeader);
            if (customerAccount == null || customerAccount.CustomerId != customer.Id)
            {
                errorResult = CanonicalError(HttpStatusCode.NotFound, "ACCOUNT_NOT_FOUND", "Unknown accountReference for this customer.");
                return false;
            }

            // FindCustomerAccountDTO doesn't populate product chart-of-account fields — without
            // this, balance/statement lookups below silently return zero/empty (same gap noted
            // in CustomerAccountStatementController).
            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);

            account = customerAccount;
            return true;
        }

        [HttpPost, Route("balance")]
        public IHttpActionResult Balance([FromBody] CanonicalBalanceRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                if (!TryResolveAccount(request?.CustomerReference, request?.AccountReference, serviceHeader, out var account, out var error))
                    return error;

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { account }, serviceHeader, false, false);

                return Envelope(new
                {
                    accountReference = account.FullAccountNumber,
                    currency = Currency,
                    availableBalance = account.AvailableBalance,
                    ledgerBalance = account.BookBalance,
                    asOf = DateTimeOffset.Now
                });
            }
            catch (Exception ex)
            {
                return CanonicalError(HttpStatusCode.InternalServerError, "SERVICE_UNAVAILABLE", ex.Message);
            }
        }

        [HttpPost, Route("transactions")]
        public IHttpActionResult Transactions([FromBody] CanonicalMiniStatementRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                if (!TryResolveAccount(request?.CustomerReference, request?.AccountReference, serviceHeader, out var account, out var error))
                    return error;

                var limit = request.Limit > 0 ? request.Limit : 10;

                var page = _journalEntryAppService.FindLastXGeneralLedgerTransactionsByCustomerAccountId(account, 90, limit, true, serviceHeader);

                var transactions = (page?.PageCollection ?? new List<GeneralLedgerTransaction>())
                    .Select(line => new
                    {
                        transactionReference = line.JournalId.ToString(),
                        date = new DateTimeOffset(DateTime.SpecifyKind(line.JournalCreatedDate, DateTimeKind.Local)),
                        type = line.Credit > 0 ? "CREDIT" : "DEBIT",
                        description = line.JournalPrimaryDescription,
                        amount = line.Credit > 0 ? line.Credit : line.Debit,
                        currency = Currency,
                        balanceAfter = line.RunningBalance
                    });

                return Envelope(new
                {
                    accountReference = account.FullAccountNumber,
                    transactions
                });
            }
            catch (Exception ex)
            {
                return CanonicalError(HttpStatusCode.InternalServerError, "SERVICE_UNAVAILABLE", ex.Message);
            }
        }
    }

    public sealed class CanonicalBalanceRequest
    {
        public string CustomerReference { get; set; }
        public string AccountReference { get; set; }
    }

    public sealed class CanonicalMiniStatementRequest
    {
        public string CustomerReference { get; set; }
        public string AccountReference { get; set; }
        public int Limit { get; set; }
    }
}
