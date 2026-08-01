using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Management;
using WebApplication1.Helpers;

namespace SwiftFinancials.Web.Areas.Accounts.Controllers
{
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
            investmentProductDTO.ValidateAll();

            if (!investmentProductDTO.HasErrors)
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var createdInvestmentProduct = _investmentProductAppService.AddNewInvestmentProduct(investmentProductDTO, serviceHeader);


                return Ok(createdInvestmentProduct);
            }
            else
            {
                var errorMessages = investmentProductDTO.ErrorMessages;

                return Json(new
                {
                    success = false,
                    message = errorMessages.ToString()
                });
            }
        }

       
        [HttpPut]

        [Route("")]
        public async Task<IHttpActionResult> Edit(InvestmentProductDTO investmentProductBindingModel)
        {

            investmentProductBindingModel.ValidateAll();

            if (ModelState.IsValid)
            {

                var serviceHeader = Utils.CreateServiceHeader();

                _investmentProductAppService.UpdateInvestmentProduct(investmentProductBindingModel, serviceHeader);
              
                return Json(new
                {
                    success = true,
                    message = "Edited Invetsments Product successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to edit product"
                });
            }
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
