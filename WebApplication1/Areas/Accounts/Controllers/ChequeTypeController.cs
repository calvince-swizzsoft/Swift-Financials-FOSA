using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/chequetypes")]
    public class ChequeTypeController : ApiController
    {
        private readonly IChequeTypeAppService _chequeTypeAppService;

        public ChequeTypeController(IChequeTypeAppService chequeTypeAppService)
        {
            _chequeTypeAppService = chequeTypeAppService ?? throw new ArgumentNullException(nameof(chequeTypeAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _chequeTypeAppService.FindChequeTypes(pageIndex, pageSize, serviceHeader)
                    : _chequeTypeAppService.FindChequeTypes(text, pageIndex, pageSize, serviceHeader);

                if (page == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var chequeTypes = _chequeTypeAppService.FindChequeTypes(serviceHeader);

                return Ok(new { success = true, message = "", data = chequeTypes ?? new List<ChequeTypeDTO>() });
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

                var chequeType = _chequeTypeAppService.FindChequeType(id, serviceHeader);

                if (chequeType == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = chequeType });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // The reference MVC controller (Areas/Accounts/Controllers/ChequeTypeController.cs)
        // collects the ChequeTypeDTO, selected commissions, and selected loan/investment
        // products across three separate session-backed endpoints (StoreSelectedApplicableCharges,
        // StoreSelectedLoanProducts, StoreSelectedInvestmentProducts) before a stateless Create
        // POST reads them back out of Session. There's no session here, so all three travel
        // together in one request body — same fields, same "charges and at least one product
        // category are both required" rule the reference Create enforced.
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CreateChequeTypeRequest request)
        {
            try
            {
                if (request?.ChequeType == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid cheque type data", data = (object)null });
                }

                request.ChequeType.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (request.ChequeType.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.ChequeType.ErrorMessages), data = (object)null });
                }

                if (request.Commissions == null || !request.Commissions.Any())
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "No charges selected", data = (object)null });
                }

                var hasLoanProducts = request.AttachedProducts?.LoanProductCollection != null && request.AttachedProducts.LoanProductCollection.Any();
                var hasInvestmentProducts = request.AttachedProducts?.InvestmentProductCollection != null && request.AttachedProducts.InvestmentProductCollection.Any();

                if (!hasLoanProducts && !hasInvestmentProducts)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "No products selected", data = (object)null });
                }

                var createdChequeType = _chequeTypeAppService.AddNewChequeType(request.ChequeType, serviceHeader);

                if (createdChequeType == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Error adding cheque type", data = (object)null });
                }

                var commissionsUpdated = _chequeTypeAppService.UpdateCommissions(createdChequeType.Id, request.Commissions, serviceHeader);

                var productsUpdated = _chequeTypeAppService.UpdateAttachedProducts(createdChequeType.Id, request.AttachedProducts, serviceHeader);

                if (!commissionsUpdated || !productsUpdated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Error updating commissions or attached products", data = createdChequeType });
                }

                return Ok(new { success = true, message = "Cheque type, commissions, and attached products successfully created/updated", data = createdChequeType });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, ChequeTypeDTO chequeTypeDTO)
        {
            try
            {
                if (chequeTypeDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid cheque type data", data = (object)null });
                }

                chequeTypeDTO.Id = id;
                chequeTypeDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (chequeTypeDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", chequeTypeDTO.ErrorMessages), data = (object)null });
                }

                var existing = _chequeTypeAppService.FindChequeType(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = _chequeTypeAppService.UpdateChequeType(chequeTypeDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update cheque type", data = (object)null });
                }

                var refreshed = _chequeTypeAppService.FindChequeType(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> GetCommissions(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var commissions = _chequeTypeAppService.FindCommissions(id, serviceHeader);

                return Ok(new { success = true, message = "", data = commissions ?? new List<CommissionDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/commissions")]
        public async Task<IHttpActionResult> UpdateCommissions(Guid id, List<CommissionDTO> commissions)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _chequeTypeAppService.UpdateCommissions(id, commissions ?? new List<CommissionDTO>(), serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update commissions", data = (object)null });
                }

                var refreshed = _chequeTypeAppService.FindCommissions(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}/attached-products")]
        public async Task<IHttpActionResult> GetAttachedProducts(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var attachedProducts = _chequeTypeAppService.FindAttachedProducts(id, serviceHeader, false);

                return Ok(new { success = true, message = "", data = attachedProducts ?? new ProductCollectionInfo() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:guid}/attached-products")]
        public async Task<IHttpActionResult> UpdateAttachedProducts(Guid id, ProductCollectionInfo attachedProducts)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var updated = _chequeTypeAppService.UpdateAttachedProducts(id, attachedProducts ?? new ProductCollectionInfo(), serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update attached products", data = (object)null });
                }

                var refreshed = _chequeTypeAppService.FindAttachedProducts(id, serviceHeader, false);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class CreateChequeTypeRequest
    {
        public ChequeTypeDTO ChequeType { get; set; }
        public List<CommissionDTO> Commissions { get; set; }
        public ProductCollectionInfo AttachedProducts { get; set; }
    }
}
