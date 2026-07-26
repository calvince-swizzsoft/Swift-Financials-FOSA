using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.EmployeeInBranchAgg
{
    public static class EmployeeInBranchFactory
    {
        public static EmployeeInBranch CreateEmployeeInBranch(Guid employeeId, Guid branchId)
        {
            var employeeInRole = new EmployeeInBranch();

            employeeInRole.GenerateNewIdentity();

            employeeInRole.EmployeeId = employeeId;

            employeeInRole.BranchId = branchId;

            employeeInRole.CreatedDate = DateTime.Now;

            return employeeInRole;
        }
    }
}
