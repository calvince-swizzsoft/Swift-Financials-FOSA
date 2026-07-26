using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherEntryAgg
{
    public static class JournalVoucherEntrySpecifications
    {
        public static Specification<JournalVoucherEntry> DefaultSpec()
        {
            Specification<JournalVoucherEntry> specification = new TrueSpecification<JournalVoucherEntry>();

            return specification;
        }

        public static Specification<JournalVoucherEntry> JournalVoucherEntryWithJournalVoucherId(Guid journalVoucherId)
        {
            Specification<JournalVoucherEntry> specification = DefaultSpec();

            if (journalVoucherId != null && journalVoucherId != Guid.Empty)
            {
                var journalVoucherIdSpec = new DirectSpecification<JournalVoucherEntry>(c => c.JournalVoucherId == journalVoucherId);

                specification &= journalVoucherIdSpec;
            }

            return specification;
        }

        public static Specification<JournalVoucherEntry> PostedJournalVoucherEntryWithJournalVoucherId(Guid journalVoucherId)
        {
            Specification<JournalVoucherEntry> specification = new DirectSpecification<JournalVoucherEntry>(x => x.JournalVoucherId == journalVoucherId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }
    }
}
