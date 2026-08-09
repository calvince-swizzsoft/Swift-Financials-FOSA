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
    // Adapted from the reference MVC LevyController (Areas/Accounts). Two things
    // deliberately not ported forward, both real bugs in the reference app:
    // - Its Edit POST action calls UpdateLevySplitsByLevyIdAsync with a freshly-empty
    //   ObservableCollection<LevySplitDTO>() on every single edit, silently wiping the
    //   levy's splits. PUT /{id} here only touches the Levy's own fields; splits have
    //   their own sub-resource endpoint.
    // - Its Create POST hardcodes levyDTO.LevySplitsTotalPercentage = 100 unconditionally
    //   right before calling ValidateAll(), which drives LevyDTO's own
    //   [CustomValidation(..., "CheckLevySplitsTotalPercentage")] attribute — making that
    //   validation a permanent no-op regardless of what the splits actually sum to. This
    //   controller computes it from the real submitted splits instead.
    [Authorize]
    [RoutePrefix("api/accounts/levies")]
    public class LevyController : ApiController
    {
        private readonly ILevyAppService _levyAppService;

        public LevyController(ILevyAppService levyAppService)
        {
            _levyAppService = levyAppService ?? throw new ArgumentNullException(nameof(levyAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var levies = _levyAppService.FindLevies(serviceHeader);

                return Ok(new { success = true, message = "", data = levies ?? new List<LevyDTO>() });
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
                    ? _levyAppService.FindLevies(pageIndex, pageSize, serviceHeader)
                    : _levyAppService.FindLevies(text, pageIndex, pageSize, serviceHeader);

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

                var levy = _levyAppService.FindLevy(id, serviceHeader);

                if (levy == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = levy });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // LevySplits stay optional on create — a levy can be staged before its GL split
        // is finalized, same reasoning Commission uses for its own sub-resources. When
        // splits are sent, they must sum to 100% (real rule, kept). Note: unlike
        // CommissionController.Create, there is no ErrorMessageResult/409 duplicate check
        // here — LevyAppService.AddNewLevy has no duplicate-description guard at all
        // (checked directly; its only ErrorMessageResult write is dead code that gets
        // overwritten by the AutoMapper projection on return, so it's never actually
        // observable on the returned DTO).
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateLevyRequest request)
        {
            try
            {
                if (request?.Levy == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid levy data", data = (object)null });
                }

                if (!TryValidateSplitPercentages(request.LevySplits, out var splitError))
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = splitError, data = (object)null });
                }

                request.Levy.LevySplitsTotalPercentage = (request.LevySplits == null || !request.LevySplits.Any())
                    ? 100
                    : request.LevySplits.Sum(s => s.Percentage);

                request.Levy.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (request.Levy.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.Levy.ErrorMessages), data = (object)null });
                }

                var createdLevy = _levyAppService.AddNewLevy(request.Levy, serviceHeader);

                if (createdLevy == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the levy could not be created.", data = (object)null });
                }

                if (request.LevySplits != null)
                {
                    _levyAppService.UpdateLevySplits(createdLevy.Id, request.LevySplits, serviceHeader);
                }

                var refreshed = _levyAppService.FindLevy(createdLevy.Id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Updates only the Levy's own fields. Deliberately does not touch LevySplits —
        // see the class-level comment on the reference app's edit-wipes-splits bug.
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, LevyDTO levyDTO)
        {
            try
            {
                if (levyDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid levy data", data = (object)null });
                }

                levyDTO.Id = id;

                // Not touching splits on this endpoint — satisfy LevyDTO's own
                // [CustomValidation] (requires exactly 100) without implying anything
                // about the real split total, which GET .../splits reflects accurately.
                levyDTO.LevySplitsTotalPercentage = 100;

                levyDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (levyDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", levyDTO.ErrorMessages), data = (object)null });
                }

                var updated = _levyAppService.UpdateLevy(levyDTO, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _levyAppService.FindLevy(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
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

                var splits = _levyAppService.FindLevySplits(id, serviceHeader);

                return Ok(new { success = true, message = "", data = splits ?? new List<LevySplitDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/splits")]
        public async Task<IHttpActionResult> UpdateSplits(Guid id, List<LevySplitDTO> levySplits)
        {
            try
            {
                if (!TryValidateSplitPercentages(levySplits, out var splitError))
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = splitError, data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _levyAppService.UpdateLevySplits(id, levySplits ?? new List<LevySplitDTO>(), serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshed = _levyAppService.FindLevySplits(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed ?? new List<LevySplitDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static bool TryValidateSplitPercentages(List<LevySplitDTO> splits, out string error)
        {
            error = null;

            if (splits == null || !splits.Any())
            {
                return true;
            }

            var total = splits.Sum(s => s.Percentage);

            if (Math.Abs(total - 100d) > 0.01d)
            {
                error = string.Format("Total levy split percentage must equal 100% (got {0}%).", total);
                return false;
            }

            return true;
        }
    }

    public class CreateLevyRequest
    {
        public LevyDTO Levy { get; set; }
        public List<LevySplitDTO> LevySplits { get; set; }
    }
}
