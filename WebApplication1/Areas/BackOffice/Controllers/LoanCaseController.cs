using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
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

        public LoanCaseController(
            ILoanCaseAppService loanCaseAppService,
            ICustomerAppService customerAppService,
            ILoanProductAppService loanProductAppService,
            ISavingsProductAppService savingsProductAppService,
            ILoanPurposeAppService loanPurposeAppService,
            ILoaningRemarkAppService loaningRemarkAppService,
            ICustomerDocumentAppService customerDocumentAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            _loanCaseAppService = loanCaseAppService ?? throw new ArgumentNullException(nameof(loanCaseAppService));
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _loanProductAppService = loanProductAppService ?? throw new ArgumentNullException(nameof(loanProductAppService));
            _savingsProductAppService = savingsProductAppService ?? throw new ArgumentNullException(nameof(savingsProductAppService));
            _loanPurposeAppService = loanPurposeAppService ?? throw new ArgumentNullException(nameof(loanPurposeAppService));
            _loaningRemarkAppService = loaningRemarkAppService ?? throw new ArgumentNullException(nameof(loaningRemarkAppService));
            _customerDocumentAppService = customerDocumentAppService ?? throw new ArgumentNullException(nameof(customerDocumentAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
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

    public class CreateLoanCaseRequest
    {
        public LoanCaseDTO LoanCase { get; set; }
        public List<LoanGuarantorDTO> Guarantors { get; set; }
        public List<Guid> CollateralDocumentIds { get; set; }
    }
}
