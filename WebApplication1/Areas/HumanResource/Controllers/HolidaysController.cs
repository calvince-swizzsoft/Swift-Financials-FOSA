
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Code 22005 ("Holidays", ControllerName: Holiday,
    // AreaName: HumanResource) — the nav entry already existed server-side
    // but had no REST controller behind it (only the legacy WCF
    // HolidayService.svc + the IHolidayAppService app-service layer, both
    // already wired into DI in UnityConfig.cs/Container.cs). Mirrors
    // DepartmentsController's shape (GET "", POST "", PUT "{id}") but adds
    // DELETE and paged/text search since IHolidayAppService actually
    // supports both here, unlike Department/Designation/EmployeeType.
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/humanresource/holidays")]
    public class HolidaysController : ApiController
    {
        private readonly IHolidayAppService _holidayAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public HolidaysController(
            IHolidayAppService holidayAppService,
            IPostingPeriodAppService postingPeriodAppService)
        {
            _holidayAppService = holidayAppService;
            _postingPeriodAppService = postingPeriodAppService;
        }

        // Every posting period, unpaged — same "small reference list, no
        // pagination needed" shape as api/accounts/commissions etc. used
        // for picker dropdowns elsewhere. Placed ahead of the "{id}"-style
        // routes below purely for readability; Web API attribute routing
        // already prefers the literal "posting-periods" segment over a
        // parameterized one regardless of declaration order.
        [HttpGet]
        [Route("posting-periods")]
        public async Task<IHttpActionResult> PostingPeriods()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var postingPeriods = _postingPeriodAppService.FindPostingPeriods(serviceHeader);

                return Ok(postingPeriods);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var holidays = _holidayAppService.FindHolidays(text, pageIndex, pageSize, serviceHeader);

                if (holidays == null)
                {
                    return NotFound();
                }

                return Ok(holidays);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(HolidayDTO holidayDTO)
        {
            try
            {
                if (holidayDTO == null)
                {
                    return BadRequest("Holiday payload is required.");
                }

                // HolidayDTO.CheckDates validates DurationStartDate/EndDate
                // against PostingPeriodDurationStartDate/EndDate on the same
                // DTO — populated here from the real posting period record
                // rather than trusted from the client, so a stale/forged
                // bound on the request can't widen what date range passes
                // validation.
                var postingPeriod = _postingPeriodAppService.FindPostingPeriod(holidayDTO.PostingPeriodId, Utils.CreateServiceHeader());
                if (postingPeriod == null)
                {
                    return BadRequest("Selected posting period was not found.");
                }
                holidayDTO.PostingPeriodDurationStartDate = postingPeriod.DurationStartDate;
                holidayDTO.PostingPeriodDurationEndDate = postingPeriod.DurationEndDate;

                holidayDTO.ValidateAll();

                if (!holidayDTO.HasErrors)
                {
                    var serviceHeader = Utils.CreateServiceHeader();

                    var createdHolidayDTO = _holidayAppService.AddNewHoliday(holidayDTO, serviceHeader);

                    return Ok(createdHolidayDTO);
                }
                else
                {
                    return BadRequest(holidayDTO.ErrorMessages.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, HolidayDTO holidayDTO)
        {
            try
            {
                if (holidayDTO == null)
                {
                    return BadRequest("Holiday payload is required.");
                }
                holidayDTO.Id = id;

                var postingPeriod = _postingPeriodAppService.FindPostingPeriod(holidayDTO.PostingPeriodId, Utils.CreateServiceHeader());
                if (postingPeriod == null)
                {
                    return BadRequest("Selected posting period was not found.");
                }
                holidayDTO.PostingPeriodDurationStartDate = postingPeriod.DurationStartDate;
                holidayDTO.PostingPeriodDurationEndDate = postingPeriod.DurationEndDate;

                holidayDTO.ValidateAll();

                if (!holidayDTO.HasErrors)
                {
                    var serviceHeader = Utils.CreateServiceHeader();

                    var updated = _holidayAppService.UpdateHoliday(holidayDTO, serviceHeader);

                    if (!updated)
                    {
                        return NotFound();
                    }

                    return Ok(holidayDTO);
                }
                else
                {
                    return BadRequest(holidayDTO.ErrorMessages.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _holidayAppService.RemoveHoliday(id, serviceHeader);

                if (!removed)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
