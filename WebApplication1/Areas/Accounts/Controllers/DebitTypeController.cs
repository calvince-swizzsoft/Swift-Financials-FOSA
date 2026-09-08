using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/debittypes")]
    public class DebitTypeController : ApiController
    {
        private readonly IDebitTypeAppService _service;
        public DebitTypeController(IDebitTypeAppService service) { _service = service ?? throw new ArgumentNullException(nameof(service)); }
        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            var header = Utils.CreateServiceHeader();
            var items = _service.FindDebitTypes(header) ?? new List<DebitTypeDTO>();
            _service.FetchDebitTypesProductDescription(items, header);
            return Ok(new { success = true, message = "", data = items });
        }
        [HttpGet, Route("paged")]
        public IHttpActionResult GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200) return Error("Invalid pagination; page size must be between 1 and 200.");
            var header = Utils.CreateServiceHeader();
            var page = _service.FindDebitTypes(text ?? "", pageIndex, pageSize, header)
                ?? new PageCollectionInfo<DebitTypeDTO> { PageCollection = new List<DebitTypeDTO>(), ItemsCount = 0 };
            _service.FetchDebitTypesProductDescription(page.PageCollection, header);
            return Ok(new { success = true, message = "", data = page });
        }
        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var header = Utils.CreateServiceHeader();
            var item = _service.FindDebitType(id, header);
            if (item == null) return NotFound();
            _service.FetchDebitTypesProductDescription(new List<DebitTypeDTO> { item }, header);
            return Ok(new { success = true, message = "", data = item });
        }
        [HttpGet, Route("{id:guid}/configuration")]
        public IHttpActionResult GetConfiguration(Guid id)
        {
            var header = Utils.CreateServiceHeader();
            if (_service.FindDebitType(id, header) == null) return NotFound();
            return Ok(new { success = true, message = "", data = new { Commissions = _service.FindCommissions(id, header) ?? new List<CommissionDTO>() } });
        }
        [HttpPost, Route("")]
        public IHttpActionResult Create(SaveDebitTypeRequest request) { return Save(Guid.Empty, request); }
        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, SaveDebitTypeRequest request) { return Save(id, request); }
        private IHttpActionResult Save(Guid id, SaveDebitTypeRequest request)
        {
            if (request?.DebitType == null) return Error("Debit type data is required.");
            var header = Utils.CreateServiceHeader();
            if (id != Guid.Empty && _service.FindDebitType(id, header) == null) return NotFound();
            request.DebitType.Id = id;
            try
            {
                var saved = _service.SaveConfiguredDebitType(request.DebitType, request.Commissions, header);
                return Ok(new { success = true, message = "Debit type saved successfully.", data = saved });
            }
            catch (ArgumentException exception) { return Error(exception.Message); }
        }
        private IHttpActionResult Error(string message)
        {
            return Content(HttpStatusCode.BadRequest, new { success = false, message, data = (object)null });
        }
    }
    public class SaveDebitTypeRequest
    {
        public DebitTypeDTO DebitType { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
    }
}
