using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.MessagingModule.Services;
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
    [RoutePrefix("api/accounts/text-alert-charges")]
    public class TextAlertChargeController : ApiController
    {
        private readonly ITextAlertAppService _textAlertAppService;

        public TextAlertChargeController(ITextAlertAppService textAlertAppService)
        {
            _textAlertAppService = textAlertAppService ?? throw new ArgumentNullException(nameof(textAlertAppService));
        }

        [HttpGet]
        [Route("options")]
        public async Task<IHttpActionResult> GetOptions()
        {
            var transactionCodes = Enum.GetValues(typeof(SystemTransactionCode)).Cast<SystemTransactionCode>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) })
                .OrderBy(item => item.Description).ToList();
            var chargeBenefactors = Enum.GetValues(typeof(ChargeBenefactor)).Cast<ChargeBenefactor>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) })
                .OrderBy(item => item.Value).ToList();
            return Ok(new { success = true, message = "", data = new { TransactionCodes = transactionCodes, ChargeBenefactors = chargeBenefactors } });
        }

        [HttpGet]
        [Route("{systemTransactionCode:int}")]
        public async Task<IHttpActionResult> Get(int systemTransactionCode)
        {
            if (!Enum.IsDefined(typeof(SystemTransactionCode), systemTransactionCode))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a valid system transaction code.", data = (object)null });

            var commissions = _textAlertAppService.FindCommissions(systemTransactionCode, Utils.CreateServiceHeader()) ?? new List<CommissionDTO>();
            var first = commissions.FirstOrDefault();
            return Ok(new { success = true, message = "", data = new {
                SystemTransactionCode = systemTransactionCode,
                CommissionIds = commissions.Select(item => item.Id).ToList(),
                Commissions = commissions,
                ChargeBenefactor = first == null ? (int)ChargeBenefactor.Customer : first.ChargeBenefactor
            }});
        }

        [HttpPut]
        [Route("{systemTransactionCode:int}")]
        public async Task<IHttpActionResult> Update(int systemTransactionCode, UpdateTextAlertChargeRequest request)
        {
            try
            {
                if (request == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Text alert charge mapping data is required.", data = (object)null });

                var updated = _textAlertAppService.UpdateCommissionsByIds(systemTransactionCode, request.CommissionIds, request.ChargeBenefactor, Utils.CreateServiceHeader());
                if (!updated)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The text alert charge mapping could not be updated.", data = (object)null });
                return Ok(new { success = true, message = "Text alert charges updated successfully.", data = (object)null });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }

    public class UpdateTextAlertChargeRequest
    {
        public List<Guid> CommissionIds { get; set; }
        public int ChargeBenefactor { get; set; }
    }
}
