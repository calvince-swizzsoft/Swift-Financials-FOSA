using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // NavigationMenu.cs Code 22021 ("Salary Groups") — second of the
    // Salary sub-area's three-part dependency chain (Heads -> Groups ->
    // Cards), per the user-supplied WebApplication1/Areas/Salary Groups.md.
    // A group bundles SalaryHead entries with a value (fixed or
    // percentage), a minimum value, and a rounding type.
    //
    // UpdateSalaryGroupEntries (PUT /{id}/entries below) is a full-replace,
    // same convention as ChequeTypeController's /commissions and
    // /attached-products — always send the complete desired entry list, not
    // a diff. Its own diff logic inside SalaryGroupAppService matches
    // entries purely by Id (SalaryGroupEntryDTOEqualityComparer): entries
    // whose Id is already present are left completely untouched even if
    // their other fields differ, entries with Id == Guid.Empty are treated
    // as brand-new inserts, and any persisted Id missing from the new list
    // is deleted. There is no in-place update path for an existing entry's
    // value — Salary Groups.md's own instructions describe this exact
    // remove-then-re-add workflow ("select on the salary group entry and
    // click on the remove button. Re-enter the correct details and click
    // on the add button"), so the frontend must send existing entries back
    // with Id blank if their values changed, not just resend their old Id.
    //
    // Adding a new entry here also auto-creates a matching SalaryCardEntry
    // (seeded from the group's own value) on every SalaryCard already
    // linked to this group, and removing one deletes the matching
    // SalaryCardEntry from every card — real cascading behavior already
    // inside SalaryGroupAppService, not something this controller adds.
    [Authorize]
    [RoutePrefix("api/humanresource/salarygroups")]
    public class SalaryGroupsController : ApiController
    {
        private readonly ISalaryGroupAppService _salaryGroupAppService;

        public SalaryGroupsController(ISalaryGroupAppService salaryGroupAppService)
        {
            _salaryGroupAppService = salaryGroupAppService ?? throw new ArgumentNullException(nameof(salaryGroupAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var salaryGroups = _salaryGroupAppService.FindSalaryGroups(text, pageIndex, pageSize, serviceHeader);

                return Ok(salaryGroups);
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

                var salaryGroup = _salaryGroupAppService.FindSalaryGroup(id, serviceHeader);

                if (salaryGroup == null)
                    return NotFound();

                return Ok(salaryGroup);
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

                var entries = _salaryGroupAppService.FindSalaryGroupEntriesBySalaryGroupId(id, serviceHeader);

                return Ok(entries ?? new List<SalaryGroupEntryDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SalaryGroupDTO salaryGroupDTO)
        {
            try
            {
                if (salaryGroupDTO == null || string.IsNullOrWhiteSpace(salaryGroupDTO.Description))
                    return BadRequest("A Description is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _salaryGroupAppService.AddNewSalaryGroup(salaryGroupDTO, serviceHeader);

                if (created == null)
                    throw new InvalidOperationException("Failed to save the salary group.");

                return Ok(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Update(Guid id, SalaryGroupDTO salaryGroupDTO)
        {
            try
            {
                if (salaryGroupDTO == null || string.IsNullOrWhiteSpace(salaryGroupDTO.Description))
                    return BadRequest("A Description is required.");

                salaryGroupDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _salaryGroupAppService.UpdateSalaryGroup(salaryGroupDTO, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(salaryGroupDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full-replace — see class remarks for the Id-based diff semantics
        // this delegates to.
        [HttpPut]
        [Route("{id:guid}/entries")]
        public IHttpActionResult UpdateEntries(Guid id, List<SalaryGroupEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var group = _salaryGroupAppService.FindSalaryGroup(id, serviceHeader);
                if (group == null)
                    return NotFound();

                var updated = _salaryGroupAppService.UpdateSalaryGroupEntries(id, entries ?? new List<SalaryGroupEntryDTO>(), serviceHeader);

                if (!updated)
                    throw new InvalidOperationException("Failed to save the salary group entries.");

                var refreshed = _salaryGroupAppService.FindSalaryGroupEntriesBySalaryGroupId(id, serviceHeader);

                return Ok(refreshed ?? new List<SalaryGroupEntryDTO>());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
