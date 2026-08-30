
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
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

        [HttpGet]
        [Route("configuration-options")]
        public IHttpActionResult GetConfigurationOptions()
        {
            var chargeTypes = Enum.GetValues(typeof(SavingsProductKnownChargeType)).Cast<SavingsProductKnownChargeType>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) });
            var chargeBenefactors = Enum.GetValues(typeof(ChargeBenefactor)).Cast<ChargeBenefactor>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) });
            return Ok(new { success = true, data = new { ChargeTypes = chargeTypes, ChargeBenefactors = chargeBenefactors } });
        }

        [HttpGet]
        [Route("{id:guid}/commissions")]
        public IHttpActionResult GetCommissions(Guid id, int knownChargeType)
        {
            if (!Enum.IsDefined(typeof(SavingsProductKnownChargeType), knownChargeType))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a supported savings-product charge type." });

            var commissions = _savingsProductAppService.FindCommissions(id, knownChargeType, Utils.CreateServiceHeader()) ?? new List<CommissionDTO>();
            var first = commissions.FirstOrDefault();
            return Ok(new
            {
                success = true,
                data = new
                {
                    CommissionIds = commissions.Select(item => item.Id),
                    ChargeBenefactor = first == null ? (int)ChargeBenefactor.Customer : first.ChargeBenefactor
                }
            });
        }

        [HttpPut]
        [Route("{id:guid}/commissions")]
        public IHttpActionResult UpdateCommissions(Guid id, UpdateSavingsProductCommissionsRequest request)
        {
            if (request == null || !Enum.IsDefined(typeof(SavingsProductKnownChargeType), request.KnownChargeType))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a supported savings-product charge type." });
            if (!Enum.IsDefined(typeof(ChargeBenefactor), request.ChargeBenefactor))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select who bears the charges." });

            var ids = (request.CommissionIds ?? new List<Guid>()).Where(value => value != Guid.Empty).Distinct().ToList();
            if (ids.Count != (request.CommissionIds ?? new List<Guid>()).Count)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Charge selections must contain unique, valid identifiers." });

            var serviceHeader = Utils.CreateServiceHeader();
            var available = _commissionAppService.FindCommissions(serviceHeader) ?? new List<CommissionDTO>();
            var commissions = available.Where(item => ids.Contains(item.Id) && !item.IsLocked).ToList();
            if (commissions.Count != ids.Count)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "One or more selected charges do not exist or are locked." });

            if (!_savingsProductAppService.UpdateCommissions(id, commissions, request.KnownChargeType, request.ChargeBenefactor, serviceHeader))
                return Content(HttpStatusCode.NotFound, new { success = false, message = "Savings product was not found or its charge mapping could not be updated." });

            return Ok(new { success = true, message = "Savings product charge mapping updated successfully." });
        }

        [HttpGet]
        [Route("{id:guid}/exemptions")]
        public IHttpActionResult GetExemptions(Guid id)
        {
            var exemptions = _savingsProductAppService.FindSavingsProductExemptions(id, Utils.CreateServiceHeader()) ?? new List<SavingsProductExemptionDTO>();
            return Ok(new { success = true, data = exemptions });
        }

        [HttpPut]
        [Route("{id:guid}/exemptions")]
        public IHttpActionResult UpdateExemptions(Guid id, List<SavingsProductExemptionDTO> exemptions)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var validationErrors = _savingsProductAppService.ValidateSavingsProductExemptions(id, exemptions, serviceHeader);
            if (validationErrors.Any())
                return Content(HttpStatusCode.BadRequest, new
                {
                    success = false,
                    message = string.Join(" ", validationErrors.SelectMany(item => item.Value)),
                    validationErrors
                });

            if (!_savingsProductAppService.UpdateSavingsProductExemptions(id, exemptions, serviceHeader))
                return Content(HttpStatusCode.NotFound, new { success = false, message = "Savings product was not found or its exemptions could not be updated." });

            return Ok(new { success = true, message = "Savings product exemptions updated successfully." });
        }
    }

    public class UpdateSavingsProductCommissionsRequest
    {
        public int KnownChargeType { get; set; }
        public int ChargeBenefactor { get; set; }
        public List<Guid> CommissionIds { get; set; }
    }
}
