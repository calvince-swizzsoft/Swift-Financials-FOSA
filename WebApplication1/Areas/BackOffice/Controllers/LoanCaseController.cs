using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
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

namespace WebApplication1.Areas.BackOffice.Controllers
{
    // Adapted from the reference MVC LoanRegistrationController
    // (Areas/Loaning/Controllers/LoanRegistrationController.cs) — see
    // Areas/BackOffice/WORKFLOW.md for the module's functional design and
    // where this fits (§5 intake -> this controller's Create -> §6
    // appraisal, not yet built).
    //
    // What was deliberately NOT ported, and why:
    // - The reference controller is a session/TempData/ViewBag-driven MVC
    //   wizard: guarantors accumulate in Session["loanguarantorsDTOs"]
    //   across separate Add/Remove postbacks before a final Create submit,
    //   and half a dozen actions re-populate ViewBag select lists purely
    //   for server-rendered dropdowns. None of that exists in a stateless
    //   JSON API — Create here takes the whole submission (loan case +
    //   guarantors + collateral document ids) in one request via
    //   CreateLoanCaseRequest instead.
    // - GetDocumentsAsync hand-rolls raw ADO.NET SQL directly against
    //   swiftFin_SpecimenCapture to fetch passport/signature/ID photos, and
    //   LoaneeLookup calls `MessageBox.Show(Form.ActiveForm, ...)` —
    //   System.Windows.Forms UI code that cannot execute in a web request
    //   pipeline at all. Neither is real server logic; identity documents
    //   belong to the existing Customer/CustomerDocument endpoints, not
    //   this controller.
    // - LoaneeLookup's composite "customer 360" view (standing orders,
    //   payouts, in-process loan applications, released collaterals) is
    //   read-model convenience for the old UI, not new business logic —
    //   every piece of it already has (or, for in-process loan applications,
    //   now has here — see CustomerInProcess below) a real endpoint of its
    //   own. Composing a bespoke aggregate endpoint would just duplicate
    //   StandingOrderController/CreditBatchController/etc.
    //
    // What WAS faithfully reproduced, because LoanCaseAppService itself does
    // none of it — AddNewLoanCase only rejects a duplicate in-process
    // application and persists whatever DTO it's handed; every other rule
    // below lives in the reference *controller*, so it has to live here:
    // - The ~40-field loan-product snapshot copied onto the loan case at
    //   registration time (interest rate, term, guarantor/security mode,
    //   take-home, etc. — see CopyLoanProductSnapshot).
    // - Guarantor rules: minimum/maximum guarantor counts, self-guarantee
    //   permission, and per-guarantor share sufficiency.
    // - The minimum-membership-period gate (customer must have been a
    //   member for LoanRegistrationMinimumMembershipPeriod months).
    //
    // Two corrections made against the reference, not just ported forward:
    // - Guarantor share values (TotalShares/CommittedShares/AppraisalFactor)
    //   are computed here from real data (customer accounts, sibling
    //   guarantees, ILoanProductAppService.GetGuarantorAppraisalFactor), not
    //   trusted from the request body — same reasoning as the
    //   InterAccountTransferBatch fix (BATCH-PROCEDURES-CONCEPTS.md): a
    //   client-supplied share balance is not a real security check.
    //   Consequently LoanGuarantorDTO's own ValidateAmountGuaranteed
    //   validator (which correctly multiplies TotalShares by
    //   AppraisalFactor) is the enforced rule here, not the reference
    //   controller's narrower manual check, which never applied the
    //   appraisal factor at all.
    // - The reference Create action calls `loanCaseDTO.ValidateAll();` and
    //   then never checks `HasErrors` — every one of LoanCaseDTO's
    //   CustomValidation rules (security sufficiency, amount-applied range,
    //   retirement age, budget balance) silently runs and is silently
    //   discarded. That's dead validation, not intentional leniency — this
    //   controller checks HasErrors and returns 400 with the real messages.
    // - LoanCaseAppService.UpdateLoanCaseAsync itself had a bug fixed
    //   alongside this controller: two lines directly after restoring
    //   persisted.CreatedDate immediately re-stamped it to DateTime.UtcNow,
    //   and unconditionally set CancelledBy on every plain update, not just
    //   cancellations. Removed — same class of bug as
    //   JournalReversalBatch's UpdateJournalReversalBatch fix.
    //
    // Scope decisions, not gaps to silently paper over:
    // - BranchId is supplied by the caller on the request body. The
    //   reference resolved "current user's branch" via
    //   ApplicationUserManager (ASP.NET Identity, MVC-only); no Web API
    //   controller in this repo does that lookup today, and ServiceHeader
    //   carries no BranchId claim to fall back on.
    // - Branch budget-balance validation (LoanCaseDTO.BranchBudgetBalance /
    //   BranchCompanyEnforceBudgetControl) is left unpopulated, same as the
    //   reference Create action — computing a real branch budget balance is
    //   IBudgetAppService's job and out of scope here; the DTO's
    //   ValidateBudgetBalance rule simply won't fire (defaults to false)
    //   until a caller populates those fields.
    //
    // Appraisal (reference: Areas/Loaning/Controllers/AppraiseLoanController.cs):
    // - Real bug fixed in LoanCaseAppService.AppraiseLoanCase/Async
    //   themselves, same class as the UpdateLoanCaseAsync fix above: the
    //   guard clause force-set persisted.Status to Registered
    //   *unconditionally, immediately after fetching* — before even the
    //   null check — so a missing loan case id threw a
    //   NullReferenceException instead of a clean "not found", and the
    //   "must be Registered or Deferred" precondition was tautologically
    //   always true for any case that did exist. Removed the force-set;
    //   the guard now checks the entity's real status, as WORKFLOW.md §4
    //   already flagged this bug and asked for.
    // - GET .../appraisal-worksheet reproduces the real, computable part of
    //   the reference GET Appraise action: maximum loan via the product's
    //   investments multiplier, outstanding balance on this loan product,
    //   maximum entitled, a simple-interest loan+interest estimate, and a
    //   standard amortization PMT. Not reproduced: the `id` parameter's own
    //   branch (treats a customer id as a loan product id and never uses
    //   the result — dead/buggy in the reference), the "isEmployee" loop
    //   that iterates accounts and does nothing (`foreach { }`, empty
    //   body), and the composite standing-orders/payouts/loan-applications
    //   view-model padding — all real endpoints of their own elsewhere,
    //   same reasoning as LoaneeLookup above.
    // - POST .../appraise takes the appraisal *outcome* fields directly
    //   (not a full LoanCaseDTO staged in session) plus an optional income-
    //   adjustments replace, mirroring the reference POST Appraise action's
    //   two calls (AppraiseLoanCaseAsync + UpdateLoanAppraisalFactorsAsync)
    //   folded into one request.
    //
    // Approval (reference: Areas/Loaning/Controllers/ApproveLoanController.cs):
    // - Same guard-clause bug shape fixed in ApproveLoanCase/Async — see
    //   above. Also fixed in AuditLoanCase/Async (below). Still open in
    //   MarkLoanCaseDisbursed.
    // - The reference Approve action re-copies the same ~40 loan-product
    //   fields Create already snapshots onto the DTO before calling
    //   ApproveLoanCaseAsync — but ApproveLoanCase never reads any of them
    //   off the incoming DTO, only approvedAmount/approvedAmountRemarks/
    //   approvedPrincipalPayment/approvedInterestPayment/
    //   monthlyPaybackAmount/totalPaybackAmount/approvalRemarks and the
    //   persisted entity's own Id/Status. Not reproduced — pure busywork,
    //   the same lesson as "a DTO's fields are not proof of behavior" from
    //   BATCH-PROCEDURES-CONCEPTS.md §5, this time about a *controller*
    //   re-populating fields nobody downstream reads.
    // - The reference also calls loanCaseDTO.ValidateAll() and never checks
    //   the result — same dead-validation-call shape fixed in Create — but
    //   here it's not even reproduced as a no-op: the CustomValidation
    //   rules it would run (amount-applied range, security sufficiency,
    //   retirement age) were already meaningfully enforced once, at Create,
    //   against a fully-populated DTO. Running them again here against a
    //   lean approve-request payload would just produce validation noise
    //   for fields this endpoint doesn't ask for. Real requirements are
    //   checked explicitly instead: approvalRemarks always required,
    //   approvedAmount > 0 required only when Option == Approve (the
    //   reference required a nonzero approvedAmount even to reject/defer,
    //   which reads like unconditional MVC form validation, not a
    //   deliberate business rule).
    //
    // Audit/verification (reference:
    // Areas/Loaning/Controllers/LoanVerificationController.cs, "Verify" in
    // the UI, AuditLoanCase/AuditLoanOption.Audit internally — same status,
    // two names):
    // - Same guard-clause bug shape fixed in AuditLoanCase/Async. This is
    //   the consequential transition (Approved -> Audited): it creates the
    //   customer's loan/savings CustomerAccounts if missing, computes the
    //   repayment PV/PMT off LoanRegistration/LoanInterest, recovers any
    //   upfront dynamic charges, and builds/updates the repayment
    //   StandingOrder — all real, business-critical domain logic, entirely
    //   inside AuditLoanCase itself. Treated as a black box here, same as
    //   LoanDisbursementBatchController treats PostLoanDisbursementBatchEntry.
    // - Same reference pattern as Approve: the reference Verify action
    //   re-copies the ~40 loan-product fields (AuditLoanCase reads none of
    //   them off the DTO — only auditRemarks) and calls ValidateAll()
    //   without checking the result. Neither reproduced, same reasoning as
    //   Approve above. Real requirement: auditRemarks required for every
    //   option (the reference's actual gate, `AuditRemarks != null`).
    [Authorize]
    [RoutePrefix("api/backoffice/loancases")]
    public class LoanCaseController : ApiController
    {
        private readonly ILoanCaseAppService _loanCaseAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly ILoanPurposeAppService _loanPurposeAppService;
        private readonly ILoaningRemarkAppService _loaningRemarkAppService;
        private readonly ICustomerDocumentAppService _customerDocumentAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IIncomeAdjustmentAppService _incomeAdjustmentAppService;
        private readonly IStandingOrderAppService _standingOrderAppService;
        private readonly ICreditBatchAppService _creditBatchAppService;
        private readonly ILoanDisbursementBatchAppService _loanDisbursementBatchAppService;
        private readonly IFileRegisterAppService _fileRegisterAppService;
        private readonly IWorkflowAppService _workflowAppService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public LoanCaseController(
            ILoanCaseAppService loanCaseAppService,
            ICustomerAppService customerAppService,
            ILoanProductAppService loanProductAppService,
            ISavingsProductAppService savingsProductAppService,
            ILoanPurposeAppService loanPurposeAppService,
            ILoaningRemarkAppService loaningRemarkAppService,
            ICustomerDocumentAppService customerDocumentAppService,
            ICustomerAccountAppService customerAccountAppService,
            IIncomeAdjustmentAppService incomeAdjustmentAppService,
            IStandingOrderAppService standingOrderAppService,
            ICreditBatchAppService creditBatchAppService,
            ILoanDisbursementBatchAppService loanDisbursementBatchAppService,
            IFileRegisterAppService fileRegisterAppService,
            IWorkflowAppService workflowAppService,
            IAuthorizationAppService authorizationAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _loanProductAppService = loanProductAppService ?? throw new ArgumentNullException(nameof(loanProductAppService));
            _savingsProductAppService = savingsProductAppService ?? throw new ArgumentNullException(nameof(savingsProductAppService));
            _loanPurposeAppService = loanPurposeAppService ?? throw new ArgumentNullException(nameof(loanPurposeAppService));
            _loaningRemarkAppService = loaningRemarkAppService ?? throw new ArgumentNullException(nameof(loaningRemarkAppService));
            _customerDocumentAppService = customerDocumentAppService ?? throw new ArgumentNullException(nameof(customerDocumentAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _incomeAdjustmentAppService = incomeAdjustmentAppService ?? throw new ArgumentNullException(nameof(incomeAdjustmentAppService));
            _standingOrderAppService = standingOrderAppService ?? throw new ArgumentNullException(nameof(standingOrderAppService));
            _creditBatchAppService = creditBatchAppService ?? throw new ArgumentNullException(nameof(creditBatchAppService));
            _loanDisbursementBatchAppService = loanDisbursementBatchAppService ?? throw new ArgumentNullException(nameof(loanDisbursementBatchAppService));
            _fileRegisterAppService = fileRegisterAppService ?? throw new ArgumentNullException(nameof(fileRegisterAppService));
            _workflowAppService = workflowAppService ?? throw new ArgumentNullException(nameof(workflowAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        // Mirrors the reference Index grid — status is a real, required
        // filter there too (defaults to Registered, the "just opened, not
        // yet appraised" queue).
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(int status = (int)LoanCaseStatus.Registered, string text = "", int loanCaseFilter = 0, int pageIndex = 0, int pageSize = 20, bool includeBatchStatus = true)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanCaseAppService.FindLoanCasesByStatus(status, text ?? "", loanCaseFilter, pageIndex, pageSize, includeBatchStatus, serviceHeader);

                return Ok(ApiResponse("", page));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Mirrors reference Details — loan case plus its guarantors and
        // collaterals in one response, since the reference view always
        // shows all three together.
        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanCase = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                if (loanCase == null)
                    return NotFound();

                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, serviceHeader);
                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, serviceHeader);

                return Ok(ApiResponse("", new
                {
                    loanCase,
                    guarantors = guarantors ?? new List<LoanGuarantorDTO>(),
                    collaterals = collaterals ?? new List<LoanCollateralDTO>()
                }));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/guarantors")]
        public IHttpActionResult GetGuarantors(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, serviceHeader);

                return Ok(ApiResponse("", guarantors ?? new List<LoanGuarantorDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/collaterals")]
        public IHttpActionResult GetCollaterals(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, serviceHeader);

                return Ok(ApiResponse("", collaterals ?? new List<LoanCollateralDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Full-replace of a loan case's attached collateral documents —
        // UpdateLoanCollaterals already existed on ILoanCaseAppService and
        // was only ever called internally from Create; this is the "beyond
        // initial attach" gap. The reference AddCollateralController is
        // dead/mislabeled code (never touches LoanCollateralDTO or any real
        // collateral operation despite its name — see class comment) and
        // was not ported; this exposes the real app-service method
        // directly instead.
        [HttpPut]
        [Route("{id:guid}/collaterals")]
        public IHttpActionResult PutCollaterals(Guid id, List<Guid> documentIds)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanCase = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (loanCase == null)
                    return NotFound();

                var collateralDocuments = new List<CustomerDocumentDTO>();

                foreach (var documentId in documentIds ?? new List<Guid>())
                {
                    var document = _customerDocumentAppService.FindCustomerDocument(documentId, serviceHeader);
                    if (document == null)
                        return ErrorResponse($"Document {documentId} not found");

                    collateralDocuments.Add(document);
                }

                var updated = _loanCaseAppService.UpdateLoanCollaterals(id, collateralDocuments, serviceHeader);
                if (!updated)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Failed to update loan case collaterals"));

                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, serviceHeader);

                return Ok(ApiResponse("Loan case collaterals updated successfully", collaterals ?? new List<LoanCollateralDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Same duplicate-application guard AddNewLoanCase enforces server-
        // side (see class comment) — exposed so the registration screen can
        // warn the loan officer before a submit gets rejected.
        [HttpGet]
        [Route("customers/{customerId:guid}/in-process")]
        public IHttpActionResult CustomerInProcess(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanCases = _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(customerId, serviceHeader);

                return Ok(ApiResponse("", loanCases ?? new List<LoanCaseDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Mirrors reference LoanGuarantorLookUp — resolves a prospective
        // guarantor's real share balance, what they've already committed to
        // other loans, and the loan product's appraisal factor, so the
        // registration screen can show/validate an amount-guaranteed figure
        // before the guarantor is actually attached. Values here are
        // computed the same way Create computes them (see
        // EnrichAndValidateGuarantors) — not just decorative.
        [HttpGet]
        [Route("guarantors/lookup")]
        public IHttpActionResult GuarantorLookup(Guid guarantorId, Guid loanProductId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var guarantor = _customerAppService.FindCustomer(guarantorId, serviceHeader);
                if (guarantor == null)
                    return NotFound();

                var loanProduct = _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);
                if (loanProduct == null)
                    return ErrorResponse("Loan product not found");

                var totalShares = ComputeTotalShares(guarantorId, serviceHeader);
                var committedShares = ComputeCommittedShares(guarantorId, serviceHeader);
                var appraisalFactor = _loanProductAppService.GetGuarantorAppraisalFactor(loanProductId, totalShares, serviceHeader);

                return Ok(ApiResponse("", new
                {
                    guarantorId = guarantor.Id,
                    guarantor.SerialNumber,
                    fullName = guarantor.FullName,
                    employerDescription = guarantor.StationZoneDivisionEmployerDescription,
                    stationDescription = guarantor.StationDescription,
                    identificationNumber = guarantor.IndividualIdentityCardNumber,
                    payrollNumber = guarantor.IndividualPayrollNumbers,
                    totalShares,
                    committedShares,
                    appraisalFactor,
                    availableToGuarantee = (totalShares * Convert.ToDecimal(appraisalFactor)) - committedShares
                }));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Registers a new loan case: snapshots the loan product onto the
        // case, validates and enriches guarantors, resolves collateral
        // documents, then persists all three together. See the class-level
        // comment for what's faithfully reproduced vs. deliberately not.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreateLoanCaseRequest request)
        {
            if (request?.LoanCase == null)
                return ErrorResponse("Request body with a loan case is required");

            var loanCaseDTO = request.LoanCase;
            var guarantors = request.Guarantors ?? new List<LoanGuarantorDTO>();
            var collateralDocumentIds = request.CollateralDocumentIds ?? new List<Guid>();

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var customer = _customerAppService.FindCustomer(loanCaseDTO.CustomerId, serviceHeader);
                if (customer == null)
                    return ErrorResponse("Customer not found");

                if (customer.RecordStatus != (int)RecordStatus.Approved)
                    return ErrorResponse("The selected customer has not yet been approved");

                var loanProduct = _loanProductAppService.FindLoanProduct(loanCaseDTO.LoanProductId, serviceHeader);
                if (loanProduct == null)
                    return ErrorResponse("Loan product not found");

                var registrationPermission = GetLoanStagePermission(
                    loanProduct.LoanRegistrationLoanProductSection,
                    SystemPermissionType.FrontOfficeLoanRegistration,
                    SystemPermissionType.BackOfficeLoanRegistration);
                var registrationPermissionError = ValidateMappedPermission(registrationPermission, serviceHeader);
                if (registrationPermissionError != null)
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope(registrationPermissionError));

                if (loanCaseDTO.SavingsProductId == null || loanCaseDTO.SavingsProductId == Guid.Empty)
                    return ErrorResponse("Savings product is required");

                var savingsProduct = _savingsProductAppService.FindSavingsProduct(loanCaseDTO.SavingsProductId.Value, loanCaseDTO.BranchId, serviceHeader);
                if (savingsProduct == null)
                    return ErrorResponse("Savings product not found");

                if (loanCaseDTO.LoanPurposeId == null || loanCaseDTO.LoanPurposeId == Guid.Empty)
                    return ErrorResponse("Loan purpose is required");

                var loanPurpose = _loanPurposeAppService.FindLoanPurpose(loanCaseDTO.LoanPurposeId.Value, serviceHeader);
                if (loanPurpose == null)
                    return ErrorResponse("Loan purpose not found");

                if (loanCaseDTO.RegistrationRemarkId == Guid.Empty)
                    return ErrorResponse("Registration remark is required");

                var loaningRemark = _loaningRemarkAppService.FindLoaningRemark(loanCaseDTO.RegistrationRemarkId, serviceHeader);
                if (loaningRemark == null)
                    return ErrorResponse("Loaning remark not found");

                // Membership period gate — reference LoanRegistrationController.Create.
                var membershipMonths = ((DateTime.Now.Year - customer.CreatedDate.Year) * 12) + DateTime.Now.Month - customer.CreatedDate.Month;
                if (membershipMonths < loanProduct.LoanRegistrationMinimumMembershipPeriod)
                    return ErrorResponse($"The selected customer's membership period is less than the minimum of {loanProduct.LoanRegistrationMinimumMembershipPeriod} months required for the selected loan product");

                var guarantorError = EnrichAndValidateGuarantors(guarantors, loanCaseDTO, loanProduct, serviceHeader);
                if (guarantorError != null)
                    return ErrorResponse(guarantorError);

                var collateralDocuments = new List<CustomerDocumentDTO>();
                foreach (var documentId in collateralDocumentIds)
                {
                    var document = _customerDocumentAppService.FindCustomerDocument(documentId, serviceHeader);
                    if (document != null)
                        collateralDocuments.Add(document);
                }

                CopyLoanProductSnapshot(loanCaseDTO, loanProduct);

                loanCaseDTO.SavingsProductDescription = savingsProduct.Description;
                loanCaseDTO.LoanPurposeDescription = loanPurpose.Description;
                loanCaseDTO.Remarks = loaningRemark.Description;

                loanCaseDTO.TotalNumberOfGuarantors = guarantors.Count;
                loanCaseDTO.TotalAmountGuaranteed = guarantors.Sum(g => g.AmountGuaranteed);
                loanCaseDTO.TotalCollateralAmount = collateralDocuments.Sum(d => d.CollateralValue);

                loanCaseDTO.Status = (int)LoanCaseStatus.Registered;
                loanCaseDTO.CreatedBy = serviceHeader.ApplicationUserName;

                loanCaseDTO.ValidateAll();
                if (loanCaseDTO.HasErrors)
                    return ErrorResponse(string.Join("; ", loanCaseDTO.ErrorMessages));

                var created = _loanCaseAppService.AddNewLoanCase(loanCaseDTO, serviceHeader);

                if (created == null)
                    return ErrorResponse("Failed to register the loan case");

                if (!string.IsNullOrEmpty(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope(created.ErrorMessageResult));

                if (collateralDocuments.Any())
                    _loanCaseAppService.UpdateLoanCollaterals(created.Id, collateralDocuments, serviceHeader);

                if (guarantors.Any())
                {
                    foreach (var guarantor in guarantors)
                        guarantor.LoanCaseId = created.Id;

                    _loanCaseAppService.UpdateLoanGuarantors(created.Id, guarantors, serviceHeader);
                }

                var appraisalPermission = GetLoanStagePermission(created, SystemPermissionType.FrontOfficeLoanAppraisal, SystemPermissionType.BackOfficeLoanAppraisal);
                if (!OriginateLoanStageWorkflow(created, appraisalPermission, serviceHeader))
                    throw new InvalidOperationException("The loan case was registered, but its appraisal workflow could not be created");

                var refreshed = _loanCaseAppService.FindLoanCase(created.Id, serviceHeader);

                return Ok(ApiResponse("Loan case registered successfully", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Reference has no dedicated Edit action for LoanCase itself
        // (guarantors/collaterals/appraisal factors each have their own
        // update entry points on ILoanCaseAppService, unchanged here) — this
        // exposes the one plain-field update the app service does support.
        [HttpPut]
        [Route("{id:guid}")]
        public async System.Threading.Tasks.Task<IHttpActionResult> Update(Guid id, LoanCaseDTO loanCaseDTO)
        {
            if (loanCaseDTO == null)
                return ErrorResponse("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                loanCaseDTO.Id = id;

                var updated = await _loanCaseAppService.UpdateLoanCaseAsync(loanCaseDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                var refreshed = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                return Ok(ApiResponse("Operation success", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // System-computed appraisal figures for a Registered/Deferred case —
        // the real, non-dead part of the reference GET Appraise action (see
        // class comment). Read-only: doesn't change anything, just gives the
        // appraiser numbers to work from before deciding.
        [HttpGet]
        [Route("{id:guid}/appraisal-worksheet")]
        public IHttpActionResult GetAppraisalWorksheet(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanCase = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (loanCase == null)
                    return NotFound();

                var loanProduct = _loanProductAppService.FindLoanProduct(loanCase.LoanProductId, serviceHeader);
                if (loanProduct == null)
                    return ErrorResponse("Loan product not found");

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(loanCase.CustomerId, serviceHeader) ?? new List<CustomerAccountDTO>();

                var investmentsBalance = accounts.Where(a => a.CustomerAccountTypeProductCode == (int)ProductCode.Investment).Sum(a => a.BookBalance);
                var savingsBalance = accounts.Where(a => a.CustomerAccountTypeProductCode == (int)ProductCode.Savings).Sum(a => a.BookBalance);
                var totalShares = investmentsBalance + savingsBalance;

                var maximumLoan = investmentsBalance * Convert.ToDecimal(loanProduct.LoanRegistrationInvestmentsMultiplier);

                var loanProductAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(loanCase.CustomerId, loanCase.LoanProductId, serviceHeader) ?? new List<CustomerAccountDTO>();
                var outstandingLoansBalance = loanProductAccounts.Sum(a => a.BookBalance + a.CarryForwardsBalance);

                var maximumEntitled = maximumLoan - outstandingLoansBalance;

                var loanPart = loanCase.AmountApplied;
                var interestPart = loanPart * Convert.ToDecimal((loanCase.LoanInterestAnnualPercentageRate / 100) * (loanCase.LoanRegistrationTermInMonths / 12.0));
                var loanPlusInterest = loanPart + interestPart;

                var monthlyInterestRate = loanCase.LoanInterestAnnualPercentageRate / (12 * 100);
                var termInMonths = loanCase.LoanRegistrationTermInMonths;
                var paymentPerPeriod = termInMonths > 0 && monthlyInterestRate > 0
                    ? Math.Round((double)loanPart * (monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, termInMonths)) / (Math.Pow(1 + monthlyInterestRate, termInMonths) - 1), 2)
                    : 0d;

                var appraisalFactors = _loanCaseAppService.FindLoanAppraisalFactorsByLoanCaseId(id, serviceHeader) ?? new List<LoanAppraisalFactorDTO>();
                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, serviceHeader) ?? new List<LoanGuarantorDTO>();
                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, serviceHeader) ?? new List<LoanCollateralDTO>();
                var loanAccounts = accounts.Where(a => a.CustomerAccountTypeProductCode == (int)ProductCode.Loan).ToList();
                var standingOrders = accounts
                    .SelectMany(a => _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(a.Id, serviceHeader) ?? new List<StandingOrderDTO>())
                    .GroupBy(s => s.Id)
                    .Select(g => g.First())
                    .ToList();
                var loanApplications = _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(loanCase.CustomerId, serviceHeader) ?? new List<LoanCaseDTO>();
                var attachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(id, serviceHeader) ?? new List<AttachedLoanDTO>();
                var fileRegister = _fileRegisterAppService.FindFileRegisterAndLastDepartmentByCustomerId(loanCase.CustomerId, serviceHeader);
                var fileReadyForAppraisal = fileRegister?.FileRegister != null && fileRegister.FileRegister.Status == (int)FileMovementStatus.Received;

                return Ok(ApiResponse("", new
                {
                    loanCase,
                    totalShares,
                    investmentsBalance,
                    savingsBalance,
                    maximumLoan,
                    outstandingLoansBalance,
                    maximumEntitled,
                    loanPart,
                    interestPart,
                    loanPlusInterest,
                    paymentPerPeriod,
                    appraisalFactors,
                    guarantors,
                    collaterals,
                    customerAccounts = accounts,
                    loanAccounts,
                    standingOrders,
                    loanApplications,
                    attachedLoans,
                    fileRegister,
                    fileReadyForAppraisal
                }));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/appraisal-factors")]
        public IHttpActionResult GetAppraisalFactors(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var factors = _loanCaseAppService.FindLoanAppraisalFactorsByLoanCaseId(id, serviceHeader);

                return Ok(ApiResponse("", factors ?? new List<LoanAppraisalFactorDTO>()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Transitions a Registered/Deferred case to Appraised or Rejected.
        // Also replaces the case's income-adjustment appraisal factors in
        // the same call when supplied — mirrors the reference POST Appraise
        // action's AppraiseLoanCaseAsync + UpdateLoanAppraisalFactorsAsync
        // pair. See class comment for the guard-clause bug fixed in
        // AppraiseLoanCase/Async alongside this endpoint.
        [HttpPost]
        [Route("{id:guid}/appraise")]
        public IHttpActionResult Appraise(Guid id, AppraiseLoanCaseRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (!Enum.IsDefined(typeof(LoanAppraisalOption), request.Option))
                return ErrorResponse("Invalid appraisal option");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                var customerFile = _fileRegisterAppService.FindFileRegisterAndLastDepartmentByCustomerId(existing.CustomerId, serviceHeader);
                if (customerFile?.FileRegister == null || customerFile.FileRegister.Status != (int)FileMovementStatus.Received)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("The customer's physical file must be received through File Tracking before appraisal"));

                WorkflowItemDTO workflowItem;
                var appraisalPermission = GetLoanStagePermission(existing, SystemPermissionType.FrontOfficeLoanAppraisal, SystemPermissionType.BackOfficeLoanAppraisal);
                var workflowError = ValidateFinalLoanStageItem(id, request.WorkflowItemId, appraisalPermission, serviceHeader, out workflowItem);
                if (workflowError != null)
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope(workflowError));

                if (request.IncomeAdjustments != null)
                {
                    foreach (var factor in request.IncomeAdjustments)
                    {
                        if (factor.IncomeAdjustmentId == Guid.Empty)
                            return ErrorResponse("Every income adjustment entry requires an IncomeAdjustmentId");

                        var incomeAdjustment = _incomeAdjustmentAppService.FindIncomeAdjustment(factor.IncomeAdjustmentId, serviceHeader);
                        if (incomeAdjustment == null)
                            return ErrorResponse($"Income adjustment {factor.IncomeAdjustmentId} not found");

                        factor.LoanCaseId = id;
                        factor.Description = incomeAdjustment.Description;
                        factor.Type = incomeAdjustment.Type;
                    }

                    if (request.IncomeAdjustments.Select(f => f.IncomeAdjustmentId).Distinct().Count() != request.IncomeAdjustments.Count)
                        return ErrorResponse("The same income adjustment was submitted more than once");
                }

                existing.LoanAppraisalOption = request.Option;
                existing.LoanProductLatestIncome = request.LoanProductLatestIncome;
                existing.AppraisedNetIncome = request.AppraisedNetIncome;
                existing.AppraisedAbility = request.AppraisedAbility;
                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(existing.CustomerId, serviceHeader) ?? new List<CustomerAccountDTO>();
                var investmentsBalance = accounts.Where(a => a.CustomerAccountTypeProductCode == (int)ProductCode.Investment).Sum(a => a.BookBalance);
                var maximumLoan = investmentsBalance * Convert.ToDecimal(existing.LoanRegistrationInvestmentsMultiplier);
                var sameProductAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(existing.CustomerId, existing.LoanProductId, serviceHeader) ?? new List<CustomerAccountDTO>();
                var maximumEntitled = Math.Max(0m, maximumLoan - sameProductAccounts.Sum(a => a.BookBalance + a.CarryForwardsBalance));
                existing.SystemAppraisedAmount = Math.Min(existing.AmountApplied, maximumEntitled);
                existing.SystemAppraisalRemarks = existing.SystemAppraisedAmount >= existing.AmountApplied
                    ? "The applied amount is within the member's investment-based entitlement."
                    : "The applied amount exceeds the member's investment-based entitlement.";
                if (request.Option == (int)LoanAppraisalOption.Appraise
                    && request.AppraisedAmount != existing.SystemAppraisedAmount
                    && string.IsNullOrWhiteSpace(request.AppraisedAmountRemarks))
                    return ErrorResponse("Appraised amount remarks are required when overriding the system-appraised amount");
                existing.AppraisedAmount = request.AppraisedAmount;
                existing.AppraisedAmountRemarks = request.AppraisedAmountRemarks;
                existing.AppraisalRemarks = request.AppraisalRemarks;
                existing.MonthlyPaybackAmount = request.MonthlyPaybackAmount;
                existing.TotalPaybackAmount = request.TotalPaybackAmount;
                existing.TotalLoansBalance = request.TotalLoansBalance;

                if (request.Option == (int)LoanAppraisalOption.Appraise && request.AttachedLoanAccountIds != null)
                {
                    var selectedIds = request.AttachedLoanAccountIds.Distinct().ToList();
                    var selectedAccounts = accounts.Where(a => selectedIds.Contains(a.Id) && a.CustomerAccountTypeProductCode == (int)ProductCode.Loan).ToList();
                    if (selectedAccounts.Count != selectedIds.Count)
                        return ErrorResponse("Every attached account must be a loan account belonging to the loanee");

                    var attachedLoans = selectedAccounts.Select(a => new AttachedLoanDTO
                    {
                        LoanCaseId = id,
                        CustomerAccountId = a.Id,
                        PrincipalBalance = a.BookBalance,
                        CarryForwardsBalance = a.CarryForwardsBalance
                    }).ToList();
                    _loanCaseAppService.UpdateAttachedLoans(id, attachedLoans, serviceHeader);
                }

                // AppraiseLoanCase commits before its legacy alert broker call. If that
                // optional integration failed on an earlier request, the durable case is
                // already Appraised while this workflow item is still pending. Treat the
                // identical retry as a recovery request and finish the workflow chain.
                var isCommittedRetry = request.Option == (int)LoanAppraisalOption.Appraise
                    && existing.Status == (int)LoanCaseStatus.Appraised
                    && workflowItem != null;
                var appraised = isCommittedRetry
                    || _loanCaseAppService.AppraiseLoanCase(existing, request.Option, request.ModuleNavigationItemCode, serviceHeader);

                if (!appraised)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Loan case is not in a Registered or Deferred state, or the appraisal option is invalid"));

                if (request.Option == (int)LoanAppraisalOption.Appraise && request.IncomeAdjustments != null && request.IncomeAdjustments.Any())
                    _loanCaseAppService.UpdateLoanAppraisalFactors(id, request.IncomeAdjustments, serviceHeader);

                var refreshed = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                if (!CompleteLoanStageWorkflow(workflowItem, request.AppraisalRemarks, request.UsedBiometrics, serviceHeader))
                    throw new InvalidOperationException("The appraisal was saved, but its workflow item could not be completed");

                if (request.Option == (int)LoanAppraisalOption.Appraise
                    && !OriginateLoanStageWorkflow(refreshed, GetLoanStagePermission(refreshed, SystemPermissionType.FrontOfficeLoanApproval, SystemPermissionType.BackOfficeLoanApproval), serviceHeader))
                    throw new InvalidOperationException("The appraisal was saved, but the approval workflow could not be created");

                return Ok(ApiResponse("Loan case appraisal recorded successfully", refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Transitions an Appraised case to Approved, Rejected, or Deferred.
        // See class comment: the reference Approve action re-snapshots the
        // same ~40 loan-product fields Create already snapshots, but
        // ApproveLoanCase never reads any of them off the DTO — only
        // approvedAmount/approvedAmountRemarks/approvedPrincipalPayment/
        // approvedInterestPayment/monthlyPaybackAmount/totalPaybackAmount/
        // approvalRemarks and the persisted entity's own Id/Status. Not
        // reproduced here; it would be pure busywork.
        [HttpPost]
        [Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id, ApproveLoanCaseRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (!Enum.IsDefined(typeof(LoanApprovalOption), request.Option))
                return ErrorResponse("Invalid approval option");

            if (string.IsNullOrWhiteSpace(request.ApprovalRemarks))
                return ErrorResponse("Approval remarks are required");

            if (request.Option == (int)LoanApprovalOption.Approve && request.ApprovedAmount <= 0)
                return ErrorResponse("Approved amount must be greater than zero");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                WorkflowItemDTO workflowItem;
                var approvalPermission = GetLoanStagePermission(existing, SystemPermissionType.FrontOfficeLoanApproval, SystemPermissionType.BackOfficeLoanApproval);
                if (request.Option == (int)LoanApprovalOption.Approve
                    && request.ApprovedAmount != existing.AppraisedAmount
                    && string.IsNullOrWhiteSpace(request.ApprovedAmountRemarks))
                    return ErrorResponse("Approved amount remarks are required when changing the appraised amount");
                var workflowError = ValidateFinalLoanStageItem(id, request.WorkflowItemId, approvalPermission, serviceHeader, out workflowItem);
                if (workflowError != null)
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope(workflowError));

                existing.LoanApprovalOption = request.Option;
                existing.ApprovedAmount = request.ApprovedAmount;
                existing.ApprovedAmountRemarks = request.ApprovedAmountRemarks;
                existing.ApprovedPrincipalPayment = request.ApprovedPrincipalPayment;
                existing.ApprovedInterestPayment = request.ApprovedInterestPayment;
                var repaymentSchedule = request.Option == (int)LoanApprovalOption.Approve
                    ? BuildRepaymentSchedule(request.ApprovedAmount, existing.LoanInterestAnnualPercentageRate, existing.LoanRegistrationTermInMonths, existing.InterestCalculationModeDescription)
                    : new List<RepaymentScheduleEntry>();
                existing.MonthlyPaybackAmount = repaymentSchedule.Any() ? repaymentSchedule.First().Payment : 0m;
                existing.TotalPaybackAmount = repaymentSchedule.Sum(item => item.Payment);
                existing.ApprovalRemarks = request.ApprovalRemarks;

                var approved = _loanCaseAppService.ApproveLoanCase(existing, request.Option, serviceHeader);

                if (!approved)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Loan case is not in an Appraised state, or the approval option is invalid"));

                var refreshed = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                if (!CompleteLoanStageWorkflow(workflowItem, request.ApprovalRemarks, request.UsedBiometrics, serviceHeader))
                    throw new InvalidOperationException("The approval was saved, but its workflow item could not be completed");

                if (request.Option == (int)LoanApprovalOption.Approve
                    && refreshed?.Status == (int)LoanCaseStatus.Approved
                    && !OriginateLoanStageWorkflow(refreshed, GetLoanStagePermission(refreshed, SystemPermissionType.FrontOfficeLoanAudit, SystemPermissionType.BackOfficeLoanAudit), serviceHeader))
                    throw new InvalidOperationException("The approval was saved, but the verification workflow could not be created");

                if (request.Option == (int)LoanApprovalOption.Defer
                    && !OriginateLoanStageWorkflow(refreshed, GetLoanStagePermission(refreshed, SystemPermissionType.FrontOfficeLoanAppraisal, SystemPermissionType.BackOfficeLoanAppraisal), serviceHeader))
                    throw new InvalidOperationException("The deferral was saved, but the new appraisal workflow could not be created");

                // If the loan product has LoanRegistrationBypassAudit set,
                // ApproveLoanCase auto-chains straight into AuditLoanCase —
                // refreshed.status may already be Audited here, not just
                // Approved. See WORKFLOW.md §4.
                var message = refreshed?.Status == (int)LoanCaseStatus.Audited
                    ? "Loan case approved and automatically verified (product bypasses verification)"
                    : "Loan case approval recorded successfully";

                return Ok(ApiResponse(message, refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Transitions an Approved case to Audited ("Verified" in the UI
        // label), Rejected, or Deferred. Unlike Appraise/Approve, this one
        // takes almost no input — everything AuditLoanCase actually does
        // (create the loan/savings CustomerAccounts if missing, compute the
        // repayment PV/PMT, recover upfront dynamic charges, build/update
        // the repayment StandingOrder) is driven entirely by fields already
        // on the persisted case from registration (LoanRegistration/
        // LoanInterest) and approval (ApprovedAmount/ApprovedPrincipalPayment/
        // ApprovedInterestPayment) — this is real, business-critical domain
        // logic, treat it as a black box, don't try to precompute or
        // second-guess its result client-side. See class comment and
        // WORKFLOW.md §14.4 for what was found while building this.
        [HttpPost]
        [Route("{id:guid}/audit")]
        public IHttpActionResult Audit(Guid id, AuditLoanCaseRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (!Enum.IsDefined(typeof(LoanAuditOption), request.Option))
                return ErrorResponse("Invalid audit option");

            if (string.IsNullOrWhiteSpace(request.AuditRemarks))
                return ErrorResponse("Audit remarks are required");

            if (request.Option == (int)LoanAuditOption.Audit && string.IsNullOrWhiteSpace(request.Reference))
                return ErrorResponse("Verification reference is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                WorkflowItemDTO workflowItem;
                var auditPermission = GetLoanStagePermission(existing, SystemPermissionType.FrontOfficeLoanAudit, SystemPermissionType.BackOfficeLoanAudit);
                var workflowError = ValidateFinalLoanStageItem(id, request.WorkflowItemId, auditPermission, serviceHeader, out workflowItem);
                if (workflowError != null)
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope(workflowError));

                existing.LoanAuditOption = request.Option;
                existing.AuditRemarks = request.AuditRemarks;
                existing.Reference = request.Reference;

                var audited = _loanCaseAppService.AuditLoanCase(existing, request.Option, serviceHeader);

                if (!audited)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Loan case is not in an Approved state, or the audit option is invalid"));

                var refreshed = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                if (!CompleteLoanStageWorkflow(workflowItem, request.AuditRemarks, request.UsedBiometrics, serviceHeader))
                    throw new InvalidOperationException("The verification was saved, but its workflow item could not be completed");

                if (request.Option == (int)LoanAuditOption.Defer
                    && !OriginateLoanStageWorkflow(refreshed, GetLoanStagePermission(refreshed, SystemPermissionType.FrontOfficeLoanAppraisal, SystemPermissionType.BackOfficeLoanAppraisal), serviceHeader))
                    throw new InvalidOperationException("The deferral was saved, but the new appraisal workflow could not be created");

                var message = request.Option == (int)LoanAuditOption.Audit && refreshed != null && refreshed.LoanRegistrationCreateStandingOrderOnLoanAudit
                    ? "Loan case verified — loan/savings accounts and repayment standing order have been set up"
                    : "Loan case verification recorded successfully";

                return Ok(ApiResponse(message, refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Cancellation — reference: Areas/Loaning/Controllers/LoanCancellationController.cs.
        // CancelLoanCase only reads loanCaseDTO.Id off the incoming DTO —
        // the status transition, CancelledBy/CancelledDate, and (on Reject)
        // guarantor release are all computed from the persisted entity
        // inside the app service itself, so this endpoint needs nothing but
        // the option. Only ever succeeds against an Audited case (the
        // reference screen exists specifically for the audited-but-not-yet-
        // disbursed window): Defer sends it back to Deferred, Reject sends
        // it to Rejected and releases its guarantors.
        [HttpPost]
        [Route("{id:guid}/cancel")]
        public IHttpActionResult Cancel(Guid id, CancelLoanCaseRequest request)
        {
            if (request == null)
                return ErrorResponse("Request body is required");

            if (!Enum.IsDefined(typeof(LoanCancellationOption), request.Option))
                return ErrorResponse("Invalid cancellation option");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _loanCaseAppService.FindLoanCase(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                var cancellationPermission = GetLoanStagePermission(existing, SystemPermissionType.FrontOfficeLoanAudit, SystemPermissionType.BackOfficeLoanAudit);
                var cancellationPermissionError = ValidateMappedPermission(cancellationPermission, serviceHeader);
                if (cancellationPermissionError != null)
                    return Content(HttpStatusCode.Forbidden, ErrorEnvelope(cancellationPermissionError));

                var cancelled = _loanCaseAppService.CancelLoanCase(existing, request.Option, serviceHeader);

                if (!cancelled)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Loan case is not in an Audited state, or the cancellation option is invalid"));

                var refreshed = _loanCaseAppService.FindLoanCase(id, serviceHeader);

                if (request.Option == (int)LoanCancellationOption.Defer
                    && !OriginateLoanStageWorkflow(refreshed, GetLoanStagePermission(refreshed, SystemPermissionType.FrontOfficeLoanAppraisal, SystemPermissionType.BackOfficeLoanAppraisal), serviceHeader))
                    throw new InvalidOperationException("The cancellation deferral was saved, but the new appraisal workflow could not be created");

                var message = request.Option == (int)LoanCancellationOption.Reject
                    ? "Loan case cancelled and its guarantors released"
                    : "Loan case deferred successfully";

                return Ok(ApiResponse(message, refreshed));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/cancellation-worksheet")]
        public IHttpActionResult GetCancellationWorksheet(Guid id)
        {
            try
            {
                var header = Utils.CreateServiceHeader();
                var loanCase = _loanCaseAppService.FindLoanCase(id, header);
                if (loanCase == null) return NotFound();
                if (loanCase.Status != (int)LoanCaseStatus.Audited)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Only an Audited loan case awaiting disbursement can be cancelled"));

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(loanCase.CustomerId, header) ?? new List<CustomerAccountDTO>();
                var loanAccounts = accounts.Where(account => account.CustomerAccountTypeProductCode == (int)ProductCode.Loan).ToList();
                var standingOrders = accounts.SelectMany(account => _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(account.Id, header) ?? new List<StandingOrderDTO>()).GroupBy(item => item.Id).Select(group => group.First()).ToList();
                var payouts = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByCustomerId((int)BatchStatus.Posted, loanCase.CustomerId, header) ?? new List<LoanDisbursementBatchEntryDTO>();
                var applications = _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(loanCase.CustomerId, header) ?? new List<LoanCaseDTO>();
                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, header) ?? new List<LoanGuarantorDTO>();
                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, header) ?? new List<LoanCollateralDTO>();
                var attachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(id, header) ?? new List<AttachedLoanDTO>();
                var scheduleAmount = loanCase.ApprovedAmount > 0 ? loanCase.ApprovedAmount : loanCase.AmountApplied;
                var repaymentSchedule = BuildRepaymentSchedule(scheduleAmount, loanCase.LoanInterestAnnualPercentageRate, loanCase.LoanRegistrationTermInMonths, loanCase.InterestCalculationModeDescription);
                return Ok(ApiResponse("", new { loanCase, guarantors, collaterals, attachedLoans, repaymentSchedule, loanAccounts, standingOrders, payouts, applications }));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        // Returns an error message on failure, null on success. Mutates each
        // guarantor with server-computed share data — see class comment.
        private string EnrichAndValidateGuarantors(List<LoanGuarantorDTO> guarantors, LoanCaseDTO loanCaseDTO, LoanProductDTO loanProduct, ServiceHeader serviceHeader)
        {
            if (!loanProduct.LoanRegistrationMicrocredit && loanProduct.LoanRegistrationSecurityRequired)
            {
                if (guarantors.Count < loanProduct.LoanRegistrationMinimumGuarantors)
                    return $"The selected loan product requires a minimum of {loanProduct.LoanRegistrationMinimumGuarantors} guarantors and a maximum of {loanProduct.LoanRegistrationMaximumGuarantees}";

                if (guarantors.Count > loanProduct.LoanRegistrationMaximumGuarantees)
                    return "The number of maximum guarantees must not be exceeded";
            }

            foreach (var guarantor in guarantors)
            {
                if (guarantor.GuarantorId == Guid.Empty)
                    return "Every guarantor entry requires a GuarantorId";

                var guarantorCustomer = _customerAppService.FindCustomer(guarantor.GuarantorId, serviceHeader);
                if (guarantorCustomer == null)
                    return $"Guarantor {guarantor.GuarantorId} not found";

                if (guarantor.GuarantorId == loanCaseDTO.CustomerId && !loanProduct.LoanRegistrationAllowSelfGuarantee)
                    return "The selected loan product does not allow self-guarantee";

                guarantor.CustomerId = guarantor.GuarantorId;
                guarantor.LoaneeCustomerId = loanCaseDTO.CustomerId;
                guarantor.LoanProductId = loanCaseDTO.LoanProductId;
                guarantor.LoanProductLoanRegistrationGuarantorSecurityMode = loanProduct.LoanRegistrationGuarantorSecurityMode;
                guarantor.LoanProductLoanRegistrationMicrocredit = loanProduct.LoanRegistrationMicrocredit;
                guarantor.MaximumGuarantees = loanProduct.LoanRegistrationMaximumGuarantees;
                guarantor.CurrentGuarantees = guarantors.Count;
                guarantor.CreatedBy = serviceHeader.ApplicationUserName;

                // Recomputed server-side, not trusted from the request — see class comment.
                guarantor.TotalShares = ComputeTotalShares(guarantor.GuarantorId, serviceHeader);
                guarantor.CommittedShares = ComputeCommittedShares(guarantor.GuarantorId, serviceHeader);
                guarantor.AppraisalFactor = _loanProductAppService.GetGuarantorAppraisalFactor(loanCaseDTO.LoanProductId, guarantor.TotalShares, serviceHeader);

                guarantor.ValidateAll();
                if (guarantor.HasErrors)
                    return string.Join("; ", guarantor.ErrorMessages);
            }

            if (!loanProduct.LoanRegistrationMicrocredit && loanProduct.LoanRegistrationSecurityRequired
                && loanProduct.LoanRegistrationGuarantorSecurityMode == (int)GuarantorSecurityMode.Investments
                && guarantors.Sum(g => g.AmountGuaranteed) < loanCaseDTO.AmountApplied)
            {
                return "The total amount guaranteed does not fully secure the amount applied";
            }

            return null;
        }

        // Sum of savings + investment book balances — same composition the
        // reference LoanGuarantorLookUp used.
        private decimal ComputeTotalShares(Guid customerId, ServiceHeader serviceHeader)
        {
            var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId, serviceHeader) ?? new List<CustomerAccountDTO>();

            return accounts
                .Where(a => a.CustomerAccountTypeProductCode == (int)ProductCode.Savings || a.CustomerAccountTypeProductCode == (int)ProductCode.Investment)
                .Sum(a => a.BookBalance);
        }

        // Sum of what this customer has already pledged as a guarantor on
        // other loans.
        private decimal ComputeCommittedShares(Guid customerId, ServiceHeader serviceHeader)
        {
            var existingGuarantees = _loanCaseAppService.FindLoanGuarantorsByCustomerId(customerId, serviceHeader) ?? new List<LoanGuarantorDTO>();

            return existingGuarantees.Sum(g => g.AmountGuaranteed);
        }

        // The ~40-field loan-product-at-registration-time snapshot the
        // reference LoanProductLookup/Create actions copy onto the loan
        // case — verbatim field-for-field, since this is exactly what makes
        // a loan case's terms independent of later changes to the product.
        private void CopyLoanProductSnapshot(LoanCaseDTO loanCaseDTO, LoanProductDTO loanProduct)
        {
            loanCaseDTO.LoanProductDescription = loanProduct.Description;
            loanCaseDTO.InterestCalculationModeDescription = loanProduct.LoanInterestCalculationModeDescription;
            loanCaseDTO.LoanInterestAnnualPercentageRate = loanProduct.LoanInterestAnnualPercentageRate;
            loanCaseDTO.LoanProductSectionDescription = loanProduct.LoanRegistrationLoanProductSectionDescription;
            loanCaseDTO.LoanRegistrationTermInMonths = loanProduct.LoanRegistrationTermInMonths;
            loanCaseDTO.LoanRegistrationMaximumAmount = loanProduct.LoanRegistrationMaximumAmount;
            loanCaseDTO.LoanInterestChargeMode = loanProduct.LoanInterestChargeMode;
            loanCaseDTO.LoanInterestRecoveryMode = loanProduct.LoanInterestRecoveryMode;
            loanCaseDTO.LoanInterestCalculationMode = loanProduct.LoanInterestCalculationMode;
            loanCaseDTO.LoanRegistrationPaymentFrequencyPerYear = loanProduct.LoanRegistrationPaymentFrequencyPerYear;
            loanCaseDTO.LoanRegistrationMinimumAmount = loanProduct.LoanRegistrationMinimumAmount;
            loanCaseDTO.LoanRegistrationMinimumInterestAmount = loanProduct.LoanRegistrationMinimumInterestAmount;
            loanCaseDTO.LoanRegistrationMinimumGuarantors = loanProduct.LoanRegistrationMinimumGuarantors;
            loanCaseDTO.LoanRegistrationMinimumMembershipPeriod = loanProduct.LoanRegistrationMinimumMembershipPeriod;
            loanCaseDTO.LoanRegistrationMaximumGuarantees = loanProduct.LoanRegistrationMaximumGuarantees;
            loanCaseDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement = loanProduct.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement;
            loanCaseDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage = loanProduct.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage;
            loanCaseDTO.LoanRegistrationLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
            loanCaseDTO.LoanRegistrationLoanProductCategory = loanProduct.LoanRegistrationLoanProductCategory;
            loanCaseDTO.LoanRegistrationConsecutiveIncome = loanProduct.LoanRegistrationConsecutiveIncome;
            loanCaseDTO.LoanRegistrationInvestmentsMultiplier = loanProduct.LoanRegistrationInvestmentsMultiplier;
            loanCaseDTO.LoanRegistrationRejectIfMemberHasBalance = loanProduct.LoanRegistrationRejectIfMemberHasBalance;
            loanCaseDTO.LoanRegistrationSecurityRequired = loanProduct.LoanRegistrationSecurityRequired;
            loanCaseDTO.LoanRegistrationAllowSelfGuarantee = loanProduct.LoanRegistrationAllowSelfGuarantee;
            loanCaseDTO.LoanRegistrationGracePeriod = loanProduct.LoanRegistrationGracePeriod;
            loanCaseDTO.LoanRegistrationPaymentDueDate = loanProduct.LoanRegistrationPaymentDueDate;
            loanCaseDTO.LoanRegistrationPayoutRecoveryMode = loanProduct.LoanRegistrationPayoutRecoveryMode;
            loanCaseDTO.LoanRegistrationPayoutRecoveryPercentage = loanProduct.LoanRegistrationPayoutRecoveryPercentage;
            loanCaseDTO.LoanRegistrationAggregateCheckOffRecoveryMode = loanProduct.LoanRegistrationAggregateCheckOffRecoveryMode;
            loanCaseDTO.LoanRegistrationChargeClearanceFee = loanProduct.LoanRegistrationChargeClearanceFee;
            loanCaseDTO.LoanRegistrationMicrocredit = loanProduct.LoanRegistrationMicrocredit;
            loanCaseDTO.LoanRegistrationStandingOrderTrigger = loanProduct.LoanRegistrationStandingOrderTrigger;
            loanCaseDTO.LoanRegistrationTrackArrears = loanProduct.LoanRegistrationTrackArrears;
            loanCaseDTO.LoanRegistrationChargeArrearsFee = loanProduct.LoanRegistrationChargeArrearsFee;
            loanCaseDTO.LoanRegistrationEnforceSystemAppraisalRecommendation = loanProduct.LoanRegistrationEnforceSystemAppraisalRecommendation;
            loanCaseDTO.LoanRegistrationBypassAudit = loanProduct.LoanRegistrationBypassAudit;
            loanCaseDTO.LoanRegistrationGuarantorSecurityMode = loanProduct.LoanRegistrationGuarantorSecurityMode;
            loanCaseDTO.LoanRegistrationRoundingType = loanProduct.LoanRegistrationRoundingType;
            loanCaseDTO.LoanRegistrationDisburseMicroLoanLessDeductions = loanProduct.LoanRegistrationDisburseMicroLoanLessDeductions;
            loanCaseDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal = loanProduct.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal;
            loanCaseDTO.LoanRegistrationThrottleScheduledArrearsRecovery = loanProduct.LoanRegistrationThrottleScheduledArrearsRecovery;
            loanCaseDTO.LoanRegistrationCreateStandingOrderOnLoanAudit = loanProduct.LoanRegistrationCreateStandingOrderOnLoanAudit;
            loanCaseDTO.TakeHomeType = loanProduct.TakeHomeType;
            loanCaseDTO.TakeHomePercentage = loanProduct.TakeHomePercentage;
            loanCaseDTO.TakeHomeFixedAmount = loanProduct.TakeHomeFixedAmount;
        }

        [HttpGet]
        [Route("cancellation-queue")]
        public IHttpActionResult CancellationQueue(int loanProductSection = (int)LoanProductSection.FOSA, DateTime? startDate = null, DateTime? endDate = null, string text = "", int loanCaseFilter = 0, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                if (!Enum.IsDefined(typeof(LoanProductSection), loanProductSection))
                    return ErrorResponse("Invalid loan product section");
                var from = (startDate ?? DateTime.Today.AddYears(-10)).Date;
                var to = (endDate ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);
                if (from > to) return ErrorResponse("Start date cannot be after end date");
                var header = Utils.CreateServiceHeader();
                var page = _loanCaseAppService.FindLoanCasesBySectionAndStatus(loanProductSection, (int)LoanCaseStatus.Audited, from, to, text ?? "", loanCaseFilter, pageIndex, Math.Min(Math.Max(pageSize, 1), 100), true, header);
                return Ok(ApiResponse("", page));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet]
        [Route("{id:guid}/approval-worksheet")]
        public IHttpActionResult GetApprovalWorksheet(Guid id)
        {
            try
            {
                var header = Utils.CreateServiceHeader();
                var loanCase = _loanCaseAppService.FindLoanCase(id, header);
                if (loanCase == null) return NotFound();
                if (loanCase.Status != (int)LoanCaseStatus.Appraised)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Only an Appraised loan case can be reviewed for approval"));

                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, header) ?? new List<LoanGuarantorDTO>();
                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, header) ?? new List<LoanCollateralDTO>();
                var attachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(id, header) ?? new List<AttachedLoanDTO>();
                var scheduleAmount = loanCase.AppraisedAmount > 0 ? loanCase.AppraisedAmount : loanCase.AmountApplied;
                var repaymentSchedule = BuildRepaymentSchedule(scheduleAmount, loanCase.LoanInterestAnnualPercentageRate, loanCase.LoanRegistrationTermInMonths, loanCase.InterestCalculationModeDescription);
                return Ok(ApiResponse("", new { loanCase, guarantors, collaterals, attachedLoans, repaymentSchedule }));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet]
        [Route("{id:guid}/verification-worksheet")]
        public IHttpActionResult GetVerificationWorksheet(Guid id)
        {
            try
            {
                var header = Utils.CreateServiceHeader();
                var loanCase = _loanCaseAppService.FindLoanCase(id, header);
                if (loanCase == null) return NotFound();
                if (loanCase.Status != (int)LoanCaseStatus.Approved)
                    return Content(HttpStatusCode.Conflict, ErrorEnvelope("Only an Approved loan case can be reviewed for verification"));

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(loanCase.CustomerId, header) ?? new List<CustomerAccountDTO>();
                var loanAccounts = accounts.Where(account => account.CustomerAccountTypeProductCode == (int)ProductCode.Loan).ToList();
                var standingOrders = accounts.SelectMany(account => _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(account.Id, header) ?? new List<StandingOrderDTO>()).GroupBy(item => item.Id).Select(group => group.First()).ToList();
                var payouts = _loanDisbursementBatchAppService.FindLoanDisbursementBatchEntriesByCustomerId((int)BatchStatus.Posted, loanCase.CustomerId, header) ?? new List<LoanDisbursementBatchEntryDTO>();
                var applications = _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(loanCase.CustomerId, header) ?? new List<LoanCaseDTO>();
                var guarantors = _loanCaseAppService.FindLoanGuarantorsByLoanCaseId(id, header) ?? new List<LoanGuarantorDTO>();
                var collaterals = _loanCaseAppService.FindLoanCollateralsByLoanCaseId(id, header) ?? new List<LoanCollateralDTO>();
                var attachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(id, header) ?? new List<AttachedLoanDTO>();
                var repaymentSchedule = BuildRepaymentSchedule(loanCase.ApprovedAmount, loanCase.LoanInterestAnnualPercentageRate, loanCase.LoanRegistrationTermInMonths, loanCase.InterestCalculationModeDescription);
                return Ok(ApiResponse("", new { loanCase, guarantors, collaterals, attachedLoans, repaymentSchedule, loanAccounts, standingOrders, payouts, applications }));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        // Customer 360 read model used while registering a loan. It restores
        // the active legacy Registration tabs without bringing MVC session
        // state into the REST API.
        [HttpGet]
        [Route("customers/{customerId:guid}/registration-context")]
        public IHttpActionResult RegistrationContext(Guid customerId, Guid? loanProductId = null)
        {
            try
            {
                var header = Utils.CreateServiceHeader();
                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId, header) ?? new List<CustomerAccountDTO>();
                var standingOrders = accounts.SelectMany(account => _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(account.Id, header) ?? new List<StandingOrderDTO>()).GroupBy(item => item.Id).Select(group => group.First()).ToList();
                var applications = _loanCaseAppService.FindLoanCasesByCustomerIdInProcess(customerId, header) ?? new List<LoanCaseDTO>();
                var payouts = _creditBatchAppService.FindCreditBatchEntriesByCustomerId((int)CreditBatchType.Payout, customerId, header) ?? new List<CreditBatchEntryDTO>();
                var collaterals = _customerDocumentAppService.FindCustomerDocuments(customerId, 1, header) ?? new List<CustomerDocumentDTO>();
                var investmentBalance = accounts.Where(account => account.CustomerAccountTypeProductCode == (int)ProductCode.Investment).Sum(account => account.BookBalance);
                var selectedProductLoanBalance = loanProductId.HasValue && loanProductId.Value != Guid.Empty
                    ? (_customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerId, loanProductId.Value, header) ?? new List<CustomerAccountDTO>()).Sum(account => account.BookBalance + account.CarryForwardsBalance)
                    : 0m;
                return Ok(ApiResponse("", new { accounts, standingOrders, payouts, applications, collaterals, investmentBalance, selectedProductLoanBalance }));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        // Loan workflow items are stage assignments. Earlier roles in a
        // configured approval chain can sign off through the generic
        // workflow endpoint; the final unlocked item must accompany the
        // detailed loan-stage request because that request carries the
        // financial fields the generic workflow DTO cannot represent.
        private bool OriginateLoanStageWorkflow(LoanCaseDTO loanCase, SystemPermissionType permissionType, ServiceHeader serviceHeader)
        {
            var roles = _authorizationAppService.GetRolesAndApprovalPriorityByPermissionType((int)permissionType, serviceHeader)
                ?? new List<SystemPermissionTypeInRoleDTO>();
            var approvalRoles = roles.Where(x => x.ApprovalPriority > 0 && x.RequiredApprovers > 0).ToList();

            // An unmapped stage preserves the existing direct-processing
            // behavior instead of creating a workflow with no actionable
            // items and permanently stranding the loan.
            if (!approvalRoles.Any()) return true;

            var existing = _workflowAppService.FindWorkflow(loanCase.Id, (int)permissionType, serviceHeader);
            if (existing != null && existing.MatchedStatus == (int)WorkflowMatchedStatus.NotMatched) return true;

            return _workflowAppService.AddNewWorkflow(new WorkflowDTO
            {
                RecordId = loanCase.Id,
                BranchId = loanCase.BranchId,
                SystemPermissionType = (int)permissionType,
                RequiredApprovals = approvalRoles.Sum(x => x.RequiredApprovers)
            }, approvalRoles, serviceHeader);
        }

        private SystemPermissionType GetLoanStagePermission(LoanCaseDTO loanCase, SystemPermissionType fosaPermission, SystemPermissionType bosaPermission)
        {
            return GetLoanStagePermission(loanCase.LoanRegistrationLoanProductSection, fosaPermission, bosaPermission);
        }

        private SystemPermissionType GetLoanStagePermission(int loanProductSection, SystemPermissionType fosaPermission, SystemPermissionType bosaPermission)
        {
            return loanProductSection == (int)LoanProductSection.FOSA ? fosaPermission : bosaPermission;
        }

        private string ValidateMappedPermission(SystemPermissionType permissionType, ServiceHeader serviceHeader)
        {
            var mappedRoles = _authorizationAppService.GetRolesAndApprovalPriorityByPermissionType((int)permissionType, serviceHeader)
                ?? new List<SystemPermissionTypeInRoleDTO>();
            if (!mappedRoles.Any()) return null;

            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            return mappedRoles.Any(mapping => callerRoles.Any(role => string.Equals(role, mapping.RoleName, StringComparison.OrdinalIgnoreCase)))
                ? null
                : $"The current user does not hold a role mapped to {permissionType}";
        }

        private string ValidateFinalLoanStageItem(Guid loanCaseId, Guid workflowItemId, SystemPermissionType permissionType, ServiceHeader serviceHeader, out WorkflowItemDTO workflowItem)
        {
            workflowItem = null;
            var workflow = _workflowAppService.FindWorkflow(loanCaseId, (int)permissionType, serviceHeader);

            // No role mapping at origination means no workflow and the
            // endpoint intentionally remains directly usable.
            if (workflow == null) return null;
            if (workflowItemId == Guid.Empty) return "workflowItemId is required for this assigned loan stage";

            workflowItem = _workflowAppService.FindWorkflowItem(workflowItemId, serviceHeader);
            if (workflowItem == null || workflowItem.WorkflowId != workflow.Id || workflowItem.WorkflowRecordId != loanCaseId)
                return "The workflow item does not belong to this loan case";
            if (workflowItem.WorkflowSystemPermissionType != (int)permissionType)
                return "The workflow item is for a different loan stage";
            if (workflowItem.Status != (int)WorkflowRecordStatus.Pending || workflowItem.IsLocked)
                return "The workflow item is not currently actionable";

            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            var assignedRoleName = workflowItem.RoleName;
            if (!callerRoles.Any(r => string.Equals(r, assignedRoleName, StringComparison.OrdinalIgnoreCase)))
                return "The current user does not hold the role assigned to this loan stage";
            if (!workflowItem.IsLastItemInOverallApprovalChain)
                return "This workflow item is an earlier approval stage; approve it in Workflow Tasks before the final loan-stage action";

            return null;
        }

        private bool CompleteLoanStageWorkflow(WorkflowItemDTO workflowItem, string remarks, bool usedBiometrics, ServiceHeader serviceHeader)
        {
            if (workflowItem == null) return true;

            workflowItem.Status = (int)WorkflowApprovalOption.Approved;
            workflowItem.Remarks = remarks ?? string.Empty;

            if (!_workflowAppService.ApproveWorkflowItem(workflowItem, usedBiometrics, serviceHeader))
                return false;

            return _workflowAppService.MarkWorkflowMatched(
                workflowItem.WorkflowRecordId,
                workflowItem.WorkflowSystemPermissionType,
                serviceHeader);
        }

        private List<RepaymentScheduleEntry> BuildRepaymentSchedule(decimal amount, double annualRate, int termInMonths, string calculationMode)
        {
            var result = new List<RepaymentScheduleEntry>();
            if (amount <= 0 || termInMonths <= 0) return result;
            var monthlyRate = Convert.ToDecimal(annualRate / 12d / 100d);
            var remaining = amount;

            for (var period = 1; period <= termInMonths; period++)
            {
                decimal payment;
                decimal interest;
                decimal principal;
                switch (calculationMode ?? string.Empty)
                {
                    case "Reducing Balance":
                    case "Amortization (Diminishing Balance)":
                        var factor = Convert.ToDecimal(Math.Pow(1d + Convert.ToDouble(monthlyRate), termInMonths));
                        payment = monthlyRate == 0 ? amount / termInMonths : amount * monthlyRate * factor / (factor - 1m);
                        interest = remaining * monthlyRate;
                        principal = payment - interest;
                        break;
                    case "Amortization (Straight Line)":
                        payment = amount / termInMonths + amount * monthlyRate;
                        interest = remaining * monthlyRate;
                        principal = payment - interest;
                        break;
                    case "Straight Line":
                    case "Fixed Interest":
                    default:
                        interest = amount * Convert.ToDecimal(annualRate / 100d) / termInMonths;
                        principal = amount / termInMonths;
                        payment = principal + interest;
                        break;
                }

                if (period == termInMonths) principal = remaining;
                payment = principal + interest;
                var ending = Math.Max(0m, remaining - principal);
                result.Add(new RepaymentScheduleEntry
                {
                    Period = period,
                    DueDate = DateTime.Today.AddMonths(period),
                    StartingBalance = Math.Round(remaining, 2),
                    Payment = Math.Round(payment, 2),
                    InterestPayment = Math.Round(interest, 2),
                    PrincipalPayment = Math.Round(principal, 2),
                    EndingBalance = Math.Round(ending, 2)
                });
                remaining = ending;
            }
            return result;
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

    public class RepaymentScheduleEntry
    {
        public int Period { get; set; }
        public DateTime DueDate { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal Payment { get; set; }
        public decimal InterestPayment { get; set; }
        public decimal PrincipalPayment { get; set; }
        public decimal EndingBalance { get; set; }
    }

    public class CreateLoanCaseRequest
    {
        public LoanCaseDTO LoanCase { get; set; }
        public List<LoanGuarantorDTO> Guarantors { get; set; }
        public List<Guid> CollateralDocumentIds { get; set; }
    }

    public class AppraiseLoanCaseRequest
    {
        public Guid WorkflowItemId { get; set; }
        public bool UsedBiometrics { get; set; }

        // LoanAppraisalOption: 1 = Appraise, 2 = Reject.
        public int Option { get; set; }

        public int ModuleNavigationItemCode { get; set; }

        public decimal LoanProductLatestIncome { get; set; }
        public decimal AppraisedNetIncome { get; set; }
        public decimal AppraisedAbility { get; set; }
        public decimal SystemAppraisedAmount { get; set; }
        public string SystemAppraisalRemarks { get; set; }
        public decimal AppraisedAmount { get; set; }
        public string AppraisedAmountRemarks { get; set; }
        public string AppraisalRemarks { get; set; }
        public decimal MonthlyPaybackAmount { get; set; }
        public decimal TotalPaybackAmount { get; set; }
        public decimal TotalLoansBalance { get; set; }

        // Only applied when Option == Appraise (a rejection releases
        // guarantors and has no appraisal figures to keep).
        public List<LoanAppraisalFactorDTO> IncomeAdjustments { get; set; }
        public List<Guid> AttachedLoanAccountIds { get; set; }
    }

    public class ApproveLoanCaseRequest
    {
        public Guid WorkflowItemId { get; set; }
        public bool UsedBiometrics { get; set; }

        // LoanApprovalOption: 1 = Approve, 2 = Reject, 4 = Defer.
        public int Option { get; set; }

        // Required only when Option == Approve.
        public decimal ApprovedAmount { get; set; }

        public string ApprovedAmountRemarks { get; set; }
        public decimal ApprovedPrincipalPayment { get; set; }
        public decimal ApprovedInterestPayment { get; set; }
        public decimal MonthlyPaybackAmount { get; set; }
        public decimal TotalPaybackAmount { get; set; }

        // Required for every option.
        public string ApprovalRemarks { get; set; }
    }

    public class AuditLoanCaseRequest
    {
        public Guid WorkflowItemId { get; set; }
        public bool UsedBiometrics { get; set; }

        // LoanAuditOption: 1 = Audit ("Verify" in the UI label), 2 = Reject, 4 = Defer.
        public int Option { get; set; }

        // Required for every option.
        public string AuditRemarks { get; set; }
        public string Reference { get; set; }
    }

    public class CancelLoanCaseRequest
    {
        // LoanCancellationOption: 1 = Defer, 2 = Reject.
        public int Option { get; set; }
    }
}
