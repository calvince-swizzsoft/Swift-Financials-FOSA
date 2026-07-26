using Domain.Seedwork;

namespace Domain.MainBoundedContext.Aggregates.AuditLogAgg
{
    public class AuditLog : Entity
    {
        public string EventType { get; set; }

        public string TableName { get; set; }

        public string RecordID { get; set; }

        public string AdditionalNarration { get; set; }

        public string ApplicationUserName { get; set; }

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
