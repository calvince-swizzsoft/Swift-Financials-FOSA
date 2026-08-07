using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
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
    [RoutePrefix("api/accounts/chartofaccounts")]
    public class ChartOfAccountController : ApiController
    {
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public ChartOfAccountController(IChartOfAccountAppService chartOfAccountAppService)
        {
            _chartOfAccountAppService = chartOfAccountAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var chartOfAccounts = _chartOfAccountAppService.FindChartOfAccounts(text ?? "", pageIndex, pageSize, serviceHeader);

                if (chartOfAccounts == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = chartOfAccounts });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetChartOfAccount(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var chartOfAccount = _chartOfAccountAppService.FindChartOfAccount(id, serviceHeader);

                if (chartOfAccount == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = chartOfAccount });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Separate read model from the flat CRUD list — this is the only place
        // Depth/Children come back populated (AddNewChartOfAccount/UpdateChartOfAccount
        // don't maintain them), built for hierarchical rendering.
        [HttpGet]
        [Route("tree")]
        public async Task<IHttpActionResult> Tree()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var generalLedgerAccounts = _chartOfAccountAppService.FindGeneralLedgerAccounts(serviceHeader, updateDepth: true);

                return Ok(new { success = true, message = "", data = generalLedgerAccounts });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(ChartOfAccountDTO chartOfAccountDTO)
        {
            try
            {
                chartOfAccountDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (chartOfAccountDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", chartOfAccountDTO.ErrorMessages), data = (object)null });
                }

                // AddNewChartOfAccount silently falls back to creating a root account
                // if ParentId doesn't resolve to a persisted account, instead of erroring —
                // catch that here rather than let a typo'd ParentId silently produce the
                // wrong hierarchy.
                if (chartOfAccountDTO.ParentId.HasValue && chartOfAccountDTO.ParentId.Value != Guid.Empty)
                {
                    var parent = _chartOfAccountAppService.FindChartOfAccount(chartOfAccountDTO.ParentId.Value, serviceHeader);

                    if (parent == null)
                    {
                        return Content(HttpStatusCode.BadRequest, new { success = false, message = "Parent chart of account not found.", data = (object)null });
                    }
                }

                var createdChartOfAccountDTO = _chartOfAccountAppService.AddNewChartOfAccount(chartOfAccountDTO, serviceHeader);

                if (createdChartOfAccountDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the chart of account could not be created.", data = (object)null });
                }

                // Duplicate AccountCode is reported by echoing the DTO back with
                // ErrorMessageResult set (Id stays Guid.Empty) — same pattern as Treasury.
                if (!string.IsNullOrWhiteSpace(createdChartOfAccountDTO.ErrorMessageResult))
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = createdChartOfAccountDTO.ErrorMessageResult, data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = createdChartOfAccountDTO });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, ChartOfAccountDTO chartOfAccountDTO)
        {
            try
            {
                chartOfAccountDTO.Id = id;
                chartOfAccountDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (chartOfAccountDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", chartOfAccountDTO.ErrorMessages), data = (object)null });
                }

                if (chartOfAccountDTO.ParentId.HasValue && chartOfAccountDTO.ParentId.Value != Guid.Empty)
                {
                    var parent = _chartOfAccountAppService.FindChartOfAccount(chartOfAccountDTO.ParentId.Value, serviceHeader);

                    if (parent == null)
                    {
                        return Content(HttpStatusCode.BadRequest, new { success = false, message = "Parent chart of account not found.", data = (object)null });
                    }
                }

                bool updated;

                try
                {
                    // Unlike AddNewChartOfAccount, UpdateChartOfAccount throws on a
                    // duplicate AccountCode instead of using ErrorMessageResult —
                    // translate that to 409 rather than letting it fall through to 500.
                    updated = _chartOfAccountAppService.UpdateChartOfAccount(chartOfAccountDTO, serviceHeader);
                }
                catch (InvalidOperationException ex)
                {
                    return Content(HttpStatusCode.Conflict, new { success = false, message = ex.Message, data = (object)null });
                }

                if (!updated)
                {
                    return NotFound();
                }

                var refreshedChartOfAccountDTO = _chartOfAccountAppService.FindChartOfAccount(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshedChartOfAccountDTO });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("systemgeneralledgermappings")]
        public async Task<IHttpActionResult> SystemGeneralLedgerAccountMappings(int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var mappings = _chartOfAccountAppService.FindSystemGeneralLedgerAccountMappings(pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = mappings });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // MapSystemGeneralLedgerAccountCodeToChartOfAccount already updates the
        // existing mapping for this code if one exists, or creates it if not —
        // one idempotent upsert endpoint, not separate POST/PUT.
        [HttpPut]
        [Route("systemgeneralledgermappings/{systemGeneralLedgerAccountCode}")]
        public async Task<IHttpActionResult> MapSystemGeneralLedgerAccountCode(int systemGeneralLedgerAccountCode, [FromBody] Guid chartOfAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                if (chartOfAccountId == Guid.Empty)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "chartOfAccountId is required.", data = (object)null });
                }

                var target = _chartOfAccountAppService.FindChartOfAccount(chartOfAccountId, serviceHeader);

                if (target == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Chart of account not found.", data = (object)null });
                }

                var mapped = _chartOfAccountAppService.MapSystemGeneralLedgerAccountCodeToChartOfAccount(systemGeneralLedgerAccountCode, chartOfAccountId, serviceHeader);

                if (!mapped)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the mapping could not be saved.", data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = (object)null });
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
