using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeePasswordHistoryAgg
{
    public static class EmployeePasswordHistoryFactory
    {
        public static EmployeePasswordHistory CreateEmployeePasswordHistory(Guid employeeId, string passwordHash)
        {
            var employeePasswordHistory = new EmployeePasswordHistory();

            employeePasswordHistory.GenerateNewIdentity();

            employeePasswordHistory.EmployeeId = employeeId;

            employeePasswordHistory.PasswordHash = passwordHash;

            employeePasswordHistory.CreatedDate = DateTime.Now;

            return employeePasswordHistory;
        }
    }
}
