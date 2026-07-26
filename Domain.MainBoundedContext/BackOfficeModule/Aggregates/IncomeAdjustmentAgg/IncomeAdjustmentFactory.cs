using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.IncomeAdjustmentAgg
{
    public static class IncomeAdjustmentFactory
    {
        public static IncomeAdjustment CreateIncomeAdjustment(string description, int type)
        {
            var incomeAdjustment = new IncomeAdjustment();

            incomeAdjustment.GenerateNewIdentity();

            incomeAdjustment.Description = description;

            incomeAdjustment.Type = type;

            incomeAdjustment.CreatedDate = DateTime.Now;

            return incomeAdjustment;
        }
    }
}
