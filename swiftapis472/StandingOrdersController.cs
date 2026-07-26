using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;


//using System.Web;
using System.Web.Http;

namespace swiftapis472
{


    [RoutePrefix("api/standingorders")]
    //  [ApiController]
    public class StandingOrdersController : ApiController
    {

        private readonly IStandingOrderAppService _standingOrderAppService;


        public StandingOrdersController(IStandingOrderAppService standingOrderAppService)
        {
            _standingOrderAppService = standingOrderAppService;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index()
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


                var standingOrders = _standingOrderAppService.FindStandingOrders(serviceHeader);
                if (standingOrders == null)
                {
                   
                    return NotFound();
                }
                return Ok(standingOrders);
             //   return Ok(standingOrders);
            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

    }
}