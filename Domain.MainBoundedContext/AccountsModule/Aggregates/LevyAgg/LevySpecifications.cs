using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg
{
    public static class LevySpecifications
    {
        public static Specification<Levy> DefaultSpec()
        {
            Specification<Levy> specification = new TrueSpecification<Levy>();

            return specification;
        }

        public static Specification<Levy> LevyFullText(string text)
        {
            Specification<Levy> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Levy>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
