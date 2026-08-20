using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using System;
using System.Linq;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // NavigationMenu.cs Code 21008 ("Next-Of-Kin", ControllerName
    // "NextOfKin", under Registry > Operations > Customers). Backed by
    // INextOfKinAppService (Application.MainBoundedContext/RegistryModule/Services) —
    // NOT WebApplication1/Services/NextOfKinService.cs, a raw-ADO.NET class
    // in the presentation project that bypassed the repository/AutoMapper
    // layers every other module here goes through; that class is dead code
    // as of this controller (still referenced by the legacy, already-dead
    // CustomersController.cs bundled-registration flow — see
    // AlertPreferencesDrawer.jsx's comment on that controller — but not by
    // anything current). A per-customer sub-resource, same shape as Account
    // Alerts (CustomerController.cs's /account-alerts routes) and
    // CustomerDocumentController's picker — every real operation here
    // (FindNextOfKins/Add/Update/percentage validation) is scoped to one
    // customer at a time, not a standalone browse-all screen.
    [Authorize]
    [RoutePrefix("api/registry/nextofkin")]
    public class NextOfKinController : ApiController
    {
        private readonly INextOfKinAppService _nextOfKinAppService;

        public NextOfKinController(INextOfKinAppService nextOfKinAppService)
        {
            _nextOfKinAppService = nextOfKinAppService ?? throw new ArgumentNullException(nameof(nextOfKinAppService));
        }

        // customerId required — this is a per-customer picker, not a
        // general browse (see class remarks). Bundles a percentage summary
        // alongside the list, computed here from the same rows, since every
        // caller needs both to render the "remaining allocation" state.
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetByCustomer(Guid customerId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var items = _nextOfKinAppService.FindNextOfKins(customerId, serviceHeader) ?? new System.Collections.Generic.List<NextOfKinDTO>();

                var totalPercentage = items.Sum(x => x.NominatedPercentage);
                var summary = new
                {
                    TotalNextOfKins = items.Count,
                    TotalPercentage = totalPercentage,
                    RemainingPercentage = Math.Max(0, 100 - totalPercentage),
                };

                return Ok(new { success = true, message = "", data = new { items, summary } });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(NextOfKinDTO nextOfKin)
        {
            try
            {
                if (nextOfKin == null)
                    return BadRequest("Next of kin payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _nextOfKinAppService.AddNewNextOfKin(nextOfKin, serviceHeader);

                return Ok(new { success = true, message = "", data = created });
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
        public IHttpActionResult Update(Guid id, NextOfKinDTO nextOfKin)
        {
            try
            {
                if (nextOfKin == null)
                    return BadRequest("Next of kin payload is required.");

                var serviceHeader = Utils.CreateServiceHeader();

                nextOfKin.Id = id;
                var updated = _nextOfKinAppService.UpdateNextOfKin(nextOfKin, serviceHeader);

                if (!updated)
                    return NotFound();

                return Ok(new { success = true, message = "", data = _nextOfKinAppService.FindNextOfKin(id, serviceHeader) });
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

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var removed = _nextOfKinAppService.RemoveNextOfKin(id, serviceHeader);

                if (!removed)
                    return NotFound();

                return Ok(new { success = true, message = "" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
