using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.Aggregates.AuditTrailAgg
{
    public static class AuditTrailFactory
    {
        public static AuditTrail CreateAuditTrail(string eventType, string activity, Guid? customerId, string applicationUserName, string applicationUserDesignation, string environmentUserName,
            string environmentMachineName, string environmentDomainName, string environmentOSVersion, string environmentMACAddress, string environmentMotherboardSerialNumber,
           string environmentProcessorId, string environmentIPAddress, ServiceHeader serviceHeader)
        {
            var auditTrail = new AuditTrail();

            auditTrail.GenerateNewIdentity();

            auditTrail.EventType = eventType;

            auditTrail.Activity = activity;

            auditTrail.CustomerId = customerId;

            auditTrail.ApplicationUserName = applicationUserName ?? serviceHeader.ApplicationUserName;

            auditTrail.ApplicationUserDesignation = applicationUserDesignation;

            auditTrail.EnvironmentUserName = environmentUserName ?? serviceHeader.EnvironmentUserName;

            auditTrail.EnvironmentMachineName = environmentMachineName ?? serviceHeader.EnvironmentMachineName;

            auditTrail.EnvironmentDomainName = environmentDomainName ?? serviceHeader.EnvironmentDomainName;

            auditTrail.EnvironmentOSVersion = environmentOSVersion ?? serviceHeader.EnvironmentOSVersion;

            auditTrail.EnvironmentMACAddress = environmentMACAddress ?? serviceHeader.EnvironmentMACAddress;

            auditTrail.EnvironmentMotherboardSerialNumber = environmentMotherboardSerialNumber ?? serviceHeader.EnvironmentMotherboardSerialNumber;

            auditTrail.EnvironmentProcessorId = environmentProcessorId ?? serviceHeader.EnvironmentProcessorId;

            auditTrail.EnvironmentIPAddress = environmentIPAddress ?? serviceHeader.EnvironmentIPAddress;

            auditTrail.CreatedBy = applicationUserName ?? serviceHeader.ApplicationUserName;

            auditTrail.CreatedDate = DateTime.Now;

            return auditTrail;
        }
    }
}
