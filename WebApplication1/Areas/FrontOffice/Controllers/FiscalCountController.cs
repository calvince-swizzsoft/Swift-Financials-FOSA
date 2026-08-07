using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC FiscalCountController — a standalone
    // audit/browse view over denomination-count records (FiscalCountDTO),
    // routed through IFiscalCountAppService instead of the monolithic
    // _channelService.
    //
    // Fixed, not ported forward: the reference controller's grid action
    // (Index(JQueryDataTablesModel)) called _channelService
    // .FindTreasuriesByFilterInPageAsync — the wrong service entirely (Treasury,
    // not FiscalCount), so it never actually listed fiscal counts. This uses
    // IFiscalCountAppService.FindFiscalCounts.
    //
    // Posting a fiscal count directly here is for ad-hoc/manual denomination
    // records only — the normal path is inline, as part of a treasury cash
    // movement (CashManagementController), an End of Day close
    // (EndOfDayController), or a cash transfer request (TransfersController,
    // which has nowhere of its own to persist a denomination breakdown and
    // writes a companion FiscalCount instead) — all three already build/post
    // their own FiscalCountDTO.
    [Authorize]
    [RoutePrefix("api/frontoffice/fiscalcounts")]
    public class FiscalCountController : ApiController
    {
        private readonly IFiscalCountAppService _fiscalCountAppService;

        public FiscalCountController(IFiscalCountAppService fiscalCountAppService)
        {
            _fiscalCountAppService = fiscalCountAppService ?? throw new ArgumentNullException(nameof(fiscalCountAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var fiscalCounts = (startDate.HasValue || endDate.HasValue)
                    ? _fiscalCountAppService.FindFiscalCounts(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _fiscalCountAppService.FindFiscalCounts(text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = fiscalCounts });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var fiscalCount = _fiscalCountAppService.FindFiscalCount(id, serviceHeader);

                if (fiscalCount == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = fiscalCount });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(FiscalCountDTO fiscalCountDTO)
        {
            if (fiscalCountDTO == null)
                return BadRequest("Request body is required");

            fiscalCountDTO.ValidateAll();
            if (fiscalCountDTO.HasErrors)
                return BadRequest(string.Join("; ", fiscalCountDTO.ErrorMessages));

            var countedTotal = Utils.SumDenominationValues(
                fiscalCountDTO.DenominationOneThousandValue, fiscalCountDTO.DenominationFiveHundredValue,
                fiscalCountDTO.DenominationTwoHundredValue, fiscalCountDTO.DenominationOneHundredValue,
                fiscalCountDTO.DenominationFiftyValue, fiscalCountDTO.DenominationFourtyValue,
                fiscalCountDTO.DenominationTwentyValue, fiscalCountDTO.DenominationTenValue,
                fiscalCountDTO.DenominationFiveValue, fiscalCountDTO.DenominationOneValue,
                fiscalCountDTO.DenominationFiftyCentValue);

            if (countedTotal != fiscalCountDTO.TotalValue)
                return BadRequest($"Counted denominations ({countedTotal}) do not match the total value ({fiscalCountDTO.TotalValue}).");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _fiscalCountAppService.AddNewFiscalCount(fiscalCountDTO, serviceHeader);

                if (created == null)
                    return BadRequest("Failed to create the fiscal count");

                return Ok(new { success = true, message = "Fiscal count created successfully", data = created });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
