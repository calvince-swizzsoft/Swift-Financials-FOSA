using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Management;
using WebApplication1.Helpers;

namespace SwiftFinancials.Web.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/investmentsproducts")]
    public class InvestmentsProductController : ApiController
    {
        private readonly IInvestmentProductAppService _investmentProductAppService;

        public InvestmentsProductController(IInvestmentProductAppService investmentProductAppService)
        {
            _investmentProductAppService = investmentProductAppService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(InvestmentProductDTO investmentProductDTO)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var validationErrors = _investmentProductAppService.ValidateInvestmentProduct(investmentProductDTO, serviceHeader);
            if (validationErrors.Any()) return Content(HttpStatusCode.BadRequest, new
            {
                success = false,
                message = string.Join(" ", validationErrors.SelectMany(x => x.Value)),
                validationErrors
            });
            var created = _investmentProductAppService.AddNewInvestmentProduct(investmentProductDTO, serviceHeader);
            if (created == null) return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to create Investment Product." });
            return Content(HttpStatusCode.Created, new { success = true, message = "Investment Product created successfully", data = created });
        }

       
        [HttpPut]

        [Route("")]
        public async Task<IHttpActionResult> Edit(InvestmentProductDTO investmentProductBindingModel)
        {

            var serviceHeader = Utils.CreateServiceHeader();
            var validationErrors = _investmentProductAppService.ValidateInvestmentProduct(investmentProductBindingModel, serviceHeader);
            if (investmentProductBindingModel == null || investmentProductBindingModel.Id == Guid.Empty)
                validationErrors["Id"] = new[] { "Investment product Id is required." };
            if (validationErrors.Any()) return Content(HttpStatusCode.BadRequest, new
            {
                success = false,
                message = string.Join(" ", validationErrors.SelectMany(x => x.Value)),
                validationErrors
            });
            if (!_investmentProductAppService.UpdateInvestmentProduct(investmentProductBindingModel, serviceHeader))
                return Content(HttpStatusCode.NotFound, new { success = false, message = "Investment Product was not found or could not be updated." });
            return Ok(new { success = true, message = "Investment Product updated successfully" });
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetInvestmentProductsAsync()
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var investmentProductDTOs = _investmentProductAppService.FindInvestmentProducts(serviceHeader);
            return Ok(investmentProductDTOs);
        }
    }
}
