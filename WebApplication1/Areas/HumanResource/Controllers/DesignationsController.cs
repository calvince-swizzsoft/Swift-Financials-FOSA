
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

                foreach (var designation in designations ?? new System.Collections.Generic.List<DesignationDTO>())
                {
                    designation.TransactionThresholds = new ObservableCollection<TransactionThresholdDTO>(
                        _designationAppService.FindTransactionThresholdCollection(designation.Id, serviceHeader)
                        ?? new System.Collections.Generic.List<TransactionThresholdDTO>());
                }

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

                    if (createdDesignationDTO == null)
                        return BadRequest("The designation could not be created.");

                    return Ok(createdDesignationDTO);
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
        public async Task<IHttpActionResult> UpdateDesignation(Guid id, DesignationDTO designationDTO)
        {

            try
            {

                designationDTO.Id = id;
                var serviceHeader = Utils.CreateServiceHeader();
                var updatedDesignationDTO = _designationAppService.UpdateDesignation(designationDTO, serviceHeader);

                if (!updatedDesignationDTO)
                    return NotFound();

                return Ok(updatedDesignationDTO);
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("{id:guid}/transaction-thresholds")]
        public IHttpActionResult GetTransactionThresholds(Guid id)
        {
            var thresholds = _designationAppService.FindTransactionThresholdCollection(id, Utils.CreateServiceHeader());
            return Ok(thresholds ?? new List<TransactionThresholdDTO>());
        }

        [HttpPut]
        [Route("{id:guid}/transaction-thresholds")]
        public IHttpActionResult UpdateTransactionThresholds(Guid id, List<TransactionThresholdDTO> thresholds)
        {
            var updated = _designationAppService.UpdateTransactionThresholdCollection(
                id,
                thresholds ?? new List<TransactionThresholdDTO>(),
                Utils.CreateServiceHeader());

            return updated ? (IHttpActionResult)Ok() : BadRequest("Transaction thresholds could not be updated.");
        }
    }

}
