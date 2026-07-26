using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.Aggregates.AuditLogArchiveAgg
{
    public static class AuditLogArchiveFactory
    {
        public static AuditLogArchive CreateAuditLogArchive(string eventType, string tableName, string recordID, string additionalNarration, ServiceHeader serviceHeader)
        {
            var auditLog = new AuditLogArchive();

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

            auditLog.CreatedDate = DateTime.Now;

            return auditLog;
        }
    }
}
