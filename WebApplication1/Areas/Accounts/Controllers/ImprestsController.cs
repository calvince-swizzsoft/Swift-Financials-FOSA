using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;

using System;

using System.Collections.ObjectModel;

using System.Linq;

using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;
using static WebApplication1.Controllers.ValuesController;

namespace TestApis.Controllers
{

    [RoutePrefix("api/imprests")]
    public class ImprestsController : ApiController
    {

        private readonly IImprestAppService _imprestAppService;

        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        private readonly IBankLinkageAppService _bankLinkageAppService;

        

        public ImprestsController(
            IImprestAppService imprestAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            IBankLinkageAppService bankLinkageAppService

            )

        {
            _imprestAppService = imprestAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _bankLinkageAppService = bankLinkageAppService;
        
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetImprests(bool? posted = null)
        {
            var serviceHeader = Utils.CreateServiceHeader();
            var imprests = _imprestAppService.FindImprests(serviceHeader);

            // Apply filtering if 'posted' param is provided
            if (posted.HasValue)
            {
                imprests = imprests.Where(i => i.Posted == posted.Value).ToList();
            }

            return Json(new ApiResponse<object>
            {
                Success = true,
                Message = imprests?.Count > 0 ? $"{imprests.Count} imprests found." : "No imprests found.",
                Data = imprests
            });
        }



        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddImprest([FromBody] ImprestDTO imprestDTO)
        {

            var serviceHeader = Utils.CreateServiceHeader();

            if (imprestDTO != null)
            {
                //imprestDTO.AmountRequested = 0;

                var linesTotal = 0.00m;

                foreach (var gl in imprestDTO.ImprestLines)
                {

                    linesTotal = linesTotal + gl.Amount;

                    if (gl.ImprestDebtorsChartOfAccountId != Guid.Empty)
                    {
                        var debitGl = _chartOfAccountAppService.FindChartOfAccount(gl.ImprestDebtorsChartOfAccountId, serviceHeader);
                        gl.LineNo = debitGl.AccountCode;

                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "YOU HAVE A LINE WITHOUT PROPERT EXPENSECHARTOFAACCOUNTID"
                        });
                    }

                }

                if (imprestDTO.AmountRequested != linesTotal)
                {

                    return Json(new
                    {
                        success = false,
                        message = "Amounts in Lines dont add up to value of Total Amount"
                    });
                }



                var bank = _bankLinkageAppService.FindBankLinkage(imprestDTO.BankId, serviceHeader);


                imprestDTO.BranchId = bank.BranchId;
                imprestDTO.BankId = bank.Id;
                imprestDTO.BankBranchName = bank.BankBranchName;
                imprestDTO.BankChartOfAccountId = bank.ChartOfAccountId;


                imprestDTO.ValidateAll();

                if (imprestDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = imprestDTO.ErrorMessages
                    });
                }

                var result = _imprestAppService.AddNewImprest(imprestDTO, serviceHeader);
         

                if (result != null)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully added imprest with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to add imprest with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> UpdateImprest([FromBody] ImprestDTO imprestDTO)
        {

            var serviceHeader = Utils.CreateServiceHeader();

            if (imprestDTO != null)
            {

                imprestDTO.ValidateAll();


                if (imprestDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = imprestDTO.ErrorMessages
                    });
                }

                var result = _imprestAppService.UpdateImprest(imprestDTO, serviceHeader);
              

                if (result)
                {


                    return Json(new
                    {
                        success = true,
                        message = "Successfully updated Imprest Header with lines."
                    });
                }


                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to update Imprest header with lines."
                    });
                }

            }


            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Body is incomplete"
                });

            }

        }


        [HttpPost]
        [Route("post/{id}")]
        public async Task<IHttpActionResult> PostImprest(Guid id)
        {

            var serviceHeader = Utils.CreateServiceHeader();

            ImprestDTO imprestDTO = null;


            var imprestDTOs = _imprestAppService.FindImprests(serviceHeader);
       
            if (imprestDTOs != null)
            {

                imprestDTO = imprestDTOs.FirstOrDefault(p => p.Id == id);
            }


            if (imprestDTO != null)
            {

                if (imprestDTO.BranchId == Guid.Empty || imprestDTO.BankId == Guid.Empty)
                {
                    
                    var banks = _bankLinkageAppService.FindBankLinkages(serviceHeader);
                    var bank = banks[0];
                    imprestDTO.BranchId = bank.BranchId;
                    imprestDTO.BankId = bank.Id;
                    imprestDTO.BankBranchName = bank.BankBranchName;
                }


                imprestDTO.ValidateAll();
                if (imprestDTO.ErrorMessages.Count > 0)
                {


                    return Json(new
                    {
                        success = false,
                        message = imprestDTO.ErrorMessages
                    });
                }

                //var transactionModel = new TransactionModel();

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                var result = _imprestAppService.PostImprest(imprestDTO, moduleNavigationItemCode, serviceHeader);
              
                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }





        [HttpPost]
        [Route("surrender")]
        public async Task<IHttpActionResult> PostSurrender([FromBody] ImprestDTO imprestDTO)
        {

            var serviceHeader = Utils.CreateServiceHeader();


            if (imprestDTO != null)
            {
                imprestDTO.ValidateAll();
                if (imprestDTO.ErrorMessages.Count > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = imprestDTO.ErrorMessages
                    });
                }

                int moduleNavigationItemCode = 0;

                var tariffs = new ObservableCollection<TariffWrapper>();

                var result = _imprestAppService.PostSurrender(imprestDTO, moduleNavigationItemCode, serviceHeader);
             
                if (result != null)
                {

                    return Json(new
                    {
                        success = true,
                        message = "Succesfully posted Journal",
                        data = result
                    });
                }

                else
                {


                    return Json(new
                    {
                        success = false,
                        message = "Failed to post journal"
                    });
                }


            }

            else
            {

                return Json(new
                {
                    success = false,
                    message = "Request Object is empty"
                });
            }

        }


    }
}
