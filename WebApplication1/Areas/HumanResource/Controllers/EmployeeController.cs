using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.xmp.impl;
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

    [Authorize]
    [RoutePrefix("api/humanresource/employees")]
    public class EmployeeController : ApiController
    {
        private readonly IEmployeeAppService _employeeAppService;
        public EmployeeController(IEmployeeAppService  employeeAppService) { 
        
            _employeeAppService = employeeAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader(); 

                var employees = _employeeAppService.FindEmployees(serviceHeader);

         
                if (employees == null)
                {
                    return NotFound();
                }

                return Ok(employees);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Find(Guid id)
        {
            try
            {
                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();
                var employee = _employeeAppService.FindEmployee(id, serviceHeader);

                if (employee == null)
                    return NotFound();

                return Ok(employee);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(EmployeeDTO employeeDTO)
        {
            try
            {
                employeeDTO.ValidateAll();

                if (!employeeDTO.HasErrors)
                {

                    var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                    var createdEmployeeDTO = _employeeAppService.AddNewEmployee(employeeDTO, serviceHeader);

                   
                    return Ok(createdEmployeeDTO);
                }

                else
                {
                    return BadRequest(employeeDTO.ErrorMessages.ToString());
                }

            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateEmployee(Guid id, EmployeeDTO employeeDTO)
        {

            try
            {
                if (employeeDTO == null)
                    return BadRequest("Employee details are required.");

                employeeDTO.Id = id;
                employeeDTO.ValidateAll();
                if (employeeDTO.HasErrors)
                    return BadRequest(string.Join("; ", employeeDTO.ErrorMessages));

                var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();

                var updatedEmployeeDTO = _employeeAppService.UpdateEmployee(employeeDTO, serviceHeader);

                

                return Ok(updatedEmployeeDTO);
            }

            catch (Exception)
            {
                throw;
            }
        }





    }
}
