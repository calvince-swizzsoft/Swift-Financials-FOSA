using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
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
    [RoutePrefix("api/accounts/well-known-charges")]
    public class WellKnownChargeController : ApiController
    {
        private readonly ICommissionAppService _commissionAppService;

        public WellKnownChargeController(ICommissionAppService commissionAppService)
        {
            _commissionAppService = commissionAppService ?? throw new ArgumentNullException(nameof(commissionAppService));
        }

        [HttpGet]
        [Route("transaction-types")]
        public async Task<IHttpActionResult> GetTransactionTypes()
        {
            var items = Enum.GetValues(typeof(SystemTransactionType)).Cast<SystemTransactionType>()
                .Select(value => new { Value = (int)value, Description = EnumHelper.GetDescription(value) })
                .OrderBy(item => item.Description)
                .ToList();
            return Ok(new { success = true, message = "", data = items });
        }

        [HttpGet]
        [Route("{systemTransactionType:int}")]
        public async Task<IHttpActionResult> Get(int systemTransactionType)
        {
            if (!Enum.IsDefined(typeof(SystemTransactionType), systemTransactionType))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Select a valid predefined system transaction type.", data = (object)null });

            var mappings = _commissionAppService.GetCommissionsForSystemTransactionType(systemTransactionType, Utils.CreateServiceHeader()) ?? new List<CommissionDTO>();
            var first = mappings.FirstOrDefault();
            return Ok(new
            {
                success = true,
                message = "",
                data = new
                {
                    SystemTransactionType = systemTransactionType,
                    CommissionIds = mappings.Select(item => item.Id).ToList(),
                    Commissions = mappings,
                    ComplementType = first == null ? (int)ChargeType.Percentage : first.ComplementType,
                    ComplementPercentage = first == null ? 0d : first.ComplementPercentage,
                    ComplementFixedAmount = first == null ? 0m : first.ComplementFixedAmount
                }
            });
        }

        [HttpPut]
        [Route("{systemTransactionType:int}")]
        public async Task<IHttpActionResult> Update(int systemTransactionType, UpdateWellKnownChargeRequest request)
        {
            try
            {
                if (request == null)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Well-known charge mapping data is required.", data = (object)null });

                var complement = new ChargeDTO
                {
                    Type = request.ComplementType,
                    Percentage = request.ComplementPercentage,
                    FixedAmount = request.ComplementFixedAmount
                };
                var updated = _commissionAppService.MapSystemTransactionTypeToCommissionIds(
                    systemTransactionType,
                    request.CommissionIds,
                    complement,
                    Utils.CreateServiceHeader());
                if (!updated)
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "The well-known charge mapping could not be updated.", data = (object)null });

                return Ok(new { success = true, message = "Well-known charges updated successfully.", data = (object)null });
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }

    public class UpdateWellKnownChargeRequest
    {
        public List<Guid> CommissionIds { get; set; }
        public int ComplementType { get; set; }
        public double ComplementPercentage { get; set; }
        public decimal ComplementFixedAmount { get; set; }
    }
}
