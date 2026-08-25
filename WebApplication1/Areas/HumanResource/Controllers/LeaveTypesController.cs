using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using System;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Leave Types has no NavigationMenu leaf of its own anywhere under HR >
    // Operations > Leave (only Application/22016, Approval/22017,
    // Recall/22018 exist) — it's the reference table those three point at
    // via LeaveApplicationDTO.LeaveTypeId, same relationship
    // Department/Designation have to Employee. Exposed here as an
    // ungated utility page (reachable from the Leave Application screen,
    // not from the module tree), same pattern as
    // Administration/Roles/Create having no roles-list route of its own.
    //
    // ILeaveTypeAppService has no Delete at all (confirmed in
    // LeaveTypeAppService.cs) — no DELETE route here, same
    // don't-fake-a-capability rule CostCenters/ChequeTypes/Holidays follow.
    // AddNewLeaveType/UpdateLeaveType both run LeaveTypeBindingModel
    // validation internally and throw InvalidOperationException on failure
    // (HasErrors is never surfaced as a return value) — caught below and
    // turned into a clean 400 rather than a 500.
    [Authorize]
    [RoutePrefix("api/humanresource/leavetypes")]
    public class LeaveTypesController : ApiController
    {
        private readonly ILeaveTypeAppService _leaveTypeAppService;

        public LeaveTypesController(ILeaveTypeAppService leaveTypeAppService)
        {
            _leaveTypeAppService = leaveTypeAppService ?? throw new ArgumentNullException(nameof(leaveTypeAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var leaveTypes = _leaveTypeAppService.FindLeaveTypes(text, pageIndex, pageSize, serviceHeader);

                return Ok(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Bound to LeaveTypeBindingModel, not LeaveTypeDTO directly, since
        // LeaveTypeDTO's own settable fields don't include IsLocked (see
        // ToDTO below) and this is the shape AddNewLeaveType/UpdateLeaveType
        // validate against internally anyway.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LeaveTypeBindingModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Leave type payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var leaveTypeDTO = ToDTO(model);

                var created = _leaveTypeAppService.AddNewLeaveType(leaveTypeDTO, serviceHeader);

                return Ok(created);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("The request could not be completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, LeaveTypeBindingModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Leave type payload is required.");

                model.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var leaveTypeDTO = ToDTO(model);

                var updated = _leaveTypeAppService.UpdateLeaveType(leaveTypeDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(leaveTypeDTO);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("The request could not be completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // LeaveTypeDTO.IsLocked has a private setter (`get; private set;`)
        // — genuinely unreachable from any external caller, not just this
        // one, so it's deliberately left out here rather than worked
        // around. AddNewLeaveType/UpdateLeaveType both gate their
        // Lock()/UnLock() calls on leaveTypeDTO.IsLocked, which means as
        // things stand today there is no way for ANY caller to create or
        // lock a LeaveType as locked through this app-service method —
        // that's a pre-existing constraint in the DTO itself, not
        // something introduced here. IsLocked still round-trips fine on
        // reads (GET), it just can't be set on write without changing
        // LeaveTypeDTO's own setter, which is out of scope for a REST
        // wrapper. No "Is Locked?" control in the frontend form for the
        // same reason — it would silently do nothing.
        private static LeaveTypeDTO ToDTO(LeaveTypeBindingModel model)
        {
            return new LeaveTypeDTO
            {
                Id = model.Id,
                Description = model.Description,
                Entitlement = model.Entitlement,
                TargetGender = model.TargetGender,
                IsAccrued = model.IsAccrued,
                UnitType = (byte)model.UnitType,
                ExcludeHolidays = model.ExcludeHolidays,
                ExcludeWeekends = model.ExcludeWeekends,
            };
        }
    }
}
