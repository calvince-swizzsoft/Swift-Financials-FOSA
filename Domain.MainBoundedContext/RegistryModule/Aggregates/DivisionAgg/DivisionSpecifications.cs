using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg
{
    public static class DivisionSpecifications
    {
        public static Specification<Division> DefaultSpec()
        {
            Specification<Division> specification = new TrueSpecification<Division>();

            return specification;
        }

        public static ISpecification<Division> DivisionWithEmployerId(Guid employerId)
        {
            Specification<Division> specification = DefaultSpec();

            if (employerId != null && employerId != Guid.Empty)
            {
                specification &= new DirectSpecification<Division>(x => x.EmployerId == employerId);
            }

            return specification;
        }
    }
}
