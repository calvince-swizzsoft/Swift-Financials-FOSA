
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using iTextSharp.text;
using iTextSharp.xmp.impl;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;



namespace WebaPPLICATION1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/savingsproducts")]
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

            savingsProductDTO.ValidateAll();


            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            if (!savingsProductDTO.HasErrors)
            {
                var results = _savingsProductAppService.AddNewSavingsProduct(savingsProductDTO, serviceHeader);

                return Json(new
                {
                    Success = true,
                    Message = "Savings Product created successfully"
                });
            }
            else
            {
                var errorMessages = savingsProductDTO.ErrorMessages;
                return Json(new
                {
                    Success = false,
                    Message = "Failed to create Savings Product"
                });
            }
        }

        public async Task<IHttpActionResult> Edit(Guid id)
        {
            // await ServeNavigationMenus();

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

            var savingsProductDTO = _savingsProductAppService.FindSavingsProduct(id, Guid.NewGuid(), serviceHeader);

            return Ok(savingsProductDTO);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Edit(Guid id, SavingsProductDTO savingsProductBindingModel)
        {
            if (ModelState.IsValid)
            {

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
                _savingsProductAppService.UpdateSavingsProduct(savingsProductBindingModel, serviceHeader);


                return Json(new
                {

                    success = true,
                    message = "Edited Savings Product successfully"
                });
            }
            else
            {

                return Json(new
                {

                    success = false,
                    message = "Failed to Edit Savings Product"
                });
            }
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
