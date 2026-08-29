using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [Authorize]
    [RoutePrefix("api/humanresource/employeetypes")]
    public class EmployeeTypesController : ApiController
    {

        private readonly IEmployeeTypeAppService _employeeTypeAppService;

        public EmployeeTypesController(
            IEmployeeTypeAppService employeeTypeAppService)
        {
            _employeeTypeAppService = employeeTypeAppService;
        }
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();
                var employeeTypes = _employeeTypeAppService.FindEmployeeTypes(serviceHeader);

                if (employeeTypes == null)
                {
                    return NotFound();
                }

                return Ok(employeeTypes);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(EmployeeTypeDTO employeeTypeDTO)
        {
            try
            {
                if (employeeTypeDTO == null)
                    return BadRequest("Employee type details are required.");

                employeeTypeDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (!employeeTypeDTO.HasErrors)
                {
                    var validationError = _employeeTypeAppService.ValidateEmployeeType(employeeTypeDTO, null, serviceHeader);
                    if (!string.IsNullOrWhiteSpace(validationError))
                        return BadRequest(validationError);

                    var createdEmployeeTypeDTO = _employeeTypeAppService.AddNewEmployeeType(employeeTypeDTO, serviceHeader);
                    if (createdEmployeeTypeDTO == null)
                        return BadRequest("The employee type could not be created.");

                    if (!string.IsNullOrWhiteSpace(createdEmployeeTypeDTO.ErrorMessageResult))
                        return BadRequest(createdEmployeeTypeDTO.ErrorMessageResult);

                    return Ok(createdEmployeeTypeDTO);
                }

                else
                {
                    return BadRequest(string.Join(" ", employeeTypeDTO.ErrorMessages));
                }

            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeType(Guid id, EmployeeTypeDTO employeeTypeDTO)
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

                employeeTypeDTO.Id = id;
                var validationError = _employeeTypeAppService.ValidateEmployeeType(employeeTypeDTO, id, serviceHeader);
                if (!string.IsNullOrWhiteSpace(validationError))
                    return BadRequest(validationError);

                var updatedEmployeeTypeDTO = _employeeTypeAppService.UpdateEmployeeType(employeeTypeDTO, serviceHeader);

                return Ok(updatedEmployeeTypeDTO);
            }

            catch (Exception)
            {
                throw;
            }
        }


        //employee
    }

}
