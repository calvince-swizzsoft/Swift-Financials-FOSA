using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
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
    [RoutePrefix("api/accounts/banklinkages")]
    public class BankLinkageController : ApiController
    {
        private readonly IBankLinkageAppService _bankLinkageAppService;
        private readonly IBankAppService _bankAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public BankLinkageController(
            IBankLinkageAppService bankLinkageAppService,
            IBankAppService bankAppService,
            IChartOfAccountAppService chartOfAccountAppService)
        {
            _bankLinkageAppService = bankLinkageAppService;
            _bankAppService = bankAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Index(string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var page = string.IsNullOrWhiteSpace(text)
                    ? _bankLinkageAppService.FindBankLinkages(pageIndex, pageSize, serviceHeader)
                    : _bankLinkageAppService.FindBankLinkages(text, pageIndex, pageSize, serviceHeader);

                if (page == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = page });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var bankLinkages = _bankLinkageAppService.FindBankLinkages(serviceHeader);

                if (bankLinkages == null)
                {
                    bankLinkages = new List<BankLinkageDTO>();
                }

                var balances = new Dictionary<Tuple<Guid, Guid>, decimal>();
                foreach (var branchGroup in bankLinkages.Where(item => item.BranchId != Guid.Empty && item.ChartOfAccountId != Guid.Empty).GroupBy(item => item.BranchId))
                {
                    var generalLedgerAccounts = branchGroup.Select(item => item.ChartOfAccountId).Distinct()
                        .Select(accountId => _chartOfAccountAppService.FindGeneralLedgerAccount(accountId, serviceHeader))
                        .Where(account => account != null).ToList();
                    _chartOfAccountAppService.FetchGeneralLedgerAccountBalances(branchGroup.Key, generalLedgerAccounts, DateTime.Now, serviceHeader, TransactionDateFilter.CreatedDate, false);
                    foreach (var account in generalLedgerAccounts)
                        balances[Tuple.Create(branchGroup.Key, account.Id)] = account.Balance;
                }

                foreach (var bankLinkage in bankLinkages)
                {
                    var bank = _bankAppService.FindBank(bankLinkage.BankId, serviceHeader);
                    if (bank != null && bank.Id != Guid.Empty)
                    {
                        bankLinkage.SwiftCode = bank.SwiftCode;
                        bankLinkage.Address = bank.Address;
                        bankLinkage.City = bank.City;
                        bankLinkage.IbanNo = bank.IbanNo;
                        bankLinkage.No = bank.No;
                    }

                    decimal balance;
                    bankLinkage.BankLinkageBalance = balances.TryGetValue(Tuple.Create(bankLinkage.BranchId, bankLinkage.ChartOfAccountId), out balance)
                        ? balance
                        : 0m;
                }

                return Ok(new { success = true, message = "", data = bankLinkages });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetBankLinkage(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var bankLinkage = _bankLinkageAppService.FindBankLinkage(id, serviceHeader);

                if (bankLinkage == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = bankLinkage });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("by-bank-account/{bankAccountId}")]
        public async Task<IHttpActionResult> GetByBankAccountId(Guid bankAccountId)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var bankLinkage = _bankLinkageAppService.FindBankLinkageByBankAccountId(bankAccountId, serviceHeader);

                if (bankLinkage == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = bankLinkage });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(BankLinkageDTO bankLinkageDTO)
        {
            try
            {
                if (bankLinkageDTO == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid bank linkage data", data = (object)null });
                }

                bankLinkageDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (bankLinkageDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", bankLinkageDTO.ErrorMessages), data = (object)null });
                }

                var createdBankLinkage = _bankLinkageAppService.AddNewBankLinkage(bankLinkageDTO, serviceHeader);

                if (createdBankLinkage == null)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Sorry, but the bank linkage could not be created.", data = (object)null });
                }

                return Ok(new { success = true, message = "Operation Success", data = createdBankLinkage });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(Guid id, BankLinkageDTO bankLinkageDTO)
        {
            try
            {
                if (bankLinkageDTO == null || bankLinkageDTO.Id != id)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = "Invalid bank linkage data", data = (object)null });
                }

                bankLinkageDTO.ValidateAll();

                var serviceHeader = Utils.CreateServiceHeader();

                if (bankLinkageDTO.HasErrors)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", bankLinkageDTO.ErrorMessages), data = (object)null });
                }

                var existing = _bankLinkageAppService.FindBankLinkage(id, serviceHeader);
                if (existing == null)
                {
                    return NotFound();
                }

                var updated = _bankLinkageAppService.UpdateBankLinkage(bankLinkageDTO, serviceHeader);

                if (!updated)
                {
                    return Content(HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update bank linkage", data = (object)null });
                }

                var refreshed = _bankLinkageAppService.FindBankLinkage(id, serviceHeader);

                return Ok(new { success = true, message = "Operation Success", data = refreshed });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
