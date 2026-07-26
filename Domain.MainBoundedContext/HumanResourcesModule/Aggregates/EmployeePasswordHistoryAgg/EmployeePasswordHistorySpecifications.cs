using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeePasswordHistoryAgg
{
    public static class EmployeePasswordHistorySpecifications
    {
        public static Specification<EmployeePasswordHistory> DefaultSpec()
        {
            Specification<EmployeePasswordHistory> specification = new TrueSpecification<EmployeePasswordHistory>();

            return specification;
        }

        public static ISpecification<EmployeePasswordHistory> EmployeePasswordHistoryWithEmployeeId(Guid employeeId)
        {
            Specification<EmployeePasswordHistory> specification = DefaultSpec();

            if (employeeId != null && employeeId != Guid.Empty)
            {
                specification &= new DirectSpecification<EmployeePasswordHistory>(x => x.EmployeeId == employeeId);
            }

            return specification;
        }
    }
}
