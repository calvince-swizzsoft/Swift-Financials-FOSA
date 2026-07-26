using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg
{
    public static class DepartmentFactory
    {
        public static Department CreateDepartment(string description)
        {
            var department = new Department();

            department.GenerateNewIdentity();

            department.Description = description;

            department.CreatedDate = DateTime.Now;

            return department;
        }
    }
}
