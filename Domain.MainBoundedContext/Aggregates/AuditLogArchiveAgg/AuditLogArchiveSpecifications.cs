using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.Aggregates.AuditLogArchiveAgg
{
    public static class AuditLogArchiveSpecifications
    {
        public static Specification<AuditLogArchive> DefaultSpec()
        {
            Specification<AuditLogArchive> specification = new TrueSpecification<AuditLogArchive>();

            return specification;
        }

        public static ISpecification<AuditLogArchive> AuditLogArchiveWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            Specification<AuditLogArchive> specification = new TrueSpecification<AuditLogArchive>();

            if (startDate != null && endDate != null)
            {
                endDate = UberUtil.AdjustTimeSpan(endDate);

                var dateRangeSpec = new DirectSpecification<AuditLogArchive>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                specification &= dateRangeSpec;

                if (!String.IsNullOrWhiteSpace(text))
                {
                    var eventTypeSpec = new DirectSpecification<AuditLogArchive>(c => c.EventType.Contains(text));
                    var tableNameSpec = new DirectSpecification<AuditLogArchive>(c => c.TableName.Contains(text));
                    var recordIDSpec = new DirectSpecification<AuditLogArchive>(c => c.RecordID.Contains(text));
                    var additionalNarrationSpec = new DirectSpecification<AuditLogArchive>(c => c.AdditionalNarration.Contains(text));

                    var applicationUserNameSpec = new DirectSpecification<AuditLogArchive>(c => c.ApplicationUserName.Contains(text));
                    var environmentUserNameSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentUserName.Contains(text));
                    var environmentMachineNameSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMachineName.Contains(text));
                    var environmentOSVersionSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentOSVersion.Contains(text));
                    var environmentMACAddressSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMACAddress.Contains(text));
                    var environmentMotherboardSerialNumberSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMotherboardSerialNumber.Contains(text));
                    var environmentProcessorIdSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentProcessorId.Contains(text));
                    var environmentIPAddressSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentIPAddress.Contains(text));

                    specification &= (eventTypeSpec | tableNameSpec | recordIDSpec | additionalNarrationSpec |
                        applicationUserNameSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentMACAddressSpec | environmentMotherboardSerialNumberSpec | environmentProcessorIdSpec | environmentIPAddressSpec);
                }
            }

            return specification;
        }

        public static Specification<AuditLogArchive> AuditLogArchiveFullText(string text)
        {
            Specification<AuditLogArchive> specification = new TrueSpecification<AuditLogArchive>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var eventTypeSpec = new DirectSpecification<AuditLogArchive>(c => c.EventType.Contains(text));
                var tableNameSpec = new DirectSpecification<AuditLogArchive>(c => c.TableName.Contains(text));
                var recordIDSpec = new DirectSpecification<AuditLogArchive>(c => c.RecordID.Contains(text));
                var additionalNarrationSpec = new DirectSpecification<AuditLogArchive>(c => c.AdditionalNarration.Contains(text));

                var applicationUserNameSpec = new DirectSpecification<AuditLogArchive>(c => c.ApplicationUserName.Contains(text));
                var environmentUserNameSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentUserName.Contains(text));
                var environmentMachineNameSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMachineName.Contains(text));
                var environmentOSVersionSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentOSVersion.Contains(text));
                var environmentMACAddressSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMACAddress.Contains(text));
                var environmentMotherboardSerialNumberSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentMotherboardSerialNumber.Contains(text));
                var environmentProcessorIdSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentProcessorId.Contains(text));
                var environmentIPAddressSpec = new DirectSpecification<AuditLogArchive>(c => c.EnvironmentIPAddress.Contains(text));

                specification &= (eventTypeSpec | tableNameSpec | recordIDSpec | additionalNarrationSpec |
                    applicationUserNameSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentMACAddressSpec | environmentMotherboardSerialNumberSpec | environmentProcessorIdSpec | environmentIPAddressSpec);
            }

            return specification;
        }
    }
}
