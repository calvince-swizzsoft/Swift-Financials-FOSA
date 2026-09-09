using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/alternatechannels")]
    public class AlternateChannelController : ApiController
    {
        private readonly IAlternateChannelAppService _alternateChannelAppService;

        public AlternateChannelController(IAlternateChannelAppService alternateChannelAppService)
        {
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
        }

        [HttpGet]
        [Route("paged")]
        public IHttpActionResult GetPaged(string text = "", int filter = 0, int pageIndex = 0, int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Page index must be non-negative and page size must be between 1 and 100.", data = (object)null });
            if (!Enum.IsDefined(typeof(AlternateChannelFilter), filter))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a valid alternate channel search filter.", data = (object)null });

            var page = _alternateChannelAppService.FindAlternateChannels(text ?? "", filter, pageIndex, pageSize, Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "", data = page });
        }
    }
}
