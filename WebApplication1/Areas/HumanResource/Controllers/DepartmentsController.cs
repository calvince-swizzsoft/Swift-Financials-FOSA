
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using WebApplication1.Helpers;

//[EnableCors(origins: "*", headers: "*", methods: "*")]
namespace WebApplication1.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [Authorize]
    [RoutePrefix("api/humanresource/departments")]
    public class DepartmentsController : ApiController
    {

        private readonly IDepartmentAppService _departmentAppService;

        public DepartmentsController(IDepartmentAppService departmentAppService)
        {
            _departmentAppService = departmentAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {

                var serviceHeader = Utils.CreateServiceHeader();

                var departments = _departmentAppService.FindDepartments(serviceHeader);

                if (departments == null)
                {
                    return NotFound();
                }

                return Ok(departments);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(DepartmentDTO departmentDTO)
        {
            try
            {
                departmentDTO.ValidateAll();

                if (!departmentDTO.HasErrors)
                {
                    //var employeeDTO = await _channelService.AddTellerAsync(employeeDTO);

                    var serviceHeader = Utils.CreateServiceHeader();

                    var createdDepartmentDTO = _departmentAppService.AddNewDepartment(departmentDTO, serviceHeader);

                    return Ok(departmentDTO);
                }

                else
                {
                    return BadRequest(departmentDTO.ErrorMessages.ToString());
                }

            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateDepartment(DepartmentDTO departmentDTO)
        {

            try
            {

               var serviceHeader = Utils.CreateServiceHeader();

                var updatedDepartmentDTO = _departmentAppService.UpdateDepartment(departmentDTO, serviceHeader);

                return Ok(updatedDepartmentDTO);
            }

            catch (Exception)
            {
                throw;
            }
        }


        //employee
    }

}
