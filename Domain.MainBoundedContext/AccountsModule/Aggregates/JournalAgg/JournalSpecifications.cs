using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg
{
    public static class JournalSpecifications
    {
        public static ISpecification<Journal> JournalWithId(Guid journalId)
        {
            Specification<Journal> specification = new DirectSpecification<Journal>(x => x.Id == journalId);

            return specification;
        }

        public static ISpecification<Journal> JournalWithAlternateChannelLogId(DateTime startDate, DateTime endDate, params Guid[] alternateChannelLogIds)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<Journal> specification = new DirectSpecification<Journal>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            var alternateChannelLogIdSpecs = new List<Specification<Journal>>();

            if (alternateChannelLogIds != null)
            {
                Array.ForEach(alternateChannelLogIds, (alternateChannelLogId) =>
                {
                    var alternateChannelLogIdSpec = new DirectSpecification<Journal>(x => x.AlternateChannelLogId == alternateChannelLogId);

                    alternateChannelLogIdSpecs.Add(alternateChannelLogIdSpec);
                });

                if (alternateChannelLogIdSpecs.Any())
                {
                    var spec0 = alternateChannelLogIdSpecs[0];

                    for (int i = 1; i < alternateChannelLogIdSpecs.Count; i++)
                    {
                        spec0 |= alternateChannelLogIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static ISpecification<Journal> JournalWithAlternateChannelLogId(params Guid[] alternateChannelLogIds)
        {
            Specification<Journal> specification = new TrueSpecification<Journal>();

            var alternateChannelLogIdSpecs = new List<Specification<Journal>>();

            if (alternateChannelLogIds != null)
            {
                Array.ForEach(alternateChannelLogIds, (alternateChannelLogId) =>
                {
                    var alternateChannelLogIdSpec = new DirectSpecification<Journal>(x => x.AlternateChannelLogId == alternateChannelLogId);

                    alternateChannelLogIdSpecs.Add(alternateChannelLogIdSpec);
                });

                if (alternateChannelLogIdSpecs.Any())
                {
                    var spec0 = alternateChannelLogIdSpecs[0];

                    for (int i = 1; i < alternateChannelLogIdSpecs.Count; i++)
                    {
                        spec0 |= alternateChannelLogIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static ISpecification<Journal> JournalWithDateRange(DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<Journal> specification = new DirectSpecification<Journal>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            return specification;
        }

        public static ISpecification<Journal> ReversibleJournalWithDateRangeAndFullText(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, ServiceHeader serviceHeader)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<Journal> specification = new DirectSpecification<Journal>(x =>
                x.CreatedDate >= startDate && x.CreatedDate <= endDate &&
                x.TransactionCode == systemTransactionCode &&
                !x.IsLocked /*locked entries cannot be reversed*/ &&
                x.ApplicationUserName != serviceHeader.ApplicationUserName /*user cannot reverse own tx*/);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalFilter)journalFilter)
                {
                    case JournalFilter.PostingPeriod:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.PostingPeriod.Description) > 0);
                        break;
                    case JournalFilter.Branch:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case JournalFilter.TotalValue:

                        var totalValue = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out totalValue))
                        {
                            specification &= new DirectSpecification<Journal>(x => x.TotalValue == totalValue);
                        }

                        break;
                    case JournalFilter.PrimaryDescription:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.PrimaryDescription) > 0);
                        break;
                    case JournalFilter.SecondaryDescription:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.SecondaryDescription) > 0);
                        break;
                    case JournalFilter.Reference:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case JournalFilter.ApplicationUserName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.ApplicationUserName) > 0);
                        break;
                    case JournalFilter.EnvironmentUserName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentUserName) > 0);
                        break;
                    case JournalFilter.EnvironmentMachineName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMachineName) > 0);
                        break;
                    case JournalFilter.EnvironmentDomainName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentDomainName) > 0);
                        break;
                    case JournalFilter.EnvironmentOSVersion:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentOSVersion) > 0);
                        break;
                    case JournalFilter.EnvironmentMACAddress:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMACAddress) > 0);
                        break;
                    case JournalFilter.EnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalFilter.EnvironmentProcessorId:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentProcessorId) > 0);
                        break;
                    case JournalFilter.EnvironmentIPAddress:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentIPAddress) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static ISpecification<Journal> JournalWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text, int journalFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<Journal> specification = new DirectSpecification<Journal>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalFilter)journalFilter)
                {
                    case JournalFilter.PostingPeriod:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.PostingPeriod.Description) > 0);
                        break;
                    case JournalFilter.Branch:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.Branch.Description) > 0);
                        break;
                    case JournalFilter.TotalValue:

                        var totalValue = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out totalValue))
                        {
                            specification &= new DirectSpecification<Journal>(x => x.TotalValue == totalValue);
                        }

                        break;
                    case JournalFilter.PrimaryDescription:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.PrimaryDescription) > 0);
                        break;
                    case JournalFilter.SecondaryDescription:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.SecondaryDescription) > 0);
                        break;
                    case JournalFilter.Reference:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case JournalFilter.ApplicationUserName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.ApplicationUserName) > 0);
                        break;
                    case JournalFilter.EnvironmentUserName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentUserName) > 0);
                        break;
                    case JournalFilter.EnvironmentMachineName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMachineName) > 0);
                        break;
                    case JournalFilter.EnvironmentDomainName:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentDomainName) > 0);
                        break;
                    case JournalFilter.EnvironmentOSVersion:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentOSVersion) > 0);
                        break;
                    case JournalFilter.EnvironmentMACAddress:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMACAddress) > 0);
                        break;
                    case JournalFilter.EnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalFilter.EnvironmentProcessorId:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentProcessorId) > 0);
                        break;
                    case JournalFilter.EnvironmentIPAddress:
                        specification &= new DirectSpecification<Journal>(c => SqlFunctions.PatIndex(text, c.EnvironmentIPAddress) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
