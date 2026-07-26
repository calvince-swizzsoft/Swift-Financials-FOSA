using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.RefereeAgg
{
    public static class RefereeSpecifications
    {
        public static Specification<Referee> DefaultSpec()
        {
            Specification<Referee> specification = new TrueSpecification<Referee>();

            return specification;
        }

        public static ISpecification<Referee> RefereeWithCustomerId(Guid customerId)
        {
            Specification<Referee> specification = new TrueSpecification<Referee>();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<Referee>(x => x.CustomerId == customerId);
            }

            return specification;
        }
    }
}
