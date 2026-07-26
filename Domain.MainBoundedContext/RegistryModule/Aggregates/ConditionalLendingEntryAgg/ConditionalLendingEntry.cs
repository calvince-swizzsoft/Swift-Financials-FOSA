using Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingEntryAgg
{
    public class ConditionalLendingEntry : Entity
    {
        public Guid ConditionalLendingId { get; set; }

        public virtual ConditionalLending ConditionalLending { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public string Remarks { get; set; }

        

        
    }
}
