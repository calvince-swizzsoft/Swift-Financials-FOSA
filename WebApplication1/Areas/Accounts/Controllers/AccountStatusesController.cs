using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApplication1.Helpers;

namespace WebApplication1.Areas.Accounts.Controllers
{
    // Read-only Customer 360 inquiry described by Dashboard > Utilities > Account Statuses.
    // No transaction endpoints belong here: employees must never transact on their own accounts.
    [Authorize]
    [RoutePrefix("api/accounts/account-statuses")]
    public class AccountStatusesController : ApiController
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString;

        public AccountStatusesController(ICustomerAppService customerAppService, ICustomerAccountAppService customerAccountAppService, IAuthorizationAppService authorizationAppService)
        {
            _customerAppService = customerAppService ?? throw new ArgumentNullException(nameof(customerAppService));
            _customerAccountAppService = customerAccountAppService ?? throw new ArgumentNullException(nameof(customerAccountAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        [HttpGet, Route("customers")]
        public async Task<IHttpActionResult> SearchCustomers([FromUri] string text = "", [FromUri] int customerFilter = 0, [FromUri] int pageIndex = 0, [FromUri] int pageSize = 20)
        {
            if (pageIndex < 0 || pageSize < 1 || pageSize > 100)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "pageIndex must be non-negative and pageSize must be between 1 and 100." });
            try
            {
                var header = Utils.CreateServiceHeader();
                var page = string.IsNullOrWhiteSpace(text)
                    ? await _customerAppService.FindCustomersAsync(pageIndex, pageSize, header)
                    : await _customerAppService.FindCustomersAsync(text.Trim(), customerFilter, pageIndex, pageSize, header);
                return Ok(new { success = true, message = "Customers retrieved successfully.", data = page });
            }
            catch (Exception) { throw; }
        }

        [HttpGet, Route("customers/{customerId:guid}")]
        public async Task<IHttpActionResult> GetCustomerStatus(Guid customerId)
        {
            try
            {
                var header = Utils.CreateServiceHeader();
                var customer = await _customerAppService.FindCustomerAsync(customerId, header);
                if (customer == null)
                    return Content(HttpStatusCode.NotFound, new { success = false, message = "Customer not found." });

                var access = CanViewCustomer(customerId, header);
                if (!access.Allowed)
                    return Content(HttpStatusCode.Forbidden, new { success = false, message = access.Message });

                var accounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId, header) ?? new List<Application.MainBoundedContext.DTO.AccountsModule.CustomerAccountDTO>();
                var referees = await _customerAppService.FindRefereeCollectionAsync(customerId, header);

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var signatories = await QueryAsync(connection, @"SELECT s.Id,s.CustomerAccountId,s.FirstName,s.LastName,s.IdentityCardNumber,s.Relationship,s.Address_MobileLine AS MobileLine,s.Address_Email AS Email,s.Remarks,s.CreatedDate FROM swiftFin_CustomerAccountSignatories s INNER JOIN swiftFin_CustomerAccounts a ON a.Id=s.CustomerAccountId WHERE a.CustomerId=@CustomerId ORDER BY s.CreatedDate DESC", customerId);
                    var standingOrders = await QueryAsync(connection, @"SELECT so.Id,so.BenefactorCustomerAccountId,so.BeneficiaryCustomerAccountId,so.Duration_StartDate AS StartDate,so.Duration_EndDate AS EndDate,so.Schedule_Frequency AS Frequency,so.Schedule_ExpectedRunDate AS ExpectedRunDate,so.PaymentPerPeriod,so.Remarks,so.IsLocked FROM swiftFin_StandingOrders so WHERE so.BenefactorCustomerAccountId IN (SELECT Id FROM swiftFin_CustomerAccounts WHERE CustomerId=@CustomerId) OR so.BeneficiaryCustomerAccountId IN (SELECT Id FROM swiftFin_CustomerAccounts WHERE CustomerId=@CustomerId) ORDER BY so.CreatedDate DESC", customerId);
                    var alternateChannels = await QueryAsync(connection, @"SELECT ac.Id,ac.CustomerAccountId,ac.Type,ac.CardNumber,ac.ValidFrom,ac.Expires,ac.DailyLimit,ac.Remarks,ac.IsLocked FROM swiftFin_AlternateChannels ac INNER JOIN swiftFin_CustomerAccounts a ON a.Id=ac.CustomerAccountId WHERE a.CustomerId=@CustomerId ORDER BY ac.CreatedDate DESC", customerId);
                    var unclearedCheques = await QueryAsync(connection, @"SELECT ec.Id,ec.CustomerAccountId,ec.Number,ec.Amount,ec.Drawer,ec.DrawerBank,ec.DrawerBankBranch,ec.WriteDate,ec.MaturityDate,ec.Remarks FROM swiftFin_ExternalCheques ec INNER JOIN swiftFin_CustomerAccounts a ON a.Id=ec.CustomerAccountId WHERE a.CustomerId=@CustomerId AND ec.IsCleared=0 ORDER BY ec.CreatedDate DESC", customerId);
                    var fixedDeposits = await QueryAsync(connection, @"SELECT fd.Id,fd.CustomerAccountId,fd.BranchId,fd.Category,fd.Value,fd.Term,fd.Rate,fd.Status,fd.MaturityDate,fd.ExpectedInterest,fd.TotalExpected,fd.Remarks FROM swiftFin_FixedDeposits fd INNER JOIN swiftFin_CustomerAccounts a ON a.Id=fd.CustomerAccountId WHERE a.CustomerId=@CustomerId ORDER BY fd.CreatedDate DESC", customerId);
                    var electronicFundsTransfers = await QueryAsync(connection, @"SELECT e.Id,e.CustomerAccountId,e.WireTransferBatchId,e.Amount,e.Payee,e.AccountNumber,e.Reference,e.ThirdPartyResponse,e.Status,e.CreatedDate FROM swiftFin_WireTransferBatchEntries e INNER JOIN swiftFin_CustomerAccounts a ON a.Id=e.CustomerAccountId WHERE a.CustomerId=@CustomerId ORDER BY e.CreatedDate DESC", customerId);
                    var loansGuaranteed = await QueryAsync(connection, @"SELECT lg.Id,lg.CustomerId,lg.LoaneeCustomerId,lg.LoanProductId,lg.LoanCaseId,lg.Status,lg.TotalShares,lg.CommittedShares,lg.AmountGuaranteed,lg.AmountPledged,lg.CreatedDate FROM swiftFin_LoanGuarantors lg WHERE lg.CustomerId=@CustomerId ORDER BY lg.CreatedDate DESC", customerId);
                    var loanGuarantors = await QueryAsync(connection, @"SELECT lg.Id,lg.CustomerId,lg.LoaneeCustomerId,lg.LoanProductId,lg.LoanCaseId,lg.Status,lg.TotalShares,lg.CommittedShares,lg.AmountGuaranteed,lg.AmountPledged,lg.CreatedDate FROM swiftFin_LoanGuarantors lg WHERE lg.LoaneeCustomerId=@CustomerId ORDER BY lg.CreatedDate DESC", customerId);

                    return Ok(new { success = true, message = "Customer account status retrieved successfully.", data = new { customer, accounts, referees, signatories, standingOrders, alternateChannels, unclearedCheques, fixedDeposits, electronicFundsTransfers, loansGuaranteed, loanGuarantors, isEmployeeAccount = access.IsEmployee, isOwnEmployeeAccount = access.IsOwnEmployee } });
                }
            }
            catch (Exception) { throw; }
        }

        private AccessDecision CanViewCustomer(Guid customerId, Infrastructure.Crosscutting.Framework.Utils.ServiceHeader header)
        {
            var principal = HttpContext.Current?.User as ClaimsPrincipal;
            Guid callerEmployeeId;
            var callerIsEmployee = Guid.TryParse(principal?.FindFirst("EmployeeId")?.Value, out callerEmployeeId);
            if (!callerIsEmployee) return AccessDecision.Allow(false, false);

            Guid? targetEmployeeId = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT TOP 1 Id FROM swiftFin_Employees WHERE CustomerId=@CustomerId", connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);
                connection.Open();
                var value = command.ExecuteScalar();
                if (value != null && value != DBNull.Value) targetEmployeeId = (Guid)value;
            }
            if (!targetEmployeeId.HasValue) return AccessDecision.Allow(false, false);
            if (targetEmployeeId.Value == callerEmployeeId) return AccessDecision.Allow(true, true);

            var allowedRoles = _authorizationAppService.GetRolesForSystemPermissionType((int)SystemPermissionType.EmployeeCustomerAccountViewing, header) ?? new string[0];
            var permitted = header.ApplicationUserRoles.Any(role => allowedRoles.Any(allowed => string.Equals(role, allowed, StringComparison.OrdinalIgnoreCase)));
            return permitted ? AccessDecision.Allow(true, false) : AccessDecision.Deny("You do not have Employee Account Viewing permission to view another employee's account status.");
        }

        private static async Task<List<Dictionary<string, object>>> QueryAsync(SqlConnection connection, string sql, Guid customerId)
        {
            var rows = new List<Dictionary<string, object>>();
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }

        private sealed class AccessDecision
        {
            public bool Allowed { get; private set; }
            public bool IsEmployee { get; private set; }
            public bool IsOwnEmployee { get; private set; }
            public string Message { get; private set; }
            public static AccessDecision Allow(bool employee, bool own) { return new AccessDecision { Allowed = true, IsEmployee = employee, IsOwnEmployee = own }; }
            public static AccessDecision Deny(string message) { return new AccessDecision { Allowed = false, Message = message }; }
        }
    }
}
