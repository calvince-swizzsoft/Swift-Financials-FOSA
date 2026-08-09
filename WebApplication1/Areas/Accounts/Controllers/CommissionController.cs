using Application.MainBoundedContext.AccountsModule.Services;
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
    // Adapted from the reference MVC CommissionController/ChargesController (Areas/Accounts).
    // The reference app actually has two parallel, partially-duplicate controllers doing
    // this — ChargesController is a buggier, session-heavy reimplementation of the same
    // Commission CRUD (percentage validation with dead/commented code, one Create branch
    // that skips saving graduated scales/levies entirely) — not ported. TiersController
    // (meant to own GraduatedScale) never actually persisted anything — its Create POST
    // has the real save call commented out. See COMMISSION-LEVY-CHARGE-CONCEPTS.md for the
    // domain model this sits on top of.
    //
    // GraduatedScale/CommissionSplit/Levy never had a clean, working, standalone screen in
    // the reference app, so — same pattern as ChequeTypeController's commissions/attached-
    // products — they're sub-resources here (GET/PUT .../graduated-scales, .../splits,
    // .../levies) rather than separate top-level controllers.
    [Authorize]
    [RoutePrefix("api/accounts/commissions")]
    public class CommissionController : ApiController
    {
        private readonly ICommissionAppService _commissionAppService;

        public CommissionController(ICommissionAppService commissionAppService)
        {
            _commissionAppService = commissionAppService ?? throw new ArgumentNullException(nameof(commissionAppService));
        }

        // Unpaged — kept as the existing contract for pickers (docs/api/commission-api-spec.md,
        // ChequeTypeController's Create form). Do not turn this into a paged endpoint; see
        // GET /paged for the admin-screen listing instead.
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = _commissionAppService.FindCommissions(serviceHeader);

                return Ok(new { success = true, message = "", data = commissions ?? new List<CommissionDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _commissionAppService.FindCommissions(pageIndex, pageSize, serviceHeader)
                    : _commissionAppService.FindCommissions(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commission = _commissionAppService.FindCommission(id, serviceHeader);

                if (commission == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = commission });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GraduatedScales/Levies stay optional on create — a commission can be staged
        // before its rate/levy structure is finalized (same reasoning ChequeType uses for
        // its own sub-resources). Splits are validated to sum to 100% when non-empty —
        // the one piece of the reference ChargesController's validation that wasn't
        // dead/commented-out code, kept because it's a real, meaningful rule (splits
        // divide 100% of the computed commission amount across GL accounts).
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateCommissionRequest request)
        {
            try
            {
                if (request?.Commission == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid commission data", data = (object)null });
                }

                request.Commission.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (request.Commission.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.Commission.ErrorMessages), data = (object)null });
                }

                if (!TryValidateSplitPercentages(request.Splits, out var splitError))
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = splitError, data = (object)null });
                }

                var createdCommission = _commissionAppService.AddNewCommission(request.Commission, serviceHeader);

                if (createdCommission == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the commission could not be created.", data = (object)null });
                }

                // Duplicate Description is reported by echoing the DTO back with
                // ErrorMessageResult set (Id stays Guid.Empty) — same pattern as CostCenter.
                if (!string.IsNullOrWhiteSpace(createdCommission.ErrorMessageResult))
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = createdCommission.ErrorMessageResult, data = (object)null });
                }

                if (request.GraduatedScales != null)
                {
                    _commissionAppService.UpdateGraduatedScales(createdCommission.Id, request.GraduatedScales, serviceHeader);
                }

                if (request.Splits != null)
                {
                    _commissionAppService.UpdateCommissionSplits(createdCommission.Id, request.Splits, serviceHeader);
                }

                if (request.Levies != null)
                {
                    _commissionAppService.UpdateLevies(createdCommission.Id, request.Levies, serviceHeader);
                }

                var refreshed = _commissionAppService.FindCommission(createdCommission.Id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Updates only the Commission's own fields (Description/MaximumCharge/RoundingType/
        // IsLocked). Deliberately does not touch graduated scales/splits/levies — the
        // reference app's Edit action left these alone too (only Create touched them),
        // made explicit and consistent here: use the sub-resource endpoints below.
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, CommissionDTO commissionDTO)
        {
            try
            {
                if (commissionDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid commission data", data = (object)null });
                }

                commissionDTO.Id = id;
                commissionDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (commissionDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", commissionDTO.ErrorMessages), data = (object)null });
                }

                var updated = _commissionAppService.UpdateCommission(commissionDTO, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _commissionAppService.FindCommission(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/graduated-scales")]
        public async Task<IHttpActionResult> GetGraduatedScales(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var scales = _commissionAppService.FindGraduatedScales(id, serviceHeader);

                return Ok(new { success = true, message = "", data = scales ?? new List<GraduatedScaleDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/graduated-scales")]
        public async Task<IHttpActionResult> UpdateGraduatedScales(Guid id, List<GraduatedScaleDTO> graduatedScales)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _commissionAppService.UpdateGraduatedScales(id, graduatedScales ?? new List<GraduatedScaleDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _commissionAppService.FindGraduatedScales(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<GraduatedScaleDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/splits")]
        public async Task<IHttpActionResult> GetSplits(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var splits = _commissionAppService.FindCommissionSplits(id, serviceHeader);

                return Ok(new { success = true, message = "", data = splits ?? new List<CommissionSplitDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/splits")]
        public async Task<IHttpActionResult> UpdateSplits(Guid id, List<CommissionSplitDTO> splits)
        {
            try
            {
                if (!TryValidateSplitPercentages(splits, out var splitError))
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = splitError, data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _commissionAppService.UpdateCommissionSplits(id, splits ?? new List<CommissionSplitDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _commissionAppService.FindCommissionSplits(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<CommissionSplitDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/levies")]
        public async Task<IHttpActionResult> GetLevies(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var levies = _commissionAppService.FindLevies(id, serviceHeader);

                return Ok(new { success = true, message = "", data = levies ?? new List<LevyDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Full replace of which Levy records attach to this Commission — only each
        // LevyDTO.Id is read (matches ChequeTypeController's commissions sub-resource,
        // same join-table pattern). Does not create/edit the Levy records themselves;
        // use LevyController for that.
        [HttpPut]
        [Route("{id:guid}/levies")]
        public async Task<IHttpActionResult> UpdateLevies(Guid id, List<LevyDTO> levies)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _commissionAppService.UpdateLevies(id, levies ?? new List<LevyDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _commissionAppService.FindLevies(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LevyDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Splits divide 100% of the computed commission amount across GL accounts — the
        // one real validation rule the reference ChargesController enforced (amid a lot of
        // dead/commented-out percentage-checking code). An empty list clears all splits
        // and is exempt from the check (nothing to sum).
        private static bool TryValidateSplitPercentages(List<CommissionSplitDTO> splits, out string error)
        {
            error = null;

            if (splits == null || !splits.Any())
            {
                return true;
            }

            var total = splits.Sum(s => s.Percentage);

            if (Math.Abs(total - 100d) > 0.01d)
            {
                error = string.Format("Total split percentage must equal 100% (got {0}%).", total);
                return false;
            }

            return true;
        }
    }

    public class CreateCommissionRequest
    {
        public CommissionDTO Commission { get; set; }
        public List<GraduatedScaleDTO> GraduatedScales { get; set; }
        public List<CommissionSplitDTO> Splits { get; set; }
        public List<LevyDTO> Levies { get; set; }
    }
}
