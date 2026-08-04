using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [RoutePrefix("api/accounts/statements/customer-account")]
    public class CustomerAccountStatementController : ApiController
    {
        private readonly ICustomerAccountAppService _customerAccountAppService;

        private readonly IJournalEntryAppService _journalEntryAppService;

        private readonly IMediaAppService _mediaAppService;

        public CustomerAccountStatementController(
            ICustomerAccountAppService customerAccountAppService,
            IJournalEntryAppService journalEntryAppService,
            IMediaAppService mediaAppService)
        {
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _journalEntryAppService = journalEntryAppService ?? throw new ArgumentNullException(nameof(journalEntryAppService));
            _mediaAppService = mediaAppService ?? throw new ArgumentNullException(nameof(mediaAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        // Last N days / last N transactions — the teller "print mini statement" view.
        [HttpGet, Route("{customerAccountId:guid}/mini")]
        public IHttpActionResult GetMiniStatement(
            Guid customerAccountId,
            [FromUri] int lastXDays = 90,
            [FromUri] int lastXItems = 20,
            [FromUri] bool tallyDebitsCredits = true)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var statement = _journalEntryAppService.FindLastXGeneralLedgerTransactionsByCustomerAccountId(customerAccount, lastXDays, lastXItems, tallyDebitsCredits, serviceHeader);

                return ApiResponse(true, "Mini statement retrieved successfully", statement);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Full statement for an arbitrary date range, paged.
        [HttpGet, Route("{customerAccountId:guid}")]
        public IHttpActionResult GetStatement(
            Guid customerAccountId,
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] int pageIndex = 0,
            [FromUri] int pageSize = 20,
            [FromUri] string text = "",
            [FromUri] int journalEntryFilter = 0,
            [FromUri] bool tallyDebitsCredits = true)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
                if (customerAccount == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

                var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
                var effectiveEndDate = endDate ?? DateTime.Today;

                if (effectiveStartDate > effectiveEndDate)
                    return ErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be after endDate");

                var statement = _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRangeInPage(
                    pageIndex, pageSize, customerAccount, effectiveStartDate, effectiveEndDate, text, journalEntryFilter, tallyDebitsCredits, serviceHeader);

                return ApiResponse(true, "Statement retrieved successfully", statement);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // Rendered PDF for the date range — same report the teller counter prints today.
        [HttpGet, Route("{customerAccountId:guid}/print")]
        public HttpResponseMessage PrintStatement(
            Guid customerAccountId,
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] bool chargeForPrinting = false,
            [FromUri] bool includeInterestStatement = false,
            [FromUri] int moduleNavigationItemCode = 0)
        {
            var serviceHeader = Utils.CreateServiceHeader();

            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);
            if (customerAccount == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer account not found");

            var effectiveStartDate = startDate ?? DateTime.Today.AddMonths(-1);
            var effectiveEndDate = endDate ?? DateTime.Today;

            if (effectiveStartDate > effectiveEndDate)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "startDate cannot be after endDate");

            try
            {
                var blobDatabaseConnectionString = ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString;

                var media = _mediaAppService.PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(
                    customerAccount, effectiveStartDate, effectiveEndDate, chargeForPrinting, includeInterestStatement, moduleNavigationItemCode, blobDatabaseConnectionString, serviceHeader);

                if (media == null || media.Content == null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to generate statement PDF");

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(media.Content);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(media.ContentType) ? "application/pdf" : media.ContentType);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.IsNullOrEmpty(media.FileName) ? $"Statement_{customerAccountId}_{effectiveStartDate:yyyyMMdd}_{effectiveEndDate:yyyyMMdd}.pdf" : media.FileName
                };

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
