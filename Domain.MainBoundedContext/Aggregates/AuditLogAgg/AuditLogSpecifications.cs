using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.Aggregates.AuditLogAgg
{
    public static class AuditLogSpecifications
    {
        public static Specification<AuditLog> DefaultSpec()
        {
            Specification<AuditLog> specification = new TrueSpecification<AuditLog>();

            return specification;
        }

        public static ISpecification<AuditLog> AuditLogWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            Specification<AuditLog> specification = new TrueSpecification<AuditLog>();

            if (startDate != null && endDate != null)
            {
                endDate = UberUtil.AdjustTimeSpan(endDate);

                var dateRangeSpec = new DirectSpecification<AuditLog>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                specification &= dateRangeSpec;

                if (!String.IsNullOrWhiteSpace(text))
                {
                    text = text.SanitizePatIndexInput();

                    var eventTypeSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EventType) > 0);
                    var tableNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.TableName) > 0);
                    var recordIDSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.RecordID) > 0);
                    var additionalNarrationSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.AdditionalNarration) > 0);

                    var applicationUserNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.ApplicationUserName) > 0);
                    var environmentUserNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentUserName) > 0);
                    var environmentMachineNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMachineName) > 0);
                    var environmentOSVersionSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentOSVersion) > 0);
                    var environmentMACAddressSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMACAddress) > 0);
                    var environmentMotherboardSerialNumberSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMotherboardSerialNumber) > 0);
                    var environmentProcessorIdSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentProcessorId) > 0);
                    var environmentIPAddressSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentIPAddress) > 0);

                    specification &= (eventTypeSpec | tableNameSpec | recordIDSpec | additionalNarrationSpec |
                        applicationUserNameSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentMACAddressSpec | environmentMotherboardSerialNumberSpec | environmentProcessorIdSpec | environmentIPAddressSpec);
                }
            }

            return specification;
        }

        public static Specification<AuditLog> AuditLogFullText(string text)
        {
            Specification<AuditLog> specification = new TrueSpecification<AuditLog>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                var eventTypeSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EventType) > 0);
                var tableNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.TableName) > 0);
                var recordIDSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.RecordID) > 0);
                var additionalNarrationSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.AdditionalNarration) > 0);

                var applicationUserNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.ApplicationUserName) > 0);
                var environmentUserNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentUserName) > 0);
                var environmentMachineNameSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMachineName) > 0);
                var environmentOSVersionSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentOSVersion) > 0);
                var environmentMACAddressSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMACAddress) > 0);
                var environmentMotherboardSerialNumberSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentMotherboardSerialNumber) > 0);
                var environmentProcessorIdSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentProcessorId) > 0);
                var environmentIPAddressSpec = new DirectSpecification<AuditLog>(c => SqlFunctions.PatIndex(text, c.EnvironmentIPAddress) > 0);

                specification &= (eventTypeSpec | tableNameSpec | recordIDSpec | additionalNarrationSpec |
                    applicationUserNameSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentMACAddressSpec | environmentMotherboardSerialNumberSpec | environmentProcessorIdSpec | environmentIPAddressSpec);
            }

            return specification;
        }
    }
}
