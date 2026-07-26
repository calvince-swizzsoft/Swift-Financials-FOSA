using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalArchiveAgg
{
    public class JournalArchiveSpecifications
    {
        public static Specification<JournalArchive> DefaultSpec()
        {
            Specification<JournalArchive> specification = new TrueSpecification<JournalArchive>();

            return specification;
        }
    }
}
