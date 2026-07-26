using System;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO
{
    public class AuditTrailDTO
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        [Display(Name = "Activity")]
        public string Activity { get; set; }

        [Display(Name = "Customer")]
        public Guid? CustomerId { get; set; }

        [Display(Name = "App. User Name")]
        public string ApplicationUserName { get; set; }

        [Display(Name = "App. Designation")]
        public string ApplicationUserDesignation { get; set; }

        [Display(Name = "Env. User Name")]
        public string EnvironmentUserName { get; set; }

        [Display(Name = "Env. Machine Name")]
        public string EnvironmentMachineName { get; set; }

        [Display(Name = "Env. Domain Name")]
        public string EnvironmentDomainName { get; set; }

        [Display(Name = "Env. Operating System Version")]
        public string EnvironmentOSVersion { get; set; }

        [Display(Name = "Env. MAC Address")]
        public string EnvironmentMACAddress { get; set; }

        [Display(Name = "Env. Motherboard Serial Number")]
        public string EnvironmentMotherboardSerialNumber { get; set; }

        [Display(Name = "Env. Processor Id")]
        public string EnvironmentProcessorId { get; set; }

        [Display(Name = "Env. IP Address")]
        public string EnvironmentIPAddress { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
