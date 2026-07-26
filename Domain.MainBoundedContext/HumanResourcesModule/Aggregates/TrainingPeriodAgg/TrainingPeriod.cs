using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TrainingPeriodAgg
{
    public class TrainingPeriod : Entity
    {
        public string Description { get; set; }

        public string Venue { get; set; }

        public virtual Duration Duration { get; set; }

        public decimal TotalValue { get; set; }

        public string DocumentNumber { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }

        public string Remarks { get; set; }


        HashSet<TrainingPeriodEntry> _trainingPeriodEntries;
        public virtual ICollection<TrainingPeriodEntry> TrainingPeriodEntries
        {
            get
            {
                if (_trainingPeriodEntries == null)
                {
                    _trainingPeriodEntries = new HashSet<TrainingPeriodEntry>();
                }
                return _trainingPeriodEntries;
            }
            private set
            {
                _trainingPeriodEntries = new HashSet<TrainingPeriodEntry>(value);
            }
        }
    }
}
