using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class TimeDuration : ValueObject<TimeDuration>
    {
        public TimeSpan StartTime { get; private set; }

        public TimeSpan EndTime { get; private set; }

        public TimeDuration(TimeSpan startTime, TimeSpan endTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        private TimeDuration()
        {

        }
    }
}
