using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // CreditType is existing domain master data. This API deliberately delegates
    // persistence and relationship replacement to ICreditTypeAppService; controllers
    // only translate HTTP requests and resolve picker options through focused services.
    [Authorize]
    [RoutePrefix("api/accounts/credittypes")]
    public class CreditTypeController : ApiController
    {
        private readonly ICreditTypeAppService _creditTypeAppService;

        public CreditTypeController(ICreditTypeAppService creditTypeAppService)
        {
            _creditTypeAppService = creditTypeAppService ?? throw new ArgumentNullException(nameof(creditTypeAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var items = _creditTypeAppService.FindCreditTypes(serviceHeader);
            return Ok(new { success = true, message = "", data = items ?? new List<CreditTypeDTO>() });
        }

        [HttpGet]
        [Route("paged")]
        public IHttpActionResult GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? _creditTypeAppService.FindCreditTypes(pageIndex, pageSize, serviceHeader)
                : _creditTypeAppService.FindCreditTypes(text, pageIndex, pageSize, serviceHeader);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var item = _creditTypeAppService.FindCreditType(id, serviceHeader);
            return item == null ? (IHttpActionResult)NotFound() : Ok(new { success = true, message = "", data = item });
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(SaveCreditTypeRequest request)
        {
            if (request?.CreditType == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Credit type data is required", data = (object)null });

            request.CreditType.ValidateAll();
            if (request.CreditType.HasErrors)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.CreditType.ErrorMessages), data = (object)null });

            var serviceHeader = Utils.CreateServiceHeader();
            var created = _creditTypeAppService.AddNewCreditType(request.CreditType, serviceHeader);
            if (created == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Credit type could not be created", data = (object)null });

            if (!SaveRelationships(created.Id, request, serviceHeader))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Credit type was created but its relationships could not be saved", data = created });

            return Ok(new { success = true, message = "Credit type created successfully", data = _creditTypeAppService.FindCreditType(created.Id, serviceHeader) });
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, SaveCreditTypeRequest request)
        {
            if (request?.CreditType == null)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Credit type data is required", data = (object)null });

            request.CreditType.Id = id;
            request.CreditType.ValidateAll();
            if (request.CreditType.HasErrors)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.CreditType.ErrorMessages), data = (object)null });

            var serviceHeader = Utils.CreateServiceHeader();
            if (_creditTypeAppService.FindCreditType(id, serviceHeader) == null)
                return NotFound();
            if (!_creditTypeAppService.UpdateCreditType(request.CreditType, serviceHeader))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Credit type could not be updated", data = (object)null });
            if (!SaveRelationships(id, request, serviceHeader))
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Credit type was updated but its relationships could not be saved", data = (object)null });

            return Ok(new { success = true, message = "Credit type updated successfully", data = _creditTypeAppService.FindCreditType(id, serviceHeader) });
        }

        [HttpGet]
        [Route("{id:guid}/configuration")]
        public IHttpActionResult GetConfiguration(Guid id)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            if (_creditTypeAppService.FindCreditType(id, serviceHeader) == null)
                return NotFound();

            var data = new CreditTypeConfigurationDTO
            {
                Commissions = _creditTypeAppService.FindCommissions(id, serviceHeader) ?? new List<CommissionDTO>(),
                DirectDebits = _creditTypeAppService.FindDirectDebits(id, serviceHeader) ?? new List<DirectDebitDTO>(),
                AttachedProducts = _creditTypeAppService.FindAttachedProducts(id, serviceHeader, false) ?? new ProductCollectionInfo(),
                ConcessionExemptProducts = _creditTypeAppService.FindConcessionExemptProducts(id, serviceHeader, false) ?? new ProductCollectionInfo()
            };
            return Ok(new { success = true, message = "", data });
        }

        private bool SaveRelationships(Guid id, SaveCreditTypeRequest request, Infrastructure.Crosscutting.Framework.Utils.ServiceHeader serviceHeader)
        {
            return _creditTypeAppService.UpdateCommissions(id, request.Commissions ?? new List<CommissionDTO>(), serviceHeader)
                && _creditTypeAppService.UpdateDirectDebits(id, request.DirectDebits ?? new List<DirectDebitDTO>(), serviceHeader)
                && _creditTypeAppService.UpdateAttachedProducts(id, request.AttachedProducts ?? new ProductCollectionInfo(), serviceHeader)
                && _creditTypeAppService.UpdateConcessionExemptProducts(id, request.ConcessionExemptProducts ?? new ProductCollectionInfo(), serviceHeader);
        }
    }

    public class SaveCreditTypeRequest
    {
        public CreditTypeDTO CreditType { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
        public List<DirectDebitDTO> DirectDebits { get; set; }
        public ProductCollectionInfo AttachedProducts { get; set; }
        public ProductCollectionInfo ConcessionExemptProducts { get; set; }
    }

    public class CreditTypeConfigurationDTO
    {
        public List<CommissionDTO> Commissions { get; set; }
        public List<DirectDebitDTO> DirectDebits { get; set; }
        public ProductCollectionInfo AttachedProducts { get; set; }
        public ProductCollectionInfo ConcessionExemptProducts { get; set; }
    }
}
