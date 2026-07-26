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
    [RoutePrefix("api/frontoffice/treasurys")]
    public class TreasurysController : ApiController
    {

        private readonly ITreasuryAppService _treasuryAppService;

        public TreasurysController (ITreasuryAppService treasuryAppService)
        {
            _treasuryAppService = treasuryAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index()
        {
            //Guid branchId;

           // bool includeBalance = true;

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

                //if (branchId == Guid.Empty || !Guid.TryParse(branchId.ToString(), out parseId))
                //{
                //           return BadRequest("Invalid Id");
                //}

                // branchId = Guid.Parse("D6537BE3-0F2B-4569-9DB1-25B580143C76");

                //var treasuries = await _channelService.FindTreasuryByBranchIdAsync(branchId, includeBalance, GetServiceHeader());

                var treasuries = _treasuryAppService.FindTreasuries(serviceHeader);
                // var treasuries = await _channelService.FindTreasuriesByFilterInPageAsync
                if (treasuries == null)
                {
                    return NotFound();
                }
                return Ok(treasuries);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(TreasuryDTO treasuryDTO)
        {
            try
            {
                treasuryDTO.ValidateAll();


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

                if (!treasuryDTO.HasErrors)
                {

                    var createdTreasuryDTO = _treasuryAppService.AddNewTreasury(treasuryDTO, serviceHeader);

                   
                    return Ok(createdTreasuryDTO);
                }

                else
                {
                    return BadRequest(treasuryDTO.ErrorMessages.ToString());
                }

            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateTreasury(TreasuryDTO treasuryDTO)
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
              
                var updatedTreasuryDTO = _treasuryAppService.UpdateTreasury(treasuryDTO, serviceHeader);

                return Ok(updatedTreasuryDTO);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //employee
    }



}
