using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg
{
    public class EducationRegister : Domain.Seedwork.Entity
    {
        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public Guid EducationVenueId { get; set; }

        public virtual EducationVenue EducationVenue { get; private set; }

        public string Description { get; set; }

        public virtual Duration Duration { get; set; }

        public string Remarks { get; set; }

        

        
    }
}
