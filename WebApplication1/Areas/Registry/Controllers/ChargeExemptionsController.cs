using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // NavigationMenu.cs Code 21010 ("Charges Exemptions", ControllerName
    // "ChargeExemptions", under Registry > Operations > Customers) had no
    // domain/DTO/table of its own anywhere in the codebase — but
    // ICommissionExemptionAppService (Application.MainBoundedContext/RegistryModule/Services)
    // already fully implements exactly this concept (which customers are
    // exempt from which commission charge), domain/DTO/AutoMapper-complete,
    // registered in DI, just missing a REST controller. This exposes it
    // under the "charge exemptions" naming the nav uses — same reasoning as
    // moduleRouteMap.js Code 23009 ("Charges") pointing at the Commissions
    // screen: this codebase's legacy nav calls Commission-related things
    // "Charges".
    //
    // Model: a CommissionExemption is a named exemption group tied to
    // exactly one commission (AddNewCommissionExemptionAsync rejects a
    // second exemption group for the same CommissionId); a
    // CommissionExemptionEntry is one customer's membership in that group
    // (a customer with an entry is exempt from that commission — see
    // CommissionExemptionAppService.FetchCustomerCommissionExemptionStatus,
    // consulted elsewhere at charge-calculation time).
    [Authorize]
    [RoutePrefix("api/registry/chargeexemptions")]
    public class ChargeExemptionsController : ApiController
    {
        private readonly ICommissionExemptionAppService _commissionExemptionAppService;

        public ChargeExemptionsController(ICommissionExemptionAppService commissionExemptionAppService)
        {
            _commissionExemptionAppService = commissionExemptionAppService ?? throw new ArgumentNullException(nameof(commissionExemptionAppService));
        }

        // Unpaged — reserved for pickers, same convention as
        // CommissionController's GET "" vs GET "paged" split.
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var items = await _commissionExemptionAppService.FindCommissionExemptionsAsync(serviceHeader) ?? new List<CommissionExemptionDTO>();

                return Ok(new { success = true, message = "", data = items });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = await _commissionExemptionAppService.FindCommissionExemptionsAsync(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var item = await _commissionExemptionAppService.FindCommissionExemptionAsync(id, serviceHeader);

                if (item == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = item });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CommissionExemptionDTO commissionExemption)
        {
            try
            {
                if (commissionExemption == null)
                    return BadRequest("Charge exemption payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = await _commissionExemptionAppService.AddNewCommissionExemptionAsync(commissionExemption, serviceHeader);

                if (created == null)
                    return InternalServerError(new InvalidOperationException("Failed to create the charge exemption."));

                return Ok(new { success = true, message = "", data = created });
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
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, CommissionExemptionDTO commissionExemption)
        {
            try
            {
                if (commissionExemption == null)
                    return BadRequest("Charge exemption payload is required.");

                commissionExemption.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _commissionExemptionAppService.UpdateCommissionExemptionAsync(commissionExemption, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(new { success = true, message = "", data = await _commissionExemptionAppService.FindCommissionExemptionAsync(id, serviceHeader) });
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

        [HttpGet]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> GetEntries(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var entries = await _commissionExemptionAppService.FindCommissionExemptionEntriesByCommissionExemptionIdAsync(id, serviceHeader) ?? new List<CommissionExemptionEntryDTO>();

                return Ok(new { success = true, message = "", data = entries });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full replace on save — per Areas/Registry/Charges Exemptions.md's
        // edit flow ("adding or removing customers... click the update
        // button to save changes"), the UI stages customer add/remove
        // locally and commits everything in one call, same shape
        // AlertPreferencesDrawer.jsx already uses for account alerts. Backs
        // onto UpdateCommissionExemptionEntryCollectionAsync, which deletes
        // every existing entry for this exemption and re-inserts exactly
        // what's submitted — there's no per-row Id to track client-side.
        [HttpPut]
        [Route("{id:guid}/entries")]
        public async Task<IHttpActionResult> ReplaceEntries(Guid id, List<CommissionExemptionEntryDTO> entries)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = await _commissionExemptionAppService.UpdateCommissionExemptionEntryCollectionAsync(id, entries ?? new List<CommissionExemptionEntryDTO>(), serviceHeader);

                if (!updated)
                    return NotFound();

                var current = await _commissionExemptionAppService.FindCommissionExemptionEntriesByCommissionExemptionIdAsync(id, serviceHeader) ?? new List<CommissionExemptionEntryDTO>();

                return Ok(new { success = true, message = "", data = current });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
