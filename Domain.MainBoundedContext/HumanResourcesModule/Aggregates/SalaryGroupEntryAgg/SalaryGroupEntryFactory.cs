using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg
{
    public static class SalaryGroupEntryFactory
    {
        public static SalaryGroupEntry CreateSalaryGroupEntry(Guid salaryGroupId, Guid salaryHeadId, Charge charge, decimal minimumValue, int roundingType)
        {
            var salaryGroupEntry = new SalaryGroupEntry();

            salaryGroupEntry.GenerateNewIdentity();

            salaryGroupEntry.SalaryGroupId = salaryGroupId;

            salaryGroupEntry.SalaryHeadId = salaryHeadId;

            salaryGroupEntry.Charge = charge;

            salaryGroupEntry.MinimumValue = minimumValue;

            salaryGroupEntry.RoundingType = (byte)roundingType;

            salaryGroupEntry.CreatedDate = DateTime.Now;

            return salaryGroupEntry;
        }
    }
}
