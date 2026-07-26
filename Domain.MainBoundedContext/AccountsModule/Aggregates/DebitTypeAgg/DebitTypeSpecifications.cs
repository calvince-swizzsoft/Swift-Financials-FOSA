using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg
{
    public static class DebitTypeSpecifications
    {
        public static Specification<DebitType> DefaultSpec()
        {
            Specification<DebitType> specification = new TrueSpecification<DebitType>();

            return specification;
        }

        public static Specification<DebitType> DebitTypeFullText(string text)
        {
            Specification<DebitType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<DebitType>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<DebitType> MandatoryDebitTypes(bool isMandatory)
        {
            Specification<DebitType> specification  = new DirectSpecification<DebitType>(c => c.IsMandatory==isMandatory);

            return specification;
        }

        public static ISpecification<DebitType> DebitTypeDescription(string Description)
        {
            Specification<DebitType> specification = new TrueSpecification<DebitType>();

            specification &= new DirectSpecification<DebitType>(c => c.Description == Description);

            return specification;
        }
    }
}
