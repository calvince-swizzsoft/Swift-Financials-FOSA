using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg
{
    public static class NextOfKinSpecifications
    {
        public static Specification<NextOfKin> DefaultSpec()
        {
            Specification<NextOfKin> specification = new TrueSpecification<NextOfKin>();

            return specification;
        }

        public static ISpecification<NextOfKin> NextOfKinWithCustomerId(Guid customerId)
        {
            Specification<NextOfKin> specification = new TrueSpecification<NextOfKin>();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<NextOfKin>(x => x.CustomerId == customerId);
            }

            return specification;
        }
    }
}
