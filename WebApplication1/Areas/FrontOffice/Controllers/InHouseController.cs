using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    // Adapted from the reference MVC InHouseController — SACCO-issued outward
    // cheques (loan disbursement/payout cheques), routed through
    // IInHouseChequeAppService instead of the monolithic _channelService.
    //
    // The GL-account/customer-account/cheque-type/branch/payee lookup actions on
    // the reference controller (GetGLAccounts, GetCustomerAccountDetails,
    // GetChequeTypeDetails, GetBranches, GetPayeeLookupData) are not reproduced —
    // each duplicates an already-documented endpoint (chart-of-accounts,
    // customer-accounts, branches) or a plain paged lookup this controller
    // already exposes (GetUnprinted doubles as payee lookup).
    //
    // PrintInHouseCheque itself does no local printing (confirmed in
    // InHouseChequeAppService — it only flips IsPrinted/PrintedNumber and posts a
    // GL journal); the client renders/prints the cheque and reports back the
    // printed cheque number.
    [Authorize]
    [RoutePrefix("api/frontoffice/inhousecheques")]
    public class InHouseController : ApiController
    {
        private readonly IInHouseChequeAppService _inHouseChequeAppService;

        public InHouseController(IInHouseChequeAppService inHouseChequeAppService)
        {
            _inHouseChequeAppService = inHouseChequeAppService ?? throw new ArgumentNullException(nameof(inHouseChequeAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheques = (startDate.HasValue || endDate.HasValue)
                    ? _inHouseChequeAppService.FindInHouseCheques(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _inHouseChequeAppService.FindInHouseCheques(text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = cheques });
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

                var cheque = _inHouseChequeAppService.FindInHouseCheque(id, serviceHeader);

                if (cheque == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = cheque });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Printing queue — cheques built but not yet printed, for a branch.
        [HttpGet]
        [Route("unprinted")]
        public IHttpActionResult GetUnprinted(Guid branchId, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var cheques = _inHouseChequeAppService.FindUnPrintedInHouseChequesByBranchId(branchId, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = cheques });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Build a batch of in-house cheque entries in one submission (the
        // reference controller's AddEntry/RemoveEntries/Create sequence — here
        // the client assembles the batch client-side and submits it in one call,
        // per the ChequeBankingRequest composite-DTO precedent).
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] InHouseChequeBatchRequest request)
        {
            if (request?.Cheques == null || !request.Cheques.Any())
                return BadRequest("No cheque entries found.");

            foreach (var cheque in request.Cheques)
            {
                cheque.ValidateAll();

                if (cheque.HasErrors)
                    return BadRequest(string.Join("; ", cheque.ErrorMessages));
            }

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var result = _inHouseChequeAppService.AddNewInHouseCheques(new List<InHouseChequeDTO>(request.Cheques), request.ModuleNavigationItemCode, serviceHeader);

                if (!result)
                    return BadRequest("Failed to submit the cheque entries.");

                return Ok(new { success = true, message = "Cheque(s) submitted successfully.", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("{id:guid}/print")]
        public IHttpActionResult Print(Guid id, [FromBody] InHouseChequePrintRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required");

            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _inHouseChequeAppService.FindInHouseCheque(id, serviceHeader);

                if (existing == null)
                    return NotFound();

                existing.PrintedNumber = request.PrintedNumber;

                var result = _inHouseChequeAppService.PrintInHouseCheque(existing, request.BankLinkage, request.ModuleNavigationItemCode, serviceHeader);

                if (!result)
                    return BadRequest("Failed to print the cheque.");

                var updated = _inHouseChequeAppService.FindInHouseCheque(id, serviceHeader);

                return Ok(new { success = true, message = "Cheque printed successfully.", data = updated });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class InHouseChequeBatchRequest
    {
        public List<InHouseChequeDTO> Cheques { get; set; } = new List<InHouseChequeDTO>();

        public int ModuleNavigationItemCode { get; set; }
    }

    public class InHouseChequePrintRequest
    {
        public string PrintedNumber { get; set; }

        public BankLinkageDTO BankLinkage { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }
}
