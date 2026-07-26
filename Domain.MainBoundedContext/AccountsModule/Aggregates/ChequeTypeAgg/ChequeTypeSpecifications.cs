using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg
{
    public static class ChequeTypeSpecifications
    {
        public static Specification<ChequeType> DefaultSpec()
        {
            Specification<ChequeType> specification = new TrueSpecification<ChequeType>();

            return specification;
        }

        public static Specification<ChequeType> ChequeTypeFullText(string text)
        {
            Specification<ChequeType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<ChequeType>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
