using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.RefereeAgg
{
    public class Referee : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid WitnessId { get; set; }

        public virtual Customer Witness { get; private set; }

        public string Remarks { get; set; }
    }
}
