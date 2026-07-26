using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Duration : ValueObject<Duration>
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public Duration(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        private Duration()
        {

        }
    }
}
