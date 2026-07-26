using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg
{
    public static class PostingPeriodSpecifications
    {
        public static Specification<PostingPeriod> DefaultSpec()
        {
            Specification<PostingPeriod> specification = new TrueSpecification<PostingPeriod>();

            return specification;
        }

        public static Specification<PostingPeriod> CurrentPostingPeriod()
        {
            return new DirectSpecification<PostingPeriod>(x => !x.IsClosed && !x.IsLocked && x.IsActive);
        }

        public static Specification<PostingPeriod> PostingPeriodFullText(string text)
        {
            Specification<PostingPeriod> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<PostingPeriod>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
