using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Schedule : ValueObject<Schedule>
    {
        public short Frequency { get; private set; }

        public DateTime ExpectedRunDate { get; private set; }

        public DateTime ActualRunDate { get; private set; }

        public byte ExecuteAttemptCount { get; private set; }

        public bool ForceExecute { get; private set; }

        public Schedule(int frequency, DateTime expectedRunDate, DateTime actualRunDate, int executeAttemptCount, bool forceExecute)
        {
            this.Frequency = (short)frequency;
            this.ActualRunDate = actualRunDate;
            this.ExpectedRunDate = expectedRunDate;
            this.ExecuteAttemptCount = (byte)executeAttemptCount;
            this.ForceExecute = ForceExecute;
        }

        private Schedule()
        { }
    }
}
