using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts/indefinite-charges")]
    public class IndefiniteChargeController : ApiController
    {
        private readonly IDynamicChargeAppService _dynamicChargeAppService;

        public IndefiniteChargeController(IDynamicChargeAppService dynamicChargeAppService)
        {
            _dynamicChargeAppService = dynamicChargeAppService ?? throw new ArgumentNullException(nameof(dynamicChargeAppService));
        }

        [HttpGet]
        [Route("options")]
        public async Task<IHttpActionResult> GetOptions()
        {
            var recoveryModes = Enum.GetValues(typeof(DynamicChargeRecoveryMode)).Cast<DynamicChargeRecoveryMode>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) }).ToList();
            var recoverySources = Enum.GetValues(typeof(DynamicChargeRecoverySource)).Cast<DynamicChargeRecoverySource>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) }).ToList();
            return Ok(new { success = true, message = "", data = new { RecoveryModes = recoveryModes, RecoverySources = recoverySources } });
        }

        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetPaged(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 200)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Page index must be non-negative and page size must be between 1 and 200.", data = (object)null });
            var header = Utils.CreateServiceHeader();
            var page = string.IsNullOrWhiteSpace(text)
                ? _dynamicChargeAppService.FindDynamicCharges(pageIndex, pageSize, header)
                : _dynamicChargeAppService.FindDynamicCharges(text.Trim(), pageIndex, pageSize, header);
            return Ok(new { success = true, message = "", data = page });
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var header = Utils.CreateServiceHeader();
            var charge = _dynamicChargeAppService.FindDynamicCharge(id, header);
            if (charge == null) return NotFound();
            var commissions = _dynamicChargeAppService.FindCommissions(id, header) ?? new List<CommissionDTO>();
            return Ok(new { success = true, message = "", data = new { DynamicCharge = charge, CommissionIds = commissions.Select(item => item.Id).ToList(), Commissions = commissions } });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(SaveIndefiniteChargeRequest request)
        {
            try
            {
                if (request == null) return Content(HttpStatusCode.BadRequest, new { success = false, message = "Indefinite charge data is required.", data = (object)null });
                var created = _dynamicChargeAppService.AddNewDynamicChargeConfiguration(request.DynamicCharge, request.CommissionIds, Utils.CreateServiceHeader());
                if (created == null) return Content(HttpStatusCode.BadRequest, new { success = false, message = "The indefinite charge could not be created.", data = (object)null });
                return Ok(new { success = true, message = "Indefinite charge created successfully.", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> Update(Guid id, SaveIndefiniteChargeRequest request)
        {
            try
            {
                if (request?.DynamicCharge == null) return Content(HttpStatusCode.BadRequest, new { success = false, message = "Indefinite charge data is required.", data = (object)null });
                request.DynamicCharge.Id = id;
                var updated = _dynamicChargeAppService.UpdateDynamicChargeConfiguration(request.DynamicCharge, request.CommissionIds, Utils.CreateServiceHeader());
                if (updated == null) return NotFound();
                return Ok(new { success = true, message = "Indefinite charge updated successfully.", data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }

    public class SaveIndefiniteChargeRequest
    {
        public DynamicChargeDTO DynamicCharge { get; set; }
        public List<Guid> CommissionIds { get; set; }
    }
}
