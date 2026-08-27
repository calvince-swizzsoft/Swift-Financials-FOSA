using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SwiftFinancials.AuditLogDispatcher.Configuration
{
    public class AuditLogMessageProcessor : MessageProcessor<List<AuditLogBCP>>
    {
        private readonly ILogger _logger;
        private readonly IAuditLogAppService _auditLogAppService;
        private readonly AuditLogDispatcherConfigSection _configuration;

        public AuditLogMessageProcessor(ILogger logger, IAuditLogAppService auditLogAppService,
            AuditLogDispatcherConfigSection configuration)
            : base(configuration.AuditLogDispatcherSettingsItems.QueuePath,
                  configuration.AuditLogDispatcherSettingsItems.QueueReceivers)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _auditLogAppService = auditLogAppService ?? throw new ArgumentNullException(nameof(auditLogAppService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->AuditLogMessageProcessor...", exception,
                _configuration.AuditLogDispatcherSettingsItems.QueuePath);
        }

        protected override async Task Process(List<AuditLogBCP> auditLogs, int appSpecific)
        {
            if ((MessageCategory)appSpecific != MessageCategory.AuditLog)
                throw new InvalidOperationException("The audit-log queue received an unsupported message category.");
            if (auditLogs == null || !auditLogs.Any()) return;

            var applicationDomainName = auditLogs[0].AppDomainName;
            var settings = FindSettings(applicationDomainName);
            if (settings == null)
                throw new InvalidOperationException(string.Format(
                    "No audit-log dispatcher settings exist for application domain '{0}'.", applicationDomainName));
            if (settings.Enabled != 1) return;

            var serviceHeader = new ServiceHeader { ApplicationDomainName = applicationDomainName };
            if (!await _auditLogAppService.AddNewAuditLogsAsync(auditLogs.Select(Map).ToList(), serviceHeader))
                throw new InvalidOperationException("The audit-log batch could not be persisted.");
        }

        private AuditLogDispatcherSettingsElement FindSettings(string applicationDomainName)
        {
            foreach (var item in _configuration.AuditLogDispatcherSettingsItems)
            {
                var settings = (AuditLogDispatcherSettingsElement)item;
                if (string.Equals(settings.UniqueId, applicationDomainName, StringComparison.OrdinalIgnoreCase))
                    return settings;
            }
            return null;
        }

        private static AuditLogDTO Map(AuditLogBCP auditLog)
        {
            return new AuditLogDTO
            {
                EventType = auditLog.EventType, TableName = auditLog.TableName, RecordID = auditLog.RecordID,
                AdditionalNarration = Serialize(auditLog.AuditInfoWrapper),
                ApplicationUserName = auditLog.ApplicationUserName, EnvironmentUserName = auditLog.EnvironmentUserName,
                EnvironmentMachineName = auditLog.EnvironmentMachineName, EnvironmentDomainName = auditLog.EnvironmentDomainName,
                EnvironmentOSVersion = auditLog.EnvironmentOSVersion, EnvironmentMACAddress = auditLog.EnvironmentMACAddress,
                EnvironmentMotherboardSerialNumber = auditLog.EnvironmentMotherboardSerialNumber,
                EnvironmentProcessorId = auditLog.EnvironmentProcessorId, EnvironmentIPAddress = auditLog.EnvironmentIPAddress,
                CreatedBy = auditLog.CreatedBy, CreatedDate = auditLog.CreatedDate
            };
        }

        private static string Serialize(AuditInfoWrapper auditInfo)
        {
            if (auditInfo == null) return null;
            var serializer = new XmlSerializer(typeof(AuditInfoWrapper));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, auditInfo);
                return writer.ToString();
            }
        }
    }
}
