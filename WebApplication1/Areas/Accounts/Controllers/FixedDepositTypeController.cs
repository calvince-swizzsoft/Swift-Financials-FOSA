using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adaptation of the reference MVC FixedDepositTypeController (Areas/Accounts) — a
    // product-setup screen for fixed deposit products, previously missing entirely from
    // this repo (no controller anywhere reached IFixedDepositTypeAppService), which meant
    // FixedDepositController (Areas/FrontOffice) had no way to originate a real fixed
    // deposit — every FixedDepositDTO.FixedDepositTypeId needs a real, persisted type.
    //
    // The reference Create action is a session-driven wizard whose two picker parameters
    // are misleadingly named: "commisionIds" actually selects attached LOAN PRODUCTS
    // (ProductCollectionInfo.LoanProductCollection), and "ExcemptedommisionId" actually
    // selects applicable LEVIES — not commissions/exemptions despite the names. Collapsed
    // here into direct sub-resource GET/PUT routes (same pattern as CommissionController's
    // graduated-scales/splits/levies and LoanProductController's sub-collections), with
    // Create additionally accepting attached-product/levy ids up front in one request
    // instead of a session round-trip. GraduatedScales (interest-rate bands by deposit
    // amount range) has no UI at all in the reference app despite
    // IFixedDepositTypeAppService already supporting it — exposed here as a sub-resource
    // too, matching the LoanProductController/CommissionController precedent of exposing
    // real app-service capability that never got a screen.
    [Authorize]
    [RoutePrefix("api/accounts/fixeddeposittypes")]
    public class FixedDepositTypeController : ApiController
    {
        private readonly IFixedDepositTypeAppService _fixedDepositTypeAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ILevyAppService _levyAppService;

        public FixedDepositTypeController(IFixedDepositTypeAppService fixedDepositTypeAppService, ILoanProductAppService loanProductAppService, ILevyAppService levyAppService)
        {
            _fixedDepositTypeAppService = fixedDepositTypeAppService ?? throw new ArgumentNullException(nameof(fixedDepositTypeAppService));
            _loanProductAppService = loanProductAppService ?? throw new ArgumentNullException(nameof(loanProductAppService));
            _levyAppService = levyAppService ?? throw new ArgumentNullException(nameof(levyAppService));
        }

        // Unpaged — for pickers (e.g. FixedDepositController's Create form).
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var fixedDepositTypes = _fixedDepositTypeAppService.FindFixedDepositTypes(serviceHeader);

                return Ok(new { success = true, message = "", data = fixedDepositTypes ?? new List<FixedDepositTypeDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public IHttpActionResult GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _fixedDepositTypeAppService.FindFixedDepositTypes(pageIndex, pageSize, serviceHeader)
                    : _fixedDepositTypeAppService.FindFixedDepositTypes(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("months/{months:int}")]
        public IHttpActionResult GetByMonths(int months)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var fixedDepositTypes = _fixedDepositTypeAppService.FindFixedDepositTypesByMonths(months, serviceHeader);

                return Ok(new { success = true, message = "", data = fixedDepositTypes ?? new List<FixedDepositTypeDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var fixedDepositType = _fixedDepositTypeAppService.FindFixedDepositType(id, serviceHeader);

                if (fixedDepositType == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = fixedDepositType });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreateFixedDepositTypeRequest request)
        {
            if (request?.FixedDepositType == null)
                return BadRequest("Request body is required");

            request.FixedDepositType.ValidateAll();
            if (request.FixedDepositType.HasErrors)
                return BadRequest(string.Join("; ", request.FixedDepositType.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _fixedDepositTypeAppService.AddNewFixedDepositType(request.FixedDepositType, request.EnforceFixedDepositBands, serviceHeader);

                if (created == null || created.ErrorMessageResult != null)
                    return BadRequest(created?.ErrorMessageResult ?? "Failed to create the fixed deposit type");

                if (request.AttachedLoanProductIds != null && request.AttachedLoanProductIds.Any())
                {
                    var attachedProducts = new ProductCollectionInfo
                    {
                        LoanProductCollection = request.AttachedLoanProductIds
                            .Select(id => _loanProductAppService.FindLoanProduct(id, serviceHeader))
                            .Where(p => p != null)
                            .ToList()
                    };

                    _fixedDepositTypeAppService.UpdateAttachedProducts(created.Id, attachedProducts, serviceHeader);
                }

                if (request.LevyIds != null && request.LevyIds.Any())
                {
                    var levies = request.LevyIds
                        .Select(id => _levyAppService.FindLevy(id, serviceHeader))
                        .Where(l => l != null)
                        .ToList();

                    _fixedDepositTypeAppService.UpdateLevies(created.Id, levies, serviceHeader);
                }

                if (request.GraduatedScales != null && request.GraduatedScales.Any())
                    _fixedDepositTypeAppService.UpdateGraduatedScales(created.Id, request.GraduatedScales, serviceHeader);

                var refreshed = _fixedDepositTypeAppService.FindFixedDepositType(created.Id, serviceHeader);

                return Ok(new { success = true, message = "Fixed deposit type created successfully", data = refreshed ?? created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands = true)
        {
            if (fixedDepositTypeDTO == null)
                return BadRequest("Request body is required");

            fixedDepositTypeDTO.Id = id;

            fixedDepositTypeDTO.ValidateAll();
            if (fixedDepositTypeDTO.HasErrors)
                return BadRequest(string.Join("; ", fixedDepositTypeDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _fixedDepositTypeAppService.UpdateFixedDepositType(fixedDepositTypeDTO, enforceFixedDepositBands, serviceHeader);

                if (!result)
                    return BadRequest(fixedDepositTypeDTO.ErrorMessageResult ?? "Failed to update the fixed deposit type");

                var updated = _fixedDepositTypeAppService.FindFixedDepositType(id, serviceHeader);

                return Ok(new { success = true, message = "Fixed deposit type updated successfully", data = updated });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/levies")]
        public IHttpActionResult GetLevies(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var levies = _fixedDepositTypeAppService.FindLevies(id, serviceHeader);

                return Ok(new { success = true, message = "", data = levies ?? new List<LevyDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/levies")]
        public IHttpActionResult UpdateLevies(Guid id, List<LevyDTO> levies)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _fixedDepositTypeAppService.UpdateLevies(id, levies ?? new List<LevyDTO>(), serviceHeader);

                if (!result)
                    return BadRequest("Failed to update the fixed deposit type's levies");

                var refreshed = _fixedDepositTypeAppService.FindLevies(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LevyDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/attached-products")]
        public IHttpActionResult GetAttachedProducts(Guid id, bool useCache = true)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var attachedProducts = _fixedDepositTypeAppService.FindAttachedProducts(id, serviceHeader, useCache);

                return Ok(new { success = true, message = "", data = attachedProducts });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/attached-products")]
        public IHttpActionResult UpdateAttachedProducts(Guid id, ProductCollectionInfo attachedProducts)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _fixedDepositTypeAppService.UpdateAttachedProducts(id, attachedProducts ?? new ProductCollectionInfo(), serviceHeader);

                if (!result)
                    return BadRequest("Failed to update the fixed deposit type's attached products");

                var refreshed = _fixedDepositTypeAppService.FindAttachedProducts(id, serviceHeader, false);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/graduated-scales")]
        public IHttpActionResult GetGraduatedScales(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var scales = _fixedDepositTypeAppService.FindGraduatedScales(id, serviceHeader);

                return Ok(new { success = true, message = "", data = scales ?? new List<FixedDepositTypeGraduatedScaleDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/graduated-scales")]
        public IHttpActionResult UpdateGraduatedScales(Guid id, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _fixedDepositTypeAppService.UpdateGraduatedScales(id, graduatedScales ?? new List<FixedDepositTypeGraduatedScaleDTO>(), serviceHeader);

                if (!result)
                    return BadRequest("Failed to update the fixed deposit type's graduated scales");

                var refreshed = _fixedDepositTypeAppService.FindGraduatedScales(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<FixedDepositTypeGraduatedScaleDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Interest/levy preview for a prospective deposit — read-only, no state change.
        [HttpGet]
        [Route("{id:guid}/tariffs")]
        public IHttpActionResult GetTariffs(Guid id, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var tariffs = _fixedDepositTypeAppService.ComputeTariffs(id, totalValue, debitChartOfAccountId, debitChartOfAccountCode, debitChartOfAccountName ?? string.Empty, serviceHeader);

                return Ok(new { success = true, message = "", data = tariffs ?? new List<TariffWrapper>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class CreateFixedDepositTypeRequest
    {
        public FixedDepositTypeDTO FixedDepositType { get; set; }

        public bool EnforceFixedDepositBands { get; set; }

        public List<Guid> AttachedLoanProductIds { get; set; } = new List<Guid>();

        public List<Guid> LevyIds { get; set; } = new List<Guid>();

        public List<FixedDepositTypeGraduatedScaleDTO> GraduatedScales { get; set; } = new List<FixedDepositTypeGraduatedScaleDTO>();
    }
}
