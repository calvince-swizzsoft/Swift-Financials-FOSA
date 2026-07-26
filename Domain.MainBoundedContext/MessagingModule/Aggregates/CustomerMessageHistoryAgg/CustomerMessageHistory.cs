using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg
{
    public class CustomerMessageHistory : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public int MessageCategory { get; set; }

        public string Subject { get; set; }

        public string Recipient { get; set; }

        public string Body { get; set; }
    }
}
