using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg
{
    public static class SalaryHeadFactory
    {
        public static SalaryHead CreateSalaryHead(Guid chartOfAccountId, string description, int type, CustomerAccountType customerAccountType)
        {
            var salaryHead = new SalaryHead();

            salaryHead.GenerateNewIdentity();

            salaryHead.ChartOfAccountId = chartOfAccountId;

            salaryHead.Description = description;

            salaryHead.Type = type;

            salaryHead.CustomerAccountType = customerAccountType;

            salaryHead.CreatedDate = DateTime.Now;

            return salaryHead;
        }
    }
}
