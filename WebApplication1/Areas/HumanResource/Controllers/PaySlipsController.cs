using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Code 22025 ("Payslips") — the browse/detail side of
    // IPaySlipAppService, plus the actual posting trigger. PostPaySlip
    // itself lives on ISalaryPeriodAppService, not IPaySlipAppService — it
    // does real G/L journal posting and, if the parent Salary Period has
    // ExecutePayoutStandingOrders set, queues standing-order payouts too
    // (see SalaryPeriodsController remarks). This is the actual
    // money-movement step; ProcessSalaryPeriod (SalaryPeriodsController)
    // only ever stages Pending payslips, it never posts anything.
    //
    // The moduleNavigationItemCode PostPaySlip takes is fixed to this
    // controller's own nav Code, never accepted from the client — it only
    // exists to attribute the resulting G/L journals to the right nav item
    // for audit purposes, not to select behavior.
    [Authorize]
    [RoutePrefix("api/humanresource/payslips")]
    public class PaySlipsController : ApiController
    {
        private const int PayslipsModuleNavigationItemCode = 22025;

        private readonly IPaySlipAppService _paySlipAppService;
        private readonly ISalaryPeriodAppService _salaryPeriodAppService;

        public PaySlipsController(
            IPaySlipAppService paySlipAppService,
            ISalaryPeriodAppService salaryPeriodAppService)
        {
            _paySlipAppService = paySlipAppService ?? throw new ArgumentNullException(nameof(paySlipAppService));
            _salaryPeriodAppService = salaryPeriodAppService ?? throw new ArgumentNullException(nameof(salaryPeriodAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(Guid salaryPeriodId, string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                if (salaryPeriodId == Guid.Empty)
                    return BadRequest("A salaryPeriodId is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var paySlips = _paySlipAppService.FindPaySlipsBySalaryPeriodId(salaryPeriodId, text, pageIndex, pageSize, serviceHeader);

                return Ok(paySlips);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult Summary(Guid salaryPeriodId)
        {
            try
            {
                if (salaryPeriodId == Guid.Empty)
                    return BadRequest("A salaryPeriodId is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var total = _paySlipAppService.CountPaySlipsBySalaryPeriodId(salaryPeriodId, serviceHeader);
                var posted = _paySlipAppService.CountPostedPaySlipsBySalaryPeriodId(salaryPeriodId, serviceHeader);

                return Ok(new { Total = total, Posted = posted, Pending = total - posted });
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

                var paySlip = _paySlipAppService.FindPaySlip(id, serviceHeader);

                if (paySlip == null)
                    return NotFound();

                return Ok(paySlip);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/entries")]
        public IHttpActionResult Entries(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = _paySlipAppService.FindPaySlipEntriesByPaySlipId(id, serviceHeader);

                return Ok(entries ?? new List<PaySlipEntryDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/post")]
        public IHttpActionResult Post(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var paySlip = _paySlipAppService.FindPaySlip(id, serviceHeader);
                if (paySlip == null)
                    return NotFound();

                if (paySlip.Status != (int)PaySlipStatus.Pending)
                    return Content(HttpStatusCode.Conflict, new { Message = "Only a Pending payslip can be posted." });

                var posted = _salaryPeriodAppService.PostPaySlip(id, PayslipsModuleNavigationItemCode, serviceHeader);

                if (!posted)
                    return Content(HttpStatusCode.Conflict, new { Message = "The payslip cannot be posted until the employee has a Basic Pay earning and a resolvable payroll savings account." });

                var refreshed = _paySlipAppService.FindPaySlip(id, serviceHeader);

                return Ok(refreshed);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
