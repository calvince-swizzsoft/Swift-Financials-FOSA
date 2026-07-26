using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Controllers
{



    [RoutePrefix("api/stoexecution")]
    public class StandingOrderExecutionController : ApiController
    {
        private readonly IStandingOrderAppService _standingOrderAppService;

        private readonly IRecurringBatchAppService _recurringBatchAppService;

        private readonly ILoanProductAppService _loanProductAppService;

        private readonly ISavingsProductAppService _savingsProductAppService;

        private readonly IInvestmentProductAppService _investmentProductAppService;

        public StandingOrderExecutionController(
            IStandingOrderAppService standingOrderAppService,
            IRecurringBatchAppService recurringBatchAppService,
            ILoanProductAppService loanProductAppService,
            ISavingsProductAppService savingsProductAppService,
            InvestmentProductAppService investmentProductAppService

            )
        {
            _standingOrderAppService = standingOrderAppService;
            _recurringBatchAppService = recurringBatchAppService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
        }



        [HttpPost]
        [Route("")]
        //public async Task<IHttpActionResult> Create(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> selectedRows, List<LoanProductDTO> selectedRows1, List<InvestmentProductDTO> selectedRows2, List<EmployeeDTO> selectedRows3, ObservableCollection<LoanProductDTO> loans)
        public async Task<IHttpActionResult> Create([FromBody] StandingOrderBatchRequest standingOrderBatchRequest)
        {
            var recurringBatchDTO = standingOrderBatchRequest.recurringBatch;
            var selectedRows = standingOrderBatchRequest.loanProducts;
            var selectedRows1 = standingOrderBatchRequest.loanProducts1;
            var selectedRows2 = standingOrderBatchRequest.investmentProducts;
            var selectedRows3 = standingOrderBatchRequest.employers;
            var savings = standingOrderBatchRequest.savings;


            recurringBatchDTO.Type = (int)RecurringBatchType.StandingOrder;

            recurringBatchDTO.ValidateAll();

            int Priority = recurringBatchDTO.Priority;

            bool success = false;

            var savingsCollection = new List<SavingsProductDTO>(savings);


            if (!recurringBatchDTO.HasErrors)
            {
                //if (selectedRows.Any())
                if (savingsCollection.Any())
                {
                          var serviceHeader = new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                EnvironmentIPAddress = "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
            };

                    foreach (var savingCollection in savingsCollection)
                    {
                        // var savingsProductDTO = await _channelService.ChargeLoanDynamicFeesAsync(recurringBatchDTO, selectedRows1, GetServiceHeader())
                        //await _channelService.ChargeLoanDynamicFeesAsync(recurringBatchDTO, loans, GetServiceHeader());

                       
                        success = _recurringBatchAppService.ChargeDynamicFees(recurringBatchDTO, savingsCollection, serviceHeader);

                      

                    }
                }
                return Ok(success);
            }

            else
            {
                var errorMessages = recurringBatchDTO.ErrorMessages;
                //  return View(recurringBatchDTO);
                return Ok(errorMessages);
            }
        }


        [HttpGet]
        [Route("loansproducts")]
        public async Task<IHttpActionResult> GetLoanProductsAsync()
        {
            var serviceHeader = new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                EnvironmentIPAddress = "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
            };
            var loanProductDTOs = _loanProductAppService.FindLoanProducts(serviceHeader);

            return Ok(loanProductDTOs);
        }


        [HttpGet]
        [Route("savingsproducts")]
        public async Task<IHttpActionResult> GetSavingsProductsAsync()
        {
            try
            {
                var serviceHeader = new ServiceHeader
                {
                    ApplicationDomainName = "SwiftApis",
                    ApplicationUserName = "Admin",
                    EnvironmentDomainName = "SwiftApis",
                    //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    EnvironmentIPAddress = "",
                    EnvironmentMACAddress = "",
                    EnvironmentMachineName = Environment.MachineName,
                    EnvironmentMotherboardSerialNumber = "",
                    EnvironmentOSVersion = Environment.OSVersion.ToString(),
                    EnvironmentProcessorId = "",
                    EnvironmentUserName = Environment.UserName
                };

                var savingProductDTOs = _savingsProductAppService.FindSavingsProducts(serviceHeader);

  
                return Ok(savingProductDTOs);

            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("investmentproducts")]
        public async Task<IHttpActionResult> GetInvestmentProductsAsync()
        {

            try
            {

                var serviceHeader = new ServiceHeader
                {
                    ApplicationDomainName = "SwiftApis",
                    ApplicationUserName = "Admin",
                    EnvironmentDomainName = "SwiftApis",
                    //EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    EnvironmentIPAddress = "",
                    EnvironmentMACAddress = "",
                    EnvironmentMachineName = Environment.MachineName,
                    EnvironmentMotherboardSerialNumber = "",
                    EnvironmentOSVersion = Environment.OSVersion.ToString(),
                    EnvironmentProcessorId = "",
                    EnvironmentUserName = Environment.UserName
                };

                var investmentProductDTOs = _investmentProductAppService.FindInvestmentProducts(serviceHeader);

 

                return Ok(investmentProductDTOs);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        //[HttpGet]
        //[Route("recurringbatches")]
        //public async Task<IHttpActionResult> GetRecurringBatches()
        //{
        //    try
        //    {

        //        var recurringBatchDTOs = await _channelService.FindRecurringBatchesAsync(GetServiceHeader());

        //        return Ok(recurringBatchDTOs);
        //    }

        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }


        //}

        //[HttpGet]
        //[Route("recurringbatchentries")]
        //public async Task<IHttpActionResult> GetRecurringBatchEntries()
        //{
        //    try
        //    {

        //        var recurringBatchEntryDTOs = await _channelService.FindRecurringBatchEntriesAsync();

        //        return Ok(recurringBatchEntryDTOs);
        //    }

        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }


        //}
        public class StandingOrderBatchRequest
        {

            public RecurringBatchDTO recurringBatch { get; set; }

            public List<LoanProductDTO> loanProducts { get; set; } = new List<LoanProductDTO>();

            public List<LoanProductDTO> loanProducts1 { get; set; } = new List<LoanProductDTO>();

            public List<InvestmentProductDTO> investmentProducts { get; set; } = new List<InvestmentProductDTO>();

            public List<EmployeeDTO> employers { get; set; } = new List<EmployeeDTO>();

            public List<SavingsProductDTO> savings { get; set; } = new List<SavingsProductDTO>();
        }




    }
}