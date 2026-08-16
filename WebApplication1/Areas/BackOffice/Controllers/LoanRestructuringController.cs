using Application.MainBoundedContext.BackOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // Adapted from the reference MVC LoanRestructuringController
    // (Areas/Loaning/Controllers/LoanRestructuringController.cs). The
    // reference screen is a picker (customer account -> loan product ->
    // current balances) feeding one real submit; the picker lookups
    // (customer accounts by product code, loan product details) are
    // already covered by CustomerAccountController/LoanProductController
    // elsewhere in this repo, so only the real operation is exposed here:
    // RestructureLoan(branchId, customerAccountId, NPer, Pmt, reference,
    // moduleNavigationItemCode) — a new term (NPer) and payment (Pmt) for
    // an existing loan account, keyed by the loan CustomerAccountId (not a
    // LoanCaseId — restructuring acts on the disbursed loan account
    // itself, unlike every other lifecycle action in this module).
    [Authorize]
    [RoutePrefix("api/backoffice/loanrestructuring")]
    public class LoanRestructuringController : ApiController
    {
        private readonly ILoanCaseAppService _loanCaseAppService;

        public LoanRestructuringController(ILoanCaseAppService loanCaseAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Restructure(RestructureLoanRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (request.BranchId == Guid.Empty)
                return ErrorResponse("BranchId is required");

            if (request.CustomerAccountId == Guid.Empty)
                return ErrorResponse("CustomerAccountId is required");

            if (request.NPer <= 0)
                return ErrorResponse("NPer (number of periods) must be greater than zero");

            if (request.Pmt <= 0)
                return ErrorResponse("Pmt (payment per period) must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.Reference))
                return ErrorResponse("Reference is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var restructured = _loanCaseAppService.RestructureLoan(request.BranchId, request.CustomerAccountId, request.NPer, request.Pmt, request.Reference, request.ModuleNavigationItemCode, serviceHeader);

                if (!restructured)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to restructure the loan — the account may not be found, may not have an outstanding principal balance, or may have an outstanding interest balance"));

                return Ok(ApiResponse("Loan restructured successfully", null));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private object ApiResponse(string message, object data)
        {
            return new { success = true, message, data };
        }

        private object ErrorEnvelope(string message)
        {
            return new { success = false, message, data = (object)null };
        }

        private IHttpActionResult ErrorResponse(string message)
        {
            return Content(HttpStatusCode.BadRequest, ErrorEnvelope(message));
        }
    }

    public class RestructureLoanRequest
    {
        public Guid BranchId { get; set; }
        public Guid CustomerAccountId { get; set; }
        public double NPer { get; set; }
        public double Pmt { get; set; }
        public string Reference { get; set; }
        public int ModuleNavigationItemCode { get; set; }
    }
}
