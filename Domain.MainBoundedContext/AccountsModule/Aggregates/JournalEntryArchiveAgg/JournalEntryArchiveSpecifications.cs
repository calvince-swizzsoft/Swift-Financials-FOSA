using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryArchiveAgg
{
    public static class JournalEntryArchiveSpecifications
    {
        public static Specification<JournalEntryArchive> DefaultSpec()
        {
            Specification<JournalEntryArchive> specification = new TrueSpecification<JournalEntryArchive>();

            return specification;
        }
    }
}
