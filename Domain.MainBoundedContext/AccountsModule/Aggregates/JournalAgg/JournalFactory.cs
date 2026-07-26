using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg
{
    public static class JournalFactory
    {
        public static Journal CreateJournal(Guid? parentJournalId, Guid postingPeriodId, Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, ServiceHeader serviceHeader, bool suppressAccountAlert = false)
        {
            var journal = new Journal();

            journal.GenerateNewIdentity();

            journal.ParentId = (parentJournalId != null && parentJournalId != Guid.Empty) ? parentJournalId : null;

            journal.PostingPeriodId = postingPeriodId;

            journal.BranchId = branchId;

            journal.AlternateChannelLogId = (alternateChannelLogId != null && alternateChannelLogId != Guid.Empty) ? alternateChannelLogId : null;

            journal.TotalValue = totalValue;

            journal.ModuleNavigationItemCode = moduleNavigationItemCode;

            journal.Reference = reference.ToTitleCase();

            journal.PrimaryDescription = primaryDescription.ToTitleCase();

            journal.SecondaryDescription = secondaryDescription.ToTitleCase();

            journal.ApplicationUserName = serviceHeader.ApplicationUserName;

            journal.EnvironmentUserName = serviceHeader.EnvironmentUserName;

            journal.EnvironmentMachineName = serviceHeader.EnvironmentMachineName;

            journal.EnvironmentDomainName = serviceHeader.EnvironmentDomainName;

            journal.EnvironmentOSVersion = serviceHeader.EnvironmentOSVersion;

            journal.EnvironmentMACAddress = serviceHeader.EnvironmentMACAddress;

            journal.EnvironmentMotherboardSerialNumber = serviceHeader.EnvironmentMotherboardSerialNumber;

            journal.EnvironmentProcessorId = serviceHeader.EnvironmentProcessorId;

            journal.EnvironmentIPAddress = serviceHeader.EnvironmentIPAddress;

            journal.TransactionCode = (short)transactionCode;

            journal.CreatedBy = serviceHeader.ApplicationUserName;

            journal.CreatedDate = DateTime.Now;

            journal.ValueDate = valueDate ?? journal.CreatedDate;

            journal.SuppressAccountAlert = suppressAccountAlert;

            journal.GenerateNewCustomIdentity();

            return journal;
        }
    }
}
