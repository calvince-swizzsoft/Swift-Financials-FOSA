using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalArchiveAgg
{
    public static class JournalArchiveFactory
    {
        public static JournalArchive CreateJournalArchive(Guid? parentJournalArchiveId, Guid postingPeriodId, Guid branchId, Guid? journalVoucherId, Guid? fiscalCountId, Guid? alternateChannelLogArchiveId, Guid? generalLedgerId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, string systemTraceAuditNumber, bool isLocked, ServiceHeader serviceHeader)
        {
            var journalArchive = new JournalArchive();

            journalArchive.GenerateNewIdentity();

            journalArchive.ParentId = (parentJournalArchiveId != null && parentJournalArchiveId != Guid.Empty) ? parentJournalArchiveId : null;

            journalArchive.PostingPeriodId = postingPeriodId;

            journalArchive.BranchId = branchId;

            journalArchive.JournalVoucherId = (journalVoucherId != null && journalVoucherId != Guid.Empty) ? journalVoucherId : null;

            journalArchive.FiscalCountId = (fiscalCountId != null && fiscalCountId != Guid.Empty) ? fiscalCountId : null;

            journalArchive.AlternateChannelLogArchiveId = (alternateChannelLogArchiveId != null && alternateChannelLogArchiveId != Guid.Empty) ? alternateChannelLogArchiveId : null;

            journalArchive.GeneralLedgerId = (generalLedgerId != null && generalLedgerId != Guid.Empty) ? generalLedgerId : null;

            journalArchive.TotalValue = totalValue;

            journalArchive.ModuleNavigationItemCode = moduleNavigationItemCode;

            journalArchive.Reference = reference.ToTitleCase();

            journalArchive.PrimaryDescription = primaryDescription.ToTitleCase();

            journalArchive.SecondaryDescription = secondaryDescription.ToTitleCase();

            journalArchive.ApplicationUserName = serviceHeader.ApplicationUserName;

            journalArchive.EnvironmentUserName = serviceHeader.EnvironmentUserName;

            journalArchive.EnvironmentMachineName = serviceHeader.EnvironmentMachineName;

            journalArchive.EnvironmentDomainName = serviceHeader.EnvironmentDomainName;

            journalArchive.EnvironmentOSVersion = serviceHeader.EnvironmentOSVersion;

            journalArchive.EnvironmentMACAddress = serviceHeader.EnvironmentMACAddress;

            journalArchive.EnvironmentMotherboardSerialNumber = serviceHeader.EnvironmentMotherboardSerialNumber;

            journalArchive.EnvironmentProcessorId = serviceHeader.EnvironmentProcessorId;

            journalArchive.TransactionCode = (short)transactionCode;

            journalArchive.SystemTraceAuditNumber = systemTraceAuditNumber;

            journalArchive.IsLocked = isLocked;

            journalArchive.CreatedDate = DateTime.Now;

            journalArchive.ValueDate = valueDate ?? journalArchive.CreatedDate;

            return journalArchive;
        }
    }
}
