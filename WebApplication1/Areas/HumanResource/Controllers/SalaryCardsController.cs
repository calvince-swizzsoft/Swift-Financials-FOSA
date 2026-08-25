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
    // NavigationMenu.cs Code 22022 ("Salary Cards") — third of the Salary
    // sub-area's dependency chain (Heads -> Groups -> Cards), per the
    // user-supplied WebApplication1/Areas/Salary Cards.md. A card links one
    // employee to one salary group; AddNewSalaryCard enforces at most one
    // card per employee (returns null on a second attempt — reported here
    // as 409) and, on creation, auto-generates a SalaryCardEntry for every
    // entry currently on the chosen group, seeded with the group's own
    // value as the starting card value — the per-employee override the
    // .md doc calls "Card value" (vs "Group Value") is edited afterward via
    // PUT entries/{entryId}, one entry at a time, not through the card
    // itself.
    //
    // ZeroizeOneOffEarnings (also on ISalaryCardAppService) is deliberately
    // not exposed here — it's a Salary Processing/Payslips-cycle operation
    // (clearing one-off earnings after they've been paid out), not part of
    // the Cards.md-described card-management workflow this controller
    // covers.
    [Authorize]
    [RoutePrefix("api/humanresource/salarycards")]
    public class SalaryCardsController : ApiController
    {
        private readonly ISalaryCardAppService _salaryCardAppService;

        public SalaryCardsController(ISalaryCardAppService salaryCardAppService)
        {
            _salaryCardAppService = salaryCardAppService ?? throw new ArgumentNullException(nameof(salaryCardAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryCards = _salaryCardAppService.FindSalaryCards(text, pageIndex, pageSize, serviceHeader);

                return Ok(salaryCards);
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

                var salaryCard = _salaryCardAppService.FindSalaryCard(id, serviceHeader);

                if (salaryCard == null)
                    return NotFound();

                return Ok(salaryCard);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Lets the create form check "does this employee already have a
        // card" up front, before a doomed submit.
        [HttpGet]
        [Route("by-employee/{employeeId:guid}")]
        public IHttpActionResult GetByEmployee(Guid employeeId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryCard = _salaryCardAppService.FindSalaryCardByEmployeeId(employeeId, serviceHeader);

                if (salaryCard == null)
                    return NotFound();

                return Ok(salaryCard);
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

                var entries = _salaryCardAppService.FindSalaryCardEntriesBySalaryCardId(id, serviceHeader);

                return Ok(entries ?? new List<SalaryCardEntryDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SalaryCardDTO salaryCardDTO)
        {
            try
            {
                if (salaryCardDTO == null)
                    return BadRequest("Salary card payload is required.");

                if (salaryCardDTO.EmployeeId == Guid.Empty)
                    return BadRequest("An Employee is required.");

                if (salaryCardDTO.SalaryGroupId == Guid.Empty)
                    return BadRequest("A Salary Group is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _salaryCardAppService.AddNewSalaryCard(salaryCardDTO, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.Conflict, new { Message = "This employee already has a salary card." });

                return Ok(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, SalaryCardDTO salaryCardDTO)
        {
            try
            {
                if (salaryCardDTO == null)
                    return BadRequest("Salary card payload is required.");

                if (salaryCardDTO.SalaryGroupId == Guid.Empty)
                    return BadRequest("A Salary Group is required.");

                salaryCardDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _salaryCardAppService.UpdateSalaryCard(salaryCardDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(salaryCardDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Wipes and regenerates this card's entries from its (possibly
        // just-reassigned) SalaryGroupId's current entries — the .md doc's
        // "reset button" (§ How to edit a salary card, step 2).
        [HttpPost]
        [Route("{id:guid}/reset-entries")]
        public IHttpActionResult ResetEntries(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryCard = _salaryCardAppService.FindSalaryCard(id, serviceHeader);
                if (salaryCard == null)
                    return NotFound();

                var reset = _salaryCardAppService.ResetSalaryCardEntries(salaryCard, serviceHeader);

                if (!reset)
                    return InternalServerError(new InvalidOperationException("Failed to reset the salary card entries."));

                var refreshed = _salaryCardAppService.FindSalaryCardEntriesBySalaryCardId(id, serviceHeader);

                return Ok(refreshed ?? new List<SalaryCardEntryDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Updates one entry's own card-level override (ChargeType/
        // ChargePercentage/ChargeFixedAmount) — the "Card value" the .md
        // doc describes modifying per salary head, distinct from the
        // group's own value.
        [HttpPut]
        [Route("entries/{entryId:guid}")]
        public IHttpActionResult UpdateEntry(Guid entryId, SalaryCardEntryDTO salaryCardEntryDTO)
        {
            try
            {
                if (salaryCardEntryDTO == null)
                    return BadRequest("Salary card entry payload is required.");

                salaryCardEntryDTO.Id = entryId;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _salaryCardAppService.UpdateSalaryCardEntry(salaryCardEntryDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(salaryCardEntryDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
