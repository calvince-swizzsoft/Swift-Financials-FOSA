using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg
{
    public static class DirectDebitSpecifications
    {
        public static Specification<DirectDebit> DefaultSpec()
        {
            Specification<DirectDebit> specification = new TrueSpecification<DirectDebit>();

            return specification;
        }

        public static Specification<DirectDebit> DirectDebitFullText(string text)
        {
            Specification<DirectDebit> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<DirectDebit>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }


        public static ISpecification<DirectDebit> DirectDebitDescription(string Description)
        {
            Specification<DirectDebit> specification = new TrueSpecification<DirectDebit>();

            specification &= new DirectSpecification<DirectDebit>(c => c.Description == Description);

            return specification;
        }
    }
}
