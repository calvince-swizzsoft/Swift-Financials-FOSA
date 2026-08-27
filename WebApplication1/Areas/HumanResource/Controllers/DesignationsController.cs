
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/humanresource/designations")]
    public class DesignationsController : ApiController
    {


        private readonly IDesignationAppService _designationAppService;

        public DesignationsController(IDesignationAppService designationAppService)
        {
            _designationAppService = designationAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {

                var serviceHeader = Utils.CreateServiceHeader();

                var designations = _designationAppService.FindDesignations(serviceHeader);

                return Ok(designations);
            }

            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(DesignationDTO designationDTO)
        {
            try
            {
                
                designationDTO.ValidateAll();

                if (!designationDTO.HasErrors)
                {

                    var serviceHeader = Utils.CreateServiceHeader();
                    //var employeeDTO = await _channelService.AddTellerAsync(employeeDTO);

                    var createdDesignationDTO = _designationAppService.AddNewDesignation(designationDTO, serviceHeader);

                    return Ok(designationDTO);
                }

                else
                {
                    return BadRequest(designationDTO.ErrorMessages.ToString());
                }

            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateDesignation(DesignationDTO designationDTO)
        {

            try
            {

                var serviceHeader = new ServiceHeader();
                var updatedDesignationDTO = _designationAppService.UpdateDesignation(designationDTO, serviceHeader);

                return Ok(updatedDesignationDTO);
            }

            catch (Exception)
            {
                throw;
            }
        }


        //employee
    }

}
