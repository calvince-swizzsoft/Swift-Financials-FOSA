using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Helpers
{
    public static class Utils
    {

        public static ServiceHeader CreateServiceHeader()
        {
            return new ServiceHeader
            {
                ApplicationDomainName = "SwiftApis",
                ApplicationUserName = "Admin",
                EnvironmentDomainName = "SwiftApis",
                EnvironmentIPAddress = "",
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