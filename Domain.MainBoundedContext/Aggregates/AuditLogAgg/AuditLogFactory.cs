using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.Aggregates.AuditLogAgg
{
    public static class AuditLogFactory
    {
        public static AuditLog CreateAuditLog(string eventType, string tableName, string recordID, string additionalNarration, ServiceHeader serviceHeader)
        {
            var auditLog = new AuditLog();

            auditLog.GenerateNewIdentity();

            auditLog.EventType = eventType;

            auditLog.TableName = tableName;

            auditLog.RecordID = recordID;

            auditLog.AdditionalNarration = additionalNarration;

            auditLog.ApplicationUserName = serviceHeader.ApplicationUserName;

            auditLog.EnvironmentUserName = serviceHeader.EnvironmentUserName;

            auditLog.EnvironmentMachineName = serviceHeader.EnvironmentMachineName;

            auditLog.EnvironmentDomainName = serviceHeader.EnvironmentDomainName;

            auditLog.EnvironmentOSVersion = serviceHeader.EnvironmentOSVersion;

            auditLog.EnvironmentMACAddress = serviceHeader.EnvironmentMACAddress;

            auditLog.EnvironmentMotherboardSerialNumber = serviceHeader.EnvironmentMotherboardSerialNumber;

            auditLog.EnvironmentProcessorId = serviceHeader.EnvironmentProcessorId;

            auditLog.EnvironmentIPAddress = serviceHeader.EnvironmentIPAddress;

            auditLog.CreatedBy = serviceHeader.ApplicationUserName;

            auditLog.CreatedDate = DateTime.Now;

            return auditLog;
        }

        public static AuditLog CreateAuditLog(string EventType, string TableName, string RecordID, string AdditionalNarration, string ApplicationUserName, string EnvironmentUserName,
            string EnvironmentMachineName, string EnvironmentDomainName, string EnvironmentOSVersion, string EnvironmentMACAddress, string EnvironmentMotherboardSerialNumber,
           string EnvironmentProcessorId, string EnvironmentIPAddress, string CreatedBy, DateTime CreatedDate)
        {
            var auditLog = new AuditLog();

            auditLog.GenerateNewIdentity();

            auditLog.EventType = EventType;

            auditLog.TableName = TableName;

            auditLog.RecordID = RecordID;

            auditLog.AdditionalNarration = AdditionalNarration;

            auditLog.ApplicationUserName = ApplicationUserName;

            auditLog.EnvironmentUserName = EnvironmentUserName;

            auditLog.EnvironmentMachineName = EnvironmentMachineName;

            auditLog.EnvironmentDomainName = EnvironmentDomainName;

            auditLog.EnvironmentOSVersion = EnvironmentOSVersion;

            auditLog.EnvironmentMACAddress = EnvironmentMACAddress;

            auditLog.EnvironmentMotherboardSerialNumber = EnvironmentMotherboardSerialNumber;

            auditLog.EnvironmentProcessorId = EnvironmentProcessorId;

            auditLog.EnvironmentIPAddress = EnvironmentIPAddress;

            auditLog.CreatedBy = ApplicationUserName;

            auditLog.CreatedDate = CreatedDate;

            return auditLog;
        }
    }
}
