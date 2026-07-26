using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg
{
    public static class CreditTypeSpecifications
    {
        public static Specification<CreditType> DefaultSpec()
        {
            Specification<CreditType> specification = new TrueSpecification<CreditType>();

            return specification;
        }

        public static Specification<CreditType> CreditTypeFullText(string text)
        {
            Specification<CreditType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<CreditType>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
