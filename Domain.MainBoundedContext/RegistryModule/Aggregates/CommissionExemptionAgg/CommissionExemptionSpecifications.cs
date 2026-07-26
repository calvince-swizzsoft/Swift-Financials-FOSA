using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg
{
    public static class CommissionExemptionSpecifications
    {
        public static Specification<CommissionExemption> DefaultSpec()
        {
            Specification<CommissionExemption> specification = new TrueSpecification<CommissionExemption>();

            return specification;
        }

        public static Specification<CommissionExemption> CommissionExemptionFullText(string text)
        {
            Specification<CommissionExemption> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<CommissionExemption>(c => c.Description.Contains(text));
                var commissionDescriptionSpec = new DirectSpecification<CommissionExemption>(c => c.Commission.Description.Contains(text));

                specification &= (descriptionSpec | commissionDescriptionSpec);
            }

            return specification;
        }

        public static ISpecification<CommissionExemption> CommissionExemptionWithCommissionId(Guid commissionId)
        {
            Specification<CommissionExemption> specification = new TrueSpecification<CommissionExemption>();

            if (commissionId != null && commissionId != Guid.Empty)
            {
                specification &= new DirectSpecification<CommissionExemption>(x => x.CommissionId == commissionId);
            }

            return specification;
        }
    }
}
