using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/administration/companies")]
    public class CompanyController : ApiController
    {
        private readonly ICompanyAppService _companyAppService;

        public CompanyController(ICompanyAppService companyAppService)
        {
            _companyAppService = companyAppService;
        }

        [HttpGet, Route("")]
        public IHttpActionResult Index([FromUri] int pageIndex = 0, [FromUri] int pageSize = 20, [FromUri] string text = "")
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var companies = string.IsNullOrWhiteSpace(text)
                    ? _companyAppService.FindCompanies(pageIndex, pageSize, serviceHeader)
                    : _companyAppService.FindCompanies(text, pageIndex, pageSize, serviceHeader);

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("all")]
        public IHttpActionResult GetAll()
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var companies = _companyAppService.FindCompanies(serviceHeader);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("count")]
        public async System.Threading.Tasks.Task<IHttpActionResult> GetCount()
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var count = await _companyAppService.GetCompaniesCountAsync(serviceHeader);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var company = _companyAppService.FindCompany(id, serviceHeader);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(CreateCompanyRequest request)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                if (request?.Company == null)
                    return BadRequest("Invalid company data");

                var company = _companyAppService.AddNewCompany(request.Company, serviceHeader);

                if (request.MandatoryDebitTypes != null)
                    _companyAppService.UpdateDebitTypes(company.Id, request.MandatoryDebitTypes, serviceHeader);

                if (request.MandatoryProducts != null)
                    _companyAppService.UpdateAttachedProducts(company.Id, request.MandatoryProducts, serviceHeader);

                return Ok(company);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, CompanyDTO companyDto)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                if (companyDto == null || companyDto.Id != id)
                    return BadRequest("Invalid company data");

                var updated = _companyAppService.UpdateCompany(companyDto, serviceHeader);
                if (!updated)
                    return InternalServerError(new InvalidOperationException("Failed to update company"));

                var company = _companyAppService.FindCompany(id, serviceHeader);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:guid}/debit-types")]
        public IHttpActionResult GetDebitTypes(Guid id)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var debitTypes = _companyAppService.FindDebitTypes(id, serviceHeader);
                return Ok(debitTypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id:guid}/debit-types")]
        public IHttpActionResult UpdateDebitTypes(Guid id, List<DebitTypeDTO> debitTypes)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var updated = _companyAppService.UpdateDebitTypes(id, debitTypes ?? new List<DebitTypeDTO>(), serviceHeader);
                if (!updated)
                    return InternalServerError(new InvalidOperationException("Failed to update debit types"));

                var refreshed = _companyAppService.FindDebitTypes(id, serviceHeader);
                return Ok(refreshed);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:guid}/attached-products")]
        public IHttpActionResult GetAttachedProducts(Guid id)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var attachedProducts = _companyAppService.FindAttachedProducts(id, serviceHeader);
                return Ok(attachedProducts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id:guid}/attached-products")]
        public IHttpActionResult UpdateAttachedProducts(Guid id, ProductCollectionInfo attachedProducts)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            try
            {
                var updated = _companyAppService.UpdateAttachedProducts(id, attachedProducts ?? new ProductCollectionInfo(), serviceHeader);
                if (!updated)
                    return InternalServerError(new InvalidOperationException("Failed to update attached products"));

                var refreshed = _companyAppService.FindAttachedProducts(id, serviceHeader);
                return Ok(refreshed);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class CreateCompanyRequest
    {
        public CompanyDTO Company { get; set; }
        public List<DebitTypeDTO> MandatoryDebitTypes { get; set; }
        public ProductCollectionInfo MandatoryProducts { get; set; }
    }
}
