using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.Aggregates.AuditTrailAgg
{
    public static class AuditTrailSpecifications
    {
        public static Specification<AuditTrail> DefaultSpec()
        {
            Specification<AuditTrail> specification = new TrueSpecification<AuditTrail>();

            return specification;
        }

        public static ISpecification<AuditTrail> AuditTrailWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            Specification<AuditTrail> specification = new TrueSpecification<AuditTrail>();

            if (startDate != null && endDate != null)
            {
                endDate = UberUtil.AdjustTimeSpan(endDate);

                var dateRangeSpec = new DirectSpecification<AuditTrail>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                specification &= dateRangeSpec;

                if (!String.IsNullOrWhiteSpace(text))
                {
                    text = text.SanitizePatIndexInput();

                    var eventTypeSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EventType) > 0);
                    var activitySpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.Activity) > 0);

                    var applicationUserNameSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.ApplicationUserName) > 0);
                    var applicationUserDesignationSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.ApplicationUserDesignation) > 0);
                    var environmentUserNameSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentUserName) > 0);
                    var environmentMachineNameSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentMachineName) > 0);
                    var environmentOSVersionSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentOSVersion) > 0);
                    var environmentMACAddressSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentMACAddress) > 0);
                    var environmentMotherboardSerialNumberSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentMotherboardSerialNumber) > 0);
                    var environmentProcessorIdSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentProcessorId) > 0);
                    var environmentIPAddressSpec = new DirectSpecification<AuditTrail>(c => SqlFunctions.PatIndex(text, c.EnvironmentIPAddress) > 0);

                    specification &= (eventTypeSpec | activitySpec |
                        applicationUserNameSpec | applicationUserDesignationSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentMACAddressSpec | environmentMotherboardSerialNumberSpec | environmentProcessorIdSpec | environmentIPAddressSpec);
                }
            }

            return specification;
        }
    }
}
