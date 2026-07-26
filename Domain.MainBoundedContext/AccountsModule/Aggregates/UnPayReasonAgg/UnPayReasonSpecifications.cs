using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonAgg
{
    public static class UnPayReasonSpecifications
    {
        public static Specification<UnPayReason> DefaultSpec()
        {
            Specification<UnPayReason> specification = new TrueSpecification<UnPayReason>();

            return specification;
        }

        public static Specification<UnPayReason> UnPayReasonFullText(string text)
        {
            Specification<UnPayReason> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<UnPayReason>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<UnPayReason> UnapayReasonDescription(string Description)
        {
            Specification<UnPayReason> specification = new TrueSpecification<UnPayReason>();

            specification &= new DirectSpecification<UnPayReason>(c => c.Description == Description);

            return specification;
        }
    }
}
