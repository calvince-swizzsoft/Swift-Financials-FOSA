using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace WebApplication1.Helpers
{
    public static class Utils
    {

        public static ServiceHeader CreateServiceHeader()
        {
            var applicationDomain = ConfigurationManager.AppSettings["ApplicationDomainName"];
            if (string.IsNullOrWhiteSpace(applicationDomain) || ConfigurationManager.ConnectionStrings[applicationDomain] == null)
                throw new ConfigurationErrorsException("ApplicationDomainName must identify a configured database connection.");
            var principal = HttpContext.Current?.User as ClaimsPrincipal;

            var applicationUserName = principal?.Identity?.Name ?? "System";

            // Roles come from the validated JWT's role claims, not any client-supplied value.
            var applicationUserRoles = principal?.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList() ?? new List<string>();
            Guid applicationUserBranchId;
            var branchClaim = principal?.FindFirst("BranchId")?.Value;
            var hasApplicationUserBranch = Guid.TryParse(branchClaim, out applicationUserBranchId);
            Guid applicationUserEmployeeId;
            var employeeClaim = principal?.FindFirst("EmployeeId")?.Value;
            var hasApplicationUserEmployee = Guid.TryParse(employeeClaim, out applicationUserEmployeeId);
            var request = HttpContext.Current?.Request;
            var clientIPAddress = request?.UserHostAddress ?? "";

            return new ServiceHeader
            {
                ApplicationDomainName = applicationDomain,
                ApplicationUserName = applicationUserName,   // was hardcoded — now pulled from the validated JWT
                ApplicationUserRoles = applicationUserRoles,
                ApplicationUserBranchId = hasApplicationUserBranch ? (Guid?)applicationUserBranchId : null,
                ApplicationUserEmployeeId = hasApplicationUserEmployee ? (Guid?)applicationUserEmployeeId : null,
                EnforceTransactionThresholds = principal?.Identity?.IsAuthenticated == true,
                EnvironmentDomainName = applicationDomain,
                EnvironmentIPAddress = HttpContext.Current?.Request?.UserHostAddress ?? "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName,
                ClientIPAddress = clientIPAddress,
                ClientDeviceId = request?.Headers["X-Client-Device-Id"] ?? "",
                ClientUserAgent = request?.UserAgent ?? "",
                ServerMachineName = Environment.MachineName,
                ServerOSVersion = Environment.OSVersion.ToString()
            };
        }

        // Denomination fields on FiscalCountDTO/CashTransferRequestDTO each hold that
        // denomination's own monetary subtotal (not a raw note/coin piece count) — e.g.
        // DenominationOneThousandValue is "how much was counted in 1000-notes", already
        // in currency terms. Reconciling against a transaction total is therefore a plain
        // sum, not a count-times-face-value calculation.
        public static decimal SumDenominationValues(
            decimal oneThousandValue, decimal fiveHundredValue, decimal twoHundredValue, decimal oneHundredValue,
            decimal fiftyValue, decimal fourtyValue, decimal twentyValue, decimal tenValue, decimal fiveValue,
            decimal oneValue, decimal fiftyCentValue)
        {
            return oneThousandValue + fiveHundredValue + twoHundredValue + oneHundredValue + fiftyValue
                 + fourtyValue + twentyValue + tenValue + fiveValue + oneValue + fiftyCentValue;
        }

    }
}
