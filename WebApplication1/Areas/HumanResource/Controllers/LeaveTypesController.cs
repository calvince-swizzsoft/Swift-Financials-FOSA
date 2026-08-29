using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;
        private const int LeaveApplicationModuleCode = 22016;

        public LeaveTypesController(ILeaveTypeAppService leaveTypeAppService, INavigationItemInRoleAppService navigationItemInRoleAppService)
        {
            _leaveTypeAppService = leaveTypeAppService ?? throw new ArgumentNullException(nameof(leaveTypeAppService));
            _navigationItemInRoleAppService = navigationItemInRoleAppService ?? throw new ArgumentNullException(nameof(navigationItemInRoleAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var leaveTypes = _leaveTypeAppService.FindLeaveTypes(text, pageIndex, pageSize, serviceHeader);

                return Ok(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Bound to the write model so validation and lifecycle fields have
        // one explicit API shape before being mapped to the application DTO.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LeaveTypeBindingModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Leave type payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var leaveTypeDTO = ToDTO(model);

                var created = _leaveTypeAppService.AddNewLeaveType(leaveTypeDTO, serviceHeader);

                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                if (!HasPermission(serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var leaveTypeDTO = ToDTO(model);

                var updated = _leaveTypeAppService.UpdateLeaveType(leaveTypeDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(leaveTypeDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

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
                IsLocked = model.IsLocked,
            };
        }

        private bool HasPermission(ServiceHeader serviceHeader)
        {
            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            var grantedRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(LeaveApplicationModuleCode, serviceHeader) ?? new string[0];
            return callerRoles.Any(callerRole => grantedRoles.Any(grantedRole =>
                string.Equals(callerRole, grantedRole, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
