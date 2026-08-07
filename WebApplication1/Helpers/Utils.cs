using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace WebApplication1.Helpers
{
    public static class Utils
    {

        public static ServiceHeader CreateServiceHeader()
        {

            var principal = HttpContext.Current?.User as ClaimsPrincipal;

            var applicationUserName = principal?.Identity?.Name ?? "System";

            // Roles come from the validated JWT's role claims, not any client-supplied value.
            var applicationUserRoles = principal?.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList() ?? new List<string>();

            return new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = applicationUserName,   // was hardcoded — now pulled from the validated JWT
                ApplicationUserRoles = applicationUserRoles,
                EnvironmentDomainName = "SwiftApis",
                EnvironmentIPAddress = HttpContext.Current?.Request?.UserHostAddress ?? "",
                EnvironmentMACAddress = "",
                EnvironmentMachineName = Environment.MachineName,
                EnvironmentMotherboardSerialNumber = "",
                EnvironmentOSVersion = Environment.OSVersion.ToString(),
                EnvironmentProcessorId = "",
                EnvironmentUserName = Environment.UserName
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