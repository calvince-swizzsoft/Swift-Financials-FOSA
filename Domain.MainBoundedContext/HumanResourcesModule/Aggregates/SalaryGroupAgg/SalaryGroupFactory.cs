using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg
{
    public static class SalaryGroupFactory
    {
        public static SalaryGroup CreateSalaryGroup(string description)
        {
            var salaryGroup = new SalaryGroup();

            salaryGroup.GenerateNewIdentity();

            salaryGroup.Description = description;

            salaryGroup.CreatedDate = DateTime.Now;

            return salaryGroup;
        }
    }
}
