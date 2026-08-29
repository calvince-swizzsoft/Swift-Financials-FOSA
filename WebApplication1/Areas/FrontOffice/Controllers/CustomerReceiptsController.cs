using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC CustomerReceiptsController — a free-form GL
    // voucher entry at the till, not tied to a specific customer account
    // transaction type (§9 of WORKFLOW.md).
    //
    // KNOWN GAP, not silently dropped: the reference controller posted via
    // _channelService.AddJournalWithApportionmentsAsync, splitting one receipt
    // total across multiple chart-of-account lines (an Apportionments collection
    // on TransactionModel). IJournalAppService in this repo has no apportioned-
    // posting overload — only single debit/credit-pair AddNewJournal methods
    // (checked: Application.MainBoundedContext/AccountsModule/Services/
    // IJournalAppService.cs). Adding real multi-line apportionment support is
    // app-service/domain work, not a controller-layer port, so this endpoint
    // posts a single-line receipt (one chart of account) for now. Multi-line
    // apportionment needs a deliberate follow-up on IJournalAppService before
    // this controller can support it.
    [Authorize]
    [RoutePrefix("api/frontoffice/customerreceipts")]
    public class CustomerReceiptsController : ApiController
    {
        private readonly IJournalAppService _journalAppService;
        private readonly ITellerAppService _tellerAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public CustomerReceiptsController(
            IJournalAppService journalAppService,
            ITellerAppService tellerAppService,
            IBranchAppService branchAppService,
            IPostingPeriodAppService postingPeriodAppService)
        {
            _journalAppService = journalAppService ?? throw new ArgumentNullException(nameof(journalAppService));
            _tellerAppService = tellerAppService ?? throw new ArgumentNullException(nameof(tellerAppService));
            _branchAppService = branchAppService ?? throw new ArgumentNullException(nameof(branchAppService));
            _postingPeriodAppService = postingPeriodAppService ?? throw new ArgumentNullException(nameof(postingPeriodAppService));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CustomerReceiptRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required");

            if (request.ChartOfAccountId == Guid.Empty || request.TotalValue <= 0)
                return BadRequest("chartOfAccountId and a positive totalValue are required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var teller = GetCurrentTeller(serviceHeader);

                if (teller == null || teller.IsLocked)
                    return BadRequest("Current user has no linked, unlocked teller record.");

                var tellerLimitError = _tellerAppService.ValidateCashMovement(
                    teller.Id,
                    request.TotalValue,
                    true,
                    serviceHeader);

                if (!string.IsNullOrWhiteSpace(tellerLimitError))
                    return BadRequest(tellerLimitError);

                var branch = _branchAppService.FindBranch(teller.EmployeeBranchId, serviceHeader);
                var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                var secondaryDescription = string.Format("B{0}/T{1}/#{2}", branch.Code, teller.Code, teller.ItemsCount);

                var journal = _journalAppService.AddNewJournal(
                    teller.EmployeeBranchId,
                    null,
                    request.TotalValue,
                    request.PrimaryDescription ?? "ok",
                    secondaryDescription,
                    request.Reference,
                    request.ModuleNavigationItemCode,
                    (int)SystemTransactionCode.GeneralCashReceipt,
                    DateTime.Today,
                    request.ChartOfAccountId,
                    teller.ChartOfAccountId ?? Guid.Empty,
                    serviceHeader);

                if (journal == null)
                    return BadRequest("Failed to post the receipt");

                return Ok(new { success = true, message = "Operation success", data = journal });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TellerDTO GetCurrentTeller(ServiceHeader serviceHeader)
        {
            var employeeIdClaim = (HttpContext.Current?.User as ClaimsPrincipal)?.FindFirst("EmployeeId");

            if (employeeIdClaim == null || !Guid.TryParse(employeeIdClaim.Value, out var employeeId))
                throw new InvalidOperationException("Current user has no linked employee/teller record.");

            return _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);
        }
    }

    public class CustomerReceiptRequest
    {
        public Guid ChartOfAccountId { get; set; }

        public decimal TotalValue { get; set; }

        public string Reference { get; set; }

        public string PrimaryDescription { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }
}
