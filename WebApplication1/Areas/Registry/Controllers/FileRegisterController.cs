using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Registry.Controllers
{
    // REST adaptation of the legacy Registry Receive, Recall,
    // SingleDestination and MultiDestination MVC screens. Those controllers
    // only staged view models; FileRegisterAppService owns the real file
    // lifecycle and remains the source of business behaviour here.
    [Authorize]
    [RoutePrefix("api/registry/fileregisters")]
    public class FileRegisterController : ApiController
    {
        private readonly IFileRegisterAppService _fileRegisterAppService;

        public FileRegisterController(IFileRegisterAppService fileRegisterAppService)
        {
            _fileRegisterAppService = fileRegisterAppService ?? throw new ArgumentNullException(nameof(fileRegisterAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", int customerFilter = 0, int pageIndex = 0, int pageSize = 20, int? status = null, Guid? departmentId = null)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 1000)
                return BadRequest("pageIndex must be non-negative and pageSize must be between 1 and 1000");

            try
            {
                var header = Utils.CreateServiceHeader();
                var page = status.HasValue && departmentId.HasValue && departmentId.Value != Guid.Empty
                    ? _fileRegisterAppService.FindFileRegisters(status.Value, departmentId.Value, text ?? "", customerFilter, pageIndex, pageSize, header)
                    : _fileRegisterAppService.FindFileRegisters(text ?? "", customerFilter, pageIndex, pageSize, header);
                return Ok(Envelope("", page));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var item = _fileRegisterAppService.FindFileRegister(id, Utils.CreateServiceHeader());
                return item == null ? (IHttpActionResult)NotFound() : Ok(Envelope("", item));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet]
        [Route("customers/{customerId:guid}")]
        public IHttpActionResult GetByCustomer(Guid customerId)
        {
            try
            {
                var item = _fileRegisterAppService.FindFileRegisterAndLastDepartmentByCustomerId(customerId, Utils.CreateServiceHeader());
                return item == null || item.FileRegister == null ? (IHttpActionResult)NotFound() : Ok(Envelope("", item));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpGet]
        [Route("{id:guid}/history")]
        public IHttpActionResult History(Guid id, int pageIndex = 0, int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 1000)
                return BadRequest("pageIndex must be non-negative and pageSize must be between 1 and 1000");
            try
            {
                var page = _fileRegisterAppService.FindFileMovementHistoryByFileRegisterId(id, pageIndex, pageSize, Utils.CreateServiceHeader());
                return Ok(Envelope("", page));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpPost]
        [Route("dispatch")]
        public IHttpActionResult Dispatch(DispatchFilesRequest request)
        {
            if (request == null || request.CustomerIds == null || !request.CustomerIds.Any()) return BadRequest("At least one customer is required");
            if (request.SourceDepartmentId == Guid.Empty || request.DestinationDepartmentId == Guid.Empty) return BadRequest("Source and destination departments are required");
            if (request.SourceDepartmentId == request.DestinationDepartmentId) return BadRequest("Source and destination departments must be different");
            if (string.IsNullOrWhiteSpace(request.Carrier)) return BadRequest("Carrier is required");

            try
            {
                var movements = request.CustomerIds.Distinct().Select(customerId => new FileMovementHistoryDTO
                {
                    FileRegisterCustomerId = customerId,
                    SourceDepartmentId = request.SourceDepartmentId,
                    DestinationDepartmentId = request.DestinationDepartmentId,
                    Remarks = request.Remarks,
                    Carrier = request.Carrier
                }).ToList();
                var saved = _fileRegisterAppService.MultiDestinationDispatch(movements, Utils.CreateServiceHeader());
                if (saved)
                    return Ok(Envelope("Files dispatched successfully", null));

                return Content(HttpStatusCode.Conflict, Envelope("Files could not be dispatched", null, false));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        [HttpPost]
        [Route("receive")]
        public IHttpActionResult Receive(FileRegisterSelectionRequest request) => ChangeStatus(request, true);

        [HttpPost]
        [Route("recall")]
        public IHttpActionResult Recall(FileRegisterSelectionRequest request) => ChangeStatus(request, false);

        private IHttpActionResult ChangeStatus(FileRegisterSelectionRequest request, bool receive)
        {
            if (request == null || request.FileRegisterIds == null || !request.FileRegisterIds.Any()) return BadRequest("At least one file register is required");
            try
            {
                var header = Utils.CreateServiceHeader();
                var files = request.FileRegisterIds.Distinct().Select(id => _fileRegisterAppService.FindFileRegister(id, header)).Where(x => x != null).ToList();
                if (files.Count != request.FileRegisterIds.Distinct().Count()) return BadRequest("One or more file registers were not found");
                var saved = receive ? _fileRegisterAppService.ReceiveFiles(files, header) : _fileRegisterAppService.RecallFiles(files, header);
                var action = receive ? "received" : "recalled";
                if (saved)
                    return Ok(Envelope($"Files {action} successfully", null));

                return Content(HttpStatusCode.Conflict, Envelope($"Files could not be {action}", null, false));
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        private object Envelope(string message, object data, bool success = true) => new { success, message, data };
    }

    public class DispatchFilesRequest
    {
        public List<Guid> CustomerIds { get; set; }
        public Guid SourceDepartmentId { get; set; }
        public Guid DestinationDepartmentId { get; set; }
        public string Remarks { get; set; }
        public string Carrier { get; set; }
    }

    public class FileRegisterSelectionRequest
    {
        public List<Guid> FileRegisterIds { get; set; }
    }
}
