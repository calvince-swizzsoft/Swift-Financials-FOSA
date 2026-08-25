using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/costcenters")]
    public class CostCenterController : ApiController
    {
        private readonly ICostCenterAppService _costCenterAppService;

        public CostCenterController(ICostCenterAppService costCenterAppService)
        {
            _costCenterAppService = costCenterAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var costCenters = _costCenterAppService.FindCostCenters(text ?? "", pageIndex, pageSize, serviceHeader);

                if (costCenters == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = costCenters });
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetCostCenter(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var costCenter = _costCenterAppService.FindCostCenter(id, serviceHeader);

                if (costCenter == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = costCenter });
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CostCenterDTO costCenterDTO)
        {
            try
            {
                costCenterDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (costCenterDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", costCenterDTO.ErrorMessages), data = (object)null });
                }

                var createdCostCenterDTO = _costCenterAppService.AddNewCostCenter(costCenterDTO, serviceHeader);

                if (createdCostCenterDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the cost center could not be created.", data = (object)null });
                }

                // Duplicate Description is reported by echoing the DTO back with
                // ErrorMessageResult set (Id stays Guid.Empty) — same pattern as Treasury.
                if (!string.IsNullOrWhiteSpace(createdCostCenterDTO.ErrorMessageResult))
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = createdCostCenterDTO.ErrorMessageResult, data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = createdCostCenterDTO });
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, CostCenterDTO costCenterDTO)
        {
            try
            {
                costCenterDTO.Id = id;
                costCenterDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (costCenterDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", costCenterDTO.ErrorMessages), data = (object)null });
                }

                // UpdateCostCenter returns bool, not the updated entity — re-fetch so
                // `data` reflects what was actually saved. Note there's no duplicate-
                // description check on update (only on create), unlike Treasury/ChartOfAccount.
                var updated = _costCenterAppService.UpdateCostCenter(costCenterDTO, serviceHeader);

                if (!updated)
                {
                    return NotFound();
                }

                var refreshedCostCenterDTO = _costCenterAppService.FindCostCenter(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshedCostCenterDTO });
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
