using Microsoft.AspNetCore.Mvc;
using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using Microsoft.AspNetCore.Http.HttpResults;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwiftApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingOrdersController : ControllerBase
    {

         private readonly IStandingOrderAppService _standingOrderAppService;


        public StandingOrdersController(IStandingOrderAppService standingOrderAppService)
        {
            _standingOrderAppService = standingOrderAppService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            try
            {
                 var serviceHeader = new ServiceHeader
                {
                    ApplicationDomainName = "SwiftApis",
                    ApplicationUserName = "Admin",
                    EnvironmentDomainName = "SwiftApis",
                    EnvironmentIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
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
             
                    return StatusCode(404, "No standing orders found.");
                }

                return StatusCode(200, standingOrders);
            }

            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<StandingOrdersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StandingOrdersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<StandingOrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StandingOrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
