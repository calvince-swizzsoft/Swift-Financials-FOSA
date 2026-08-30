using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Full adaptation of the reference MVC LoanProductController (Areas/Accounts) — the
    // earlier pass only ever wired up GetAll() to unblock ChequeTypeController's picker,
    // see the history note in docs/api/loan-product-api-spec.md.
    //
    // The reference Create/Edit views are a session-driven wizard: Details/Create/Edit
    // stage LoanCycles/Deductibles/AuxiliaryConditions/AuxiliaryAppraisal/DynamicCharges/
    // AppraisalProducts/Commissions into Session across several small AJAX round-trips,
    // then a single Create POST flushes all of it via UpdateAssociatedData. There's no
    // real behavior in that beyond "attach these sub-collections to the new/existing loan
    // product" — collapsed here into direct sub-resource GET/PUT routes (same pattern as
    // CommissionController's graduated-scales/splits/levies), with Create additionally
    // accepting all of them up front in one request instead of a session round-trip.
    //
    // Not ported, all session-only wizard plumbing with no persistence behavior of their
    // own: StoreSelectedWellknownCharges/StoreSelectedCharges/StoreSelectedProducts/
    // ProcessSelectedLoans/ProcessSelectedInvestmentProducts/ProcessSelectedSavingProducts/
    // SaveSelection just wrote to Session and returned success — superseded by the
    // sub-resource PUT endpoints below.
    //
    // Also not ported: GetInvestmentProductDetails/GetSavingDetails/GetloanDetails/
    // GetLoanProductDetails — four near-identical single-record lookups (by investment/
    // savings/loan product id) used only to populate a description label while picking a
    // linked product in the wizard. Redundant with this controller's own GET {id} (and
    // SavingsProductController's, for the savings case) — no distinct behavior to preserve.
    [Authorize]
    [RoutePrefix("api/accounts/loanproducts")]
    public class LoanProductController : ApiController
    {
        private readonly ILoanProductAppService _loanProductAppService;

        public LoanProductController(ILoanProductAppService loanProductAppService)
        {
            _loanProductAppService = loanProductAppService ?? throw new ArgumentNullException(nameof(loanProductAppService));
        }

        // Unpaged — kept as the existing contract for pickers (docs/api/loan-product-api-spec.md,
        // ChequeTypeController's Create form). Do not turn this into a paged endpoint; see
        // GET /paged for the admin-screen listing instead.
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);

                return Ok(new { success = true, message = "", data = loanProducts ?? new List<LoanProductDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Reference had two near-duplicate DataTables actions (Index / LoanProductIndex) —
        // one plain text search, one additionally filtered by LoanRegistrationLoanProductSection.
        // Kept as two routes matching the two ILoanProductAppService overloads rather than
        // faking an "optional section" query param on one route.
        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _loanProductAppService.FindLoanProducts(pageIndex, pageSize, serviceHeader)
                    : _loanProductAppService.FindLoanProducts(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("paged/section/{section:int}")]
        public async Task<IHttpActionResult> GetPagedBySection(int section, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = _loanProductAppService.FindLoanProducts(section, text ?? string.Empty, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanProduct = _loanProductAppService.FindLoanProduct(id, serviceHeader);

                if (loanProduct == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = loanProduct });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Sub-collections stay optional on create — same reasoning CommissionController uses
        // for GraduatedScales/Splits/Levies: a loan product can be staged before its cycles/
        // deductibles/appraisal wiring is finalized. Replaces the reference wizard's session
        // dance (Create + several Store*/Add* round-trips + UpdateAssociatedData) with one call.
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateLoanProductRequest request)
        {
            try
            {
                if (request?.LoanProduct == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid loan product data", data = (object)null });
                }

                request.LoanProduct.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                var businessErrors = _loanProductAppService.ValidateLoanProduct(request.LoanProduct, serviceHeader);
                if (request.LoanProduct.HasErrors || businessErrors.Any())
                {
                    var messages = request.LoanProduct.ErrorMessages.Concat(businessErrors.SelectMany(item => item.Value)).Distinct().ToArray();
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", messages), validationErrors = businessErrors, data = (object)null });
                }

                var created = _loanProductAppService.AddNewLoanProductConfiguration(new LoanProductConfigurationDTO
                {
                    LoanProduct = request.LoanProduct,
                    Deductibles = request.Deductibles,
                    LoanCycles = request.LoanCycles,
                    AuxiliaryConditions = request.AuxiliaryConditions,
                    AuxiliaryAppraisalFactors = request.AuxiliaryAppraisalFactors,
                    DynamicCharges = request.DynamicCharges,
                    AppraisalProducts = request.AppraisalProducts,
                    Commissions = request.Commissions,
                    CommissionKnownChargeType = request.CommissionKnownChargeType,
                    CommissionChargeBasisValue = request.CommissionChargeBasisValue
                }, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the loan product could not be created.", data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Updates only the LoanProduct's own fields — matches the reference Edit action,
        // which never touched sub-collections (only Create's wizard did). Use the
        // sub-resource endpoints below to change those after creation.
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, LoanProductDTO loanProductDTO)
        {
            try
            {
                if (loanProductDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid loan product data", data = (object)null });
                }

                loanProductDTO.Id = id;
                loanProductDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                var businessErrors = _loanProductAppService.ValidateLoanProduct(loanProductDTO, serviceHeader);
                if (loanProductDTO.HasErrors || businessErrors.Any())
                {
                    var messages = loanProductDTO.ErrorMessages.Concat(businessErrors.SelectMany(item => item.Value)).Distinct().ToArray();
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", messages), validationErrors = businessErrors, data = (object)null });
                }

                var updated = _loanProductAppService.UpdateLoanProduct(loanProductDTO, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindLoanProduct(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/dynamic-charges")]
        public async Task<IHttpActionResult> GetDynamicCharges(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var charges = _loanProductAppService.FindDynamicCharges(id, serviceHeader);

                return Ok(new { success = true, message = "", data = charges ?? new List<DynamicChargeDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/dynamic-charges")]
        public async Task<IHttpActionResult> UpdateDynamicCharges(Guid id, List<DynamicChargeDTO> dynamicCharges)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateDynamicCharges(id, dynamicCharges ?? new List<DynamicChargeDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindDynamicCharges(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<DynamicChargeDTO>() });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/loan-cycles")]
        public async Task<IHttpActionResult> GetLoanCycles(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cycles = _loanProductAppService.FindLoanCycles(id, serviceHeader);

                return Ok(new { success = true, message = "", data = cycles ?? new List<LoanCycleDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/loan-cycles")]
        public async Task<IHttpActionResult> UpdateLoanCycles(Guid id, List<LoanCycleDTO> loanCycles)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateLoanCycles(id, loanCycles ?? new List<LoanCycleDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindLoanCycles(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LoanCycleDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // "Auxiliary conditions" are keyed off a base loan product (this one) pointing at a
        // target loan product plus an eligibility condition/percentage — e.g. "must not also
        // hold an active loan under product X". {id} below is the base loan product.
        [HttpGet]
        [Route("{id:guid}/auxiliary-conditions")]
        public async Task<IHttpActionResult> GetAuxiliaryConditions(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var conditions = _loanProductAppService.FindLoanProductAuxiliaryConditions(id, serviceHeader);

                return Ok(new { success = true, message = "", data = conditions ?? new List<LoanProductAuxiliaryConditionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/auxiliary-conditions")]
        public async Task<IHttpActionResult> UpdateAuxiliaryConditions(Guid id, List<LoanProductAuxiliaryConditionDTO> auxiliaryConditions)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateLoanProductAuxiliaryConditions(id, auxiliaryConditions ?? new List<LoanProductAuxiliaryConditionDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindLoanProductAuxiliaryConditions(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LoanProductAuxiliaryConditionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/deductibles")]
        public async Task<IHttpActionResult> GetDeductibles(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deductibles = _loanProductAppService.FindLoanProductDeductibles(id, serviceHeader);

                return Ok(new { success = true, message = "", data = deductibles ?? new List<LoanProductDeductibleDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/deductibles")]
        public async Task<IHttpActionResult> UpdateDeductibles(Guid id, List<LoanProductDeductibleDTO> deductibles)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateLoanProductDeductibles(id, deductibles ?? new List<LoanProductDeductibleDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindLoanProductDeductibles(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LoanProductDeductibleDTO>() });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/auxiliary-appraisal-factors")]
        public async Task<IHttpActionResult> GetAuxiliaryAppraisalFactors(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var factors = _loanProductAppService.FindLoanProductAuxilliaryAppraisalFactors(id, serviceHeader);

                return Ok(new { success = true, message = "", data = factors ?? new List<LoanProductAuxilliaryAppraisalFactorDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/auxiliary-appraisal-factors")]
        public async Task<IHttpActionResult> UpdateAuxiliaryAppraisalFactors(Guid id, List<LoanProductAuxilliaryAppraisalFactorDTO> auxiliaryAppraisalFactors)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateLoanProductAuxilliaryAppraisalFactors(id, auxiliaryAppraisalFactors ?? new List<LoanProductAuxilliaryAppraisalFactorDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindLoanProductAuxilliaryAppraisalFactors(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LoanProductAuxilliaryAppraisalFactorDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Products consulted during appraisal (investments qualification, loan recovery,
        // eligible income deduction) across loan/investment/savings products — the one
        // sub-resource that isn't a flat list, so it round-trips ProductCollectionInfo as-is.
        [HttpGet]
        [Route("{id:guid}/appraisal-products")]
        public async Task<IHttpActionResult> GetAppraisalProducts(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var appraisalProducts = _loanProductAppService.FindAppraisalProducts(id, serviceHeader);

                return Ok(new { success = true, message = "", data = appraisalProducts ?? new ProductCollectionInfo() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/appraisal-products")]
        public async Task<IHttpActionResult> UpdateAppraisalProducts(Guid id, ProductCollectionInfo appraisalProducts)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateAppraisalProducts(id, appraisalProducts ?? new ProductCollectionInfo(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindAppraisalProducts(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new ProductCollectionInfo() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Commissions attached to a loan product are scoped by knownChargeType (e.g. Loan
        // Clearance Fee, Express/Normal Disbursement Fee — Infrastructure.Crosscutting.Framework.Utils.
        // LoanProductKnownChargeType) — there's no meaningful "all commissions" default, so
        // it's a required query param rather than optional/defaulted to 0.
        [HttpGet]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> GetCommissions(Guid id, int knownChargeType)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = _loanProductAppService.FindCommissions(id, knownChargeType, serviceHeader);

                return Ok(new { success = true, message = "", data = commissions ?? new List<CommissionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full replace of which Commission records attach to this loan product under the
        // given knownChargeType — only each CommissionDTO.Id is read (same join-table
        // pattern as CommissionController's own levies sub-resource). ChargeBasisValue
        // (Principal vs. Book Balance) applies to the whole batch, not per-commission —
        // matches how the reference wizard derived it from a single selected charge.
        [HttpPut]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> UpdateCommissions(Guid id, UpdateLoanProductCommissionsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid commissions data", data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _loanProductAppService.UpdateCommissions(id, request.Commissions ?? new List<CommissionDTO>(), request.KnownChargeType, request.ChargeBasisValue, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _loanProductAppService.FindCommissions(id, request.KnownChargeType, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<CommissionDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CreateLoanProductRequest
    {
        public LoanProductDTO LoanProduct { get; set; }
        public List<LoanProductDeductibleDTO> Deductibles { get; set; }
        public List<LoanCycleDTO> LoanCycles { get; set; }
        public List<LoanProductAuxiliaryConditionDTO> AuxiliaryConditions { get; set; }
        public List<LoanProductAuxilliaryAppraisalFactorDTO> AuxiliaryAppraisalFactors { get; set; }
        public List<DynamicChargeDTO> DynamicCharges { get; set; }
        public ProductCollectionInfo AppraisalProducts { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
        public int CommissionKnownChargeType { get; set; }
        public int CommissionChargeBasisValue { get; set; }
    }

    public class UpdateLoanProductCommissionsRequest
    {
        public int KnownChargeType { get; set; }
        public int ChargeBasisValue { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
    }
}
