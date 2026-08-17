using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System.Collections.Generic;
using System.Linq;
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
    // current balances) feeding one real submit. The generic customer-
    // accounts endpoint has no product-code filter, so this controller
    // exposes a loan-only paged account lookup alongside the operation:
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
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public LoanRestructuringController(ILoanCaseAppService loanCaseAppService, ICustomerAccountAppService customerAccountAppService, IAuthorizationAppService authorizationAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        [HttpGet]
        [Route("accounts")]
        public IHttpActionResult Accounts(string text = "", int customerFilter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var page = _customerAccountAppService.FindCustomerAccountsByProductCode(
                    (int)ProductCode.Loan, text ?? string.Empty, customerFilter, pageIndex, pageSize, serviceHeader);

                return Ok(ApiResponse("Loan accounts retrieved successfully", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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

                var customerAccount = _customerAccountAppService.FindCustomerAccounts(request.CustomerAccountId, serviceHeader);
                if (customerAccount == null)
                    return Content(HttpStatusCode.NotFound, ErrorEnvelope("Customer account not found"));

                if (customerAccount.CustomerAccountTypeProductCode != (int)ProductCode.Loan)
                    return ErrorResponse("Only loan accounts can be restructured");

                var permissionType = customerAccount.CustomerAccountTypeTargetProductLoanProductSection == (int)LoanProductSection.FOSA
                    ? SystemPermissionType.FrontOfficeLoanRestructuring
                    : SystemPermissionType.BackOfficeLoanRestructuring;
                var mappedRoles = _authorizationAppService.GetRolesAndApprovalPriorityByPermissionType((int)permissionType, serviceHeader)
                    ?? new List<SystemPermissionTypeInRoleDTO>();
                var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
                if (mappedRoles.Any() && !mappedRoles.Any(mapping => callerRoles.Any(role => string.Equals(role, mapping.RoleName, StringComparison.OrdinalIgnoreCase))))
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope($"The current user does not hold a role mapped to {permissionType}"));

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
