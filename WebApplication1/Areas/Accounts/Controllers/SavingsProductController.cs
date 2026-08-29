
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using iTextSharp.text;
using iTextSharp.xmp.impl;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;



namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[AllowAnonymous]
    [Authorize]
    [RoutePrefix("api/accounts/savingsproducts")]
    public class SavingsProductController : ApiController
    {

        private readonly ISavingsProductAppService _savingsProductAppService;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly ICommissionAppService _commissionAppService;

        public SavingsProductController(
            ISavingsProductAppService savingsProductAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            ICommissionAppService commissionAppService
            )
        {
            _savingsProductAppService = savingsProductAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _commissionAppService = commissionAppService; 
        }

 
        public async Task<IHttpActionResult> ChartOfAccountLookUp(Guid? id, SavingsProductDTO savingsProductDTO)
        {
            // await ServeNavigationMenus();

            Guid parseId;

            if (id == Guid.Empty || !Guid.TryParse(id.ToString(), out parseId))
            {
                return Json(new { success = false });
            }

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            var chartOfAccount = _chartOfAccountAppService.FindChartOfAccount(parseId, serviceHeader);
          

            if (chartOfAccount != null)
            {
                savingsProductDTO.ChartOfAccountId = chartOfAccount.Id;
                savingsProductDTO.ChartOfAccountAccountName = chartOfAccount.AccountName;


                return Json(new
                {
                    success = true,
                    data = new
                    {
                        ChartOfAccountId = savingsProductDTO.ChartOfAccountId,
                        ChartOfAccountAccountName = savingsProductDTO.ChartOfAccountAccountName
                    }
                });
            }
            return Json(new { success = false, message = "Product Not Found!" });
        }


        public async Task<IHttpActionResult> Details(Guid id)
        {
            // await ServeNavigationMenus();

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();


            var savingsProductDTO = _savingsProductAppService.FindSavingsProduct(id, Guid.NewGuid(), serviceHeader);

            var comissionDTOs = _commissionAppService.FindCommissions(serviceHeader);
            //  var commissionDTOs = await _channelService.FindCommissionsBySavingsProductIdAsync(savingsProductDTO.Id, savingsProductDTO.ChargeType, GetServiceHeader());

            var excemptions = _savingsProductAppService.FindSavingsProductExemptions(savingsProductDTO.Id, serviceHeader);
            
            //var excemptions = await _channelService.FindSavingsProductExemptionsBySavingsProductIdAsync(savingsProductDTO.Id, GetServiceHeader());

            var rPriority = savingsProductDTO.Priority;

            string savings = "Savings", loans = "Loans", investments = "Investments", directDebits = "Direct Debits";

            var mapping = new Dictionary<string, int>
            {
                { "Loans", 0 },
                { "Investments", 1 },
                { "Savings", 2 },
                { "Direct Debits", 3 }
            };

            if (rPriority == 0)
            {
                savingsProductDTO.PriorityDescription = "Loans";
            }

            if (rPriority == 1)
            {
                savingsProductDTO.PriorityDescription = "Investments";
            }

            if (rPriority == 2)
            {
                savingsProductDTO.PriorityDescription = "Savings";
            }

            if (rPriority == 3)
            {
                savingsProductDTO.PriorityDescription = "Direct Debits";
            }


            return Json(
                new
                {
                    success = true,
                    savingsProductDTO
                });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(SavingsProductDTO savingsProductDTO)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var validationErrors = _savingsProductAppService.ValidateSavingsProduct(savingsProductDTO, serviceHeader);
            if (validationErrors.Any())
                return Content(HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = string.Join(" ", validationErrors.SelectMany(item => item.Value)),
                    validationErrors
                });

            var result = _savingsProductAppService.AddNewSavingsProduct(savingsProductDTO, serviceHeader);
            if (result == null)
                return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to create Savings Product." });

            return Content(HttpStatusCode.Created, new { success = true, message = "Savings Product created successfully", data = result });
        }


        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> Edit(SavingsProductDTO savingsProductBindingModel)
        {
            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var validationErrors = _savingsProductAppService.ValidateSavingsProduct(savingsProductBindingModel, serviceHeader);
            if (savingsProductBindingModel == null || savingsProductBindingModel.Id == Guid.Empty)
                validationErrors["Id"] = new[] { "Savings product Id is required." };
            if (validationErrors.Any())
                return Content(HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = string.Join(" ", validationErrors.SelectMany(item => item.Value)),
                    validationErrors
                });

            if (!_savingsProductAppService.UpdateSavingsProduct(savingsProductBindingModel, serviceHeader))
                return Content(HttpStatusCode.NotFound, new { success = false, message = "Savings Product was not found or could not be updated." });

            return Ok(new { success = true, message = "Edited Savings Product successfully" });
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetSavingsProductsAsync()
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
            var savingsProductDTOs = _savingsProductAppService.FindSavingsProducts(serviceHeader);

            return Ok(savingsProductDTOs);
        }
    }
}
