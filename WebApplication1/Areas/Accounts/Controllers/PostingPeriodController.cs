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
    // NavigationMenu.cs has two separate entries backed by this one app
    // service: "Posting Periods" (Accounts > Setup, Code 23019,
    // ControllerName "PostingPeriod" — the CRUD screen, Create/Update below)
    // and "Posting Period Closing" (Accounts > Operations > Transactions
    // Journal, Code 23034, ControllerName "ClosingPostingPeriod" — Close
    // below). IPostingPeriodAppService was already fully built and
    // registered in DI; neither had a REST controller.
    //
    // Close is a real, hard-to-reverse financial operation, not a status
    // flag flip — ClosePostingPeriod computes every income/expense G/L
    // account's balance per branch as of the period's end date and posts
    // real double-entry fiscal-period-closing journals against the
    // Appropriation account (see PostingPeriodAppService.ClosePostingPeriod).
    // It returns false (not an exception) if the Appropriation G/L mapping
    // isn't configured or there are no branches — Close below surfaces that
    // distinctly rather than a generic failure message, since it's a setup
    // problem the caller can actually fix.
    [Authorize]
    [RoutePrefix("api/accounts/postingperiods")]
    public class PostingPeriodController : ApiController
    {
        // Fixed to the "Posting Period Closing" nav Code — this controller
        // only ever closes periods from that one screen, so there's no
        // reason to trust a client-supplied module code for what's
        // ultimately a journal-reference/audit value.
        private const int ClosingPostingPeriodModuleCode = 0x000059D8 + 34;

        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public PostingPeriodController(IPostingPeriodAppService postingPeriodAppService)
        {
            _postingPeriodAppService = postingPeriodAppService ?? throw new ArgumentNullException(nameof(postingPeriodAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var postingPeriods = _postingPeriodAppService.FindPostingPeriods(serviceHeader);

                return Ok(new { success = true, message = "", data = postingPeriods ?? new System.Collections.Generic.List<PostingPeriodDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _postingPeriodAppService.FindPostingPeriods(pageIndex, pageSize, serviceHeader)
                    : _postingPeriodAppService.FindPostingPeriods(text, pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var postingPeriod = _postingPeriodAppService.FindPostingPeriod(id, serviceHeader);

                if (postingPeriod == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = postingPeriod });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(PostingPeriodDTO postingPeriod)
        {
            try
            {
                if (postingPeriod == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Posting period payload is required." });

                postingPeriod.ValidateAll();
                if (postingPeriod.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", postingPeriod.ErrorMessages) });

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _postingPeriodAppService.AddNewPostingPeriod(postingPeriod, serviceHeader);

                if (created == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the posting period could not be created." });

                return Ok(new { success = true, message = "Operation Success", data = created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, PostingPeriodDTO postingPeriod)
        {
            try
            {
                if (postingPeriod == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Posting period payload is required." });

                postingPeriod.Id = id;
                postingPeriod.ValidateAll();
                if (postingPeriod.HasErrors)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", postingPeriod.ErrorMessages) });

                var serviceHeader = Utils.CreateServiceHeader();

                // UpdatePostingPeriod silently no-ops (returns false) once the
                // period is closed — same "closed periods are immutable" rule
                // Close below establishes.
                var updated = _postingPeriodAppService.UpdatePostingPeriod(postingPeriod, serviceHeader);

                if (!updated)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Failed to update the posting period — it may already be closed." });

                var refreshed = _postingPeriodAppService.FindPostingPeriod(id, serviceHeader);
                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/close")]
        public async Task<IHttpActionResult> Close(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var postingPeriod = _postingPeriodAppService.FindPostingPeriod(id, serviceHeader);
                if (postingPeriod == null)
                    return NotFound();

                if (postingPeriod.IsClosed)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "This posting period is already closed." });

                var closed = _postingPeriodAppService.ClosePostingPeriod(postingPeriod, ClosingPostingPeriodModuleCode, serviceHeader);

                if (!closed)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Failed to close the posting period — check that at least one branch exists and the Appropriation G/L account mapping is configured." });

                var refreshed = _postingPeriodAppService.FindPostingPeriod(id, serviceHeader);
                return Ok(new { success = true, message = "Posting period closed successfully", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
