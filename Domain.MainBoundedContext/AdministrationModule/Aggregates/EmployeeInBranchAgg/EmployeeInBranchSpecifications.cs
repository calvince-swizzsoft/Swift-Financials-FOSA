using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.EmployeeInBranchAgg
{
    public static class EmployeeInBranchSpecifications
    {
        public static Specification<EmployeeInBranch> Employee(Guid employeeId)
        {
            Specification<EmployeeInBranch> specification =
                new DirectSpecification<EmployeeInBranch>(m => m.EmployeeId == employeeId);

            return specification;
        }

        public static Specification<EmployeeInBranch> EmployeeAndBranch(Guid employeeId, Guid branchId)
        {
            Specification<EmployeeInBranch> specification =
                new DirectSpecification<EmployeeInBranch>(m => m.EmployeeId == employeeId && m.BranchId == branchId);

            return specification;
        }
    }
}
