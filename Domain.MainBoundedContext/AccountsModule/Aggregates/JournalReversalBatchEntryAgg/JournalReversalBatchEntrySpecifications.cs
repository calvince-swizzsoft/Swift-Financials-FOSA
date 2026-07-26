using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg
{
    public static class JournalReversalBatchEntrySpecifications
    {
        public static Specification<JournalReversalBatchEntry> DefaultSpec()
        {
            Specification<JournalReversalBatchEntry> specification = new TrueSpecification<JournalReversalBatchEntry>();

            return specification;
        }

        public static Specification<JournalReversalBatchEntry> JournalReversalBatchEntryWithJournalReversalBatchId(Guid journalReversalBatchId, string text)
        {
            Specification<JournalReversalBatchEntry> specification = new DirectSpecification<JournalReversalBatchEntry>(c => c.JournalReversalBatchId == journalReversalBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var referenceSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Remarks.Contains(text));

                var primaryDescriptionSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.PrimaryDescription.Contains(text));
                var secondaryDescriptionSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.SecondaryDescription.Contains(text));
                var jounralReferenceSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.Reference.Contains(text));

                var applicationUserNameSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.ApplicationUserName.Contains(text));
                var environmentUserNameSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.EnvironmentUserName.Contains(text));
                var environmentMachineNameSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.EnvironmentMachineName.Contains(text));
                var environmentOSVersionSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.EnvironmentOSVersion.Contains(text));
                var environmentIPAddressSpec = new DirectSpecification<JournalReversalBatchEntry>(c => c.Journal.EnvironmentIPAddress.Contains(text));

                specification &= (referenceSpec | primaryDescriptionSpec | secondaryDescriptionSpec | jounralReferenceSpec | applicationUserNameSpec | environmentUserNameSpec | environmentMachineNameSpec | environmentOSVersionSpec | environmentIPAddressSpec);
            }

            return specification;
        }

        public static Specification<JournalReversalBatchEntry> PostedJournalReversalBatchEntryWithJournalReversalBatchId(Guid journalReversalBatchId)
        {
            Specification<JournalReversalBatchEntry> specification = new DirectSpecification<JournalReversalBatchEntry>(x => x.JournalReversalBatchId == journalReversalBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<JournalReversalBatchEntry> QueableJournalReversalBatchEntries()
        {
            Specification<JournalReversalBatchEntry> specification = new DirectSpecification<JournalReversalBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.JournalReversalBatch.Status == (int)BatchStatus.Posted);

            return specification;
        }
    }
}
