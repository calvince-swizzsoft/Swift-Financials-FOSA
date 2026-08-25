using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Codes 22023 ("Salary Periods") / 22024 ("Salary
    // Processing") / 22026 ("Period Closing") — per the user-supplied
    // WebApplication1/Areas/Salary Processing.md. Unlike Heads/Groups/
    // Cards, this is not CRUD over a reference table: ProcessSalaryPeriod
    // computes real payroll (PAYE/NSSF/NHIF/provident fund, loan and
    // investment standing-order deductions) into a batch of *unposted*
    // PaySlip records — it never posts anything to the G/L itself. The
    // actual posting (and, if the period has ExecutePayoutStandingOrders
    // set, queuing standing-order payouts) happens per-payslip via
    // PostPaySlip, exposed on PaySlipsController instead. Read the full
    // ~500-line ProcessSalaryPeriod and ~300-line PostPaySlip bodies in
    // SalaryPeriodAppService.cs before touching this file — neither is
    // reimplemented here, both are called as-is.
    //
    // ProcessSalaryPeriod is idempotent by design (it calls
    // IPaySlipAppService.PurgePaySlips first, wiping any previously staged
    // -but-not-yet-posted payslips for this period before regenerating) —
    // re-running it is expected, not a duplication bug. It silently no-ops
    // (returns false, no explanation) unless the period's Status is Open;
    // this controller checks that itself first so the caller gets a clear
    // 409 instead of an ambiguous failure.
    //
    // AddNewSalaryPeriod has an unusual failure shape worth calling out:
    // on a duplicate open/closed period for the same posting period/month/
    // employee category, it does NOT return null or throw — it returns the
    // same DTO back with ErrorMessageResult populated and nothing
    // persisted. UpdateSalaryPeriod hits the identical business rule but
    // throws InvalidOperationException instead. Both shapes are handled
    // below rather than assuming one.
    //
    // ProcessSalaryPeriod's own List<EmployeeDTO> parameter is resolved
    // here from the client's selected Salary Group/Branch/Department ids
    // rather than trusted as a raw employee list — real group/branch/
    // department membership is derived from SalaryCards (the only place
    // an employee is actually linked to a Salary Group), not something the
    // client can just assert by sending arbitrary employee ids. There's no
    // FindSalaryCards-by-group lookup on ISalaryCardAppService, so this
    // fetches every card and filters in memory — fine at this data volume,
    // matching FindSalaryCards()'s own unpaged "browse everything" shape.
    [Authorize]
    [RoutePrefix("api/humanresource/salaryperiods")]
    public class SalaryPeriodsController : ApiController
    {
        private readonly ISalaryPeriodAppService _salaryPeriodAppService;
        private readonly ISalaryCardAppService _salaryCardAppService;
        private readonly IEmployeeAppService _employeeAppService;

        public SalaryPeriodsController(
            ISalaryPeriodAppService salaryPeriodAppService,
            ISalaryCardAppService salaryCardAppService,
            IEmployeeAppService employeeAppService)
        {
            _salaryPeriodAppService = salaryPeriodAppService ?? throw new ArgumentNullException(nameof(salaryPeriodAppService));
            _salaryCardAppService = salaryCardAppService ?? throw new ArgumentNullException(nameof(salaryCardAppService));
            _employeeAppService = employeeAppService ?? throw new ArgumentNullException(nameof(employeeAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryPeriods = _salaryPeriodAppService.FindSalaryPeriods(text, pageIndex, pageSize, serviceHeader);

                return Ok(salaryPeriods);
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

                var salaryPeriod = _salaryPeriodAppService.FindSalaryPeriod(id, serviceHeader);

                if (salaryPeriod == null)
                    return NotFound();

                return Ok(salaryPeriod);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SalaryProcessingDTO salaryPeriodDTO)
        {
            try
            {
                if (salaryPeriodDTO == null)
                    return BadRequest("Salary period payload is required.");

                if (salaryPeriodDTO.PostingPeriodId == Guid.Empty)
                    return BadRequest("A Posting Period is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _salaryPeriodAppService.AddNewSalaryPeriod(salaryPeriodDTO, serviceHeader);

                if (created == null)
                    return InternalServerError(new InvalidOperationException("Failed to save the salary period."));

                if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                    return Content(HttpStatusCode.Conflict, new { Message = created.ErrorMessageResult });

                return Ok(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, SalaryProcessingDTO salaryPeriodDTO)
        {
            try
            {
                if (salaryPeriodDTO == null)
                    return BadRequest("Salary period payload is required.");

                salaryPeriodDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _salaryPeriodAppService.UpdateSalaryPeriod(salaryPeriodDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(salaryPeriodDTO);
            }
            catch (InvalidOperationException)
            {
                return Content(HttpStatusCode.Conflict, new { Message = "The request could not be completed." });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // body: { SalaryGroupIds: [...], BranchIds: [...] (optional),
        // DepartmentIds: [...] (optional) }. Per Salary Processing.md:
        // choosing a salary group is mandatory; branches/departments are an
        // optional further narrowing (omit to include every branch/
        // department in the selected groups).
        [HttpPost]
        [Route("{id:guid}/process")]
        public IHttpActionResult Process(Guid id, [FromBody] ProcessSalaryPeriodRequest request)
        {
            try
            {
                if (request == null || request.SalaryGroupIds == null || !request.SalaryGroupIds.Any())
                    return BadRequest("At least one Salary Group is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var salaryPeriod = _salaryPeriodAppService.FindSalaryPeriod(id, serviceHeader);
                if (salaryPeriod == null)
                    return NotFound();

                if (salaryPeriod.Status != (int)SalaryPeriodStatus.Open)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only an Open salary period can be processed." });

                var groupIds = new HashSet<Guid>(request.SalaryGroupIds);
                var branchIds = request.BranchIds != null && request.BranchIds.Any() ? new HashSet<Guid>(request.BranchIds) : null;
                var departmentIds = request.DepartmentIds != null && request.DepartmentIds.Any() ? new HashSet<Guid>(request.DepartmentIds) : null;

                var salaryCards = _salaryCardAppService.FindSalaryCards(serviceHeader) ?? new List<SalaryCardDTO>();

                var matchedEmployeeIds = new HashSet<Guid>(
                    salaryCards
                        .Where(c => groupIds.Contains(c.SalaryGroupId))
                        .Where(c => branchIds == null || branchIds.Contains(c.EmployeeBranchId))
                        .Where(c => departmentIds == null || departmentIds.Contains(c.EmployeeDepartmentId))
                        .Select(c => c.EmployeeId));

                if (!matchedEmployeeIds.Any())
                    return Content(HttpStatusCode.BadRequest, new { Message = "No employees matched the selected Salary Groups/Branches/Departments." });

                var allEmployees = _employeeAppService.FindEmployees(serviceHeader) ?? new List<EmployeeDTO>();
                var employees = allEmployees.Where(e => matchedEmployeeIds.Contains(e.Id)).ToList();

                var processed = _salaryPeriodAppService.ProcessSalaryPeriod(salaryPeriod, employees, serviceHeader);

                if (!processed)
                    return InternalServerError(new InvalidOperationException("Failed to process the salary period — check that the matched employees have a Basic Pay Earning entry on their salary card."));

                return Ok(new { Processed = true, EmployeeCount = employees.Count });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/close")]
        public IHttpActionResult Close(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryPeriod = _salaryPeriodAppService.FindSalaryPeriod(id, serviceHeader);
                if (salaryPeriod == null)
                    return NotFound();

                if (salaryPeriod.Status != (int)SalaryPeriodStatus.Open)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only an Open salary period can be closed." });

                var closed = _salaryPeriodAppService.CloseSalaryPeriod(salaryPeriod, serviceHeader);

                if (!closed)
                    return InternalServerError(new InvalidOperationException("Failed to close the salary period."));

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ProcessSalaryPeriodRequest
    {
        public List<Guid> SalaryGroupIds { get; set; }
        public List<Guid> BranchIds { get; set; }
        public List<Guid> DepartmentIds { get; set; }
    }
}
