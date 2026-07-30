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

            var applicationUserName = (HttpContext.Current?.User as ClaimsPrincipal)?.Identity?.Name ?? "System";

            return new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = applicationUserName,   // was hardcoded — now pulled from the validated JWT
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