using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogArchiveAgg
{
    public static class AlternateChannelLogArchiveSpecifications
    {
        public static Specification<AlternateChannelLogArchive> DefaultSpec()
        {
            Specification<AlternateChannelLogArchive> specification = new TrueSpecification<AlternateChannelLogArchive>();

            return specification;
        }
    }
}
