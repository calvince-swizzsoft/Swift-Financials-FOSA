using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.HolidayAgg
{
    public static class HolidayFactory
    {
        public static Holiday CreateHoliday(Guid postingPeriodId, string description, Duration duration)
        {
            var holiday = new Holiday();

            holiday.GenerateNewIdentity();

            holiday.PostingPeriodId = postingPeriodId;

            holiday.Description = description;

            holiday.Duration = duration;

            holiday.CreatedDate = DateTime.Now;

            return holiday;
        }
    }
}
