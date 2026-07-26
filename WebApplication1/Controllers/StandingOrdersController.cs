using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using Application.MainBoundedContext.DTO.AccountsModule;

namespace WebApplication1.Controllers
{

    [RoutePrefix("api/standingorders")]
    public class StandingOrdersController: ApiController
    {


        private readonly IStandingOrderAppService _standingOrderAppService;


        public StandingOrdersController()
        {
       
        }


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

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody]StandingOrderDTO standingOrderDTO)
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
                
                var result = _standingOrderAppService.AddNewStandingOrder(standingOrderDTO, serviceHeader);

                return Ok(result);

            }

            catch(Exception ex)
            {
                return InternalServerError(ex);

            }
        }

}
}
