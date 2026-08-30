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
    [RoutePrefix("api/accounts/alternatechannels")]
    public class AlternateChannelChargeController : ApiController
    {
        private readonly IAlternateChannelAppService _alternateChannelAppService;

        public AlternateChannelChargeController(IAlternateChannelAppService alternateChannelAppService)
        {
            _alternateChannelAppService = alternateChannelAppService ?? throw new ArgumentNullException(nameof(alternateChannelAppService));
        }

        [HttpGet]
        [Route("charge-options")]
        public async Task<IHttpActionResult> GetOptions()
        {
            return Ok(new { success = true, message = "", data = new {
                AlternateChannelTypes = Options<AlternateChannelType>(),
                KnownChargeTypes = Options<AlternateChannelKnownChargeType>(),
                ChargeBenefactors = Options<ChargeBenefactor>()
            }});
        }

        [HttpGet]
        [Route("types/{type:int}/commissions")]
        public async Task<IHttpActionResult> GetCommissions(int type, int knownChargeType)
        {
            if (!Enum.IsDefined(typeof(AlternateChannelType), type))
                return BadRequestResponse("Select a valid alternate channel type.");
            if (!Enum.IsDefined(typeof(AlternateChannelKnownChargeType), knownChargeType))
                return BadRequestResponse("Select a valid alternate channel charge type.");

            var commissions = _alternateChannelAppService.FindCommissions(type, knownChargeType, Utils.CreateServiceHeader()) ?? new List<CommissionDTO>();
            var first = commissions.FirstOrDefault();
            return Ok(new { success = true, message = "", data = new {
                AlternateChannelType = type,
                KnownChargeType = knownChargeType,
                CommissionIds = commissions.Select(item => item.Id).ToList(),
                Commissions = commissions,
                ChargeBenefactor = first == null ? (int)ChargeBenefactor.Customer : first.ChargeBenefactor
            }});
        }

        [HttpPut]
        [Route("types/{type:int}/commissions")]
        public async Task<IHttpActionResult> UpdateCommissions(int type, UpdateAlternateChannelChargesRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequestResponse("Alternate channel charge mapping data is required.");
                var updated = _alternateChannelAppService.UpdateCommissionsByIds(type, request.CommissionIds, request.KnownChargeType, request.ChargeBenefactor, Utils.CreateServiceHeader());
                if (!updated)
                    return BadRequestResponse("The alternate channel charge mapping could not be updated.");
                return Ok(new { success = true, message = "Alternate channel charges updated successfully.", data = (object)null });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        private static List<object> Options<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Select(value => new { Value = Convert.ToInt32(value), Description = EnumHelper.GetDescription((Enum)(object)value) })
                .OrderBy(item => item.Description).Select(item => (object)item).ToList();
        }

        private IHttpActionResult BadRequestResponse(string message)
        {
            return Content(HttpStatusCode.BadRequest, new { success = false, message, data = (object)null });
        }
    }

    public class UpdateAlternateChannelChargesRequest
    {
        public int KnownChargeType { get; set; }
        public int ChargeBenefactor { get; set; }
        public List<Guid> CommissionIds { get; set; }
    }
}
