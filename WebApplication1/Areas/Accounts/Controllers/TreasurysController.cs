using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/treasurys")]
    public class TreasurysController : ApiController
    {

        private readonly ITreasuryAppService _treasuryAppService;

        public TreasurysController (ITreasuryAppService treasuryAppService)
        {
            _treasuryAppService = treasuryAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var treasuries = _treasuryAppService.FindTreasuries(text ?? "", pageIndex, pageSize, serviceHeader);

                if (treasuries == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = treasuries });
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetTreasury(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var treasury = _treasuryAppService.FindTreasury(id, serviceHeader);

                if (treasury == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = treasury });
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(TreasuryDTO treasuryDTO)
        {
            try
            {
                if (treasuryDTO == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Treasury details are required.", data = (object)null });

                treasuryDTO.ValidateAll();


                var serviceHeader = Utils.CreateServiceHeader();

                if (!treasuryDTO.HasErrors)
                {

                    var createdTreasuryDTO = _treasuryAppService.AddNewTreasury(treasuryDTO, serviceHeader);

                    if (createdTreasuryDTO == null)
                    {
                        return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the treasury could not be created.", data = (object)null });
                    }

                    // AddNewTreasury reports the one-treasury-per-branch and
                    // unique-description business rules by echoing the submitted
                    // DTO back with ErrorMessageResult set (Id stays Guid.Empty) —
                    // it doesn't throw and doesn't populate ValidateAll()'s error
                    // list, so this has to be checked separately.
                    if (!string.IsNullOrWhiteSpace(createdTreasuryDTO.ErrorMessageResult))
                    {
                        return Content(HttpStatusCode.Conflict, new { success = false, message = createdTreasuryDTO.ErrorMessageResult, data = (object)null });
                    }

                    return Ok(new { success = true, message = "Operation Success", data = createdTreasuryDTO });
                }

                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", treasuryDTO.ErrorMessages), data = (object)null });
                }

            }

            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateTreasury(Guid id, TreasuryDTO treasuryDTO)
        {
            try
            {
                if (treasuryDTO == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Treasury details are required.", data = (object)null });

                treasuryDTO.Id = id;
                treasuryDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (!treasuryDTO.HasErrors)
                {
                    // UpdateTreasury returns bool, not the updated entity — re-fetch
                    // so `data` is actually useful (and reflects what was really
                    // saved, e.g. BranchId is silently ignored on update server-side).
                    var updated = _treasuryAppService.UpdateTreasury(treasuryDTO, serviceHeader);

                    if (!updated)
                    {
                        return NotFound();
                    }

                    var refreshedTreasuryDTO = _treasuryAppService.FindTreasury(id, serviceHeader);

                    return Ok(new { success = true, message = "Operation Success", data = refreshedTreasuryDTO });
                }

                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", treasuryDTO.ErrorMessages), data = (object)null });
                }
            }

            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }
}
