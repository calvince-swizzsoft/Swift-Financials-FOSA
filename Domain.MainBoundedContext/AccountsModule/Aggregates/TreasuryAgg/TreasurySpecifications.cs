using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.TreasuryAgg
{
    public class TreasurySpecifications
    {
        public static Specification<Treasury> DefaultSpec()
        {
            Specification<Treasury> specification = new TrueSpecification<Treasury>();

            return specification;
        }

        public static ISpecification<Treasury> TreasuryWithBranchId(Guid branchId)
        {
            Specification<Treasury> specification = new DirectSpecification<Treasury>(x => x.BranchId == branchId);

            return specification;
        }

        public static Specification<Treasury> TreasuryFullText(string text)
        {
            Specification<Treasury> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Treasury>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }


        public static ISpecification<Treasury> DescriptionWithDescription(string Description)
        {
            Specification<Treasury> specification = new TrueSpecification<Treasury>();

            specification &= new DirectSpecification<Treasury>(c => c.Description == Description);

            return specification;
        }
    }
}
