using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;
using WebApplication1.Areas.Identity.Services;

namespace WebApplication1.Controllers
{
    // Backs all three HR > Operations > Leave nav leaves — Application
    // (22016), Approval (22017), Recall (22018) — which are really one
    // data source (LeaveApplication) viewed through three different
    // workflow lenses, not three separate resources: Approval is the
    // Pending queue, Recall is the Approved queue, Application is
    // everything. Same gap as Holidays/EmployeeDocuments: the domain/
    // app-service layer (ILeaveApplicationAppService, ILeaveTypeAppService)
    // was already fully built (and DI-registered) but had no REST
    // controller anywhere.
    //
    // Two things the raw app service leaves to its caller, resolved here
    // server-side rather than trusted from the client (same principle as
    // HolidaysController resolving PostingPeriod bounds):
    // - LeaveTypeExcludeWeekends/ExcludeHolidays/UnitType/IsAccrued/
    //   Entitlement/Description are plain fields duplicated onto
    //   LeaveApplicationDTO — AddNewLeaveApplication/UpdateLeaveApplication
    //   read them directly off the DTO instead of looking up LeaveTypeId
    //   themselves, so this controller resolves the real LeaveType and
    //   copies them across before calling in.
    // - DTO.Balance is read by the app service as "remaining balance after
    //   this request", not "days requested" — RecallLeaveApplication adds
    //   (DurationEndDate - DurationStartDate).TotalDays back onto it to
    //   "return" the balance, so Create computes
    //   currentBalance - thatSameDayCount up front to stay consistent with
    //   what Recall assumes when reversing it later.
    //
    // AddNewLeaveApplication/UpdateLeaveApplication/AuthorizeLeaveApplication/
    // RecallLeaveApplication all run LeaveApplicationBindingModel validation
    // internally and throw InvalidOperationException on failure (business
    // rules too — start date in the past, start after end, negative
    // balance — are also raised this way, not via HasErrors) — caught
    // below and turned into a clean 400 rather than a 500.
    //
    // Status transitions the app service itself does NOT guard (confirmed
    // in LeaveApplicationAppService.cs — AuthorizeLeaveApplication doesn't
    // check the current status before overwriting it, RecallLeaveApplication
    // doesn't check it's actually Approved) are guarded here instead:
    // Update/Authorize only act on a Pending application, Recall only on
    // an Approved one.
    [Authorize]
    [RoutePrefix("api/humanresource/leaveapplications")]
    public class LeaveApplicationsController : ApiController
    {
        private readonly ILeaveApplicationAppService _leaveApplicationAppService;
        private readonly INavigationItemInRoleAppService _navigationItemInRoleAppService;
        private readonly UserManagerService _userManagerService;
        private const int LeaveApplicationModuleCode = 22016;
        private const int LeaveApprovalModuleCode = 22017;
        private const int LeaveRecallModuleCode = 22018;

        public LeaveApplicationsController(
            ILeaveApplicationAppService leaveApplicationAppService,
            INavigationItemInRoleAppService navigationItemInRoleAppService,
            UserManagerService userManagerService)
        {
            _leaveApplicationAppService = leaveApplicationAppService ?? throw new ArgumentNullException(nameof(leaveApplicationAppService));
            _navigationItemInRoleAppService = navigationItemInRoleAppService ?? throw new ArgumentNullException(nameof(navigationItemInRoleAppService));
            _userManagerService = userManagerService ?? throw new ArgumentNullException(nameof(userManagerService));
        }

        // status omitted -> plain text-search paged browse (the
        // "Application" screen's default view, every status included).
        // status supplied -> the underlying app service's status+date-range
        // overload, spanning all time (Approval passes Pending, Recall
        // passes Approved).
        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int? status = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var requiredCode = status == (int)LeaveApplicationStatus.Pending ? LeaveApprovalModuleCode
                    : status == (int)LeaveApplicationStatus.Approved ? LeaveRecallModuleCode
                    : LeaveApplicationModuleCode;
                if (!HasPermission(requiredCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var applications = status.HasValue
                    ? _leaveApplicationAppService.FindLeaveApplications(status.Value, DateTime.MinValue, DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _leaveApplicationAppService.FindLeaveApplications(text, pageIndex, pageSize, serviceHeader);

                return Ok(applications);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasAnyLeavePermission(serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var application = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);

                if (application == null)
                    return NotFound();

                return Ok(application);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // The create/edit form's live balance preview — FindEmployeeLeaveBalances
        // keyed off the employee's most recent application for this leave
        // type (or the LeaveType's own Entitlement if they have none yet).
        [HttpGet]
        [Route("balance")]
        public IHttpActionResult Balance(Guid employeeId, Guid leaveTypeId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(LeaveApplicationModuleCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var balance = _leaveApplicationAppService.FindEmployeeLeaveBalances(employeeId, leaveTypeId, serviceHeader);

                return Ok(new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, Balance = balance });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(LeaveApplicationDTO leaveApplicationDTO)
        {
            try
            {
                if (leaveApplicationDTO == null)
                    return BadRequest("Leave application payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(LeaveApplicationModuleCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);
                var approvalRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(LeaveApprovalModuleCode, serviceHeader) ?? new string[0];
                if (!_userManagerService.HasActiveEmployeeUserInAnyRole(approvalRoles, serviceHeader))
                    return BadRequest("Leave cannot be submitted because no active employee user is currently authorized to approve it. Assign Leave Approval permission to a role with an active user, then try again.");

                var created = _leaveApplicationAppService.AddNewLeaveApplication(leaveApplicationDTO, serviceHeader);

                if (created == null)
                    throw new InvalidOperationException("Failed to save the leave application.");

                _userManagerService.NotifyActiveLeaveApprovers(approvalRoles, created, serviceHeader);

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
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, LeaveApplicationDTO leaveApplicationDTO)
        {
            try
            {
                if (leaveApplicationDTO == null)
                    return BadRequest("Leave application payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(LeaveApplicationModuleCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var persisted = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only a Pending leave application can be edited." });

                leaveApplicationDTO.Id = id;

                var updated = _leaveApplicationAppService.UpdateLeaveApplication(leaveApplicationDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(leaveApplicationDTO);
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

        // body: { Decision: "approve" | "reject", Remarks }. Status is never
        // taken from the client as a raw value — only these two real
        // transitions are reachable through this action.
        [HttpPost]
        [Route("{id:guid}/authorize")]
        public IHttpActionResult Authorize(Guid id, [FromBody] LeaveAuthorizationRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Decision))
                    return BadRequest("A Decision of 'approve' or 'reject' is required.");

                var decision = request.Decision.Trim().ToLowerInvariant();
                if (decision != "approve" && decision != "reject")
                    return BadRequest("Decision must be 'approve' or 'reject'.");

                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(LeaveApprovalModuleCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var persisted = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only a Pending leave application can be approved or rejected." });

                persisted.Status = (byte)(decision == "approve" ? LeaveApplicationStatus.Approved : LeaveApplicationStatus.Rejected);
                persisted.AuthorizationRemarks = request.Remarks;

                var authorized = _leaveApplicationAppService.AuthorizeLeaveApplication(persisted, serviceHeader);

                if (!authorized)
                    return NotFound();

                return Ok(persisted);
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

        // body: { Remarks }. Only reachable from Approved — RecallLeaveApplication
        // itself always forces Status to Recalled and returns the day count
        // to Balance regardless of prior status, so the guard here is what
        // actually stops a Pending/Rejected/already-Recalled application
        // from being "recalled".
        [HttpPost]
        [Route("{id:guid}/recall")]
        public IHttpActionResult Recall(Guid id, [FromBody] LeaveRecallRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                if (!HasPermission(LeaveRecallModuleCode, serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

                var persisted = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                if (persisted.Status != (byte)LeaveApplicationStatus.Approved)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only an Approved leave application can be recalled." });

                persisted.RecallRemarks = request?.Remarks;

                var recalled = _leaveApplicationAppService.RecallLeaveApplication(persisted, serviceHeader);

                if (!recalled)
                    return NotFound();

                return Ok(persisted);
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

        private bool HasAnyLeavePermission(ServiceHeader serviceHeader)
        {
            return HasPermission(LeaveApplicationModuleCode, serviceHeader)
                || HasPermission(LeaveApprovalModuleCode, serviceHeader)
                || HasPermission(LeaveRecallModuleCode, serviceHeader);
        }

        [HttpGet]
        [Route("approval-readiness")]
        public IHttpActionResult ApprovalReadiness()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            if (!HasAnyLeavePermission(serviceHeader)) return StatusCode(HttpStatusCode.Forbidden);

            var approvalRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(LeaveApprovalModuleCode, serviceHeader) ?? new string[0];
            var pending = _leaveApplicationAppService.FindLeaveApplications(
                (int)LeaveApplicationStatus.Pending, DateTime.MinValue, DateTime.MaxValue, string.Empty, 0, 1, serviceHeader);

            return Ok(new
            {
                HasEligibleApprover = _userManagerService.HasActiveEmployeeUserInAnyRole(approvalRoles, serviceHeader),
                PendingApplications = pending?.ItemsCount ?? 0
            });
        }

        private bool HasPermission(int moduleCode, ServiceHeader serviceHeader)
        {
            var callerRoles = serviceHeader.ApplicationUserRoles ?? new System.Collections.Generic.List<string>();
            var grantedRoles = _navigationItemInRoleAppService.GetRolesForNavigationItemCode(moduleCode, serviceHeader) ?? new string[0];
            return callerRoles.Any(callerRole => grantedRoles.Any(grantedRole =>
                string.Equals(callerRole, grantedRole, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public class LeaveAuthorizationRequest
    {
        public string Decision { get; set; }
        public string Remarks { get; set; }
    }

    public class LeaveRecallRequest
    {
        public string Remarks { get; set; }
    }
}
