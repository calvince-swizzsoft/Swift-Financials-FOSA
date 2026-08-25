using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Adapted from the reference MVC CoA_ChequeBooksController — chequebook
    // issuance against a customer's savings account, plus per-leaf payment
    // voucher pay/flag management. Routed through IChequeBookAppService
    // instead of the monolithic _channelService.
    //
    // Not ported literally: the reference controller's POST Edit action took
    // a ChequeBookDTO as its route/view model but actually validated and
    // saved a CustomerAccountDTO parameter via UpdateCustomerAccountAsync —
    // a copy-paste bug that never touched a chequebook at all. Update here
    // correctly calls IChequeBookAppService.UpdateChequeBook. The reference
    // controller's GetSavingsProductsAsync/GetInvestmentProductsAsync (Create
    // form product pickers) and Add (TempData-staged multi-step wizard) are
    // also not reproduced — they duplicate already-documented product
    // listing endpoints or rely on session state this stateless API doesn't
    // have.
    //
    // ChequeBookAppService lives in AccountsModule (not FrontOfficeModule),
    // and the reference controller lived under Areas/Accounts — this
    // controller follows that placement rather than grouping with the other
    // (FrontOfficeModule-owned) cheque controllers.
    [Authorize]
    [RoutePrefix("api/accounts/chequebooks")]
    public class ChequeBookController : ApiController
    {
        private readonly IChequeBookAppService _chequeBookAppService;

        public ChequeBookController(IChequeBookAppService chequeBookAppService)
        {
            _chequeBookAppService = chequeBookAppService ?? throw new ArgumentNullException(nameof(chequeBookAppService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Index(string text = "", int? type = null, int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = type.HasValue
                    ? _chequeBookAppService.FindChequeBooks(type.Value, text ?? "", pageIndex, pageSize, serviceHeader)
                    : _chequeBookAppService.FindChequeBooks(text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Unpaged — for pickers, matching the ChequeTypeController /all precedent.
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var chequeBooks = _chequeBookAppService.FindChequeBooks(serviceHeader);

                return Ok(new { success = true, message = "", data = chequeBooks ?? new List<ChequeBookDTO>() });
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

                var chequeBook = _chequeBookAppService.FindChequeBook(id, serviceHeader);

                if (chequeBook == null)
                    return NotFound();

                return Ok(new { success = true, message = "", data = chequeBook });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CreateChequeBookRequest request)
        {
            try
            {
                if (request?.ChequeBook == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid cheque book data", data = (object)null });
                }

                request.ChequeBook.ValidateAll();

                if (request.ChequeBook.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", request.ChequeBook.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var created = _chequeBookAppService.AddNewChequeBook(request.ChequeBook, request.ModuleNavigationItemCode, serviceHeader);

                if (created == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the cheque book could not be created — check that the number of vouchers and initial voucher number are both greater than zero.", data = (object)null });
                }

                return Ok(new { success = true, message = "Cheque book created successfully", data = created });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Also how a chequebook is activated/locked — IsActive: false->true
        // triggers ActivateChequeBook (and deactivates the customer's other
        // chequebooks), IsLocked toggles Lock/UnLock. Both handled inside
        // IChequeBookAppService.UpdateChequeBook.
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, ChequeBookDTO chequeBookDTO)
        {
            try
            {
                if (chequeBookDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid cheque book data", data = (object)null });
                }

                chequeBookDTO.Id = id;
                chequeBookDTO.ValidateAll();

                if (chequeBookDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", chequeBookDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var existing = _chequeBookAppService.FindChequeBook(id, serviceHeader);
                if (existing == null)
                    return NotFound();

                var updated = _chequeBookAppService.UpdateChequeBook(chequeBookDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update cheque book", data = (object)null });
                }

                var refreshed = _chequeBookAppService.FindChequeBook(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Per-leaf vouchers for a chequebook (paged) — the printable/payable
        // cheque-leaf ledger AddNewChequeBook seeds one row per leaf for.
        [HttpGet]
        [Route("{id:guid}/vouchers")]
        public IHttpActionResult GetVouchers(Guid id, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var vouchers = _chequeBookAppService.FindPaymentVouchersByChequeBookId(id, text ?? "", pageIndex, pageSize, serviceHeader);

                return Ok(new { success = true, message = "", data = vouchers });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Matches a presented cheque leaf against its issuing chequebook by
        // voucher number + chequebook reference — the same lookup
        // ElectronicJournalAppService uses internally to auto-match KBACTS
        // clearing-house records against ChequeBook vouchers.
        [HttpGet]
        [Route("vouchers/match")]
        public IHttpActionResult MatchVoucher(int chequeBookType, int voucherNumber, string chequeBookReference)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var vouchers = _chequeBookAppService.FindPaymentVouchersByVoucherNumberAndChequeBookReference(chequeBookType, voucherNumber, chequeBookReference ?? "", serviceHeader);

                return Ok(new { success = true, message = "", data = vouchers ?? new List<PaymentVoucherDTO>() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Full DTO expected back (fetched from GetVouchers, then edited) —
        // same fetch-edit-resubmit contract ChequeTypeController.Update uses.
        // Payee/Reference/Amount/WriteDate are the fields PayVoucher actually
        // persists, so they're validated; ChequeBookId travels along from the
        // original fetch but isn't re-used by PayVoucher itself.
        [HttpPost]
        [Route("vouchers/{id:guid}/pay")]
        public IHttpActionResult PayVoucher(Guid id, PaymentVoucherDTO paymentVoucherDTO)
        {
            try
            {
                if (paymentVoucherDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid payment voucher data", data = (object)null });
                }

                paymentVoucherDTO.Id = id;
                paymentVoucherDTO.ValidateAll();

                if (paymentVoucherDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", paymentVoucherDTO.ErrorMessages), data = (object)null });
                }

                var serviceHeader = Utils.CreateServiceHeader();

                var paid = _chequeBookAppService.PayVoucher(paymentVoucherDTO, serviceHeader);

                if (!paid)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Failed to pay the voucher — it may not exist or has already been paid.", data = (object)null });
                }

                return Ok(new { success = true, message = "Voucher paid successfully", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Flag/Unflag only touches Status + Reference server-side (see
        // FlagVoucher), so — unlike Pay — this doesn't run full DTO
        // validation; Payee/Amount/WriteDate aren't read for this action and
        // shouldn't block it.
        [HttpPost]
        [Route("vouchers/{id:guid}/flag")]
        public IHttpActionResult FlagVoucher(Guid id, PaymentVoucherDTO paymentVoucherDTO)
        {
            try
            {
                if (paymentVoucherDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid payment voucher data", data = (object)null });
                }

                paymentVoucherDTO.Id = id;

                var serviceHeader = Utils.CreateServiceHeader();

                var flagged = _chequeBookAppService.FlagVoucher(paymentVoucherDTO, serviceHeader);

                if (!flagged)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Failed to update the voucher — it may not exist or has already been paid.", data = (object)null });
                }

                return Ok(new { success = true, message = "Voucher updated successfully", data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CreateChequeBookRequest
    {
        public ChequeBookDTO ChequeBook { get; set; }

        public int ModuleNavigationItemCode { get; set; }
    }
}
