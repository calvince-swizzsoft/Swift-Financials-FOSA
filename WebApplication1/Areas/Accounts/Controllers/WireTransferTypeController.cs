using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/wiretransfertypes")]
    public class WireTransferTypeController : ApiController
    {
        private readonly IWireTransferTypeAppService _service;

        public WireTransferTypeController(IWireTransferTypeAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200) return BadRequest("Page index and page size are invalid.");
            var header = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? _service.FindWireTransferTypes(pageIndex, pageSize, header)
                : _service.FindWireTransferTypes(text.Trim(), pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            var items = _service.FindWireTransferTypes(Utils.CreateServiceHeader());
            return Ok(new { success = true, message = "", data = items ?? new List<WireTransferTypeDTO>() });
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var item = _service.FindWireTransferType(id, Utils.CreateServiceHeader());
            return item == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data = item });
        }

        [HttpGet, Route("{id:guid}/commissions")]
        public IHttpActionResult GetCommissions(Guid id)
        {
            var header = Utils.CreateServiceHeader();
            if (_service.FindWireTransferType(id, header) == null) return NotFound();
            return Ok(new { success = true, message = "", data = _service.FindCommissions(id, header) ?? new List<CommissionDTO>() });
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(SaveWireTransferTypeRequest request)
        {
            var validation = Validate(request); if (validation != null) return validation;
            var header = Utils.CreateServiceHeader();
            var created = _service.AddNewWireTransferType(request.WireTransferType, header);
            if (created == null || created.Id == Guid.Empty)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = created?.ErrorMessageResult ?? "Wire transfer type could not be created.", data = (object)null });
            if (!_service.UpdateCommissions(created.Id, request.Commissions, header))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Wire transfer type was created, but its applicable charges could not be saved.", data = created });
            return Ok(new { success = true, message = "Wire transfer type created successfully.", data = _service.FindWireTransferType(created.Id, header) });
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, SaveWireTransferTypeRequest request)
        {
            if (request?.WireTransferType != null) request.WireTransferType.Id = id;
            var validation = Validate(request); if (validation != null) return validation;
            var header = Utils.CreateServiceHeader();
            if (_service.FindWireTransferType(id, header) == null) return NotFound();
            if (!_service.UpdateWireTransferType(request.WireTransferType, header))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Wire transfer type could not be updated.", data = (object)null });
            if (!_service.UpdateCommissions(id, request.Commissions, header))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Wire transfer type was updated, but its applicable charges could not be saved.", data = (object)null });
            return Ok(new { success = true, message = "Wire transfer type updated successfully.", data = _service.FindWireTransferType(id, header) });
        }

        private IHttpActionResult Validate(SaveWireTransferTypeRequest request)
        {
            if (request?.WireTransferType == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Wire transfer type data is required.", data = (object)null });
            request.WireTransferType.Description = request.WireTransferType.Description?.Trim();
            request.WireTransferType.ValidateAll();
            if (request.WireTransferType.HasErrors)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.WireTransferType.ErrorMessages), data = (object)null });
            if (!Enum.IsDefined(typeof(TransactionOwnership), request.WireTransferType.TransactionOwnership))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a valid transaction ownership.", data = (object)null });
            if (request.Commissions == null || !request.Commissions.Any(x => x != null && x.Id != Guid.Empty))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select at least one applicable charge.", data = (object)null });
            request.Commissions = request.Commissions.Where(x => x != null && x.Id != Guid.Empty).GroupBy(x => x.Id).Select(x => x.First()).ToList();
            return null;
        }
    }

    public class SaveWireTransferTypeRequest
    {
        public WireTransferTypeDTO WireTransferType { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
    }
}
