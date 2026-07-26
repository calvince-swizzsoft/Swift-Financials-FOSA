using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO
{
    public class SignalRClientProfile
    {
        [DataMember]
        [Display(Name = "Connection Id")]
        public string ConnectionId { get; set; }

        [DataMember]
        [Display(Name = "Client Type")]
        public string ClientType { get; set; }

        [DataMember]
        [Display(Name = "App. Domain Name")]
        public string ApplicationDomainName { get; set; }

        [DataMember]
        [Display(Name = "App. User Name")]
        public string ApplicationUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. User Name")]
        public string EnvironmentUserName { get; set; }

        [DataMember]
        [Display(Name = "Env. Machine Name")]
        public string EnvironmentMachineName { get; set; }

        [DataMember]
        [Display(Name = "Env. Domain Name")]
        public string EnvironmentDomainName { get; set; }

        [DataMember]
        [Display(Name = "Env. Operating System Version")]
        public string EnvironmentOSVersion { get; set; }

        [DataMember]
        [Display(Name = "Env. MAC Address")]
        public string EnvironmentMACAddress { get; set; }

        [DataMember]
        [Display(Name = "Env. Motherboard Serial Number")]
        public string EnvironmentMotherboardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Env. Processor Id")]
        public string EnvironmentProcessorId { get; set; }

        [DataMember]
        [Display(Name = "Env. IP Address")]
        public string EnvironmentIPAddress { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Format("{0}", this.EnvironmentMachineName));
            stringBuilder.AppendLine(string.Format("{0}", this.ApplicationDomainName));
            stringBuilder.AppendLine(string.Format("{0}", this.EnvironmentMACAddress));
            stringBuilder.AppendLine(string.Format("{0}", this.ConnectionId));

            return string.Format("{0}", stringBuilder);
        }
    }
}
