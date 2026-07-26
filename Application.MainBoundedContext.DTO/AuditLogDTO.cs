using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class AuditLogDTO : BindingModelBase<AuditLogDTO>
    {
        public AuditLogDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        [DataMember]
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        [DataMember]
        [Display(Name = "Record Id")]
        public string RecordID { get; set; }

        [DataMember]
        [Display(Name = "Narration")]
        public string AdditionalNarration { get; set; }

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

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
