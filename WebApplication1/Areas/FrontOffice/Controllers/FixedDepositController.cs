using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC FixedDepositController — routes through
    // IFixedDepositAppService directly instead of the monolithic _channelService.
    // Customer-account lookups (GetCustomerAccountDetails in the reference controller)
    // are not reproduced here — already covered by the customer-accounts API
    // (docs/api/customer-accounts-api-spec.md); this controller only owns the fixed
    // deposit lifecycle: origination (InvokeFixedDeposit), verify/post
    // (AuditFixedDeposit), batch early termination (RevokeFixedDeposits), and
    // maturity liquidation (PayFixedDeposit).
    [Authorize]
    [RoutePrefix("api/frontoffice/fixeddeposits")]
    public class FixedDepositController : ApiController
    {
        private readonly IFixedDepositAppService _fixedDepositAppService;

        public FixedDepositController(IFixedDepositAppService fixedDepositAppService)
        {
            _fixedDepositAppService = fixedDepositAppService ?? throw new ArgumentNullException(nameof(fixedDepositAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = _fixedDepositAppService.FindFixedDeposits(text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = deposits });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposit = _fixedDepositAppService.FindFixedDeposit(id, serviceHeader);

                if (deposit == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = deposit });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("customer-account/{customerAccountId:guid}")]
        public IHttpActionResult GetByCustomerAccount(Guid customerAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = _fixedDepositAppService.FindFixedDepositsByCustomerAccountId(customerAccountId, serviceHeader);

                return Ok(new { success = true, message = "", data = deposits });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Maturity payout queue — deposits due for a Pay Principal/roll-over decision.
        [HttpGet]
        [Route("payable")]
        public IHttpActionResult GetPayable(DateTime? startDate, DateTime? endDate, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = _fixedDepositAppService.FindPayableFixedDeposits(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = deposits });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Early-termination queue.
        [HttpGet]
        [Route("revocable")]
        public IHttpActionResult GetRevocable(DateTime? startDate, DateTime? endDate, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = _fixedDepositAppService.FindRevocableFixedDeposits(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = deposits });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id:guid}/payables")]
        public IHttpActionResult GetPayables(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var payables = _fixedDepositAppService.FindFixedDepositPayablesByFixedDepositId(id, serviceHeader);

                return Ok(new { success = true, message = "", data = payables });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id:guid}/payables")]
        public IHttpActionResult UpdatePayables(Guid id, [FromBody] List<FixedDepositPayableDTO> payables)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _fixedDepositAppService.UpdateFixedDepositPayables(id, payables ?? new List<FixedDepositPayableDTO>(), serviceHeader);

                if (!result)
                    return BadRequest("Failed to update fixed deposit payables");

                return Ok(new { success = true, message = "Fixed deposit payables updated successfully", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Origination — a fixed deposit is opened at the counter against an existing
        // customer account.
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(FixedDepositDTO fixedDepositDTO)
        {
            if (fixedDepositDTO == null)
                return BadRequest("Request body is required");

            fixedDepositDTO.ValidateAll();
            if (fixedDepositDTO.HasErrors)
                return BadRequest(string.Join("; ", fixedDepositDTO.ErrorMessages));

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var created = _fixedDepositAppService.InvokeFixedDeposit(fixedDepositDTO, serviceHeader);

                if (created == null)
                    return BadRequest("Failed to create the fixed deposit");

                return Ok(new { success = true, message = "Fixed deposit created successfully", data = created });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Verify/Post (or Reject) a newly-opened fixed deposit — checker step.
        [HttpPost]
        [Route("{id:guid}/verify")]
        public IHttpActionResult Verify(Guid id, [FromBody] FixedDepositVerifyRequest request)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _fixedDepositAppService.FindFixedDeposit(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                var option = request?.Approve == true ? (int)FixedDepositAuthOption.Post : (int)FixedDepositAuthOption.Reject;

                var result = _fixedDepositAppService.AuditFixedDeposit(existing, option, request?.ModuleNavigationItemCode ?? 0, serviceHeader);

                if (!result)
                    return Content(System.Net.HttpStatusCode.Conflict, new { success = false, message = "Failed to process the fixed deposit verification", data = (object)null });

                var updated = _fixedDepositAppService.FindFixedDeposit(id, serviceHeader);

                return Ok(new { success = true, message = "Operation success", data = updated });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Batch early termination.
        [HttpPost]
        [Route("terminate")]
        public IHttpActionResult Terminate([FromBody] FixedDepositBatchRequest request)
        {
            if (request?.SelectedFixedDepositIds == null || !request.SelectedFixedDepositIds.Any())
                return BadRequest("No fixed deposit selected for termination.");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = request.SelectedFixedDepositIds
                    .Select(id => _fixedDepositAppService.FindFixedDeposit(id, serviceHeader))
                    .Where(d => d != null)
                    .ToList();

                if (!deposits.Any())
                    return BadRequest("The selected fixed deposit(s) could not be found.");

                var result = _fixedDepositAppService.RevokeFixedDeposits(deposits, request.ModuleNavigationItemCode, serviceHeader);

                if (!result)
                    return BadRequest("An error occurred while terminating the selected fixed deposit(s).");

                return Ok(new { success = true, message = "The selected fixed deposit(s) were successfully terminated.", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Batch maturity liquidation — each deposit must have reached MaturityDate.
        [HttpPost]
        [Route("liquidate")]
        public IHttpActionResult Liquidate([FromBody] FixedDepositBatchRequest request)
        {
            if (request?.SelectedFixedDepositIds == null || !request.SelectedFixedDepositIds.Any())
                return BadRequest("No fixed deposit selected for liquidation.");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var deposits = request.SelectedFixedDepositIds
                    .Select(id => _fixedDepositAppService.FindFixedDeposit(id, serviceHeader))
                    .Where(d => d != null)
                    .ToList();

                if (!deposits.Any())
                    return BadRequest("The selected fixed deposit(s) could not be found.");

                var notMatured = deposits.Where(d => d.MaturityDate > DateTime.Now).ToList();
                if (notMatured.Any())
                    return BadRequest($"The fixed deposit with account number {notMatured.First().CustomerAccountFullAccountNumber} has not yet reached maturity and cannot be liquidated.");

                foreach (var deposit in deposits)
                {
                    var result = _fixedDepositAppService.PayFixedDeposit(deposit, request.ModuleNavigationItemCode, serviceHeader);

                    if (!result)
                        return BadRequest($"An error occurred while liquidating the fixed deposit with ID {deposit.Id}.");
                }

                return Ok(new { success = true, message = "The selected fixed deposit(s) were successfully liquidated.", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class FixedDepositVerifyRequest
    {
        public bool Approve { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }

    public class FixedDepositBatchRequest
    {
        public List<Guid> SelectedFixedDepositIds { get; set; } = new List<Guid>();

        public int ModuleNavigationItemCode { get; set; }
    }
}
