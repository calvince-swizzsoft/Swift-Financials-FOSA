using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/frontoffice/tellers")]
    public class TellerController : ApiController
    {


        private readonly ITellerAppService _tellerAppService;

        public TellerController (ITellerAppService tellerAppService)
        {

            _tellerAppService = tellerAppService;
            
        }

        [HttpGet]

        [Route("")]
        public async Task<IHttpActionResult> Index()
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

                //var tellers = await _channelService.FindTellersByTypeAsync(tellerDTO.Type, tellerDTO.Reference, true, GetServiceHeader());

                var tellers = _tellerAppService.FindTellers(serviceHeader);


                if (tellers == null)
                {

                    return NotFound();
                }

                return Ok(tellers);

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPost]

        [Route("")]
        public async Task<IHttpActionResult> Create(TellerDTO tellerDTO)
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

                tellerDTO.ValidateAll();

                UpdateTellerAccounts(tellerDTO);

                if (!tellerDTO.HasErrors)
                {

                    var createdTellerDTO = _tellerAppService.AddNewTeller(tellerDTO, serviceHeader);

                    return Ok(createdTellerDTO);
                }

                else
                {
                    return BadRequest(tellerDTO.ErrorMessages.ToString());
                }

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateTeller(TellerDTO tellerDTO)
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


                var updatedTellerDTO = _tellerAppService.UpdateTeller(tellerDTO, serviceHeader);

                return Ok(updatedTellerDTO);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);

            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetTeller(Guid tellerId)
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

                var teller = _tellerAppService.FindTeller(tellerId, serviceHeader);
            
                return Ok(teller);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);

            }
        }


        [HttpDelete]

        //public async Task<IHttpActionResult> DeleteTeller(Guid id)
        //{
        //    Guid parseId;

        //    try
        //    {

        //        if (id == Guid.Empty || !Guid.TryParse(id.ToString(), out parseId))
        //        {

        //            return BadRequest("Invalid Id");

        //        }

        //    }

        //    catch (Exception ex)
        //    {

        //        return InternalServerError(ex);

        //    }
        //}


        private void UpdateTellerAccounts(TellerDTO tellerDTO)
        {
            switch ((TellerType)tellerDTO.Type)
            {
                case TellerType.InhousePointOfSale:
                case TellerType.AutomatedTellerMachine:
                    tellerDTO.ShortageChartOfAccountId = tellerDTO.ChartOfAccountId;
                    break;

                case TellerType.AgentPointOfSale:
                    tellerDTO.ChartOfAccountId = tellerDTO.CommissionCustomerAccountCustomerAccountTypeTargetProductId;
                    tellerDTO.ShortageChartOfAccountId = tellerDTO.CommissionCustomerAccountCustomerAccountTypeTargetProductId;
                    break;
            }
        }

        [HttpGet]
        [Route("teller")]
        public async Task<IHttpActionResult> GetTellerByEmployeeId(Guid employeeId)
        {

            bool includeBalance = default(bool);

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
                includeBalance = true;

                //var teller = await _channelService.FindTellerByEmployeeIdAsync(employeeId, includeBalance, GetServiceHeader());
                var teller = _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);
                return Ok(teller);

            }


            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




    }
}