using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/administration/banks")]
    public class BankController : ApiController
    {
        private readonly IBankAppService _bankAppService;

        public BankController(IBankAppService bankAppService)
        {
            _bankAppService = bankAppService ?? throw new ArgumentNullException(nameof(bankAppService));
        }

        private IHttpActionResult ApiResponse(bool success, string message, object data = null)
        {
            return Ok(new { success, message, data });
        }

        private IHttpActionResult ErrorResponse(HttpStatusCode statusCode, string message)
        {
            return Content(statusCode, new { success = false, message });
        }

        [HttpGet, Route("")]
        public IHttpActionResult Get([FromUri] int pageIndex = 0, [FromUri] int pageSize = 20, [FromUri] string text = "")
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _bankAppService.FindBanks(pageIndex, pageSize, serviceHeader)
                    : _bankAppService.FindBanks(text, pageIndex, pageSize, serviceHeader);

                return ApiResponse(true, "Banks retrieved successfully", page);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var banks = _bankAppService.FindBanks(serviceHeader);

                return ApiResponse(true, "Banks retrieved successfully", banks ?? Enumerable.Empty<BankDTO>());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var bank = _bankAppService.FindBank(id, serviceHeader);
                if (bank == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Bank not found");

                return ApiResponse(true, "Bank retrieved successfully", bank);
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("{id:guid}/branches")]
        public IHttpActionResult GetBranches(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var bank = _bankAppService.FindBank(id, serviceHeader);
                if (bank == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Bank not found");

                var branches = _bankAppService.FindBankBranches(id, serviceHeader);

                return ApiResponse(true, "Bank branches retrieved successfully", branches ?? Enumerable.Empty<BankBranchDTO>().ToList());
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create([FromBody] CreateBankRequest request)
        {
            try
            {
                if (request?.Bank == null)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid bank data");

                request.Bank.ValidateAll();
                if (request.Bank.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join(" ", request.Bank.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var createdBank = _bankAppService.AddNewBank(request.Bank, serviceHeader);
                if (createdBank == null)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to create bank");

                var branches = request.Branches != null && request.Branches.Any()
                    && _bankAppService.UpdateBankBranches(createdBank.Id, request.Branches, serviceHeader)
                        ? _bankAppService.FindBankBranches(createdBank.Id, serviceHeader)
                        : new List<BankBranchDTO>();

                return Content(HttpStatusCode.Created, new
                {
                    success = true,
                    message = "Bank created successfully",
                    data = new { bank = createdBank, branches }
                });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] CreateBankRequest request)
        {
            try
            {
                if (request?.Bank == null || request.Bank.Id != id)
                    return ErrorResponse(HttpStatusCode.BadRequest, "Invalid bank data");

                request.Bank.ValidateAll();
                if (request.Bank.HasErrors)
                    return ErrorResponse(HttpStatusCode.BadRequest, string.Join(" ", request.Bank.ErrorMessages));

                var serviceHeader = Utils.CreateServiceHeader();

                var existingBank = _bankAppService.FindBank(id, serviceHeader);
                if (existingBank == null)
                    return ErrorResponse(HttpStatusCode.NotFound, "Bank not found");

                var updated = _bankAppService.UpdateBank(request.Bank, serviceHeader);
                if (!updated)
                    return ErrorResponse(HttpStatusCode.InternalServerError, "Failed to update bank");

                if (request.Branches != null)
                    _bankAppService.UpdateBankBranches(id, request.Branches, serviceHeader);

                var updatedBank = _bankAppService.FindBank(id, serviceHeader);
                var branches = _bankAppService.FindBankBranches(id, serviceHeader) ?? new List<BankBranchDTO>();

                return ApiResponse(true, "Bank updated successfully", new { bank = updatedBank, branches });
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

    public class CreateBankRequest
    {
        public BankDTO Bank { get; set; }
        public List<BankBranchDTO> Branches { get; set; }
    }
}
