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



    }
}