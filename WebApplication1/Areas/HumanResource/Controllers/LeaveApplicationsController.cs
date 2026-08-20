using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

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
        private readonly ILeaveTypeAppService _leaveTypeAppService;

        public LeaveApplicationsController(
            ILeaveApplicationAppService leaveApplicationAppService,
            ILeaveTypeAppService leaveTypeAppService)
        {
            _leaveApplicationAppService = leaveApplicationAppService ?? throw new ArgumentNullException(nameof(leaveApplicationAppService));
            _leaveTypeAppService = leaveTypeAppService ?? throw new ArgumentNullException(nameof(leaveTypeAppService));
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

                var applications = status.HasValue
                    ? _leaveApplicationAppService.FindLeaveApplications(status.Value, DateTime.MinValue, DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _leaveApplicationAppService.FindLeaveApplications(text, pageIndex, pageSize, serviceHeader);

                return Ok(applications);
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

                var application = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);

                if (application == null)
                    return NotFound();

                return Ok(application);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

                var balance = _leaveApplicationAppService.FindEmployeeLeaveBalances(employeeId, leaveTypeId, serviceHeader);

                return Ok(new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, Balance = balance });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

                var leaveType = _leaveTypeAppService.FindLeaveType(leaveApplicationDTO.LeaveTypeId, serviceHeader);
                if (leaveType == null)
                    return BadRequest("Selected leave type was not found.");

                ApplyLeaveType(leaveApplicationDTO, leaveType);

                var currentBalance = _leaveApplicationAppService.FindEmployeeLeaveBalances(leaveApplicationDTO.EmployeeId, leaveApplicationDTO.LeaveTypeId, serviceHeader);
                var requestedDays = (decimal)(leaveApplicationDTO.DurationEndDate - leaveApplicationDTO.DurationStartDate).TotalDays;
                leaveApplicationDTO.Balance = currentBalance - requestedDays;

                var created = _leaveApplicationAppService.AddNewLeaveApplication(leaveApplicationDTO, serviceHeader);

                if (created == null)
                    return InternalServerError(new InvalidOperationException("Failed to save the leave application."));

                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

                var persisted = _leaveApplicationAppService.FindLeaveApplication(id, serviceHeader);
                if (persisted == null)
                    return NotFound();

                if (persisted.Status != (byte)LeaveApplicationStatus.Pending)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only a Pending leave application can be edited." });

                leaveApplicationDTO.Id = id;

                var leaveType = _leaveTypeAppService.FindLeaveType(leaveApplicationDTO.LeaveTypeId, serviceHeader);
                if (leaveType == null)
                    return BadRequest("Selected leave type was not found.");

                ApplyLeaveType(leaveApplicationDTO, leaveType);

                var currentBalance = _leaveApplicationAppService.FindEmployeeLeaveBalances(persisted.EmployeeId, leaveApplicationDTO.LeaveTypeId, serviceHeader);
                var requestedDays = (decimal)(leaveApplicationDTO.DurationEndDate - leaveApplicationDTO.DurationStartDate).TotalDays;
                leaveApplicationDTO.Balance = currentBalance - requestedDays;

                var updated = _leaveApplicationAppService.UpdateLeaveApplication(leaveApplicationDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(leaveApplicationDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static void ApplyLeaveType(LeaveApplicationDTO leaveApplicationDTO, LeaveTypeDTO leaveType)
        {
            leaveApplicationDTO.LeaveTypeDescription = leaveType.Description;
            leaveApplicationDTO.LeaveTypeUnitType = leaveType.UnitType;
            leaveApplicationDTO.LeaveTypeIsAccrued = leaveType.IsAccrued;
            leaveApplicationDTO.LeaveTypeEntitlement = leaveType.Entitlement;
            leaveApplicationDTO.LeaveTypeExcludeHolidays = leaveType.ExcludeHolidays;
            leaveApplicationDTO.LeaveTypeExcludeWeekends = leaveType.ExcludeWeekends;
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
