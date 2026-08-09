using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // ILoanProductAppService (Accounts module) previously had no dedicated
    // controller at all — only ever injected into the legacy ValuesController
    // monolith, no route exposed a plain list. Distinct from the separate
    // legacy loan-product API the Loaning/LoanProducts.jsx frontend screen
    // talks to — this is the Accounts-module LoanProduct aggregate only.
    // Added so any picker (e.g. ChequeTypeController's Create — see
    // CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md) has a real endpoint to list
    // loan products from.
    [Authorize]
    [RoutePrefix("api/accounts/loanproducts")]
    public class LoanProductController : ApiController
    {
        private readonly ILoanProductAppService _loanProductAppService;

        public LoanProductController(ILoanProductAppService loanProductAppService)
        {
            _loanProductAppService = loanProductAppService ?? throw new ArgumentNullException(nameof(loanProductAppService));
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);

                return Ok(new { success = true, message = "", data = loanProducts ?? new List<LoanProductDTO>() });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
