using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg
{
    public static class SalaryCardEntryFactory
    {
        public static SalaryCardEntry CreateSalaryCardEntry(Guid salaryCardId, Guid salaryGroupEntryId, Charge charge)
        {
            var salaryCardEntry = new SalaryCardEntry();

            salaryCardEntry.GenerateNewIdentity();

            salaryCardEntry.SalaryCardId = salaryCardId;

            salaryCardEntry.SalaryGroupEntryId = salaryGroupEntryId;

            salaryCardEntry.Charge = charge;

            salaryCardEntry.CreatedDate = DateTime.Now;

            return salaryCardEntry;
        }
    }
}
