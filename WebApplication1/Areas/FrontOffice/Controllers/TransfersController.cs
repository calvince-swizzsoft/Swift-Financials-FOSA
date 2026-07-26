using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/frontoffice/transfers")]
    public class TransfersController : ApiController
    {

        private readonly ICashTransferRequestAppService _cashTransferRequestAppService;
        private readonly IExternalChequeAppService _externalChequeAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ITellerAppService _tellerAppService;

        public TransfersController(
            ICashTransferRequestAppService cashTransferRequestAppService,
            IExternalChequeAppService externalChequeAppService,
            IChartOfAccountAppService chartOfAccountAppService,
            ITellerAppService tellerAppService
            )
        {
            _cashTransferRequestAppService = cashTransferRequestAppService;
            _externalChequeAppService = externalChequeAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _tellerAppService = tellerAppService;
        }


        TellerDTO _selectedTeller;
        public TellerDTO SelectedTeller
        {
            get { return _selectedTeller; }
            set
            {
                if (_selectedTeller != value)
                {
                    _selectedTeller = value;

                }
            }
        }



        // GET: FrontOffice/Transfers


        [HttpGet]
        [Route("cheques")]
        public async Task<IHttpActionResult> Cheques(Guid? TellerId)
        {
            var model = new CashTransferRequestDTO();

            //            var currentUser = await _applicationUserManager.FindByIdAsync(User.Identity.GetUserId());

            // _selectedTeller = await GetCurrentTeller();


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

            
            var untransferredChequesList = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId((Guid)TellerId, "", serviceHeader);

            var untransferredChequesValue = untransferredChequesList.Sum(cheque => cheque.Amount);

            model.EmployeeId = _selectedTeller.EmployeeId;
            model.TotalCredits = _selectedTeller.TotalCredits;
            model.TotalDebits = _selectedTeller.TotalDebits;
            model.BookBalance = _selectedTeller.BookBalance;

            model.OpeningBalance = _selectedTeller.OpeningBalance;
            model.ClosingBalance = _selectedTeller.ClosingBalance;

            model.UntransferredChequesValue = untransferredChequesValue;
            // return View(model);
            return Ok(model);
        }


        [HttpGet]
        [Route("cash")]
        public async Task<IHttpActionResult> cash()
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

                //var cashtransferrequsts = await _channelService.FindCashTransferRequestsAsync(GetServiceHeader());

                var cashTransferRequests = _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);
                return Ok(cashTransferRequests);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }




        [HttpPost]
        [Route("cash")]
        public async Task<IHttpActionResult> Create(CashTransferRequestDTO cashTransferRequestDTO)
        {

            var selectedTeller = await GetCurrentTeller();

            //get teller cash balance status



            var missingParameters = new List<string>();

            if (selectedTeller == null)
            {
                missingParameters.Add("Teller");
            }

            // Check if any parameter is missing
            if (missingParameters.Any())
            {
                var missingMessage = $"Some features may not work, you are missing {string.Join(", ", missingParameters)}";

                return Json(new { success = false, message = "Operation error: " + missingMessage });
            }

            cashTransferRequestDTO.EmployeeId = selectedTeller.EmployeeId;

            if (!cashTransferRequestDTO.HasErrors)
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

                
                var successRequest = _cashTransferRequestAppService.AddNewCashTransferRequestAsync(cashTransferRequestDTO, serviceHeader);

                if (successRequest != null)
                {

                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Operation Failed" });
                }
            }
            else
            {
                var errorMessages = cashTransferRequestDTO.ErrorMessages;

                return Json(new { success = false, message = errorMessages });
            }
        }


        [HttpPost]
        [Route("cheques")]
        public async Task<IHttpActionResult> TransferSelectedChequesAsync(List<ExternalChequeDTO> cheques)
        {
            if (cheques == null || cheques.Count == 0)
            {
                return Json(new { success = false, message = "No cheques were selected for transfer." });
            }

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

                List<ExternalChequeDTO> selectedCheques = new List<ExternalChequeDTO>(cheques);

                //var currentUser = await _applicationUserManager.FindByIdAsync(User.Identity.GetUserId());
                _selectedTeller = await GetCurrentTeller();


                var chequesInHandChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesInHand, serviceHeader);

                if (SelectedTeller != null && SelectedTeller.ChartOfAccountId.HasValue && chequesInHandChartOfAccountId != Guid.Empty)
                {

                    //var transferred = await _channelService.TransferExternalChequesAsync(selectedCheques, SelectedTeller, 0, GetServiceHeader());

                    var transferred = _externalChequeAppService.TransferExternalCheques(selectedCheques, SelectedTeller, 0, serviceHeader);

                    if (!transferred)
                    {
                        return Json(new { success = false, message = "Transfer failed. Please try again." });
                    }


                    var untransferredCheques = _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(SelectedTeller.Id, "", serviceHeader);
                    
                    var untransferredChequesValue = untransferredCheques.Sum(cheque => cheque.Amount);

                    //model.EmployeeId = SelectedTeller.EmployeeId;
                    //model.TotalCredits = SelectedTeller.TotalCredits;
                    //model.TotalDebits = SelectedTeller.TotalDebits;
                    //model.BookBalance = SelectedTeller.BookBalance;

                    //model.OpeningBalance = SelectedTeller.OpeningBalance;
                    //model.ClosingBalance = SelectedTeller.ClosingBalance;

                    //model.UntransferredChequesValue = untransferredChequesValue;
                    // Construct a JSON response directly
                    var response = new
                    {
                        success = true,
                        message = "Cheques transferred successfully.",
                        data = new
                        {
                            //EmployeeId = SelectedTeller.EmployeeId,
                            TotalCredits = SelectedTeller.TotalCredits,
                            TotalDebits = SelectedTeller.TotalDebits,
                            BookBalance = SelectedTeller.BookBalance,
                            OpeningBalance = SelectedTeller.OpeningBalance,
                            ClosingBalance = SelectedTeller.ClosingBalance,
                            UntransferredChequesValue = untransferredChequesValue
                        }
                    };

                    return Json(response);

                }

                else
                {

                    var message = "Sorry, but the requisite teller and / or external cheques in hand account has not been setup!";

                    return Json(new { success = false, message = "Operation error: " + message });
                }

            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new { success = false, message = "An error occurred while transferring cheques: " + ex.Message });
            }
        }




        private async Task<TellerDTO> GetCurrentTeller()
        {
           // bool includeBalance = true;


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



            // Get the current user
            //var user = await _applicationUserManager.FindByIdAsync(User.Identity.GetUserId());
            // var teller = await _channelService.FindTellerByEmployeeIdAsync(Guid.Parse("50BDE4A6-1F50-F111-9B87-C8E2651EF92A"), includeBalance, GetServiceHeader());

            var teller = _tellerAppService.FindTellerByEmployeeId(Guid.Parse("50BDE4A6-1F50-F111-9B87-C8E2651EF92A"), serviceHeader);
            
                       
            return teller;
        }


        [HttpPost]
        [Route("cash/acknowledge")]
        public async Task<IHttpActionResult> AcknowledgeCashTransferRequest(CashTransferRequestDTO cashTransferRequestDTO, int option)
        {

            var selectedTeller = await GetCurrentTeller();

            var missingParameters = new List<string>();

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


            if (selectedTeller == null)
            {
                missingParameters.Add("Teller");
            }

            // Check if any parameter is missing
            if (missingParameters.Any())
            {
                var missingMessage = $"Some features may not work, you are missing {string.Join(", ", missingParameters)}";

                return Json(new { success = false, message = "Operation error: " + missingMessage });
            }

            cashTransferRequestDTO.EmployeeId = selectedTeller.EmployeeId;

            if (!cashTransferRequestDTO.HasErrors)
            {
                //var successRequest = await _channelService.AddCashTransferRequestAsync(cashTransferRequestDTO, GetServiceHeader());

            
                var successRequest = await _cashTransferRequestAppService.AcknowledgeCashTransferRequestAsync(cashTransferRequestDTO, option, serviceHeader);

                if (successRequest)
                {
                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to Acknowledge cash transfer request" });
                }
            }
            else
            {
                var errorMessages = cashTransferRequestDTO.ErrorMessages;

                return Json(new { success = false, message = errorMessages });
            }
        }


        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetCashTransferRequests()
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

                var cashTransferRequests = await _cashTransferRequestAppService.FindCashTransferRequestsAsync(serviceHeader);
              
                return Ok(cashTransferRequests);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("cash/utilize")]
        public async Task<IHttpActionResult> UtilizeCashTransferRequest(Guid request)
        {

            if (request == Guid.Empty)
            {
                return BadRequest("Request Id is required for this operation");
            }


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

                var cashTransferRequest = await _cashTransferRequestAppService.FindCashTransferRequestAsync(request, serviceHeader);

                var successRequest = await _cashTransferRequestAppService.UtilizeCashTransferRequestAsync(request, serviceHeader);

                if (successRequest)
                {
                    return Json(new { success = true, message = "Operation Success" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to mark cash transfer request as Utilized" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex });
            }
        }
    }
}