using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg
{
    public static class LevySplitSpecifications
    {
        public static Specification<LevySplit> DefaultSpec()
        {
            Specification<LevySplit> specification = new TrueSpecification<LevySplit>();

            return specification;
        }

        public static ISpecification<LevySplit> LevySplitWithLevyId(Guid levyId)
        {
            Specification<LevySplit> specification = new TrueSpecification<LevySplit>();

            if (levyId != null && levyId != Guid.Empty)
            {
                specification &= new DirectSpecification<LevySplit>(x => x.LevyId == levyId);
            }

            return specification;
        }
    }
}
