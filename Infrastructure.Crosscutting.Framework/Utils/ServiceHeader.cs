using System.Collections.Generic;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public class ServiceHeader
    {
        public string ApplicationDomainName { get; set; }

        public string ApplicationUserName { get; set; }

        /// <summary>
        /// Roles held by the calling user, sourced server-side from the validated auth token/identity.
        /// Never trust a client-supplied role name in its place.
        /// </summary>
        public List<string> ApplicationUserRoles { get; set; } = new List<string>();

        /// <summary>Branch from the authenticated identity. This is server-derived and must not be populated from request bodies.</summary>
        public System.Guid? ApplicationUserBranchId { get; set; }

        public string EnvironmentUserName { get; set; }

        public string EnvironmentMachineName { get; set; }

        public string EnvironmentDomainName { get; set; }

        public string EnvironmentOSVersion { get; set; }

        public string EnvironmentMACAddress { get; set; }

        public string EnvironmentMotherboardSerialNumber { get; set; }

        public string EnvironmentProcessorId { get; set; }

        public string EnvironmentIPAddress { get; set; }
    }
}
