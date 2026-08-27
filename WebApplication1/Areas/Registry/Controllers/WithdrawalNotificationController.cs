using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    [Authorize, RoutePrefix("api/registry/withdrawal-notifications")]
    public class WithdrawalNotificationController : ApiController
    {
        readonly IWithdrawalNotificationAppService _service;
        readonly ICustomerAccountAppService _accounts;
        readonly ILoanCaseAppService _loans;
        readonly IInsuranceCompanyAppService _insurers;

        public WithdrawalNotificationController(IWithdrawalNotificationAppService service, ICustomerAccountAppService accounts,
            ILoanCaseAppService loans, IInsuranceCompanyAppService insurers)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            _loans = loans ?? throw new ArgumentNullException(nameof(loans));
            _insurers = insurers ?? throw new ArgumentNullException(nameof(insurers));
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get(int? status = null, string text = "")
        {
            var data = _service.FindWithdrawalNotifications(Header()) ?? new List<WithdrawalNotificationDTO>();
            if (status.HasValue) data = data.Where(x => x.Status == status.Value).ToList();
            if (!string.IsNullOrWhiteSpace(text)) data = data.Where(x => Has(x.CustomerFullName, text) || Has(x.CustomerReference2, text) || Has(x.CustomerIndividualIdentityCardNumber, text)).ToList();
            return OkResult("Withdrawal notifications retrieved successfully", data.OrderByDescending(x => x.CreatedDate));
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var data = _service.FindWithdrawalNotification(id, Header());
            return data == null ? Fail(HttpStatusCode.NotFound, "Withdrawal notification not found") : OkResult("Withdrawal notification retrieved successfully", data);
        }

        [HttpGet, Route("customer/{customerId:guid}/position")]
        public IHttpActionResult Position(Guid customerId)
        {
            var header = Header();
            var all = _accounts.FindCustomerAccountsByCustomerId(customerId, header) ?? new List<CustomerAccountDTO>();
            var savings = all.Where(x => x.CustomerAccountTypeProductCode == (int)ProductCode.Savings).ToList();
            var loans = all.Where(x => x.CustomerAccountTypeProductCode == (int)ProductCode.Loan).ToList();
            var investments = all.Where(x => x.CustomerAccountTypeProductCode == (int)ProductCode.Investment).ToList();
            var guarantees = (_loans.FindLoanGuarantorsByCustomerId(customerId, header) ?? new List<LoanGuarantorDTO>())
                .Where(x => x.Status == (int)LoanGuarantorStatus.Attached).ToList();
            var assets = investments.Where(x => x.CustomerAccountTypeTargetProductIsRefundable).Sum(x => Math.Max(0m, x.BookBalance));
            var liabilities = loans.Sum(x => Math.Max(0m, -x.PrincipalBalance) + Math.Max(0m, -x.InterestBalance) + Math.Max(0m, -x.CarryForwardsBalance));
            return OkResult("Customer position retrieved successfully", new { savings, loans, investments, guaranteedLoans = guarantees,
                totalLoansGuaranteed = guarantees.Count, refundableInvestments = assets, loanLiability = liabilities,
                netRefundable = assets - liabilities, branchId = all.Select(x => x.BranchId).FirstOrDefault() });
        }

        [HttpGet, Route("insurance-companies")]
        public IHttpActionResult Insurers() { return OkResult("Insurance companies retrieved successfully", _insurers.FindInsuranceCompanies(Header()) ?? new List<InsuranceCompanyDTO>()); }

        [HttpGet, Route("{id:guid}/settlements")]
        public IHttpActionResult Settlements(Guid id) { return OkResult("Settlements retrieved successfully", _service.FindWithdrawalSettlementsByWithdrawalNotificationId(id, Header()) ?? new List<WithdrawalSettlementDTO>()); }

        [HttpPost, Route("")]
        public IHttpActionResult Create(WithdrawalNotificationDTO dto)
        {
            if (dto == null || dto.CustomerId == Guid.Empty || dto.BranchId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Remarks) || !Enum.IsDefined(typeof(WithdrawalNotificationCategory), dto.Category))
                return Fail(HttpStatusCode.BadRequest, "Customer, branch, category and remarks are required");
            var header = Header();
            if (dto.Category != (int)WithdrawalNotificationCategory.Deceased)
            {
                var all = _accounts.FindCustomerAccountsByCustomerId(dto.CustomerId, header) ?? new List<CustomerAccountDTO>();
                var assets = all.Where(x => x.CustomerAccountTypeProductCode == (int)ProductCode.Investment && x.CustomerAccountTypeTargetProductIsRefundable).Sum(x => Math.Max(0m, x.BookBalance));
                var liabilities = all.Where(x => x.CustomerAccountTypeProductCode == (int)ProductCode.Loan).Sum(x => Math.Max(0m, -x.PrincipalBalance) + Math.Max(0m, -x.InterestBalance) + Math.Max(0m, -x.CarryForwardsBalance));
                if (assets - liabilities < 0m) return Fail(HttpStatusCode.BadRequest, "Net refundable amount must be zero or greater.");
                if ((_loans.FindLoanGuarantorsByCustomerId(dto.CustomerId, header) ?? new List<LoanGuarantorDTO>()).Any(x => x.Status == (int)LoanGuarantorStatus.Attached)) return Fail(HttpStatusCode.BadRequest, "All active loan guarantees must be substituted first.");
            }
            var created = _service.AddNewWithdrawalNotification(dto, header);
            if (created == null || !string.IsNullOrWhiteSpace(created.ErrorMessageResult)) return Fail(HttpStatusCode.BadRequest, created?.ErrorMessageResult ?? "Registration failed");
            return Content(HttpStatusCode.Created, new { success = true, message = "Withdrawal notification registered successfully", data = created });
        }

        [HttpPost, Route("{id:guid}/approval")]
        public IHttpActionResult Approval(Guid id, WorkflowRequest request) { return Workflow(request, r => _service.ApproveWithdrawalNotification(new WithdrawalNotificationDTO { Id = id, ApprovalRemarks = r.Remarks }, r.Option, Header()), "approval", "registered or deferred"); }

        [HttpPost, Route("{id:guid}/verification")]
        public IHttpActionResult Verification(Guid id, WorkflowRequest request) { return Workflow(request, r => _service.AuditWithdrawalNotification(new WithdrawalNotificationDTO { Id = id, AuditRemarks = r.Remarks }, r.Option, Header()), "verification", "approved"); }

        [HttpPost, Route("{id:guid}/settlement")]
        public IHttpActionResult Settlement(Guid id, SettlementRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Remarks)) return Fail(HttpStatusCode.BadRequest, "Settlement remarks are required");
            var dto = _service.FindWithdrawalNotification(id, Header());
            if (dto == null) return Fail(HttpStatusCode.NotFound, "Withdrawal notification not found");
            dto.SettlementRemarks = request.Remarks; dto.SettlementType = request.SettlementType;
            return _service.SettleWithdrawalNotification(dto, request.Option, 21027, Header()) ? OkResult("Settlement updated successfully") : Fail(HttpStatusCode.Conflict, "Only verified notifications can be settled");
        }

        [HttpPost, Route("{id:guid}/death-claim")]
        public IHttpActionResult DeathClaim(Guid id, DeathClaimRequest request)
        {
            if (request?.InsuranceCompany == null || request.Settlements == null || !request.Settlements.Any()) return Fail(HttpStatusCode.BadRequest, "Insurer and settlements are required");
            var dto = _service.FindWithdrawalNotification(id, Header());
            if (dto == null) return Fail(HttpStatusCode.NotFound, "Withdrawal notification not found");
            return _service.ProcessDeathSettlements(dto, request.Settlements, request.InsuranceCompany, 21028, Header()) ? OkResult("Death claim settled successfully") : Fail(HttpStatusCode.Conflict, "Death claim settlement failed");
        }

        IHttpActionResult Workflow(WorkflowRequest request, Func<WorkflowRequest, bool> action, string name, string required) { if (request == null || string.IsNullOrWhiteSpace(request.Remarks)) return Fail(HttpStatusCode.BadRequest, name + " remarks are required"); return action(request) ? OkResult("Withdrawal " + name + " updated successfully") : Fail(HttpStatusCode.Conflict, "Only " + required + " notifications can enter " + name); }
        static ServiceHeader Header() { return Utils.CreateServiceHeader(); }
        static bool Has(string value, string term) { return value != null && value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0; }
        IHttpActionResult OkResult(string message, object data = null) { return Ok(new { success = true, message, data }); }
        IHttpActionResult Fail(HttpStatusCode status, string message) { return Content(status, new { success = false, message }); }
    }

    public class WorkflowRequest { public int Option { get; set; } public string Remarks { get; set; } }
    public class SettlementRequest : WorkflowRequest { public int SettlementType { get; set; } = 1; public int ModuleNavigationItemCode { get; set; } }
    public class DeathClaimRequest { public InsuranceCompanyDTO InsuranceCompany { get; set; } public List<WithdrawalSettlementDTO> Settlements { get; set; } public int ModuleNavigationItemCode { get; set; } }
}
