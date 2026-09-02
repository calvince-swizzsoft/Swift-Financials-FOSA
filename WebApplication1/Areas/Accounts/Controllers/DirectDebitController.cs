using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/directdebits")]
    public class DirectDebitController : ApiController
    {
        private readonly IDirectDebitAppService _directDebitAppService;

        public DirectDebitController(IDirectDebitAppService directDebitAppService)
        {
            _directDebitAppService = directDebitAppService ?? throw new ArgumentNullException(nameof(directDebitAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var items = _directDebitAppService.FindDirectDebits(serviceHeader) ?? new List<DirectDebitDTO>();
            _directDebitAppService.FetchDirectDebitsProductDescription(items, serviceHeader);
            return Ok(new { success = true, message = "", data = items });
        }

        [HttpGet]
        [Route("paged")]
        public IHttpActionResult GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? _directDebitAppService.FindDirectDebits(pageIndex, pageSize, serviceHeader)
                : _directDebitAppService.FindDirectDebits(text, pageIndex, pageSize, serviceHeader);
            if (page?.PageCollection != null)
                _directDebitAppService.FetchDirectDebitsProductDescription(page.PageCollection, serviceHeader);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var item = _directDebitAppService.FindDirectDebit(id, serviceHeader);
            if (item == null) return NotFound();
            _directDebitAppService.FetchDirectDebitsProductDescription(new List<DirectDebitDTO> { item }, serviceHeader);
            return Ok(new { success = true, message = "", data = item });
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(DirectDebitDTO directDebit)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var errors = _directDebitAppService.ValidateDirectDebit(directDebit, serviceHeader);
            if (errors.Any())
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", errors), data = (object)null });

            var created = _directDebitAppService.AddNewDirectDebit(directDebit, serviceHeader);
            if (created == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Direct debit could not be created", data = (object)null });
            if (!string.IsNullOrWhiteSpace(created.ErrorMessageResult))
                return Content(HttpStatusCode.Conflict, new { success = false, message = created.ErrorMessageResult, data = (object)null });

            return Ok(new { success = true, message = "Direct debit created successfully", data = created });
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, DirectDebitDTO directDebit)
        {
            if (directDebit != null) directDebit.Id = id;
            var serviceHeader = Utils.CreateServiceHeader();
            var errors = _directDebitAppService.ValidateDirectDebit(directDebit, serviceHeader);
            if (errors.Any())
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", errors), data = (object)null });
            if (!_directDebitAppService.UpdateDirectDebit(directDebit, serviceHeader))
                return NotFound();

            return Ok(new { success = true, message = "Direct debit updated successfully", data = _directDebitAppService.FindDirectDebit(id, serviceHeader) });
        }
    }
}
