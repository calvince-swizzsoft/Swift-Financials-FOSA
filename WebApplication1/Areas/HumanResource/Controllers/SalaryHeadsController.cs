using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Code 22020 ("Salary Heads", ControllerName: Salary,
    // under HumanResource > Operations > Salary) — same gap as Holidays/
    // Leave: ISalaryHeadAppService was already fully built and DI-registered
    // but had no REST controller. First of the Salary sub-area's three
    // pieces (Heads -> Groups -> Cards, in that dependency order per the
    // user-supplied WebApplication1/Areas/Salary Heads.md — a salary head is
    // an Earning or Deduction pay-structure component; Groups bundle heads
    // with per-group values; Cards link an employee to a group). Groups/
    // Cards are their own follow-up passes, not built here.
    //
    // AddNewSalaryHead has a real business rule the DTO/binding model don't
    // express: seven of the SalaryHeadType values (the statutory/basic-pay
    // ones — see the switch in SalaryHeadAppService.cs) may only exist ONCE
    // system-wide; AddNewSalaryHead silently returns null both for an
    // invalid Type and for a disallowed duplicate of one of those seven,
    // with no way to tell which from the return value alone — this
    // controller checks Type validity itself first so a null result can
    // only mean "duplicate singleton type", and reports that distinctly.
    //
    // CustomerAccountTypeTargetProductCode (the linked product's own
    // denormalized Code) is taken as given from the client rather than
    // re-resolved from CustomerAccountTypeProductCode/TargetProductId
    // against SavingsProduct/LoanProduct/InvestmentProduct — it's cosmetic
    // (the real FK is TargetProductId), and the frontend picker only ever
    // sources it from the matching real product list endpoint, never free
    // typed.
    [Authorize]
    [RoutePrefix("api/humanresource/salaryheads")]
    public class SalaryHeadsController : ApiController
    {
        private readonly ISalaryHeadAppService _salaryHeadAppService;

        public SalaryHeadsController(ISalaryHeadAppService salaryHeadAppService)
        {
            _salaryHeadAppService = salaryHeadAppService ?? throw new ArgumentNullException(nameof(salaryHeadAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryHeads = _salaryHeadAppService.FindSalaryHeads(text, pageIndex, pageSize, serviceHeader);

                return Ok(salaryHeads);
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

                var salaryHead = _salaryHeadAppService.FindSalaryHead(id, serviceHeader);

                if (salaryHead == null)
                    return NotFound();

                return Ok(salaryHead);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SalaryHeadDTO salaryHeadDTO)
        {
            try
            {
                if (salaryHeadDTO == null)
                    return BadRequest("Salary head payload is required.");

                if (!Enum.IsDefined(typeof(SalaryHeadType), salaryHeadDTO.Type))
                    return BadRequest("A valid Type is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _salaryHeadAppService.AddNewSalaryHead(salaryHeadDTO, serviceHeader);

                if (created == null)
                    return Content(System.Net.HttpStatusCode.Conflict, new { Message = "A salary head of this type already exists — this type only allows one." });

                return Ok(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, SalaryHeadDTO salaryHeadDTO)
        {
            try
            {
                if (salaryHeadDTO == null)
                    return BadRequest("Salary head payload is required.");

                if (!Enum.IsDefined(typeof(SalaryHeadType), salaryHeadDTO.Type))
                    return BadRequest("A valid Type is required.");

                salaryHeadDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _salaryHeadAppService.UpdateSalaryHead(salaryHeadDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(salaryHeadDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
